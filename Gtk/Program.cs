using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.GTK;

namespace Jammit.Gtk
{
  class MainClass
  {
    [STAThread]
    public static void Main(string[] args)
    {
      global::Gtk.Application.Init();
      Xamarin.Forms.Forms.Init();

      global::LibVLCSharp.Forms.Shared.LibVLCSharpFormsRenderer.Init();
      global::LibVLCSharp.Shared.Core.Initialize();

      var dataDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
      dataDir = System.IO.Path.Combine(dataDir, "Unjammit");
      // Create dataDir, if it doesnt' exist.
      if (!System.IO.Directory.Exists(dataDir))
        System.IO.Directory.CreateDirectory(dataDir);

      Jammit.Forms.App.DataDirectory = dataDir;
      Jammit.Forms.App.MediaLoader = new Model.FileSystemJcfLoader(dataDir);
      Jammit.Forms.App.PlayerFactory = (media) => new Audio.VlcJcfPlayer(media);

      var app = new Jammit.Forms.App();
      var window = new FormsWindow();
      window.LoadApplication(app);
      window.SetApplicationTitle("Unjammit!");
      window.Show();

      global::Gtk.Application.Run();
    }
  }
}
