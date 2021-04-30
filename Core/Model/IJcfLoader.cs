using System;
using System.Collections.Generic;
using System.Text;

namespace Jammit.Model
{
  public interface IJcfLoader
  {
    JcfMedia LoadMedia(Model.SongInfo song);

    System.IO.Stream LoadNotation(JcfMedia media, ScoreInfo score, uint index);

    System.IO.Stream LoadAlbumCover(JcfMedia media);

    void LoadFullSongInfo(SongInfo song, string songPath);
  }
}
