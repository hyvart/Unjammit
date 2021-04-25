using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
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
        Sku = xe.Attribute("sku").Value,
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

    #endregion  private members

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
        new XElement("sku", song.Sku),
        new XElement("artist", song.Artist),
        new XElement("album", song.Album),
        new XElement("title", song.Title),
        new XElement("instrument", song.Instrument),
        new XElement("genre", song.Genre));
    }

    #region INotifyPropertyChanged members

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion  INotifyPropertyChanged members

    #region ILibrary members

    public SongInfo AddSong(Stream contentStream)
    {
      using (var archive = new ZipArchive(contentStream, ZipArchiveMode.Read))
      {
        //TODO: Throw explicitly if non-compliant.

        var infoEntry = archive.Entries.First(e => e.Name == "info.plist");
        var dict = Claunia.PropertyList.PropertyListParser.Parse(infoEntry.Open()) as Claunia.PropertyList.NSDictionary;
        var song = new SongInfo()
        {
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

        var songDir = Directory.CreateDirectory(Path.Combine(_storagePath, "Tracks", $"{song.Sku}.jcf"));

        // Delete target directory if exists, then add again.
        if (_cache.ContainsKey(song))
        {
          _cache.Remove(song);
          songDir.GetDirectories(song.Sku + "*", SearchOption.TopDirectoryOnly).All(d =>
          {
            d.Delete(true); return true;
          });
        }

        var entryPath = infoEntry.FullName.Remove(infoEntry.FullName.IndexOf("info.plist"));
        foreach (var entry in archive.Entries.Where(e => e.FullName.StartsWith(entryPath)))
        {
          entry.ExtractToFile(Path.Combine(songDir.FullName, entry.Name));
        }

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
        var songPath = Path.Combine(_storagePath, "Tracks", $"{song.Sku}.jcf");
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

    #endregion  ILibrary members
  }
}
