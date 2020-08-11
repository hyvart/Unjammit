using System;

using Xamarin.Forms;

namespace Jammit.Forms.Models
{
  class HomeMenuItem : BindableObject
  {
    public static BindableProperty TitleProperty =
      BindableProperty.Create(nameof(Title), typeof(string), typeof(HomeMenuItem), "[Title]");

    public string Title
    {
      get
      {
        return (string)GetValue(TitleProperty);
      }

      set
      {
        SetValue(TitleProperty, value);
      }
    }

    public Type TargetType { get; set; }
  }
}
