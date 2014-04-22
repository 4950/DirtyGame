#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using CleanGame.Game.Core;
using CleanGame.Game.Core.Systems;
using CleanGame.Game.Core.Systems.Render;
using CleanGame.Game.SGraphics;
using CleanGame.Game.Systems;
using CleanGame.Game.Map;
using EntityFramework;
using EntityFramework.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.GamerServices;
using EntityFramework.Systems;
using CleanGame.Game.Core.Systems.Monster;
using CleanGame.Game.Core.GameStates;
using CoreUI;
using CoreUI.DrawEngines;
using CleanGame.Game.Input;
using CleanGame.Game.Util;
using CoreUI;
using CoreUI.DrawEngines;
using CleanGame.Game.Input;

using CleanGame.Game.Core.Components;
using FarseerPhysics.Dynamics;
using CleanGame.Game.Core.Systems.Movement;
using CleanGame.Game.Core.Components.Render;
using CleanGame.Game.Core.Components.Movement;
using CleanGame.Game.Core.Events;
using GameService;



#endregion

namespace CleanGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Dirty : Microsoft.Xna.Framework.Game
    {
        private Map map;
        public Physics physics;
        public EntityFramework.World world;
        public GraphicsDeviceManager graphics;
        public Renderer renderer;
        public EntityFactory entityFactory;
        public ResourceManager resourceManager;
        public GameStateManager gameStateManager;
        public AISystem aiSystem;
        private readonly int MAX_MONSTERS = 20;
        public CoreUIEngine UIEngine;
        public GameLogicSystem gLogicSystem;
        public MonoGameDrawEngine UIDraw;
        public InputManager inputManager;
        public InputContext baseContext;
        public Entity player;
        public WeaponSystem weaponSystem;
        public EntityRef gameEntity;
        public MouseState mouseState;
        private DisplayMode defaultDisplayMode;
        public DisplayMode currrentDisplayMode;
        public string mapName;

        private void Exit(Keys key)
        {
            Exit();
        }
        /// <summary>
        /// This is to be used for generating xml from entites for testing purposes
        /// </summary>
        private void SaveTestEntites()
        {
            //Entity monsterWeapon = world.EntityMgr.GetEntityByName("FlametowerWeapon");
            //MonsterData meleeData = MonsterData.RangedMonster;
            //meleeData.weapon = monsterWeapon;
            //meleeData.Health += (int)(meleeData.Health * (1 / 5f));
            //Entity monster = entityFactory.CreateBasicMonster(new Vector2(), "Flametower", "Content\\Flametower.xml", meleeData);
            //monster.Name = "Flametower";

            //Entity e = entityFactory.CreateMeleeWeaponEntity("Basic Sword", "sword", 50, 25, 50, 1f, 100, 2, "SwordMeleeSpriteSheet", "Content\\MeleeAnimation.xml");
            //e.Name = "BasicSword";
            //e.Refresh();
            //e = entityFactory.CreateRangedWeaponEntity("Doomsbow", "bow", "bow", 400, 25, 10, "arrow", -1, 1f, 100, 0);
            //e.Name = "Doomsbow";
            //e.Refresh();
            //e = entityFactory.CreateRangedWeaponEntity("Spear", "spear", "spear", 200, 35, 5, "spear", 5, 2f, 150, 10);
            //e.Name = "Spear";
            //e.Refresh();
            //e = entityFactory.CreateRangedWeaponEntity("Scattershot", "bow", "bow", 400, 25, 10, "arrow", 10, 1f, 100, 10);
            //e.Name = "Scattershot";
            //e.Refresh();
            //e = entityFactory.CreateRangedWeaponEntity("Sniper", "bow", "bow", 600, 100, 30, "arrow", 10, 4f, 100, 10);
            //e.Name = "Sniper";
            //e.Refresh();
            //Entity flametowerWeapon = entityFactory.CreateRangedWeaponEntity("FlametowerWeapon", "bow", "bow", 150, 5, 30, "Flames", -1, 3f, 100, 10);
            //flametowerWeapon.Name = "FlametowerWeapon";
            //flametowerWeapon.Refresh();
            //Entity monsterMelee = entityFactory.CreateMeleeWeaponEntity("Monstersword", "sword", 50, 15 + 15 * (1 / 5f), -1, 2f, 100, 0, "SwordMeleeSpriteSheet", "Content\\MeleeAnimation.xml");
            //monsterMelee.Name = "Monstersword";
            //monsterMelee.Refresh();
            //Entity monsterWeapon = world.EntityMgr.GetEntityByName("Monsterbow"); /*entityFactory.CreateRangedWeaponEntity("Monsterbow", "bow", "bow", 400, 20 + 20 * (1 / 5f), 10, "arrow", -1, 3f, 100, 0);
            //monsterWeapon.Name = "Monsterbow";
            //monsterWeapon.Refresh();*/
            //MonsterData rangedData = MonsterData.RangedMonster;
            //rangedData.weapon = monsterWeapon;
            //rangedData.Health += (int)(rangedData.Health * (1 / 5f));

            //Entity e = entityFactory.CreateBasicMonster(new Vector2(), "playerSheet", "Content\\PlayerAnimation.xml", rangedData);
            //e.Refresh();

            world.EntityMgr.SerializeEntities(App.Path + "test.xml");
        }
        public void ResetToMainMenu()
        {
            //clear old stuff
            GameplayDataCaptureSystem.Instance.EndSession();
            world.RemoveAllSystems();
            world.EntityMgr.RemoveAllEntities();
            baseContext.RemoveAllHandlers();
            inputManager.RemoveAllContexts();
            UIEngine.Children.RemoveAllItems();
            map.ClearMap();

            //load new stuff
            physics = new Physics(world.EntityMgr);
            CreateInputContext();

            Event endGame = new Event();
            endGame.name = "GameStateMainMenu";
            EventManager.Instance.TriggerEvent(endGame);

            SoundSystem.Instance.Loop = true;
            SoundSystem.Instance.PlayBackgroundMusic("DST-ChordLesson01.mp3");
        }
        private void LoadSystems()
        {
            aiSystem = new AISystem(this, entityFactory, physics, renderer);
            world.AddSystem(aiSystem);
            world.AddSystem(new SpriteRenderSystem(renderer));
            world.AddSystem(new PlayerControlSystem(entityFactory, renderer, this));
            weaponSystem = new WeaponSystem(this);
            world.AddSystem(weaponSystem);
            world.AddSystem(new CameraUpdateSystem(renderer));
            world.AddSystem(new SpawnerSystem(entityFactory, this));
            world.AddSystem(new ProjectileSystem(this));
            world.AddSystem(new GrenadeSystem(this));
            gLogicSystem = new GameLogicSystem(this, renderer, UIEngine);
            world.AddSystem(gLogicSystem);
            world.AddSystem(new PhysicsSystem(physics, renderer, this));
            world.AddSystem(new AnimationSystem(this));
            world.AddSystem(new MovementSystem(aiSystem));
            world.AddSystem(new AOESystem(this));
            world.AddSystem(new SeparationSystem());
            world.AddSystem(new MeleeSystem(this));
            world.AddSystem(new PropertySystem());
        }
        private void InitUI()
        {
            //init UI
            UIDraw = new MonoGameDrawEngine(GraphicsDevice, Content);
            UIEngine = new CoreUIEngine(UIDraw, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);
            SpriteFont defaultFont = resourceManager.GetResource<SpriteFont>("MedSharp");
            UIDraw.setDefaultFont(defaultFont);
        }
        private void AddComponentTypes()
        {
            //add component types to list
            Component.ComponentTypes.Add(typeof(PropertyComponent<int>));
            Component.ComponentTypes.Add(typeof(PropertyComponent<string>));
            Component.ComponentTypes.Add(typeof(WeaponComponent));
            Component.ComponentTypes.Add(typeof(MonsterComponent));
            Component.ComponentTypes.Add(typeof(StatsComponent));
            Component.ComponentTypes.Add(typeof(MovementComponent));
            Component.ComponentTypes.Add(typeof(AnimationComponent));
            Component.ComponentTypes.Add(typeof(SpatialComponent));
            Component.ComponentTypes.Add(typeof(SpriteComponent));
            Component.ComponentTypes.Add(typeof(TimeComponent));
            Component.ComponentTypes.Add(typeof(PhysicsComponent));
            Component.ComponentTypes.Add(typeof(DirectionComponent));
            Component.ComponentTypes.Add(typeof(SeparationComponent));
            Component.ComponentTypes.Add(typeof(InventoryComponent));
            Component.ComponentTypes.Add(typeof(SnipComponent));
        }
        private void SetupPlayer()
        {
            player = entityFactory.CreatePlayerEntity();
            player.Name = "Player";
            //player.Refresh();

            //weapons
            Entity e = entityFactory.CloneEntity(world.EntityMgr.GetEntityByName("BasicSword"));
            e.Refresh();
            player.GetComponent<InventoryComponent>().addWeapon(e, player);
            e = entityFactory.CloneEntity(world.EntityMgr.GetEntityByName("Doomsbow"));
            e.Refresh();
            player.GetComponent<InventoryComponent>().addWeapon(e, player);
            e = entityFactory.CloneEntity(world.EntityMgr.GetEntityByName("Spear"));
            e.Refresh();
            player.GetComponent<InventoryComponent>().addWeapon(e, player);
            e = entityFactory.CloneEntity(world.EntityMgr.GetEntityByName("Scattershot"));
            e.Refresh();
            player.GetComponent<InventoryComponent>().addWeapon(e, player);
            e = entityFactory.CloneEntity(world.EntityMgr.GetEntityByName("Sniper"));
            e.Refresh();
            player.GetComponent<InventoryComponent>().addWeapon(e, player);
        }
        private void InitSound()
        {
            //start sound
            SoundSystem.Instance.SetGame(this);
            SoundSystem.Instance.MaxVolume = .20f;
            SoundSystem.Instance.AddBackgroundMusic("DST-BreakOut.mp3");
            SoundSystem.Instance.AddBackgroundMusic("DST-ClubFight.mp3");
            SoundSystem.Instance.AddBackgroundMusic("DST-DasElectron.mp3");
            SoundSystem.Instance.AddBackgroundMusic("DST-DawnRise.mp3");
            SoundSystem.Instance.AddBackgroundMusic("DST-DaysOfRealms.mp3");
            SoundSystem.Instance.AddBackgroundMusic("DST-KiloByte.mp3");
            SoundSystem.Instance.AddBackgroundMusic("DST-MushroomRoad.mp3");

            SoundSystem.Instance.Loop = true;
            SoundSystem.Instance.PlayBackgroundMusic("DST-ChordLesson01.mp3");
        }
        public void LoadXMLBase()
        {
            //load serialized entities
            world.EntityMgr.DeserializeEntities(App.Path + "Main.xml");
            world.EntityMgr.DeserializeEntities(App.Path + "Monsters.xml");
            gameEntity = world.EntityMgr.GetEntityByName("Game").reference;
        }
        public void CreateInputContext()
        {
            inputManager = InputManager.Instance;
            baseContext = new InputContext();
            inputManager.AddInputContext(baseContext);
            //baseContext.RegisterHandler(Keys.Escape, Exit, null);
        }
        public void ToggleFullscreen()
        {
            if (graphics.IsFullScreen == false)
            {
                graphics.IsFullScreen = true;
                graphics.ApplyChanges();
            }
            else
            {
                graphics.ToggleFullScreen();
                /*
                graphics.PreferredBackBufferWidth = defaultDisplayMode.Width;
                graphics.PreferredBackBufferHeight = defaultDisplayMode.Height;
                graphics.ApplyChanges();
                graphics.IsFullScreen = false;
                graphics.PreferredBackBufferWidth = currrentDisplayMode.Width;
                graphics.PreferredBackBufferHeight = currrentDisplayMode.Height;
                graphics.ApplyChanges();*/
            }
        }
        public Dirty()
        {
            GameplayDataCaptureSystem.Instance.InitContext(App.PublishVersion);
            GameplayDataCaptureSystem.Instance.Login();
            Settings.Instance.LoadSettings();

            //Check version against server
            if (App.PublishVersion != "Develop")
            {
                string liveVersion = GameplayDataCaptureSystem.Instance.GetLiveGameVersion();
                if (liveVersion != "" && liveVersion != App.PublishVersion)
                {
                    System.Windows.Forms.MessageBox.Show("An update is available. Please visit https://toweroffense.azurewebsites.com to download.\nGame will now exit.\n\nCurrent Version: " + App.PublishVersion + "\nLatest Version: " + liveVersion, "Update Required");
                    this.Exit();
                }
            }

            CreateInputContext();

            graphics = new GraphicsDeviceManager(this);
            graphics.PreferMultiSampling = true;

            defaultDisplayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;

            DisplayMode smallest = null;
            DisplayMode sm86 = null;
            Vector2 preferred = Settings.Instance.Global.Resolution;

            //for some god damn reason I can't check (smallest == null) so these stupid bools must be used
            bool smNull = true;
            bool sm86Null = true;
            foreach (DisplayMode d in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes[graphics.PreferredBackBufferFormat])
            {
                if (smNull)
                {
                    smallest = d;
                    smNull = false;
                }
                if (d.Width == preferred.X && d.Height == preferred.Y)
                    if (sm86Null)
                    {
                        sm86 = d;
                        sm86Null = false;
                    }
                if (smallest.Width >= d.Width || smallest.Height >= d.Height)
                        smallest = d;
            }

            if (!sm86Null)
                currrentDisplayMode = sm86;
            else if (!smNull)
                currrentDisplayMode = smallest;
            else
                currrentDisplayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;

            Settings.Instance.Global.Resolution = new Vector2(currrentDisplayMode.Width, currrentDisplayMode.Height);

            graphics.PreferredBackBufferWidth = currrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = currrentDisplayMode.Height;
            graphics.IsFullScreen = Settings.Instance.Global.Fullscreen;
            graphics.ApplyChanges();

            IsMouseVisible = true;
            Window.Title = "Tower Offense";
        }
        public void StartTutorial()
        {
            world.AddSystem(new TutorialSystem(this));
            StartSession("Arena.tmx");
            gLogicSystem.SetupTutorial();
        }
        public void StartSession(string mapname)
        {
            LoadSystems();
            LoadXMLBase();
            SetupPlayer();

            LoadMap(mapname);

            //Loading in the different scenarios from the Scenarios.XML
            gLogicSystem.decodeScenariosXML("Content\\Scenarios.xml");

            //Setting up a scenario
            //TODO: Needs to change to be a selection
//            gLogicSystem.setupScenario("scenario1");
            LoadScenario(mapname);

            SoundSystem.Instance.Loop = false;
            SoundSystem.Instance.RemoveBackgroundMusic("DST-ChordLesson01.mp3");
            SoundSystem.Instance.PlayRand();

            CleanGame.Game.Core.Events.Event startGame = new CleanGame.Game.Core.Events.Event();
            startGame.name = "GameStateGame";
            EventManager.Instance.TriggerEvent(startGame);
        }
        public void LoadScenario(string mapName)
        {
            //Setting up the scenario for the map
            gLogicSystem.StartNewRound();
            //Scenario playingScenario = gLogicSystem.randomScenario(mapName); //This will change to gLogicSystem.scenarioForPlayerScore(string mapName, float playerScore)
            //gLogicSystem.setupScenario(playingScenario);
            //player.Refresh();
        }
        private void LoadMap(string mapname)
        {
            

            map.LoadMap(mapname, graphics.GraphicsDevice, Content);
            renderer.ActiveMap = map;

            mapName = mapname;

            //Need to be moved
            Entity wall = entityFactory.CreateWallEntity(new Vector2(0f, 0f), new Vector2(0f, renderer.ActiveMap.getPixelHeight()),
                            new Vector2(renderer.ActiveMap.getPixelWidth(), 0f), new Vector2(renderer.ActiveMap.getPixelWidth(), renderer.ActiveMap.getPixelHeight()));
            wall.Refresh();
        }
        protected override void OnDeactivated(object sender, EventArgs args)
        {
            base.OnDeactivated(sender, args);

            if (gameStateManager != null && gameStateManager.CurrentState.GetType() == typeof(GamePlay))//if in game, pause
            {
                Event startGame = new Event();
                startGame.name = "GameStateGameMenu";
                EventManager.Instance.TriggerEvent(startGame);
            }
        }
        protected override void LoadContent()
        {

            renderer = new Renderer(graphics, new Camera(new Vector2(currrentDisplayMode.Width, currrentDisplayMode.Height)));

            resourceManager = new ResourceManager(Content);

            world = new EntityFramework.World();
            physics = new Physics(world.EntityMgr);

            entityFactory = new EntityFactory(world.EntityMgr, resourceManager);
            map = new Map(graphics.GraphicsDevice, entityFactory);

            InitUI();

            gameStateManager = new GameStateManager(this);

            AddComponentTypes();

            InitSound();
        }

        protected override void UnloadContent()
        {
            GameplayDataCaptureSystem.Instance.EndSession();
            Settings.Instance.SaveSettings();
        }
        public bool GameWon
        {
            get;
            set;
        }
        public bool ClearField = false; //Ling's added variable to clear landmines
       

        protected override void Update(GameTime gameTime)
        {
            inputManager.DispatchInput();


            gameStateManager.CurrentState.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            mouseState = Mouse.GetState();
            UIEngine.GetInput(mouseState.X, mouseState.Y, mouseState.LeftButton == ButtonState.Pressed, mouseState.RightButton == ButtonState.Pressed, mouseState.MiddleButton == ButtonState.Pressed);


            SoundSystem.Instance.Update();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (!GraphicsDevice.IsDisposed)
            {
                UIDraw.SetDevice(GraphicsDevice);
                UIEngine.Update((float)gameTime.ElapsedGameTime.TotalMilliseconds);
                renderer.Render();
                UIEngine.Render();
                base.Draw(gameTime);
            }
        }
    }
}
