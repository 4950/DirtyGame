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
        PlayerDamageTaken,
        PlayerWeaponFirstHit,
        PlayerHitByWeapon,
        MonsterHitByWeapon,
        MonsterKilled,
        RoundEnded,
        RoundHealth,
        PlayerDiedWithScore,
        MonsterSpawned,
        ScenarioName,
        VersionNumber,
        ComboEndValue,
        ComboEndReason
    }
    public class GameplayDataCaptureSystem : Singleton<GameplayDataCaptureSystem>
    {
        static Uri BrowseUri = new Uri("https://csci4950.azurewebsites.net/Account/Login");
        static Uri DataUri = new Uri("https://csci4950.azurewebsites.net/odata");
        GameService.Container serviceContainer;
        private String Cookies = "";
        private bool LoggedIn = false;
        private int CurrentSessionID = -1;
        private GameService.GameSession CurrentSession;
        private bool sending = false;
        private string Version;

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
            loginWindow.Text = "CSCI 4950 Project";
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
        }

        /// <summary>
        /// Inits the Data Context and readies for logging
        /// </summary>
        public void InitContext(String Version)
        {
            this.Version = Version;

            serviceContainer = new GameService.Container(DataUri);
            serviceContainer.SaveChangesDefaultOptions = SaveChangesOptions.Batch;
            serviceContainer.MergeOption = System.Data.Services.Client.MergeOption.PreserveChanges;

            serviceContainer.ReceivingResponse += (s, e) =>
            {
                String ContentType = e.ResponseMessage.GetHeader("Content-Type");
                if (ContentType != null && !(ContentType.Contains("application/atom+xml") || ContentType.Contains("multipart/mixed")))
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
        /// <summary>
        /// Starts a new capture session
        /// </summary>
        public bool NewSession()
        {
            if (!LoggedIn)//Attempt to log in
                Login();
            if (!LoggedIn)//Failed to log in
                return false;
            if (CurrentSession != null)
                EndSession();

            GameService.GameSession gs = new GameService.GameSession();
            serviceContainer.AddToGameSession(gs);
            var serviceResponse = serviceContainer.SaveChanges();
            CurrentSessionID = gs.SessionID;
            CurrentSession = gs;
            foreach (var operationResponse in serviceResponse)
            {
                if (operationResponse.StatusCode != 201)
                    return false;
            }

            LogEvent(CaptureEventType.VersionNumber, Version);

            return true;
        }

        /// <summary>
        /// Ends the current session and returns session data
        /// </summary>
        /// <returns></returns>
        public GameService.GameSession EndSession()
        {
            FlushData();

            if (CurrentSession != null)
            {
                var gs = CurrentSession;
                CurrentSession = null;
                CurrentSessionID = -1;

                gs.Completed = true;
                serviceContainer.UpdateObject(gs);
                serviceContainer.SaveChanges();

                gs = serviceContainer.GameSession.Where(gamesession => gamesession.SessionID == gs.SessionID).FirstOrDefault();

#if DEBUG
                MessageBox.Show("Session ID: " + gs.SessionID + "\n\nAccuracy: " + (gs.HitRate * 100) + "%\nPlayerScore: " + gs.SessionScore, "Round Results");
#else
                MessageBox.Show("Continue to next round", "Round Finished");
#endif

                return gs;
            }
            return null;
        }

        /// <summary>
        /// Wrties all pending data to server. Shows a window
        /// </summary>
        /// <returns>True on success</returns>
        public bool FlushData()
        {
            if (!LoggedIn)//Attempt to log in
                Login();
            if (!LoggedIn)//Failed to log in
                return false;

            Form f = new Form();
            f.Size = new System.Drawing.Size(100, 50);
            f.FormBorderStyle = FormBorderStyle.None;
            f.StartPosition = FormStartPosition.CenterScreen;
            f.TopMost = true;

            Label l = new Label();
            l.Dock = DockStyle.Fill;
            l.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            l.Text = "Sending data...";
            l.ForeColor = System.Drawing.Color.Red;
            f.Controls.Add(l);

            f.Show();
            f.Activate();

            Thread.Sleep(100);

            var serviceResponse = serviceContainer.SaveChanges();
            foreach (var operationResponse in serviceResponse)
            {
                if (operationResponse.StatusCode != 201)
                {
                    f.Hide();
                    return false;
                }
            }
            f.Hide();

            return true;
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
        private void UpdateCallback(IAsyncResult result)
        {
            try
            {
                serviceContainer.EndSaveChanges(result);
            }
            catch (Exception e)
            {
                //if (LoggedIn)//If not logged in, we know what the error is
                //    throw e;
            }
            finally
            {
                sending = false;
            }
        }
    }
}
