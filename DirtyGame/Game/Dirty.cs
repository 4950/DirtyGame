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

using CoreUI;
using CoreUI.DrawEngines;
using DirtyGame.game.Input;

using DirtyGame.game.Core.Components;
using FarseerPhysics.Dynamics;
using DirtyGame.game.Core.Systems.Movement;



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

        private void Exit(Keys key)
        {
            Exit();
        }
        public Dirty()
        {

            inputManager = InputManager.Instance;
            baseContext = new InputContext();
            inputManager.AddInputContext(baseContext);
            baseContext.RegisterHandler(Keys.Escape, Exit, null);
            
            graphics = new GraphicsDeviceManager(this);

            resourceManager = new ResourceManager(Content);
            //init UI
            UIDraw = new MonoGameDrawEngine(graphics.GraphicsDevice, Content);
            UIEngine = new CoreUIEngine(UIDraw, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);
            SpriteFont defaultFont = Content.Load<SpriteFont>("default");
            UIDraw.setDefaultFont(defaultFont);      
            resourceManager = new ResourceManager(Content);                       
            renderer = new Renderer(graphics, new Camera(new Vector2(800, 600)));
            world = new EntityFramework.World();
            physics = new Physics(world.EntityMgr);

            gameStateManager = new GameStateManager(this);
            entityFactory = new EntityFactory(world.EntityMgr, resourceManager);
            aiSystem = new AISystem(this, entityFactory);
            world.AddSystem(aiSystem);
            world.AddSystem(new SpriteRenderSystem(renderer));
            world.AddSystem(new PlayerControlSystem(entityFactory, renderer, this));
            world.AddSystem(new WeaponSystem());
            world.AddSystem(new CameraUpdateSystem(renderer));
            world.AddSystem(new MapBoundarySystem(renderer));
            world.AddSystem(new SpawnerSystem(entityFactory));
            world.AddSystem(new HUDSystem(renderer, UIEngine));
            world.AddSystem(new ProjectileSystem(this));
            //world.AddSystem(new MonsterSystem(aiSystem));
            gLogicSystem = new GameLogicSystem(this);
            world.AddSystem(gLogicSystem);         
            world.AddSystem(new PhysicsSystem(physics, renderer));
            world.AddSystem(new GameLogicSystem(this));
            world.AddSystem(new AnimationSystem());
            world.AddSystem(new MovementSystem(aiSystem));
            world.AddSystem(new SeparationSystem());
            map = new Map(graphics.GraphicsDevice);

            

            SpriteSheet playerSpriteSheet =  new SpriteSheet(resourceManager.GetResource<Texture2D>("playerSheet"), "Content\\PlayerAnimation.xml");
            SpriteSheet monsterSpriteSheet = new SpriteSheet(resourceManager.GetResource<Texture2D>("monsterSheet_JUNK"), "Content\\MonsterAnimation.xml");
            
            
            player = entityFactory.CreatePlayerEntity(playerSpriteSheet);
            player.Refresh();

            //weapons
            Entity e = entityFactory.CreateRangedWeaponEntity("Doomsbow", "bow", "bow", 400, 25, 10, "arrow", -1, 1f);
            e.Refresh();
            player.GetComponent<InventoryComponent>().addWeapon(e);
            e = entityFactory.CreateRangedWeaponEntity("Spear", "spear", "spear", 200, 35, 5, "spear", 5, 2f);
            e.Refresh();
            player.GetComponent<InventoryComponent>().addWeapon(e);

            Entity monsterWeapon = entityFactory.CreateRangedWeaponEntity("Monsterbow", "bow", "bow", 400, 20, 10, "arrow", -1, 3f);
            monsterWeapon.Refresh();
            MonsterData rangedData = MonsterData.RangedMonster;
            rangedData.weapon = monsterWeapon;

            e = entityFactory.CreateSpawner(100, 100, playerSpriteSheet, new Rectangle(0, 0, 46, 46), rangedData, 1, new TimeSpan(0, 0, 0, 0, 1000));
            e.Refresh();
            e = entityFactory.CreateSpawner(300, 100, monsterSpriteSheet, new Rectangle(0, 0, 46, 46), MonsterData.BasicMonster, 1, new TimeSpan(0, 0, 0, 0, 2000));
            e.Refresh();
            e = entityFactory.CreateSpawner(100, 300, playerSpriteSheet, new Rectangle(0, 0, 46, 46), rangedData, 1, new TimeSpan(0, 0, 0, 0, 3000));
            e.Refresh();
            e = entityFactory.CreateSpawner(300, 300, monsterSpriteSheet, new Rectangle(0, 0, 46, 46), MonsterData.BasicMonster, 1, new TimeSpan(0, 0, 0, 0, 500));
            e.Refresh();

            
        }

        protected override void LoadContent()
        {
            map.LoadMap("Cave.tmx", graphics.GraphicsDevice, Content);
            renderer.ActiveMap = map;

            //Need to be moved
            Entity wall = entityFactory.CreateWallEntity(new Vector2(0f, 0f), new Vector2(0f, renderer.ActiveMap.getPixelHeight()),
                            new Vector2(renderer.ActiveMap.getPixelWidth(), 0f), new Vector2(renderer.ActiveMap.getPixelWidth(), renderer.ActiveMap.getPixelHeight()));
            wall.Refresh();
            
        }

        protected override void UnloadContent()
        {
           
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
