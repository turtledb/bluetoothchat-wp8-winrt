using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

// (c) Erkki Nokso-Koivisto 25/Nov/2012

namespace BluetoothChat
{
    class ChatMessage {
        public String Id { get; set; }
        public String Content { get; set; }
    };

    class ChatMessages
    {
        public static ObservableCollection<ChatMessage> Data = new ObservableCollection<ChatMessage>();
    }
}
