using Jammit.Forms.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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
        new MemoryStream(rm.GetObject("SettingsPage_LocaleImage", CultureInfo.GetCultureInfo("en-US")) as byte[]));

      LocaleImageEs.Source = ImageSource.FromStream(() =>
        new MemoryStream(rm.GetObject("SettingsPage_LocaleImage", CultureInfo.GetCultureInfo("es-MX")) as byte[]));
    }

    #region Page overrides

    protected override void OnAppearing()
    {
      LocalePicker.SelectedItem = Settings.Culture;
    }

    #endregion  Page overrides

    private void SaveButton_Clicked(object sender, EventArgs e)
    {
      //Hack: Manually flushing settings.
      //TODO: Replace with tow-way binding.

      Settings.ServiceUri = ServiceUriEntry.Text;
      Settings.Culture = LocalePicker.SelectedItem as string;

      LocaleLabel.Text = "Language settings will be reflected after the app is restarted.";
    }

    private void AuthorizeButton_Clicked(object sender, EventArgs e)
    {
      App.Client.RequestAuthorization().Wait();
    }

    private async void DeleteDataButton_Clicked(object sender, System.EventArgs e)
    {
      if (await DisplayAlert("Please confirm", "Your local library and service credentials will be completely deleted.\nThis can not be undone.", "Yes", "No"))
      {
        foreach (var song in App.Library.Songs)
        {
          App.Library.RemoveSong(song);
        }

        foreach(var userDir in new string[] { "Downloads", "Tracks" })
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

        Settings.Credentials = default;
        Settings.ServiceUri = default;
      }
    }

    //TODO: Make Settings-level or App-level static member.
    ILocaleSwitcher _localeSwitcher = DependencyService.Get<ILocaleSwitcher>();
    void LocalePicker_SelectedIndexChanged(object sender, System.EventArgs e)
    {
      CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo((sender as Picker).SelectedItem as string);
      _localeSwitcher?.SwitchLocale((sender as Picker).SelectedItem as string);
    }
  }
}
