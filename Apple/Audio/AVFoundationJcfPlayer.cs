using System;
using System.Collections.Generic;
using System.Text;

namespace Jammit.Audio
{
  public class AVFoundationJcfPlayer : IJcfPlayer
  {
    #region IJcfPlayer members

    public void Play()
    {

    }

    public void Pause()
    {

    }

    public void Stop()
    {

    }

    public uint GetVolume(Model.PlayableTrackInfo track)
    {
      throw new NotImplementedException();
    }

    public void SetVolume(Model.PlayableTrackInfo track, uint volume)
    {
      throw new NotImplementedException();
    }

    public TimeSpan Position { get; set; }

    public TimeSpan Length { get; set; }

    public PlaybackStatus State { get; private set; }

    #endregion // IJcfPlayer members
  }
}
