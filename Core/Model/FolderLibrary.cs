using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Jammit.Model
{
  public class FolderLibrary : ILibrary
  {
    #region private members

    private const string LibraryFileName = "library.xml";

    private string _storagePath;
    private string _libraryPath;
    private IDictionary<Guid, SongInfo> _cache;
    protected Client.IClient _client;

    private void InitCache()
    {
      _cache = new Dictionary<Guid, SongInfo>();

      using (var stream = File.OpenRead(_libraryPath))
      {
        var doc = XDocument.Load(stream);
        foreach (var xe in doc.Element("songs").Elements())
        {
          var song = FromXml(xe);
          _cache[song.Id] = song;
        }
      }

      Songs = _cache.Values.ToList();
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
      _libraryPath = Path.Combine(_storagePath, "library.xml");
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

    #region ILibrary members

    public void AddSong(SongInfo song)
    {
      if (!/*Forms.Settings.SkipDownload*/ false)
      {
        // Download th efile.
        var downloadTask = Task.Run(async () => await _client.DownloadSong(song.Id));
        downloadTask.Wait();

        // Make sure Tracks and Downloads dirs exist.
        var downloadsDir = Directory.CreateDirectory(Path.Combine(_storagePath, "Downloads"));
        var tracksDir = Directory.CreateDirectory(Path.Combine(_storagePath, "Tracks"));

        // Extract downloaded ZIP contents.
        var zipPath = Path.Combine(downloadsDir.FullName, $"{song.Id}.zip");
        using (var zipStream = File.Create(zipPath))
        {
          downloadTask.Result.CopyTo(zipStream);
        }
        ZipFile.ExtractToDirectory(zipPath, tracksDir.FullName);

        _cache[song.Id] = song;
        Songs = _cache.Values.ToList();

        Save();

        // Cleanup
        if (!/*Forms.Settings.SkipDownload*/ false)
          File.Delete(zipPath);
      }
      else
      {
        _cache[song.Id] = song;
        Songs = _cache.Values.ToList();

        Save();
      }
    }

    public List<SongInfo> GetSongs()
    {
      return _cache.Values.ToList();
    }

    public List<SongInfo> Songs { get; set; }

    public void RemoveSong(Guid id)
    {
      try
      {
        var songPath = Path.Combine(_storagePath, "Tracks", $"{id.ToString().ToUpper()}.jcf");
        Directory.Delete(songPath, true);
      }
      catch (Exception)
      {
        //TODO: Handle exception.
      }

      _cache.Remove(id);
      Songs = _cache.Values.ToList();

      Save();
    }

    #endregion // ILibrary members
  }
}
