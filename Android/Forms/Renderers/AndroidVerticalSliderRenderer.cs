using System;

using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Widget;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Jammit.Forms.Views.VerticalSlider), typeof(Jammit.Forms.Renderers.AndroidVerticalSliderRenderer))]
namespace Jammit.Forms.Renderers
{
  public class AndroidVerticalSliderRenderer : SliderRenderer
  {
    [Obsolete]
    public AndroidVerticalSliderRenderer() {}

    protected override void OnElementChanged(ElementChangedEventArgs<Slider> e)
    {
      base.OnElementChanged(e);

      if (Control != null)
      {
        Control.Rotation = 270;
      }
    }
  }
}
