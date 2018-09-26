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
using Plugin.DeviceInfo;

namespace Jammit.Forms.Client
{
  class RestClient : IClient
  {
    #region private members

    private AuthorizationStatus _authStatus;
    private double _songDownloadProgress;

    #endregion // private members

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

          var tracks = jsonObject["_embedded"]["track"] as JArray;
          foreach (var track in tracks)
          {
            result.Add(new SongInfo(
              Guid.Parse(track["id"].ToString()),
              track["artist"].ToString(),
              track["album"].ToString(),
              track["title"].ToString(),
              track["instrument"].ToString(),
              track["genre"].ToString()
            ));
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

      result.Sort(Comparer<SongInfo>.Create(
        (s1, s2) => s1.Artist.CompareTo(s2.Artist) * 10 + s1.Title.CompareTo(s2.Title)
      ));
      return result;
    }

    public async Task<Stream> DownloadSong(SongInfo song)
    {
      // https://stackoverflow.com/questions/36698677
      using (var client = new HttpClient(new HttpClientHandler(), false))
      {
        client.BaseAddress = new Uri($"{Settings.ServiceUri}/download?id={song.Id.ToString().ToUpper()}");
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
          SongDownloadProgress = e.ProgressPercentage;
        };
        client.DownloadFileCompleted += (sender, e) =>
        {
          //TODO
        };

        var uri = new Uri($"{Settings.ServiceUri}/download?id={song.Id.ToString().ToUpper()}");
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
        json.Add("id", CrossDeviceInfo.Current.Id);
        json.Add("platform", CrossDeviceInfo.Current.Platform.ToString());
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

    #endregion // IClient members
  }
}
