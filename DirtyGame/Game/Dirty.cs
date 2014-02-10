#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using DirtyGame.game.Core;
using DirtyGame.game.Core.Systems;
using DirtyGame.game.Core.Systems.Render;
using DirtyGame.game.SGraphics;
using DirtyGame.game.Systems;
using DirtyGame.game.Map;
using EntityFramework;
using EntityFramework.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using EntityFramework.Systems;
using DirtyGame.game.Core.Systems.Monster;
using DirtyGame.game.Core.GameStates;
using CoreUI;
using CoreUI.DrawEngines;
using DirtyGame.game.Input;
using DirtyGame.game.Util;
using CoreUI;
using CoreUI.DrawEngines;
using DirtyGame.game.Input;

using DirtyGame.game.Core.Components;
using FarseerPhysics.Dynamics;
using DirtyGame.game.Core.Systems.Movement;
using DirtyGame.game.Core.Components.Render;
using DirtyGame.game.Core.Components.Movement;



#endregion

namespace DirtyGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Dirty : Game
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
        public Dirty()
        {
            GameplayDataCaptureSystem.Instance.CreateSession(true);

            inputManager = InputManager.Instance;
            baseContext = new InputContext();
            inputManager.AddInputContext(baseContext);
            baseContext.RegisterHandler(Keys.Escape, Exit, null);

            graphics = new GraphicsDeviceManager(this);

            resourceManager = new ResourceManager(Content);
            //init UI
            UIDraw = new MonoGameDrawEngine(graphics.GraphicsDevice, Content);
            UIEngine = new CoreUIEngine(UIDraw, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);
            SpriteFont defaultFont = resourceManager.GetResource<SpriteFont>("default");
            UIDraw.setDefaultFont(defaultFont);
            resourceManager = new ResourceManager(Content);
            renderer = new Renderer(graphics, new Camera(new Vector2(800, 600)));
            world = new EntityFramework.World();
            physics = new Physics(world.EntityMgr);

            gameStateManager = new GameStateManager(this);
            entityFactory = new EntityFactory(world.EntityMgr, resourceManager);
            aiSystem = new AISystem(this, entityFactory, physics);
            world.AddSystem(aiSystem);
            world.AddSystem(new SpriteRenderSystem(renderer));
            world.AddSystem(new PlayerControlSystem(entityFactory, renderer, this));
            weaponSystem = new WeaponSystem(this);
            world.AddSystem(weaponSystem);
            world.AddSystem(new CameraUpdateSystem(renderer));
            world.AddSystem(new MapBoundarySystem(renderer));
            world.AddSystem(new SpawnerSystem(entityFactory, this));
            world.AddSystem(new HUDSystem(renderer, UIEngine));
            world.AddSystem(new ProjectileSystem(this));
            gLogicSystem = new GameLogicSystem(this);

            world.AddSystem(new PhysicsSystem(physics, renderer, this));
            world.AddSystem(new AnimationSystem(this));
            world.AddSystem(new MovementSystem(aiSystem));
            world.AddSystem(new AOESystem(this));
            world.AddSystem(new SeparationSystem());
            world.AddSystem(new PropertySystem());
            map = new Map(graphics.GraphicsDevice, entityFactory);

            Entity e;

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
            Component.ComponentTypes.Add(typeof(HealthComponent));
            Component.ComponentTypes.Add(typeof(PhysicsComponent));
            Component.ComponentTypes.Add(typeof(DirectionComponent));
            Component.ComponentTypes.Add(typeof(SeparationComponent));
            Component.ComponentTypes.Add(typeof(InventoryComponent));

            //load serialized entities
            world.EntityMgr.DeserializeEntities(App.Path + "Main.xml");
            world.EntityMgr.DeserializeEntities(App.Path + "Monsters.xml");
            gameEntity = world.EntityMgr.GetEntityByName("Game").reference;
            world.AddSystem(gLogicSystem);

            //SaveTestEntites();

            player = entityFactory.CreatePlayerEntity();
            player.Name = "Player";
            player.Refresh();

            //weapons
            e = entityFactory.CloneEntity(world.EntityMgr.GetEntityByName("BasicSword"));
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

            gLogicSystem.SetupNextRound();

        }
        public void LoadMap(string mapname)
        {
            GameplayDataCaptureSystem.Instance.LogEvent(CaptureEventType.MapSelected, mapname);

            map.LoadMap(mapname, graphics.GraphicsDevice, Content);
            renderer.ActiveMap = map;

            //Need to be moved
            Entity wall = entityFactory.CreateWallEntity(new Vector2(0f, 0f), new Vector2(0f, renderer.ActiveMap.getPixelHeight()),
                            new Vector2(renderer.ActiveMap.getPixelWidth(), 0f), new Vector2(renderer.ActiveMap.getPixelWidth(), renderer.ActiveMap.getPixelHeight()));
            wall.Refresh();
        }
        protected override void LoadContent()
        {
            //map.LoadMap("Forest.tmx", graphics.GraphicsDevice, Content);
            //renderer.ActiveMap = map;

            //Need to be moved
            //Entity wall = entityFactory.CreateWallEntity(new Vector2(0f, 0f), new Vector2(0f, renderer.ActiveMap.getPixelHeight()),
            //                new Vector2(renderer.ActiveMap.getPixelWidth(), 0f), new Vector2(renderer.ActiveMap.getPixelWidth(), renderer.ActiveMap.getPixelHeight()));
            // wall.Refresh();

        }

        protected override void UnloadContent()
        {
            GameplayDataCaptureSystem.Instance.FlushSessions();
        }
        public bool GameWon
        {
            get;
            set;
        }

        protected override void Update(GameTime gameTime)
        {
            inputManager.DispatchInput();


            gameStateManager.CurrentState.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            MouseState ms = Mouse.GetState();
            UIEngine.GetInput(ms.X, ms.Y, ms.LeftButton == ButtonState.Pressed, ms.RightButton == ButtonState.Pressed, ms.MiddleButton == ButtonState.Pressed);

            physics.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            UIEngine.Update((float)gameTime.ElapsedGameTime.TotalMilliseconds);
            renderer.Render();
            UIEngine.Render();
            base.Draw(gameTime);
        }
    }
}
