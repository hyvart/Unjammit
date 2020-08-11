using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Jammit.Forms.Xaml
{
  [ContentProperty("Text")]
  public class TranslateExtension : IMarkupExtension<BindingBase>
  {
    public string Text { get; set; }

    #region IMarkupExtension

    public BindingBase ProvideValue(IServiceProvider serviceProvider)
    {
      return ProvideValue(serviceProvider);
    }

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
    {
      var binding = new Binding
      {
        Mode = BindingMode.OneWay,
        Path = $"[{Text}]",
        Source = LocalizationResourceManager.Instance
      };

      return binding;
    }

    #endregion IMarkupExtension
  }
}
