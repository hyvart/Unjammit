using System;
using System.Collections.Generic;
using System.Text;

namespace Jammit.Model
{
  public class JcfMedia : IComparable<JcfMedia>
  {
    public JcfMedia(Model.SongInfo song, string path)
    {
      Song = song;
      Path = path;
      InstrumentTracks = new List<Model.NotatedTrackInfo>();
      Scores = new List<ScoreInfo>();
    }

    public Model.SongInfo Song { get; private set; }

    public string Path { get; private set; }

    public IList<Model.NotatedTrackInfo> InstrumentTracks { get; }

    public Model.FileTrackInfo BackingTrack { get; set; }

    public Model.TrackInfo ClickTrack { get; set; }

    public TimeSpan Length { get; set; }

    //TODO: EmptyTrack, InputTrack

    //TODO: Beat

    //TODO: Section

    //TODO: WaveForm?

    public IList<ScoreInfo> Scores { get; private set; }

    #region IComparable members

    public int CompareTo(JcfMedia other)
    {
      return Song.Id.CompareTo(other.Song.Id);
    }

    #endregion
  }
}
