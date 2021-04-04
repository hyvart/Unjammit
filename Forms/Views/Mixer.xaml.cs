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
  public partial class Mixer : ContentView
  {
    public Mixer()
    {
      InitializeComponent();
    }

    #region Bindable Properties

    public static readonly BindableProperty MediaProperty =
      BindableProperty.Create("Media", typeof(Model.JcfMedia), typeof(Model.JcfMedia));

    public static readonly BindableProperty PlayerProperty =
      BindableProperty.Create("Player", typeof(Audio.IJcfPlayer), typeof(Audio.IJcfPlayer));

    #endregion  Bindable Properties

    #region Properties

    public Model.JcfMedia Media
    {
      get
      {
        return (Model.JcfMedia)GetValue(MediaProperty);
      }

      set
      {
        SetValue(MediaProperty, value);
      }
    }

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

      if (MediaProperty.PropertyName == propertyName)
      {
        foreach (var track in Media.InstrumentTracks)
        {
          ControlsLayout.Children.Add(new TrackSlider
          {
            Track = track,
            Player = this.Player,//KEEP?
            Volume = Settings.Get(Settings.TrackVolumeKey(track), 100)
          });
        }
      }
      else if (PlayerProperty.PropertyName == propertyName)
      {
      }
    }

    void TrackSlider_PropertyChanged(System.Object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == TrackSlider.TrackProperty.PropertyName)
      {
        var slider = sender as TrackSlider;
        slider.Volume = Settings.Get(Settings.TrackVolumeKey(slider.Track), 100);
      }
    }

    #endregion  Events
  }
}
