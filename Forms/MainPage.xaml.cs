using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

using Jammit.Model;

namespace Jammit.Forms
{
  public partial class MainPage : ContentPage
  {
    public MainPage()
    {
      InitializeComponent();

      double pad;
      if (Plugin.DeviceInfo.Abstractions.Platform.macOS == Plugin.DeviceInfo.CrossDeviceInfo.Current.Platform)
      {
        pad = 250;
      }
      else
      {
        var screenWidth = Xamarin.Essentials.DeviceDisplay.ScreenMetrics.Width;
        var screenDensity = Xamarin.Essentials.DeviceDisplay.ScreenMetrics.Density;
        pad = screenWidth / screenDensity / 4;
      }

      var thickness = new Thickness(pad, 0, pad, 0);
      OpenButtonsLayout.Padding = thickness;
      SettingsButtonsLayout.Padding = thickness;
    }

    [Obsolete]
    public static string DeviceId => Plugin.DeviceInfo.CrossDeviceInfo.Current.Id;

    [Obsolete]
    //public static string DevicePlatform => Xamarin.Essentials.DeviceInfo.Platform; // Not macOS-ready.
    public static string DevicePlatform => Plugin.DeviceInfo.CrossDeviceInfo.Current.Platform.ToString();

    #region Events

    protected override void OnAppearing()
    {
      base.OnAppearing();
    }

    #endregion // Events

    private void LibraryView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
      Navigation.PushModalAsync(new SongPage(e.Item as SongInfo));
    }

    private async void CatalogButton_Clicked(object sender, EventArgs e)
    {
      try
      {
        await Navigation.PushModalAsync(new CatalogPage());
      }
      catch (Exception ex)
      {
        string message;
        if (ex.InnerException is UriFormatException)
          message = $"Invalid server address: [ {Settings.ServiceUri} ]";
        else if (ex.InnerException != null)
          message = ex.InnerException.Message;
        else
          message = ex.Message;

        await DisplayAlert("Error", message, "Cancel");
      }
    }

    private void LibraryItem_Delete(object sender, EventArgs e)
    {
      var song = (sender as MenuItem).BindingContext as SongInfo;
      App.Library.RemoveSong(song);
    }

    private void SettingsButton_Clicked(object sender, EventArgs e)
    {
      Navigation.PushModalAsync(new SettingsPage());
    }

    private async void OpenButton_Clicked(object sender, EventArgs e)
    {
      Plugin.FilePicker.Abstractions.FileData picked = null;
      try
      {
        picked = await Plugin.FilePicker.CrossFilePicker.Current.PickFile( App.AllowedFileTypes );
        if (picked == null)
          return;

        var song =  App.Library.AddSong(picked.GetStream());

        await DisplayAlert("Imported Song", song.ToString(), "OK");
      }
      catch (Exception ex)
      {
        await DisplayAlert("Error", $"Could not process file {picked.FilePath}.", "OK");
      }
    }

    private async void AboutButton_Clicked(object sender, EventArgs e)
    {
      await DisplayAlert("Info", $"Unjammit! Version 0.1.x", "OK");
    }
  }
}