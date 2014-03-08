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

            if (gamesession.Completed)
                RunScoreCalculations(gamesession);
            
            return Updated(gamesession);
        }

        private async void RunScoreCalculations(GameSession gs)
        {
            await db.Database.ExecuteSqlCommandAsync(@"
DECLARE @Session INT;
SET @Session = {0};
DECLARE @HitRate FLOAT;
DECLARE @KillRate FLOAT;
DECLARE @RoundHealth FLOAT;
DECLARE @HealthRemaining FLOAT;
SET @HitRate = CAST((SELECT COUNT(*) FROM GameEventModels WHERE (SessionId = @Session AND Type = 'PlayerWeaponFirstHit')) AS FLOAT) / (SELECT COUNT(*) FROM GameEventModels WHERE (SessionId = @Session AND Type = 'PlayerWeaponFired'));
SET @KillRate = CAST((SELECT COUNT(*) FROM GameEventModels WHERE (SessionId = @Session AND Type = 'MonsterKilled')) AS FLOAT) / (SELECT COUNT(*) FROM GameEventModels WHERE (SessionId = @Session AND Type = 'MonsterSpawned'));
SELECT @RoundHealth = Data FROM GameEventModels WHERE (SessionId = @Session AND Type = 'RoundHealth')
SET @HealthRemaining = @RoundHealth / 100

DECLARE @TotalHealth FLOAT;
DECLARE @Monster VARCHAR(50);
DECLARE db_cursor CURSOR FOR SELECT Data FROM GameEventModels WHERE (SessionId = @Session AND Type = 'MonsterSpawned');

SET @TotalHealth = 0;
OPEN db_cursor   

FETCH NEXT FROM db_cursor INTO @Monster

WHILE @@FETCH_STATUS = 0   
BEGIN   
    
    SET @TotalHealth = @TotalHealth + CASE @Monster 
        WHEN 'MeleeMonster' THEN 240
        WHEN 'SuicideBomber' THEN 1
        WHEN 'LandmineDropper' THEN 120
        WHEN 'RangedMonster' THEN 120
        WHEN 'Grenadier' THEN 100
        WHEN 'Flametower' THEN 100
        WHEN 'SnipMonster' THEN 120
        WHEN 'WallHugger' THEN 25
        ELSE 0
    END;

    FETCH NEXT FROM db_cursor INTO @Monster 
END

DECLARE @TotalDamage FLOAT;
DECLARE @CurDmg FLOAT;
DECLARE db_cursor2 CURSOR FOR SELECT Data FROM GameEventModels WHERE (SessionId = @Session AND Type = 'MonsterDamageTaken');

SET @TotalDamage = 0;
OPEN db_cursor2  

FETCH NEXT FROM db_cursor2 INTO @CurDmg
WHILE @@FETCH_STATUS = 0   
BEGIN   
    SET @TotalDamage = @TotalDamage + @CurDmg;
    FETCH NEXT FROM db_cursor2 INTO @CurDmg
END

DECLARE @DamageDealt FLOAT;
SET @DamageDealt = @TotalDamage / @TotalHealth;

/*SET @HitRate = 0;
SET @DamageDealt = 0;
SET @KillRate = 0;
SET @HealthRemaining = 0;*/

DECLARE @PlayerScore FLOAT;
SET @PlayerScore = (dbo.InlineMaxF(3 * (@HitRate), .01) + 2 * @DamageDealt + 2 * @KillRate + 4 * @HealthRemaining) / (3 * (1) + 2 * 1 + 2 * 1 + 4 * 0);

/*PRINT 'Hit Rate: ' + CAST(@HitRate AS VARCHAR)
PRINT 'Kill Rate: ' + CAST(@KillRate AS VARCHAR)
PRINT 'Health Remaining: ' + CAST(@HealthRemaining AS VARCHAR)
PRINT 'DamageDealt: ' + CAST(@DamageDealt AS VARCHAR)
PRINT 'Player Score: ' + CAST(@PlayerScore AS VARCHAR)*/

UPDATE GameSessions SET HitRate = @HitRate, KillRate = @KillRate, HealthRemaining = @HealthRemaining, DamageDealt = @DamageDealt, SessionScore = @PlayerScore WHERE SessionID = @Session;
            ", gs.SessionID);
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
