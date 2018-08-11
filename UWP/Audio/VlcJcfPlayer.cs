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
  [Obsolete("Use FFmpegJcfPlayer instead.")]
  public class VlcJcfPlayer : BindableObject, IJcfPlayer
  {
    Dictionary<PlayableTrackInfo, VLC.MediaElement> mediaElements;

    public VlcJcfPlayer(Model.JcfMedia media, VLC.MediaElement backingMediaElement, VLC.MediaElement[] instrumentMediaElements)
    {
      // Capacity: backing + instruments
      mediaElements = new Dictionary<PlayableTrackInfo, VLC.MediaElement>(instrumentMediaElements.Length + 1);

      var mediaPath = System.IO.Path.Combine("Tracks", $"{media.Song.Id}.jcf");
      for(int i=0; i<media.InstrumentTracks.Count; i++)
      {
        InitMediaElement(instrumentMediaElements[i], media.InstrumentTracks[i], mediaPath);
      }

      InitMediaElement(backingMediaElement, media.BackingTrack, mediaPath);
    }

    private void InitMediaElement(VLC.MediaElement mediaElement, PlayableTrackInfo track, string basePath)
    {
      var token = "{" + track.Identifier.ToString().ToUpper() + "}";
      var trackPath = System.IO.Path.Combine(basePath, $"{track.Identifier}_jcfx");
      var fileTask = Task.Run(async () => await ApplicationData.Current.LocalFolder.GetFileAsync(trackPath));
      fileTask.Wait();
      StorageApplicationPermissions.FutureAccessList.AddOrReplace(token, fileTask.Result);

      mediaElement.Source = $"winrt://{token}";
      mediaElement.CurrentStateChanged += OnMediaPlayerStateChanged;

      mediaElements[track] = mediaElement;
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

    public event EventHandler PositionChanged;

    public void Play()
    {
      if (PlaybackStatus.Playing == State)
        return;

      foreach (var element in mediaElements.Values)
      {
        element.Play();
      }

      State = PlaybackStatus.Playing;
    }

    public void Pause()
    {
      if (PlaybackStatus.Paused == State)
        return;

      foreach (var element in mediaElements.Values)
      {
        element.Pause();
      }

      State = PlaybackStatus.Paused;
    }

    public void Stop()
    {
      if (PlaybackStatus.Stopped == State)
        return;

      foreach (var element in mediaElements.Values)
      {
        element.Stop();
      }

      State = PlaybackStatus.Stopped;
    }

    public uint GetVolume(PlayableTrackInfo track)
    {
      return (uint)mediaElements[track].Volume;
    }

    public void SetVolume(PlayableTrackInfo track, uint volume)
    {
      mediaElements[track].Volume = (int)volume;
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
