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

    public SongInfo Song { get; set; }

    public Model2.JcfMedia Media { get; set; }

  }
}