using Xamarin.Forms;

[assembly: ExportRenderer(typeof(Jammit.Forms.Views.DisabledScrollView), typeof(Jammit.Forms.Renderers.MacOSDisabledScrollViewRenderer))]
namespace Jammit.Forms.Renderers
{
  public class MacOSDisabledScrollViewRenderer : Xamarin.Forms.Platform.MacOS.ScrollViewRenderer
  {
    // https://apptyrant.com/2015/05/18/how-to-disable-nsscrollview-scrolling/
    public override void ScrollWheel(AppKit.NSEvent theEvent)
    {
      //TODO: Horrible hack! Making a delegate scrollview do the job.
      var elem = Element as Views.DisabledScrollView;
      var psc = elem.NativeParentScroller;
      var nssv = psc as AppKit.NSScrollView;
      nssv.ScrollWheel(theEvent);
    }
  }
}
