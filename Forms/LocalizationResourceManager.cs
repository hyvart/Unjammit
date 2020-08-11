﻿using System.ComponentModel;
using System.Globalization;
using System.Threading;

/// <summary>
/// See https://devblogs.microsoft.com/xamarin/mastering-multilingual-in-xamarin-forms/
/// </summary>
namespace Jammit.Forms
{
  //TODO: Rename this class!
  public class LocalizationResourceManager : INotifyPropertyChanged
  {
    public static LocalizationResourceManager Instance { get; } = new LocalizationResourceManager();

    private LocalizationResourceManager()
    {
      if (Resources.Localized.Culture != null)
        SetCulture(Resources.Localized.Culture);
    }

    public string this[string text]
    {
      get
      {
        return Resources.Localized.ResourceManager.GetString(text, Resources.Localized.Culture);
      }
    }

    public void SetCulture(CultureInfo culture)
    {
      Thread.CurrentThread.CurrentUICulture = culture;
      Resources.Localized.Culture = culture;

      Invalidate();
    }

    public void Invalidate()
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
    }

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion INotifyPropertyChanged

  }
}
