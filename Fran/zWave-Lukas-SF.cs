using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
//using Windows.Web.Http;
using Windows.UI.Popups;
using System.Net;
using System.IO;
using Windows.Networking.Sockets;
using Windows.Networking;


namespace Fran
{
    class zWave
    {
        public string _benutzer, _password, _phpsessid, _errormsg;
        public IPAddress _ipaddresse;
        public bool _error;

        public async void connect(string ipaddress)
        {
            
            _ipaddresse = IPAddress.Parse(ipaddress);
            getPPHSSID();
        }
        protected async void getPPHSSID()
        {
            //try
            //{
            
             

              HttpClient httpClient = new HttpClient();
              var headers = httpClient.DefaultRequestHeaders;
              string header = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Safari/537.36 Edge/15.15063";
              if (!headers.UserAgent.TryParseAdd(header))
              {
                  throw new Exception("Invalid header value: " + header);
              }

              Uri requestUri = new Uri("http://" + _ipaddresse.ToString() + ":8083/smarthome/#/?login=" + _benutzer + "&password=" + _password);
              HttpResponseMessage httpResponse = new HttpResponseMessage();

              try
              {
                  httpResponse = await httpClient.GetAsync(requestUri);
                  httpResponse.EnsureSuccessStatusCode();

                  requestUri = new Uri("http://192.168.178.11:8083/ZAutomation/api/v1/devices");
                  httpResponse = await httpClient.GetAsync(requestUri);
                  //var dialog = new MessageDialog(httpResponse.);
                  //await dialog.ShowAsync();
              }
              catch (Exception ex)
              {
                  _errormsg = ex.Message;
                  _error = true;
              }
        }
    }
}
