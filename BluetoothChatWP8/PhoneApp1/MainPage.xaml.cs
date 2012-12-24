using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PhoneApp1.Resources;
using System.Collections.Generic;

// (c) Erkki Nokso-Koivisto 25/Nov/2012

namespace PhoneApp1
{
    public partial class MainPage : PhoneApplicationPage
    {

        public MainPage()
        {
            InitializeComponent();
            SetupAppBar();
            ListMessages.LayoutUpdated += ListMessages_LayoutUpdated;
            ChatConnection.ConnectionStateChanged += ChatConnection_ConnectionStateChanged;
            Loaded += MainPage_Loaded;
        }

        void ChatConnection_ConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e)
        {
            if (e.State == ChatConnection.STATE_CONNECTED)
            {
                TextBlockStatus.Text = "Connected";
            }
            else
            {
                TextBlockStatus.Text = "Disconnected";
            }
        }

        void ListMessages_LayoutUpdated(object sender, EventArgs e)
        {
            if (ListMessages.Items.Count > 0)
            {
                object last = ListMessages.Items[ListMessages.Items.Count - 1];
                ListMessages.ScrollIntoView(last);
            }
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            ListMessages.ItemsSource = ChatMessages.Data;
            ChatConnection.StartListeningIncoming();
            //MessageBox.Show(Windows.ApplicationModel.Package.Current.Id.FamilyName);
        }

        public void SetupAppBar() {
            ApplicationBar = new ApplicationBar();
            ApplicationBarMenuItem miConnect = new ApplicationBarMenuItem("connect");
            miConnect.Click += miConnect_Click;
            ApplicationBar.MenuItems.Add(miConnect);
        }

        void miConnect_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/ConnectTo.xaml", UriKind.Relative));
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
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