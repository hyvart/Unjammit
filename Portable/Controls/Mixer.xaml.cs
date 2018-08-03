using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Jammit.Portable.Controls
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
      BindableProperty.Create("Media", typeof(Model2.JcfMedia), typeof(Model2.JcfMedia));

    public static readonly BindableProperty PlayerProperty =
      BindableProperty.Create("Player", typeof(Audio.IJcfPlayer), typeof(Audio.IJcfPlayer));

    #endregion // Bindable Properties

    #region Properties

    public Model2.JcfMedia Media
    {
      get
      {
        return (Model2.JcfMedia)GetValue(MediaProperty);
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

    #endregion // Properties

    #region Events

    protected override void OnPropertyChanged(string propertyName = null)
    {
      base.OnPropertyChanged(propertyName);

      if (MediaProperty.PropertyName == propertyName)
      {
        foreach (var track in Media.InstrumentTracks)
        {
          ControlsLayout.Children.Add(new Label { Text = track.Title });
          ControlsLayout.Children.Add(new Slider(0, 100, 50));
        }
      }
      else if (PlayerProperty.PropertyName == propertyName)
      {

      }
    }

    private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
    }

    #endregion // Events
  }
}