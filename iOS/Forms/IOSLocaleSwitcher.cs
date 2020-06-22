using System;

using Foundation;
using Xamarin.Forms;

// See https://stackoverflow.com/questions/55064378
[assembly: Dependency(typeof(Jammit.Forms.IOSLocaleSwitcher))]
namespace Jammit.Forms
{
  public class IOSLocaleSwitcher : ILocaleSwitcher
  {
    public IOSLocaleSwitcher()
    {
    }

    #region ILocaleSwitcher

    //TODO: Doesn't seem to work
    public void SwitchLocale(string locale)
    {
      NSUserDefaults.StandardUserDefaults.SetValueForKey(NSArray.FromStrings(locale.Substring(0, 2)), new NSString("AppleLanguages"));
      NSUserDefaults.StandardUserDefaults.Synchronize();
    }

    #endregion ILocaleSwitcher
  }
}
