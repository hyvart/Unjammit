using System;
using System.Collections.Generic;

using Jammit.Model;
using LibVLCSharp.Shared;

using Path = System.IO.Path;

namespace Jammit.Audio
{
  public class VlcJcfPlayer : IJcfPlayer
  {
    #region private members

    LibVLC _libVLC;
    Dictionary<PlayableTrackInfo, MediaPlayer> _players;
    PlayableTrackInfo _backingTrack; // Keep for tracking player.

    #endregion // private members

    public VlcJcfPlayer(JcfMedia media, MediaConfiguration[] configs)
    {
      _libVLC = new LibVLC();
      _players = new Dictionary<PlayableTrackInfo, MediaPlayer>(media.InstrumentTracks.Count + 1);

      var backingPath = "file://" + Path.Combine(media.Path, media.BackingTrack.Identifier.ToString().ToUpper() + "_jcfx");
      var backingPlayer = new MediaPlayer(_libVLC);
      backingPlayer.Media = new Media(_libVLC, backingPath, FromType.FromLocation);
      foreach (var config in configs)
        backingPlayer.Media.AddOption(config);
      backingPlayer.PositionChanged += Player_PositionChanged;
      _players[media.BackingTrack] = backingPlayer;
      _backingTrack = media.BackingTrack;

      foreach (var track in media.InstrumentTracks)
      {
        var path = "file://" + Path.Combine(media.Path, track.Identifier.ToString().ToUpper() + "_jcfx");
        var player = new MediaPlayer(_libVLC);
        player.Media = new Media(_libVLC, path, FromType.FromLocation);
        foreach(var config in configs)
          player.Media.AddOption(config);
        _players[track] = player;
      }

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
        return TimeSpan.FromMilliseconds(_players[_backingTrack].Position * _players[_backingTrack].Length);
      }

      set
      {
        foreach(var player in _players.Values)
          player.Position = (float)(value.TotalMilliseconds / Length.TotalMilliseconds);

        PositionChanged?.Invoke(this, new EventArgs());
      }
    }

    public TimeSpan Length { get; private set; }

    public PlaybackStatus State
    {
      get
      {
        switch (_players[_backingTrack].State)
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
      return (uint)_players[track].Volume;
    }

    public void Pause()
    {
      if (State != PlaybackStatus.Paused)
        foreach(var player in _players.Values)
          player.Pause();
    }

    public void Play()
    {
      if (State != PlaybackStatus.Playing)
        foreach(var player in _players.Values)
          player.Play();
    }

    public void SetVolume(PlayableTrackInfo track, uint volume)
    {
      _players[track].Volume = (int)volume;
    }

    public void Stop()
    {
      if (State != PlaybackStatus.Stopped)
        foreach(var player in _players.Values)
          player.Stop();

      Position = TimeSpan.Zero;
    }

    #endregion //IJcfPlayer
  }
}