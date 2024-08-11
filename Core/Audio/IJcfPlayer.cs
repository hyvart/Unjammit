using System;

namespace Jammit.Audio
{
  public interface IJcfPlayer
  {
    void Play();

    void Pause();

    void Stop();

    TrackState.AudioStatus GetAudioStatus(Model.PlayableTrackInfo track);

    uint GetVolume(Model.PlayableTrackInfo track);

    void SetVolume(Model.PlayableTrackInfo track, uint volume);

    void Mute(Model.PlayableTrackInfo track);

    void Unmute(Model.PlayableTrackInfo track);

    uint TotalBeats { get; }

    TimeSpan Length { get; }

    TimeSpan Position { get; set; }

    PlaybackStatus State { get; }

    uint Countdown { get; set; }

    event EventHandler PositionChanged;
  }
}
