using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.OData;
using System.Web.Http.OData.Routing;
using TowerSite.Models;
using Microsoft.AspNet.Identity;

namespace TowerSite.Controllers
{
    /*
    To add a route for this controller, merge these statements into the Register method of the WebApiConfig class. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using TowerSite.Models;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<GameSession>("GameSession");
    config.Routes.MapODataRoute("odata", "odata", builder.GetEdmModel());
    */
    public class GameSessionController : ODataController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET odata/GameSession
        [Queryable]
        public IQueryable<GameSession> GetGameSession()
        {
            String userID = User.Identity.GetUserId();
            if (userID == null)
                return null;
            return db.GameSessions.Where(gamesession => gamesession.UserID == userID).OrderBy(gamesession => gamesession.SessionID);
        }

        // GET odata/GameSession(5)
        [Queryable]
        public SingleResult<GameSession> GetGameSession([FromODataUri] int key)
        {
            return SingleResult.Create(db.GameSessions.Where(gamesession => gamesession.ID == key).OrderBy(gamesession => gamesession.SessionID));
        }

        // PUT odata/GameSession(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, GameSession gamesession)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != gamesession.ID)
            {
                return BadRequest();
            }

            db.Entry(gamesession).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameSessionExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(gamesession);
        }

        // POST odata/GameSession
        public async Task<IHttpActionResult> Post(GameSession gamesession)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            gamesession.UserID = User.Identity.GetUserId();
            IQueryable<GameSession> q = db.GameSessions.OrderByDescending(gs => gs.SessionID);
            if (q.Count() > 0)
                gamesession.SessionID = q.First().SessionID + 1;
            else
                gamesession.SessionID = 0;
            db.GameSessions.Add(gamesession);
            await db.SaveChangesAsync();

            return Created(gamesession);
        }

        // PATCH odata/GameSession(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<GameSession> patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            GameSession gamesession = await db.GameSessions.FindAsync(key);
            if (gamesession == null)
            {
                return NotFound();
            }

            patch.Patch(gamesession);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameSessionExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(gamesession);
        }

        // DELETE odata/GameSession(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            GameSession gamesession = await db.GameSessions.FindAsync(key);
            if (gamesession == null)
            {
                return NotFound();
            }

            db.GameSessions.Remove(gamesession);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool GameSessionExists(int key)
        {
            return db.GameSessions.Count(e => e.ID == key) > 0;
        }
    }
}
