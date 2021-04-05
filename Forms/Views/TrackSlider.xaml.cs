using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Jammit.Forms.Views
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class TrackSlider : ContentView
  {
    #region static members

    static Color NormalButtonTextColor;
    static Color NormalButtonBackgroundColor;
    static Color MutedButtonTextColor = Color.DarkRed;
    static Color MutedButtonBackgroundColor = Color.LightPink;

    #endregion static members

    #region private members

    enum State
    {
      Normal,
      Muted,
      Solo
    }

    State _state;

    #endregion private members

    public TrackSlider()
    {
      _state = State.Normal;

      InitializeComponent();

      NormalButtonBackgroundColor = MuteButton.BackgroundColor;
      NormalButtonTextColor = MuteButton.TextColor;
    }

    #region Bindable properties

    //TODO: Analize keeping here.
    public static readonly BindableProperty PlayerProperty =
      BindableProperty.Create("Player", typeof(Audio.IJcfPlayer), typeof(Audio.IJcfPlayer));

    public static readonly BindableProperty TrackProperty =
      BindableProperty.Create("Track", typeof(Model.PlayableTrackInfo), typeof(Model.PlayableTrackInfo));

    #endregion Bindable properties

    #region Properties

    public Model.PlayableTrackInfo Track
    {
      get
      {
        return (Model.PlayableTrackInfo)GetValue(TrackProperty);
      }

      set
      {
        SetValue(TrackProperty, value);
      }
    }

    public double Volume
    {
      get => VolumeSlider.Value;
      set
      {
        VolumeSlider.Value = value;
      }
    }

    //TODO: Keep here?
    public Audio.IJcfPlayer Player
    {
      get
      {
        return (Audio.IJcfPlayer)GetValue(PlayerProperty);
      }

      set
      {
        SetValue(PlayerProperty, value);
      }
    }

    #endregion Properties

    #region Events

    protected override void OnPropertyChanged(string propertyName = null)
    {
      base.OnPropertyChanged(propertyName);

      if (TrackProperty.PropertyName == propertyName)
      {
        //TODO: Bind!
        TitleLabel.Text = Track.Title;
      }
    }

    #endregion Events

    private void VolumeSlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
      //TODO: How about setting the track volume right here and drop the Volume property?
      if (State.Muted != _state)
      {
        if (Track != null)
        {
          Player.SetVolume(Track, (uint)e.NewValue);

          Settings.Set(Settings.TrackVolumeKey(Track), (uint)e.NewValue);
        }
      }
    }

    private void MuteButton_Clicked(object sender, EventArgs e)
    {
      if (State.Muted == _state)
      {
        Volume = (uint)VolumeSlider.Value;
        _state = State.Normal;

        //TODO: Ewww! Use styles and binding instead.
        MuteButton.TextColor = NormalButtonTextColor;
        MuteButton.BackgroundColor = NormalButtonBackgroundColor;
      }
      else
      {
        Volume = 0;
        _state = State.Muted;

        Settings.SetTrackMuted(Track, true);

        //TODO: Ewww! Use styles and binding instead.
        MuteButton.TextColor = MutedButtonTextColor;
        MuteButton.BackgroundColor = MutedButtonBackgroundColor;
      }
    }
  }
}
