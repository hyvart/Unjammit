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
        // See https://github.com/susairajs/Xamarin-iOS-Vertical-Slider/blob/1bfdfa35103afc13742bd5ead4db76fe8e8d4d0a/XamariniOSSlider/ViewController.cs#L23
        Control.Transform = CoreGraphics.CGAffineTransform.MakeRotation(-NMath.PI / 2);
      }
    }

    #endregion ViewRenderer overrides
  }
}
