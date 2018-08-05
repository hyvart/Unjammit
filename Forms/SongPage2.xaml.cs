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

      PositionSlider.Maximum = Player.Length.TotalSeconds;//TODO: Bind!!!

      AlbumImage.Source = ImageSource.FromStream(() => { return App.MediaLoader.LoadAlbumCover(Media); });
    }

    // #region Binding properties

    // public static readonly BindableProperty PlayerProperty =
    //   BindableProperty.Create("Player", typeof(Audio.IJcfPlayer), typeof(Audio.IJcfPlayer));

    // #endregion // Binding properties

    #region Properties

    public SongInfo Song { get; set; }

    public Model.JcfMedia Media { get; set; }

    public Audio.IJcfPlayer Player { get; private set; }
    // public Audio.IJcfPlayer Player
    // {
    //   get
    //   {
    //     return (Audio.IJcfPlayer)GetValue(PlayerProperty);
    //   }

    //   private set
    //   {
    //     SetValue(PlayerProperty, value);
    //   }
    // }

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
      Player.Stop();

      Navigation.PopModalAsync();
    }

    #endregion //Handlers
  }
}