using System;
using System.Collections.Generic;
using System.Text;

namespace Jammit.Model
{
  public class ScoreInfo
  {
    public ScoreInfo(NotatedTrackInfo track, string type)
    {
      Track = track;

      switch (type)
      {
        case "Score":
          PageCount = track.NotationPages;
          break;
        case "Tablature":
          PageCount = track.TablaturePages;
          break;

        default:
          throw new Exception($"Unrecognized score type [{type}]");
      }

      Type = type;
    }

    public NotatedTrackInfo Track { get; private set; }

    public string Type { get; private set; }

    public uint PageCount { get; private set; }

    public override string ToString() => $"{Track.Title} - {Type}";
  }
}
