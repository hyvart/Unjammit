using System;
using Xamarin.Forms;

namespace Unjammit.Gtk
{
  class MainClass
  {
    [STAThread]
    public static void Main(string[] args)
    {
      global::Gtk.Application.Init();
//      Forms.Init();

//      var app = new Unjammit.Forms.App();
//      var window = new FormsWindow();
//      window.LoadApplication(app);
//      window.SetApplicationTitle("Unjammit!");
//      window.Show();

      global::Gtk.Application.Run();
    }
  }
}
