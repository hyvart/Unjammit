using System;

namespace Jammit.Audio
{
  public class TrackState
  {
    public enum AudioStatus
    {
      On,
      Muted,      // Black-listed off
      Solo,       // White-listed on
      Excluded,   // Muted by exclusion of Solo white list
      AutoMuted   // i.e. click track countdown
    }

    public AudioStatus Status = AudioStatus.On;

    public uint Volume = 100;
  }
}
