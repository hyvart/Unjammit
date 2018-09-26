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

      this.FilesPath.Text = App.DataDirectory;

      if (Plugin.DeviceInfo.Abstractions.Platform.macOS != Plugin.DeviceInfo.CrossDeviceInfo.Current.Platform)
        this.VersionLabel.Text = "Version " + Xamarin.Essentials.VersionTracking.CurrentVersion;
      else
        this.VersionLabel.Text = "Version ?";
    }

    [Obsolete]
    public static string DeviceId => Plugin.DeviceInfo.CrossDeviceInfo.Current.Id;

    [Obsolete]
    //public static string DevicePlatform => Xamarin.Essentials.DeviceInfo.Platform; // Not Apple-ready.
    public static string DevicePlatform => Plugin.DeviceInfo.CrossDeviceInfo.Current.Platform.ToString();

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

    #region Events

    protected override void OnAppearing()
    {
      base.OnAppearing();
    }

    #endregion // Events
  }
}
