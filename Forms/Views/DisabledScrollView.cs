using System;
using System.Reflection;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Jammit.Forms.Views
{
  public class DisabledScrollView : ScrollView
  {
    protected override void LayoutChildren(double x, double y, double width, double height)
    {
      base.LayoutChildren(x, y, width, height);

      if (null != NativeParentScroller)
        return;

      var baseFields = typeof(ScrollView).GetRuntimeFields();
      FieldInfo field = null;
      foreach (var f in baseFields)
      {
        if (f.Name == "ScrollToRequested")
          field = f;
      }

      var evt = field.GetValue(ParentScroller);
      NativeParentScroller = (evt as EventHandler<ScrollToRequestedEventArgs>).Target;
    }

    public object NativeParentScroller { get; private set; }

    public ScrollView ParentScroller { get; set; }
  }
}
