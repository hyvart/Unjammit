using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Jammit.UWP
{
  public sealed partial class MainPage
  {
    public MainPage()
    {
      this.InitializeComponent();

      LoadApplication(
        new Jammit.Forms.App(
          Xamarin.Essentials.FileSystem.AppDataDirectory,
          (media) => { return new Audio.FFmpegJcfPlayer(media); },
          new Model.FileSystemJcfLoader(Xamarin.Essentials.FileSystem.AppDataDirectory)
        )
      );
    }
  }
}
