using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Jammit.Forms.Views
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class ScoreSelector : ContentView
  {
    private SortedSet<string> _instrumentsSet = new SortedSet<string>();
    private SortedSet<string> _typesSet = new SortedSet<string>();
    private IDictionary<string, Model.ScoreInfo> _scores = new Dictionary<string, Model.ScoreInfo>(4);
    private string _selectedInstrument;
    private string _selectedType;
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
          _scores[score.ToString()] = score;

          if (_instrumentsSet.Add(score.Track.Title))
          {
            var rb = new RadioButton
            {
              GroupName = "Instruments",
              Content = score.Track.Title,
            };
            rb.CheckedChanged += Instrument_CheckedChanged;
            if (Device.RuntimePlatform == Device.iOS)
              rb.FontSize = 14; // Small

            InstrumentsLayout.Children.Add(rb);

            if (InstrumentsLayout.Children.Count == 1)
              rb.IsChecked = true;//TODO: Override via settings
          }

          if (_typesSet.Add(score.Type))
          {
            var rb = new RadioButton
            {
              GroupName = "Scores",
              Content = score.Type
            };
            rb.CheckedChanged += Types_CheckedChanged;
            if (Device.RuntimePlatform == Device.iOS)
              rb.FontSize = 14; // Small

            TypesLayout.Children.Add(rb);

            if (TypesLayout.Children.Count == 1)
              rb.IsChecked = true;//TODO: Override via settings
          }
        }
      }
    }

    private void Instrument_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
      var rb = sender as RadioButton;
      if (rb.IsChecked)
      {
        _selectedInstrument = rb.Content.ToString();

        if (!string.IsNullOrEmpty(_selectedType))
          SelectedScore = _scores[$"{_selectedInstrument} - {_selectedType}"];
      }
    }

    private void Types_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
      var rb = sender as RadioButton;
      if (rb.IsChecked)
      {
        _selectedType = rb.Content.ToString();

        if (!string.IsNullOrEmpty(_selectedInstrument))
          SelectedScore = _scores[$"{_selectedInstrument} - {_selectedType}"];
      }
    }

    public event EventHandler SelectedScoreChanged;
  }
}
