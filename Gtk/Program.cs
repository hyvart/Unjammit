using System;

using Xamarin.Forms.Platform.GTK;

namespace Jammit.Gtk
{
  class MainClass
  {
    [STAThread]
    public static void Main(string[] args)
    {
      global::Gtk.Application.Init();
      global::Xamarin.Forms.Forms.Init();

      var dataDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
      dataDir = System.IO.Path.Combine(dataDir, "Unjammit");
      // Create dataDir, if it doesnt' exist.
      if (!System.IO.Directory.Exists(dataDir))
        System.IO.Directory.CreateDirectory(dataDir);

      Jammit.Forms.App.DataDirectory = dataDir;
      Jammit.Forms.App.MediaLoader = new Model.FileSystemJcfLoader(dataDir);
      Jammit.Forms.App.PlayerFactory = async (media) => await System.Threading.Tasks.Task.Run(() =>
        new Audio.VlcJcfPlayer(media, new LibVLCSharp.Shared.MediaConfiguration[] { }, new string[] { })
      );
      Jammit.Forms.App.PlayerFactory = async (media) => await System.Threading.Tasks.Task.Run(() =>
      {
        Audio.IJcfPlayer player = null;
        try
        {
          player = new Audio.VlcJcfPlayer(media, new LibVLCSharp.Shared.MediaConfiguration[] { }, new string[] { });
        }
        catch (LibVLCSharp.Shared.VLCException)
        {
          player = new Audio.NAudioJcfPlayer(
            media,
            new Audio.StubWavePlayer(),
            System.IO.Path.Combine(Jammit.Forms.App.DataDirectory, "Tracks"),
            Forms.Resources.Assets.Stick)
          {
            TimerAction = () =>
            {
              Xamarin.Forms.Device.StartTimer(new TimeSpan(0, 0, 1), () =>
              {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => (player as Audio.NAudioJcfPlayer).NotifyPositionChanged());

                return player.State == Audio.PlaybackStatus.Playing;
              });
            }
          };
        }

        return player;
      });

      var app = new Jammit.Forms.App();
      var window = new FormsWindow();
      window.LoadApplication(app);
      window.SetApplicationTitle("Unjammit!");
      window.Show();

      try
      {
        // Best-effort load VLC core.
        // See runtime requirements at:
        // https://github.com/videolan/libvlcsharp/blob/3.x/docs/linux-setup.md
        // NOTE: Major Linux distros (Ubuntu 20.04 just released) ship libVLC 3.
        // LibVLCSharp expects libVLC 4.
        // TODO: Ship with LibVLC 4 nightly build binaries: https://nightlies.videolan.org/
        // TODO: Write a GitHub issue!
        global::LibVLCSharp.Shared.Core.Initialize();
      }
      finally
      {
        global::Gtk.Application.Run();

      }
    }
  }
}
