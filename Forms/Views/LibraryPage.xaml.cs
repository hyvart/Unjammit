using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

using Jammit.Model;

namespace Jammit.Forms.Views
{
  public partial class LibraryPage : ContentPage
  {
    public LibraryPage()
    {
      InitializeComponent();

      //TODO: Defining a header throws a NullReferenceExeption in macOS starting with Xamarin.Forms 3.3.
      if (Plugin.DeviceInfo.Abstractions.Platform.macOS == Plugin.DeviceInfo.CrossDeviceInfo.Current.Platform)
        LibraryView.Header = null;
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
      var mdi = "";
      var version = "";
      if (Plugin.DeviceInfo.Abstractions.Platform.macOS != Plugin.DeviceInfo.CrossDeviceInfo.Current.Platform)
      {
        mdi = Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.ToString();
        version = Xamarin.Essentials.VersionTracking.CurrentVersion;
      }
      await DisplayAlert("Info", $"Unjammit! Version [{version}]\nDisplayInfo: [{mdi}]", "OK");
    }

    void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
      LibraryView.BeginRefresh();

      if (string.IsNullOrWhiteSpace(e.NewTextValue))
        LibraryView.ItemsSource = App.Library.Songs;
      else
        LibraryView.ItemsSource = App.Library.Songs.Where(
          s => s.Title.IndexOf(e.NewTextValue, StringComparison.OrdinalIgnoreCase) >= 0 ||
          s.Artist.IndexOf(e.NewTextValue, StringComparison.OrdinalIgnoreCase) >= 0 ||
          s.Album.IndexOf(e.NewTextValue, StringComparison.OrdinalIgnoreCase) >= 0
        );

      LibraryView.EndRefresh();
    }
  }
}