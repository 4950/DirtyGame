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
    }
}