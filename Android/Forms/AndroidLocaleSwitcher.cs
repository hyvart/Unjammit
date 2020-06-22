using System;

using Xamarin.Forms;

// See https://stackoverflow.com/questions/56901366

[assembly: Dependency(typeof(Jammit.Forms.AndroidLocaleSwitcher))]
namespace Jammit.Forms
{
  public class AndroidLocaleSwitcher : ILocaleSwitcher
  {
    public AndroidLocaleSwitcher()
    {
    }

    #region ILocaleSwitcher

    //TODO: Doesn't seem to work.
    public void SwitchLocale(string locale)
    {
      var jLocale = new Java.Util.Locale(locale);
      Java.Util.Locale.Default = jLocale;

      var context = global::Android.App.Application.Context;
      context.Resources.Configuration.SetLocale(jLocale);
      // Deprecated. See https://developer.android.com/reference/android/content/res/Resources#updateConfiguration(android.content.res.Configuration,%20android.util.DisplayMetrics)
      context.Resources.UpdateConfiguration(context.Resources.Configuration, context.Resources.DisplayMetrics);
    }

    #endregion ILocaleSwitcher
  }
}
