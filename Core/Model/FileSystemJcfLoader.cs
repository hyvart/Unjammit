﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Claunia.PropertyList;

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

    public JcfMedia LoadMedia(SongInfo song)
    {
      string songPath = Path.Combine(dataDirectory, "Tracks", $"{song.Sku}" + ".jcf");

      var result = new JcfMedia(song, songPath);

      // Load length
      var beatArray = PropertyListParser.Parse(Path.Combine(songPath, "beats.plist")) as NSArray;
      var dictionary = beatArray[beatArray.Count - 1] as NSDictionary;
      var position = dictionary.Double("position") ?? 0;
      result.Length = TimeSpan.FromSeconds(position);

      LoadTracks(result, songPath);

      LoadBeats(result, songPath);

      LoadSections(result, songPath);

      LoadScoreNodes(result, songPath);

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

    public void LoadFullSongInfo(SongInfo song, string songPath)
    {
      var infoDict = PropertyListParser.Parse(Path.Combine(songPath, "info.plist")) as NSDictionary;
      song.Tempo = infoDict.String("bpm");
      song.WrittenBy = infoDict.String("writtenBy");
      song.PublishedBy = infoDict.String("publishedBy");
      song.CourtesyOf = infoDict.String("courtesyOf");

      if (!File.Exists(Path.Combine(songPath, "tuning.plist")))
        return;

      var tuningDict = PropertyListParser.Parse(Path.Combine(songPath, "tuning.plist")) as NSDictionary;
      var notesDict = tuningDict["notes"] as NSDictionary;
      song.Tunings = new List<string>();
      foreach(var instrumentKey in notesDict.Keys)
      {
        var instrumentDict = notesDict[instrumentKey] as NSDictionary;
        var tunings = instrumentDict.Where((p) => p.Value is NSArray);

        foreach (var tuning in tunings)
        {
          string songTuning = string.Empty;
          foreach (var tuningEntry in tuning.Value as NSArray)
          {
            songTuning += tuningEntry.ToString() + " ";
          }
          song.Tunings.Add(songTuning);
        }
      }
    }

    #endregion  IJcfLoader members

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

              media.Scores.Add(new ScoreInfo(notated, "Score"));
              if (tablaturePages > 0)
              {
                media.Scores.Add(new ScoreInfo(notated, "Tablature"));
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

    private void LoadBeats(JcfMedia media, string songPath)
    {
      var beatArray = PropertyListParser.Parse(Path.Combine(songPath, "beats.plist")) as NSArray;
      var ghostArray = PropertyListParser.Parse(Path.Combine(songPath, "ghost.plist")) as NSArray;

      var beats = new List<Beat>(beatArray.Count);
      for (var i = 0; i < beatArray.Count; i++)
      {
        var beastDict = beatArray[i] as NSDictionary;
        var isDownbeat = beastDict.Bool("isDownbeat") ?? false;
        var isGhostBeat = (ghostArray[i] as NSDictionary).Bool("isGhostBeat") ?? false;
        beats.Add(new Beat(beastDict.Double("position") ?? 0, isDownbeat, isGhostBeat));
      }

      media.Beats = beats;
    }

    private void LoadSections(JcfMedia media, string songPath)
    {
      var sectionArray = PropertyListParser.Parse(Path.Combine(songPath, "sections.plist")) as NSArray;
      media.Sections = sectionArray.OfType<NSDictionary>().Select(dict => new Section
      {
        BeatIdx = dict.Int("beat") ?? int.Parse(dict.String("beat")),    // Key can sometimes be string instead of integer.
        Beat = media.Beats[dict.Int("beat") ?? int.Parse(dict.String("beat"))],
        Number = dict.Int("number") ?? int.Parse(dict.String("number")), // Key can sometimes be string instead of integer.
        Type = dict.Int("type").Value
      }).ToList();
    }

    /// <summary>
    /// Loads score nodes for each instrument track.
    /// </summary>
    /// <param name="media">Must already have InstrumentTracks set.</param>
    /// <param name="songPath"></param>
    private void LoadScoreNodes(JcfMedia media, string songPath)
    {
      using (var stream = File.OpenRead(Path.Combine(songPath, "nowline.nodes")))
      {
        var numTracks = stream.ReadInt32LE();
        var temp = new byte[32];
        for (var i = 0; i < numTracks; i++)
        {
          stream.Read(temp, 0, 32);
          var title = Encoding.UTF8.GetString(temp, 0, 32);
          title = title.Remove(title.IndexOf('\0'));

          stream.Read(temp, 0, 32);
          var type = Encoding.UTF8.GetString(temp, 0, 32);
          type = type.Remove(type.IndexOf('\0'));

          var nodes = BeatInfo.FromStream(stream);
          var index = media.InstrumentTracks.FirstOrDefault(track => track.Title == title);
          media.ScoreNodes[index] = new ScoreNodes(title, type, nodes);
        }
      }
    }
  }
}
