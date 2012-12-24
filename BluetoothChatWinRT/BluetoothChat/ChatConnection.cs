using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Proximity;
using Windows.Networking.Sockets;
using System.ComponentModel;
using Windows.Storage.Streams;
using System.Threading;
using System.Windows;
using Windows.UI.Popups;
// (c) Erkki Nokso-Koivisto 25/Nov/2012

// Good reading:
// http://msdn.microsoft.com/en-us/library/windowsphone/develop/jj207007(v=vs.105).aspx
// http://blogs.windows.com/windows_phone/b/wpdev/archive/2012/11/19/networking-in-windows-phone-8.aspx

namespace BluetoothChat
{
    
    class ConnectionStateChangedEventArgs : EventArgs {

        public readonly int State;
        public readonly string Reason;

        public ConnectionStateChangedEventArgs(int state, string reason) {
            this.State = state;
            this.Reason = reason;
        }
    }

    class ChatConnection
    {

        // UUID's are from BlueToothChatService.java
        // INSECURE UID is available only on >= Android SDK10 BluetoothChat sample

        private const string UUID_CHAT_SERVICE_SECURE = "{fa87c0d0-afac-11de-8a39-0800200c9a66}";
        private const string UUID_CHAT_SERVICE_INSECURE = "{8ce255c0-200a-11e0-ac64-0800200c9a66}";

        public const int STATE_DISCONNECTED = 0;
        public const int STATE_CONNECTING = 1;
        public const int STATE_CONNECTED = 2;

        public static event EventHandler<ConnectionStateChangedEventArgs> ConnectionStateChanged;
        public static int State = STATE_DISCONNECTED;

        private static IInputStream inputStream;
        private static IOutputStream outputStream;
        private static StreamSocket socket;
        //private static BackgroundWorker pollnext;
        
        private static DataReader reader;
        private static DataWriter writer;
        
        public async static void ConnectAsync(PeerInformation peerInfo) {

            try
            {

                if (State != STATE_DISCONNECTED)
                    return;

                StopListeningIncoming();

                State = STATE_CONNECTING;

                socket = await PeerFinder.ConnectAsync(peerInfo);
                
                inputStream = socket.InputStream;
                outputStream = socket.OutputStream;

                writer = new DataWriter(outputStream);
                reader = new DataReader(inputStream);
                // allows LoadAsync() to load less data than requested (1024 bytes in this case) without
                // blocking.. 
                reader.InputStreamOptions = InputStreamOptions.Partial;


                State = STATE_CONNECTED;

                if (ConnectionStateChanged != null)
                {
                    ConnectionStateChanged(null, new ConnectionStateChangedEventArgs(STATE_CONNECTED, "connected"));
                }
       
                 await readSocket();

            }
            catch (Exception ex)
            {
                
                System.Diagnostics.Debug.WriteLine("ERROR/ConnectAsync/" + ex.ToString());
                //MessageDialog md = new MessageDialog().
                Toast.Show("Failed to connect, make sure bluetooth chat app is running on target device..");
                CleanUp();
            }

        }


        private static void CleanUp() {

            try
            {
                System.Diagnostics.Debug.WriteLine("ERROR/CleanUp/Disconnecting");
                socket.Dispose();
                State = STATE_DISCONNECTED;

            }
            catch (Exception)
            {
            }
           
            if (ConnectionStateChanged != null)
            {
                ConnectionStateChanged(null, new ConnectionStateChangedEventArgs(STATE_DISCONNECTED, "closed"));
            }
 

           StartListeningIncoming();
        }

        private static async Task readSocket()
        {
            while (State == STATE_CONNECTED)
            {

                try
                {
                    
                    uint len = await reader.LoadAsync(1024);

                    if (len > 0)
                    {

                        String msg = reader.ReadString(len);

                        ChatMessage chatMsg = new ChatMessage()
                        {
                            Content = "<< " + msg
                        };

                        ChatMessages.Data.Add(chatMsg);
                    }

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("ERROR/readSocket/" + ex.ToString());
                    CleanUp();
                    break;
                }
            }

            State = STATE_DISCONNECTED; 
            
        }

        public async static void SendChatMessageAsync(ChatMessage msg)
        {
            try
            {
                if (State == STATE_CONNECTED)
                {
                    writer.WriteString(msg.Content);
                    await writer.StoreAsync();
                    System.Diagnostics.Debug.WriteLine("Chat msg sent/" + msg.Content);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR/SendChatMessageAsync/" + ex.ToString());
                CleanUp();
            }
        }

        public static void Close() {
            CleanUp();
        }

//============================================================================================================
// Didn't succeed on sending any kind of trigger from Android to WP8
// 

        public static void StartListeningIncoming()
        {
            if (State == STATE_DISCONNECTED)
            {
                try
                {

                    System.Diagnostics.Debug.WriteLine("StartListeningIncoming");
                    PeerFinder.ConnectionRequested -= PeerFinder_ConnectionRequested;
                    PeerFinder.ConnectionRequested += PeerFinder_ConnectionRequested;
                    PeerFinder.TriggeredConnectionStateChanged += PeerFinder_TriggeredConnectionStateChanged;
                    PeerFinder.Start();
                    System.Diagnostics.Debug.WriteLine("pm type:" + PeerFinder.SupportedDiscoveryTypes);
                    
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("StartListeningIncoming/Error=" + ex.ToString());
                }
            }
        }

        static void PeerFinder_TriggeredConnectionStateChanged(object sender, TriggeredConnectionStateChangedEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("PeerFinder_TriggeredConnectionStateChanged");
        }

        public static void StopListeningIncoming()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("StopListeningIncoming");
                PeerFinder.Stop();
            }
            catch (Exception)
            {
            }
        }

        private static void PeerFinder_ConnectionRequested(object sender, ConnectionRequestedEventArgs args)
        {
            Toast.Show("Connection requested");
            System.Diagnostics.Debug.WriteLine("PeerFinder_ConnectionRequested");
            ConnectAsync(args.PeerInformation);
            StopListeningIncoming();
        }

    }
}
