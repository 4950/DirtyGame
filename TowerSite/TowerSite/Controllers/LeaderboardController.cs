using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TowerSite.Models;
using System.Diagnostics;
using PagedList;
using PagedList.Mvc;

namespace TowerSite.Controllers
{
    public class LeaderboardController : Controller
    {
        /* SQL to generate View:
        IF object_id(N'Leaderboard', 'V') IS NOT NULL
        DROP VIEW Leaderboard
        GO

        CREATE VIEW Leaderboard AS
        SELECT TOP 1000 IsNull(ROW_NUMBER() OVER(ORDER BY A.LinearELO DESC), -1) AS Rank, B.UserName, A.ELO, A.LinearELO FROM PlayerELOes A, AspNetUsers B WHERE A.UserID = B.Id AND A.GamesPlayed > 1 ORDER BY LinearELO DESC
        */

        private Entities db = new Entities();

        // GET: /Leaderboard/
        [AllowAnonymous]
        public async Task<ActionResult> Index(string searchString, int? page)
        {
            List<Leaderboard> list = null;
            try
            {
                if (searchString != null && searchString != "")
                {
                    page = 1;
                    list = await db.Leaderboards.AsNoTracking().Where(s => s.UserName.ToUpper().Contains(searchString.ToUpper())).ToListAsync();
                }
                else
                    list = await db.Leaderboards.AsNoTracking().ToListAsync();
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(list.ToPagedList(pageNumber, pageSize));
        }

        //// GET: /Leaderboard/Details/5
        //public async Task<ActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Leaderboard leaderboard = await db.Leaderboards.FindAsync(id);
        //    if (leaderboard == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(leaderboard);
        //}

        //// GET: /Leaderboard/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: /Leaderboard/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Create([Bind(Include="ID,UserName,ELO,LinearELO")] Leaderboard leaderboard)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Leaderboards.Add(leaderboard);
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }

        //    return View(leaderboard);
        //}

        //// GET: /Leaderboard/Edit/5
        //public async Task<ActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Leaderboard leaderboard = await db.Leaderboards.FindAsync(id);
        //    if (leaderboard == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(leaderboard);
        //}

        //// POST: /Leaderboard/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Edit([Bind(Include="ID,UserName,ELO,LinearELO")] Leaderboard leaderboard)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(leaderboard).State = EntityState.Modified;
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }
        //    return View(leaderboard);
        //}

        //// GET: /Leaderboard/Delete/5
        //public async Task<ActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Leaderboard leaderboard = await db.Leaderboards.FindAsync(id);
        //    if (leaderboard == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(leaderboard);
        //}

        //// POST: /Leaderboard/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DeleteConfirmed(int id)
        //{
        //    Leaderboard leaderboard = await db.Leaderboards.FindAsync(id);
        //    db.Leaderboards.Remove(leaderboard);
        //    await db.SaveChangesAsync();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
