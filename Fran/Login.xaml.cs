using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace Fran
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class Login : Page
    {
        zWave zWay;
        public Login()
        {
            this.InitializeComponent();
        }

        void Frame_Navigated(object sender, NavigationFailedEventHandler e)
        {
            zWay = (zWave)sender;
        }
        private void Login_Button(object sender, RoutedEventArgs e)
        {
            
            zWay.Benutzer = "admin"; //TB_Nutzername.Text;
            zWay.Password = " ForteXX125";// TB_Passwort.Password;
            zWay.IPAdresse = "192.168.178.5";// TB_IPAdresse;
            if (TB_Port.Text != "")
                zWay.Port = TB_Port.Text;

            zWay.Init();
            if (zWay.Error)
            {
                //Logout
            }
        }
    }
}
