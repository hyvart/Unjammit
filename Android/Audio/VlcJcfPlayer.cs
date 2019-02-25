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
      var path = System.IO.Path.Combine(media.Path, media.BackingTrack.Identifier.ToString().ToUpper() + "_jcfx");
      if (!System.IO.File.Exists(path + ".aifc"))
        System.IO.File.Copy(path, path + ".aifc");

      path += ".aifc";

      _libVLC = vlc;
      _player = player;
      var config = new MediaConfiguration();
      config.EnableHardwareDecoding();

      _media = new Media(_libVLC, $"file://{path}");
      _media.AddOption(config);

      _player.Media = _media;
      var x = _player.AudioTrackCount;
      var z = _player.AudioTrack;
      var a = _player.AudioTrackDescription;
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