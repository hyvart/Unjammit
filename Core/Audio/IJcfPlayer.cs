using System;

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

    event EventHandler PositionChanged;

    //event EventHandler BeatChanged;
    event EventHandler CountdownFinished;
  }
}
