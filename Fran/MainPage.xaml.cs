using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using AutoMapper;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x407 dokumentiert.

namespace Fran
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public class ListElement
    {
        public string id { get; set; }
        public string title { get; set; }
        public string room { get; set; }
        public bool toggled { get; set; }
        public string deviceType { get; set; }
        public string icon { get; set; } 
        public bool activ { get; set; }
        public string value { get; set; }
    }
    public sealed partial class MainPage : Page
    { 
        zWave Zway = new zWave();
        IMapper SwitchDeviceMapper;
        IMapper SensorDeviceMapper;
        IMapper MultiSensorDeviceMapper;
        public MainPage()
        {
            /*if (!Zway.Init())
            {
                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(Login));
            }
            else
            { */
            
                this.InitializeComponent();
                var config = new MapperConfiguration(cfg => cfg.CreateMap<zWave.Device, zWave.SwitchDevice>());
                SwitchDeviceMapper = new Mapper(config);
                config = new MapperConfiguration(cfg => cfg.CreateMap<zWave.Device, zWave.SensorDevice>());
                SensorDeviceMapper = new Mapper(config);
                config = new MapperConfiguration(cfg => cfg.CreateMap<zWave.Device, zWave.MultiSensorDevice>());
                MultiSensorDeviceMapper = new Mapper(config);
                Zway.Init();
                Zway.getAllDevices();
                ShowInList();
            //}
        }

        private ListElement GetSwitchDeviceElement(int i)
        {
            ListElement element = null;
            try
            {
                zWave.SwitchDevice deviceelement = SwitchDeviceMapper.Map<zWave.SwitchDevice>(Zway.All_Elements.devices[i]);
                element = new ListElement
                {
                    id = deviceelement.id,
                    title = deviceelement.metrics.title,
                    room = Zway.All_Elements.GetLocation(deviceelement.location),
                    toggled = deviceelement.metrics.IsOn,
                    deviceType = deviceelement.deviceType,
                    icon = deviceelement.iconpath,
                    activ = !deviceelement.metrics.isFailed
                };
            }
            catch(Exception e)
            {
                string meldend = e.Message;
            }
            return element;
        }
        private ListElement GetSensorDeviceElement(int i)
        {
            ListElement element = null;
            try
            {
                zWave.SensorDevice deviceelement = SensorDeviceMapper.Map<zWave.SensorDevice>(Zway.All_Elements.devices[i]);
                element = new ListElement
                {
                    id = deviceelement.id,
                    title = deviceelement.metrics.title,
                    room = Zway.All_Elements.GetLocation(deviceelement.location),
                    value = deviceelement.metrics.level,
                    deviceType = deviceelement.deviceType,
                    icon = deviceelement.iconpath,
                    activ = !deviceelement.metrics.isFailed
                };
            }
            catch (Exception e)
            {
                string meldend = e.Message;
            }
            return element;
        }
        private ListElement GetMultiSensorDeviceElement(int i)
        {
            ListElement element = null;
            try
            {
                zWave.MultiSensorDevice deviceelement = MultiSensorDeviceMapper.Map<zWave.MultiSensorDevice>(Zway.All_Elements.devices[i]);
                element = new ListElement
                {
                    id = deviceelement.id,
                    title = deviceelement.metrics.title,
                    room = Zway.All_Elements.GetLocation(deviceelement.location),
                    value = deviceelement.metrics.value,
                    deviceType = deviceelement.deviceType,
                    icon = deviceelement.iconpath,
                    activ = !deviceelement.metrics.isFailed
                };
            }
            catch (Exception e)
            {
                string meldend = e.Message;
            }
            return element;
        }
        private ListElement GetDeviceElement(int i)
        {
            ListElement element = null;
            try
            {
                zWave.Device deviceelement = Zway.All_Elements.devices[i];
                element = new ListElement
                {
                    id = deviceelement.id,
                    title = deviceelement.metrics.title,
                    room = Zway.All_Elements.GetLocation(deviceelement.location),
                    deviceType = deviceelement.deviceType,
                    icon = deviceelement.iconpath,
                    activ = !deviceelement.metrics.isFailed
                };
            }
            catch (Exception e)
            {
                string meldend = e.Message;
            }
            return element;
        }

        private void ShowInList()
        {
            for (int i = 0; i < Zway.All_Elements.devices.Length; i++)
            {
                if (Zway.All_Elements.devices[i].visibility)
                {
                    if (Zway.All_Elements.devices[i].deviceType == "switchBinary")
                    {
                        deviceList.Items.Add(DataContext = GetSwitchDeviceElement(i));
                        deviceList.ItemTemplateSelector.SelectTemplate(deviceList.Items[i]);
                    }
                    else if (Zway.All_Elements.devices[i].deviceType == "sensorBinary")
                    {
                        deviceList.Items.Add(DataContext = GetSensorDeviceElement(i));
                        deviceList.ItemTemplateSelector.SelectTemplate(deviceList.Items[i]);
                    }
                    else if (Zway.All_Elements.devices[i].deviceType == "sensorMultilevel")
                    {
                        deviceList.Items.Add(DataContext = GetMultiSensorDeviceElement(i));
                        deviceList.ItemTemplateSelector.SelectTemplate(deviceList.Items[i]);
                    }
                    else
                    {
                        deviceList.Items.Add(DataContext = GetDeviceElement(i));
                        deviceList.ItemTemplateSelector.SelectTemplate(deviceList.Items[i]);
                    }
                }
            }
        }
        
        private void Switch_AddToggledEvent(object sender, RoutedEventArgs e)
        {
            ToggleSwitch osender = (ToggleSwitch)sender;
            osender.Toggled += Switch_Toggled;
        }

        private void Switch_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch osender = (ToggleSwitch)sender;
            ListElement list = (ListElement)osender.DataContext;
            zWave.SwitchDevice toggledDevice = SwitchDeviceMapper.Map<zWave.SwitchDevice>(Zway.All_Elements.GetDeviceById(list.id));
            if (osender.IsOn)
            {
                toggledDevice.TurnOn();
            }
            else
            {
                toggledDevice.TurnOff();
            }
        }

        private void Image_Show(object sender, RoutedEventArgs e)
        {
            Image osender = (Image)sender;
            Grid imagegrid = (Grid)osender.Parent;
            ProgressRing wait = (ProgressRing)imagegrid.Children[0];
            wait.IsActive = false;
            osender.Visibility = Visibility.Visible;
        }
    }

    public class DeviceTemplates : DataTemplateSelector
    {

        public DataTemplate SwitchTemplate { get; set; }
        public DataTemplate SensorTemplate { get; set; }
        public DataTemplate MultiSensorTemplate { get; set; }
        public DataTemplate DefaultTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            ListElement element = item as ListElement;
            if (element.deviceType == "switchBinary")
            {
                return SwitchTemplate;
            }
            else if (element.deviceType == "sensorBinary")
            {
                return SensorTemplate;
            }
            else if (element.deviceType == "sensorMultilevel")
            {
                return MultiSensorTemplate;
            }
            return DefaultTemplate;
        }
    }
}
