using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jammit.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Jammit.Forms.Views
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class SongPage : ContentPage
  {
    #region static members

    static Color NormalButtonTextColor;
    static Color NormalButtonBackgroundColor;

    #endregion static members

    #region private fields

    private int _beatIndex;

    #endregion private fields

    public SongPage(SongInfo song)
    {
      // Needed to actually bind local properties.
      BindingContext = this;

      Song = song;
      Media = App.MediaLoader.LoadMedia(song);
      Player = App.PlayerFactory(Media);
      PageIndex = 0;

      InitializeComponent();

      NormalButtonBackgroundColor = PlayButton.BackgroundColor;
      NormalButtonTextColor = PlayButton.TextColor;

      if (Device.Android == Device.RuntimePlatform)
      {
        RepeatButton.MinimumWidthRequest = 3;
        StartButton.MinimumWidthRequest = 3;
        BackButton.MinimumWidthRequest = 3;
        PlayButton.MinimumWidthRequest = 3;
        StopButton.MinimumWidthRequest = 3;
        ForwardButton.MinimumWidthRequest = 3;
        EndButton.MinimumWidthRequest = 3;

        RepeatButton.MinimumHeightRequest = 3;
        StartButton.MinimumHeightRequest = 3;
        BackButton.MinimumHeightRequest = 3;
        PlayButton.MinimumHeightRequest = 3;
        StopButton.MinimumHeightRequest = 3;
        ForwardButton.MinimumHeightRequest = 3;
        EndButton.MinimumHeightRequest = 3;
      }

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

      //TODO: Fix UI proportions on Android.
      if (Device.Android == Device.RuntimePlatform)
        AlbumImage.IsVisible = false;
      else
        AlbumImage.Source = ImageSource.FromStream(() => { return App.MediaLoader.LoadAlbumCover(Media); });

      _beatIndex = 0;
    }

    #region Page overrides

    protected override void OnAppearing()
    {
      base.OnAppearing();

      if (null == ScorePicker.SelectedItem)
        return;

      var track = (ScorePicker.SelectedItem as ScoreInfo).Track;
      var h = track.ScoreSystemHeight - track.ScoreSystemInterval;
      CursorFrame.HeightRequest = h;
      CursorBar.HeightRequest = h;

      var y = -track.ScoreSystemInterval / 4;
        //(track.ScoreSystemInterval / 4) /*+ track.ScoreSystemInterval*/ + (track.ScoreSystemHeight - track.ScoreSystemInterval);
      CursorFrame.TranslationY = y;
      CursorBar.TranslationY = y;
    }

    #endregion Page overrides

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
        Player.Pause();

        PlayButton.BackgroundColor = Color.PaleGoldenrod;
        PlayButton.TextColor = Color.Olive;
        PlayButton.BorderColor = PlayButton.TextColor;
      }
      else
      {
        Player.Play();

        Device.StartTimer(TimeSpan.FromMilliseconds(30), () =>
        {
          MoveCursor(Player.Position);

          return Player.State == Audio.PlaybackStatus.Playing;// && Player.Position < Player.Length;
        });

        PlayButton.BackgroundColor = Color.PaleGreen;
        PlayButton.TextColor = Color.DarkGreen;
        PlayButton.BorderColor = PlayButton.TextColor;
      }
    }

    private void StopButton_Clicked(object sender, EventArgs e)
    {
      Player.Stop();

      PlayButton.BackgroundColor = NormalButtonBackgroundColor;
      PlayButton.TextColor = NormalButtonTextColor;
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

    #endregion Handlers

    private void FindBeat(double totalSeconds, int start, int end)
    {
      int mid = (start + end) / 2;
      if (mid == start)
      {
        _beatIndex = mid;
      }
      else if (Media.Beats[mid].Time < totalSeconds)
      {
        FindBeat(totalSeconds, mid, end);
      }
      else if (Media.Beats[mid].Time > totalSeconds)
      {
        // If [mid] is the very next major element, finish.
        if (Media.Beats[mid - 1].Time <= totalSeconds)
        {
          _beatIndex = mid - 1;
          return;
        }

        FindBeat(totalSeconds, start, mid);
      }
      else
      {
        // Unlikely, double equality.
        _beatIndex = mid;
      }
    }

    private void MoveCursor(TimeSpan position)
    {
#if false
      //TODO: EWWW! Use FindBeat instead!
      for (int i = 0; i < Media.Beats.Count - 1; i++)
      {
        if (Media.Beats[i + 1].Time > position.TotalSeconds)
        {
          _beatIndex = i;
          break;
        }
      }
      //_beatIndex = Media.Beats.Count - 1;
#else
      FindBeat(position.TotalSeconds, 0, Media.Beats.Count);
#endif
      var track = (ScorePicker.SelectedItem as ScoreInfo).Track;
      var nodes = Media.ScoreNodes[track].Nodes;
      CursorBar.TranslationX = nodes[_beatIndex].X;

      //var yOffset = (track.ScoreSystemInterval / 4) + track.ScoreSystemInterval * (nodes[_beatIndex].Row) + (track.ScoreSystemHeight - track.ScoreSystemInterval);
      //CursorFrame.TranslationY = yOffset;
      //CursorBar.TranslationY = yOffset;

      BeatLabel.Text =
        $"P: {position}\n" +
        $"S: {Player.State}\n" +
        $"X: {nodes[_beatIndex].X}\n" +
        $"R: {nodes[_beatIndex].Row}\n" +
        $"M: {nodes[_beatIndex].Measure}\n" +
        $"TX: {CursorBar.TranslationX}\n" +
        $"TY: {CursorBar.TranslationY}\n" +
        $"Idx:{_beatIndex}\n" +
        $"BT: {Media.Beats[_beatIndex].Time}\n";
    }

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
