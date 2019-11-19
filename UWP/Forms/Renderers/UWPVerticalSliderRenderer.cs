﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

namespace Jammit.Forms.Renderers
{
  public class UWPVerticalSliderRenderer : SliderRenderer
  {
    public UWPVerticalSliderRenderer() { }

    protected override void OnElementChanged(ElementChangedEventArgs<Slider> e)
    {
      SetNativeControl(new FormsSlider() { /*Rotation = 270*/ });

      base.OnElementChanged(e);
    }
  }
}
