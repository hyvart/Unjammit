﻿using System;
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
  public partial class SongPage : ContentPage
  {
    /// <summary>
    /// Score information (Instrument track name and type).
    /// </summary>
    public struct ScoreInfo
    {
      public string TrackName;
      public string Type;
      public TrackInfo Track;
      public override string ToString() => $"{TrackName} - {Type}";
    }

    #region Bindable properteis

    public static readonly BindableProperty PlayerProperty =
      BindableProperty.Create("Player", typeof(Audio.ISongPlayer), typeof(Audio.ISongPlayer), null, BindingMode.TwoWay);

    public static readonly BindableProperty LengthProperty =
      BindableProperty.Create("Length", typeof(TimeSpan), typeof(TimeSpan), TimeSpan.FromSeconds(600), BindingMode.TwoWay);

    public static readonly BindableProperty PositionProperty =
      BindableProperty.Create("Position", typeof(TimeSpan), typeof(TimeSpan), TimeSpan.Zero, BindingMode.TwoWay);

    Audio.ISongPlayer Player
    {
      get
      {
        return (Audio.ISongPlayer)GetValue(PlayerProperty);
      }

      set
      {
        SetValue(PlayerProperty, value);
      }
    }

    TimeSpan Length => Player.Length;

    public TimeSpan Position
    {
      get
      {
        return Player.Position;
      }

      set
      {
        Player.Position = value;
      }
    }

    #endregion // Bindable properteis

    public SongInfo Song { get; set; }

    public ISong SongContents { get; set; }

    public List<ScoreInfo> ScoresInfo { get; private set; }

    public SongPage(SongInfo song)
    {
      BindingContext = this; // Needed to actually bind local properties.
      Song = song;
      SongContents = new FolderSong(song);

      ScoresInfo = new List<ScoreInfo>();
      var tracks = SongContents.Tracks.Where(t => t is NotatedTrackInfo);
      foreach (var track in tracks)
      {
        var notated = track as NotatedTrackInfo;
        if (notated.NotationPages > 0)
          ScoresInfo.Add(new ScoreInfo { TrackName = notated.Title, Type = "Score", Track = notated });
        if (notated.TablaturePages > 0)
          ScoresInfo.Add(new ScoreInfo { TrackName = notated.Title, Type = "Tablature", Track = notated });
      }

      Player = App.SongPlayerFactory(SongContents);

      InitializeComponent();

      ScorePicker.SelectedIndex = 0;//TODO: Set up in markup (XAML)?
      var scoreInfo = (ScoreInfo)ScorePicker.SelectedItem;
      ScoreImage.Source = ImageSource.FromStream(() => { return SongContents.GetNotation(scoreInfo.Track)[0]; });
      AlbumImage.Source = ImageSource.FromStream(() => { return SongContents.GetCover(); });
    }

    private void SongPage_Close(object sender, EventArgs e)
    {
      Navigation.PopModalAsync();
    }

    private void ScorePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
      var scoreInfo = (ScoreInfo)ScorePicker.SelectedItem;
      if (scoreInfo.Type == "Score")
        ScoreImage.Source = ImageSource.FromStream(() => { return SongContents.GetNotation(scoreInfo.Track)[0]; });
      else // Tablature
        ScoreImage.Source = ImageSource.FromStream(() => { return SongContents.GetTablature(scoreInfo.Track)[0]; });
    }

    private void PlayButton_Clicked(object sender, EventArgs e)
    {
      if (Player.State == Audio.PlaybackStatus.Playing)
      {
        Player.Stop();
        PlayButton.Text = "Play";
      }
      else
      {
        Player.Play();
        PlayButton.Text = "Stop";
      }
    }
  }
}
