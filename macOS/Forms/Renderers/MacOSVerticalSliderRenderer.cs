using System;

using Xamarin.Forms;
using Xamarin.Forms.Platform.MacOS;

///
// See https://xamarinexperience.blogspot.com/2019/04/vertical-slider-in-xamarin-forms.html
///
[assembly: ExportRenderer(typeof(Jammit.Forms.Views.VerticalSlider), typeof(Jammit.Forms.Renderers.MacOSVerticalSliderRenderer))]
namespace Jammit.Forms.Renderers
{
  /// <summary>
  /// See https://github.com/xamarin/Xamarin.Forms/blob/release-4.3.0-sr2/Xamarin.Forms.Platform.MacOS/Renderers/SliderRenderer.cs
  /// </summary>
  public class MacOSVerticalSliderRenderer : SliderRenderer
  {
    public MacOSVerticalSliderRenderer()
    {
    }

    protected override void OnElementChanged(ElementChangedEventArgs<Slider> e)
    {
      //TODO: Likely remove MacOSVerticalSlider.
      //SetNativeControl(new Views.MacOSVerticalSlider());
      base.OnElementChanged(e);

      if (Control != null)
        Control.IsVertical = 1;
    }
  }
}
