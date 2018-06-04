using System;

using AVFoundation;
using Foundation;
using Jammit.Model;
using Xamarin.Forms;

namespace Jammit.Audio.macOS
{
  public class MacOSSongPlayer : BindableObject, ISongPlayer
  {
    #region private members

    private ISong song;
    private AVAudioPlayer player;
    private string trackPath;

    #endregion

    public MacOSSongPlayer(ISong songContents)
    {
      ActivateAudioSession();

      this.song = songContents;
      foreach (var t in song.Tracks)
      {
        if (t.Class == "JMFileTrack")
        {
          trackPath = $"{t.Identifier}_jcfx";

          break; //TODO: Add all file channel tracks.
        }
        else if (t.Class == "JMClickTrack")
        {
          //TODO
        }
      }
    }

    //TODO: Find macOS equivalent for AVAudioSession.
    private void ActivateAudioSession()
    {
      //var session = AVAudioSession.SharedInstance();
      //session.SetCategory(AVAudioSessionCategory.Ambient);// Play in the background
      //session.SetActive(true);
    }

    private void DeactivateAudioSession()
    {
      //AVAudioSession.SharedInstance().SetActive(false);
    }

    private void ReactivateAudioSession()
    {
      //AVAudioSession.SharedInstance().SetActive(true);
    }

    #region Bindable properties

    public static readonly BindableProperty LengthProperty =
      BindableProperty.Create("Length", typeof(TimeSpan), typeof(TimeSpan), TimeSpan.FromSeconds(600), BindingMode.OneWay);

    public static readonly BindableProperty PositionProperty =
      BindableProperty.Create("Position", typeof(TimeSpan), typeof(TimeSpan), TimeSpan.Zero, BindingMode.TwoWay);

    void OnDurationChanged(NSObservedChange obj)
    {
      //Length = TimeSpan.FromSeconds(player.Duration);
    }

    void OnPositionChanged(NSObservedChange obj)
    {
      SetValue(PositionProperty, TimeSpan.FromSeconds(player.CurrentTime));
    }

    #endregion // Bindable properties

    #region ISongPlayer members

    public void Play()
    {
      if (PlaybackStatus.Playing == State)
        return;
      
      // Dispose any existing playback.
      if (player != null)
      {
        player.Stop();
        player.Dispose();
      }

      NSError err;
      var stream = song.GetSeekableContentStream(trackPath);
      player = AVAudioPlayer.FromData(NSData.FromStream(stream), out err);

      player.Volume = 0.75f;
      player.FinishedPlaying += delegate {
        player.Dispose();
        player = null;
      };
      player.NumberOfLoops = 1;

      var options = NSKeyValueObservingOptions.New;

      player.AddObserver("duration", options, OnDurationChanged); //TODO: Ensure this triggers. Else, delete.
      player.AddObserver("currentTime", options, OnPositionChanged);

      Length = TimeSpan.FromSeconds(player.Duration);

      player.Play();
      State = PlaybackStatus.Playing;
    }

    public void Pause()
    {
      if (PlaybackStatus.Paused == State)
        return;
      
      Stop();
    }

    public void Stop()
    {
      if (PlaybackStatus.Stopped == State)
        return;
      
      if (player != null)
      {
        player.Stop();
        player.Dispose();
      }

      State = PlaybackStatus.Stopped;
    }

    public PlaybackStatus State { get; private set; }

    public TimeSpan Position
    {
      get
      {
        return (TimeSpan)GetValue(PositionProperty);
      }

      set
      {
        SetValue(PositionProperty, value);

        if (PlaybackStatus.Playing == State)
          player.CurrentTime = value.TotalSeconds;
      }
    }

    public long PositionSamples => 0;

    public TimeSpan Length
    {
      get
      {
        return (TimeSpan)GetValue(LengthProperty);
      }

      private set
      {
        SetValue(LengthProperty, value);
      }
    }

    public int Channels => 1;

    public string GetChannelName(int channel) => "UNIQUE";

    public float GetChannelVolume(int channel) => player.Volume;

    public void SetChannelVolume(int channel, float volume)
    {
      player.Volume = volume;
    }

    #endregion
  }
}
