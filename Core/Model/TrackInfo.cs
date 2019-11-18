using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jammit.Model
{
  public abstract class TrackInfo : IComparable<TrackInfo>
  {
    #region Properties

    public virtual string Class { get; set; }

    public Guid Identifier { get; set; }

    #endregion

    #region IComparable

    public int CompareTo(TrackInfo other)
    {
      return Identifier.CompareTo(other.Identifier);
    }

    #endregion IComparable
  }

  /// <summary>
  /// JMEmptyTrack
  /// </summary>
  public class EmptyTrackInfo : TrackInfo
  {
    #region Properties

    public override string Class => "JMEmptyTrack";

    #endregion
  }

  /// <summary>
  /// JMClickTrack, JMInputTrack
  /// </summary>
  public class PlayableTrackInfo : TrackInfo
  {
    #region Properties

    public string Title { get; set; }

    public override string ToString()
    {
      return $"[{Class}] - [{Title}]";
    }

    #endregion
  }

  /// <summary>
  /// JMFileTrack 
  /// </summary>
  public class FileTrackInfo : PlayableTrackInfo
  {
    public override string Class => "JMFileTrack";

    public uint ScoreSystemHeight { get; set; }

    public uint ScoreSystemInterval { get; set; }
  }

  public class NotatedTrackInfo : FileTrackInfo
  {
    public NotatedTrackInfo(FileTrackInfo source)
    {
      this.Identifier = source.Identifier;
      this.Title = source.Title;
      this.ScoreSystemHeight = source.ScoreSystemHeight;
      this.ScoreSystemInterval = source.ScoreSystemInterval;
    }

    public uint NotationPages { get; set; }

    public uint TablaturePages { get; set; }
  }
}
