using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TowerSite.Models
{
    public class GameSession
    {
        public int ID { get; set; }
        public String UserID { get; set; }
        public int SessionID { get; set; }
        public bool Completed { get; set; }
        public float HitRate { get; set; }
        public float KillRate { get; set; }
        public float DamageDealt { get; set; }
        public float HealthRemaining { get; set; }
        public float SessionScore { get; set; }
    }
}