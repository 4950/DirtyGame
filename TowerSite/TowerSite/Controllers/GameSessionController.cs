﻿using System;
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
using System.Diagnostics;

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

        [HttpPost]
        public async Task<IHttpActionResult> Scenario(ODataActionParameters parameters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            String xml = "";
            try
            {
                string userID = User.Identity.GetUserId();
                if (userID == null)
                    return BadRequest("Not logged in");

                var query = db.Database.SqlQuery<ScenarioELO>(@"
DECLARE @UserID NVARCHAR(MAX);
SET @UserID = @p0;


DECLARE @GamesPlayed INT;
SELECT @GamesPlayed = GamesPlayed FROM PlayerELOes WHERE UserID = @UserID;


IF( @@ROWCOUNT <> 1 )/* No Player, add to table*/
BEGIN
	INSERT INTO PlayerELOes (ELO, LinearELO, UserID) VALUES (800, 800, @UserID);
    SET @GamesPlayed = 0;
END

/*
SET @GamesPlayed = (@GamesPlayed % 75) + 34;

SELECT * FROM ScenarioELOes WHERE ID = @GamesPlayed;
*/

DECLARE @PlayerELO INT;
SELECT @PlayerELO = LinearELO FROM PlayerELOes WHERE UserID = @UserID;

SELECT TOP 1 * FROM ScenarioELOes WHERE DATALENGTH(ScenarioXML) > 0  ORDER BY ABS( LinearELO - @PlayerELO )
", userID);
                var res = await query.FirstOrDefaultAsync();

                var eloQuery = db.Database.SqlQuery<PlayerELO>(@"
DECLARE @UserID NVARCHAR(MAX);
SET @UserID = @p0;

DECLARE @PlayerELO INT;
SELECT * FROM PlayerELOes WHERE UserID = @UserID;
", userID);
               /* var rankQuery = db.Database.SqlQuery<PlayerRank>(@"
DECLARE @UserID NVARCHAR(MAX);
SET @UserID = @p0;

SELECT Ranking FROM
(SELECT RANK() OVER (ORDER BY ELO DESC) AS Ranking, UserID
    FROM PlayerELOes WHERE gamesPlayed<>0) AS Temp WHERE UserID = @UserID;
", userID);*/

                var elo = await eloQuery.FirstOrDefaultAsync();

                //var rank = await rankQuery.FirstOrDefaultAsync();

                //  Trace.WriteLine("XML: \n" + "<base>" + res.ScenarioXML
                //     + "<elo value=\"" + elo.ELO + "\"></elo></base>");

                //Rip apart the XML here because hatred and bile
                xml = res.ScenarioXML;
                xml = xml.Insert(xml.IndexOf(">") + 1, "<base>");
                xml += "<elo value=\"" + elo.ELO +
                    //"\" rank=\""+ rank.Ranking +
                    "\"></elo></base>";
                Trace.WriteLine("Serving: "+res.ScenarioID+" LinearELO: "+res.LinearELO);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
            }

            return Ok(xml);
        }

        [HttpPost]
        public async Task<IHttpActionResult> ELORank(ODataActionParameters parameters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            String ELORank = "";
            Trace.WriteLine("ELORank Called!");
            try
            {
                string userID = User.Identity.GetUserId();
                if (userID == null)
                    return BadRequest("Not logged in");

                var eloQuery = db.Database.SqlQuery<PlayerELO>(@"
DECLARE @UserID NVARCHAR(MAX);
SET @UserID = @p0;

DECLARE @PlayerELO INT;
SELECT * FROM PlayerELOes WHERE UserID = @UserID;
", userID);
                var rankQuery = db.Database.SqlQuery<PlayerRank>(@"
DECLARE @UserID NVARCHAR(MAX);
SET @UserID = @p0;

SELECT Ranking FROM
(SELECT RANK() OVER (ORDER BY ELO DESC) AS Ranking, UserID
    FROM PlayerELOes WHERE gamesPlayed<>0) AS Temp WHERE UserID = @UserID;
", userID);

                var elo = await eloQuery.FirstOrDefaultAsync();

                PlayerRank rank = new PlayerRank();
                try
                {
                    rank = await rankQuery.FirstOrDefaultAsync();
                }
                catch (Exception e)
                {
                    Trace.WriteLine("possibly new player?");
                    Trace.WriteLine(e.Message);
                    rank.Ranking = -1;
                }


                if (rank.Ranking <= 0)
                {
                    rank.Ranking = -1;
                }

                ELORank = "" + elo.ELO + ","+rank.Ranking;
                Trace.WriteLine("ELORank: " + ELORank);

            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                ELORank = "0,-1";
            }

            return Ok(ELORank);
        }



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
            {
                await RunScoreCalculations(gamesession);
                await db.Entry(gamesession).ReloadAsync();
            }

            return Updated(gamesession);
        }

        private async Task RunScoreCalculations(GameSession gs)
        {
            try
            {
                var res = await db.Database.ExecuteSqlCommandAsync(@"
DECLARE @Session INT;
SET @Session = @p0;
DECLARE @HitRate FLOAT;
DECLARE @KillRate FLOAT;
DECLARE @RoundHealth FLOAT;
DECLARE @HealthRemaining FLOAT;
DECLARE @WepFired INT;

/*Check if round ended properly*/
SELECT * FROM GameEventModels WHERE (SessionId = @Session AND Type = 'RoundEnded');
IF( @@ROWCOUNT <> 1 )
	RETURN;

/*Hit Rate*/
SET @WepFired = (SELECT COUNT(*) FROM GameEventModels WHERE (SessionId = @Session AND Type = 'MonsterWeaponFired'));
IF @WepFired = 0 SET @HitRate = 0;
ELSE SET @HitRate = CAST((SELECT COUNT(*) FROM GameEventModels WHERE (SessionId = @Session AND Type = 'PlayerDamageTaken')) AS FLOAT) / @WepFired;

DECLARE @Spawned FLOAT;
SET @Spawned = (SELECT COUNT(*) FROM GameEventModels WHERE (SessionId = @Session AND Type = 'MonsterSpawned'));
IF @Spawned > 0
    SET @KillRate = CAST((SELECT COUNT(*) FROM GameEventModels WHERE (SessionId = @Session AND Type = 'MonsterKilled')) AS FLOAT) / @Spawned;
ELSE
    SET @KillRate = 0;

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
        WHEN 'SuicideBomber' THEN 60
        WHEN 'LandmineDropper' THEN 120
        WHEN 'RangedMonster' THEN 120
        WHEN 'Grenadier' THEN 100
        WHEN 'Flametower' THEN 160
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
/*SET @PlayerScore = (dbo.InlineMaxF(3 * (1 - @HitRate), .01) + 2 * @DamageDealt + 2 * @KillRate + 3 * @HealthRemaining) / (3 * (1 - .25) + 2 * 1 + 2 * 1 + 4 * 0);*/
SET @PlayerScore = (2 * @DamageDealt + 2 * @KillRate + 3 * @HealthRemaining) / (2 * 1 + 2 * 1 + 3 * 0);

/*PRINT 'Hit Rate: ' + CAST(@HitRate AS VARCHAR)
PRINT 'Kill Rate: ' + CAST(@KillRate AS VARCHAR)
PRINT 'Health Remaining: ' + CAST(@HealthRemaining AS VARCHAR)
PRINT 'DamageDealt: ' + CAST(@DamageDealt AS VARCHAR)
PRINT 'Player Score: ' + CAST(@PlayerScore AS VARCHAR)*/

UPDATE GameSessions SET HitRate = @HitRate, KillRate = @KillRate, HealthRemaining = @HealthRemaining, DamageDealt = @DamageDealt, SessionScore = @PlayerScore WHERE SessionID = @Session;
SELECT * FROM GameSessions WHERE SessionID = @Session;
            ", gs.SessionID);

                await db.Entry(gs).ReloadAsync();

                res = await db.Database.ExecuteSqlCommandAsync(@"
DECLARE @SessionID INT;
DECLARE @UserID NVARCHAR(MAX);
DECLARE @PlayerScore FLOAT;

SET @SessionID = @p0;

SELECT @UserID = UserID, @PlayerScore = SessionScore FROM GameSessions WHERE SessionID = @SessionID;

/*Check if round ended properly*/
SELECT * FROM GameEventModels WHERE (SessionId = @SessionID AND Type = 'RoundEnded');
IF( @@ROWCOUNT <> 1 )
	RETURN;

DECLARE @ScenarioID NVARCHAR(MAX);
/*Get Scenario*/
SELECT @ScenarioID = Data FROM GameEventModels WHERE (SessionId = @SessionID AND Type = 'ScenarioName');

DECLARE @PlayerGamesPlayed INT;
DECLARE @PlayerELO FLOAT;
DECLARE @PlayerELOLinear FLOAT;
/*Check if player ELO exists*/
SELECT @PlayerELO = ELO, @PlayerELOLinear = LinearELO, @PlayerGamesPlayed = GamesPlayed FROM PlayerELOes WHERE UserID = @UserID;
IF( @@ROWCOUNT <> 1 )/* No ELO, set to default */
BEGIN
	INSERT INTO PlayerELOes (ELO, LinearELO, UserID) VALUES (800, 800, @UserID);
	SET @PlayerELO = 800;
	SET @PlayerELOLinear = 800;
	SET @PlayerGamesPlayed = 0;
END

DECLARE @ScenarioGamesPlayed INT;
DECLARE @ScenarioELO FLOAT;
DECLARE @ScenarioELOLinear FLOAT;
/*Check if player ELO exists*/
SELECT @ScenarioELO = ELO, @ScenarioELOLinear = LinearELO, @ScenarioGamesPlayed = GamesPlayed FROM ScenarioELOes WHERE ScenarioID = @ScenarioID;
IF( @@ROWCOUNT <> 1 )/* No ELO, set to default */
BEGIN
	INSERT INTO ScenarioELOes (ELO, LinearELO, ScenarioID) VALUES (800, 800, @ScenarioID);
	SET @ScenarioELO = 800;
	SET @ScenarioELOLinear = 800;
	SET @ScenarioGamesPlayed = 0;
END

/*Calculate regular ELO*/
DECLARE @EPlayer FLOAT;
DECLARE @EScen FLOAT;

SET @EPlayer = 1.0 / (1.0 + POWER(10.0, (@ScenarioELO - @PlayerELO) / 400.0));
SET @EScen = 1.0 / (1.0 + POWER(10.0, (@PlayerELO - @ScenarioELO) / 400.0));

DECLARE @SPlayer FLOAT;
DECLARE @SScen FLOAT;

SELECT * FROM GameEventModels WHERE (SessionId = @SessionID AND Type = 'PlayerDied');
IF( @@ROWCOUNT <> 1 )
BEGIN
	SET @SPlayer = 1;
	SET @SScen = 0;
END
ELSE
BEGIN
	SET @SPlayer = 0;
	SET @SScen = 1;
END

DECLARE @PlayerK FLOAT;
DECLARE @ScenK FLOAT;

IF( @PlayerGamesPlayed < 30 )
    SET @PlayerK = 50;
ELSE IF ( @PlayerELO < 2200)
	SET @PlayerK = 30;
ELSE IF ( @PlayerELO < 2400)
	SET @PlayerK = 20;
ELSE
	SET @PlayerK = 10;

IF( @ScenarioGamesPlayed < 30)
    SET @ScenK = 30;
ELSE IF ( @ScenarioELO < 2200)
	SET @ScenK = 30;
ELSE IF ( @ScenarioELO < 2400)
	SET @ScenK = 20;
ELSE
	SET @ScenK = 10;

SET @PlayerELO = @PlayerELO + @PlayerK * (@SPlayer - @EPlayer);
SET @ScenarioELO = @ScenarioELO + @ScenK * (@SScen - @EScen);

/*Calculate linear ELO*/
SET @EPlayer = 1.0 / (1.0 + POWER(10.0, (@ScenarioELOLinear - @PlayerELOLinear) / 400.0));
SET @EScen = 1.0 / (1.0 + POWER(10.0, (@PlayerELOLinear - @ScenarioELOLinear) / 400.0));


/*Calculate expected*/
IF( @SPlayer = 1 )
	SET @SPlayer = (@PlayerScore / 1.75) * (1.0 - @EPlayer) + @EPlayer;
ELSE
	SET @SPlayer = (@PlayerScore / 1.75) * (1.0 - @EPlayer);

SET @SScen = 1 - @SPlayer;

/*Set K Values */
IF( @PlayerGamesPlayed < 30 )
    SET @PlayerK = 25;
ELSE IF ( @PlayerELOLinear < 2200)
	SET @PlayerK = 30;
ELSE IF ( @PlayerELOLinear < 2400)
	SET @PlayerK = 20;
ELSE
	SET @PlayerK = 10;

IF( @ScenarioGamesPlayed < 30)
    SET @ScenK = 25;
ELSE IF ( @ScenarioELOLinear < 2200)
	SET @ScenK = 30;
ELSE IF ( @ScenarioELOLinear < 2400)
	SET @ScenK = 20;
ELSE
	SET @ScenK = 10;

SET @PlayerELOLinear = @PlayerELOLinear + @PlayerK * (@SPlayer - @EPlayer);
SET @ScenarioELOLinear = @ScenarioELOLinear + @ScenK * (@SScen - @EScen);

/*Write Back new ELOs*/
UPDATE PlayerELOes SET ELO = ROUND(@PlayerELO, 0), LinearELO = ROUND(@PlayerELOLinear, 0), GamesPlayed = (@PlayerGamesPlayed + 1) WHERE UserID = @UserID;
UPDATE ScenarioELOes SET ELO = ROUND(@ScenarioELO, 0), LinearELO = ROUND(@ScenarioELOLinear, 0), GamesPlayed = (@ScenarioGamesPlayed + 1) WHERE ScenarioID = @ScenarioID;
                ", gs.SessionID);

            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
            }
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
