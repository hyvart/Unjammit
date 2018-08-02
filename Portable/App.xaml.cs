using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using PCLStorage;
using Jammit.Audio;
using Jammit.Model;
using Jammit.Model2;

namespace Jammit.Portable
{
  public partial class App : Application
  {
    public App(IFileSystem fileSystem, Func<ISong, ISongPlayer> songPlayerFactory, IJcfLoader loader)
    {
      InitializeComponent();

      App.Client = new Client.RestClient();
      App.Library = new FolderLibrary(fileSystem.LocalStorage.Path);
      App.FileSystem = fileSystem;
      App.SongPlayerFactory = songPlayerFactory;
      App.MediaLoader = loader;

      MainPage = new Jammit.Portable.MainPage();
    }

    public App(IFileSystem fileSystem, Func<JcfMedia, IJcfPlayer> playerFactory, IJcfLoader loader)
    {
      App.Client = new Client.RestClient();
      App.Library = new FolderLibrary(Xamarin.Essentials.FileSystem.AppDataDirectory);
      App.FileSystem = fileSystem;
      App.PlayerFactory = playerFactory;
      App.MediaLoader = loader;

      MainPage = new Jammit.Portable.MainPage();
    }

    public App(IFileSystem fileSystem) : this(fileSystem, (s) => { return new MockSongPlayer(s); }, null) {}

    #region Properties

    public static Client.IClient Client { get; private set; }

    public static ILibrary Library { get; private set; }

    public static IFileSystem FileSystem { get; private set; }

    public static Func<ISong, ISongPlayer> SongPlayerFactory { get; private set; }

    public static Func<JcfMedia, IJcfPlayer> PlayerFactory { get; private set; }

    public static IJcfLoader MediaLoader { get; private set; }

    #endregion

    protected override void OnStart()
    {
      // Handle when your app starts
      MainPage.BackgroundColor = Color.FromHex(Settings.Dummy);
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
