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

    //TODO: Deprecate?
    Task AddSong(SongInfo song);

    SongInfo AddSong(System.IO.Stream conentStream);

    void RemoveSong(SongInfo song);
  }
}
