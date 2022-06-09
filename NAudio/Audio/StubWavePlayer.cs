using System;
using NAudio.Wave;

namespace Jammit.Audio
{
  public class StubWavePlayer : IWavePlayer
  {
    public StubWavePlayer() { }

    #region IWavePlayer

    public float Volume { get; set; }

    public PlaybackState PlaybackState { get; set; } = PlaybackState.Stopped;

    public WaveFormat OutputWaveFormat => new WaveFormat(44100, 16, 2);

    public event EventHandler<StoppedEventArgs> PlaybackStopped;

    public void Dispose() { }

    public void Init(IWaveProvider waveProvider) { }

    public void Pause()
    {
      PlaybackState = PlaybackState.Paused;
    }

    public void Play()
    {
      PlaybackState = PlaybackState.Playing;
    }

    public void Stop()
    {
      PlaybackState = PlaybackState.Stopped;
    }

    #endregion IWavePlayer
  }
}
