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
    protected override void OnCreate(Bundle savedInstanceState)
    {
      TabLayoutResource = Resource.Layout.Tabbar;
      ToolbarResource = Resource.Layout.Toolbar;

      base.OnCreate(savedInstanceState);
      global::LibVLCSharp.Shared.Core.Initialize();

      var config = new MediaConfiguration();
      config.EnableHardwareDecoding = true;

      Jammit.Forms.App.DataDirectory = Xamarin.Essentials.FileSystem.AppDataDirectory;
      Jammit.Forms.App.PlayerFactory = (media) => new Audio.VlcJcfPlayer(media, new MediaConfiguration[]{ config });
      Jammit.Forms.App.MediaLoader = new Model.FileSystemJcfLoader(Xamarin.Essentials.FileSystem.AppDataDirectory);

      global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

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