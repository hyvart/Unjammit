using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jammit.Model;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Xamarin.Forms;

namespace Jammit.Audio
{
  public class FFmpegJcfPlayer : BindableObject, IJcfPlayer
  {
    #region private members

    private Dictionary<PlayableTrackInfo, MediaPlayer> _players;

    private MediaTimelineController _mediaTimelineController;

    private void InitPlayer(PlayableTrackInfo track, string mediaPath)
    {
      var uri = new Uri($"{mediaPath}/{track.Identifier.ToString().ToUpper()}_jcfx");
      var file = Task.Run(async () => await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri)).Result;
      var stream = Task.Run(async () => await file.OpenReadAsync()).Result;
      var ffmpegSource = FFmpegInterop.FFmpegInteropMSS.CreateFFmpegInteropMSSFromStream(stream, false, false);

      var player = new MediaPlayer();
      player.CommandManager.IsEnabled = false;
      player.TimelineController = _mediaTimelineController;
      player.Source = MediaSource.CreateFromMediaStreamSource(ffmpegSource.GetMediaStreamSource());

      _players[track] = player;
    }

    #endregion

    public FFmpegJcfPlayer(JcfMedia media)
    {
      // Capacity => instruments + backing (TODO: + click)
      _players = new Dictionary<PlayableTrackInfo, MediaPlayer>(media.InstrumentTracks.Count + 1);
      _mediaTimelineController = new MediaTimelineController();
      _mediaTimelineController.PositionChanged += (sender, args) =>
      {
        SetValue(PositionProperty, sender.Position);
      };
      var mediaPath = $"ms-appdata:///local/Tracks/{media.Song.Id.ToString().ToUpper()}.jcf";

      foreach (var track in media.InstrumentTracks)
      {
        InitPlayer(track, mediaPath);
      }
      InitPlayer(media.BackingTrack, mediaPath);

      if (_mediaTimelineController.Duration.HasValue)
        Length = _mediaTimelineController.Duration.Value;
      else
        Length = _players[media.BackingTrack].PlaybackSession.NaturalDuration;
    }

    #region Bindable Properties

    public static readonly BindableProperty LengthProperty =
      BindableProperty.Create("Length", typeof(TimeSpan), typeof(TimeSpan), TimeSpan.FromSeconds(100), BindingMode.OneWayToSource);

    public static readonly BindableProperty PositionProperty =
      BindableProperty.Create("Position", typeof(TimeSpan), typeof(TimeSpan), TimeSpan.Zero, BindingMode.TwoWay);

    #endregion // Bindable Properties

    #region IJcfPlayer members

    public void Play()
    {
      if (PlaybackStatus.Playing == State)
        return;

      _mediaTimelineController.Start();

      State = PlaybackStatus.Playing;
    }

    public void Pause()
    {
      if (PlaybackStatus.Paused == State)
        return;

      _mediaTimelineController.Pause();

      State = PlaybackStatus.Paused;
    }

    public void Stop()
    {
      if (PlaybackStatus.Stopped == State)
        return;

      _mediaTimelineController.Pause();

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

    public TimeSpan Position
    {
      get
      {
        return (TimeSpan)GetValue(PositionProperty);
      }

      set
      {
        _mediaTimelineController.Position = value;
      }
    }

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
