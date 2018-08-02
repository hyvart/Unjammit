using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jammit.Model;

namespace Jammit.Audio
{
  public class VlcJcfPlayer : IJcfPlayer
  {
    #region IJcfPlayer members

    public async void Play()
    {
    }

    public void Pause()
    {
      throw new NotImplementedException();
    }

    public void Stop()
    {
      throw new NotImplementedException();
    }

    public uint GetVolume(PlayableTrackInfo track)
    {
      throw new NotImplementedException();
    }

    public void SetVolume(PlayableTrackInfo track, uint volume)
    {
      throw new NotImplementedException();
    }

    public TimeSpan Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public TimeSpan Length => throw new NotImplementedException();

    public PlaybackStatus State => throw new NotImplementedException();

    #endregion // IJcfPlayer members
  }
}
