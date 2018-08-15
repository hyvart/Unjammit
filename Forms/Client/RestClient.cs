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
  class RestClient : IClient, INotifyPropertyChanged
  {
    #region private members

    private AuthorizationStatus _authStatus;

    #endregion // private members

    #region INotifyPropertyChanged members

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    #region IClient methods

    public async Task<List<SongInfo>> LoadCatalog()
    {
      var result = new List<SongInfo>();

      using (var cliente = new HttpClient())
      {
        cliente.BaseAddress = new Uri($"{Settings.ServiceUri}/track");
        cliente.DefaultRequestHeaders.Clear();
        cliente.DefaultRequestHeaders.Add("Accept", "application/json");

        var response = await cliente.GetAsync(cliente.BaseAddress.AbsoluteUri);
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
      }

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

    #endregion

    #region IClient properties

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

    #endregion
  }
}
