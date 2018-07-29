using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

using Jammit.Model;

namespace Jammit.Portable
{
  public partial class MainPage : ContentPage
  {
    public MainPage()
    {
      InitializeComponent();

      this.FilesPath.Text = Xamarin.Essentials.FileSystem.AppDataDirectory;
    }

    [Obsolete]
    public static string DeviceId => Plugin.DeviceInfo.CrossDeviceInfo.Current.Id;

    [Obsolete]
    public static string DevicePlatform => Xamarin.Essentials.DeviceInfo.Platform;

    private void LibraryView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
      //Navigation.PushModalAsync(new SongPage(e.Item as SongInfo));
      Navigation.PushModalAsync(new SongPage2(e.Item as SongInfo));
    }

    private void CatalogButton_Clicked(object sender, EventArgs e)
    {
      Navigation.PushModalAsync(new CatalogPage());
    }

    private void LibraryItem_Delete(object sender, EventArgs e)
    {
      var song = (sender as MenuItem).BindingContext as SongInfo;
      App.Library.RemoveSong(song.Id);
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
