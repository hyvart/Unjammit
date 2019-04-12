using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jammit.Model
{
  public interface ILibrary
  {
    List<SongInfo> Songs { get; }

    [Obsolete("Use AddSong(System.IO.Stream) instead.")]
    Task AddSong(SongInfo song);

    SongInfo AddSong(System.IO.Stream conentStream);

    void RemoveSong(SongInfo song);
  }
}
