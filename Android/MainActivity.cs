using System;

using Android.App;
using Android.Content.PM;
using Android.OS;

using LibVLCSharp.Shared;

namespace Jammit.Android
{
  [Activity(Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
  public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
  {
    protected override void OnCreate(Bundle savedInstanceState)
    {
      TabLayoutResource = Resource.Layout.Tabbar;
      ToolbarResource = Resource.Layout.Toolbar;

      base.OnCreate(savedInstanceState);
      global::LibVLCSharp.Shared.Core.Initialize();

      var config = new MediaConfiguration();
      config.EnableHardwareDecoding = true;

      Jammit.Forms.App.DataDirectory = Xamarin.Essentials.FileSystem.AppDataDirectory;

#if false
      // To prevent audo desync, use SoXR resampler.
      // See https://code.videolan.org/videolan/LibVLCSharp/issues/263#note_48330
      Jammit.Forms.App.PlayerFactory = async (media) => await System.Threading.Tasks.Task.Run(() =>
        new Audio.VlcJcfPlayer(media, new MediaConfiguration[] { config }, new string[] { }));
#else
      Jammit.Forms.App.PlayerFactory = async (media) => await System.Threading.Tasks.Task.Run(() =>
      {
        var player = new Audio.NAudioJcfPlayer(
          media,
          new Audio.AndroidWavePlayer { DesiredLatency = 60, NumberOfBuffers = 2 },
            System.IO.Path.Combine(Xamarin.Essentials.FileSystem.AppDataDirectory, "Tracks"),
            Forms.Resources.Assets.Stick);

        player.TimerAction = () =>
        {
          Xamarin.Forms.Device.StartTimer(new TimeSpan(0, 0, 1), () =>
          {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() => player.NotifyPositionChanged());

            return player.State == Audio.PlaybackStatus.Playing;
          });
        };

        return player;
      });
#endif

      Jammit.Forms.App.MediaLoader = new Model.FileSystemJcfLoader(Xamarin.Essentials.FileSystem.AppDataDirectory);

      global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

      global::Xamarin.Essentials.Platform.Init(this, savedInstanceState);

      global::Xamarin.Forms.MessagingCenter.Subscribe<Jammit.Forms.Views.SongPage>(this, "PreventPortrait", sender =>
      {
        RequestedOrientation = ScreenOrientation.Landscape;
      });

      global::Xamarin.Forms.MessagingCenter.Subscribe<Jammit.Forms.Views.SongPage>(this, "AllowLandScapePortrait", sender =>
      {
        RequestedOrientation = ScreenOrientation.Unspecified;
      });

      LoadApplication(new Jammit.Forms.App());
    }
  }
}
