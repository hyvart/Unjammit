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

      //TODO: Make it work and enable, then disable explicit XAML Rotation.
      //if (Control != null)
      //{
      //  Control.Transform.Rotate(NMath.PI / 2);
      //}
    }

    #endregion ViewRenderer overrides
  }
}
