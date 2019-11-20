using System;

using Xamarin.Forms;
using Xamarin.Forms.Platform.GTK;
using Xamarin.Forms.Platform.GTK.Renderers;

[assembly: ExportRenderer(typeof(Jammit.Forms.Views.VerticalSlider), typeof(Jammit.Forms.Renderers.GtkVerticalSliderRenderer))]
namespace Jammit.Forms.Renderers
{
  public class GtkVerticalSliderRenderer : SliderRenderer
  {
    #region ViewRenderer overrides

    protected override void OnElementChanged(ElementChangedEventArgs<Slider> e)
    {
      base.OnElementChanged(e);

      if (Control != null)
      {

      }
    }

    #endregion ViewRenderer overrides
  }
}
