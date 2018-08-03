using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Jammit.Model;

namespace Jammit.Audio
{
  public class AppleJcfPlayer : IJcfPlayer
  {
    #region private members

    private JcfMedia media;
    private IAVAudioPlayer player;

    #endregion // private members

    public AppleJcfPlayer(JcfMedia media, IAVAudioPlayer player)
    {
      this.media = media;
      this.player = player;
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

      var track = media.InstrumentTracks[0];
      var path = Path.Combine(media.Path, $"{track.Identifier}_jcfx");
      var stream = File.OpenRead(path);

      //TODO: Load stream into player.
      //player = AVAudioPlayer.FromData(NSData.FromStream(stream), out err);
      player.Volume = 0.50f;
      //TODO: //player.FinishedPlaying += delegate
      player.NumberOfLoops = 1;

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

      if (player == null)
      {
        player.Stop();
        player.Dispose();
      }

      State = PlaybackStatus.Stopped;
    }

    public uint GetVolume(Model.PlayableTrackInfo track)
    {
      throw new NotImplementedException();
    }

    public void SetVolume(Model.PlayableTrackInfo track, uint volume)
    {
      throw new NotImplementedException();
    }

    public TimeSpan Position { get; set; }

    public TimeSpan Length { get; private set; }

    public PlaybackStatus State { get; private set; }

    #endregion // IJcfPlayer members
  }
}
