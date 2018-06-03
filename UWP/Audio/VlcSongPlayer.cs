using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jammit.Model;
using Windows.Storage;
using Windows.Storage.AccessCache;

namespace Jammit.Audio
{
  public class VlcSongPlayer : ISongPlayer
  {
    #region private members

    VLC.MediaElement mediaElement;

    #endregion // private members

    public VlcSongPlayer(ISong s, VLC.MediaElement mediaElement)
    {
      string token = "";
      foreach (var t in s.Tracks)
      {
        if (t.Class == "JMFileTrack")
        {
          token = "{" + t.Identifier.ToString().ToUpper() + "}";

          // Store track file reference in storage cache (winrt://).
          string trackPath = $"Tracks\\{s.Metadata.Id}.jcf\\{t.Identifier}_jcfx";
          var fileTask = Task.Run(async() => await ApplicationData.Current.LocalFolder.GetFileAsync(trackPath));
          fileTask.Wait();
          StorageApplicationPermissions.FutureAccessList.AddOrReplace(token, fileTask.Result);
        }
        else
        {
          //TODO: Click track
        }
      }

      this.mediaElement = mediaElement;
      this.mediaElement.Source = $"winrt://{token}";
    }

    #region ISongPlayer members

    public long PositionSamples => throw new NotImplementedException();

    public TimeSpan Position
    {
      get
      {
        return this.mediaElement.Position;
      }

      set
      {
        this.mediaElement.Position = value;
      }
    }

    public TimeSpan Length
    {
      get
      {
        long length = this.mediaElement.MediaPlayer.length();
        if (length < 0)
          return TimeSpan.Zero;

        return TimeSpan.FromMilliseconds(length);
      }
    }

    public PlaybackStatus State { get; private set; }

    public int Channels => throw new NotImplementedException();

    public string GetChannelName(int channel)
    {
      throw new NotImplementedException();
    }

    public float GetChannelVolume(int channel)
    {
      return this.mediaElement.Volume;
    }

    public void Pause()
    {
      if (PlaybackStatus.Paused == this.State)
        return;

      this.mediaElement.Pause();
      this.State = PlaybackStatus.Paused;
    }

    public void Play()
    {
      if (PlaybackStatus.Playing == this.State)
        return;

      var player = this.mediaElement.MediaPlayer;
      var length = player.length();
      this.mediaElement.Play();
      length = player.length();
      this.State = PlaybackStatus.Playing;
    }

    public void SetChannelVolume(int channel, float volume)
    {
      this.mediaElement.Volume = (int)volume;
    }

    public void Stop()
    {
      if (PlaybackStatus.Stopped == this.State)
        return;

      var length = mediaElement.MediaPlayer.length();

      this.mediaElement.Stop();
      this.State = PlaybackStatus.Stopped;
    }

    #endregion // ISongPlayer members
  }
}
