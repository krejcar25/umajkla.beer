using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static beer.umajkla.win.AppConfig;

namespace beer.umajkla.win
{
    public delegate void CloseWindowEventHandler();
    public class Client
    {
        public static event CloseWindowEventHandler RequestWindowClose;
        public static event EventHandler WorkStarted;
        public static event EventHandler WorkDone;
        private static int countRunning = 0;
        public static Guid eventId => CurrentConfig.EventId;
        public static bool dialogNeeded
        {
            get
            {
                return CurrentConfig.FirstStartDialogRequired;
            }

            set
            {
                CurrentConfig.FirstStartDialogRequired = value;
            }
        }

        public static string Run(string task, string method, string id = "", string json = "")
        {
            if (dialogNeeded)
            {
                FirstStart startDialog = new FirstStart();
                startDialog.ShowDialog();
                CurrentConfig.ServerAddress = startDialog.Value;
                CurrentConfig.username = startDialog.user.Text;
                CurrentConfig.password = startDialog.pass.Text;
                dialogNeeded = false;
                CurrentConfig.Save();
            }

            WorkStarted?.Invoke(new object(), new EventArgs());
            countRunning++;
            bool retry = true;
            while (retry)
            {
                try
                {
                    using (var client = new WebClient())
                    {
                        client.Headers.Add("username", CurrentConfig.username);
                        client.Headers.Add("password", CurrentConfig.password);
                        client.Headers.Add("X-Requested-With", "XMLHttpRequest");

                        retry = false;
                        if (method == "GET")
                        {
                            client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                            client.Encoding = Encoding.UTF8;
                            if (string.IsNullOrEmpty(id))
                            {
                                string ret = client.DownloadString(string.Format("{0}/{1}", CurrentConfig.ServerAddress, task));
                                countRunning--;
                                if (countRunning == 0) WorkDone?.Invoke(new object(), new EventArgs());
                                return ret;
                            }
                            else
                            {
                                string ret = client.DownloadString(string.Format("{0}/{1}/{2}", CurrentConfig.ServerAddress, task, id));
                                countRunning--;
                                if (countRunning == 0) WorkDone?.Invoke(new object(), new EventArgs());
                                return ret;
                            }


                        }
                        else
                        {
                            client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                            client.Encoding = Encoding.UTF8;
                            if (string.IsNullOrEmpty(id))
                            {
                                string ret = client.UploadString(string.Format("{0}/{1}", CurrentConfig.ServerAddress, task), method, json);
                                countRunning--;
                                if (countRunning == 0) WorkDone?.Invoke(new object(), new EventArgs());
                                return ret;
                            }
                            else
                            {
                                string ret = client.UploadString(string.Format("{0}/{1}/{2}", CurrentConfig.ServerAddress, task, id), method, json);
                                countRunning--;
                                if (countRunning == 0) WorkDone?.Invoke(new object(), new EventArgs());
                                return ret;
                            }

                        }
                    }
                }
                catch (WebException e)
                {
                    if (e.Status == WebExceptionStatus.ProtocolError)
                    {
                        if (((HttpWebResponse)e.Response).StatusCode == HttpStatusCode.Unauthorized)
                        {
                            dialogNeeded = true;
                            return Run(task, method, id, json);
                        }
                    }
                    else
                    {
                        retry = CheckInternet();
                    }
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
                    RequestWindowClose?.Invoke();
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
