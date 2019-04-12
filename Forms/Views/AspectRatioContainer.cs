using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;

namespace Jammit.Forms.Views
{
  /// <summary>
  /// Note: May not be necessary.
  /// https://www.youtube.com/watch?v=FhHYwZxO9cY
  /// </summary>
  public class AspectRatioContainer : ContentView
  {
    protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
    {
      //return base.OnMeasure(widthConstraint, widthConstraint * AspectRatio);
      return base.OnMeasure(widthConstraint, widthConstraint * AspectRatio);
    }

    public static BindableProperty AspectRatioProperty =
      BindableProperty.Create(nameof(AspectRatio), typeof(double), typeof(AspectRatioContainer), (double)1);

    public double AspectRatio
    {
      get
      {
        return (double)GetValue(AspectRatioProperty);
      }

      set
      {
        SetValue(AspectRatioProperty, value);
      }
    }
  }
}
