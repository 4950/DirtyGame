//#define LOCAL

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

#if (LOCAL)
using GameService = GameEventTester.GameServiceLocal;
#else
using GameService = GameEventTester.GameService;
#endif

namespace GameEventTester
{
    class Program
    {
        static String Cookies = "";
        static bool LoggedIn = false;
        static int CurrentSession = 0;

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
#if (LOCAL)
            Uri uri = new Uri("https://localhost:44300/odata");
#else
            Uri uri = new Uri("https://toweroffense.azurewebsites.net/odata");
#endif
            GameService.Container container = new GameService.Container(uri);

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
                        case "listevents":
                            ListAllEvents(container);
                            break;
                        case "listsessions":
                            ListAllSessions(container);
                            break;
                        case "newsession":
                            GameService.GameSession gs = new GameService.GameSession();
                            AddSession(container, gs);
                            break;
                        case "add":
                            GameService.GameEventModel ge = new GameService.GameEventModel();
                            ge.SessionId = CurrentSession;
                            ge.Timestamp = DateTime.Now;
                            ge.Type = lARgs[1];
                            ge.Data = lARgs[2];
                            AddEvent(container, ge);
                            break;
                        case "printsession":
                            Console.WriteLine("Current session: " + CurrentSession);
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
            loginWindow.Size = new System.Drawing.Size(300, 850);
            loginWindow.FormBorderStyle = FormBorderStyle.FixedToolWindow;

            WebBrowser browse = new WebBrowser();
            browse.Dock = DockStyle.Fill;
#if (LOCAL)
            browse.Url = new Uri("https://localhost:44300/Account/Login");
#else
            browse.Url = new Uri("https://toweroffense.azurewebsites.net/Account/Login");
#endif
            browse.DocumentCompleted += browse_DocumentCompleted;
            loginWindow.Controls.Add(browse);

            loginWindow.ShowDialog();
        }

        static void browse_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            WebBrowser b = sender as WebBrowser;
            Cookies = GetGlobalCookies(b.Document.Url.AbsoluteUri);
            if(Cookies != null && Cookies.Contains(".AspNet.ApplicationCookie"))
            {
                LoggedIn = true;
                Form f = b.Parent as Form;
                f.Hide();
            }
        }
        static void DisplayEvent(GameService.GameEventModel ge)
        {
            Console.WriteLine("{0} {1} {2} {3} {4}", ge.ID, ge.SessionId, ge.Timestamp, ge.Type, ge.Data);
        }

        static void ListAllEvents(GameService.Container container)
        {
            foreach (var p in container.GameEvent)
            {
                DisplayEvent(p);
            }
        }
        static void DisplaySession(GameService.GameSession ge)
        {
            Console.WriteLine("{0} {1} {2} {3}", ge.ID, ge.UserID, ge.SessionID, ge.SessionScore);
        }
        static void ListAllSessions(GameService.Container container)
        {
            foreach (var p in container.GameSession)
            {
                DisplaySession(p);
            }
        }
        static void AddSession(GameService.Container container, GameService.GameSession ge)
        {
            container.AddToGameSession(ge);
            var serviceResponse = container.SaveChanges();
            CurrentSession = ge.SessionID;
            foreach (var operationResponse in serviceResponse)
            {
                Console.WriteLine(operationResponse.StatusCode);
            }
        }
        static void AddEvent(GameService.Container container, GameService.GameEventModel ge)
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
