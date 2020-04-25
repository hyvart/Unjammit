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
        new Audio.VlcJcfPlayer(media, new LibVLCSharp.Shared.MediaConfiguration[] {}, new string[] {})
      );

      var app = new Jammit.Forms.App();
      var window = new FormsWindow();
      window.LoadApplication(app);
      window.SetApplicationTitle("Unjammit!");
      window.Show();

      global::Gtk.Application.Run();

      // See runtime requirements at:
      // https://github.com/videolan/libvlcsharp/blob/3.x/docs/linux-setup.md
      global::LibVLCSharp.Shared.Core.Initialize();
    }
  }
}
