using System;
using System.Collections.Generic;
using System.Text;

namespace Jammit.Audio
{
  public interface IAVAudioPlayer
  {
    void Play();

    void PlayAtTime(double time);

    void Stop();

    void Dispose();

    double Duration { get; }

    double CurrentTime { get; set; }

    float Volume { get; set; }

    int NumberOfLoops { get; set; }

    event EventHandler PositionChanged;
  }
}
