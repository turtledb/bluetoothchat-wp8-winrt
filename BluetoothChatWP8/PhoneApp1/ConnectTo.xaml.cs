using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Threading.Tasks;
using System.Threading;
using System.ComponentModel;
using System.Collections.Generic;
using Windows.Networking.Proximity;
using Windows.Foundation;

// (c) Erkki Nokso-Koivisto 25/Nov/2012

namespace PhoneApp1
{
    public partial class ConnectTo : PhoneApplicationPage
    {

        const uint ERR_BLUETOOTH_OFF = 0x8007048F; 
        IReadOnlyList<PeerInformation> devices;

        public ConnectTo()
        {
            InitializeComponent();
            SetupAppBar();
            Loaded += ConnectTo_Loaded;
            ListDevices.SelectionChanged += ListDevices_SelectionChanged;
        }

        void ListDevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selected = ListDevices.SelectedIndex;
            ChatConnection.ConnectAsync(devices[selected]);
            System.Diagnostics.Debug.WriteLine("ca return");
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        void ConnectTo_Loaded(object sender, RoutedEventArgs e)
        {
            findPaired();
        }


        async Task findPaired() {

            System.Diagnostics.Debug.WriteLine("findPaired");

            bool bluetoothOff = false;

            try
            {
                
                PeerFinder.AlternateIdentities["Bluetooth:PAIRED"] = "";

                if (PeerFinder.AlternateIdentities.ContainsKey("Windows") == false)
                {
                    PeerFinder.AlternateIdentities.Add("Windows", "{afda1570-a6ae-4026-b451-15a4fef4be4d_a7m0rynx2c63e!BluetoothChat}");
                }

                IReadOnlyList<PeerInformation> result = await PeerFinder.FindAllPeersAsync();

                if (result == null || result.Count == 0)
                {
                    MessageBox.Show("No paired devices found, please pair one device first..");
                    await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings-bluetooth:"));
                }
                else
                {
                    MessageBox.Show("Found paired devices, click device on list to connect..");
                    devices = result;
                    ListDevices.ItemsSource = devices;
                }
            }
            catch (Exception ex)
            {
                if ((uint)ex.HResult == ERR_BLUETOOTH_OFF)
                {
                    bluetoothOff = true;
                }
            }

            if (bluetoothOff)
            {
                MessageBox.Show("Turn on bluetooth first..");
                await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings-bluetooth:"));
            }
 

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            ListDevices = null;
           
        }

        public void SetupAppBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBarMenuItem miRefresh = new ApplicationBarMenuItem("refresh");
            miRefresh.Click += miRefresh_Click;
            ApplicationBar.MenuItems.Add(miRefresh);
        }

        void miRefresh_Click(object sender, EventArgs e)
        {
            findPaired();
        }
    }
}