using System;

namespace Jammit.Forms.Converters
{
  public class DoubleToTimeStampConverter : Xamarin.Forms.IValueConverter
  {
    #region IValueConverter

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        return TimeSpan.FromSeconds((double)value).ToString(@"mm\:ss");
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion IValueConverter
  }

  public class DoubleToTimeStampBackConverter : Xamarin.Forms.IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return TimeSpan.FromSeconds((double)value);
    }
  }
}
