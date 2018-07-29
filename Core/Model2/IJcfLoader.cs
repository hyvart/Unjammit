using System;
using System.Collections.Generic;
using System.Text;

namespace Jammit.Model2
{
  public interface IJcfLoader
  {
    JcfMedia LoadMedia(Guid guid);

    System.IO.Stream LoadNotation(JcfMedia media, ScoreInfo track, uint index);

    System.IO.Stream LoadAlbumCover(JcfMedia media);
  }
}
