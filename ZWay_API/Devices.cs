using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace ZWay_API
{
    //Main Device Class
    public class Devices
    {
        [JsonProperty("creationTime")]
        public int CreationTime { get; set; }
        [JsonProperty("creatorId")]
        public string CreatorId { get; set; }
        [JsonProperty("deviceType")]
        public string DeviceType { get; set; }
        [JsonProperty("customIcons")]
        public CustomIcons CustomIcons { get; set; }
        [JsonProperty("h")]
        public int H { get; set; }
        [JsonProperty("hasHistory")]
        public bool HasHistory  { get; set; }
        [JsonProperty("id")]
        public string ID { get; set; }
        [JsonProperty("location")]
        public int Location { get; set; }
        [JsonProperty("metrics")]
        public Metrics Metrics { get; set; }
        [JsonProperty("order")]
        public Orders Order { get; set; }
        [JsonProperty("permanently_hidden")]
        public bool PermanentlyHidden { get; set; }
        [JsonProperty("probeType")]
        public string ProbeType { get; set; }
        [JsonProperty("tags")]
        public List<string> Tags { get; set; }
        [JsonProperty("visibility")]
        public bool Visibility { get; set; }
        [JsonProperty("updateTime")]
        public int UpdateTime { get; set; }

    }
    //Inheritance Classes of Device
    public class BatteryDevices : Devices
    {
        [JsonProperty("metrics")]
        public new BatteryMetrics Metrics { get; set; }
    }
    public class DoorLockDevices : Devices
    {
        [JsonProperty("metrics")]
        public new DoorLockMetrics Metrics { get; set; }
        public DoorLockCommands Commands = new DoorLockCommands();
    }
    public class ThermostatDevices : Devices
    {
        [JsonProperty("metrics")]
        public new ThermostatMetrics Metrics { get; set; }
        public ThermostatCommands Commands = new ThermostatCommands();
    }
    public class SwitchBinaryDevices : Devices
    {
        [JsonProperty("metrics")]
        public new SwitchBinaryMetrics Metrics { get; set; }
        public SwitchBinaryCommands Commands = new SwitchBinaryCommands();
        public string Titel { get { return Metrics.Title; } }

    }
    public class SwitchMultilevelDevices : Devices
    {
        [JsonProperty("metrics")]
        public new SwitchMultilevelMetrics Metrics { get; set; }
        public SwitchMultilevelCommands Commands = new SwitchMultilevelCommands();
    }
    public class SensorBinaryDevices : Devices
    {
        [JsonProperty("metrics")]
        public new SensorBinaryMetrics Metrics { get; set; }
        public SensorBinaryCommands Commands = new SensorBinaryCommands();
    }
    public class SensorMultilevelDevices : Devices
    {
        [JsonProperty("metrics")]
        public new SensorMulitlevelMetrics Metrics { get; set; }
        public SensorMulitlevelCommands Commands = new SensorMulitlevelCommands();
    }
    public class ToggleButtonDevices : Devices
    {
        [JsonProperty("metrics")]
        public new ToggleButtonMetrics Metrics { get; set; }
        public ToggleButtonCommands Commands = new ToggleButtonCommands();
    }
    public class CameraDevices : Devices
    {
        [JsonProperty("metrics")]
        public new CameraMetrics Metrics { get; set; }
        public CameraCommands Commands = new CameraCommands();
    }
    public class SwitchControlDevices : Devices
    {
        [JsonProperty("metrics")]
        public new SwitchControlMetrics Metrics { get; set; }
        public SwitchControlCommands Commands = new SwitchControlCommands();
    }
    public class TextDevices : Devices
    {
        [JsonProperty("metrics")]
        public new TextMetrics Metrics { get; set; }
    }
    public class SwitchRGBDevices : Devices
    {
        [JsonProperty("metrics")]
        public new SwitchRGBMetrics Metrics { get; set; }
        public SwitchRGBCommands Commands = new SwitchRGBCommands();
    }

    public class Orders
    {
        [JsonProperty("rooms")]
        public int Rooms { get; set; }
        [JsonProperty("elements")]
        public int Elements { get; set; }
        [JsonProperty("dashboard")]
        public int Dashboard { get; set; }
        [JsonProperty("room")]
        public int Room { get; set; }
    }

    public class CustomIcons
    {
        [JsonProperty("level")]
        public DeviceStates Level { get; set; }
    }
    public class DeviceStates
    {
        [JsonProperty("on")]
        public string On { get; set; }
        [JsonProperty("off")]
        public string Off { get; set; }
    }

    //Main Metrics Class
    public class Metrics
    {
        [JsonProperty("Icon")]
        public string Icon { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("level")]
        public string Level { get; set; }
    }
    //Inheritance Classes of Metrics
    public class BatteryMetrics : Metrics
    {
        [JsonProperty("probTitle")]
        public string ProbTitle { get; set; }
        [JsonProperty("scaleTitle")]
        public string ScaleTitle { get; set; }
    }
    public class DoorLockMetrics : Metrics
    {

    }
    public class ThermostatMetrics : Metrics
    {
        [JsonProperty("scaleTitle")]
        public string ScaleTitle { get; set; }
        [JsonProperty("min")]
        public string Min { get; set; }
        [JsonProperty("max")]
        public string Max { get; set; }
    }
    public class SwitchBinaryMetrics : Metrics
    {
        [JsonProperty("isFailed")]
        public bool IsFailed { get; set; }
        public bool Toggled { get { return Level == "on" ? true : false; } }
        public bool IsAvailable { get { return !IsFailed; } }
    }
    public class SwitchMultilevelMetrics : Metrics
    {

    }
    public class SensorBinaryMetrics : Metrics
    {
        [JsonProperty("probTitle")]
        public string ProbTitle { get; set; }
    }
    public class SensorMulitlevelMetrics : Metrics
    {
        [JsonProperty("probTitle")]
        public string ProbTitle { get; set; }
        [JsonProperty("scaleTitle")]
        public string ScaleTitle { get; set; }
        [JsonProperty("isFailed")]
        public bool IsFailed { get; set; }
        public bool IsAvailable { get { return !IsFailed; } }
    }
    public class ToggleButtonMetrics : Metrics
    {

    }
    public class CameraMetrics : Metrics
    {
        [JsonProperty("level")]
        public new string Level = null;
    }
    public class SwitchControlMetrics : Metrics
    {
        [JsonProperty("change")]
        public string Change { get; set; }
    }
    public class TextMetrics : Metrics
    {
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("level")]
        public new string Level = null;
    }
    public class SwitchRGBMetrics : Metrics
    {
        [JsonProperty("color")]
        RGBColor Color { get; set; }
    }


    public class DoorLockCommands
    {
        public void open() { }
        public void close() { }
    }
    public class ThermostatCommands
    {
        public void exact(string level) { }
    }
    public class SwitchBinaryCommands
    {
        public void on(ZWay serv, Devices device)
        {
            try
            {
                serv.CommandRequest(device.ID + "/command/on");
            }
            catch (Exception e)
            {

            }
        }
        public void off(ZWay serv, Devices device)
        {
            try
            {
                serv.CommandRequest(device.ID + "/command/off");
            }
            catch (Exception e)
            {

            }
        }
        public void update() { }
    }
    public class SwitchMultilevelCommands
    {
        public void on() { }
        public void off() { }
        public void min() { }
        public void max() { }
        public void increase() { }
        public void decrease() { }
        public void update() { }
        public void exact(string level) { }

    }
    public class SensorBinaryCommands
    {
        public void update() { }
    }
    public class SensorMulitlevelCommands
    {
        public void update() { }
    }
    public class ToggleButtonCommands
    {
        public void on() { }
    }
    public class CameraCommands
    {
        public void zoomIn() { }
        public void zoomOut() { }
        public void up() { }
        public void down() { }
        public void left() { }
        public void right() { }
        public void close() { }
        public void open() { }
    }
    public class SwitchControlCommands
    {
        public void on() { }
        public void off() { }
        public void upstart() { }
        public void upstop() { }
        public void downstart() { }
        public void downstop() { }
        public void exact(string level) { }
    }
    public class SwitchRGBCommands
    {
        public void on() { }
        public void off() { }
        public void exact() { }
    }


    public class RGBColor
    {
        [JsonProperty("r")]
        string R { get; set; }
        [JsonProperty("g")]
        string G { get; set; }
        [JsonProperty("b")]
        string B { get; set; }

        public string GetParam()
        {
            return "red=" + R + "&green=" + G + "&blue=" + B;
        }
    }

    public class DevicesConverter : JsonCreationConverter<Devices>
    {
        protected override Devices Create(Type objectType, JObject jObject)
        {
            string DeviceType = GetValue("deviceType", jObject);
            switch (DeviceType)
            {
                case "battery":
                    return new BatteryDevices();
                case "doorlock":
                    return new DoorLockDevices();                    
                case "switchBinary":
                    return new SwitchBinaryDevices();                    
                case "switchMultilevel":
                    return new SwitchMultilevelDevices();                    
                case "sensorBinary":
                    return new SensorBinaryDevices();                    
                case "sensorMultilevel":
                    return new SensorMultilevelDevices();                    
                case "toggleButton":
                    return new ToggleButtonDevices();                    
                case "camera":
                    return new CameraDevices();                    
                case "switchControl":
                    return new SwitchControlDevices();                    
                case "text":
                    return new TextDevices();                    
                case "sensoreMultiline":
                    return new Devices();                                       // Need to be implemented            
                case "switchRGB":
                    return new SwitchRGBDevices();                    
                default:
                    return new Devices();                    
            }
            
        }
        private string GetValue(string fieldName, JObject jObject)
        {
            return jObject[fieldName].Value<string>();
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class JsonCreationConverter<T> : JsonConverter
    {
        protected abstract T Create(Type objectType, JObject jObject);
        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }
        public override bool CanWrite
        {
            get { return false; }
        }
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jObject = JObject.Load(reader);
            T target = Create(objectType, jObject);
            serializer.Populate(jObject.CreateReader(), target);
            return target;
        }
    }

}
