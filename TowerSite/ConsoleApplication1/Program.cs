using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerSite.Models;

namespace LocalDBAccess
{
    public class Program
    {
        private 
        static void Main(string[] args)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            db.Database.SqlQuery();

        }
    }
}
