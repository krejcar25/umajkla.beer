using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace umajkla.beer_win
{
    class Client
    {
        public static event EventHandler WorkStarted;
        public static event EventHandler WorkDone;
        private static int countRunning = 0;
        public static Guid eventId;

        public static string Run(string task, string method, string id = "", string json = "")
        {
            WorkStarted?.Invoke(new object(), new EventArgs());
            countRunning++;
            bool retry = true;
            while (retry)
            {
                try
                {
                    retry = false;
                    if (method == "GET")
                    {
                        using (var client = new WebClient())
                        {
                            client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                            client.Encoding = Encoding.UTF8;
                            if (string.IsNullOrEmpty(id))
                            {
                                string ret = client.DownloadString(string.Format("http://umajkla.beer/api/{0}", task));
                                countRunning--;
                                if (countRunning==0) WorkDone?.Invoke(new object(), new EventArgs());
                                return ret;
                            }
                            else
                            {
                                string ret = client.DownloadString(string.Format("http://umajkla.beer/api/{0}/{1}", task, id));
                                countRunning--;
                                if (countRunning == 0) WorkDone?.Invoke(new object(), new EventArgs());
                                return ret;
                            }

                        }
                    }
                    else
                    {
                        using (var client = new WebClient())
                        {
                            client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                            client.Encoding = Encoding.UTF8;
                            if (string.IsNullOrEmpty(id))
                            {
                                string ret = client.UploadString(string.Format("http://umajkla.beer/api/{0}", task), method, json);
                                countRunning--;
                                if (countRunning == 0) WorkDone?.Invoke(new object(), new EventArgs());
                                return ret;
                            }
                            else
                            {
                                string ret = client.UploadString(string.Format("http://umajkla.beer/api/{0}/{1}", task, id), method, json);
                                countRunning--;
                                if (countRunning == 0) WorkDone?.Invoke(new object(), new EventArgs());
                                return ret;
                            }
                        }
                    }
                }
                catch (WebException)
                {
                    retry = CheckInternet();
                }
            }
            countRunning--;
            if (countRunning == 0) WorkDone?.Invoke(new object(), new EventArgs());
            return "";
        }

        private static bool CheckInternet()
        {
            if (MessageBox.Show("Není k dispozici připojení k zadanému serveru. Prosím zkontrolujte připojení a akci opakujte.\r\n" +
                "Kliknutím na OK akci opakujete. Klinutím na Zrušit ukončíte program.", "Chyba připojení",
                MessageBoxButton.OKCancel, MessageBoxImage.Error) == MessageBoxResult.OK)
            {
                return true;
            }
            else
            {
                if (MessageBox.Show("Kliknutí na tlačítko Ano ukončí program. Ukončením programu dojde k automatickému uložení současné objednávky.\r\n" +
                    "Kliknutí na tlačítko ne opakuje operaci opakujete", "Ověření", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                {
                    Environment.Exit(0);
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
