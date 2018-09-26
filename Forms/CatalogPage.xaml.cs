using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Jammit.Model;

namespace Jammit.Forms
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class CatalogPage : ContentPage
  {
    public static List<SongInfo> Catalog { get; private set; }

    public CatalogPage()
    {
      InitializeComponent();

      Task.Run(async () => await LoadCatalog()).Wait();

      //TODO: Move back into XAML bindings.
      this.CatalogView.ItemsSource = Catalog;
    }

    private async Task LoadCatalog()
    {
      Catalog = await App.Client.LoadCatalog();
    }

    private async void LoadButton_Clicked(object sender, EventArgs e)
    {
      await LoadCatalog();
    }

    private void CloseButton_Clicked(object sender, EventArgs e)
    {
      Navigation.PopModalAsync();
    }

    private void DownloadButton_Clicked(object sender, EventArgs e)
    {
      if (null == CatalogView.SelectedItem)
        return;

      Task.Run(async() => await App.Library.AddSong(CatalogView.SelectedItem as SongInfo));
    }
  }
}