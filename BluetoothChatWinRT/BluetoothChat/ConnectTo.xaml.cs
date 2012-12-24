using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Networking.Proximity;
using System.Threading.Tasks;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace BluetoothChat
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class ConnectTo : Page
    {

        const uint ERR_BLUETOOTH_OFF = 0x8007048F;
        IReadOnlyList<PeerInformation> devices;

        public ConnectTo()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            findPaired();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        async Task findPaired()
        {

            System.Diagnostics.Debug.WriteLine("findPaired");

            bool bluetoothOff = false;

            try
            {
                
                PeerFinder.AlternateIdentities["Bluetooth:PAIRED"] = "";
                
                if (PeerFinder.AlternateIdentities.ContainsKey("WindowsPhone") == false)
                {
                    PeerFinder.AlternateIdentities.Add("WindowsPhone", "{affd4af3-390e-4e91-b204-8e0e4676b0ea}");
                }

                IReadOnlyList<PeerInformation> result = await PeerFinder.FindAllPeersAsync();

                if (result == null || result.Count == 0)
                {
                    Toast.Show("No paired devices found, please pair one device first..");
                    await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings-bluetooth:"));
                }
                else
                {
                    Toast.Show("Found paired devices, click device on list to connect..");
                    devices = result;
                    ListDevices.ItemsSource = devices;
                }
            }
            catch (Exception ex)
            {
                // 0x80070306 win8
                System.Diagnostics.Debug.WriteLine("findPaired/" + ex.ToString());

                if ((uint)ex.HResult == ERR_BLUETOOTH_OFF)
                {
                    bluetoothOff = true;
                }
            }

            if (bluetoothOff)
            {
                Toast.Show("Turn on bluetooth first..");
                await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings-bluetooth:"));
            }


        }
    }
}
