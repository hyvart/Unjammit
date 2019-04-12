using System;

using Foundation;
using AVFoundation;

namespace Jammit.Audio
{
  public class IOSAVAudioPlayer : IAVAudioPlayer
  {
    #region private members

    AVAudioPlayer player;
    NSTimer timer;

    #endregion // private members

    public IOSAVAudioPlayer(Model.PlayableTrackInfo track, System.IO.Stream stream)
    {
      NSError error;
      player = AVAudioPlayer.FromData(NSData.FromStream(stream), out error);

      //TODO: Do something useful here or remove (beware nullptr after playback done).
      player.FinishedPlaying += delegate {};
      player.PrepareToPlay();

      timer = NSTimer.CreateRepeatingScheduledTimer(1, delegate {
        PositionChanged?.Invoke(this, new EventArgs());
      });
    }

    #region IAvAudioPlayer members

    public event EventHandler PositionChanged;

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

    public double CurrentTime
    {
      get
      {
        return player.CurrentTime;
      }

      set
      {
        player.CurrentTime = value;
      }
    }

    public float Volume
    {
      get
      {
        return player.Volume;
      }

      set
      {
        player.Volume = value;
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
