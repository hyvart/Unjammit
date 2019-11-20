using System;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Jammit.Forms.Views.VerticalSlider), typeof(Jammit.Forms.Renderers.IOSVerticalSliderRenderer))]
namespace Jammit.Forms.Renderers
{
  public class IOSVerticalSliderRenderer : SliderRenderer
  {
    #region ViewRenderer overrides

    protected override void OnElementChanged(ElementChangedEventArgs<Slider> e)
    {
      base.OnElementChanged(e);

      if (Control != null)
      {
        Control.Transform.Rotate(new nfloat(Math.PI / 2));
      }
    }

    #endregion ViewRenderer overrides
  }
}
