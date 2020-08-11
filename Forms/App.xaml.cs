using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using Jammit.Audio;
using Jammit.Model;
using System.Threading.Tasks;

namespace Jammit.Forms
{
  public partial class App : Application
  {
    [Obsolete("Use parameter-less constructor.")]
    public App(string dataDirectory, Func<JcfMedia, Task<IJcfPlayer>> playerFactory, IJcfLoader loader)
    {
      App.Client = new Jammit.Forms.Client.RestClient();
      App.DataDirectory = dataDirectory;
      App.Library = new FolderLibrary(dataDirectory, Client);
      App.PlayerFactory = playerFactory;
      App.MediaLoader = loader;

      MainPage = new Jammit.Forms.Views.MainPage();
    }

    public App()
    {
      InitializeComponent();

      App.Client = new Jammit.Forms.Client.BasicHttpClient();
      App.Library = new FolderLibrary(DataDirectory, Client);

      //System.Globalization.CultureInfo.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo(Settings.Culture);
      LocalizationResourceManager.Instance.SetCulture(System.Globalization.CultureInfo.GetCultureInfo(Settings.Culture));

      MainPage = new Jammit.Forms.Views.MainPage();
    }

    #region Properties

    public static Jammit.Client.IClient Client { get; /*private*/ set; }

    public static ILibrary Library { get; /*private*/ set; }

    [Obsolete("Remove when Xamarin.Essentials supports all platforms.")]
    public static string DataDirectory { get; /*private*/ set; }

    public static Func<JcfMedia, Task<IJcfPlayer>> PlayerFactory { get; /*private*/ set; }

    public static IJcfLoader MediaLoader { get; /*private*/ set; }

    public static string[] AllowedFileTypes { get; set; } = new string[] { ".zip" };

    #endregion

    protected override void OnStart()
    {
      // Handle when your app starts
    }

    protected override void OnSleep()
    {
      // Handle when your app sleeps
    }

    protected override void OnResume()
    {
      // Handle when your app resumes
    }
  }
}
