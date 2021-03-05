using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace Jammit.iOS
{
  // The UIApplicationDelegate for the application. This class is responsible for launching the 
  // User Interface of the application, as well as listening (and optionally responding) to 
  // application events from iOS.
  [Register("AppDelegate")]
  public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
  {
    //
    // This method is invoked when the application has loaded and is ready to run. In this 
    // method you should instantiate the window, load the UI into it and then make the window
    // visible.
    //
    // You have 17 seconds to return from this method, or iOS will terminate your application.
    //
    public override bool FinishedLaunching(UIApplication app, NSDictionary options)
    {
      global::Xamarin.Forms.Forms.Init();

      // Audio options
      NSError error = AVFoundation.AVAudioSession.SharedInstance().SetCategory(AVFoundation.AVAudioSessionCategory.Playback);
      Jammit.Forms.App.AllowedFileTypes = new string[] { "com.pkware.zip-archive" };
      Jammit.Forms.App.DataDirectory = Xamarin.Essentials.FileSystem.AppDataDirectory;

#if false
      Jammit.Forms.App.PlayerFactory = async (media) => await System.Threading.Tasks.Task.Run(() =>
      {
        return new Audio.AppleJcfPlayer(media, (track, stream) =>
        {
          return new Audio.IOSAVAudioPlayer(track, stream);
        });
      });
#else
      Jammit.Forms.App.PlayerFactory = async (media) => await System.Threading.Tasks.Task.Run(() =>
      {
        var player = new Audio.NAudioJcfPlayer(
          media,
          new Audio.AVAudioWavePlayer() { DesiredLatency = 60, NumberOfBuffers = 2 },
          System.IO.Path.Combine(Xamarin.Essentials.FileSystem.AppDataDirectory, "Tracks"),
          Forms.Resources.Assets.Stick);

        player.TimerAction = () =>
        {
          Xamarin.Forms.Device.StartTimer(new System.TimeSpan(0, 0, 1), () =>
          {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() => player.NotifyPositionChanged());

            return player.State == Audio.PlaybackStatus.Playing;
          });
        };

        return player;
      });
#endif

      Jammit.Forms.App.MediaLoader = new Model.FileSystemJcfLoader(Xamarin.Essentials.FileSystem.AppDataDirectory);

      LoadApplication(new Jammit.Forms.App());

      return base.FinishedLaunching(app, options);
    }
  }
}
