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
using System.Web.Security;
using TowerSite.Models;
using Microsoft.AspNet.Identity;
using System.Diagnostics;

namespace TowerSite.Controllers
{
    /*
    To add a route for this controller, merge these statements into the Register method of the WebApiConfig class. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using TowerSite.Models;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<GameEventModel>("GameEvent");
    config.Routes.MapODataRoute("odata", "odata", builder.GetEdmModel());
    */
    public class GameEventController : ODataController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET odata/GameEvent
        [Queryable]
        public IQueryable<GameEventModel> GetGameEvent()
        {
            String UserID = User.Identity.GetUserId();
            int? sessionID = null;
            IQueryable<GameSession> q = db.GameSessions.AsNoTracking().Where(gs => gs.UserID == UserID).OrderByDescending(gs => gs.SessionID);
            if (q.Count() > 0)
                sessionID = q.First().SessionID;
            if (sessionID != null)
                return db.GameEventModels.AsNoTracking().Where(ge => ge.SessionId == sessionID);
            return null;
        }

        // GET odata/GameEvent(5)
        [Queryable]
        public SingleResult<GameEventModel> GetGameEventModel([FromODataUri] int key)
        {
            return SingleResult.Create(db.GameEventModels.AsNoTracking().Where(gameeventmodel => gameeventmodel.ID == key));
        }

        // PUT odata/GameEvent(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, GameEventModel gameeventmodel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != gameeventmodel.ID)
            {
                return BadRequest();
            }

            db.Entry(gameeventmodel).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameEventModelExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(gameeventmodel);
        }

        // POST odata/GameEvent
        public async Task<IHttpActionResult> Post(GameEventModel gameeventmodel)
        {
            db.Configuration.AutoDetectChangesEnabled = false;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.GameEventModels.Add(gameeventmodel);
            await db.SaveChangesAsync();

            return Created(gameeventmodel);
        }

        // PATCH odata/GameEvent(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<GameEventModel> patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            GameEventModel gameeventmodel = await db.GameEventModels.FindAsync(key);
            if (gameeventmodel == null)
            {
                return NotFound();
            }

            patch.Patch(gameeventmodel);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameEventModelExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(gameeventmodel);
        }

        // DELETE odata/GameEvent(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            GameEventModel gameeventmodel = await db.GameEventModels.FindAsync(key);
            if (gameeventmodel == null)
            {
                return NotFound();
            }

            db.GameEventModels.Remove(gameeventmodel);
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

        private bool GameEventModelExists(int key)
        {
            return db.GameEventModels.Count(e => e.ID == key) > 0;
        }
    }
}
