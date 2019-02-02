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
  public partial class SongPage : ContentPage
  {
    public SongPage(SongInfo song)
    {
      // Needed to actually bind local properties.
      BindingContext = this;

      Song = song;
      Media = App.MediaLoader.LoadMedia(song);
      Player = App.PlayerFactory(Media);
      PageIndex = 0;

      InitializeComponent();

      //TODO: Move to XAML?
      //TODO: Why is it still needed?
      PositionSlider.SetBinding(
        Slider.ValueProperty,
        new Binding(
          nameof(Player.Position),
          BindingMode.TwoWay));

      Player.PositionChanged += (player, args) =>
      {
        var newPosition = (player as Audio.IJcfPlayer).Position;
        if (newPosition.TotalSeconds != PositionSlider.Value)
          PositionSlider.Value = newPosition.TotalSeconds;
      };

      //TODO: Should be set in binding.
      ScorePicker.SelectedIndex = 0;

      AlbumImage.Source = ImageSource.FromStream(() => { return App.MediaLoader.LoadAlbumCover(Media); });
    }

    #region Properties

    public SongInfo Song { get; set; }

    public Model.JcfMedia Media { get; set; }

    public Audio.IJcfPlayer Player { get; private set; }

    public static BindableProperty PageIndexProperty =
      BindableProperty.Create("PageIndex", typeof(uint), typeof(uint), (uint)0);

    public uint PageIndex
    {
      get
      {
        return (uint)GetValue(PageIndexProperty);
      }

      private set
      {
        SetValue(PageIndexProperty, value);
      }
    }

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

    void PositionSlider_ValueChanged(object sender, Xamarin.Forms.ValueChangedEventArgs e)
    {
      // Update the player position only on manual ( > 1 ) slider changes.
      if (Math.Abs(e.NewValue - Player.Position.TotalSeconds) > 1)
        Player.Position = TimeSpan.FromSeconds(e.NewValue);
    }

    private void ScorePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
      SetScorePage(PageIndex);
    }

    #endregion //Handlers

    //TODO: Re-enable.
    private async void ScoreLayout_Scrolled(object sender, ScrolledEventArgs e)
    {
      var score = ScorePicker.SelectedItem as ScoreInfo;
      double targetY = -1;

      //TODO: Compute score height. Might change due to resizing.
      if (PageIndex < score.PageCount - 1 && ScoreLayout.Height + e.ScrollY >= ScoreImage.Height)
      {
        PageIndex++;
        targetY = ScoreImage.Height - ScoreLayout.Height;
      }
      else if (PageIndex > 0 && e.ScrollY <= 0)
      {
        PageIndex--;
        targetY = 0;
      }

      // If PageIndex changed.
      if (targetY >= 0)
      {
        SetScorePage(PageIndex);

        //TODO: Meh. Doen't really work (at least on UWP).
        await ScoreLayout.ScrollToAsync(e.ScrollX, targetY, true);
      }
    }

    void SetScorePage(uint index)
    {
      var score = ScorePicker.SelectedItem as ScoreInfo;
      if (index < 0 || index >= score.PageCount)
        return;

      PageIndex = index;
      ScoreImage.Source = ImageSource.FromStream(() =>
      {
        return App.MediaLoader.LoadNotation(Media, score, index);
      });
    }

    private void BackButton_Clicked(object sender, EventArgs e)
    {
      SetScorePage(PageIndex - 1);
    }

    private void ForwardButton_Clicked(object sender, EventArgs e)
    {
      SetScorePage(PageIndex + 1);
    }

    private void StartButton_Clicked(object sender, EventArgs e)
    {
      SetScorePage(0);
    }

    private void EndButton_Clicked(object sender, EventArgs e)
    {
      SetScorePage((ScorePicker.SelectedItem as ScoreInfo).PageCount - 1);
    }
  }
}