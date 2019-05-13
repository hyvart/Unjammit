using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Jammit.Client;
using Jammit.Model;
using Newtonsoft.Json.Linq;

namespace Jammit.Forms.Client
{
  public class BasicHttpClient : IClient
  {
    #region private members

    private AuthorizationStatus _authStatus = AuthorizationStatus.Unknown;
    private double _songDownloadProgress;

    #endregion

    #region IClient

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    public AuthorizationStatus AuthStatus
    {
      get
      {
        return _authStatus;
      }

      set
      {
        _authStatus = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AuthStatus"));
      }
    }

    public double SongDownloadProgress
    {
      get
      {
        return _songDownloadProgress;
      }

      private set
      {
        _songDownloadProgress = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SongDownloadProgress"));
      }
    }

    public Task<Stream> DownloadSong(SongInfo song)
    {
      throw new NotImplementedException();
    }

    public async Task DownloadSong(SongInfo song, string path)
    {
      // Reset Download progress.
      SongDownloadProgress = 0;

      using (var client = new System.Net.WebClient())
      {
        client.DownloadProgressChanged += (sender, e) =>
        {
          if (e.TotalBytesToReceive >= 0)
          {
            SongDownloadProgress = (double)e.BytesReceived / e.TotalBytesToReceive;

            return;
          }

          var contentLength = long.Parse(client.ResponseHeaders[System.Net.HttpResponseHeader.ContentLength]);
          if (contentLength > 0)
          {
            SongDownloadProgress = (double)e.BytesReceived / contentLength;

            return;
          }

          // Unknown content length. Throw error.
          throw new HttpRequestException("Unknown download size.");
        };
        client.DownloadFileCompleted += (sender, e) =>
        {
          //TODO
        };

        var uri = new Uri($"{Settings.ServiceUri}/{song.Id.ToString().ToUpper()}.zip");
        await client.DownloadFileTaskAsync(uri, path);
      }
    }

    public async Task<List<SongInfo>> LoadCatalog()
    {
      var result = new List<SongInfo>();

      using (var client = new HttpClient())
      {
        client.BaseAddress = new Uri($"{Settings.ServiceUri}");
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Accept", "application/json");

        var response = await client.GetAsync(client.BaseAddress.AbsoluteUri + "/catalog.json");
        AuthStatus = AuthorizationStatus.Requested;
        if (response.IsSuccessStatusCode)
        {
          AuthStatus = AuthorizationStatus.Approved;
          var jsonString = await response.Content.ReadAsStringAsync();
          var jsonObject = JObject.Parse(jsonString);

          var songs = jsonObject["songs"] as JArray;
          foreach (var song in songs)
          {
            result.Add(new SongInfo()
            {
              Id = Guid.Parse(song["id"].ToString()),
              Sku = song["sku"].ToString(),
              Artist = song["artist"].ToString(),
              Album = song["album"].ToString(),
              Title = song["title"].ToString(),
              Instrument = song["instrument"].ToString(),
              Genre = song["genre"].ToString()
            });
          }
        } // response.IsSuccessStatusCode
        else
        {
          string suffix = "";
          switch (response.StatusCode)
          {
            case System.Net.HttpStatusCode.NotFound:
              suffix = $": {client.BaseAddress.AbsoluteUri}";
              break;

            case System.Net.HttpStatusCode.Unauthorized:
              AuthStatus = AuthorizationStatus.Rejected;
              suffix = $": {client.BaseAddress.AbsoluteUri}";
              break;

            default:
              break;
          }

          throw new HttpRequestException(response.ReasonPhrase + suffix);
        } // Response failed
      }

      // See Mode.FolderLibrary.InitCache
      result.Sort(Comparer<SongInfo>.Create(
        (s1, s2) => string.Compare(s1.Artist, s2.Artist, StringComparison.Ordinal) * 100 +
                    string.Compare(s1.Title, s2.Title, StringComparison.Ordinal) * 10 +
                    string.Compare(s1.Instrument, s2.Instrument, StringComparison.Ordinal) * 1
      ));
      return result;
    }

    public Task RequestAuthorization()
    {
      throw new NotImplementedException();
    }

    #endregion // IClient
  }
}
