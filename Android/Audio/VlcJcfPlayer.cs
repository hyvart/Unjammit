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

    public VlcJcfPlayer(JcfMedia media, LibVLC vlc, MediaPlayer player)
    {
      _libVLC = vlc;
      _player = player;

      var backingPath = "file://" + Path.Combine(media.Path, media.BackingTrack.Identifier.ToString().ToUpper() + "_jcfx");
      var config = new MediaConfiguration();
      config.EnableHardwareDecoding();

      _media = new Media(_libVLC, backingPath, FromType.FromLocation);
      _media.AddOption(config);

      foreach (var track in media.InstrumentTracks)
      {
        var path = "file://" + Path.Combine(media.Path, track.Identifier.ToString().ToUpper() + "_jcfx");
        _media.AddSlave(MediaSlaveType.Audio, 4, path);
      }

      _player.Media = _media;
    }

    #region IJcfPlayer

    public TimeSpan Position
    {
      get => TimeSpan.Zero;

      set => throw new NotImplementedException();
    }

    public TimeSpan Length => TimeSpan.FromMilliseconds(300 * 1000);// TimeSpan.FromMilliseconds(_media.Duration);

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