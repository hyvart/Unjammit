﻿using System;
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
      //TODO: Remove once RadioButton is promoted from Experimental.
      global::Xamarin.Forms.Forms.SetFlags("RadioButton_Experimental");

      global::Xamarin.Forms.Forms.Init();

      // Audio options
      NSError error = AVFoundation.AVAudioSession.SharedInstance().SetCategory(AVFoundation.AVAudioSessionCategory.Playback);
      Jammit.Forms.App.AllowedFileTypes = new string[] { "com.pkware.zip-archive" };
      Jammit.Forms.App.DataDirectory = Xamarin.Essentials.FileSystem.AppDataDirectory;
      Jammit.Forms.App.PlayerFactory = async (media) => await System.Threading.Tasks.Task.Run(() =>
      {
        return new Audio.AppleJcfPlayer(media, (track, stream) =>
        {
          return new Audio.IOSAVAudioPlayer(track, stream);
        });
      });
      Jammit.Forms.App.MediaLoader = new Model.FileSystemJcfLoader(Xamarin.Essentials.FileSystem.AppDataDirectory);

      LoadApplication(new Jammit.Forms.App());

      return base.FinishedLaunching(app, options);
    }
  }
}
