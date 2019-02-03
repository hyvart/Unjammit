using System;

using Android.Media;
using Jammit.Model;

namespace Jammit.Audio
{
  public class AndroidMediaJcfPlayer : IJcfPlayer
  {
    #region private members

    private MediaPlayer _player;

    #endregion // private members

    public AndroidMediaJcfPlayer(JcfMedia media)
    {
      _player = new MediaPlayer();
      var path = media.Path;
      path += media.BackingTrack.Identifier.ToString().ToUpper();

      _player.SetDataSource(path);
      _player.Prepare();
    }

    #region IJcfPlayer

    public event EventHandler PositionChanged;

    public void Play()
    {
      _player.Start();
      State = PlaybackStatus.Playing;
    }

    public void Pause()
    {
      _player.Pause();
      State = PlaybackStatus.Paused;
    }

    public void Stop()
    {
      _player.Stop();
      State = PlaybackStatus.Stopped;
    }

    public uint GetVolume(PlayableTrackInfo track)
    {
      return 50;
    }

    public void SetVolume(PlayableTrackInfo track, uint volume)
    {
      _player.SetVolume((float)volume, (float)volume);
    }

    public TimeSpan Position
    {
      get
      {
        return TimeSpan.FromMilliseconds(_player.CurrentPosition);
      }
      set
      {
        _player.SeekTo((int) value.TotalMilliseconds);
      }
    }

    public TimeSpan Length {
      get
      {
        return TimeSpan.FromMilliseconds(_player.Duration);
      }
    }

    public PlaybackStatus State { get; private set; }

    #endregion //IJcfPlayer
  }// class AndroidMediaJcfPlayer
}// namespace Jammit.Audio