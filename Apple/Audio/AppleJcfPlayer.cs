using System;
using System.Collections.Generic;
using System.IO;

using AVFoundation;
using Foundation;

using Jammit.Model;

namespace Jammit.Audio
{
  public class AppleJcfPlayer : IJcfPlayer
  {
    #region private members

    JcfMedia media;
    Dictionary<PlayableTrackInfo, AVAudioPlayer> players;
    NSTimer timer;

    #endregion  private members

    public AppleJcfPlayer(JcfMedia media)
    {
      this.players = new Dictionary<PlayableTrackInfo, AVAudioPlayer>(media.InstrumentTracks.Count + 1);
      this.media = media;

      NSError error;
      foreach (var track in media.InstrumentTracks)
      {
        players[track] = AVAudioPlayer.FromData(NSData.FromStream(File.OpenRead(Path.Combine(media.Path, $"{track.Identifier.ToString().ToUpper()}_jcfx"))), out error);
        players[track].FinishedPlaying += delegate { };
        players[track].PrepareToPlay();

        //TODO: Do something useful here or remove (beware nullptr after playback done).

        players[track].NumberOfLoops = 0;
      }

      players[media.BackingTrack] = AVAudioPlayer.FromData(NSData.FromStream(File.OpenRead(Path.Combine(media.Path, $"{media.BackingTrack.Identifier.ToString().ToUpper()}_jcfx"))), out error);
      players[media.BackingTrack].NumberOfLoops = 0;
    }

    #region IJcfPlayer members

    public event EventHandler PositionChanged;

    public void Play()
    {
      if (PlaybackStatus.Playing == State)
        return;

      foreach (var player in players.Values)
      {
        player.Play();

        //TODO: Avoid repetition. Move into some post-construct phase.
        if (timer == null)
          timer = NSTimer.CreateRepeatingScheduledTimer(1, delegate {
            PositionChanged?.Invoke(this, new EventArgs());
          });
      }

      State = PlaybackStatus.Playing;
    }

    public void Pause()
    {
      if (PlaybackStatus.Paused == State)
        return;

      foreach (var player in players.Values)
      {
        player.Stop();
      }

      State = PlaybackStatus.Paused;
    }

    public void Stop()
    {
      if (PlaybackStatus.Stopped != State)
        foreach (var player in players.Values)
          player.Stop();

      Position = TimeSpan.Zero;
      State = PlaybackStatus.Stopped;
    }

    public uint GetVolume(PlayableTrackInfo track)
    {
      return (uint)players[track].Volume;
    }

    public void SetVolume(PlayableTrackInfo track, uint volume)
    {
      players[track].Volume = volume / 100.0f;
    }

    TrackState.AudioStatus IJcfPlayer.GetAudioStatus(PlayableTrackInfo track)
    {
      throw new NotImplementedException();
    }

    void IJcfPlayer.Mute(PlayableTrackInfo track)
    {
      throw new NotImplementedException();
    }

    void IJcfPlayer.Unmute(PlayableTrackInfo track)
    {
      throw new NotImplementedException();
    }

    public uint TotalBeats { get; private set; }

    public TimeSpan Position
    {
      get
      {
        return TimeSpan.FromSeconds(players[media.BackingTrack].CurrentTime);
      }

      set
      {
        foreach (var player in players.Values)
        {
          player.CurrentTime = value.TotalSeconds;
        }

        PositionChanged?.Invoke(this, new EventArgs());
      }
    }

    public TimeSpan Length => media.Length;

    public PlaybackStatus State { get; private set; }

    public uint Countdown { get; set; } = 0;

    #endregion  IJcfPlayer members
  }
}
