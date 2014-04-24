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
using GameService;
using System.Threading;


namespace CleanGame.Game.Core.Systems
{
    enum GameLogicState
    {
        EndingRound,
        RoundResults,
        PreRound,
        ActiveRound
    };
    public class GameLogicSystem : EntitySystem
    {
        private const float damagePnlMaxTime = .5f;
        private const byte damagePnlMaxAlpha = 128;

        //Round Result Window
        private Window RoundWindow;
        private Panel RWPanel;
        private Label RWWinLbl;
        private Label RWHealthLbl;
        private Label RWKillsLbl;
        private Label RWAccuracyLbl;
        private Label RWSkillLbl;
        private Label RWRankLbl;
        private Button RWContinueBtn;

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
        private float saveChangesTime;
        private bool cheatEndRound = false;
        private int PlayerHits;
        private int PlayerHitsMax;
        private float playerHitTime;
        private bool tutorialMode;
        private List<Label> textFloaters = new List<Label>();
        private int ScenarioPtr = 0;
        private int oldELO = 0;
        private int oldRank = -1;
        private int ELO = 0;
        private int Rank = -1;

        private GameLogicState currentState;
        private Scenario currentScenario;
        private GameService.GameService.GameSession PreviousSession;
        private List<Entity> spawners = new List<Entity>();

        //Dictionary that contains a set of scenario objects
        // private Dictionary<string, Scenario> scenarios = new Dictionary<string, Scenario>();
        private Dictionary<int, Scenario> scenarios = new Dictionary<int, Scenario>();

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
            currentState = GameLogicState.ActiveRound;
            tutorialMode = true;
            game.player.GetComponent<PhysicsComponent>().movePlayer = true;
            game.player.GetComponent<SpatialComponent>().Position = new Vector2(200, 200);
            game.player.Refresh();
            GameplayDataCaptureSystem.Instance.LogEvent(CaptureEventType.ScenarioName, "Tutorial");
        }

        public void PlayerDealtDamage()
        {
            playerHitTime = 4;
            PlayerHits++;
            if (PlayerHits > PlayerHitsMax)
                PlayerHitsMax = PlayerHits;
            HitLabel.Visibility = Visibility.Visible;
            HitLabel.Text = PlayerHits + " hits";
        }
        public void PlayerTookDamage()
        {
            ResetHitCounter(false);
            (DamagePanel.Background as MonoGameColor).color.A = damagePnlMaxAlpha;
            DamagePanel.Visibility = Visibility.Visible;
            damagePnlTime = damagePnlMaxTime;
        }
        private void ResetHitCounter(bool timeout)
        {
            if (PlayerHits > 0)
            {
                GameplayDataCaptureSystem.Instance.LogEvent(CaptureEventType.ComboEndValue, PlayerHits.ToString());
                GameplayDataCaptureSystem.Instance.LogEvent(CaptureEventType.ComboEndReason, timeout ? "Timeout" : "Damage");
            }
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
        private void decodeScenarios(XmlReader scenarioReader, bool setCurrentScenario)
        {
            try
            {
                //Reads to the start of the XML file
                scenarioReader.ReadToFollowing("base");
                scenarioReader.ReadToDescendant("root");
                //scenarioReader.ReadStartElement();

                //TESTING
                //int scenarioCount = 0;
                //int spawnerCount = 0;

                //Parse the XML for the Scenarios
                int scenarioCount = 0;
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

                    foreach (Scenario s in scenarios.Values)
                    {
                        if (s.Name == scenarioName)
                        {
                            if (setCurrentScenario)
                                currentScenario = s;
                            goto SKIP;
                        }
                    }


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

                    //scenarios.Add(scenarioName, new Scenario(scenarioName, difficultyScore, mapName, spawners, playerSpawnPoint));
                    Scenario sc = new Scenario(scenarioName, difficultyScore, mapName, spawners, playerSpawnPoint);
                    if (setCurrentScenario)
                        currentScenario = sc;
                    scenarios.Add(scenarios.Count, sc);
                    scenarioCount++;
                //spawnerCount = 0;

                SKIP: ;
                }
            }
            catch (Exception e)
            {
                App.LogStack(e);
            }
        }
        private void decodeServerScenario(string XML)
        {
            currentScenario = null;

            //Setting up the XML reader
            XmlReaderSettings xmlSettings = new XmlReaderSettings();
            xmlSettings.IgnoreWhitespace = true;
            xmlSettings.IgnoreComments = true;
            XmlReader scenarioReader = XmlReader.Create(new System.IO.StringReader(XML), xmlSettings);

            decodeScenarios(scenarioReader, true);
        }
        //private int getELOFromXML(string XML)
        //{
        //    XmlReaderSettings xmlSettings = new XmlReaderSettings();
        //    xmlSettings.IgnoreWhitespace = true;
        //    xmlSettings.IgnoreComments = true;
        //    XmlReader eloReader = XmlReader.Create(new System.IO.StringReader(XML), xmlSettings);

        //    eloReader.ReadToFollowing("base");
        //    eloReader.ReadToDescendant("elo");
        //    return Convert.ToInt32(eloReader.GetAttribute("value"));
        //}

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

            decodeScenarios(scenarioReader, false);
        }

        /// <summary>
        /// Starts a specific scenario
        /// </summary>
        /// <param name="scenarioName"></param>
        /// <param name="player"></param>
        //public void setupScenario(string scenarioName)
        //{
        //    Scenario tempScenario = scenarios[scenarioName];
        //    setupScenario(tempScenario);
        //}
        /// <summary>
        /// Starts a specific scenario
        /// </summary>
        /// <param name="scenario"></param>
        public void setupScenario(Scenario scenario)
        {
            GameplayDataCaptureSystem.Instance.LogEvent(CaptureEventType.ScenarioName, scenario.Name);
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
            //Random random = new Random();
            //int randomScenario = random.Next(0, scenarios.Count);
            //int count = -1;
            Scenario playScenario;// = new Scenario();

            //Making sure that the scenario is made for the map
            //HardCode Scenarios start with 15 to 21
            //foreach (var scenario in scenarios.Values)
            //{
            //    //break out if the random number scenario for that map
            //    if (count == randomScenario)
            //    {
            //        break;
            //    }
            //    //found a map with the same name
            //    if (scenario.MapName.Equals(mapName))
            //    {
            //        playScenario = (Scenario)scenario;
            //        count++;
            //    }
            //}

            playScenario = scenarios[ScenarioPtr % scenarios.Count];
            ScenarioPtr++;


            return playScenario;
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

                    if (currentState == GameLogicState.ActiveRound)
                    {

                        monstersdefeated++;

                        AddTextFloater("+" + (50 + PlayerHits));
                        game.gameEntity.entity.GetComponent<PropertyComponent<int>>("GameScore").value += 50 + PlayerHits;
                        game.gameEntity.entity.GetComponent<PropertyComponent<int>>("GameCash").value += 10;
                        game.gameEntity.entity.GetComponent<PropertyComponent<int>>("GameKills").value += 1;

                        GameplayDataCaptureSystem.Instance.LogEvent(CaptureEventType.MonsterKilled, e.GetComponent<PropertyComponent<String>>("MonsterType").value);
                    }
                    //}
                    if (--monstersalive == 0 && !tutorialMode)
                    {
                        foreach (Entity sp in spawners)//make sure there aren't more monsters spawning
                            if (sp.GetComponent<SpawnerComponent>().numMobs != 0)
                                return;


                        GameplayDataCaptureSystem.Instance.LogEvent(CaptureEventType.RoundEnded, game.gameEntity.entity.GetComponent<PropertyComponent<int>>("GameRound").value.ToString());
                        GameplayDataCaptureSystem.Instance.LogEvent(CaptureEventType.RoundScore, game.gameEntity.entity.GetComponent<PropertyComponent<int>>("GameScore").value.ToString());
                        GameplayDataCaptureSystem.Instance.LogEvent(CaptureEventType.RoundHealth, game.player.GetComponent<StatsComponent>().CurrentHealth.ToString());


                        if (ScenarioPtr == 27)
                        {



                            Event gamestate = new Event();
                            gamestate.name = "GameStateGameOver";
                            EventManager.Instance.TriggerEvent(gamestate);

                        }

                        StartNewRound();
                    }
                }
            }

        }
        /// <summary>
        /// Ends a round, sends data to server, then starts a new round. Data send is async
        /// </summary>
        public void StartNewRound()
        {
            currentState = GameLogicState.EndingRound;
            resetRound();

            //start new session and wait for result
            GameplayDataCaptureSystem.Instance.NewSessionResultEvent += Instance_NewSessionResultEvent;
            GameplayDataCaptureSystem.Instance.DataRetryEvent += Instance_DataRetryEvent;
            GameplayDataCaptureSystem.Instance.ELORankEvent += Instance_ELORankEvent;
            GameplayDataCaptureSystem.Instance.ScenarioXMLEvent += Instance_ScenarioXMLEvent;
            GameplayDataCaptureSystem.Instance.NewSessionAsync();

            ActionLabel.Text = "Contacting Server...";
            ActionLabel.Position = new System.Drawing.Point(0, game.currrentDisplayMode.Height / 2 - 50);
            ActionLabelBack.Visibility = Visibility.Visible;
        }

        void Instance_ELORankEvent(string ELORank)
        {
            GameplayDataCaptureSystem.Instance.ELORankEvent -= Instance_ELORankEvent;
            if (ELORank == null || ELORank == "")
            {
                MessageBox.Show("Failed to retrieve rankings from server.\nPlease check your internet settings.", "Error");
            }
            else
            {
                oldELO = ELO;
                oldRank = Rank;

                ELO = int.Parse(ELORank.Substring(0, ELORank.IndexOf(",")));
                Rank = int.Parse(ELORank.Substring(ELORank.IndexOf(",")+1));

                if (oldELO <= 0 && ELO > 0)
                {
                    oldELO = ELO;
                }
                if (oldRank <= 0 && Rank > 0)
                {
                    oldRank = Rank;
                }
            }
        }

        void Instance_ScenarioXMLEvent(string XML)
        {
            ActionLabelBack.Visibility = Visibility.Hidden;
            GameplayDataCaptureSystem.Instance.ScenarioXMLEvent -= Instance_ScenarioXMLEvent;
            if (XML == null || XML == "")
            {
                MessageBox.Show("Failed to retrieve scenario from server.\nPlease check your internet settings.", "Error");
            }
            else
            {
                decodeServerScenario(XML);
                
            }

            if (PreviousSession != null)
            {
                ShowRoundResults(PreviousSession);
            }
            else
            {
                StartPreRound();
            }
        }

        void Instance_DataRetryEvent(object sender, RetryEventArgs e)
        {
            ActionLabel.Text = "Retry Attempt " + e.Attempt + "...";
        }

        void Instance_NewSessionResultEvent(object sender, SessionEventArgs e)
        {
            GameplayDataCaptureSystem.Instance.NewSessionResultEvent -= Instance_NewSessionResultEvent;
            GameplayDataCaptureSystem.Instance.DataRetryEvent -= Instance_DataRetryEvent;


            if (currentState == GameLogicState.EndingRound)
            {
                if (!e.RequestsSucceeded)
                    MessageBox.Show("Failed to contact server. Please\ncheck your internet settings.", "Error");
                PreviousSession = e.PreviousSession;
                //GameplayDataCaptureSystem.Instance.GetELORankAsync();
                GameplayDataCaptureSystem.Instance.GetScenarioAsync();
                
            }
        }
        /// <summary>
        /// Shows the round label and sets up a timer to the next round
        /// </summary>
        public void StartPreRound()
        {
            currentState = GameLogicState.PreRound;
            //Log
            GameplayDataCaptureSystem.Instance.LogEvent(CaptureEventType.MapSelected, game.mapName);

            //next game round
            //AdvanceLevel();
            PlayerHitsMax = 0;
            game.ClearField = true;
            //start next round in 5
            roundStartTime = 5f;
            ActionLabel.Text = "Get Ready!";
            //ActionLabel.Text = "Get Ready!";
            ActionLabel.Position = new System.Drawing.Point(-ActionLabel.Size.X / 2, ActionLabel.Position.Y);
            ActionLabelBack.Visibility = CoreUI.Visibility.Visible;
        }
        private void ShowRoundResults(GameService.GameService.GameSession PrevSession)
        {
            currentState = GameLogicState.RoundResults;
            roundStartTime = 10; 

            if (game.player.GetComponent<StatsComponent>().CurrentHealth > 0)
            {
                RWWinLbl.Text = "Victory!";
                RWWinLbl.Foreground = new MonoGameColor(Color.Green);
            }
            else
            {
                RWWinLbl.Text = "Defeat";
                RWWinLbl.Foreground = new MonoGameColor(Color.Red);
            }

            RWHealthLbl.Text = string.Format("Health Remaining: {0}%", (int)Math.Round(game.player.GetComponent<StatsComponent>().CurrentHealth / game.player.GetComponent<StatsComponent>().MaxHealth * 100));
            RWAccuracyLbl.Text = string.Format("Largest Combo: {0} hits", PlayerHitsMax);
            RWKillsLbl.Text = string.Format("Monsters Killed: {0}%", (int)Math.Round((PrevSession.KillRate) * 100));
            RWSkillLbl.Text = string.Format("Skill: {0} ({1}{2})", ELO, (oldELO > ELO) ? "-" : "+", Math.Abs(ELO - oldELO));
            RWRankLbl.Text = string.Format("Rank: {0} ({1}{2})", Rank, (oldRank < Rank) ? "-" : "+", Math.Abs(Rank - oldRank));

            RoundWindow.Show();
        }
        public override void ProcessEntities(IEnumerable<Entity> entities, float dt)
        {
            if (currentState == GameLogicState.RoundResults)
            {
                RWContinueBtn.Text = string.Format("Continue...({0})", (int)Math.Ceiling(roundStartTime));
                roundStartTime -= dt;
                if (roundStartTime <= 0)
                {
                    roundStartTime = 0;

                    RoundWindow.Hide();
                    StartPreRound();
                }
            }
            else if (currentState == GameLogicState.PreRound)
            {
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

                        //Setting the movePlayer flag in the physics component of the player
                        game.player.GetComponent<PhysicsComponent>().movePlayer = true;

                        //restore player health
                        game.player.GetComponent<StatsComponent>().CurrentHealth = game.player.GetComponent<StatsComponent>().MaxHealth;

                        if (currentScenario == null)
                            setupScenario(randomScenario(game.mapName));
                        else
                            setupScenario(currentScenario);
                        game.player.Refresh();
                        currentState = GameLogicState.ActiveRound;

                    }
                }
            }
            else if (currentState == GameLogicState.ActiveRound)
            {
                if (saveChangesTime >= 0)
                {
                    saveChangesTime -= dt;
                    if (saveChangesTime <= 0)
                    {
                        saveChangesTime = 5;
                        GameplayDataCaptureSystem.Instance.SaveChangesAsync();
                    }
                }

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
                        ResetHitCounter(true);
                    }
                }


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
                    List<Entity> towers = new List<Entity>();
                    for (int i = 0; i < entities.Count(); i++)
                    {
                        Entity e = entities.ElementAt(i);

                        StatsComponent hc = e.GetComponent<StatsComponent>();
                        

                        if (hc.CurrentHealth <= 0)//dead
                        {
                            if (e.HasComponent<PlayerComponent>())//player died
                            {

                                GameplayDataCaptureSystem.Instance.LogEvent(CaptureEventType.PlayerDied, "");
                                currentState = GameLogicState.EndingRound;

                                for (int j = 0; j < entities.Count(); j++)
                                {
                                    Entity entity = entities.ElementAt(j);
                                    if (entity.HasComponent<MonsterComponent>())
                                    {
                                        World.DestroyEntity(entity);
                                        j--;
                                    }
                                }



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

                            //detect towers
                            PropertyComponent<String> mc = e.GetComponent<PropertyComponent<String>>("MonsterType");
                            if (mc != null && mc.value == "Flametower")
                                towers.Add(e);
                        }

                    }

                    if (towers.Count > 0 && towers.Count == entities.Count() - 1)//make towers vuln since monsters are dead
                    {
                        foreach (Entity e in towers)
                        {
                            StatsComponent s = e.GetComponent<StatsComponent>();
                            if (s.ImmuneTo.Contains("Longsword"))
                            {
                                s.ImmuneTo.Remove("Longsword");
                                SpriteComponent sc = e.GetComponent<SpriteComponent>();
                                sc.spriteName = sc.spriteName.Replace("Shield", "");
                                sc.SpriteSheet = game.resourceManager.GetResource<SpriteSheet>(sc.spriteName);
                                if (sc.SpriteSheet == null)
                                {
                                    sc.SpriteSheet = new SpriteSheet(game.resourceManager.GetResource<Texture2D>(sc.spriteName), sc.spriteName, sc.xmlName);
                                    game.resourceManager.AddResource<SpriteSheet>(sc.SpriteSheet, sc.spriteName);
                                }
                            }
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

            RoundWindow = new Window();
            RoundWindow.Style = Window.WindowStyle.None;
            RoundWindow.Size = new System.Drawing.Point(300, 375);
            RoundWindow.Position = new System.Drawing.Point(game.currrentDisplayMode.Width / 2 - 150, game.currrentDisplayMode.Height / 2 - 150);
            RoundWindow.Background = new MonoGameColor(trans);

            RWPanel = new Panel();
            RoundWindow.Content = RWPanel;

            RWWinLbl = new Label();
            RWWinLbl.Size = new System.Drawing.Point(300, 50);
            RWWinLbl.Position = new System.Drawing.Point(0, 0);
            RWWinLbl.TextPosition = TextPosition.Center;
            RWWinLbl.mFontInt = new MonoGameFont(game.resourceManager.GetResource<SpriteFont>("Round"));
            RWPanel.AddElement(RWWinLbl);

            RWHealthLbl = new Label();
            RWHealthLbl.Size = new System.Drawing.Point(300, 50);
            RWHealthLbl.Position = new System.Drawing.Point(0, 75);
            RWHealthLbl.TextPosition = TextPosition.Center;
            RWHealthLbl.Foreground = new MonoGameColor(Microsoft.Xna.Framework.Color.White);
            RWHealthLbl.mFontInt = new MonoGameFont(game.resourceManager.GetResource<SpriteFont>("Message"));
            RWPanel.AddElement(RWHealthLbl);

            RWKillsLbl = new Label();
            RWKillsLbl.Size = new System.Drawing.Point(300, 50);
            RWKillsLbl.Position = new System.Drawing.Point(0, 125);
            RWKillsLbl.TextPosition = TextPosition.Center;
            RWKillsLbl.Foreground = new MonoGameColor(Microsoft.Xna.Framework.Color.White);
            RWKillsLbl.mFontInt = new MonoGameFont(game.resourceManager.GetResource<SpriteFont>("Message"));
            RWPanel.AddElement(RWKillsLbl);

            RWAccuracyLbl = new Label();
            RWAccuracyLbl.Size = new System.Drawing.Point(300, 50);
            RWAccuracyLbl.Position = new System.Drawing.Point(0, 175);
            RWAccuracyLbl.TextPosition = TextPosition.Center;
            RWAccuracyLbl.Foreground = new MonoGameColor(Microsoft.Xna.Framework.Color.White);
            RWAccuracyLbl.mFontInt = new MonoGameFont(game.resourceManager.GetResource<SpriteFont>("Message"));
            RWPanel.AddElement(RWAccuracyLbl);

            RWSkillLbl = new Label();
            RWSkillLbl.Size = new System.Drawing.Point(300, 50);
            RWSkillLbl.Position = new System.Drawing.Point(0, 225);
            RWSkillLbl.TextPosition = TextPosition.Center;
            RWSkillLbl.Foreground = new MonoGameColor(Microsoft.Xna.Framework.Color.White);
            RWSkillLbl.mFontInt = new MonoGameFont(game.resourceManager.GetResource<SpriteFont>("Message"));
            RWPanel.AddElement(RWSkillLbl);

            RWRankLbl = new Label();
            RWRankLbl.Size = new System.Drawing.Point(300, 50);
            RWRankLbl.Position = new System.Drawing.Point(0, 250);
            RWRankLbl.TextPosition = TextPosition.Center;
            RWRankLbl.Foreground = new MonoGameColor(Microsoft.Xna.Framework.Color.White);
            RWRankLbl.mFontInt = new MonoGameFont(game.resourceManager.GetResource<SpriteFont>("Message"));
            RWPanel.AddElement(RWRankLbl);

            RWContinueBtn = new Button();
            RWContinueBtn.Size = new System.Drawing.Point(200, 30);
            RWContinueBtn.Position = new System.Drawing.Point(50, 325);
            RWContinueBtn.Foreground = new MonoGameColor(Microsoft.Xna.Framework.Color.White);
            RWContinueBtn.mFontInt = new MonoGameFont(game.resourceManager.GetResource<SpriteFont>("Message"));
            RWContinueBtn.Text = "Continue";
            RWContinueBtn.Click += RWContinueBtn_Click;
            RWPanel.AddElement(RWContinueBtn);

#if DEBUG
            game.baseContext.RegisterHandler(Microsoft.Xna.Framework.Input.Keys.OemTilde, CheatEndRound, null);
#endif
        }

        void RWContinueBtn_Click(object sender)
        {
            if (currentState == GameLogicState.RoundResults)
            {
                RoundWindow.Hide();
                StartPreRound();
            }
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
