using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jammit.Client
{
  public interface IClient : INotifyPropertyChanged
  {
    Task<List<Model.SongInfo>> LoadCatalog();

    Task<System.IO.Stream> DownloadSong(Model.SongInfo song);

    Task DownloadSong(Model.SongInfo song, string path);

    Task RequestAuthorization();

    #region Properties

    AuthorizationStatus AuthStatus { get; }

    int SongDownloadProgress { get; }

    #endregion
  }
}
