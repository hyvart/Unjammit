using System;

using Foundation;
using Xamarin.Forms;

[assembly: Dependency(typeof(Jammit.Forms.MacOSLocaleSwitcher))]
namespace Jammit.Forms
{
  public class MacOSLocaleSwitcher : ILocaleSwitcher
  {
    public MacOSLocaleSwitcher()
    {
    }

    #region ILocaleSwitcher

    public void SwitchLocale(string locale)
    {
      NSUserDefaults.StandardUserDefaults.SetValueForKey(NSArray.FromStrings(locale.Substring(0, 2)), new NSString("AppleLanguages"));
      NSUserDefaults.StandardUserDefaults.Synchronize();
    }

    #endregion ILocaleSwitcher
  }
}
