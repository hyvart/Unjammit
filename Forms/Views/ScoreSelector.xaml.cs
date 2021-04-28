using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Jammit.Forms.Views
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class ScoreSelector : ContentView
  {
    private Model.ScoreInfo _selectedScore;

    public ScoreSelector()
    {
      InitializeComponent();
    }

    public Model.ScoreInfo SelectedScore
    {
      get
      {
        return _selectedScore;
      }

      private set
      {
        _selectedScore = value;

        SelectedScoreChanged?.Invoke(this, EventArgs.Empty);
      }
    }

    public IList<Model.ScoreInfo> Items
    {
      set
      {
        foreach (var score in value)
        {
          var rb = new RadioButton
          {
            GroupName = "ScoreSelector",
            Content = score.ToString(),
            Value = score,
          };
          rb.CheckedChanged += Items_CheckedChanged;

          SelectorsLayout.Children.Add(rb);
        }

        SelectedScore = value[0];
      }
    }

    public event EventHandler SelectedScoreChanged;

    private void Items_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
      var button = (sender as RadioButton);

      if (button.IsChecked)
        SelectedScore = button.Value as Model.ScoreInfo;
    }
  }
}
