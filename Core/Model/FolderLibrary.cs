using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Jammit.Model
{
  public class FolderLibrary : ILibrary, INotifyPropertyChanged
  {
    #region private members

    private const string LibraryFileName = "library.xml";

    private string _storagePath;
    private string _libraryPath;
    private IDictionary<SongInfo, SongInfo> _cache;
    private Client.IClient _client;

    private void InitCache()
    {
      // See Client.RestClient.LoadCatalog
      _cache = new SortedList<SongInfo, SongInfo>(
        Comparer<SongInfo>.Create(
          (s1, s2) => string.Compare(s1.Artist,     s2.Artist,      StringComparison.Ordinal) * 100 +
                      string.Compare(s1.Title,      s2.Title,       StringComparison.Ordinal) *  10 +
                      string.Compare(s1.Instrument, s2.Instrument,  StringComparison.Ordinal) *   1
      ));

      using (var stream = File.OpenRead(_libraryPath))
      {
        var doc = XDocument.Load(stream);
        foreach (var xe in doc.Element("songs").Elements())
        {
          var song = FromXml(xe);
          _cache[song] = song;
        }
      }

      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Songs"));
    }

    private SongInfo FromXml(XElement xe)
    {
      return new SongInfo
      {
        Id = Guid.Parse(xe.Attribute("id").Value),
        Sku = xe.Element("sku").Value,
        Artist = xe.Element("artist").Value,
        Album = xe.Element("album").Value,
        Title = xe.Element("title").Value,
        Instrument = xe.Element("instrument").Value,
        Genre = xe.Element("genre").Value
      };
    }

    private void Save()
    {
      File.WriteAllText(_libraryPath, string.Empty);
      using (var stream = File.OpenWrite(_libraryPath))
      {
        //Hack: clear file
        //TODO: Remove???
        var xdoc = new XDocument(new XElement("songs"));
        foreach (var song in _cache.Values)
        {
          xdoc.Element("songs").Add(ToXml(song));
        }

        xdoc.Save(stream);
      }
    }

    #endregion // private members

    public FolderLibrary(string storagePath, Client.IClient client)
    {
      _storagePath = storagePath;
      _libraryPath = Path.Combine(_storagePath, LibraryFileName);
      _client = client;

      // If library doesn't exist, initialize.
      if (!File.Exists(_libraryPath))
        new XDocument(new XElement("songs")).Save(_libraryPath);

      InitCache();
    }

    public XElement ToXml(SongInfo song)
    {
      return new XElement("song",
        new XAttribute("id", song.Id),
        new XElement("sku", song.Sku),
        new XElement("artist", song.Artist),
        new XElement("album", song.Album),
        new XElement("title", song.Title),
        new XElement("instrument", song.Instrument),
        new XElement("genre", song.Genre));
    }

    #region INotifyPropertyChanged members

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion // INotifyPropertyChanged members

    #region ILibrary members

    public async Task AddSong(SongInfo song)
    {
      if (!/*Forms.Settings.SkipDownload*/ false)
      {
        // Make sure Tracks and Downloads dirs exist.
        var downloadsDir = Directory.CreateDirectory(Path.Combine(_storagePath, "Downloads"));
        var tracksDir = Directory.CreateDirectory(Path.Combine(_storagePath, "Tracks"));
        var zipPath = Path.Combine(downloadsDir.FullName, $"{song.Id.ToString().ToUpper()}.zip");

        await _client.DownloadSong(song, zipPath);

        // Extract downloaded ZIP contents.
        ZipFile.ExtractToDirectory(zipPath, tracksDir.FullName);

        _cache[song] = song;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Songs"));

        Save();

        // Cleanup
        if (!/*Forms.Settings.SkipDownload*/ false)
          File.Delete(zipPath);
      }
      else
      {
        _cache[song] = song;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Songs"));

        Save();
      }
    }

    public SongInfo AddSong(Stream contentStream)
    {
      using (var archive = new ZipArchive(contentStream, ZipArchiveMode.Read))
      {
        //TODO: Throw explicitly if non-compliant.

        var jcfEntry = archive.Entries.Where(e => e.FullName.EndsWith(".jcf/")).FirstOrDefault();
        var jcfName = jcfEntry.FullName.Remove(jcfEntry.FullName.IndexOf(".jcf/"));
        Guid id;
        string idString;
        try
        {
          id = Guid.Parse(jcfName);
        }
        catch (Exception)
        {
          id = Guid.NewGuid();

        }
        finally
        {
          idString = id.ToString().ToUpper();
        }

        var infoEntry = archive.Entries.Where(e => e.Name == "info.plist").FirstOrDefault();
        var dict = Claunia.PropertyList.PropertyListParser.Parse(infoEntry.Open()) as Claunia.PropertyList.NSDictionary;

        var song = new SongInfo()
        {
          Id = id,
          Sku = dict.String("sku"),
          Artist = dict.String("artist"),
          Album = dict.String("album"),
          Title = dict.String("title"),
          Genre = dict.String("genre")
        };
        switch (dict.Int("instrument"))
        {
          case 0:
            song.Instrument = "Guitar"; break;
          case 1:
            song.Instrument = "Bass"; break;
          case 2:
            song.Instrument = "Drums"; break;
          case 3:
            song.Instrument = "Keyboard"; break;
          case 4:
            song.Instrument = "Vocals"; break;
        }

        var tracksDir = Directory.CreateDirectory(Path.Combine(_storagePath, "Tracks"));

        // Delete target directory if exists, then add again.
        if (_cache.ContainsKey(song))
        {
          _cache.Remove(song);
          tracksDir.GetDirectories(song.Id.ToString().ToUpper() + "*", SearchOption.TopDirectoryOnly).All(d =>
          {
            d.Delete(true); return true;
          });
        }
        archive.ExtractToDirectory(tracksDir.FullName);

        // Rename the extracted JCF to a GUID, if needed.
        if (idString != jcfName)
          Directory.Move(Path.Combine(tracksDir.FullName, jcfName + ".jcf"), Path.Combine(tracksDir.FullName, idString + ".jcf"));

        _cache[song] = song;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Songs"));
        Save();

        return song;
      }
    }

    public List<SongInfo> Songs
    {
      get
      {
        return _cache.Values.ToList();
      }
    }

    public void RemoveSong(SongInfo song)
    {
      try
      {
        var songPath = Path.Combine(_storagePath, "Tracks", $"{song.Id.ToString().ToUpper()}.jcf");
        Directory.Delete(songPath, true);
      }
      catch (Exception)
      {
        //TODO: Handle exception.
      }

      _cache.Remove(song);
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Songs"));

      Save();
    }

    #endregion // ILibrary members
  }
}
