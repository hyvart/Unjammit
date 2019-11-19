using System;

using AppKit;

namespace Jammit.Forms.Views
{
  /// <summary>
  /// See https://github.com/xamarin/Xamarin.Forms/blob/release-4.3.0-sr2/Xamarin.Forms.Platform.MacOS/Controls/FormsNSSlider.cs
  /// See https://github.com/xamarin/xamarin-macios/blob/xamarin-mac-5.2.1.11/src/AppKit/NSSlider.cs
  /// </summary>
  public class MacOSVerticalSlider : NSSlider
  {
    public MacOSVerticalSlider() {}

    public override nint IsVertical { get => 1; }
  }
}
