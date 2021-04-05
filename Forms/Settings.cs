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

    #endregion Setting Constants

    #region Settings Functions

    public static string SelectedScoreKey(Model.SongInfo song)
    {
      return $"Song/{song.Id}/SelectedScore";
    }

    public static string MixerCollapsedKey(Model.SongInfo song)
    {
      return $"Song/{song.Id}/MixerCollapsed";
    }

    public static string TrackVolumeKey(Model.TrackInfo track)
    {
      return $"Track/{track.Identifier}/Volume";
    }

    public static string TrackMutedKey(Model.TrackInfo track)
    {
      return $"Track/{track.Identifier}/Muted";
    }

    public static string SoloTrackKey(Model.SongInfo song)
    {
      return $"Song/{song.Instrument}/SoloTrack";
    }

    public static string PositionKey(Model.SongInfo song)
    {
      return $"Song/{song.Id}/Position";
    }

    #endregion Settings Functions

    public static void Clear()
    {
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

    #region Track settings

    public static bool IsTrackMuted(Model.TrackInfo track)
    {
      return Preferences.Get(TrackMutedKey(track), false);
    }

    public static void SetTrackMuted(Model.TrackInfo track, bool value)
    {
      Preferences.Set(TrackMutedKey(track), value);
    }

    #endregion Track settings

    #region Generic settings

    public static bool Get(string key, bool defaultValue)
    {
      return Preferences.Get(key, defaultValue);
    }

    public static void Set(string key, bool value)
    {
      Preferences.Set(key, value);
    }

    public static uint Get(string key, uint defaultValue)
    {
      return (uint)Preferences.Get(key, (long)defaultValue);
    }

    public static void Set(string key, uint value)
    {
      Preferences.Set(key, (long)value);
    }

    public static float Get(string key, float defaultValue)
    {
      return Preferences.Get(key, defaultValue);
    }

    public static void Set(string key, float value)
    {
      Preferences.Set(key, value);
    }

    public static string Get(string key, string defaultValue)
    {
      return Preferences.Get(key, defaultValue);
    }

    public static void Set(string key, string value)
    {
      Preferences.Set(key, value);
    }

    public static System.TimeSpan Get(string key, System.TimeSpan defaultValue)
    {
      return System.TimeSpan.FromMilliseconds(Preferences.Get(key, defaultValue.TotalMilliseconds));
    }

    public static void Set(string key, System.TimeSpan value)
    {
      Preferences.Set(key, value.TotalMilliseconds);
    }

    #endregion Generic settings
  }
}
