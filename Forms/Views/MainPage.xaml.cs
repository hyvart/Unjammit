using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Jammit.Forms.Views
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class MainPage : MasterDetailPage
  {
    public MainPage ()
    {
      InitializeComponent();

      MasterBehavior = MasterBehavior.Popover;
    }
  }
}