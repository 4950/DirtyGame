--ENUMS
local melee, ranged, suicideBomber, wallHugger, grenadier, flameTower = 1, 2, 3, 4, 5, 6

-- Map vars
Arena = {
    name = "Arena",
    mapFile="Arena.tmx", 
    playerSpawns = { {x=652, y=784} }, 
    monsterSpawns = { {x=336, y=240}, {x=176, y=1040}, {x=1112, y=1040} }
}

-- each comp is number of {melee, ranged, suicideBomber}
composition = {} 
composition[ #composition+1 ] = {1, 0, 0, 0, 0, 0}
composition[ #composition+1 ] = {0, 1, 0, 0, 0, 0}
composition[ #composition+1 ] = {0, 0, 1, 0, 0, 0}
composition[ #composition+1 ] = {0, 0, 0, 1, 0, 0}
composition[ #composition+1 ] = {0, 0, 0, 0, 1, 0}

-- OLD Compositions
--[[
for i = 1, 10 do
    composition[ #composition+1 ] = {i, i, i}
end

for i = 1, 5 do
    composition[ #composition+1 ] = {2*i, i, i}
end

for i = 1, 5 do
    composition[ #composition+1 ] = {i, 2*i, i}
end

for i = 1, 5 do
    composition[ #composition+1 ] = {i, i, 2*i}
end ]]

-- NEW Compositions
ind = {1, 3, 5}
for _, i in ipairs(ind) do
composition[ #composition+1 ] = { 2 * i, 2 * i, 2 * i, 0, 0, 1 }
composition[ #composition+1 ] = { 2 * i, 0, 2 * i, 2 * i, 0, 1 }
composition[ #composition+1 ] = { 2 * i, 0, 0, 2 * i, 2 * i, 1 }

composition[ #composition+1 ] = { 0, 2 * i, 2 * i, 2 * i, 0, 1 }
composition[ #composition+1 ] = { 0, 2 * i, 0, 2 * i, 2 * i, 1 }
composition[ #composition+1 ] = { 2 * i, 2 * i, 0, 0, 2 * i, 1 }

composition[ #composition+1 ] = { 0, 0, 2 * i, 2 * i, 2 * i, 1 }
composition[ #composition+1 ] = { 2 * i, 0, 2 * i, 0, 2 * i, 1 }

composition[ #composition+1 ] = { 2 * i, 2 * i, 0, 2 * i, 0, 1 }

composition[ #composition+1 ] = { 0, 2 * i, 2 * i, 0, 2 * i, 1 }
end

for i = 1, 2 do
composition[ #composition + 1 ] = {4 * i, 8 * i, 0, 0, 0, 1}
composition[ #composition + 1 ] = {4 * i, 0, 8 * i, 0, 0, 1}
composition[ #composition + 1 ] = {4 * i, 0, 0, 8 * i, 0, 1}
composition[ #composition + 1 ] = {4 * i, 0, 0, 0, 8 * i, 1}

composition[ #composition + 1 ] = {0, 4 * i, 8 * i, 0, 0, 1}
composition[ #composition + 1 ] = {0, 4 * i, 0, 8 * i, 0, 1}
composition[ #composition + 1 ] = {0, 4 * i, 0, 0, 8 * i, 1}
composition[ #composition + 1 ] = {8 * i, 4 * i, 0, 0, 0, 1}

composition[ #composition + 1 ] = {0, 0, 4 * i, 8 * i, 0, 1}
composition[ #composition + 1 ] = {0, 0, 4 * i, 0, 8 * i, 1}
composition[ #composition + 1 ] = {8 * i, 0, 4 * i, 0, 0, 1}
composition[ #composition + 1 ] = {0, 8 * i, 4 * i, 0, 0, 1}

composition[ #composition + 1 ] = {0, 0, 0, 4 * i, 8 * i, 1}
composition[ #composition + 1 ] = {8 * i, 0, 0, 4 * i, 0, 1}
composition[ #composition + 1 ] = {0, 8 * i, 0, 4 * i, 0, 1}
composition[ #composition + 1 ] = {0, 0, 8 * i, 4 * i, 0, 1}

composition[ #composition + 1 ] = {8 * i, 0, 0, 0, 4 * i, 1}
composition[ #composition + 1 ] = {0, 8 * i, 0, 0, 4 * i, 1}
composition[ #composition + 1 ] = {0, 0, 8 * i, 0, 4 * i, 1}
composition[ #composition + 1 ] = {0, 0, 0, 8 * i, 4 * i, 1}
end

-- add map and composition to the "scenearios". This will be iterated and make all the compositions on a map
scenarios = {}
scenarios[ #scenarios+1 ] = {map = Arena, comp = composition}

function spawnerBuilder(x, y, monsterType, monsterWeapon, numberOfMonsters, time, healthUp, damageUp)
    local numberOfMonsters = numberOfMonsters or 1
    local time = time or 1000
    local healthUp = healthUp or 0
    local damageUp = damageUp or 0

    return 
    "\n    <spawner xPosition=\"".. x .."\" \
        yPosition=\"" .. y .."\" \
        monsterType=\"" .. monsterType .. "\" \
        monsterWeapon=\"" .. monsterWeapon .. "\" \
        numberOfMonsters=\"" .. numberOfMonsters .. "\" \
        timeSpanMilliseconds=\"" .. time .. "\" \
        healthUpModifier=\"" .. healthUp .. "\" \
        damageUpModifier=\"" .. damageUp .. "\"> \
    </spawner>"
end

-- take in an "enum" and returns the monster type and weapon as strings. Useful for spawnerBuilder
function monsterToStrings(monsterType)
    if monsterType == melee then
        return "MeleeMonster", "Monstersword"
    elseif monsterType == ranged then
        return "RangedMonster", "Monsterbow"
    elseif monsterType == suicideBomber then
        return "SuicideBomber", "BomberWeapon"
    elseif monsterType == wallHugger then
        return "WallHugger", "WallHuggerWeapon"
    elseif monsterType == grenadier then
        return "Grenadier", "GrenadeLauncher"
    elseif monsterType == flameTower then
        return "Flametower", "FlametowerWeapon"
    end
end

-- time to update some sqlers
sql = ""

-- this one is for inserting the sqls
sql2 = ""

-- counter
counter = 0

-- and now for the magics
file = "<?xml version=\"1.0\" encoding=\"utf-8\"?> \
<root id=\"Scenarios\">"
for _, scenario in ipairs(scenarios) do
    for i = 1, #scenario.comp do
        local playerSpawn = math.random(#scenario.map.playerSpawns)

        scenarioName = scenario.map.name .. "_scenario_M" .. scenario.comp[i][melee] .. "_R" .. scenario.comp[i][ranged] .."_S" 
        scenarioName = scenarioName .. scenario.comp[i][suicideBomber] .. "_W" .. scenario.comp[i][wallHugger] .. "_G" 
        scenarioName = scenarioName .. scenario.comp[i][grenadier] .. "_F" .. scenario.comp[i][flameTower]

        scenarioMetaData = " \
<scenario name=\"" .. scenarioName .. "\" \
    difficultyScore=\"0.50\" \
    map=\"" .. scenario.map.mapFile .. "\" \
    playerX=\"" .. scenario.map.playerSpawns[playerSpawn].x .. "\" \
    playerY=\"" .. scenario.map.playerSpawns[playerSpawn].y .. "\">"

        file = file .. scenarioMetaData

        sql = sql .. "UPDATE [dbo].[ScenarioELOes] SET ScenarioXML = '<?xml version=\"1.0\" encoding=\"utf-8\"?> \
<root id=\"Scenarios\">" .. scenarioMetaData

        sql2 = sql2 .. "INSERT INTO [dbo].[ScenarioELOes] VALUES('" .. scenarioName .. "', 800, 800, 0, '<?xml version=\"1.0\" encoding=\"utf-8\"?> \
<root id=\"Scenarios\">" .. scenarioMetaData

    -- THE SPAWNS
    -- j will go from 1-n, representing the type of monster
    for j = 1, #scenario.comp[i] do
        -- k will iterate through the number of each type of monster
        for k = 1, scenario.comp[i][j] do

            -- pick a spwan location. Flame Tower is centerd. random spawn location for others
            local x, y
            if j == flameTower then
                x = 650
                y = 650
            else
                local monsterSpawn = math.random(#scenario.map.monsterSpawns)
                x = scenario.map.monsterSpawns[monsterSpawn].x
                y = scenario.map.monsterSpawns[monsterSpawn].y
            end

            -- append it to the file
            spawner = spawnerBuilder(x, y, monsterToStrings(j))
            file = file .. spawner
            sql = sql .. spawner
            sql2 = sql2 .. spawner

        end
    end

    -- CLOSE THAT SCENARIO
    file = file .. "\n</scenario>"

    sql = sql .. "</scenario> \
</root>" .. "' WHERE ScenarioID = '" .. scenarioName .. "';" .. "\n"

    sql2 = sql2 .. "</scenario> \
</root>" .. "');\n"

    -- COUNT IT!
    counter = counter + 1

    end

    file = file .. "\n\n"

end

-- CLOSE DAT XML ROOT
file = file .. "\n</root>"

-- WriteOut
local outFile = io.open("Scenarios.xml", "w")
outFile:write(file)
outFile:close()

local outFile = io.open("UpdateScenarios.sql", "w")
outFile:write(sql)
outFile:close()

local outFile = io.open("InsertScenarios.sql", "w")
outFile:write(sql2)
outFile:close()

print("Just created " .. counter .. " scenarios!")