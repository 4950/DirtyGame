using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Data.SQLite;
//using System.Data.Linq;
using System.Data.Linq.Mapping;
using DbLinq.Sqlite;
using DbLinq.Data.Linq;
using System.IO;
using System.ComponentModel;

namespace Dirtygame.game.Util
{
    public enum CaptureEventType
    {
        MapSelected,
        PlayerWeaponFired,
        PlayerHitWithWeapon,
        MonsterDamageTaken,
        PlayerDamageTaken
    }
    public class CaptureSession
    {
        internal int sessionID;
        internal Guid Guid;
        internal Guid systemID;
    }
    public class GameplayDataCaptureSystem : Singleton<GameplayDataCaptureSystem>
    {
        private DataContext db;

        [Table(Name="Sessions")]
        class Session
        {
            [Column(IsPrimaryKey = true, IsDbGenerated = true)]
            public int Id { get; set; }

            [Column]
            public string Guid { get; set; }

            public static string CreateCommand = "CREATE TABLE IF NOT EXISTS Sessions ( Id INTEGER PRIMARY KEY, Guid TEXT )";
        }

        [Table(Name = "Events")]
        class Event
        {
            [Column(IsPrimaryKey = true, IsDbGenerated = true)]
            public int Id { get; set; }

            [Column]
            public int SessionId { get; set; }

            [Column]
            public DateTime Time { get; set; }

            [Column]
            public string Type { get; set; }

            [Column]
            public string Data { get; set; }

            public static string CreateCommand = "CREATE TABLE IF NOT EXISTS Events ( Id INTEGER PRIMARY KEY, SessionId INTEGER, Time INTEGER, Type TEXT, Data TEXT )";
        }

        public GameplayDataCaptureSystem()
        {
            if(!File.Exists("test.sqlite"))
                SQLiteConnection.CreateFile("test.sqlite");
            var connection = new SQLiteConnection("DbLinqProvider=Sqlite;Data Source=test.sqlite;");
            
            db = new DataContext(connection);
            db.ExecuteCommand(Session.CreateCommand);
            db.ExecuteCommand(Event.CreateCommand);
        }

        private Dictionary<Guid, CaptureSession> sessions = new Dictionary<Guid, CaptureSession>();
        private CaptureSession defaultSession;
        /// <summary>
        /// Creates a new logging session
        /// </summary>
        /// <param name="Default">Makes this session the default session</param>
        /// <returns >Returns the session id</returns>
        public Guid CreateSession(bool Default)
        {
            CaptureSession s = new CaptureSession();
            s.systemID = GetSystemID();
            s.Guid = new Guid();

            sessions.Add(s.Guid, s);

            if (Default)
                defaultSession = s;

            
            Table<Session> table = db.GetTable<Session>();

            Session ts = new Session{ Guid = s.Guid.ToString() };
            table.InsertOnSubmit(ts);

            db.SubmitChanges();
            s.sessionID = ts.Id;

            return s.Guid;
        }
        /// <summary>
        /// Logs an event to the default session
        /// </summary>
        public void LogEvent(CaptureEventType type, string Data)
        {
            if (defaultSession != null)
                LogEvent(type, Data, defaultSession.Guid);
        }
        public void LogEvent(CaptureEventType type, string Data, Guid SessionId)
        {
            if (sessions.ContainsKey(SessionId))
            {
                CaptureSession s = sessions[SessionId];

                Table<Event> table = db.GetTable<Event>();

                Event ts = new Event { SessionId = s.sessionID, Time = DateTime.Now, Type = type.ToString(), Data = Data};
                table.InsertOnSubmit(ts);

                db.SubmitChanges();

                /*
                CaptureEvent e = new CaptureEvent();
                e.data = Data;
                e.type = type;
                e.timestamp = DateTime.Now;

                s.events.Add(e);*/
            }
        }
        private Guid GetSystemID()
        {
            Guid macAddressGuid;
            NetworkInterface[] networkInterface = NetworkInterface.GetAllNetworkInterfaces();
            string id = networkInterface.FirstOrDefault().Id;

            Guid.TryParse(id, out macAddressGuid);

            return macAddressGuid;
        }
    }
}
