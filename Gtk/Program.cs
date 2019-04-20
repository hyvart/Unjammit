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

      var app = new Jammit.Forms.App();
      var window = new FormsWindow();
      window.LoadApplication(app);
      window.SetApplicationTitle("Unjammit!");
      window.Show();

      global::Gtk.Application.Run();
    }
  }
}
