using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Jammit.Model;
using Xamarin.Forms;

namespace Jammit.Audio
{
  public class AppleJcfPlayer : Xamarin.Forms.BindableObject, IJcfPlayer
  {
    #region private members

    JcfMedia media;
    Dictionary<PlayableTrackInfo, IAVAudioPlayer> players;

    #endregion // private members

    public AppleJcfPlayer(JcfMedia media, Func<PlayableTrackInfo, Stream, IAVAudioPlayer> playerFactory)
    {
      this.players = new Dictionary<PlayableTrackInfo, IAVAudioPlayer>(media.InstrumentTracks.Count + 1);
      this.media = media;

      foreach (var track in media.InstrumentTracks)
      {
        players[track] = playerFactory(track, File.OpenRead(Path.Combine(media.Path, $"{track.Identifier.ToString().ToUpper()}_jcfx")));
      }

      players[media.BackingTrack] = playerFactory(media.BackingTrack, File.OpenRead(Path.Combine(media.Path, $"{media.BackingTrack.Identifier.ToString().ToUpper()}_jcfx")));

      Length = TimeSpan.FromSeconds(players[media.BackingTrack].Duration);
    }

    #region Bindable properties

    public static readonly BindableProperty LengthProperty =
      BindableProperty.Create("Length", typeof(TimeSpan), typeof(TimeSpan), TimeSpan.FromSeconds(10), BindingMode.OneWayToSource);

    #endregion

    #region IJcfPlayer members

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

      Stop();
    }

    public void Stop()
    {
      if (PlaybackStatus.Stopped == State)
        return;

      foreach (var player in players.Values)
      {
        if (player == null)
        {
          player.Stop();
          player.Dispose();
        }

        player.Stop();
      }

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

    public TimeSpan Position { get; set; }

    public TimeSpan Length
    {
      get
      {
        return (TimeSpan)GetValue(LengthProperty);
      }

      private set
      {
        SetValue(LengthProperty, value);
      }
    }

    public PlaybackStatus State { get; private set; }

    #endregion // IJcfPlayer members
  }
}
