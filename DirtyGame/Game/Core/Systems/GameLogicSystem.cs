using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirtyGame.game.Core.Components;
using DirtyGame.game.Core.Components.Render;
using DirtyGame.game.Core.Systems.Monster;
using DirtyGame.game.Core.Systems.Util;
using DirtyGame.game.Core.Events;
using Microsoft.Xna.Framework;
using EntityFramework.Systems;
using EntityFramework;
using DirtyGame.game.SGraphics;
using CoreUI.Elements;
using CoreUI.DrawEngines;
using DirtyGame.game.Util;
using System.Xml;
using DirtyGame.game.Core.GameStates;

namespace DirtyGame.game.Core.Systems
{
    public class GameLogicSystem : EntitySystem
    {

        public int monstersdefeated;
        public int monstersalive;
        private Dirty game;
        private Label roundLabel;
        private float roundLblTime;
        private float roundTime;
        private EntityRef gameEntity;
        private bool cheatEndRound = false;

        private List<Entity> spawners = new List<Entity>();

        //Dictionary that contains a set of scenario objects
        private Dictionary<string, Scenario> scenarios = new Dictionary<string, Scenario>();

        public override void OnEntityAdded(Entity e)
        {
            if (e.HasComponent<MonsterComponent>())
            {
                monstersalive++;
            }
        }
        private void resetRound()
        {
            foreach (Entity ee in spawners)//clear old spawners
            {
                game.world.DestroyEntity(ee);
            }
            gameEntity.entity.GetComponent<PropertyComponent<int>>("GameKills").value += monstersdefeated;
            monstersdefeated = 0;

            //restore player health
            game.player.GetComponent<StatsComponent>().CurrentHealth = game.player.GetComponent<StatsComponent>().MaxHealth;
        }
        public void SetupNextRound()
        {
            resetRound();

            int CurrentLevel = gameEntity.entity.GetComponent<PropertyComponent<int>>("GameRound").value;

            roundLabel.Text = "~Round " + CurrentLevel + "~";
            roundLabel.Visibility = CoreUI.Visibility.Visible;
            roundLblTime = 3f;

            int numRanged = 2 + 2 * CurrentLevel;
            int numMelee = 2 + 2 * CurrentLevel;

            switch (CurrentLevel)
            {
                case 1:
                    numRanged = 0;
                    break;
                case 2:
                    numRanged /= 2;
                    numMelee /= 2;
                    break;
            }

            Entity e = game.entityFactory.CreateSpawner(200, 200, new Rectangle(0, 0, 46, 46), "RangedMonster", "Monsterbow", numRanged / 2, new TimeSpan(0, 0, 0, 0, 500));
            e.Refresh();
            spawners.Add(e);
            e = game.entityFactory.CreateSpawner(300, 100, new Rectangle(0, 0, 46, 46), "MeleeMonster", "Monstersword", numMelee / 2, new TimeSpan(0, 0, 0, 0, 500));
            e.Refresh();
            spawners.Add(e);
            e = game.entityFactory.CreateSpawner(100, 300, new Rectangle(0, 0, 46, 46), "RangedMonster", "Monsterbow", numRanged / 2, new TimeSpan(0, 0, 0, 0, 500));
            e.Refresh();
            spawners.Add(e);
            e = game.entityFactory.CreateSpawner(300, 640, new Rectangle(0, 0, 46, 46), "MeleeMonster", "Monstersword", numMelee / 2, new TimeSpan(0, 0, 0, 0, 500));
            e.Refresh();
            spawners.Add(e);
            e = game.entityFactory.CreateSpawner(300, 640, new Rectangle(0, 0, 46, 46), "Flametower", "FlametowerWeapon", 1, new TimeSpan(0, 0, 0, 0, 500));
            e.Refresh();
            spawners.Add(e);
            e = game.entityFactory.CreateSpawner(300, 200, new Rectangle(0, 0, 46, 46), "SuicideBomber", "BomberWeapon", 1, new TimeSpan(0, 0, 0, 0, 500));
            e.Refresh();
            spawners.Add(e);
            e = game.entityFactory.CreateSpawner(300, 200, new Rectangle(0, 0, 46, 46), "Grenadier", "GrenadeLauncher", 1, new TimeSpan(0, 0, 0, 0, 500));
            e.Refresh();
            spawners.Add(e);

            //show buy phase before starting
            //if (CurrentLevel > 1)
            //    BuyPhase();

            roundTime = 60;
        }
        private void SetupBoss()
        {
            /*resetRound();

            int CurrentLevel = gameEntity.entity.GetComponent<PropertyComponent<int>>("GameRound").value;

            roundLabel.Text = "~Boss Battle~";
            roundLabel.Visibility = CoreUI.Visibility.Visible;
            roundLblTime = 3f;

            Entity monsterWeapon = game.entityFactory.CreateRangedWeaponEntity("Monsterbow", "bow", "bow", 400, 20 + 20 * (CurrentLevel / 5f), 10, "arrow", -1, 3f, 100, 0);
            monsterWeapon.Refresh();
            MonsterData rangedData = MonsterData.RangedMonster;
            rangedData.weapon = monsterWeapon;
            rangedData.scale = 3;
            rangedData.Health = (int)(500 * (CurrentLevel / 4f));

            Entity monsterMelee = game.entityFactory.CreateMeleeWeaponEntity("Monstersword", "sword", 50, 15 + 15 * (CurrentLevel / 5f), -1, 2f, 100, 0, game.resourceManager.GetResource<SpriteSheet>("SwordMeleeSpriteSheet"));
            monsterMelee.Refresh();
            MonsterData meleeData = MonsterData.BasicMonster;
            meleeData.weapon = monsterMelee;
            meleeData.Health += (int)(meleeData.Health * (CurrentLevel / 5f));

            Entity e = game.entityFactory.CreateSpawner(100, 100, game.resourceManager.GetResource<SpriteSheet>("playerSheet"), new Rectangle(0, 0, 46, 46), rangedData, 1, new TimeSpan(0, 0, 0, 0, 1000));
            e.Refresh();
            spawners.Add(e);*/
        }
        private void AdvanceLevel()
        {
            gameEntity.entity.GetComponent<PropertyComponent<int>>("GameRound").value++;
            //if (gameEntity.entity.GetComponent<PropertyComponent<int>>("GameRound").value % 4 == 0)
            //    SetupBoss();
            //else
            SetupNextRound();
        }
        private void BuyPhase()
        {
            Events.Event buy = new Events.Event();
            buy.name = "GameStateBuy";
            EventManager.Instance.TriggerEvent(buy);
        }

        //Decoding the XML code for the scenarios.
        public void decodeScenariosXML(string xmlFile)
        {
            //Setting up the XML reader
            XmlReaderSettings xmlSettings = new XmlReaderSettings();
            xmlSettings.IgnoreWhitespace = true;
            xmlSettings.IgnoreComments = true;
            XmlReader scenarioReader = XmlReader.Create(xmlFile, xmlSettings);

            //Reads to the start of the XML file
            scenarioReader.ReadToFollowing("root");

            //TESTING
            //int scenarioCount = 0;
            //int spawnerCount = 0;

            //Parse the XML for the Scenarios
            while (scenarioReader.Read()) //(scenarioReader.ReadToFollowing("scenario"))
            {

                //scenarioCount++;

                //Temporary Variables
                //Scenario name
                string scenarioName;
                //Difficulty Score
                float difficultyScore;
                //Map name
                string mapName;
                //Location
                int xPosition;
                int yPosition;
                //? ? ? ? ?
                Rectangle spawnerRectangle;
                //Monster Type
                string monsterType;
                //Monster Weapon
                string monsterWeapon;
                //Number of Monsters
                int numberOfMonsters;
                //TimeSpan for Monsters to Spawn
                TimeSpan timePerSpawn;
                //Modifier for the spawner
                string modifier;
                //List of Spawners
                List<Spawner> spawners = new List<Spawner>();

                scenarioName = scenarioReader.GetAttribute("name");
                difficultyScore = (float) Convert.ToDouble(scenarioReader.GetAttribute("difficultyScore"));
                mapName = scenarioReader.GetAttribute("map");

                //Break out of the loop when done parsing the XML
                if (scenarioName == null || mapName == null)
                {
                    break;
                }

                scenarioReader.ReadToDescendant("spawner");
                        
                //Looping through the spawners for the scenario
                do
                {

                    //spawnerCount++;

                    xPosition = Convert.ToInt32(scenarioReader.GetAttribute("xPosition"));
                    yPosition = Convert.ToInt32(scenarioReader.GetAttribute("yPosition"));
                    spawnerRectangle = new Rectangle(Convert.ToInt32(scenarioReader.GetAttribute("rectangleValue1")),
                                                     Convert.ToInt32(scenarioReader.GetAttribute("rectangleValue2")),
                                                     Convert.ToInt32(scenarioReader.GetAttribute("rectangleValue3")),
                                                     Convert.ToInt32(scenarioReader.GetAttribute("rectangleValue4")));
                    monsterType = scenarioReader.GetAttribute("monsterType");
                    monsterWeapon = scenarioReader.GetAttribute("monsterWeapon");
                    numberOfMonsters = Convert.ToInt32(scenarioReader.GetAttribute("numberOfMonsters"));
                    timePerSpawn = new TimeSpan(Convert.ToInt32(scenarioReader.GetAttribute("timeSpanDays")),
                                                Convert.ToInt32(scenarioReader.GetAttribute("timeSpanHours")),
                                                Convert.ToInt32(scenarioReader.GetAttribute("timeSpanMinutes")),
                                                Convert.ToInt32(scenarioReader.GetAttribute("timeSpanSeconds")),
                                                Convert.ToInt32(scenarioReader.GetAttribute("timeSpanMilliseconds")));
                    modifier = scenarioReader.GetAttribute("modifier");

                    spawners.Add(new Spawner(xPosition, yPosition, spawnerRectangle, monsterType, monsterWeapon,
                                             numberOfMonsters, timePerSpawn, modifier));
                } while (scenarioReader.ReadToNextSibling("spawner"));

                scenarios.Add(scenarioName, new Scenario(scenarioName, difficultyScore, mapName, spawners));

                //spawnerCount = 0;
            }
        }

        public void setupScenario(string scenarioName)
        {
            Scenario tempScenario = scenarios[scenarioName];
            Entity e;

            //resetRound();

            foreach (Spawner s in tempScenario.Spawners)
            {
                e = game.entityFactory.CreateSpawner(s);
                e.Refresh();
                spawners.Add(e);
            }
        }

        public override void OnEntityRemoved(Entity e)
        {
            if (e.HasComponent<MonsterComponent>())
            {
                if (roundTime > 0)
                {
                    monstersdefeated++;
                    gameEntity.entity.GetComponent<PropertyComponent<int>>("GameScore").value += 50;
                    gameEntity.entity.GetComponent<PropertyComponent<int>>("GameCash").value += 10;

                    GameplayDataCaptureSystem.Instance.LogEvent(CaptureEventType.MonsterKilled, e.GetComponent<MonsterComponent>().data.Type);
                }
                if (--monstersalive == 0)
                {
                    /*
                    game.GameWon = true;

                    Event gamestate = new Event();
                    gamestate.name = "GameStateGameOver";
                    EventManager.Instance.TriggerEvent(gamestate);*/


                    GameplayDataCaptureSystem.Instance.LogEvent(CaptureEventType.RoundEnded, gameEntity.entity.GetComponent<PropertyComponent<int>>("GameRound").value.ToString());
                    GameplayDataCaptureSystem.Instance.LogEvent(CaptureEventType.RoundHealth, game.player.GetComponent<StatsComponent>().CurrentHealth.ToString());
                    //next game round
                    AdvanceLevel();
                }
            }

        }

        public override void ProcessEntities(IEnumerable<Entity> entities, float dt)
        {
            if (roundLblTime > 0)
            {
                roundLblTime -= dt;
                if (roundLblTime <= 0)
                {
                    roundLblTime = 0;
                }
            }
            //if (roundTime > 0)
            //{
            //    roundTime -= dt;
            //    if(roundLblTime == 0)//if done showing round number, show time
            //        roundLabel.Text = "Time Remaining: " + (int)roundTime + "s";

            //    if (roundTime <= 0)//if time over, end round
            //    {
            //        roundTime = 0;
            //        for (int i = 0; i < entities.Count(); i++)
            //        {
            //            Entity e = entities.ElementAt(i);
            //            if (e.HasComponent<MonsterComponent>())
            //            {
            //                World.DestroyEntity(e);
            //                i--;
            //            }
            //        }
            //    }
            //}

            if (cheatEndRound)
            {
                cheatEndRound = false;
                for (int i = 0; i < entities.Count(); i++)
                {
                    Entity e = entities.ElementAt(i);
                    if (e.HasComponent<MonsterComponent>())
                    {
                        World.DestroyEntity(e);
                        i--;
                    }
                }
            }
            else
            {
                for (int i = 0; i < entities.Count(); i++)
                {
                    Entity e = entities.ElementAt(i);

                    StatsComponent hc = e.GetComponent<StatsComponent>();
                    if (hc.CurrentHealth <= 0)//dead
                    {
                        if (e.HasComponent<PlayerComponent>())//player died
                        {

                            GameplayDataCaptureSystem.Instance.LogEvent(CaptureEventType.PlayerDiedWithScore, "");

                            game.GameWon = false;

                            Event gamestate = new Event();
                            gamestate.name = "GameStateGameOver";
                            EventManager.Instance.TriggerEvent(gamestate);

                        }
                        else
                        {
                            World.DestroyEntity(e);
                            i--;
                        }
                    }

                }
            }
        }

        public override void Initialize()
        {
            monstersdefeated = 0;

            roundLabel = new Label();
            roundLabel.Size = new System.Drawing.Point(200, 40);
            roundLabel.Position = new System.Drawing.Point(200, 0);
            roundLabel.Foreground = new MonoGameColor(Microsoft.Xna.Framework.Color.Red);
            roundLabel.Background = new MonoGameColor(Microsoft.Xna.Framework.Color.Black);
            roundLabel.TextPosition = TextPosition.Center;
            roundLabel.Visibility = CoreUI.Visibility.Hidden;
            game.UIEngine.Children.AddElement(roundLabel);
            gameEntity = game.gameEntity;
            game.baseContext.RegisterHandler(Microsoft.Xna.Framework.Input.Keys.OemTilde, CheatEndRound, null);
        }

        private void CheatEndRound(Microsoft.Xna.Framework.Input.Keys key)
        {
            cheatEndRound = true;
        }

        public GameLogicSystem(Dirty game)
            : base(SystemDescriptions.GameLogicSystem.Aspect, SystemDescriptions.GameLogicSystem.Priority)
        {
            this.game = game;

        }
    }
}
