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

    private async void OpenButton_Clicked(object sender, EventArgs e)
    {
      Plugin.FilePicker.Abstractions.FileData picked = null;
      try
      {
        picked = await Plugin.FilePicker.CrossFilePicker.Current.PickFile(new string[] { ".zip" });
        if (picked == null)
          return;
        //TODO: Create FileSystemClient. Analogous to RestClient.

        // Get song ID.
        var id = Guid.Parse(System.IO.Path.GetFileNameWithoutExtension(picked.FilePath));

        // Get song info.
        using(var archive = new System.IO.Compression.ZipArchive(picked.GetStream(), System.IO.Compression.ZipArchiveMode.Read))
        {
          var idUp = id.ToString().ToUpper();
          var entry = archive.GetEntry($"{idUp}.jcf/info.plist");
          var dict = Claunia.PropertyList.PropertyListParser.Parse(entry.Open()) as Claunia.PropertyList.NSDictionary;

          var song = new SongInfo()
          {
            Id = id,
            Artist = dict.String("artist"),
            Album = dict.String("album"),
            Title = dict.String("title"),
            Genre = dict.String("genre")
          };
          switch(dict.Int("instrument"))
          {
            case 0:
              song.Instrument = "Guitar"; break;
            case 1:
              song.Instrument = "Bass"; break;
            case 2:
              song.Instrument = "Drums"; break;
            case 3:
              song.Instrument = "Keyboard"; break;
            case 4:
              song.Instrument = "Vocals"; break;
          }

          //TODO: Have library read file. For now, just show song data.
          await DisplayAlert("Opened Song", $"{song.Artist} - {song.Title} [{song.Instrument}]", "OK");
        }

      }
      catch (Exception ex)
      {
        await DisplayAlert("Error", $"Could not process file {picked.FilePath}.", "OK");
      }
    }

    #region Events

    protected override void OnAppearing()
    {
      base.OnAppearing();
    }

    #endregion // Events
  }
}
