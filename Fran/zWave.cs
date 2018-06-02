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


namespace Fran
{
    class zWave
    {
        private string _benutzer, _password;
        private static string _port, _errormsg;
        private static IPAddress _ipaddresse;
        private static bool _error,_verbunden;
        private CookieCollection ZWaySession;
        private HttpClientHandler ClientHandler;
        private static HttpClient Client;
        public Elemente All_Elements;

        public zWave()
        {
            ZWaySession = null;
            _ipaddresse = null;
            _verbunden = false;
            _error = false;
            _errormsg = "";
            ClientHandler = new HttpClientHandler();
            Client = new HttpClient(ClientHandler);
            if(!LogInFile_Read().GetAwaiter().GetResult())
            { 
                _port = "";
                _benutzer = "";
                _password = "";
                IPAdresse = "";
            }
        }

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
                _benutzer = meinLogin.benutzer;
                _password = meinLogin.password;
                IPAdresse = meinLogin.IPAdresse;
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public bool Init()
        {
            try
            {
                /*if (ZWaySession == null)
                {
                    return false;
                }*/
                getZWaySession();
                return true;
            }
            catch (Exception e)
            {
                _errormsg = e.Message;
                _error = true;
                return false;
            }
        }
        protected void getZWaySession()
        {

            var headers = Client.DefaultRequestHeaders;
            string header = "application/json";
            if (!headers.Accept.TryParseAdd(header))
            {
                throw new Exception("Invalid header value: " + header);
            }


            var body = new Dictionary<string, string>()
            {
                {"form",  "true"},
                {"login", _benutzer},
                {"password", _password},
                {"keepme", "false"},
                {"default_ui", "1"}
            };

            var content = new FormUrlEncodedContent(body);
            try
            {
                var response = Client.PostAsync("http://" + _ipaddresse + ":" + _port + "/ZAutomation/api/v1/login", content).Result;
                response.EnsureSuccessStatusCode();
                ZWaySession = ClientHandler.CookieContainer.GetCookies(new Uri("http://" + _ipaddresse + ":" + _port + "/"));
                if (ZWaySession.Count >= 1)
                {
                    _verbunden = true;
                    //StorageFolder localCacheFolder = ApplicationData.Current.LocalCacheFolder;
                    //StorageFile sampleFile = await localCacheFolder.CreateFileAsync("dataFile.txt");
                    //await FileIO.WriteTextAsync(sampleFile, ZWaySession.ToString());
                }
                else
                {
                    _verbunden = false;
                    _error = true;
                    _errormsg += "Die Sitzung konnte nicht geöffnet werden!";
                }
            }
            catch (Exception e)
            {
                _errormsg = e.Message;
                _error = true;
            }
        }
        public void getAllDevices()
        {
            try
            {
                var response = Client.GetAsync("http://" + _ipaddresse + ":" + _port + "/ZAutomation/api/v1/devices").Result;
                response.EnsureSuccessStatusCode();
                //System.Net.Http.HttpResponseMessage
                //string responseMessage;// = new HttpResponseMessage();
                var responseMessage = response.Content.ReadAsStringAsync();
                DevicesLink tmpdev = JsonConvert.DeserializeObject<DevicesLink>(responseMessage.Result);
                response = Client.GetAsync("http://" + _ipaddresse + ":" + _port + "/ZAutomation/api/v1/locations").Result;
                response.EnsureSuccessStatusCode();
                //System.Net.Http.HttpResponseMessage
                //string responseMessage;// = new HttpResponseMessage();
                responseMessage = response.Content.ReadAsStringAsync();
                LocationsLink tmploc = JsonConvert.DeserializeObject<LocationsLink>(responseMessage.Result);
                All_Elements = tmpdev.data;
                All_Elements.locations = tmploc.data;
            }
            catch (Exception e)
            {
                _errormsg = e.Message;
                _error = true;
            }
        }
        
        class DevicesLink
        {
            public int code = -1;
            public string message = "";
            public string error = "";
            public Elemente data = null;
        }
        class LocationsLink
        {
            public int code = -1;
            public string message = "";
            public string error = "";
            public Location[] data =null;
        }
        public class Elemente
        {
            public bool structureChanged = false;
            public string updateTime = "";
            public Device[] devices = null;
            public Location[] locations;
            public string GetLocation(int roomid)
            {
                for (int i = 0; i < locations.Length; i++)
                {
                    if(locations[i].id == roomid)
                    {
                        return locations[i].title != "globalRoom" ? locations[i].title : "";
                    }
                }
                return "";
            }
            public Device GetDeviceById(string ID)
            {
                for (int i = 0; i < devices.Length; i++)
                {
                    if (devices[i].id == ID)
                    {
                        return devices[i];
                    }
                }
                return null;
            }
        }
        public class Location
        {
            public int id = -1;
            public string title = "";
        }
        public class Device
        {
            public string creationTime = "";
            public string creatorId = "";
            public string deviceType = "";
            public icons customIcons = null;
            public int h = -1;
            public bool hasHistory = false;
            public string id = "";
            public int location = -1;
            public metrics metrics = null;
            public orders order = null;
            public bool permanently_hidden = false;
            public string probeType = "";
            public string[] tags = null;
            public bool visibility = false;
            public string updateTime = "";
            public string iconpath
            {
                get
                {
                    if (metrics.isFailed)
                    {
                        return "http://" + _ipaddresse + ":" + _port + "/smarthome/storage/img/icons/placeholder.png";
                    }
                    else if (metrics.icon != String.Empty && metrics.icon.IndexOf('/') != -1)
                    {
                        return "http://" + _ipaddresse + ":" + _port + metrics.icon;
                    }
                    else
                    {
                        return "http://" + _ipaddresse + ":" + _port + "/smarthome/storage/img/icons/" + metrics.icon + "-" + metrics.level + ".png";
                    }
                }
            }

        }

        public class SwitchDevice : Device
        {
            public new SwitchMetrics metrics = null;
            public new string iconpath
            {
                get
                {
                    if (metrics.isFailed)
                    {
                        return "http://" + _ipaddresse + ":" + _port + "/smarthome/storage/img/icons/placeholder.png";
                    }
                    else if (customIcons.level != null)
                    {
                        if (metrics.IsOn)
                        {
                            return "http://" + _ipaddresse + ":" + _port + "/smarthome/user/icons/" + customIcons.level.on;
                        }
                        else
                        {
                            return "http://" + _ipaddresse + ":" + _port + "/smarthome/user/icons/" + customIcons.level.off;
                        }
                    }
                    else if (metrics.icon != String.Empty && metrics.icon.IndexOf('/') != -1)
                    {
                        return "http://" + _ipaddresse + ":" + _port + metrics.icon;
                    }
                    else
                    {
                        return "http://" + _ipaddresse + ":" + _port + "/smarthome/storage/img/icons/" + metrics.icon + "-" + metrics.level + ".png";
                    }
                }
            }
            public async void TurnOn()
            {
                try
                {
                    var response = await Client.GetAsync("http://" + _ipaddresse + ":" + _port + "/ZAutomation/api/v1/devices/" + this.id + "/command/on");
                    response.EnsureSuccessStatusCode();
                    //System.Net.Http.HttpResponseMessage
                }
                catch (Exception e)
                {
                    _errormsg = e.Message;
                    _error = true;
                    var dialog = new MessageDialog(_errormsg);
                    await dialog.ShowAsync();
                }
            }
            public async void TurnOff()
            {
                try
                {
                    var response = await Client.GetAsync("http://" + _ipaddresse + ":" + _port + "/ZAutomation/api/v1/devices/" + this.id + "/command/off");
                    response.EnsureSuccessStatusCode();
                    //System.Net.Http.HttpResponseMessage
                }
                catch (Exception e)
                {
                    _errormsg = e.Message;
                    _error = true;
                    var dialog = new MessageDialog(_errormsg);
                    await dialog.ShowAsync();
                }
            }
        };

        public class SensorDevice : Device
        {
            public new SensorMetrics metrics = null;
            public new string iconpath
            {
                get
                {
                    if (metrics.isFailed)
                    {
                        return "http://" + _ipaddresse + ":" + _port + "/smarthome/storage/img/icons/placeholder.png";
                    }
                    else if (customIcons.level != null)
                    {
                        if (metrics.IsOn)
                        {
                            return "http://" + _ipaddresse + ":" + _port + "/smarthome/user/icons/" + customIcons.level.on;
                        }
                        else
                        {
                            return "http://" + _ipaddresse + ":" + _port + "/smarthome/user/icons/" + customIcons.level.off;
                        }
                    }
                    else if (metrics.icon != String.Empty && metrics.icon.IndexOf('/') != -1)
                    {
                        return "http://" + _ipaddresse + ":" + _port + metrics.icon;
                    }
                    else
                    {
                        return "http://" + _ipaddresse + ":" + _port + "/smarthome/storage/img/icons/" + metrics.icon + "-" + metrics.level + ".png";
                    }
                }
            }
        };

        public class MultiSensorDevice : Device
        {
            public new MultiSensorMetrics metrics = null;
            public new string iconpath
            {
                get
                {
                    if (metrics.isFailed)
                    {
                        return "http://" + _ipaddresse + ":" + _port + "/smarthome/storage/img/icons/placeholder.png";
                    }
                    else if (metrics.icon != String.Empty && metrics.icon.IndexOf('/') != -1)
                    {
                        return "http://" + _ipaddresse + ":" + _port + metrics.icon;
                    }
                    else
                    {
                        return "http://" + _ipaddresse + ":" + _port + "/smarthome/storage/img/icons/" + metrics.icon + ".png";
                    }
                }
            }
        };

        public class icons
        {
            public states level = null;
        }

        public class states
        {
            public string on = "";
            public string off = "";
        }

        public class metrics
        {
            public string title = "";
            public string icon = "";
            public string text = "";
            public string level = "";
            public bool isFailed = false;
            public string probeTitle = "";
            public string scaleTitle = "";
        }
        public class SwitchMetrics : metrics
        {
            public bool IsOn { get { return level == "on" ? true : false; } }
        }
        public class SensorMetrics : metrics
        {
            public bool IsOn { get { return level == "on" ? true : false; } }
        }
        public class MultiSensorMetrics : metrics
        {
            public string value { get { return level + " " + scaleTitle; } }
        }
        public class orders
        {
            public int rooms = -1;
            public int elements = -1;
            public int dashboard = -1;
            public int room = -1;
        }

        public class LogInData
        {
            public string port = "";
            public string benutzer = "";
            public string password = "";
            public string IPAdresse = "";
        }
        public string Benutzer { get { return _benutzer; } set { _benutzer = value; } }
        public string Password { get { return _password; } set { _password = value; } }
        public string Port { get { return _port; } set { _port = value; } }
        public string IPAdresse { get { return _ipaddresse.ToString(); } set {try{_ipaddresse = IPAddress.Parse(value);}catch{_ipaddresse = null;}} }
        public string ErrorMessage { get { return _errormsg; } }
        public bool Error { get { return _error; } }
        public bool Verbunden { get { return _verbunden; } }
    }
}
