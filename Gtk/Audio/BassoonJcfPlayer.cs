using System;

using Bassoon;
using Jammit.Model;

namespace Jammit.Audio
{
  public class BassoonJcfPlayer : IJcfPlayer
  {
    public BassoonJcfPlayer(Model.JcfMedia media)
    {
      using (BassoonEngine be = new BassoonEngine())
      {
        var sound = new Sound(media.Path);
      }
    }

    #region IJcfPlayer

    public TimeSpan Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public TimeSpan Length => throw new NotImplementedException();

    public PlaybackStatus State => throw new NotImplementedException();

    public event EventHandler PositionChanged;

    public uint GetVolume(PlayableTrackInfo track)
    {
      throw new NotImplementedException();
    }

    public void Pause()
    {
      throw new NotImplementedException();
    }

    public void Play()
    {
      throw new NotImplementedException();
    }

    public void SetVolume(PlayableTrackInfo track, uint volume)
    {
      throw new NotImplementedException();
    }

    public void Stop()
    {
      throw new NotImplementedException();
    }

    #endregion //IJcfPlayer
  }
}
