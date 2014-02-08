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

namespace DirtyGame.game.Util
{
    public enum CaptureEventType
    {
        MapSelected,
        PlayerWeaponFired,
        MonsterWeaponFired,
        MonsterDamageTaken,
        PlayerDamageTaken,
        PlayerWeaponFirstHit,
        MonsterKilled,
        RoundEnded,
        RoundHealth,
        PlayerDiedWithScore,
        MonsterSpawned
    }
    public class GameplayDataCaptureSystem : Singleton<GameplayDataCaptureSystem>
    {
        private static string databaseFileName = "sessions.sqlite";
        private DataContext db;

        [Table(Name="Sessions")]
        private class Session
        {
            [Column(IsPrimaryKey = true, IsDbGenerated = true)]
            public int Id { get; set; }

            [Column]
            public string Guid { get; set; }

            public static string CreateCommand = "CREATE TABLE IF NOT EXISTS Sessions ( Id INTEGER PRIMARY KEY, Guid TEXT )";
        }

        [Table(Name = "Events")]
        private class Event
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
        private class CaptureSession
        {
            internal Table<Event> sessionTable;

            internal int sessionID;
            internal Guid Guid;
            internal Guid systemID;
        }

        public GameplayDataCaptureSystem()
        {
            if(!File.Exists(databaseFileName))
                SQLiteConnection.CreateFile(databaseFileName);
            var connection = new SQLiteConnection("DbLinqProvider=Sqlite;Data Source="+databaseFileName+";");
            
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
            s.Guid = Guid.NewGuid();

            sessions.Add(s.Guid, s);

            if (Default)
                defaultSession = s;

            
            Table<Session> table = db.GetTable<Session>();

            Session ts = new Session{ Guid = s.Guid.ToString() };
            table.InsertOnSubmit(ts);

            db.SubmitChanges();
            s.sessionID = ts.Id;
            s.sessionTable = db.GetTable<Event>();

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
        /// <summary>
        /// Writes all session data to disk
        /// </summary>
        public void FlushSessions()
        {
            db.SubmitChanges();
        }
        public void LogEvent(CaptureEventType type, string Data, Guid SessionId)
        {
            if (sessions.ContainsKey(SessionId))
            {
                CaptureSession s = sessions[SessionId];

                Event ts = new Event { SessionId = s.sessionID, Time = DateTime.Now, Type = type.ToString(), Data = Data};
                s.sessionTable.InsertOnSubmit(ts);
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
