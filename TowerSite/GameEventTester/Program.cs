using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Web.ClientServices;
using System.Net;
using System.Data.Services.Client;
using System.Runtime.InteropServices;

namespace GameEventTester
{
    class Program
    {
        static String Cookies = "";
        static bool LoggedIn = false;

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool InternetGetCookieEx(string pchURL, string pchCookieName, StringBuilder pchCookieData, ref uint pcchCookieData, int dwFlags, IntPtr lpReserved);
        const int INTERNET_COOKIE_HTTPONLY = 0x00002000;

        public static string GetGlobalCookies(string uri)
        {
            uint datasize = 1024;
            StringBuilder cookieData = new StringBuilder((int)datasize);
            if (InternetGetCookieEx(uri, null, cookieData, ref datasize, INTERNET_COOKIE_HTTPONLY, IntPtr.Zero)
                && cookieData.Length > 0)
            {
                return cookieData.ToString().Replace(';', ',');
            }
            else
            {
                return null;
            }
        }
        [STAThread]
        static void Main(string[] args)
        {

            //while(true)
            //{
            //    Thread.Sleep(50);
            //    if(browse.Document!= null && browse.Document.Cookie != null)
            //    {
            //        string cookieStr = browse.Document.Cookie;
            //        string[] cookstr = cookieStr.Split(';');
            //        foreach (string str in cookstr)
            //        {
            //            Console.WriteLine(str);
            //        }
            //    }
            //}
            //Thread th = new Thread(new ThreadStart(MainLoop));
            //th.Start();
            //ShowBrowser();
            MainLoop();

        }
        static void MainLoop()
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
            Uri uri = new Uri("https://localhost:44300/odata");
            //Uri uri = new Uri("https://toweroffense.azurewebsites.net/odata");
            GameEventService.Container container = new GameEventService.Container(uri);

            container.ReceivingResponse += (s, e) =>
            {
                HttpWebResponse wr = ((HttpWebResponseMessage)e.ResponseMessage).Response;
                if(!wr.ContentType.Contains("application/atom+xml"))
                {
                    LoggedIn = false;
                }
            };
            container.SendingRequest2 += (s, e) =>
            {
                HttpWebRequest wr = ((HttpWebRequestMessage)e.RequestMessage).HttpWebRequest;
                wr.CookieContainer = new CookieContainer();
                wr.CookieContainer.SetCookies(uri, Cookies);
                Console.WriteLine("{0} {1}", e.RequestMessage.Method, e.RequestMessage.Url);
            };

            bool done = false;
            char[] delims = { ' ', ':' };
            String line;
            while (!done)
            {
                line = Console.ReadLine();
                string[] lARgs = line.Split(delims);

                if(!LoggedIn)
                {
                    ShowBrowser();
                }
                try
                {
                    switch (lARgs[0].ToLower())
                    {
                        case "quit":
                        case "exit":
                            done = true;
                            break;
                        case "login":
                            if (System.Web.Security.Membership.ValidateUser(lARgs[1], lARgs[2]))
                            {
                                LoggedIn = true;
                                Console.WriteLine("Login successful");
                            }
                            else
                            {
                                LoggedIn = false;
                                Console.WriteLine("Login failed");
                            }
                            break;
                        case "listall":
                            ListAllEvents(container);
                            break;
                        case "add":
                            GameEventService.GameEventModel ge = new GameEventService.GameEventModel();
                            ge.SessionId = int.Parse(lARgs[1]);
                            ge.Timestamp = DateTime.Now;
                            ge.Type = lARgs[2];
                            ge.Data = lARgs[3];
                            AddEvent(container, ge);
                            break;
                        case "clearcookie":
                            Cookies = "";
                            break;
                    }
                }
                catch(Exception)
                {
                    Console.WriteLine("Command Failed");
                }
            }
        }
        [STAThread]
        static void ShowBrowser()
        {
            Form loginWindow = new Form();

            WebBrowser browse = new WebBrowser();
            browse.Dock = DockStyle.Fill;
            browse.Url = new Uri("https://localhost:44300/Account/Login");
            browse.DocumentCompleted += browse_DocumentCompleted;
            loginWindow.Controls.Add(browse);

            loginWindow.ShowDialog();
        }

        static void browse_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            WebBrowser b = sender as WebBrowser;
            Cookies = GetGlobalCookies(b.Document.Url.AbsoluteUri);
            if(Cookies.Contains(".AspNet.ApplicationCookie"))
            {
                LoggedIn = true;
                Form f = b.Parent as Form;
                f.Hide();
            }
        }
        static void DisplayEvent(GameEventService.GameEventModel ge)
        {
            Console.WriteLine("{0} {1} {2} {3}", ge.SessionId, ge.Timestamp, ge.Type, ge.Data);
        }
        static void ListAllEvents(GameEventService.Container container)
        {
            foreach (var p in container.GameEvent)
            {
                DisplayEvent(p);
            }
        }
        static void AddEvent(GameEventService.Container container, GameEventService.GameEventModel ge)
        {
            container.AddToGameEvent(ge);
            var serviceResponse = container.SaveChanges();
            foreach (var operationResponse in serviceResponse)
            {
                Console.WriteLine(operationResponse.StatusCode);
            }
        }
    }
}
