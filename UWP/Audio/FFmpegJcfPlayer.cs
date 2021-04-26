using System;
using System.Collections.Generic;
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
    #region static members

    public static async Task<FFmpegJcfPlayer> CreateAsync(JcfMedia media)
    {
      var instance = new FFmpegJcfPlayer();

      // Capacity => instruments + backing (TODO: + click)
      instance._players = new Dictionary<PlayableTrackInfo, (MediaPlayer Player, FFmpegInterop.FFmpegInteropMSS)>(media.InstrumentTracks.Count + 1);
      instance._mediaTimelineController = new MediaTimelineController();
      instance._mediaTimelineController.PositionChanged += instance.MediaTimelineController_PositionChanged;
      instance._mediaTimelineController.StateChanged += instance.MediaTimelineController_StateChanged;
      instance._mediaTimelineController.Ended += instance.MediaTimelineController_Ended;

      var mediaPath = $"ms-appdata:///local/Tracks/{media.Song.Sku}.jcf";
      foreach (var track in media.InstrumentTracks)
      {
        await instance.InitPlayer(track, mediaPath);
      }
      await instance.InitPlayer(media.BackingTrack, mediaPath);

      instance.Length = media.Length;

      return instance;
    }

    #endregion static members

    #region private members

    private Dictionary<PlayableTrackInfo, (MediaPlayer Player, FFmpegInterop.FFmpegInteropMSS)> _players;
    private MediaTimelineController _mediaTimelineController;

    private async Task InitPlayer(PlayableTrackInfo track, string mediaPath)
    {
      var uri = $"{mediaPath}/{track.Identifier.ToString().ToUpper()}_jcfx";
      var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri(uri));
      var stream = await file.OpenReadAsync();
      var ffmpegSource = await FFmpegInterop.FFmpegInteropMSS.CreateFromStreamAsync(stream);
      //TODO: Re-enable. Possible bug in FFmpegInterop.
      //var ffmpegSource = await FFmpegInterop.FFmpegInteropMSS.CreateFromUriAsync(uri);

      var player = new MediaPlayer();
      player.CommandManager.IsEnabled = false;
      player.TimelineController = _mediaTimelineController;
      player.Source = Windows.Media.Core.MediaSource.CreateFromMediaStreamSource(ffmpegSource.GetMediaStreamSource());

      // FFmpegInteropMSS instances hold the stream reference. Their scope must be kept.
      _players[track] = (player, ffmpegSource);
    }

    #endregion

    public FFmpegJcfPlayer() {}

    private void MediaTimelineController_StateChanged(MediaTimelineController sender, object args)
    {
      switch (sender.State)
      {
        case MediaTimelineControllerState.Paused:
          State = PlaybackStatus.Paused;
          break;
        case MediaTimelineControllerState.Running:
        case MediaTimelineControllerState.Stalled:
          State = PlaybackStatus.Playing;
          break;
        case MediaTimelineControllerState.Error:
          State = PlaybackStatus.Stopped;
          break;
        default:
          throw new Exception("Unknown playback state");
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

    #region IJcfPlayer members

    public event EventHandler PositionChanged;

    public void Play()
    {
      if (PlaybackStatus.Playing == State)
        return;

      _mediaTimelineController.Resume();

      //Force Playing state due to _mediaTimelineController.StateChanged delay.
      if (PlaybackStatus.Playing != State)
        State = PlaybackStatus.Playing;
    }

    public void Pause()
    {
      if (PlaybackStatus.Paused == State)
        return;

      _mediaTimelineController.Pause();
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
      // Click track not implemented here (yet).
      if (track.Class == "JMClickTrack")
        return;

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
