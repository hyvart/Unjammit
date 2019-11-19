using System;

using UIKit;

namespace Jammit.Forms.Views
{
  public class IOSVerticalSlider : UISlider
  {
    public IOSVerticalSlider()
    {
      //TODO: Not rotating at all. Research.
      //      Note, creating a transform object and assigning doesn't solve it.
      this.Transform.Rotate(new nfloat(Math.PI / 2));
    }
  }
}
