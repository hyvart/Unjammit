using System;
using System.Collections.Generic;
using System.Text;

namespace Jammit.Model
{
  public class JcfMedia : IComparable<JcfMedia>
  {
    public JcfMedia(SongInfo song, string path)
    {
      Song = song;
      Path = path;
      InstrumentTracks = new List<NotatedTrackInfo>();
      Scores = new List<ScoreInfo>();
      ScoreNodes = new SortedList<NotatedTrackInfo, ScoreNodes>();
    }

    public SongInfo Song { get; private set; }

    public string Path { get; private set; }

    public IList<NotatedTrackInfo> InstrumentTracks { get; }

    public FileTrackInfo BackingTrack { get; set; }

    public PlayableTrackInfo ClickTrack { get; set; }

    public TimeSpan Length { get; set; }

    //TODO: EmptyTrack, InputTrack

    public IReadOnlyList<Beat> Beats { get; set; }

    //TODO: Section
    public IReadOnlyList<Section> Sections { get; set; }

    public IDictionary<NotatedTrackInfo, ScoreNodes> ScoreNodes { get; set; }

    //TODO: WaveForm?

    public IList<ScoreInfo> Scores { get; private set; }

    #region IComparable members

    public int CompareTo(JcfMedia other)
    {
      return Song.Sku.CompareTo(other.Song.Sku);
    }

    #endregion
  }
}
