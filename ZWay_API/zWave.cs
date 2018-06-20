using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
//using Windows.Web.Http;
using Windows.UI.Popups;
using Windows.Storage;
using System.Net.Sockets;
using System.Net;
using Windows.Storage.Streams;
using Windows.Networking.Sockets;
using Windows.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace ZWay_API
{
    public class ZWay
    {
        private string _user, _password, _port;
        private IPAddress _ipaddresse;
        private CookieCollection _zwaysession;
        private HttpClientHandler _clienthandler;
        private HttpClient _client;
        private Elemente _elements;

        public ZWay()
        {
            _zwaysession = null;
            _ipaddresse = null;
            _clienthandler = new HttpClientHandler();
            _client = new HttpClient(_clienthandler);
            if(!LogInFile_Read().GetAwaiter().GetResult())
            { 
                _port = "8083";
                _user = "";
                _password = "";
                IPAddresse = "";
            }
        }

        //Read Account and Server-Data from LogInData.txt in Projekt
        public async Task<bool> LogInFile_Read()
        {
            try
            {
                var folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                var TempFile = await folder.GetFileAsync(@"LogInData.txt");
                var readFile = await FileIO.ReadLinesAsync(TempFile);
                string content = "";
                foreach (var line in readFile)
                {
                    content += line;
                }
                LogInData meinLogin = JsonConvert.DeserializeObject<LogInData>(content);
                _port = meinLogin.port;
                _user = meinLogin.benutzer;
                _password = meinLogin.password;
                IPAddresse = meinLogin.IPAdresse;
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool Init()
        {
            try
            {
                if (_user == "" || _password == "" || _ipaddresse == null)
                {
                    return false;
                }
                else if (_zwaysession != null && _zwaysession.Count >= 1)
                {
                    return true;
                }
                else
                {
                    getZWaySession();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected void getZWaySession()
        {

            var headers = _client.DefaultRequestHeaders;
            string contType = "application/json";
            if (!headers.Accept.TryParseAdd(contType))
            {
                throw new Exception("Invalid header value: " + contType);
            }

            var body = new Dictionary<string, string>()
            {
                {"form",  "true"},
                {"login", _user},
                {"password", _password},
                {"keepme", "false"},
                {"default_ui", "1"}
            };

            var content = new FormUrlEncodedContent(body);
            try
            {
                var response = _client.PostAsync("http://" + _ipaddresse + ":" + _port + "/ZAutomation/api/v1/login", content).Result;
                response.EnsureSuccessStatusCode();
                _zwaysession = _clienthandler.CookieContainer.GetCookies(new Uri("http://" + _ipaddresse + ":" + _port + "/"));
                if (_zwaysession.Count >= 1)
                {
                    //Verbunden = true;
                }
                else
                {
                    throw new Exception("Session can't be opened!");
                }
            }
            catch (Exception e)
            {
                throw new Exception("Session can't be opened!\n" + e.Message);
            }
        }

        public void getAllDevices()
        {
            try
            {
                var response = _client.GetAsync("http://" + _ipaddresse + ":" + _port + "/ZAutomation/api/v1/devices").Result;
                response.EnsureSuccessStatusCode();
                var responseMessage = response.Content.ReadAsStringAsync();
                _elements = new Elemente();
                JObject JsonResponse = JObject.Parse(responseMessage.Result);
                JsonResponse = JsonResponse["data"].Value<JObject>();
                _elements.structureChanged = JsonResponse["structureChanged"].Value<bool>();
                _elements.updateTime = JsonResponse["updateTime"].Value<string>();
                JArray JsonResponseArray = JsonResponse["devices"].Value<JArray>();
                _elements.devices = JsonConvert.DeserializeObject<List<Devices>>(JsonConvert.SerializeObject(JsonResponseArray), new DevicesConverter());
                //All_Elements.devices = JsonConvert.DeserializeObject<Devices[]>(test,new DevicesConverter());
                //response = Client.GetAsync("http://" + _ipaddresse + ":" + _port + "/ZAutomation/api/v1/locations").Result;
                //response.EnsureSuccessStatusCode();
                //System.Net.Http.HttpResponseMessage
                //string responseMessage;// = new HttpResponseMessage();
                //responseMessage = response.Content.ReadAsStringAsync();
                //LocationsLink tmploc = JsonConvert.DeserializeObject<LocationsLink>(responseMessage.Result);
                //All_Elements = tmpdev.data;
                //All_Elements.locations = tmploc.data;
            }
            catch (Exception e)
            {
                throw new Exception("Error by getting all devices from ZWay-Server!\n" + e.Message);
            }
        }

        public void CommandRequest(string path)
        {
            var response = _client.GetAsync("http://" + _ipaddresse + ":" + _port + "/ZAutomation/api/v1/devices/" + path).Result;
            response.EnsureSuccessStatusCode();
            var responseMessage = response.Content.ReadAsStringAsync();
        }

        public class Elemente
        {
            public bool structureChanged = false;
            public string updateTime = "";
            public List<Devices> devices = new List<Devices>();
            public Location[] locations;
            
        }
        public class Location
        {
            public int id = -1;
            public string title = "";
        }
        public class LogInData
        {
            public string port = "";
            public string benutzer = "";
            public string password = "";
            public string IPAdresse = "";
        }

        public string User { get { return _user; } set { _user = value; } }
        public string Password { get { return _password; } set { _password = value; } }
        public string Port { get { return _port; } set { _port = value; } }
        public string IPAddresse { get { return _ipaddresse.ToString(); } set {try{_ipaddresse = IPAddress.Parse(value);}catch{_ipaddresse = null;}} }
        public bool Connected { get { return _zwaysession.Count >= 1; } }
        public Elemente Elements { get { return _elements; } }
    }
}
