using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(Jammit.Forms.Views.VerticalSlider), typeof(Jammit.Forms.Renderers.UWPVerticalSliderRenderer))]
namespace Jammit.Forms.Renderers
{
  public class UWPVerticalSliderRenderer : SliderRenderer
  {
    public UWPVerticalSliderRenderer() { }

    #region ViewRenderer overrides

    protected override void OnElementChanged(ElementChangedEventArgs<Slider> e)
    {
      base.OnElementChanged(e);

      if (Control != null)
      {
        Control.Orientation = Windows.UI.Xaml.Controls.Orientation.Vertical;
      }
    }

    #endregion ViewRenderer overrides
  }
}
