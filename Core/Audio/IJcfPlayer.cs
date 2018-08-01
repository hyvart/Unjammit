using System;
using System.Collections.Generic;
using System.Text;

namespace Jammit.Audio
{
  public interface IJcfPlayer
  {
    void Play();

    void Pause();

    void Stop();

    TimeSpan Position { get; set; }

    TimeSpan Length { get; }
  }
}
