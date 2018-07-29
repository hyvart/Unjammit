using System;
using System.Collections.Generic;
using System.Text;

namespace Jammit.Model2
{
  public interface IJcfLoader
  {
    JcfMedia LoadMedia(Guid id);
  }
}
