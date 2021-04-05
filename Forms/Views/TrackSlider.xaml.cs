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

    bool _canReadMutedSetting = true;

    void Mute()
    {
      Player.SetVolume(Track, 0);
      _state = State.Muted;

      //TODO: Ewww! Use styles and binding instead.
      MuteButton.TextColor = MutedButtonTextColor;
      MuteButton.BackgroundColor = MutedButtonBackgroundColor;
    }

    void Unmute()
    {
      Player.SetVolume(Track, (uint)Volume);
      _state = State.Normal;

      //TODO: Ewww! Use styles and binding instead.
      MuteButton.TextColor = NormalButtonTextColor;
      MuteButton.BackgroundColor = NormalButtonBackgroundColor;
    }

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

    #region base overrides

    protected override void OnPropertyChanged(string propertyName = null)
    {
      base.OnPropertyChanged(propertyName);

      if (TrackProperty.PropertyName == propertyName)
      {
        //TODO: Bind!
        TitleLabel.Text = Track.Title;

        if (_canReadMutedSetting && Player != null && Settings.IsTrackMuted(Track))
        {
          _canReadMutedSetting = false;
          Mute();
        }
      }
      else if (PlayerProperty.PropertyName == propertyName)
      {
        if (_canReadMutedSetting && Track != null && Settings.IsTrackMuted(Track))
        {
          _canReadMutedSetting = false;
          Mute();
        }
      }
    }

    #endregion base overrides

    #region Events

    private void VolumeSlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
      //TODO: How about setting the track volume right here and drop the Volume property?
      if (State.Muted != _state)
      {
        Player.SetVolume(Track, (uint)e.NewValue);

        Settings.Set(Settings.TrackVolumeKey(Track), (uint)e.NewValue);
      }
    }

    private void MuteButton_Clicked(object sender, EventArgs e)
    {
      if (State.Muted == _state)
      {
        Unmute();

        Settings.SetTrackMuted(Track, false);
      }
      else
      {
        Mute();

        Settings.SetTrackMuted(Track, true);
      }
    }

    #endregion Events
  }
}
