using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
      var mdi = "";
      var version = "";
      if (Device.GTK != Device.RuntimePlatform)
      {
        mdi = Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.ToString();
        version = Xamarin.Essentials.VersionTracking.CurrentVersion;
      }
      else
      {
        mdi = "Desktop";
        version = "1.0";
      }
      await DisplayAlert("Info", $"Unjammit! Version [{version}]\nDisplayInfo: [{mdi}]", "OK");
    }
  }
}
