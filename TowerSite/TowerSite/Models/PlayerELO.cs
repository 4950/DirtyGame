using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TowerSite.Models
{
    public class PlayerELO
    {
        public int ID { get; set; }
        public String UserID { get; set; }
        public int ELO { get; set; }
        public int LinearELO { get; set; }
        public int GamesPlayed { get; set; }
    }
}