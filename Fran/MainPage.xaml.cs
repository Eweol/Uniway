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
using ZWay_API;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x407 dokumentiert.

namespace Uniway
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    
    public sealed partial class MainPage : Page
    { 
        ZWay Zway = new ZWay();
        public MainPage()
        {
            /*if (!Zway.Init())
            {
                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(Login));
            }
            else
            { */
            Zway.Init();
            Zway.getAllDevices();

            this.InitializeComponent();
            //}
        }
        
        private void Switch_AddToggledEvent(object sender, RoutedEventArgs e)
        {
            ToggleSwitch osender = (ToggleSwitch)sender;
            osender.Toggled += Switch_Toggled;
        }
        
        private void Switch_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch osender = (ToggleSwitch)sender;
            SwitchBinaryDevices device = (SwitchBinaryDevices)osender.DataContext;
            if (osender.IsOn)
            {
                device.Commands.on(Zway, device);
            }
            else
            {
                device.Commands.off(Zway, device);
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

        public DataTemplate DevicesTemplate { get; set; }
        public DataTemplate BatteryDevicesTemplate { get; set; }
        public DataTemplate DoorLockDevicesTemplate { get; set; }
        public DataTemplate ThermostatDevicesTemplate { get; set; }
        public DataTemplate SwitchBinaryDevicesTemplate { get; set; }
        public DataTemplate SwitchMultilevelDevicesTemplate { get; set; }
        public DataTemplate SensorBinaryDevicesTemplate { get; set; }
        public DataTemplate SensorMultilevelDevicesTemplate { get; set; }
        public DataTemplate ToggleButtonDevicesTemplate { get; set; }
        public DataTemplate CameraDevicesTemplate { get; set; }
        public DataTemplate SwitchControlDevicesTemplate { get; set; }
        public DataTemplate TextDevicesTemplate { get; set; }
        public DataTemplate SwitchRGBDevicesTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item.GetType() == typeof(SwitchBinaryDevices))
            {
                return SwitchBinaryDevicesTemplate;
            }
            else if (item.GetType() == typeof(SensorBinaryDevices))
            {
                return SensorBinaryDevicesTemplate;
            }
            else if (item.GetType() == typeof(SensorMultilevelDevices))
            {
                return SensorMultilevelDevicesTemplate;
            }
            return DevicesTemplate;
        }
    }
}
