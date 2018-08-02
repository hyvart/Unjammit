using System;

using AVFoundation;
using Foundation;
using Jammit.Model;
using Xamarin.Forms;

namespace Jammit.Audio
{
  public class IOSJcfPlayer : BindableObject, IJcfPlayer
  {
    #region private members

    private Model2.JcfMedia media;
    private AVAudioPlayer player;

    private void ActivateAudioSession()
    {
      var session = AVAudioSession.SharedInstance();
      session.SetCategory(AVAudioSessionCategory.Ambient); // Background
      session.SetActive(true);
    }

    private void DeactivateAudioSession()
    {
      AVAudioSession.SharedInstance().SetActive(false);
    }

    private void ReactivateAudioSession()
    {
      AVAudioSession.SharedInstance().SetActive(true);
    }

    #endregion // private members

    public IOSJcfPlayer(Model2.JcfMedia media)
    {
      this.media = media;
    }

    #region IJcfPlayer members

    public void Play()
    {
      if (PlaybackStatus.Playing == State)
        return;

      // Dispose any existing playback.
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

    #endregion // IJcfPlayer members
  }
}
