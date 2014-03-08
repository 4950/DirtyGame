DECLARE @Session INT;
SET @Session = 34;
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
SELECT * FROM GameSessions WHERE SessionID = @Session;