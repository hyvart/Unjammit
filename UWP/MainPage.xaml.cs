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

using PCLStorage;

namespace Jammit.UWP
{
  public sealed partial class MainPage
  {
    public MainPage()
    {
      this.InitializeComponent();

      // Defer Loading application until VLC MediaElement is rendered.
      this.Loaded += (sender, e) =>
      {
        LoadApplication(
          new Jammit.Forms.App(
            FileSystem.Current,
            (m) => { return new Audio.VlcJcfPlayer(m, MediaElement3, new VLC.MediaElement[] { MediaElement1, MediaElement2 }); },
            new Jammit.Model.FileSystemJcfLoader(Xamarin.Essentials.FileSystem.AppDataDirectory)
          )
        );
      };
    }
  }
}
