using System;
using System.Collections.Generic;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Localized = Jammit.Forms.Resources.Localized;

namespace Jammit.Forms.Views
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class MenuPage : ContentPage
  {
    public MenuPage()
    {
      InitializeComponent();

      //TODO: Bind (fails on Gtk, prints "Binding" on other platforms.
      if (Device.GTK == Device.RuntimePlatform)
        Title = "Menu";
      else
        Title = Localized.MenuPage_Title;
    }

    private async void OpenButton_Clicked(object sender, EventArgs e)
    {
      FileResult picked = null;
      try
      {
        picked = await FilePicker.PickAsync(new PickOptions
        {
          PickerTitle = "Select JCF archive",
          FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
          {
            { DevicePlatform.Android, new[] { "application/zip" } },
            { DevicePlatform.iOS, new[] { "com.pkware.zip-archive" } },
            { DevicePlatform.macOS, new[] { "com.pkware.zip-archive", "zip" } },
            { DevicePlatform.UWP, new[] { ".zip" } },
            { DevicePlatform.Unknown, new[] { ".zip" } }
          })
        });
        if (picked == null)
          return;

        var song = App.Library.AddSong(await picked.OpenReadAsync());

        await DisplayAlert(Localized.MenuPage_Import, song.ToString(), "OK");
      }
      catch (Exception)
      {
        var path = picked == null ? string.Empty : picked.FullPath;
        await DisplayAlert(Localized.MenuPage_ImportCatchTitle, string.Format(Localized.MenuPage_ImportCatch, path), "OK");
      }
    }

    private async void AboutButton_Clicked(object sender, EventArgs e)
    {
      string mdi;
      string version;
      if (Device.GTK != Device.RuntimePlatform)
      {
        //TODO: Log bug against Xamarin.Essentials (iOS 9.3.x)
        if (Device.RuntimePlatform == Device.iOS && DeviceInfo.Version.Major < 10)
          mdi = "Unknown";
        else
          mdi = DeviceDisplay.MainDisplayInfo.ToString();

        version = VersionTracking.CurrentVersion;
      }
      else
      {
        mdi = "Unknown";
        version = "Unknown";
      }

      await DisplayAlert("Info", $"Unjammit! Version [{version}]\nDisplayInfo: [{mdi}]", "OK");
    }
  }
}
