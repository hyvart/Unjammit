using AppKit;
using Foundation;

using Xamarin.Forms.Platform.MacOS;

namespace Jammit.macOS
{
  [Register("AppDelegate")]
  public class AppDelegate : FormsApplicationDelegate
  {
    NSWindow _window;

    public AppDelegate()
    {
      var style = NSWindowStyle.Closable | NSWindowStyle.Resizable | NSWindowStyle.Titled;

      var rect = new CoreGraphics.CGRect(200, 1000, 1024, 768);
      _window = new NSWindow(rect, style, NSBackingStore.Buffered, false);
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

      //TODO: Replace with Xamarin.Essentials API.
      string dataDir = NSSearchPath.GetDirectories(NSSearchPathDirectory.ApplicationSupportDirectory, NSSearchPathDomain.User)[0];
      dataDir = System.IO.Path.Combine(dataDir, "Unjammit");
      // Create dataDir, if it doesnt' exist.
      if (!System.IO.Directory.Exists(dataDir))
        System.IO.Directory.CreateDirectory(dataDir);

      Jammit.Forms.App.AllowedFileTypes = new string[] { "com.pkware.zip-archive" };
      Jammit.Forms.App.DataDirectory = dataDir;
      Jammit.Forms.App.PlayerFactory = (media) =>
      {
        return new Audio.AppleJcfPlayer(media, (track, stream) =>
        {
          return new Audio.MacOSAVAudioPlayer(track, stream);
        });
      };
      Jammit.Forms.App.MediaLoader = new Model.FileSystemJcfLoader(dataDir);

      LoadApplication(new Jammit.Forms.App());

      base.DidFinishLaunching(notification);
      _window.Toolbar.Visible = false;
    }

    public override void WillTerminate(NSNotification notification)
    {
      // Insert code here to tear down your application
    }
  }
}
