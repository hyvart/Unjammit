using System;

using AppKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.MacOS;

//TODO: Remove after release of https://github.com/xamarin/Xamarin.Forms/pull/14139
[assembly: ExportRenderer(typeof(Xamarin.Forms.RadioButton), typeof(Jammit.Forms.Renderers.MacOSRadioButtonRenderer))]
namespace Jammit.Forms.Renderers
{
  public class MacOSRadioButtonRenderer : RadioButtonRenderer
  {
		void HandleActivated(object sender, EventArgs args)
		{
			if (Element == null || sender == null)
			{
				return;
			}

			Element.IsChecked = (sender as NSButton).State == NSCellStateValue.On;
		}

    protected override void Dispose(bool disposing)
    {
			if (Control != null)
				Control.Activated -= HandleActivated;

			base.Dispose(disposing);
    }

    protected override void OnElementChanged(ElementChangedEventArgs<RadioButton> e)
    {
      base.OnElementChanged(e);

			if (Control != null)
				Control.Activated += HandleActivated;
    }
  }
}
