using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TowerSite.Models
{
    public class GameEventModel
    {
        public int ID { get; set; }
        public int SessionId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Type { get; set; }
        public string Data { get; set; }
    }
}