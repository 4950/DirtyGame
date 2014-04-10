using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TowerSite.Models
{
    public class ScenarioELO
    {
        public int ID { get; set; }
        public String ScenarioID { get; set; }
        public int ELO { get; set; }
        public int LinearELO { get; set; }
        public int GamesPlayed { get; set; }
        public string ScenarioXML { get; set; }
    }
}