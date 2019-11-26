using System;

using Xamarin.Forms;

//[assembly: ExportRenderer(typeof(Jammit.Forms.Views.DisabledScrollView), typeof(Jammit.Forms.Renderers.MacOSDisabledScrollViewRenderer))]
namespace Jammit.Forms.Renderers
{
  public class MacOSDisabledScrollViewRenderer : Xamarin.Forms.Platform.MacOS.ScrollViewRenderer
  {
    public MacOSDisabledScrollViewRenderer()
    {
      ElementChanged += MacOSDisabledScrollViewRenderer_ElementChanged;
      //this.ViewController.
    }

    private void MacOSDisabledScrollViewRenderer_ElementChanged(object sender, Xamarin.Forms.Platform.MacOS.VisualElementChangedEventArgs e)
    {
    }

    #region ViewRenderer overrides

    #endregion ViewRenderer overrides
  }
}
