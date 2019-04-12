using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Jammit.Forms.Views
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class MainPage : MasterDetailPage
  {
    public MainPage()
    {
      InitializeComponent();

      (Master as MenuPage).MenuListView.ItemSelected += OnMenuItemSelected;
    }

    async void OnMenuItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
      var item = e.SelectedItem as Models.HomeMenuItem;
      if (item != null)
      {
        try
        {
          Detail = new NavigationPage(Activator.CreateInstance(item.TargetType) as Page);
        }
        catch (Exception ex)
        {
          string message;
          if (ex.InnerException is UriFormatException)
            message = $"Invalid server address: [ {Settings.ServiceUri} ]";
          else if (ex.InnerException != null)
            message = ex.InnerException.Message;
          else
            message = ex.Message;

          await DisplayAlert("Error", message, "Cancel");
        }
        (Master as MenuPage).MenuListView.SelectedItem = null;
      }
    }
  }
}