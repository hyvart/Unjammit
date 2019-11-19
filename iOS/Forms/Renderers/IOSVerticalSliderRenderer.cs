﻿using System;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Jammit.Forms.Views.VerticalSlider), typeof(Jammit.Forms.Renderers.IOSVerticalSliderRenderer))]
namespace Jammit.Forms.Renderers
{
  public class IOSVerticalSliderRenderer : SliderRenderer
  {
    public IOSVerticalSliderRenderer()
    {
    }

    protected override void OnElementChanged(ElementChangedEventArgs<Slider> e)
    {
      SetNativeControl(new Views.IOSVerticalSlider());
      base.OnElementChanged(e);
    }
  }
}
