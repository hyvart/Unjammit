using System;
using System.Collections.Generic;
using System.Text;

namespace Jammit.Model2
{
  public class JcfMedia
  {
    public JcfMedia(string path)
    {
      Path = path;
      NotatedTracks = new List<Model.NotatedTrackInfo>();
      BackingTracks = new List<Model.FileTrackInfo>();
      Scores = new List<ScoreInfo>();
    }

    public string Path { get; private set; }

    public IList<Model.NotatedTrackInfo> NotatedTracks { get; private set; }

    public IList<Model.FileTrackInfo> BackingTracks { get; private set; }

    public Model.TrackInfo ClickTrack { get; set; }

    //TODO: EmptyTrack, InputTrack

    //TODO: Beat

    //TODO: Section

    //TODO: WaveForm?

    public IList<ScoreInfo> Scores { get; private set; }
  }
}
