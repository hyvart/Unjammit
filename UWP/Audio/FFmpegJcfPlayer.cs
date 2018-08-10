using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jammit.Model;
using Windows.Media.Core;
using Windows.Media.Playback;
using Xamarin.Forms;

using Windows.Storage;

namespace Jammit.Audio
{
  public class FFmpegJcfPlayer : BindableObject, IJcfPlayer
  {
    #region private members

    Dictionary<PlayableTrackInfo, MediaPlayer> _players;

    private void InitPlayer(PlayableTrackInfo track, string mediaPath)
    {
      var uri = $"{mediaPath}/{track.Identifier.ToString().ToUpper()}_jcfx";
      var ffmpegSource = FFmpegInterop.FFmpegInteropMSS.CreateFFmpegInteropMSSFromUri(uri, false, false);
      _players[track] = new MediaPlayer()
      {
        Source = MediaSource.CreateFromMediaStreamSource(ffmpegSource.GetMediaStreamSource())
      };
    }

    #endregion

    public FFmpegJcfPlayer(JcfMedia media)
    {
      // Capacity => instruments + backing (TODO: + click)
      _players = new Dictionary<PlayableTrackInfo, MediaPlayer>(media.InstrumentTracks.Count + 1);
      //var mediaPath = System.IO.Path.Combine("Tracks", $"{media.Song.Id}.jcf");
      var mediaPath = $"ms-appdata://local/Tracks/{media.Song.Id.ToString().ToUpper()}.jcf";
      var miuri = new Uri($"{mediaPath}/{media.BackingTrack.Identifier.ToString().ToUpper()}_jcfx");
      var file = Task.Run(async () => await StorageFile.GetFileFromApplicationUriAsync(miuri)).Result;

      foreach (var track in media.InstrumentTracks)
      {
        InitPlayer(track, mediaPath);
      }

      InitPlayer(media.BackingTrack, mediaPath);
    }

    #region Bindable Properties

    public static readonly BindableProperty LengthProperty =
      BindableProperty.Create("Length", typeof(TimeSpan), typeof(TimeSpan), TimeSpan.FromSeconds(600), BindingMode.OneWayToSource);

    public static readonly BindableProperty PositionProperty =
      BindableProperty.Create("Position", typeof(TimeSpan), typeof(TimeSpan), TimeSpan.Zero, BindingMode.TwoWay);

    #endregion // Bindable Properties

    #region IJcfPlayer members

    public void Play()
    {
      if (PlaybackStatus.Playing == State)
        return;

      foreach (var player in _players.Values)
      {
        player.Play();
      }

      State = PlaybackStatus.Playing;
    }

    public void Pause()
    {
      if (PlaybackStatus.Paused == State)
        return;

      foreach (var player in _players.Values)
      {
        player.Pause();
      }

      State = PlaybackStatus.Paused;
    }

    public void Stop()
    {
      if (PlaybackStatus.Stopped == State)
        return;

      foreach (var player in _players.Values)
      {
        player.Pause();
      }

      State = PlaybackStatus.Stopped;
    }

    public uint GetVolume(PlayableTrackInfo track)
    {
      return (uint)_players[track].Volume;
    }

    public void SetVolume(PlayableTrackInfo track, uint volume)
    {
      _players[track].Volume = volume;
    }

    public TimeSpan Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public TimeSpan Length
    {
      get
      {
        return (TimeSpan)GetValue(LengthProperty);
      }

      set
      {
        SetValue(LengthProperty, value);
      }
    }

    public PlaybackStatus State { get; private set; }

    #endregion // IJcfPlayer members
  }
}
