using System;
using System.Collections.Generic;
using System.IO;

using Jammit.Model;
using NAudio.Wave;

namespace Jammit.Audio
{
  public class NAudioJcfPlayer : IJcfPlayer
  {
    #region private members

    JcfMedia _media;
    readonly IDictionary<TrackInfo, TrackState> _trackStates;
    readonly IDictionary<TrackInfo, WaveChannel32> _channels;
    readonly WaveMixerStream32 _mixer;
    readonly IWavePlayer _waveOut;
    readonly ClickTrackStream _clickTrackStream;

    #endregion  private members

    //Jam.NET.Properties.Settings.Default.TrackPath
    public NAudioJcfPlayer(JcfMedia media, IWavePlayer waveOut, string tracksPath, byte[] stick)
    {
      _media = media;
      _waveOut = waveOut;
      _mixer = new WaveMixerStream32();
      _channels = new Dictionary<TrackInfo, WaveChannel32>(media.InstrumentTracks.Count + 1 + 1);
      _trackStates = new Dictionary<TrackInfo, TrackState>(media.InstrumentTracks.Count + 1 + 1);

      var songPath = Path.Combine(tracksPath, $"{media.Song.Sku}.jcf");
      foreach (var track in media.InstrumentTracks)
      {
        var stream = File.OpenRead(Path.Combine(songPath, $"{track.Identifier.ToString().ToUpper()}_jcfx"));
        _channels[track] = new WaveChannel32(new ImaWaveStream(stream));
        _trackStates[track] = new TrackState();
      }

      var backingStream = File.OpenRead(Path.Combine(songPath, $"{media.BackingTrack.Identifier.ToString().ToUpper()}_jcfx"));
      _channels[media.BackingTrack] = new WaveChannel32(new ImaWaveStream(backingStream));
      _trackStates[media.BackingTrack] = new TrackState();

      _clickTrackStream = new ClickTrackStream(media.Beats, stick);
      _clickTrackStream.OnBeatChanged += NotifyBeatChanged;
      _channels[media.ClickTrack] = new WaveChannel32(_clickTrackStream);
      _trackStates[media.ClickTrack] = new TrackState();

      foreach (var channel in _channels.Values)
      {
        _mixer.AddInputStream(channel);
        channel.Volume = 1.00f;//TODO: bind?
      }

      _waveOut.PlaybackStopped += (sender, args) => { Position = TimeSpan.Zero; };
      //_waveOut.DesiredLatency = 60;//TODO: Why?
      //_waveOut.NumberOfBuffers = 2;
      _waveOut.Init(_mixer);

      TotalBeats = (uint)_media.Beats.Count;
    }

    ~NAudioJcfPlayer()
    {
      _clickTrackStream.OnBeatChanged -= NotifyBeatChanged;

      if (PlaybackState.Playing == _waveOut.PlaybackState)
        _waveOut.Stop();

      _waveOut.Dispose();
    }

    #region IJcfPlayer members

    public event EventHandler PositionChanged;

    public void Play()
    {
      _waveOut.Play();

      TimerAction();
    }

    public void Pause()
    {
      _waveOut.Pause();
    }

    public void Stop()
    {
      _waveOut.Stop();

      Position = TimeSpan.Zero;
    }

    public TrackState.AudioStatus GetAudioStatus(Model.PlayableTrackInfo track)
    {
      return _trackStates[track].Status;
    }

    public uint GetVolume(PlayableTrackInfo track) => _trackStates[track].Volume;

    public void SetVolume(PlayableTrackInfo track, uint volume)
    {
      //_channels[track].Volume = volume / 100.0f;
      ////TODO: Add ClickTrackStream "volume"?
      ///

      _trackStates[track].Volume = volume;

      var trackAudioStatus = _trackStates[track].Status;
      if (trackAudioStatus != TrackState.AudioStatus.Muted &&
          trackAudioStatus != TrackState.AudioStatus.AutoMuted &&
          trackAudioStatus != TrackState.AudioStatus.Excluded)
        _channels[track].Volume = volume / 100f;
    }

    public void Mute(PlayableTrackInfo track)
    {
      _trackStates[track].Status = TrackState.AudioStatus.Muted;
      _channels[track].Volume = 0;
    }

    public void Unmute(PlayableTrackInfo track)
    {
      _channels[track].Volume = _trackStates[track].Volume / 100f;
      _trackStates[track].Status = TrackState.AudioStatus.On;
    }

    public uint TotalBeats { get; private set; }

    public TimeSpan Length => _media.Length;

    public TimeSpan Position
    {
      get { return _mixer.CurrentTime; }

      set
      {
        if (value == _mixer.CurrentTime)
          return;

        _mixer.CurrentTime = value;
        PositionChanged?.Invoke(this, new EventArgs());
      }
    }

    public PlaybackStatus State
    {
      get
      {
        switch (_waveOut.PlaybackState)
        {
          case PlaybackState.Stopped:
            return PlaybackStatus.Stopped;
          case PlaybackState.Playing:
            return PlaybackStatus.Playing;
          case PlaybackState.Paused:
            return PlaybackStatus.Paused;
          default:
            throw new NotImplementedException("Unrecognized PlaybackStatus value.");
        }
      }
    }

    public uint Countdown { get; set; } = 0;

    #endregion  IJcfPlayer members

    public void NotifyPositionChanged()
    {
      PositionChanged?.Invoke(this, new EventArgs());
    }

    private void NotifyBeatChanged(object sender, ClickTrackStream.BeatChangedEventArgs args)
    {
      var audioStatus = _trackStates[_media.ClickTrack].Status;
      var currentBeatIndex = args.CurrentBeatIndex;

      if ((TrackState.AudioStatus.On == audioStatus || TrackState.AudioStatus.Solo == audioStatus) &&
          0 != Countdown && Countdown <= currentBeatIndex)
      {
        _channels[_media.ClickTrack].Volume = 0;
        _trackStates[_media.ClickTrack].Status = TrackState.AudioStatus.AutoMuted;
      }
      else if (TrackState.AudioStatus.AutoMuted == audioStatus &&
          (0 == Countdown || Countdown > currentBeatIndex))
      {
        _channels[_media.ClickTrack].Volume = _trackStates[_media.ClickTrack].Volume / 100f;
        _trackStates[_media.ClickTrack].Status = TrackState.AudioStatus.On;//TODO: What aboout Solo?
      }
    }

    public Action TimerAction { get; set; }
  }
}
