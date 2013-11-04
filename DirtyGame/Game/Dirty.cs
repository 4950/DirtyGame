#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using DirtyGame.game.Core;
using DirtyGame.game.Core.Systems;
using DirtyGame.game.Core.Systems.Render;
using DirtyGame.game.Map.Generators.BSP;
using DirtyGame.game.SGraphics;
using DirtyGame.game.Systems;
using DirtyGame.game.Map;
using DirtyGame.game.Util;
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
using OpenTK.Graphics.ES11;

#endregion

namespace DirtyGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Dirty : Game
    {

        

        private Map map;        
        private Map3 map3;
        public World world;
        public GraphicsDeviceManager graphics;
        public Renderer renderer;
        public EntityFactory entityFactory;
        public ResourceManager resourceManager;
        public GameStateManager gameStateManager;
        public AISystem aiSystem;
        private readonly int MAX_MONSTERS = 20;
        private SpriteBatch batch;
        public Dirty()
        {
            graphics = new GraphicsDeviceManager(this);
            batch = new SpriteBatch(graphics.GraphicsDevice);
            resourceManager = new ResourceManager(Content);                       
            renderer = new Renderer(graphics, new Camera());
            world = new World();
            gameStateManager = new GameStateManager(this);
            entityFactory = new EntityFactory(world.EntityMgr, resourceManager);
            aiSystem = new AISystem();
            world.AddSystem(new SpriteRenderSystem(renderer));
            world.AddSystem(new PlayerControlSystem());
            world.AddSystem(new CameraUpdateSystem(renderer));
            world.AddSystem(new MapBoundarySystem(renderer));
            world.AddSystem(new SpawnerSystem(entityFactory));
            world.AddSystem(new MonsterSystem(aiSystem));
            world.AddSystem(new GameLogicSystem());
            world.AddSystem(new CollisionSystem());
            world.AddSystem(new AnimationSystem());
            map = new Map(graphics.GraphicsDevice);

            SpriteSheet playerSpriteSheet =  new SpriteSheet(resourceManager.GetResource<Texture2D>("playerSheet"), "Content\\PlayerAnimation.xml");
            SpriteSheet monsterSpriteSheet = new SpriteSheet(resourceManager.GetResource<Texture2D>("monsterSheet_JUNK"), "Content\\MonsterAnimation.xml");
            
            Entity e = entityFactory.CreatePlayerEntity(playerSpriteSheet);
            e.Refresh();

           // e = entityFactory.CreateSpawner(100, 100, playerSpriteSheet, new Rectangle(0, 0, 46, 46), MAX_MONSTERS / 4, new TimeSpan(0, 0, 0, 0, 1000));
          //  e.Refresh();
            e = entityFactory.CreateSpawner(300, 100, monsterSpriteSheet, new Rectangle(0, 0, 46, 46), MAX_MONSTERS / 4, new TimeSpan(0, 0, 0, 0, 2000));
            e.Refresh();
            e = entityFactory.CreateSpawner(100, 300, monsterSpriteSheet, new Rectangle(0, 0, 46, 46), MAX_MONSTERS / 4, new TimeSpan(0, 0, 0, 0, 3000));
            e.Refresh();
         //  e = entityFactory.CreateSpawner(300, 300, playerSpriteSheet, new Rectangle(0, 0, 46, 46), MAX_MONSTERS / 4, new TimeSpan(0, 0, 0, 0, 500));
         //   e.Refresh();

            Texture2D texture = new Texture2D(graphics.GraphicsDevice, 1000, 1, false, SurfaceFormat.Color);
            Color[] colors = new Color[1000];
            Random random = Rand.Random;
            for (int i = 0; i < 1000; ++i)
            {
                colors[i] = new Color(random.Next(256), random.Next(256), random.Next(256));
              
            }
            colors[0] = Color.White;
            colors[1] = Color.Gray;
            int a = 2;
            texture.SetData<Color>(colors);
            Tileset tileset = new Tileset(texture, 50, 50);
            BSPMapGenerator mapGenerator = new BSPMapGenerator(tileset, 100, 160, 50, 15);            
            map3 = mapGenerator.GenerateMap3(new Point(0, 0));
        }

        protected override void LoadContent()
        {
            map.LoadMap("Cave.tmx", graphics.GraphicsDevice, Content);
            renderer.ActiveMap = map3;
        }

        protected override void UnloadContent()
        {
           
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


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
