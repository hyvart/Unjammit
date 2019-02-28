using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Jammit.Audio;
using Jammit.Model;
using LibVLCSharp.Shared;

using Path = System.IO.Path;

namespace Jammit.Audio
{
  public class VlcJcfPlayer : IJcfPlayer
  {
    #region private members

    LibVLC _libVLC;
    MediaPlayer _player;
    Media _media;

    #endregion // private members

    public VlcJcfPlayer(JcfMedia media)
    {
      _libVLC = new LibVLC();
      _player = new MediaPlayer(_libVLC);

      var backingPath = "file://" + Path.Combine(media.Path, media.BackingTrack.Identifier.ToString().ToUpper() + "_jcfx");
      var config = new MediaConfiguration();
      config.EnableHardwareDecoding();

      _media = new Media(_libVLC, backingPath, FromType.FromLocation);
      _media.AddOption(config);

      //TODO: Fix. Not currently working.
      foreach (var track in media.InstrumentTracks)
      {
        var path = "file://" + Path.Combine(media.Path, track.Identifier.ToString().ToUpper() + "_jcfx");
        _media.AddSlave(MediaSlaveType.Audio, 4, path);
      }

      _player.Media = _media;
      _player.PositionChanged += Player_PositionChanged;

      Length = media.Length;
    }

    private void Player_PositionChanged(object sender, MediaPlayerPositionChangedEventArgs e)
    {
      PositionChanged?.Invoke(this, new EventArgs());
    }

    #region IJcfPlayer

    public TimeSpan Position
    {
      get
      {
        return TimeSpan.FromMilliseconds(_player.Position * _player.Length);
      }

      set
      {
        _player.Position = (float)(value.TotalMilliseconds / Length.TotalMilliseconds);
      }
    }

    public TimeSpan Length { get; private set; }

    public PlaybackStatus State
    {
      get
      {
        switch (_player.State)
        {
          case VLCState.NothingSpecial:
            return PlaybackStatus.Stopped;
          case VLCState.Opening:
            return PlaybackStatus.Stopped;
          case VLCState.Buffering:
            return PlaybackStatus.Stopped;
          case VLCState.Playing:
            return PlaybackStatus.Playing;
          case VLCState.Paused:
            return PlaybackStatus.Paused;
          case VLCState.Stopped:
          case VLCState.Ended:
          case VLCState.Error:
            return PlaybackStatus.Stopped;
          default:
            throw new Exception("Unknown playback state");
        }
      }
    }

    public event EventHandler PositionChanged;

    public uint GetVolume(PlayableTrackInfo track)
    {
      return (uint)_player.Volume;
    }

    public void Pause()
    {
      if (State != PlaybackStatus.Paused)
        _player.Pause();
    }

    public void Play()
    {
      if (State != PlaybackStatus.Playing)
        _player.Play();
    }

    public void SetVolume(PlayableTrackInfo track, uint volume)
    {
      _player.Volume = (int)volume;
    }

    public void Stop()
    {
      if (State != PlaybackStatus.Stopped)
        _player.Stop();
    } 

    #endregion //IJcfPlayer
  }
}