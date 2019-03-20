using System;
using System.Collections.Generic;
using System.Linq;
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
    }

    private void SaveButton_Clicked(object sender, EventArgs e)
    {
      //Hack: Manually flushing settings.
      //TODO: Replace with tow-way binding.
      Settings.ServiceUri = ServiceUriEntry.Text;
    }

    private void AuthorizeButton_Clicked(object sender, EventArgs e)
    {
      App.Client.RequestAuthorization().Wait();
    }

    private async void DeleteDataButton_Clicked(object sender, System.EventArgs e)
    {
      if (await DisplayAlert("Please confirm", "Your local library will be completely deleted.\nThis can not be undone.", "Yes", "No"))
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
      }
    }
  }
}