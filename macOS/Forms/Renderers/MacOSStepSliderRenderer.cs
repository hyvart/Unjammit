// Based on https://github.com/xamarin/xamarin-forms-book-samples/blob/78569/Libraries/Xamarin.FormsBook.Platform/Xamarin.FormsBook.Platform.iOS/StepSliderRenderer.cs
using System;
using System.ComponentModel;

using AppKit;

using Xamarin.Forms;
using Xamarin.Forms.Platform.MacOS;

[assembly: ExportRenderer(typeof(Xamarin.FormsBook.Platform.StepSlider),
                          typeof(Xamarin.FormsBook.Platform.MacOS.StepSliderRenderer))]

namespace Xamarin.FormsBook.Platform.MacOS
{
  public class StepSliderRenderer : ViewRenderer<StepSlider, NSSlider>
  //public class StepSliderRenderer : SliderRenderer
  {
    int steps;

    protected override void OnElementChanged(ElementChangedEventArgs<StepSlider> args)
    {
      base.OnElementChanged(args);

      if (Control == null)
      {
        SetNativeControl(new NSSlider());//CRASHES!
      }
#if false

      if (args.NewElement != null)
      {
        SetMinimum();
        SetMaximum();
        SetSteps();
        SetValue();

        //Control.ValueChanged += OnUISliderValueChanged;
        Control.Activated += OnNSSliderValueChanged;
      }
      else
      {
        //Control.ValueChanged -= OnUISliderValueChanged;
        Control.Activated -= OnNSSliderValueChanged;
      }
#endif
    }

    protected override void OnElementPropertyChanged(object sender,
                                                     PropertyChangedEventArgs args)
    {
      base.OnElementPropertyChanged(sender, args);

      if (args.PropertyName == StepSlider.MinimumProperty.PropertyName)
      {
        SetMinimum();
      }
      else if (args.PropertyName == StepSlider.MaximumProperty.PropertyName)
      {
        SetMaximum();
      }
      else if (args.PropertyName == StepSlider.StepsProperty.PropertyName)
      {
        SetSteps();
      }
      else if (args.PropertyName == StepSlider.ValueProperty.PropertyName)
      {
        SetValue();
      }
    }

    void SetMinimum()
    {
      Control.MinValue = (float)Element.Minimum;
    }

    void SetMaximum()
    {
      Control.MaxValue = (float)Element.Maximum;
    }

    void SetSteps()
    {
      steps = Element.Steps;
    }

    void SetValue()
    {
      Control.FloatValue = (float)Element.Value;
      //Control.Value = (float)Element.Value;
    }

    void OnNSSliderValueChanged(object sender, EventArgs args)
    {
      double increment = (Element.Maximum - Element.Minimum) / Element.Steps;
      //double value = increment * Math.Round(Control.Value / increment);
      double value = increment * Math.Round(Control.DoubleValue / increment);
      ((IElementController)Element).SetValueFromRenderer(StepSlider.ValueProperty, value);
    }
  }
}
