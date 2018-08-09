using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Jammit.Forms.Controls
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class TrackSlider : ContentView
  {
    public TrackSlider()
    {
      InitializeComponent();

      TitleLabel.WidthRequest = 66;
      VolumeSlider.WidthRequest = 300;

      //TODO: Inject by property only
      VolumeSlider.Value = VolumeSlider.Maximum;
    }

    #region Bindable properties

    //TODO: Analize keeping here.
    public static readonly BindableProperty PlayerProperty =
      BindableProperty.Create("Player", typeof(Audio.IJcfPlayer), typeof(Audio.IJcfPlayer));

    public static readonly BindableProperty TrackProperty =
      BindableProperty.Create("Track", typeof(Model.PlayableTrackInfo), typeof(Model.PlayableTrackInfo));

    public static readonly BindableProperty VolumeProperty =
      BindableProperty.Create("Volume", typeof(uint), typeof(uint), (uint)100);

    #endregion // Bindable properties

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

    public uint Volume
    {
      get
      {
        return (uint)GetValue(VolumeProperty);
      }

      set
      {
        SetValue(VolumeProperty, value);
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

    #endregion // Properties

    #region Events

    protected override void OnPropertyChanged(string propertyName = null)
    {
      base.OnPropertyChanged(propertyName);

      if (TrackProperty.PropertyName == propertyName)
      {
        //TODO: Bind!
        TitleLabel.Text = Track.Title;
      }
      else if (VolumeProperty.PropertyName == propertyName)
      {
        Player.SetVolume(Track, Volume);
      }
    }

    private void VolumeSlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
      //TODO: How about setting the track volume right here and drop the Volume property?
      Volume = (uint)e.NewValue;
    }

    #endregion // Events
  }
}