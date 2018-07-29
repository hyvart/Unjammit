using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Claunia.PropertyList;
using Jammit.Model;

namespace Jammit.Model2
{
  public class FileSystemJcfLoader : IJcfLoader
  {
    private readonly string dataDirectory;

    public FileSystemJcfLoader(string dataDirectory)
    {
      this.dataDirectory = dataDirectory;
    }

    #region IJcfLoader members

    public JcfMedia LoadMedia(Guid guid)
    {
      string tracksPath = Path.Combine(dataDirectory, "Tracks");
      string songPath = Path.Combine(dataDirectory, "Tracks", $"{guid}".ToUpper() + ".jcf");

      var result = new JcfMedia();

      // Load tracks
      LoadTracks(result, songPath);

      return result;
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
              media.NotatedTracks.Add(new NotatedTrackInfo(source)
              {
                NotationPages   = (uint)notationPages,
                TablaturePages  = (uint)tablaturePages
              });
            }
            else
            {
              media.BackingTracks.Add(source);
            }
            break;

          default:
            switch (dict.Count)
            {
              case 2:
                break;//TODO
              case 3:
                media.ClickTrack = new ConcreteTrackInfo()
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
    }
  }
}
