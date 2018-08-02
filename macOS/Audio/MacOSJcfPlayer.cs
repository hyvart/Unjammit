using System;

using AVFoundation;
using Foundation;
using Jammit.Model;
using Xamarin.Forms;

namespace Jammit.Audio
{
  public class MacOSJcfPlayer : BindableObject, IJcfPlayer
  {
    #region private members

    private AVAudioPlayer player;
    private Model2.JcfMedia media;

    #endregion

    public MacOSJcfPlayer(Model2.JcfMedia media)
    {
      this.media = media;
    }

    #region IJcfPlayer members

    public async void Play()
    {
      if (PlaybackStatus.Playing == State)
        return;

      // Displose any existing playback.
      if (player != null)
      {
        player.Stop();
        player.Dispose();
      }

      NSError err;

      var track = media.InstrumentTracks[0];
      var path = System.IO.Path.Combine(media.Path, $"{track.Identifier}_jcfx");
      var stream = System.IO.File.OpenRead(path);
      player = AVAudioPlayer.FromData(NSData.FromStream(stream), out err);
      player.Volume = 0.75f;
      player.FinishedPlaying += delegate
      {
        player.Dispose();
        player = null;
      };
      player.NumberOfLoops = 1;

      var options = NSKeyValueObservingOptions.New;
      player.AddObserver("duration", options, OnDurationChanged);
      player.AddObserver("currentTime", options, OnPositionChanged);

      Length = TimeSpan.FromSeconds(player.Duration);
      player.Play();

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

      if (player != null)
      {
        player.Stop();
        player.Dispose();
      }

      State = PlaybackStatus.Stopped;
    }

    public uint GetVolume(PlayableTrackInfo track) => throw new NotImplementedException();

    public void SetVolume(PlayableTrackInfo track, uint volume) => throw new NotImplementedException();

    public TimeSpan Position { get; set; }

    public TimeSpan Length { get; private set; }

    public PlaybackStatus State { get; private set; }

    #endregion

    #region Events

    void OnDurationChanged(NSObservedChange obj)
    {
    }

    void OnPositionChanged(NSObservedChange obj)
    {
    }

    #endregion
  }
}
