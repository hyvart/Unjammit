using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using ScoreInfo = Jammit.Model.ScoreInfo;

namespace Jammit.Forms.Views
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class ScoreSelector : ContentView
  {
    private ScoreInfo _selectedScore;
    private ScoreInfo[] _scoreInfos = new ScoreInfo[4];
    private int _selectedInstrumentIndex = 0;
    private int _selectedTypeIndex = 0;

    public ScoreSelector()
    {
      InitializeComponent();
    }

    public ScoreInfo SelectedScore
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

    public IList<ScoreInfo> Scores
    {
      set
      {
        SelectedScore = value[0];

        var instruments = new Dictionary<string, int>(2);
        var types = new Dictionary<string, int>(2);
        foreach (var score in value)
        {
          if (!instruments.ContainsKey(score.Track.Title))
          {
            // This assumes there is at most 2 instruments and at most 2 types.
            // Should this constraint change, a second pass may be needed to determine the size, ergo, number of bits.
            instruments[score.Track.Title] = instruments.Count << 1;

            //var x = Application.Current.Resources["ScoreSelectorTemplate"] as Xamarin.Forms.ControlTemplate;

            var rb = new RadioButton
            {
              GroupName = "Instruments",
              Content = score.Track.Title,
              Value = instruments[score.Track.Title],
              ControlTemplate = Device.macOS != Device.RuntimePlatform ?
                ScoreSelectorTemplate : null,
              TextColor = Color.White
            };
            rb.CheckedChanged += Instrument_CheckedChanged;
            if (InstrumentsLayout.Children.Count == 0)
              rb.IsChecked = true;//TODO: Override via settings

            InstrumentsLayout.Children.Add(rb);
          }

          if (!types.ContainsKey(score.Type))
          {
            types[score.Type] = types.Count << 0;

            var rb = new RadioButton
            {
              GroupName = "Scores",
              Content = score.Type,
              Value = types[score.Type],
              ControlTemplate = Device.macOS != Device.RuntimePlatform ?
                ScoreSelectorTemplate : null,
              TextColor = Color.White
            };
            rb.CheckedChanged += Types_CheckedChanged;
            if (TypesLayout.Children.Count == 0)
              rb.IsChecked = true;//TODO: Override via settings

            TypesLayout.Children.Add(rb);
          }

          _scoreInfos[instruments[score.Track.Title] | types[score.Type]] = score;
        }
      }
    }

    public event EventHandler SelectedScoreChanged;

    private void Instrument_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
      var rb = sender as RadioButton;
      if (rb.IsChecked)
      {
        _selectedInstrumentIndex = (int)rb.Value;
        if (_scoreInfos[_selectedInstrumentIndex | _selectedTypeIndex] != null)
          SelectedScore = _scoreInfos[_selectedInstrumentIndex | _selectedTypeIndex];
      }
    }

    private void Types_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
      var rb = sender as RadioButton;
      if (rb.IsChecked)
      {
        _selectedTypeIndex = (int)rb.Value;
        if (_scoreInfos[_selectedInstrumentIndex | _selectedTypeIndex] != null)
          SelectedScore = _scoreInfos[_selectedInstrumentIndex | _selectedTypeIndex];
      }
    }
  }
}
