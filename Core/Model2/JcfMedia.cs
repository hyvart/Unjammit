using System;
using System.Collections.Generic;
using System.Text;

namespace Jammit.Model2
{
  public class JcfMedia
  {
    public JcfMedia()
    {
      NotatedTracks = new List<Model.NotatedTrackInfo>();
      BackingTracks = new List<Model.FileTrackInfo>();
    }

    public IList<Model.NotatedTrackInfo> NotatedTracks { get; private set; }

    public IList<Model.FileTrackInfo> BackingTracks { get; private set; }

    public Model.TrackInfo ClickTrack { get; set; }

    //TODO: EmptyTrack, InputTrack

    //TODO: Beat

    //TODO: Section

    //TODO: WaveForm?

    //TODO: NotationData
  }
}
