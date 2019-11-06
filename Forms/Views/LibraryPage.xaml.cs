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
    }

    #region Events

    protected override void OnAppearing()
    {
      base.OnAppearing();
    }

    #endregion  Events

    private void LibraryView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
      Navigation.PushModalAsync(new SongPage(e.Item as SongInfo));
    }

    private void LibraryItem_Delete(object sender, EventArgs e)
    {
      var song = (sender as MenuItem).BindingContext as SongInfo;
      App.Library.RemoveSong(song);
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