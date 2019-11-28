using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Jammit.Forms.Views.DisabledScrollView), typeof(Jammit.Forms.Renderers.IOSDisabledScrollViewRenderer))]
namespace Jammit.Forms.Renderers
{
  public class IOSDisabledScrollViewRenderer : ScrollViewRenderer
  {
    protected override void OnElementChanged(VisualElementChangedEventArgs e)
    {
      base.OnElementChanged(e);

      ScrollEnabled = false;
    }
  }
}
