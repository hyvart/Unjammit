using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Jammit.Forms.Views
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class MenuPage : ContentPage
  {
    public MenuPage()
    {
      InitializeComponent();
    }

    #region Page overrides

    protected override void OnAppearing()
    {
    }

    #endregion  Page overrides

    private async void OpenButton_Clicked(object sender, EventArgs e)
    {
      Plugin.FilePicker.Abstractions.FileData picked = null;
      try
      {
        picked = await Plugin.FilePicker.CrossFilePicker.Current.PickFile(App.AllowedFileTypes);
        if (picked == null)
          return;

        var song = App.Library.AddSong(picked.GetStream());

        await DisplayAlert("Imported Song", song.ToString(), "OK");
      }
      catch (Exception ex)
      {
        await DisplayAlert("Error", $"Could not process file {picked.FilePath}.", "OK");
      }
    }

    private async void AboutButton_Clicked(object sender, EventArgs e)
    {
      var mdi = "";
      var version = "";
      if (Device.macOS != Device.RuntimePlatform && Device.GTK != Device.RuntimePlatform)
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
