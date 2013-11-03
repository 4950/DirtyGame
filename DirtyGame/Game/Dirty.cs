#region Using Statements
using System;
using DirtyGame.game.Core;
using DirtyGame.game.Core.Systems;
using DirtyGame.game.Core.Systems.Render;
using DirtyGame.game.Input;
using DirtyGame.game.SGraphics;
using DirtyGame.game.Systems;
using DirtyGame.game.Map;
using EntityFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using EntityFramework.Systems;
using DirtyGame.game.Core.Systems.Monster;
using DirtyGame.game.Core.GameStates;
using DirtyGame.game.Core.Components;


#endregion

namespace DirtyGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Dirty : Game
    {
        private Map map;

        public World world;
        public GraphicsDeviceManager graphics;
        public Renderer renderer;
        public EntityFactory entityFactory;
        public ResourceManager resourceManager;
        public GameStateManager gameStateManager;
        public InputManager inputManager;
        public AISystem aiSystem;
        public InputContext baseContext;
        public InputContext gamePlayContext;
        private readonly int MAX_MONSTERS = 20;

        public Dirty()
        {
        
            graphics = new GraphicsDeviceManager(this);
            inputManager = InputManager.Instance;
            resourceManager = new ResourceManager(Content);                       
            renderer = new Renderer(graphics, new Camera());
            baseContext = new InputContext();
            baseContext.RegisterHandler(Keys.Escape, Exit, null);
            inputManager.AddInputContext(baseContext);
            gameStateManager = new GameStateManager(this);
            aiSystem = new AISystem();

            world = new World();
            entityFactory = new EntityFactory(world.EntityMgr, resourceManager);                        
            world.AddSystem(new SpriteRenderSystem(renderer));
            world.AddSystem(new PlayerControlSystem(baseContext));
            world.AddSystem(new CameraUpdateSystem(renderer));
            world.AddSystem(new MapBoundarySystem(renderer));
            world.AddSystem(new SpawnerSystem(entityFactory));
            world.AddSystem(new MonsterSystem(aiSystem));
            world.AddSystem(new GameLogicSystem());
            world.AddSystem(new CollisionSystem());
            world.AddSystem(new AnimationSystem());
            map = new Map(graphics.GraphicsDevice);

            SpriteSheet playerSpriteSheet = new SpriteSheet(resourceManager.GetResource<Texture2D>("playerSheet"), "Content\\PlayerAnimation.xml");
            SpriteSheet monsterSpriteSheet = new SpriteSheet(resourceManager.GetResource<Texture2D>("monsterSheet_JUNK"), "Content\\MonsterAnimation.xml");

            Entity playerEntity = entityFactory.CreatePlayerEntity(playerSpriteSheet);
            playerEntity.Refresh();
         //   DirectionComponent playerDirection = playerEntity.GetComponent<DirectionComponent>()

       //     InputContext playerUpInput = new InputContext();
       //     playerUpInput.RegisterHandler(Keys.Up, playerEntity.GetComponent<DirectionComponent>().playerDirectionUp, null);

            Entity e = entityFactory.CreateSpawner(100, 100, playerSpriteSheet, new Rectangle(0, 0, 46, 46), MAX_MONSTERS / 4, new TimeSpan(0, 0, 0, 0, 1000));
            e.Refresh();
            e = entityFactory.CreateSpawner(300, 100, monsterSpriteSheet, new Rectangle(0, 0, 46, 46), MAX_MONSTERS / 4, new TimeSpan(0, 0, 0, 0, 2000));
            e.Refresh();
            e = entityFactory.CreateSpawner(100, 300, monsterSpriteSheet, new Rectangle(0, 0, 46, 46), MAX_MONSTERS / 4, new TimeSpan(0, 0, 0, 0, 3000));
            e.Refresh();
            e = entityFactory.CreateSpawner(300, 300, playerSpriteSheet, new Rectangle(0, 0, 46, 46), MAX_MONSTERS / 4, new TimeSpan(0, 0, 0, 0, 500));
            e.Refresh();
                                             
        }

        protected override void LoadContent()
        {
            map.LoadMap("Cave.tmx", graphics.GraphicsDevice, Content);
            renderer.ActiveMap = map;
        }

        protected override void UnloadContent()
        {
           
        }

        protected override void Update(GameTime gameTime)
        {          
            inputManager.DispatchInput();
            gameStateManager.CurrentState.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            
            renderer.Render();
            base.Draw(gameTime);
        }
    }
}
