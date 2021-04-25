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
  class RestClient : IClient
  {
    #region private members

    private AuthorizationStatus _authStatus;
    private double _songDownloadProgress;

    #endregion  private members

    #region INotifyPropertyChanged members

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    #region IClient members

    public async Task<List<SongInfo>> LoadCatalog()
    {
      var result = new List<SongInfo>();

      using (var client = new HttpClient())
      {
        client.BaseAddress = new Uri($"{Settings.ServiceUri}/track");
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Accept", "application/json");

        var response = await client.GetAsync(client.BaseAddress.AbsoluteUri);
        if (response.IsSuccessStatusCode)
        {
          var jsonString = await response.Content.ReadAsStringAsync();
          var jsonObject = JObject.Parse(jsonString);

          var songs = jsonObject["_embedded"]["track"] as JArray;
          foreach (var song in songs)
          {
            result.Add(new SongInfo()
            {
              Sku = song["sku"].ToString(),
              Artist = song["artist"].ToString(),
              Album = song["album"].ToString(),
              Title = song["title"].ToString(),
              Instrument = song["instrument"].ToString(),
              Genre = song["genre"].ToString()
            });
          }
        } // if Succeeded response
        else
        {
          string suffix = "";
          switch (response.StatusCode)
          {
            case System.Net.HttpStatusCode.NotFound:
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
        (s1, s2) => string.Compare(s1.Artist,     s2.Artist,      StringComparison.Ordinal) * 100 +
                    string.Compare(s1.Title,      s2.Title,       StringComparison.Ordinal) *  10 +
                    string.Compare(s1.Instrument, s2.Instrument,  StringComparison.Ordinal) *   1
      ));
      return result;
    }

    public async Task<Stream> DownloadSong(SongInfo song)
    {
      // https://stackoverflow.com/questions/36698677
      using (var client = new HttpClient(new HttpClientHandler(), false))
      {
        client.BaseAddress = new Uri($"{Settings.ServiceUri}/download?id={song.Sku}");
        client.DefaultRequestHeaders.Clear();

        var response = await client.GetAsync(client.BaseAddress.AbsoluteUri);
        var contentLength = response.Content.Headers.ContentLength;//TODO: bind to property.

        return await response.Content.ReadAsStreamAsync();
      }
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

        var uri = new Uri($"{Settings.ServiceUri}/download?id={song.Sku}");
        await client.DownloadFileTaskAsync(uri, path);
      }
    }

    public async Task RequestAuthorization()
    {
      using (var cliente = new HttpClient())
      {
        cliente.BaseAddress = new Uri($"{Settings.ServiceUri}/register-device");
        cliente.DefaultRequestHeaders.Clear();
        cliente.DefaultRequestHeaders.Add("Accept", "application/json");

        var json = new JObject();
        // See https://montemagno.com/unique-device-id-for-mobile-apps/
        // Disabled. Full dependency was required only for the line below.
        //json.Add("id", Plugin.DeviceInfo.CrossDeviceInfo.Current.Id);

        json.Add("platform", Xamarin.Forms.Device.RuntimePlatform);
        var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

        var response = await cliente.PostAsync(cliente.BaseAddress.AbsoluteUri, content);
        var responseContent = response.Content;
        var responseContentString = await responseContent.ReadAsStringAsync();
        var responseJson = JObject.Parse(responseContentString);
        var authorization = responseJson["authorization"].ToString();

        switch (authorization)
        {
          case "0":
            AuthStatus = AuthorizationStatus.Unknown; break;
          case "1":
            AuthStatus = AuthorizationStatus.Requested; break;
          case "2":
            AuthStatus = AuthorizationStatus.Rejected; break;
          case "3":
            AuthStatus = AuthorizationStatus.Approved; break;
          default:
            throw new Exception($"Unknown autorization status code [{authorization}].");
        }
      } // Using HttpClient
    }

    public AuthorizationStatus AuthStatus
    {
      get
      {
        return _authStatus;
      }

      private set
      {
        if (_authStatus != value)
        {
          _authStatus = value;
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AuthStatus"));
        }
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

    #endregion  IClient members
  }
}
