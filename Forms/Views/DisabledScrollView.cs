using System;
using System.Reflection;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Jammit.Forms.Views
{
  public class DisabledScrollView : ScrollView
  {
    //protected override void LayoutChildren(double x, double y, double width, double height)
    //{
    //  base.LayoutChildren(x, y, width, height);

    //  if (null != NativeParentScroller)
    //    return;

    //  var baseFields = typeof(ScrollView).GetRuntimeFields();
    //  foreach (var field in baseFields)
    //  {
    //    if (field.Name == "ScrollToRequested")
    //    {
    //      var evt = field.GetValue(ParentScroller);
    //      NativeParentScroller = (evt as EventHandler<ScrollToRequestedEventArgs>).Target;

    //      break;
    //    }
    //  }
    //}

    //public object NativeParentScroller { get; private set; }

    public ScrollView Delegate { get; set; }
  }
}
