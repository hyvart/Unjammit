using Xamarin.Essentials;

namespace Jammit.Forms
{
  /// <summary>
  /// This is the Settings static class that can be used in your Core solution or in any
  /// of your client applications. All settings are laid out the same exact way with getters
  /// and setters. 
  /// </summary>
  public static class Settings
  {
    #region Setting Constants

    private const string SettingsKey = "settings_key";
    private static readonly string SettingsDefault = string.Empty;

    private const string TrackPathKey = "trackpath_key";
    private static readonly string TrackPathDefault = ".";

    private const string ServiceUriKey = "serviceuri_key";
    private static readonly string ServiceUriDefault = string.Empty;

    private const string CredentialsKey = "credentials_key";
    private static readonly string CredentialsDefault = string.Empty;

    private const string CultureKey = "culture_key";
    private static readonly string CultureDefault = System.Globalization.CultureInfo.CurrentUICulture.Name;

    #endregion

    public static void Clear()
    {
      //AppSettings.Clear();
      Preferences.Clear();
    }

    public static string GeneralSettings
    {
      get
      {
        return Preferences.Get(SettingsKey, SettingsDefault);
      }
      set
      {
        Preferences.Set(SettingsKey, value);
      }
    }

    public static string TrackPath
    {
      get
      {
        return Preferences.Get(TrackPathKey, TrackPathDefault);
      }

      set
      {
        Preferences.Set(TrackPathKey, value);
      }
    }

    public static string ServiceUri
    {
      get
      {
        return Preferences.Get(ServiceUriKey, ServiceUriDefault);
      }

      set
      {
        Preferences.Set(ServiceUriKey, value);
      }
    }

    public static string Credentials
    {
      get
      {
        return Preferences.Get(CredentialsKey, CredentialsDefault);
      }

      set
      {
        Preferences.Set(CredentialsKey, value);
      }
    }

    public static string Culture
    {
      get
      {
        return Preferences.Get(CultureKey, CultureDefault);
      }

      set
      {
        Preferences.Set(CultureKey, value);
      }
    }
  }
}
