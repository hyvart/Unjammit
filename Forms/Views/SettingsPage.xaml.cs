using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Jammit.Forms.Resources;

namespace Jammit.Forms.Views
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class SettingsPage : ContentPage
  {
    public SettingsPage()
    {
      InitializeComponent();

      var rm = new System.Resources.ResourceManager("Jammit.Forms.Resources.Localized", typeof(SettingsPage).Assembly);

      LocaleImageEn.Source = ImageSource.FromStream(() =>
        new MemoryStream(rm.GetObject("SettingsPage_LocaleImage", CultureInfo.GetCultureInfo("en")) as byte[]));

      LocaleImageEs.Source = ImageSource.FromStream(() =>
        new MemoryStream(rm.GetObject("SettingsPage_LocaleImage", CultureInfo.GetCultureInfo("es")) as byte[]));

      LocaleImageRu.Source = ImageSource.FromStream(() =>
        new MemoryStream(rm.GetObject("SettingsPage_LocaleImage", CultureInfo.GetCultureInfo("ru")) as byte[]));
    }

    #region Page overrides

    protected override void OnAppearing()
    {
      base.OnAppearing();

      switch (CultureInfo.CurrentUICulture.Name)
      {
        case "en":
          LocaleRadioButtonEn.IsChecked = true;
          break;
        case "es":
          LocaleRadioButtonEs.IsChecked = true;
          break;
        case "ru":
          LocaleRadioButtonRu.IsChecked = true;
          break;

        default:
          if (CultureInfo.CurrentUICulture.Name.StartsWith("en-"))
          {
            LocaleRadioButtonEn.IsChecked = true;
          }
          else if (CultureInfo.CurrentUICulture.Name.StartsWith("es-"))
          {
            LocaleRadioButtonEs.IsChecked = true;
          }
          else if (CultureInfo.CurrentUICulture.Name.StartsWith("ru-"))
          {
            LocaleRadioButtonRu.IsChecked = true;
          }
          else
          {
            // Unrecognized locale. Use "en".
            LocaleRadioButtonEn.IsChecked = true;
          }
          break;
      }
    }

    #endregion  Page overrides

    private void AuthorizeButton_Clicked(object sender, EventArgs e)
    {
      App.Client.RequestAuthorization().Wait();
    }

    //TODO: Make Settings-level or App-level static member.
    ILocaleSwitcher _localeSwitcher = DependencyService.Get<ILocaleSwitcher>();
    private void LocaleRadioButtonEn_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
      if (e.Value)
      {
        LocalizationResourceManager.Instance.SetCulture(CultureInfo.GetCultureInfo("en"));
        _localeSwitcher?.SwitchLocale("en");

        LocaleLabel.Text = Localized.SettingsPage_LocaleLabel;

        Settings.Culture = "en";
      }
    }

    private void LocaleRadioButtonEs_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
      if (e.Value)
      {
        LocalizationResourceManager.Instance.SetCulture(CultureInfo.GetCultureInfo("es"));
        _localeSwitcher?.SwitchLocale("es");

        LocaleLabel.Text = Localized.SettingsPage_LocaleLabel;

        Settings.Culture = "es";
      }
    }

    private void LocaleRadioButtonRu_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
      if (e.Value)
      {
        LocalizationResourceManager.Instance.SetCulture(CultureInfo.GetCultureInfo("ru"));
        _localeSwitcher?.SwitchLocale("ru");

        LocaleLabel.Text = Localized.SettingsPage_LocaleLabel;

        Settings.Culture = "ru";
      }
    }

    private void ServiceUriEntry_Unfocused(object sender, FocusEventArgs e)
    {
      Settings.ServiceUri = ServiceUriEntry.Text;
    }

    private void ContentPage_Disappearing(object sender, EventArgs e)
    {
      Settings.ServiceUri = ServiceUriEntry.Text;
    }

    private async void DeleteDataButton_Clicked(object sender, EventArgs e)
    {
      if (await DisplayAlert(
        LocalizationResourceManager.Instance["SettingsPage_DeleteDataConfirm"],
        LocalizationResourceManager.Instance["SettingsPage_DeleteDataText"],
        LocalizationResourceManager.Instance["SettingsPage_DeleteDataYes"],
        LocalizationResourceManager.Instance["SettingsPage_DeleteDataNo"]))
      {
        foreach (var song in App.Library.Songs)
        {
          App.Library.RemoveSong(song);
        }

        foreach (var userDir in new string[] { "Downloads", "Tracks" })
        {
          var userDirPath = System.IO.Path.Combine(App.DataDirectory, userDir);
          if (System.IO.Directory.Exists(userDirPath))
          {
            var userDirInfo = new System.IO.DirectoryInfo(userDirPath);
            foreach (var file in userDirInfo.GetFiles())
              file.Delete();
            foreach (var dir in userDirInfo.GetDirectories())
              dir.Delete(true);
          }
        }

        Settings.Clear();
      }
    }
  }
}
