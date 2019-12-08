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
  public class FFmpegJcfPlayer : IJcfPlayer
  {
    #region private members

    private Dictionary<PlayableTrackInfo, (MediaPlayer Player, FFmpegInterop.FFmpegInteropMSS)> _players;
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

      // FFmpegInteropMSS instances hold the stream reference. Their scope must be kept.
      _players[track] = (player, ffmpegSource);
    }

    #endregion

    public FFmpegJcfPlayer(JcfMedia media)
    {
      // Capacity => instruments + backing (TODO: + click)
      _players = new Dictionary<PlayableTrackInfo, (MediaPlayer Player, FFmpegInterop.FFmpegInteropMSS)>(media.InstrumentTracks.Count + 1);
      _mediaTimelineController = new MediaTimelineController();
      _mediaTimelineController.PositionChanged += MediaTimelineController_PositionChanged;
      _mediaTimelineController.Ended += MediaTimelineController_Ended;
      _mediaTimelineController.StateChanged += MediaTimelineController_StateChanged;

      var mediaPath = $"ms-appdata:///local/Tracks/{media.Song.Id.ToString().ToUpper()}.jcf";
      foreach (var track in media.InstrumentTracks)
      {
        InitPlayer(track, mediaPath);
      }
      InitPlayer(media.BackingTrack, mediaPath);

      Length = media.Length;
    }

    private void MediaTimelineController_StateChanged(MediaTimelineController sender, object args)
    {
      switch (sender.State)
      {
        case MediaTimelineControllerState.Paused:
          break;
        case MediaTimelineControllerState.Running:
          break;
        case MediaTimelineControllerState.Stalled:
          break;
        case MediaTimelineControllerState.Error:
          break;
        default:
          break;
      }
    }

    private void MediaTimelineController_PositionChanged(MediaTimelineController sender, object args)
    {
      Device.BeginInvokeOnMainThread(() =>
      {
        PositionChanged?.Invoke(this, new EventArgs());
      });
    }

    private void MediaTimelineController_Ended(MediaTimelineController sender, object args)
    {
      //TODO: Trigger. Not currently hit.
      Position = TimeSpan.Zero;
      State = PlaybackStatus.Stopped;
    }

    #region Bindable Properties

    #endregion  Bindable Properties

    #region IJcfPlayer members

    public event EventHandler PositionChanged;

    public void Play()
    {
      if (PlaybackStatus.Playing == State)
        return;

      _mediaTimelineController.Resume();

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
      if (PlaybackStatus.Stopped != State)
        _mediaTimelineController.Pause();

      Position = TimeSpan.Zero;
      State = PlaybackStatus.Stopped;
    }

    public uint GetVolume(PlayableTrackInfo track)
    {
      return (uint)_players[track].Player.Volume;
    }

    public void SetVolume(PlayableTrackInfo track, uint volume)
    {
      _players[track].Player.Volume = volume / 100.0;
    }

    public TimeSpan Position
    {
      get
      {
        return _mediaTimelineController.Position;
      }

      set
      {
        _mediaTimelineController.Position = value;
      }
    }

    public TimeSpan Length { get; private set; }

    public PlaybackStatus State { get; private set; }

    #endregion  IJcfPlayer members
  }
}
