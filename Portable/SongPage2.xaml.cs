using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jammit.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Jammit.Portable
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class SongPage2 : ContentPage
  {
    public SongPage2 (SongInfo song)
    {
      // Needed to actually bind local properties.
      BindingContext = this;

      Song = song;
      Jcf = App.MediaLoader.LoadMedia(song.Id);

      InitializeComponent();
    }

    public SongInfo Song { get; set; }

    public Model2.JcfMedia Jcf { get; set; }

  }
}