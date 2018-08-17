using System;
using System.Collections.Generic;
using System.Text;

namespace Jammit.Model
{
  public class ScoreInfo
  {
    public Model.NotatedTrackInfo Track;
    public string Type;

    //TODO: Leave in NotatdTrackInfo?
    public uint PageCount;

    public override string ToString() => $"{Track.Title} - {Type}";
  }
}
