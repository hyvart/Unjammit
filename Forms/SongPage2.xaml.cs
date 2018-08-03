using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jammit.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Jammit.Forms
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class SongPage2 : ContentPage
  {
    public SongPage2 (SongInfo song)
    {
      // Needed to actually bind local properties.
      BindingContext = this;

      Song = song;
      Media = App.MediaLoader.LoadMedia(song);
      Player = App.PlayerFactory(Media);

      InitializeComponent();

      AlbumImage.Source = ImageSource.FromStream(() => { return App.MediaLoader.LoadAlbumCover(Media); });
      ScoreImage.Source = ImageSource.FromStream(() => { return App.MediaLoader.LoadNotation(Media, Media.Scores[0], 0); });
    }

    #region Properties

    public SongInfo Song { get; set; }

    public Model.JcfMedia Media { get; set; }

    public Audio.IJcfPlayer Player { get; private set; }

    #endregion

    #region Handlers

    private void PlayButton_Clicked(object sender, EventArgs e)
    {
      if (Audio.PlaybackStatus.Playing == Player.State)
      {
        Player.Stop();
        PlayButton.Text = "Play";
      }
      else
      {
        Player.Play();
        PlayButton.Text = "Stop";
      }
    }

    private void CloseButton_Clicked(object sender, EventArgs e)
    {
      Navigation.PopModalAsync();
    }

    #endregion //Handlers
  }
}