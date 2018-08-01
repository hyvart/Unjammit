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
    public Mixer ()
    {
      InitializeComponent ();
    }

    #region Bindable Properties

    public static readonly BindableProperty MediaProperty =
      BindableProperty.Create("Media", typeof(Model2.JcfMedia), typeof(Model2.JcfMedia));

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

    #endregion

    #region Events

    protected override void OnPropertyChanged(string propertyName = null)
    {
      base.OnPropertyChanged(propertyName);

      if (MediaProperty.PropertyName == propertyName)
      {
        //TODO: Notify
      }
    }

    #endregion
  }
}