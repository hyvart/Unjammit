using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Jammit.Model;

namespace Jammit.Audio
{
  public class AppleJcfPlayer : IJcfPlayer
  {
    #region private members

    JcfMedia media;
    Dictionary<PlayableTrackInfo, IAVAudioPlayer> players;

    #endregion  private members

    public AppleJcfPlayer(JcfMedia media, Func<PlayableTrackInfo, Stream, IAVAudioPlayer> playerFactory)
    {
      this.players = new Dictionary<PlayableTrackInfo, IAVAudioPlayer>(media.InstrumentTracks.Count + 1);
      this.media = media;

      foreach (var track in media.InstrumentTracks)
      {
        players[track] = playerFactory(track, File.OpenRead(Path.Combine(media.Path, $"{track.Identifier.ToString().ToUpper()}_jcfx")));
        players[track].NumberOfLoops = 0;
      }

      players[media.BackingTrack] = playerFactory(media.BackingTrack, File.OpenRead(Path.Combine(media.Path, $"{media.BackingTrack.Identifier.ToString().ToUpper()}_jcfx")));
      players[media.BackingTrack].NumberOfLoops = 0;
      players[media.BackingTrack].PositionChanged += (sender, args) =>
      {
        this.PositionChanged?.Invoke(this, new EventArgs());
      };
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

    #endregion  IJcfPlayer members
  }
}
