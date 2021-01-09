using AppKit;
using Foundation;

using Xamarin.Forms.Platform.MacOS;

namespace Jammit.macOS
{
  [Register("AppDelegate")]
  public partial class AppDelegate : FormsApplicationDelegate
  {
    NSWindow _window;

    public AppDelegate()
    {
      var style = NSWindowStyle.Closable | NSWindowStyle.Resizable | NSWindowStyle.Titled;

      var rect = new CoreGraphics.CGRect(200, 1000, 1024, 768);
      _window = new NSWindow(rect, style, NSBackingStore.Buffered, false);

      // Selector setTitleVisibility available on macOS 10.10 and later.
      var osVersion = new NSProcessInfo().OperatingSystemVersion;
      if ((osVersion.Major == 10 && osVersion.Minor > 9) || osVersion.Major > 10)
        _window.TitleVisibility = NSWindowTitleVisibility.Hidden;
    }

    public override NSWindow MainWindow
    {
      get
      {
        return _window;
      }
    }

    public override void DidFinishLaunching(NSNotification notification)
    {
      Xamarin.Forms.Forms.Init();

      // Xamarin.Essentials.FileSystem.AppDataDirectory yields '/Users/<user name>/Library' on macOS
      string dataDir = System.IO.Path.Combine(Xamarin.Essentials.FileSystem.AppDataDirectory, "Application Support", "Unjammit");
      // Create dataDir, if it doesnt' exist.
      if (!System.IO.Directory.Exists(dataDir))
        System.IO.Directory.CreateDirectory(dataDir);

      Jammit.Forms.App.AllowedFileTypes = new string[] { "com.pkware.zip-archive" };
      Jammit.Forms.App.DataDirectory = dataDir;
      Jammit.Forms.App.PlayerFactory = async (media) => await System.Threading.Tasks.Task.Run(() =>
      {
        return new Audio.AppleJcfPlayer(media, (track, stream) =>
        {
          return new Audio.MacOSAVAudioPlayer(track, stream);
        });
      });
      Jammit.Forms.App.MediaLoader = new Model.FileSystemJcfLoader(dataDir);

      LoadApplication(new Jammit.Forms.App());

      base.DidFinishLaunching(notification);
      _window.Toolbar.Visible = false;
    }

    // See https://docs.microsoft.com/en-us/xamarin/mac/user-interface/menu#status-bar-menus
    [Export("customHelp:")]
    void ShowHelp(NSObject sender)
    {
      NSWorkspace.SharedWorkspace.OpenUrl(new NSUrl(@"http://unjammit.com/help"));
    }

    public override void WillTerminate(NSNotification notification)
    {
      // Insert code here to tear down your application
    }

    #region NSApplicationDelegate overrides

    /// <summary>
    /// Make application exit when closing the main window.
    /// </summary>
    /// <param name="sender"></param>
    /// <returns></returns>
    public override bool ApplicationShouldTerminateAfterLastWindowClosed(NSApplication sender)
    {
      return true;
    }

    #endregion NSApplicationDelegate overrides
  }
}
