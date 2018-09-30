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
      _window.Title = "Xamarin.Forms on Mac!";
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
      string dataDir = NSSearchPath.GetDirectories(NSSearchPathDirectory.LibraryDirectory, NSSearchPathDomain.User)[0];
      Jammit.Forms.App.AllowedFileTypes = new string[] { "com.pkware.zip-archive" };

      LoadApplication(
        new Jammit.Forms.App(
          dataDir,
          (media) => {
            return new Audio.AppleJcfPlayer(media, (track, stream) =>
            {
              return new Audio.MacOSAVAudioPlayer(track, stream);
            });
          },
          new Model.FileSystemJcfLoader(dataDir)
        )
      );
      base.DidFinishLaunching(notification);
    }

    public override void WillTerminate(NSNotification notification)
    {
      // Insert code here to tear down your application
    }
  }
}
