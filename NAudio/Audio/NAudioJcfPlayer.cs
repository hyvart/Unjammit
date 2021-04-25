﻿using System;
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
    readonly IDictionary<TrackInfo, WaveChannel32> _channels;
    readonly WaveMixerStream32 _mixer;
    readonly IWavePlayer _waveOut;

    #endregion  private members

    //Jam.NET.Properties.Settings.Default.TrackPath
    public NAudioJcfPlayer(JcfMedia media, IWavePlayer waveOut, string tracksPath, byte[] stick)
    {
      _media = media;
      _waveOut = waveOut;
      _mixer = new WaveMixerStream32();
      _channels = new Dictionary<TrackInfo, WaveChannel32>(media.InstrumentTracks.Count + 1 + 1);

      var songPath = Path.Combine(tracksPath, $"{media.Song.Sku}.jcf");
      foreach (var track in media.InstrumentTracks)
      {
        var stream = File.OpenRead(Path.Combine(songPath, $"{track.Identifier.ToString().ToUpper()}_jcfx"));
        _channels[track] = new WaveChannel32(new ImaWaveStream(stream));
      }

      var backingStream = File.OpenRead(Path.Combine(songPath, $"{media.BackingTrack.Identifier.ToString().ToUpper()}_jcfx"));
      _channels[media.BackingTrack] = new WaveChannel32(new ImaWaveStream(backingStream));
      _channels[media.ClickTrack] = new WaveChannel32(new ClickTrackStream(media.Beats, stick));

      foreach (var channel in _channels.Values)
      {
        _mixer.AddInputStream(channel);
        channel.Volume = 1.00f;//TODO: bind?
      }

      _waveOut.PlaybackStopped += (sender, args) => { Position = TimeSpan.Zero; };
      //_waveOut.DesiredLatency = 60;//TODO: Why?
      //_waveOut.NumberOfBuffers = 2;
      _waveOut.Init(_mixer);
    }

    ~NAudioJcfPlayer()
    {
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

    public uint GetVolume(PlayableTrackInfo track) => (uint)_channels[track].Volume;

    public void SetVolume(PlayableTrackInfo track, uint volume)
    {
      _channels[track].Volume = volume / 100.0f;
    }

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

    #endregion  IJcfPlayer members

    public void NotifyPositionChanged()
    {
      PositionChanged?.Invoke(this, new EventArgs());
    }

    public Action TimerAction { get; set; }
  }
}
