using System.Reflection;

using Xamarin.Forms;

[assembly: ExportRenderer(typeof(Jammit.Forms.Views.DisabledScrollView), typeof(Jammit.Forms.Renderers.MacOSDisabledScrollViewRenderer))]
namespace Jammit.Forms.Renderers
{
  public class MacOSDisabledScrollViewRenderer : Xamarin.Forms.Platform.MacOS.ScrollViewRenderer
  {
    // https://apptyrant.com/2015/05/18/how-to-disable-nsscrollview-scrolling/
    public override void ScrollWheel(AppKit.NSEvent theEvent)
    {
      var elem = Element as Views.DisabledScrollView;
      foreach (var field in typeof(ScrollView).GetRuntimeFields())
      {
        if ("ScrollToRequested" == field.Name)
        {
          var evt = field.GetValue(elem.Delegate);
          var delegateView = (evt as System.EventHandler<ScrollToRequestedEventArgs>).Target as AppKit.NSScrollView;
          delegateView.ScrollWheel(theEvent);

          return;
        }
      }
    }
  }
}
