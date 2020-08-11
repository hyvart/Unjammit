using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Jammit.Forms.Xaml
{
  [ContentProperty("Text")]
  public class TranslateTextExtension : IMarkupExtension<BindingBase>
  {
    public string Text { get; set; }

    public string StringFormat { get; set; }

    #region IMarkupExtension

    public BindingBase ProvideValue(IServiceProvider serviceProvider)
    {
      var binding = new Binding
      {
        Mode = BindingMode.OneWay,
        Path = $"[{Text}]",
        Source = LocalizationResourceManager.Instance,
        StringFormat = StringFormat
      };

      return binding;
    }

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
    {
      return ProvideValue(serviceProvider);
    }

    #endregion IMarkupExtension
  }

  [ContentProperty("Title")]
  public class TranslateTitleExtension : IMarkupExtension<BindingBase>
  {
    public string Title { get; set; }

    #region IMarkupExtension

    public BindingBase ProvideValue(IServiceProvider serviceProvider)
    {
      return new Binding
      {
        Mode = BindingMode.OneWay,
        Path = $"[{Title}]",
        Source = LocalizationResourceManager.Instance
      };
    }

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
    {
      return ProvideValue(serviceProvider);
    }

    #endregion IMarkupExtension
  }
}
