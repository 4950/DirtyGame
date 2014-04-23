using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Objects;
using Microsoft.Data.OData;

namespace GameService
{
    public class Singleton<T> where T : class, new()
    {
        protected static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                    instance = new T();
                return instance;
            }
        }
    }
    public enum CaptureEventType
    {
        MapSelected,
        PlayerWeaponFired,
        MonsterWeaponFired,
        MonsterDamageTaken,
        MonsterType,
        PlayerDamageTaken,
        PlayerWeaponFirstHit,
        PlayerHitByWeapon,
        MonsterHitByWeapon,
        MonsterKilled,
        RoundEnded,
        RoundHealth,
        RoundScore,
        PlayerDied,
        MonsterSpawned,
        ScenarioName,
        VersionNumber,
        ComboEndValue,
        ComboEndReason
    }
    public class SessionEventArgs : EventArgs
    {
        public bool RequestsSucceeded;
        public GameService.GameSession PreviousSession;
    }
    public class RetryEventArgs : EventArgs
    {
        public int Attempt;
    }
    public class GameplayDataCaptureSystem : Singleton<GameplayDataCaptureSystem>
    {
        public delegate void NewSessionResultEventHandler(object sender, SessionEventArgs e);
        public delegate void DataRetryEventHandler(object sender, RetryEventArgs e);
        public delegate void ScenarioXMLEventHandler(string XML);
        public delegate void ELORankEventHandler(string ELORank);
        public event NewSessionResultEventHandler NewSessionResultEvent;
        public event DataRetryEventHandler DataRetryEvent;
        public event ScenarioXMLEventHandler ScenarioXMLEvent;
        public event ELORankEventHandler ELORankEvent;

        static Uri BrowseUri = new Uri("https://www.google.com/accounts/Logout?continue=https://appengine.google.com/_ah/logout?continue=https://csci4950.azurewebsites.net/Account/LogOffGame");
        static Uri DataUri = new Uri("https://csci4950.azurewebsites.net/odata");
        GameService.Container serviceContainer;
        private String Cookies = "";
        private bool LoggedIn = false;
        private int CurrentSessionID = -1;
        private GameService.GameSession CurrentSession;
        private bool sending = false;
        private string Version;

        private object IsSendingAsync = new object();

        public int SessionID { get { return CurrentSessionID; } }

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
        private void ShowBrowser()
        {
            Form loginWindow = new Form();
            loginWindow.Size = new System.Drawing.Size(300, 850);
            loginWindow.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            loginWindow.StartPosition = FormStartPosition.CenterScreen;
            loginWindow.Text = "Tower Offense";
            loginWindow.ControlBox = false;
            loginWindow.FormClosing += loginWindow_FormClosing;

            Label l = new Label();
            l.Dock = DockStyle.Fill;
            l.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            l.Text = "Loading...";
            l.Visible = false;
            l.Cursor = Cursors.WaitCursor;
            loginWindow.Controls.Add(l);

            WebBrowser browse = new WebBrowser();
            browse.Dock = DockStyle.Fill;

            browse.Url = BrowseUri;

            browse.DocumentCompleted += browse_DocumentCompleted;
            browse.Navigating += browse_Navigating;
            loginWindow.Controls.Add(browse);



            loginWindow.ShowDialog();
        }

        void loginWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
                e.Cancel = true;
        }

        void browse_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            WebBrowser b = sender as WebBrowser;
            Form f = b.Parent as Form;
            f.Controls[0].Visible = true;
        }
        private void browse_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            WebBrowser b = sender as WebBrowser;
            Form f = b.Parent as Form;
            f.Controls[0].Visible = false;
            Cookies = GetGlobalCookies(b.Document.Url.AbsoluteUri);
            if (Cookies != null && Cookies.Contains(".AspNet.ApplicationCookie"))
            {
                LoggedIn = true;
                f.Hide();
            }
            if (e.Url.ToString().ToLower().Contains("google") || e.Url.ToString().ToLower().Contains("external"))
            {
                f.Size = new System.Drawing.Size(500, 900);
            }
            else
                f.Size = new System.Drawing.Size(300, 850);
        }

        /// <summary>
        /// Inits the Data Context and readies for logging
        /// </summary>
        public void InitContext(String Version)
        {
            this.Version = Version;

            serviceContainer = new GameService.Container(DataUri);
            serviceContainer.Format.UseJson();
            serviceContainer.SaveChangesDefaultOptions = SaveChangesOptions.Batch;
            serviceContainer.MergeOption = System.Data.Services.Client.MergeOption.PreserveChanges;

            serviceContainer.ReceivingResponse += (s, e) =>
            {
                String ContentType = e.ResponseMessage.GetHeader("Content-Type");
                if (ContentType != null && !(ContentType.Contains("application/atom+xml") || ContentType.Contains("application/json") || ContentType.Contains("multipart/mixed")))
                {
                    LoggedIn = false;
                }
            };
            serviceContainer.SendingRequest2 += (s, e) =>
            {
                e.RequestMessage.SetHeader("Cookie", Cookies);
            };
        }

        /// <summary>
        /// Prompts the user to log into their account. If already logged in, logs out first
        /// </summary>
        public void Login()
        {
            Cookies = "";
            LoggedIn = false;

            ShowBrowser();
        }
        public void SaveChangesAsync()
        {
            Thread t = new Thread(new ThreadStart(() =>
            {
                lock (IsSendingAsync)
                {
                    SaveChanges();
                }
            }));
            t.Priority = ThreadPriority.BelowNormal;
            t.IsBackground = true;
            t.Start();
        }
        public void NewSessionAsync()
        {
            Thread t = new Thread(new ThreadStart(() => NewSession()));
            t.IsBackground = true;
            t.Start();
        }
        /// <summary>
        /// Ends the current session and saves to server
        /// </summary>
        /// <returns></returns>
        public bool EndSession()
        {
            if (CurrentSession != null)
            {
                lock (IsSendingAsync)
                {

                    var gs = CurrentSession;
                    CurrentSession = null;
                    CurrentSessionID = -1;

                    gs.Completed = true;
                    serviceContainer.UpdateObject(gs);

                    bool res = SaveChanges();

                    return res;
                }

            }
            return true;
        }
        /// <summary>
        /// Ends previous session if there are any, and starts a new capture session. Saves to server
        /// </summary>
        private bool NewSession()
        {
            lock (IsSendingAsync)
            {
                //end old session
                if (CurrentSession != null)
                {
                    CurrentSession.Completed = true;
                    serviceContainer.UpdateObject(CurrentSession);
                }

                //create new session
                GameService.GameSession gs = new GameService.GameSession();
                serviceContainer.AddToGameSession(gs);

                //set session vars
                SessionEventArgs s = new SessionEventArgs();
                s.PreviousSession = CurrentSession;

                bool res = SaveChanges();

                CurrentSessionID = gs.SessionID;
                CurrentSession = gs;
                if (s.PreviousSession != null)
                    s.PreviousSession = serviceContainer.GameSession.Where(gamesession => gamesession.SessionID == s.PreviousSession.SessionID).FirstOrDefault();

                //log version number
                LogEvent(CaptureEventType.VersionNumber, Version);

                //fire event
                s.RequestsSucceeded = res;
                if (NewSessionResultEvent != null) NewSessionResultEvent(this, s);

                return res;
            }
        }
        public void GetScenarioAsync()
        {
            Thread t = new Thread(new ThreadStart(() =>
            {
                lock (IsSendingAsync)
                {
                    NetworkRetry(() =>
                        {
                            Uri actionUri = new Uri(DataUri, "odata/GameSession/ELORank");
                            var ret = serviceContainer.Execute<string>(actionUri, "POST", true, null);
                            string ELORank = ret.First();
                            if (ELORankEvent != null)
                            {
                                ELORankEvent(ELORank);
                            }
                            actionUri = new Uri(DataUri, "odata/GameSession/Scenario");
                            ret = serviceContainer.Execute<string>(actionUri, "POST", true, null);
                            string scenario = ret.First();
                            if (ScenarioXMLEvent != null)
                            {
                                ScenarioXMLEvent(scenario);
                            }
                            return true;
                        });
                }
            }));
            t.IsBackground = true;
            t.Start();
        }

        public string GetLiveGameVersion()
        {
            string cur = "";
            NetworkRetry(() =>
            {
                Uri actionUri = new Uri(DataUri, "odata/GameSession/CurrentVersion");
                var ret = serviceContainer.Execute<string>(actionUri, "POST", true, null);
                cur = ret.First();
                return true;
            });
            return (cur != null) ? cur : "";
        }

        public void GetELORankAsync()
        {
            Thread t = new Thread(new ThreadStart(() =>
            {
                lock (IsSendingAsync)
                {
                    NetworkRetry(() =>
                    {
                        Uri actionUri = new Uri(DataUri, "odata/GameSession/ELORank");
                        var ret = serviceContainer.Execute<string>(actionUri, "POST", true, null);
                        string ELORank = ret.First();
                        if (ELORankEvent != null)
                            ELORankEvent(ELORank);
                        return true;
                    });
                }
            }));
            t.IsBackground = true;
            t.Start();
        }

        /// <summary>
        /// Attempts the specified network operation with exponential retries. Catches exceptions as failure
        /// </summary>
        /// <param name="networkOperation">Network operation to attempt</param>
        /// <returns></returns>
        private bool NetworkRetry(Func<bool> networkOperation)
        {
            if (!LoggedIn)//Attempt to log in
                Login();
            if (!LoggedIn)//Failed to log in
                return false;

            bool sent = false;
            for (int i = 0; i < 4; i++)
            {
                if (i > 0)
                {
                    RetryEventArgs e = new RetryEventArgs();
                    e.Attempt = i;
                    if (DataRetryEvent != null) DataRetryEvent(this, e);
                }
                try
                {
                    sent = networkOperation();
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    Thread.Sleep((int)Math.Pow(2, i + 1) * 100);
                    continue;
                }
            }
            return sent;
        }
        /// <summary>
        /// Attempts to save changes to server
        /// </summary>
        /// <returns>Success</returns>
        private bool SaveChanges()
        {
            return NetworkRetry(() =>
            {
                var serviceResponse = serviceContainer.SaveChanges();
                foreach (var operationResponse in serviceResponse)
                {
                    if (operationResponse.StatusCode != 201)
                    {
                        throw new Exception("Bad response from server");
                    }
                }
                return true;
            });
        }


        /// <summary>
        /// Logs an event to the current session
        /// </summary>
        /// <param name="type"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public bool LogEvent(CaptureEventType type, string Data)
        {
            if (!LoggedIn)//Attempt to log in
                Login();
            if (!LoggedIn)//Failed to log in
                return false;
            if (CurrentSessionID == -1)//No session created
                return false;

            GameService.GameEventModel ge = new GameService.GameEventModel();
            ge.SessionId = CurrentSessionID;
            ge.Timestamp = DateTime.Now;
            ge.Type = type.ToString();
            ge.Data = Data;
            serviceContainer.AddToGameEvent(ge);
            //if (!sending)
            //{
            //    sending = true;
            //    serviceContainer.BeginSaveChanges(UpdateCallback, null);
            //}
            return true;
        }
    }
}
