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


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BluetoothChat
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            
            this.InitializeComponent();
            TextChat.KeyUp += TextChat_KeyUp;
            ConnectSwitch.Toggled += ConnectSwitch_Toggled;
            ChatConnection.ConnectionStateChanged += ChatConnection_ConnectionStateChanged;
            ListMessages.LayoutUpdated += ListMessages_LayoutUpdated;
        }

        void ListMessages_LayoutUpdated(object sender, object e)
        {
            if (ListMessages.Items.Count > 0)
            {
                object last = ListMessages.Items[ListMessages.Items.Count - 1];
                ListMessages.ScrollIntoView(last);
            }
        }


        void ChatConnection_ConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e)
        {
            if (e.State == ChatConnection.STATE_CONNECTED)
            {
                TextBlockStatus.Text = "Connected";
                ConnectSwitch.IsOn = true;
            }
            else
            {
                TextBlockStatus.Text = "Disconnected";
                ConnectSwitch.IsOn = false;
            }
        }

        void ConnectSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            //ChatConnection.Close();
            Frame.Navigate(typeof(ConnectTo));
        }

        void TextChat_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter) {
                sendChatMsg();
                TextChat.Text = "";
            }
            
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ListMessages.ItemsSource = ChatMessages.Data;
            ChatConnection.StartListeningIncoming();
        }

        private void sendChatMsg()
        {
            String text = TextChat.Text;
            
            if (text.Length > 0)
            {
             
                // We will not check status of sending, since this is just a lazy demo app :)

                ChatMessage msg = new ChatMessage()
                {
                    Content = ">> " + text
                };

                ChatMessages.Data.Add(msg);

                ChatMessage msg2 = new ChatMessage()
                {
                    Content = text
                };

                ChatConnection.SendChatMessageAsync(msg2);
                TextChat.Text = "";

            }

        }
       
    }
}
