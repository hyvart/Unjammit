using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jammit.Model;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Xamarin.Forms;

namespace Jammit.Audio
{
  public class VlcJcfPlayer : BindableObject, IJcfPlayer
  {
    private VLC.MediaElement mediaElement;

    public VlcJcfPlayer(Model2.JcfMedia media, VLC.MediaElement mediaElement)
    {
      // Load default track
      //TODO: Load all tracks.
      var track = media.InstrumentTracks[0];
      var token = "{" + track.Identifier.ToString().ToUpper() + "}";
      var trackPath = $"Tracks\\{media.Song.Id}.jcf\\{track.Identifier}_jcfx";
      var fileTask = Task.Run(async () => await ApplicationData.Current.LocalFolder.GetFileAsync(trackPath));
      fileTask.Wait();
      StorageApplicationPermissions.FutureAccessList.AddOrReplace(token, fileTask.Result);

      // Configure media element
      this.mediaElement = mediaElement;
      this.mediaElement.Source = $"winrt://{token}";
      this.mediaElement.CurrentStateChanged += OnMediaPlayerStateChanged;
    }

    void OnMediaPlayerStateChanged(object sender, Windows.UI.Xaml.RoutedEventArgs eventArgs)
    {
    }

    #region Bindable Properties

    public static readonly BindableProperty LengthProperty =
      BindableProperty.Create("Length", typeof(TimeSpan), typeof(TimeSpan), TimeSpan.FromSeconds(600), BindingMode.OneWayToSource);

    public static readonly BindableProperty PositionProperty =
      BindableProperty.Create("Position", typeof(TimeSpan), typeof(TimeSpan), TimeSpan.Zero, BindingMode.TwoWay);

    #endregion // Bindable Properties

    #region IJcfPlayer members

    public async void Play()
    {
      if (PlaybackStatus.Playing == State)
        return;

      mediaElement.Play();
      State = PlaybackStatus.Playing;
    }

    public void Pause()
    {
      if (PlaybackStatus.Paused == State)
        return;

      mediaElement.Pause();
      State = PlaybackStatus.Paused;
    }

    public void Stop()
    {
      if (PlaybackStatus.Stopped == State)
        return;

      mediaElement.Stop();
      State = PlaybackStatus.Stopped;
    }

    public uint GetVolume(PlayableTrackInfo track)
    {
      throw new NotImplementedException();
    }

    public void SetVolume(PlayableTrackInfo track, uint volume)
    {
      throw new NotImplementedException();
    }

    public TimeSpan Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public TimeSpan Length
    {
      get
      {
        return (TimeSpan)GetValue(LengthProperty);
      }

      set
      {
        SetValue(LengthProperty, value);
      }
    }

    public PlaybackStatus State { get; private set; }

    #endregion // IJcfPlayer members
  }
}
