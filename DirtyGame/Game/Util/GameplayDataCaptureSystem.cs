using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;

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
    public struct CaptureEvent
    {
        public CaptureEventType type;
        public DateTime timestamp;
        public String data;
    }
    public class CaptureSession
    {
        internal List<CaptureEvent> events = new List<CaptureEvent>();

        internal Guid id;
        internal Guid systemID;
    }
    public partial class GameplayDataCaptureSystem : Singleton<GameplayDataCaptureSystem>
    {


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
            s.id = new Guid();

            sessions.Add(s.id, s);

            if (Default)
                defaultSession = s;

            return s.id;
        }
        /// <summary>
        /// Logs an event to the default session
        /// </summary>
        public void LogEvent(CaptureEventType type, string Data)
        {
            if (defaultSession != null)
                LogEvent(type, Data, defaultSession.id);
        }
        public void LogEvent(CaptureEventType type, string Data, Guid SessionId)
        {
            if (sessions.ContainsKey(SessionId))
            {
                CaptureSession s = sessions[SessionId];

                CaptureEvent e = new CaptureEvent();
                e.data = Data;
                e.type = type;
                e.timestamp = DateTime.Now;

                s.events.Add(e);
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
