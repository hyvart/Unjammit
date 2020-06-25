using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Jammit.Model;
using Jammit.Forms.Resources;

namespace Jammit.Forms.Views
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class CatalogPage : ContentPage
  {
    public static List<SongInfo> Catalog { get; private set; }

    public CatalogPage()
    {
      InitializeComponent();
    }

    #region Page overrides

    protected override void OnAppearing()
    {
      base.OnAppearing();

      Device.BeginInvokeOnMainThread(async () => await LoadCatalog());
    }

    #endregion Page overrides

    private async Task LoadCatalog()
    {
      try
      {
        Catalog = await App.Client.LoadCatalog();

        AuthPopup.IsVisible = false;
      }
      catch (System.Net.Http.HttpRequestException ex)
      {
        Catalog = default;
        await DisplayAlert(Localized.CatalogPage_LoadCatalogCatchTitle, ex.Message, Localized.CatalogPage_LoadCatalogCatchCancel);

        if (App.Client.AuthStatus == Jammit.Client.AuthorizationStatus.Rejected)
        {
          Settings.Credentials = default;
          AuthPopup.IsVisible = true;
        }
      }
      catch (Exception ex)
      {
        Catalog = default;
        await DisplayAlert(Localized.CatalogPage_LoadCatalogCatchTitle, ex.Message, Localized.CatalogPage_LoadCatalogCatchCancel);
      }

      //TODO: Move back into XAML bindings.
      CatalogView.ItemsSource = Catalog;
    }

    private async void LoadButton_Clicked(object sender, EventArgs e)
    {
      await LoadCatalog();
    }

    private async void DownloadButton_Clicked(object sender, EventArgs e)
    {
      if (null == CatalogView.SelectedItem)
        return;

      // Download song
      var selectedSong = CatalogView.SelectedItem as SongInfo;
      try
      {
        // Make sure Downloads directory exists.
        var downloadsDir = System.IO.Directory.CreateDirectory(System.IO.Path.Combine(App.DataDirectory, "Downloads"));
        var zipPath = System.IO.Path.Combine(downloadsDir.FullName, selectedSong.Id.ToString().ToUpper() + ".zip");

        await App.Client.DownloadSong(selectedSong, zipPath);
        var downloadedStream = System.IO.File.OpenRead(zipPath);
        var song = App.Library.AddSong(downloadedStream);
        System.IO.File.Delete(zipPath); // Delete downloaded file.

        //TODO: Assert selected item and downloaded content metadata are equal.

        await DisplayAlert(Localized.CatalogPage_DownloadButtonClicked_AlertTitle, song.ToString(), "OK");

        DownloadProgressBar.Progress = 0;
      }
      catch (Exception ex)
      {
        await DisplayAlert(Localized.CatalogPage_DownloadButtonClicked_CatchTitle, string.Format(Localized.CatalogPage_DownloadButtonClicked_CatchMessage, selectedSong.Id), "OK");
      }
    }

    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
      CatalogView.BeginRefresh();

      if (string.IsNullOrWhiteSpace(e.NewTextValue))
        CatalogView.ItemsSource = Catalog;
      else
        CatalogView.ItemsSource = Catalog.Where(
          s => s.Title.IndexOf(e.NewTextValue, StringComparison.OrdinalIgnoreCase) >= 0 ||
          s.Artist.IndexOf(e.NewTextValue, StringComparison.OrdinalIgnoreCase) >= 0 ||
          s.Album.IndexOf(e.NewTextValue, StringComparison.OrdinalIgnoreCase) >= 0
        );

      CatalogView.EndRefresh();
    }

    private async void AuthButton_Clicked(object sender, EventArgs e)
    {
      Settings.Credentials = AuthUser.Text + ':' + AuthPassword.Text;

      await LoadCatalog();
    }
  }
}
