using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace Jammit.Forms
{
  /// <summary>
  /// This is the Settings static class that can be used in your Core solution or in any
  /// of your client applications. All settings are laid out the same exact way with getters
  /// and setters. 
  /// </summary>
  public static class Settings
  {
    private static ISettings AppSettings
    {
      get
      {
        return CrossSettings.Current;
      }
    }

    #region Setting Constants

    private const string SettingsKey = "settings_key";
    private static readonly string SettingsDefault = string.Empty;

    private const string TrackPathKey = "trackpath_key";
    private static readonly string TrackPathDefault = ".";

    private const string ServiceUriKey = "serviceuri_key";
    private static readonly string ServiceUriDefault = string.Empty;

    private const string CredentialsKey = "credentials_key";
    private static readonly string CredentialsDefault = string.Empty;

    #endregion

    public static string GeneralSettings
    {
      get
      {
        return AppSettings.GetValueOrDefault(SettingsKey, SettingsDefault);
      }
      set
      {
        AppSettings.AddOrUpdateValue(SettingsKey, value);
      }
    }

    public static string TrackPath
    {
      get { return AppSettings.GetValueOrDefault(TrackPathKey, TrackPathDefault); }
      set { AppSettings.AddOrUpdateValue(TrackPathKey, value); }
    }

    public static string ServiceUri
    {
      get { return AppSettings.GetValueOrDefault(ServiceUriKey, ServiceUriDefault); }
      set { AppSettings.AddOrUpdateValue(ServiceUriKey, value); }
    }

    public static string Credentials
    {
      get { return AppSettings.GetValueOrDefault(CredentialsKey, CredentialsDefault); }
      set { AppSettings.AddOrUpdateValue(CredentialsKey, value); }
    }
  }
}