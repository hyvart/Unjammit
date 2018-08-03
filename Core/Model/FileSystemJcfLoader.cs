using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Claunia.PropertyList;
using Jammit.Model;

namespace Jammit.Model
{
  public class FileSystemJcfLoader : IJcfLoader
  {
    private readonly string dataDirectory;

    public FileSystemJcfLoader(string dataDirectory)
    {
      this.dataDirectory = dataDirectory;
    }

    #region IJcfLoader members

    public JcfMedia LoadMedia(Model.SongInfo song)
    {
      string tracksPath = Path.Combine(dataDirectory, "Tracks");
      string songPath = Path.Combine(dataDirectory, "Tracks", $"{song.Id}".ToUpper() + ".jcf");

      var result = new JcfMedia(song, songPath);

      // Load tracks
      LoadTracks(result, songPath);

      return result;
    }

    public Stream LoadAlbumCover(JcfMedia media)
    {
      return File.OpenRead(Path.Combine(media.Path, "cover.jpg"));
    }

    public Stream LoadNotation(JcfMedia media, ScoreInfo score, uint index)
    {
      var trackId = score.Track.Identifier.ToString().ToUpper();
      var path = Path.Combine(media.Path, trackId) + "_jcf";
      if ("Score" == score.Type)
        path += "n_";
      else if ("Tablature" == score.Type)
        path += "t_";

      path += string.Format("{0:D2}", index);

      return File.OpenRead(path);
    }

    #endregion // IJcfLoader members

    private void LoadTracks(JcfMedia media, string songPath)
    {
      var trackskArray = PropertyListParser.Parse(Path.Combine(songPath, "tracks.plist")) as NSArray;
      foreach (var track in trackskArray)
      {
        var dict = track as NSDictionary;
        if (dict == null)
          continue;

        Guid guid = Guid.Parse(dict.String("identifier"));
        string id = guid.ToString().ToUpper();
        string type = dict.String("class");

        switch(type)
        {
          case "JMEmptyTrack":
            //TODO
            break;

          case "JMFileTrack":
            var source = new FileTrackInfo
            {
              Identifier          = guid,
              Title               = dict.String("title"),
              ScoreSystemHeight   = (uint)dict.Int("scoreSystemHeight"),
              ScoreSystemInterval = (uint)dict.Int("scoreSystemInterval")
            };
            var notationPages   = Directory.GetFiles(songPath, $"{id}_jcfn_??").Length;
            var tablaturePages  = Directory.GetFiles(songPath, $"{id}_jcft_??").Length;
            if (notationPages + tablaturePages > 0)
            {
              var notated = new NotatedTrackInfo(source)
              {
                NotationPages   = (uint)notationPages,
                TablaturePages  = (uint)tablaturePages
              };
              media.InstrumentTracks.Add(notated);

              media.Scores.Add(new ScoreInfo()
              {
                Track = notated,
                Type = "Score"
              });
              if (tablaturePages > 0)
              {
                media.Scores.Add(new ScoreInfo()
                {
                  Track = notated,
                  Type = "Tablature"
                });
              }
            }
            else
            {
              media.BackingTrack = source;
            }
            break;

          default:
            switch (dict.Count)
            {
              case 2:
                break;//TODO
              case 3:
                if ("JMClickTrack" == type)
                  media.ClickTrack = new PlayableTrackInfo()
                  {
                    Class       = type,
                    Identifier  = guid,
                    Title       = dict.String("title")
                  };
                break;

              default:
                throw new Exception("Unrecognized track info.\n" + dict.ToString());
            }
            break;
        }
      }
    } // LoadTracks(JcfMedia, string)
  }
}
