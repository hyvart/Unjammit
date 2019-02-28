using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using LibVLCSharp.Shared;

namespace Jammit.Android
{
  [Activity(Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
  public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
  {
    LibVLC _libVLC;
    LibVLCSharp.Platforms.Android.VideoView _videoView;
    MediaPlayer _mediaPlayer;

    protected override void OnCreate(Bundle savedInstanceState)
    {
      TabLayoutResource = Resource.Layout.Tabbar;
      ToolbarResource = Resource.Layout.Toolbar;

      base.OnCreate(savedInstanceState);
      global::LibVLCSharp.Forms.Shared.LibVLCSharpFormsRenderer.Init();
      global::LibVLCSharp.Shared.Core.Initialize();

      _libVLC = new LibVLC();
      _mediaPlayer = new MediaPlayer(_libVLC);
      _videoView = new LibVLCSharp.Platforms.Android.VideoView(this) { MediaPlayer = _mediaPlayer };

      Jammit.Forms.App.DataDirectory = Xamarin.Essentials.FileSystem.AppDataDirectory;
      Jammit.Forms.App.PlayerFactory = (media) => { return new Audio.AndroidMediaJcfPlayer(media); };
      Jammit.Forms.App.MediaLoader = new Model.FileSystemJcfLoader(Xamarin.Essentials.FileSystem.AppDataDirectory);

      global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

      LoadApplication(new Jammit.Forms.App());
    }
  }
}