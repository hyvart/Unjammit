using System.Collections.Generic;

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
    private static Dictionary<string, object> _prefMap = new Dictionary<string, object>();

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
      return $"Song/{song.Sku}/SelectedScore";
    }

    public static string MixerCollapsedKey(Model.SongInfo song)
    {
      return $"Song/{song.Sku}/MixerCollapsed";
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
      return $"Song/{song.Sku}/Position";
    }

    #endregion Settings Functions

    public static void Clear()
    {
      try
      {
        Preferences.Clear();
      }
      catch(NotImplementedInReferenceAssemblyException)
      {
        _prefMap.Clear();
      }
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
        try
        {
          return Preferences.Get(TrackPathKey, TrackPathDefault);
        }
        catch(NotImplementedInReferenceAssemblyException)
        {
          object result;
          if (_prefMap.TryGetValue(TrackPathKey, out result))
          {
            return result as string;
          }

          return TrackPathDefault;
        }
      }

      set
      {
        try
        {
          Preferences.Set(TrackPathKey, value);
        }
        catch(NotImplementedInReferenceAssemblyException)
        {
          _prefMap[TrackPathKey] = value;
        }
      }
    }

    public static string ServiceUri
    {
      get
      {
        try
        {
          return Preferences.Get(ServiceUriKey, ServiceUriDefault);
        }
        catch(NotImplementedInReferenceAssemblyException)
        {
          object result;
          if (_prefMap.TryGetValue(ServiceUriKey, out result))
          {
            return result as string;
          }

          return ServiceUriDefault;
        }
      }

      set
      {
        try
        {
          Preferences.Set(ServiceUriKey, value);
        }
        catch (NotImplementedInReferenceAssemblyException)
        {
          _prefMap[ServiceUriKey] = value;
        }
      }
    }

    public static string Credentials
    {
      get
      {
        try
        {
          return Preferences.Get(CredentialsKey, CredentialsDefault);
        }
        catch (NotImplementedInReferenceAssemblyException)
        {
          object result;
          if (_prefMap.TryGetValue(CredentialsKey, out result))
            return result as string;

          return CredentialsDefault;
        }
      }

      set
      {
        try
        {
          Preferences.Set(CredentialsKey, value);
        }
        catch (NotImplementedInReferenceAssemblyException)
        {
          _prefMap[CredentialsKey] = value;
        }
      }
    }

    public static string Culture
    {
      get
      {
        try
        {
          return Preferences.Get(CultureKey, CultureDefault);
        }
        catch (NotImplementedInReferenceAssemblyException)
        {
          return CultureDefault;
        }
      }

      set
      {
        try
        {
          Preferences.Set(CultureKey, value);
        }
        catch (NotImplementedInReferenceAssemblyException)
        {
          _prefMap[CultureKey] = value;
        }
      }
    }

    #region Track settings

    public static bool IsTrackMuted(Model.TrackInfo track)
    {
      try
      {
        return Preferences.Get(TrackMutedKey(track), false);
      }
      catch (NotImplementedInReferenceAssemblyException)
      {
        object result;
        if (_prefMap.TryGetValue(TrackMutedKey(track), out result))
          return bool.Parse(result as string);

        return false;
      }
    }

    public static void SetTrackMuted(Model.TrackInfo track, bool value)
    {
      try
      {
        Preferences.Set(TrackMutedKey(track), value);
      }
      catch (NotImplementedInReferenceAssemblyException)
      {
        _prefMap[TrackMutedKey(track)] = value;
      }
    }

    #endregion Track settings

    #region Generic settings

    public static bool Get(string key, bool defaultValue)
    {
      try
      {
        return Preferences.Get(key, defaultValue);
      }
      catch(NotImplementedInReferenceAssemblyException)
      {
        return defaultValue;
      }
    }

    public static void Set(string key, bool value)
    {
      try
      {
        Preferences.Set(key, value);
      }
      catch(NotImplementedInReferenceAssemblyException)
      {
      }
    }

    public static uint Get(string key, uint defaultValue)
    {
      try
      {
        return (uint)Preferences.Get(key, (long)defaultValue);
      }
      catch(NotImplementedInReferenceAssemblyException)
      {
        return defaultValue;
      }
    }

    public static void Set(string key, uint value)
    {
      try
      {
        Preferences.Set(key, (long)value);
      }
      catch(NotImplementedInReferenceAssemblyException)
      {

      }
    }

    public static float Get(string key, float defaultValue)
    {
      try
      {
        return Preferences.Get(key, defaultValue);
      }
      catch(NotImplementedInReferenceAssemblyException)
      {
        return defaultValue;
      }
    }

    public static void Set(string key, float value)
    {
      try
      {
        Preferences.Set(key, value);
      }
      catch(NotImplementedInReferenceAssemblyException)
      {

      }
    }

    public static string Get(string key, string defaultValue)
    {
      try
      {
        return Preferences.Get(key, defaultValue);
      }
      catch(NotImplementedInReferenceAssemblyException)
      {
        return defaultValue;
      }
    }

    public static void Set(string key, string value)
    {
      try
      {
        Preferences.Set(key, value);
      }
      catch(NotImplementedInReferenceAssemblyException)
      {

      }
    }

    public static System.TimeSpan Get(string key, System.TimeSpan defaultValue)
    {
      try
      {
        return System.TimeSpan.FromMilliseconds(Preferences.Get(key, defaultValue.TotalMilliseconds));
      }
      catch(NotImplementedInReferenceAssemblyException)
      {
        return defaultValue;
      }
    }

    public static void Set(string key, System.TimeSpan value)
    {
      try
      {
        Preferences.Set(key, value.TotalMilliseconds);
      }
      catch(NotImplementedInReferenceAssemblyException)
      {
      }
    }

    #endregion Generic settings
  }
}
