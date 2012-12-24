using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace BluetoothChat
{
    class Toast
    {

        public static void Show(string message)
        {
            try
            {
                const ToastTemplateType template = Windows.UI.Notifications.ToastTemplateType.ToastText02;

                var toastXml = Windows.UI.Notifications.ToastNotificationManager.GetTemplateContent(template);

                var toastTextElements = toastXml.GetElementsByTagName("text");
                toastTextElements[0].AppendChild(toastXml.CreateTextNode("Notification"));
                toastTextElements[1].AppendChild(toastXml.CreateTextNode(message));

                var toast = new Windows.UI.Notifications.ToastNotification(toastXml);

                var toastNotifier = Windows.UI.Notifications.ToastNotificationManager.CreateToastNotifier();
                toastNotifier.Show(toast);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error showing toast:" + ex.ToString());
            }
        }
    }
}
