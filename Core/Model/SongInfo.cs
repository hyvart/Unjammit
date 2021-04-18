using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jammit.Model
{
  public class SongInfo
  {
    public SongInfo() { }

    public SongInfo(Guid id, string artist, string album, string title, string instrument, string genre)
    {
      Id = id;
      Artist = artist;
      Album = album;
      Title = title;
      Instrument = instrument;
      Genre = genre;
    }

    public Guid Id { get; set; }
    public string Sku { get; set; }
    public string Artist { get; set; }
    public string Album { get; set; }
    public string Title { get; set; }
    public string Instrument { get; set; }
    public string Genre { get; set; }

    // Other properties
    public string Tuning { get; set; }
    public string Tempo { get; set; }
    public string WrittenBy { get; set; }
    public string PublishedBy { get; set; }
    public string CourtesyOf { get; set; }

    public override string ToString()
    {
      return $"{Artist} - {Title} [{Instrument}]";
    }
  }
}
