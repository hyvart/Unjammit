using System;

namespace Jammit.Forms.Converters
{
  public class DigitalTimeConverter : Xamarin.Forms.IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return TimeSpan.FromSeconds((double)value).ToString(@"mm\:ss");
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return (double)TimeSpan.Parse((string)value).Seconds;
    }
  }
}
