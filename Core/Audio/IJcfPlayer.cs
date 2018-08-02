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

    uint GetVolume(Model.PlayableTrackInfo track);

    void SetVolume(Model.PlayableTrackInfo track, uint volume);

    TimeSpan Position { get; set; }

    TimeSpan Length { get; }

    PlaybackStatus State { get; }
  }
}
