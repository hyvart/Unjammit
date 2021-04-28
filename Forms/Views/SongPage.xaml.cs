using System;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Jammit.Model;

namespace Jammit.Forms.Views
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class SongPage : ContentPage
  {
    #region static members

    public static async Task<SongPage> CreateAsync(SongInfo song)
    {
      var instance = new SongPage();

      instance.Song = song;
      instance.Media = App.MediaLoader.LoadMedia(song);
      instance.Player = await App.PlayerFactory(instance.Media);
      instance.Player.PositionChanged += instance.Player_PositionChanged;

      instance.InitializeComponent();

      //TODO: Maybe set in binding.
      instance.ScoreSelector.Items = instance.Media.Scores;

      //TODO: Use themes!
      NormalButtonBackgroundColor = instance.PlayButton.BackgroundColor;
      NormalButtonTextColor = instance.PlayButton.TextColor;

      instance.CloseButton.Text = "⬅️" + instance.CloseButton.Text;

      return instance;
    }

    static Color NormalButtonTextColor;
    static Color NormalButtonBackgroundColor;

    #endregion static members

    #region private fields

    private int _beatIndex;
    int _sectionIndex;

    /// <summary>
    /// Platforms can vary their Player/Tracker uninitializing order.
    /// Tracking this will ensure the correct non-zero value is persisted when disappearing/closing.
    /// </summary>
    TimeSpan _lastPosition = TimeSpan.MinValue;

    #endregion private fields

    public SongPage()
    {
      // Needed to actually bind local properties.
      BindingContext = this;

      _beatIndex = 0;
      PageIndex = 0;
    }

    #region Page overrides

    protected override void OnAppearing()
    {
      base.OnAppearing();

      if (Device.Android == Device.RuntimePlatform && TargetIdiom.Phone == Device.Idiom)
        MessagingCenter.Send(this, "PreventPortrait");

      if (null == ScoreSelector.SelectedScore)
        return;

      var track = ScoreSelector.SelectedScore.Track;
      var h = track.ScoreSystemHeight * .775;
      CursorFrame.HeightRequest = h;
      CursorBar.HeightRequest = h;

      if (AlbumImage.IsVisible)
        AlbumImage.Source = ImageSource.FromStream(() => { return App.MediaLoader.LoadAlbumCover(Media); });
    }

    protected override void OnSizeAllocated(double width, double height)
    {
      base.OnSizeAllocated(width, height);

      // Page Width shoud be greater or equal. Else, there is children overflow.
      AlbumImageLayout.IsVisible = Width >= MixerLayout.Width + ProgressLayout.Width;

      // Adjust ScoreView, if needed.
      ScoreLayout_SizeChanged(null, null);

      var systemHeight = ScoreSelector.SelectedScore.Track.ScoreSystemHeight;
      ScoreContainer.HeightRequest = (double)Resources["ScoreHeight"] + systemHeight;
      ScoreImagePadLayout.HeightRequest = systemHeight;
    }

    protected override void OnDisappearing()
    {
      if (Device.Android == Device.RuntimePlatform && TargetIdiom.Phone == Device.Idiom)
        MessagingCenter.Send(this, "AllowLandScapePortrait");

      base.OnDisappearing();
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

    private void FindSection(int beatIndex, int start, int end)
    {
      int mid = (start + end) / 2;
      if (mid == start)
      {
        _sectionIndex = mid;
      }
      else if(Media.Sections[mid].BeatIdx < beatIndex)
      {
        FindSection(beatIndex, mid, end);
      }
      else if(Media.Sections[mid].BeatIdx > beatIndex)
      {
        FindSection(beatIndex, start, mid);
      }
      else
      {
        _sectionIndex = mid;
      }
    }

    private async Task MoveCursor(TimeSpan position)
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

      for (int i = 0; i  < Media.Sections.Count; i++)
      {
        if (Media.Sections[i].BeatIdx == _beatIndex)
        {
          _sectionIndex = i;
          break;
        }
      }
#else
      FindBeat(position.TotalSeconds, 0, Media.Beats.Count);
      FindSection(_beatIndex, 0, Media.Sections.Count);
#endif
      var track = ScoreSelector.SelectedScore.Track;
      var nodes = Media.ScoreNodes[track].Nodes;
      CursorBar.TranslationX = nodes[_beatIndex].X;

      var y = track.ScoreSystemInterval * (nodes[_beatIndex].Row);
      uint page = (uint)(y / ScoreImage.Height);
      if (page != PageIndex)
        SetScorePage(page);

      var yOffset = y % ScoreImage.Height;
      if (Device.macOS != Device.RuntimePlatform)
      {
        await ScoreView.ScrollToAsync(0, yOffset, false);
      }
      CursorFrame.TranslationY = yOffset;
      CursorBar.TranslationY = yOffset;

#if false
      TimelineImage.Text =
        $"P:  {position}\t"                     + $"S:  {Player.State}\n"           +
        $"BT: {Media.Beats[_beatIndex].Time}\n"                                     +
        $"\t"                                   + $"Id: {_beatIndex}\n" +
        $"X:  {nodes[_beatIndex].X}\t"          + $"R:  {nodes[_beatIndex].Row}\t"  + $"M: {nodes[_beatIndex].Measure}\n" +
        $"TX: {CursorBar.TranslationX}\t"       + $"TY: {CursorBar.TranslationY}\n" +
        $"\t"                                   + $"Pg: {page}"
        ;

      //TimelineImage.Text =
      //  $"H {Height}\n" +
      //  $"HLO.H {HeaderLayout.Height}\n" +
      //  $"SVW.H {ScoreView.Height}\n" +
      //  $"HCB.H {HideControlsButton.Height}\n" +
      //  $"CLO.H {ControlsLayout.Height}\n";
#else
      TimelineImage.Text = $"{Media.Sections[_sectionIndex].Name}\n\n\n\n\n";
#endif
    }

    void SetScorePage(uint index)
    {
      var score = ScoreSelector.SelectedScore;
      if (index < 0 || index >= score.PageCount)
        return;

      PageIndex = index;
      ScoreImage.Source = ImageSource.FromStream(() =>
      {
        return App.MediaLoader.LoadNotation(Media, score, index);
      });
      if (index + 1 < score.PageCount)
      {
        //TODO: Fix. Not working.
        //ScoreLayout.HeightRequest = 1024 + score.Track.ScoreSystemHeight;
        //ScoreImagePadLayout.HeightRequest = score.Track.ScoreSystemHeight;
        ScoreImagePad.Source = ImageSource.FromStream(() =>
        {
          return App.MediaLoader.LoadNotation(Media, score, index + 1);
        });
      }
    }

    //#region Handlers

    private void Player_PositionChanged(object sender, EventArgs e)
    {
      Device.BeginInvokeOnMainThread(() =>
      {
        var newPosition = (sender as Audio.IJcfPlayer).Position;
        if (newPosition.TotalSeconds != PositionSlider.Value)
        {
          if (newPosition == TimeSpan.Zero && _lastPosition != TimeSpan.Zero)
          {
            _lastPosition = TimeSpan.FromSeconds(PositionSlider.Value);
          }
          else
          {
            _lastPosition = TimeSpan.MinValue;
          }
          PositionSlider.Value = newPosition.TotalSeconds;
        }
      });
    }

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
          Device.BeginInvokeOnMainThread(async () => await MoveCursor(Player.Position));

          return Player.State == Audio.PlaybackStatus.Playing;
        });

        PlayButton.BackgroundColor = Color.PaleGreen;
        PlayButton.TextColor = Color.DarkGreen;
        PlayButton.BorderColor = PlayButton.TextColor;
      }
    }

    private void StopButton_Clicked(object sender, EventArgs e)
    {
      Player.Stop();

      // If Stop explicitly requested, ensure position is reset.
      _lastPosition = TimeSpan.Zero;

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

    private void BackButton_Clicked(object sender, EventArgs e)
    {
      for(int i = Media.Sections.Count-1; i >= 0; i--)
      {
        if (Media.Sections[i].BeatIdx < _beatIndex)
        {
          Player.Position = TimeSpan.FromSeconds(Media.Sections[i].Beat.Time);

          return;
        }
      }
    }

    private void ForwardButton_Clicked(object sender, EventArgs e)
    {
      for (int i = 0; i < Media.Sections.Count; i++)
      {
        if (Media.Sections[i].BeatIdx > _beatIndex)
        {
          Player.Position = TimeSpan.FromSeconds(Media.Sections[i].Beat.Time);

          return;
        }
      }
    }

    private void StartButton_Clicked(object sender, EventArgs e)
    {
      Player.Position = TimeSpan.FromSeconds(Media.Sections.First().Beat.Time);
    }

    private void EndButton_Clicked(object sender, EventArgs e)
    {
      Player.Position = TimeSpan.FromSeconds(Media.Sections.Last().Beat.Time);
    }

    private void HideControlsButton_Clicked(object sender, EventArgs e)
    {
      ControlsLayout.IsVisible = !ControlsLayout.IsVisible;
      HideControlsButton.Text = ControlsLayout.IsVisible? "⬇️" : "⬆️";
    }

    private void ScoreLayout_SizeChanged(object sender, EventArgs e)
    {
      // Hide score layout if it won't fit the screen.
      var systemHeight = ScoreSelector.SelectedScore.Track.ScoreSystemHeight;
      if (ScoreView.IsVisible && ScoreLayout.Height > 0 && ScoreLayout.Height < systemHeight)
      {
        ScoreView.IsVisible = false;
        ScoreHiddenLabel.IsVisible = true;
      }
      else if (!ScoreView.IsVisible && ScoreLayout.Height >= systemHeight)
      {
        ScoreHiddenLabel.IsVisible = false;
        ScoreView.IsVisible = true;
      }
    }

    private void ContentPage_Appearing(object sender, EventArgs e)
    {
      //TODO: Remove
      //ScorePicker.SelectedIndex = (int)Settings.Get(Settings.SelectedScoreKey(Song), 0);
      ControlsLayout.IsVisible = Settings.Get(Settings.MixerCollapsedKey(Song), true);
      PositionSlider.Value = Settings.Get(Settings.PositionKey(Song), TimeSpan.Zero).TotalSeconds;
    }

    private void ContentPage_Disappearing(object sender, EventArgs e)
    {
      //TODO: Remove
      //Settings.Set(Settings.SelectedScoreKey(Song), (uint)ScorePicker.SelectedIndex);
      Settings.Set(Settings.MixerCollapsedKey(Song), ControlsLayout.IsVisible);

      if (_lastPosition > TimeSpan.Zero)
        Settings.Set(Settings.PositionKey(Song), _lastPosition);
      else
        Settings.Set(Settings.PositionKey(Song), TimeSpan.FromSeconds(PositionSlider.Value));
      //TODO: TrackMuted
    }

    //TODO: Translate
    async void AlbumImage_Clicked(object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty(Song.Tempo))
        App.MediaLoader.LoadFullSongInfo(Song, Media.Path);

      var info = $"Song: {Song.Title}\n" +
        $"Performed by: {Song.Artist}\n" +
        $"Album: {Song.Album}\n";

      if (Song.Tunings != null && Song.Tunings.Count > 0)
      {
        info += "Tuning:\n";
        foreach (var tuning in Song.Tunings)
        {
          info += $"{tuning}\n";
        }
      }

      info +=
        $"Tempo: {Song.Tempo} BPM\n" +
        $"Written by: {Song.WrittenBy}\n" +
        $"Published by: {Song.PublishedBy}\n" +
        $"Used courtesy of: {Song.CourtesyOf}\n";

      await DisplayAlert("Song Info", info, "OK");
    }

    void ScoreSelector_SelectedScoreChanged(object sender, EventArgs e)
    {
      SetScorePage(PageIndex);
    }
  }
}
