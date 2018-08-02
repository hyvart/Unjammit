using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jammit.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Jammit.Portable
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class SongPage2 : ContentPage
  {
    public SongPage2 (SongInfo song)
    {
      // Needed to actually bind local properties.
      BindingContext = this;

      Song = song;
      Media = App.MediaLoader.LoadMedia(song.Id);

      InitializeComponent();

      AlbumImage.Source = ImageSource.FromStream(() => { return App.MediaLoader.LoadAlbumCover(Media); });
      ScoreImage.Source = ImageSource.FromStream(() => { return App.MediaLoader.LoadNotation(Media, Media.Scores[0], 0); });
    }

    #region Properties

    public SongInfo Song { get; set; }

    public Model2.JcfMedia Media { get; set; }

    #endregion

    #region Handlers

    private void PlayButton_Clicked(object sender, EventArgs e)
    {

    }

    private void CloseButton_Clicked(object sender, EventArgs e)
    {
      Navigation.PopModalAsync();
    }

    #endregion //Handlers
  }
}