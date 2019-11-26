using System;
using System.Reflection;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Jammit.Forms.Views
{
  public class DisabledScrollView : ScrollView
  {
    public DisabledScrollView()
    {
      //ClearScrollToRequested();// Crashes!
    }

    protected override void LayoutChildren(double x, double y, double width, double height)
    {
      base.LayoutChildren(x, y, width, height);
      ClearScrollToRequested();
    }

    //[Obsolete]
    //protected override SizeRequest OnSizeRequest(double widthConstraint, double heightConstraint)
    //{
    //  ClearScrollToRequested();

    //  heightConstraint = double.PositiveInfinity;
    //  SizeRequest contentRequest = Content.Measure(widthConstraint, heightConstraint, MeasureFlags.IncludeMargins);
    //  contentRequest.Minimum = new Size(Math.Min(40, contentRequest.Minimum.Width), Math.Min(40, contentRequest.Minimum.Height));

    //  return contentRequest;
    //}

    bool _cleared = false;
    void ClearScrollToRequested()
    {
      if (_cleared)
        return;

      var baseFields = typeof(ScrollView).GetRuntimeFields();
      FieldInfo field = null;
      foreach(var f in baseFields)
      {
        if (f.Name == "ScrollToRequested")
          field = f;
      }

      var evt = field.GetValue(this);
      var method = typeof(EventHandler<ScrollToRequestedEventArgs>).GetMethod("GetInvocationList");
      var invocationList = method.Invoke(evt, new object[] { }) as System.Delegate[];
      foreach(var del in invocationList)
      {
        ScrollToRequested -= del as EventHandler<ScrollToRequestedEventArgs>;
      }

      ScrollToRequested += DisabledScrollView_ScrollToRequested;

      _cleared = true;
    }

    private void DisabledScrollView_ScrollToRequested(object sender, ScrollToRequestedEventArgs e)
    {
      var y = 99;
    }
  }
}
