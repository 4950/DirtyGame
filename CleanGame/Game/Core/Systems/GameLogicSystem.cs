using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CleanGame.Game.Core.Components;
using CleanGame.Game.Core.Components.Render;
using CleanGame.Game.Core.Systems.Monster;
using CleanGame.Game.Core.Systems.Util;
using CleanGame.Game.Core.Events;
using Microsoft.Xna.Framework;
using EntityFramework.Systems;
using EntityFramework;
using CleanGame.Game.SGraphics;
using CoreUI.Elements;
using CoreUI.DrawEngines;
using CleanGame.Game.Util;
using System.Xml;
using CleanGame.Game.Core.GameStates;
using CoreUI;
using Microsoft.Xna.Framework.Graphics;


namespace CleanGame.Game.Core.Systems
{
    public class GameLogicSystem : EntitySystem
    {
        private const float damagePnlMaxTime = .5f;
        private const byte damagePnlMaxAlpha = 128;

        private List<ProgressBar> mPBs;
        private Panel pbDisplay;
        private CoreUIEngine UIEngine;
        private Renderer renderer;
        public int monstersdefeated;
        public int monstersalive;
        private Dirty game;
        private Label ActionLabel;
        private Panel ActionLabelBack;
        private Label HitLabel;
        private Panel DamagePanel;
        private float damagePnlTime;
        private float roundTime;
        private float roundStartTime;
        private bool cheatEndRound = false;
        private int PlayerHits;
        private float playerHitTime;
        private bool tutorialMode;
        private List<Label> textFloaters = new List<Label>();

        private List<Entity> spawners = new List<Entity>();

        //Dictionary that contains a set of scenario objects
        private Dictionary<string, Scenario> scenarios = new Dictionary<string, Scenario>();

        public override void OnEntityAdded(Entity e)
        {
            if (e.HasComponent<StatsComponent>())
            {
                ProgressBar pb = new ProgressBar();
                pb.Size = new System.Drawing.Point(50, 5);
                pb.Value = 100;
                pbDisplay.AddElement(pb);
                mPBs.Add(pb);

                if (e.HasComponent<MonsterComponent>())
                {
                    monstersalive++;
                }
            }
        }
        /// <summary>
        /// Resets a round and clears spawners
        /// </summary>
        private void resetRound()
        {
            foreach (Entity ee in spawners)//clear old spawners
            {
                game.world.DestroyEntity(ee);
            }
            spawners.Clear();
            //game.gameEntity.entity.GetComponent<PropertyComponent<int>>("GameKills").value += monstersdefeated;
            monstersdefeated = 0;

            //restore player health
            game.player.GetComponent<StatsComponent>().CurrentHealth = game.player.GetComponent<StatsComponent>().MaxHealth;
        }
        public void AddTextFloater(string val)
        {
            Label floater = new Label();
            SpriteFont f = game.resourceManager.GetResource<SpriteFont>("HitsSmall");
            floater.mFontInt = new MonoGameFont(f);
            floater.Size = new System.Drawing.Point(200, 50);
            floater.Position = new System.Drawing.Point(game.currrentDisplayMode.Width / 2, game.currrentDisplayMode.Height / 2);
            floater.Foreground = new MonoGameColor(Microsoft.Xna.Framework.Color.White);
            floater.TextPosition = TextPosition.Center;
            floater.Text = val;
            game.UIEngine.Children.AddElement(floater);
            textFloaters.Add(floater);
        }
        /// <summary>
        /// Sets Tutorial state
        /// </summary>
        public void SetupTutorial()
        {
            ActionLabelBack.Visibility = Visibility.Hidden;
            resetRound();
            tutorialMode = true;
            game.player.GetComponent<PhysicsComponent>().movePlayer = true;
            game.player.GetComponent<SpatialComponent>().Position = new Vector2(200, 200);
            game.player.Refresh();
        }
        /// <summary>
        /// Starts a round using the old round system
        /// </summary>
        public void SetupNextRound()
        {
            resetRound();

            int CurrentLevel = game.gameEntity.entity.GetComponent<PropertyComponent<int>>("GameRound").value;

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

            Entity e = game.entityFactory.CreateSpawner(600, 600, new Rectangle(0, 0, 46, 46), "LandmineDropper", "LandmineWeapon", numMelee + 1, new TimeSpan(0, 0, 0, 0, 500));
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
            e = game.entityFactory.CreateSpawner(300, 200, new Rectangle(0, 0, 46, 46), "SnipMonster", "SnipWeapon", 1, new TimeSpan(0, 0, 0, 0, 500));

            e.Refresh();
            spawners.Add(e);
            e = game.entityFactory.CreateSpawner(300, 200, new Rectangle(0, 0, 46, 46), "Grenadier", "GrenadeLauncher", 1, new TimeSpan(0, 0, 0, 0, 500));
            e.Refresh();
            spawners.Add(e);

            e = game.entityFactory.CreateSpawner(300, 200, new Rectangle(0, 0, 46, 46), "WallHugger", "WallHuggerWeapon", 1, new TimeSpan(0, 0, 0, 0, 500));
            e.Refresh();
            spawners.Add(e);

            //show buy phase before starting
            //if (CurrentLevel > 1)
            //    BuyPhase();

            roundTime = 60;
        }
        public void PlayerDealtDamage()
        {
            playerHitTime = 4;
            PlayerHits++;
            HitLabel.Visibility = Visibility.Visible;
            HitLabel.Text = PlayerHits + " hits";
        }
        public void PlayerTookDamage()
        {
            ResetHitCounter();
            (DamagePanel.Background as MonoGameColor).color.A = damagePnlMaxAlpha;
            DamagePanel.Visibility = Visibility.Visible;
            damagePnlTime = damagePnlMaxTime;
        }
        private void ResetHitCounter()
        {
            int mul = (int)Math.Floor(PlayerHits / 10.0d);
            mul = PlayerHits * mul;
            if (mul > 0)
            {
                game.gameEntity.entity.GetComponent<PropertyComponent<int>>("GameScore").value += mul;
                AddTextFloater("+" + mul);
            }
            playerHitTime = 0;
            PlayerHits = 0;
            HitLabel.Visibility = Visibility.Hidden;
        }


        /// <summary>
        /// Decoding the XML code for the scenarios.
        /// </summary>
        /// <param name="xmlFile"></param>
        public void decodeScenariosXML(string xmlFile)
        {
            //Setting up the XML reader
            XmlReaderSettings xmlSettings = new XmlReaderSettings();
            xmlSettings.IgnoreWhitespace = true;
            xmlSettings.IgnoreComments = true;
            XmlReader scenarioReader = XmlReader.Create(xmlFile, xmlSettings);

            //Reads to the start of the XML file
            scenarioReader.ReadToFollowing("root");
            //scenarioReader.ReadStartElement();

            //TESTING
            //int scenarioCount = 0;
            //int spawnerCount = 0;

            //Parse the XML for the Scenarios
            while (scenarioReader.ReadToFollowing("scenario"))
            {

                //scenarioCount++;

                //MAP VARIABLES
                //Temporary Variables
                //Scenario name
                string scenarioName;
                //Difficulty Score
                float difficultyScore;
                //Map name
                string mapName;
                //Player Spawn Point
                Vector2 playerSpawnPoint;

                //SPAWNER VARIABLES
                //Monster Spawner Location
                int xPosition;
                int yPosition;
                //Monster Type
                string monsterType;
                //Monster Weapon
                string monsterWeapon;
                //Number of Monsters
                int numberOfMonsters;
                //TimeSpan for Monsters to Spawn
                TimeSpan timePerSpawn;
                //Modifier for health of the spawner
                float healthUpModifier;
                //Value of damageUp modifier
                float damageUpModifier;
                //List of Spawners
                List<Spawner> spawners = new List<Spawner>();

                scenarioName = scenarioReader.GetAttribute("name");
                difficultyScore = (float)Convert.ToDouble(scenarioReader.GetAttribute("difficultyScore"));
                mapName = scenarioReader.GetAttribute("map");
                playerSpawnPoint = new Vector2((float)Convert.ToDouble(scenarioReader.GetAttribute("playerX")),
                                               (float)Convert.ToDouble(scenarioReader.GetAttribute("playerY")));

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
                    monsterType = scenarioReader.GetAttribute("monsterType");
                    monsterWeapon = scenarioReader.GetAttribute("monsterWeapon");
                    numberOfMonsters = Convert.ToInt32(scenarioReader.GetAttribute("numberOfMonsters"));
                    timePerSpawn = new TimeSpan(0, 0, 0, 0, Convert.ToInt32(scenarioReader.GetAttribute("timeSpanMilliseconds")));
                    healthUpModifier = Convert.ToInt32(scenarioReader.GetAttribute("healthUpModifier"));
                    damageUpModifier = Convert.ToInt32(scenarioReader.GetAttribute("damageUpModifier"));

                    spawners.Add(new Spawner(xPosition, yPosition, monsterType, monsterWeapon,
                                             numberOfMonsters, timePerSpawn, healthUpModifier, damageUpModifier));
                } while (scenarioReader.ReadToNextSibling("spawner"));

                scenarios.Add(scenarioName, new Scenario(scenarioName, difficultyScore, mapName, spawners, playerSpawnPoint));

                //spawnerCount = 0;
            }
        }

        /// <summary>
        /// Starts a specific scenario
        /// </summary>
        /// <param name="scenarioName"></param>
        /// <param name="player"></param>
        public void setupScenario(string scenarioName)
        {
            Scenario tempScenario = scenarios[scenarioName];
            setupScenario(tempScenario);
        }
        /// <summary>
        /// Starts a specific scenario
        /// </summary>
        /// <param name="scenario"></param>
        public void setupScenario(Scenario scenario)
        {
            game.gameEntity.entity.GetComponent<PropertyComponent<int>>("GameRound").value++;

            Entity e;

            game.player.GetComponent<SpatialComponent>().Position = scenario.PlayerSpawn;

            if (scenario.Spawners != null)
            {
                foreach (Spawner s in scenario.Spawners)
                {
                    e = game.entityFactory.CreateSpawner(s);
                    e.Refresh();
                    spawners.Add(e);
                }
            }
        }

        //Returns a random scenario for the selected map
        public Scenario randomScenario(string mapName)
        {
            //random scenario to return
            Random random = new Random();
            int randomScenario = random.Next(0, scenarios.Count);
            int count = -1;
            Scenario playScenario = new Scenario();

            //Making sure that the scenario is made for the map
            foreach (var scenario in scenarios.Values)
            {
                //break out if the random number scenario for that map
                if (count == randomScenario)
                {
                    break;
                }
                //found a map with the same name
                if (scenario.MapName.Equals(mapName))
                {
                    playScenario = (Scenario)scenario;
                    count++;
                }
            }

            return playScenario;
        }

        public void scenarioForPlayerScore(string mapName, float playerScore)
        {
            //WORK IN PROGRESS
        }

        public override void OnEntityRemoved(Entity e)
        {
            if (e.HasComponent<StatsComponent>())
            {
                pbDisplay.RemoveElement(mPBs[mPBs.Count - 1]);
                mPBs.RemoveAt(mPBs.Count - 1);

                if (e.HasComponent<MonsterComponent>())
                {
                    //if (roundTime > 0)
                    //{
                    monstersdefeated++;

                    AddTextFloater("+" + (50 + PlayerHits));
                    game.gameEntity.entity.GetComponent<PropertyComponent<int>>("GameScore").value += 50 + PlayerHits;
                    game.gameEntity.entity.GetComponent<PropertyComponent<int>>("GameCash").value += 10;
                    game.gameEntity.entity.GetComponent<PropertyComponent<int>>("GameKills").value += 1;

                    GameplayDataCaptureSystem.Instance.LogEvent(CaptureEventType.MonsterKilled, e.GetComponent<MonsterComponent>().data.Type);
                    //}
                    if (--monstersalive == 0 && !tutorialMode)
                    {
                        foreach (Entity sp in spawners)//make sure there aren't more monsters spawning
                            if (sp.GetComponent<SpawnerComponent>().numMobs != 0)
                                return;


                        GameplayDataCaptureSystem.Instance.LogEvent(CaptureEventType.RoundEnded, game.gameEntity.entity.GetComponent<PropertyComponent<int>>("GameRound").value.ToString());
                        GameplayDataCaptureSystem.Instance.LogEvent(CaptureEventType.RoundHealth, game.player.GetComponent<StatsComponent>().CurrentHealth.ToString());

                        StartPreRound();
                    }
                }
            }

        }
        /// <summary>
        /// Shows the round label and sets up a timer to the next round
        /// </summary>
        public void StartPreRound()
        {
            resetRound();
            //next game round
            //AdvanceLevel();
            game.ClearField = true;
            //start next round in 5
            roundStartTime = 5f;
            ActionLabel.Text = "Get Ready";
            ActionLabel.Position = new System.Drawing.Point(-ActionLabel.Size.X / 2, ActionLabel.Position.Y);
            ActionLabelBack.Visibility = CoreUI.Visibility.Visible;
        }
        public override void ProcessEntities(IEnumerable<Entity> entities, float dt)
        {
            for (int i = 0; i < textFloaters.Count; i++)
            {
                Label floater = textFloaters[i];
                System.Drawing.Point pos = floater.Position;
                pos.X += (int)(dt * 150);
                pos.Y -= (int)(dt * 150);
                floater.Position = pos;

                if (pos.Y <= 0)
                {
                    textFloaters.Remove(floater);
                    game.UIEngine.Children.RemoveElement(floater);
                    i--;
                }
            }
            if (damagePnlTime > 0)
            {
                (DamagePanel.Background as MonoGameColor).color.A = (byte)(damagePnlMaxAlpha * (damagePnlTime / damagePnlMaxTime));
                damagePnlTime -= dt;
                if (damagePnlTime <= 0)
                {
                    damagePnlTime = 0;
                    DamagePanel.Visibility = Visibility.Hidden;
                }
            }
            if (playerHitTime > 0)
            {
                playerHitTime -= dt;
                if (playerHitTime <= 0)
                {
                    ResetHitCounter();
                }
            }
            if (roundStartTime > 0 && !tutorialMode)
            {
                roundStartTime -= dt;

                if (roundStartTime > 4.5)
                {
                    float mult = ((5 - roundStartTime) * 2);
                    ActionLabel.Position = new System.Drawing.Point((int)(-ActionLabel.Size.X / 2 + ActionLabel.Size.X / 2 * mult), ActionLabel.Position.Y);
                }
                else if (Math.Floor(roundStartTime) == 1 && Math.Round(roundStartTime) == 1)
                {
                    ActionLabel.Text = "Go!";
                    ActionLabel.Position = new System.Drawing.Point(-ActionLabel.Size.X, ActionLabel.Position.Y);
                }
                else if (roundStartTime < 1 && roundStartTime > .5f)
                {
                    float mult = ((1 - roundStartTime) * 2);
                    ActionLabel.Position = new System.Drawing.Point((int)(-ActionLabel.Size.X / 2 + ActionLabel.Size.X / 2 * mult), ActionLabel.Position.Y);
                }
                else if (roundStartTime <= 0)
                {
                    roundStartTime = 0;

                    ActionLabelBack.Visibility = Visibility.Hidden;
                    //if (game.gameEntity.entity.GetComponent<PropertyComponent<int>>("GameRound").value == 0)
                    //{
                    //Setting the movePlayer flag in the physics component of the player
                    game.player.GetComponent<PhysicsComponent>().movePlayer = true;
                    //TODO need to have the map name here
                    setupScenario(randomScenario(game.mapName));
                    game.player.Refresh();
                    //}
                    //else
                    //    AdvanceLevel();

                }
            }
            //if (roundTime > 0)
            //{
            //    roundTime -= dt;
            //    if (roundLblTime == 0)//if done showing round number, show time
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
                resetRound();
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
                    else//update PBs
                    {
                        SpatialComponent sc = e.GetComponent<SpatialComponent>();

                        ProgressBar pb = mPBs[i];
                        pb.Maximum = hc.MaxHealth;
                        pb.Value = (int)hc.CurrentHealth;

                        Vector2 pos = sc.Position + new Vector2(sc.Width / 2 - pb.Size.X / 2, -20);
                        pos = Vector2.Transform(pos, renderer.ActiveCamera.Transform);
                        if (pos.X >= 0 && pos.Y >= 0 && pos.X <= renderer.ActiveCamera.size.X && pos.Y <= renderer.ActiveCamera.size.Y)
                        {
                            pb.Visibility = Visibility.Visible;
                            pb.Position = new System.Drawing.Point((int)pos.X, (int)pos.Y);
                        }
                        else
                        {
                            pb.Visibility = Visibility.Hidden;
                        }

                    }

                }
            }
        }

        public override void Initialize()
        {
            monstersdefeated = 0;

            DamagePanel = new Panel();
            DamagePanel.SizeMode = SizeMode.Fill;
            Color tr = Microsoft.Xna.Framework.Color.Red;
            tr.A = damagePnlMaxAlpha;
            DamagePanel.Background = new MonoGameColor(tr);
            DamagePanel.Visibility = CoreUI.Visibility.Hidden;
            game.UIEngine.Children.AddElement(DamagePanel);

            ActionLabelBack = new Panel();
            ActionLabelBack.Size = new System.Drawing.Point(game.currrentDisplayMode.Width, 100);
            ActionLabelBack.Position = new System.Drawing.Point(0, game.currrentDisplayMode.Height / 2 - 50);
            Color trans = Microsoft.Xna.Framework.Color.Black;
            trans.A = 200;
            ActionLabelBack.Background = new MonoGameColor(trans);
            ActionLabelBack.Visibility = CoreUI.Visibility.Hidden;
            game.UIEngine.Children.AddElement(ActionLabelBack);

            ActionLabel = new Label();
            ActionLabel.mFontInt = new MonoGameFont(game.resourceManager.GetResource<SpriteFont>("Round"));
            ActionLabel.Size = new System.Drawing.Point(game.currrentDisplayMode.Width, 100);
            ActionLabel.Position = new System.Drawing.Point(0, game.currrentDisplayMode.Height / 2 - 50);
            ActionLabel.Foreground = new MonoGameColor(Microsoft.Xna.Framework.Color.Red);
            ActionLabel.TextPosition = TextPosition.Center;
            ActionLabel.Visibility = CoreUI.Visibility.Visible;
            ActionLabelBack.AddElement(ActionLabel);

            HitLabel = new Label();
            SpriteFont f = game.resourceManager.GetResource<SpriteFont>("Hits");
            HitLabel.mFontInt = new MonoGameFont(f);
            HitLabel.Size = new System.Drawing.Point(200, 50);
            HitLabel.Position = new System.Drawing.Point(50, 100);
            HitLabel.Foreground = new MonoGameColor(Microsoft.Xna.Framework.Color.White);
            HitLabel.TextPosition = TextPosition.Left;
            HitLabel.Visibility = CoreUI.Visibility.Hidden;
            game.UIEngine.Children.AddElement(HitLabel);

            game.baseContext.RegisterHandler(Microsoft.Xna.Framework.Input.Keys.OemTilde, CheatEndRound, null);
        }

        private void CheatEndRound(Microsoft.Xna.Framework.Input.Keys key)
        {
            cheatEndRound = true;
        }

        public GameLogicSystem(Dirty game, Renderer r, CoreUIEngine UIEngine)
            : base(SystemDescriptions.GameLogicSystem.Aspect, SystemDescriptions.GameLogicSystem.Priority)
        {
            this.game = game;
            pbDisplay = new Panel();
            mPBs = new List<ProgressBar>();
            this.UIEngine = UIEngine;
            renderer = r;
            UIEngine.Children.AddElement(pbDisplay);
        }
    }
}
