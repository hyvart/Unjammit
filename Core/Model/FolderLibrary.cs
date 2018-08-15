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
      _cache = new SortedList<SongInfo, SongInfo>(
        Comparer<SongInfo>.Create((s1, s2) => s1.Artist.CompareTo(s2.Artist) * 10 + s1.Title.CompareTo(s2.Title))
      );

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
        new XAttribute("id", song.Id.ToString().ToUpper()),
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
        // Download th efile.
        //var downloadTask = Task.Run(async () => await _client.DownloadSong(song.Id));
        //downloadTask.Wait();
        var download = await _client.DownloadSong(song.Id);

        // Make sure Tracks and Downloads dirs exist.
        var downloadsDir = Directory.CreateDirectory(Path.Combine(_storagePath, "Downloads"));
        var tracksDir = Directory.CreateDirectory(Path.Combine(_storagePath, "Tracks"));

        // Extract downloaded ZIP contents.
        var zipPath = Path.Combine(downloadsDir.FullName, $"{song.Id}.zip");
        using (var zipStream = File.Create(zipPath))
        {
          //downloadTask.Result.CopyTo(zipStream);
          download.CopyTo(zipStream);
        }
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
