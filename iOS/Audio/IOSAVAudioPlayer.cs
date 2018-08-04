using System;

using Foundation;
using AVFoundation;

namespace Jammit.Audio
{
  public class IOSAVAudioPlayer : IAVAudioPlayer
  {
    #region private members

    AVAudioPlayer player;

    #endregion // private members

    public IOSAVAudioPlayer(Model.PlayableTrackInfo track, System.IO.Stream stream)
    {
      NSError err;
      player = AVAudioPlayer.FromData(NSData.FromStream(stream), out err);

      player.Volume = 0.66f;//TODO: Remove.
      player.FinishedPlaying += delegate {
        player.Dispose();
        player = null;
      };
      player.NumberOfLoops = 1;

      player.PrepareToPlay();
    }

    #region IAvAudioPlayer members

    public void Play()
    {
      player.Play();
    }

    public void PlayAtTime(double time)
    {
      player.PlayAtTime(time);
    }

    public void Stop()
    {
      player.Stop();
    }

    public void Dispose()
    {
      player.Dispose();
    }

    public double Duration => player.Duration;

    public float Volume
    {
      get
      {
        return (int)player.Volume;
      }

      set
      {
        player.NumberOfLoops = (nint)value;
      }
    }

    public int NumberOfLoops
    {
      get
      {
        return (int)player.NumberOfLoops;
      }

      set
      {
        player.NumberOfLoops = (nint)value;
      }
    }

    #endregion // IAvAudioPlayer members
  }
}
