#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using DirtyGame.game.Core;
using DirtyGame.game.Core.Components;
using DirtyGame.game.Core.Components.Movement;
using DirtyGame.game.Core.Systems;
using DirtyGame.game.Core.Systems.Movement;
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
        private GridMap _gridMap;
        public World world;
        public GraphicsDeviceManager graphics;
        public Renderer renderer;
        public EntityFactory entityFactory;
        public ResourceManager resourceManager;
        public GameStateManager gameStateManager;
        public AISystem aiSystem;
        private readonly int MAX_MONSTERS = 20;
        
        public Dirty()
        {
            graphics = new GraphicsDeviceManager(this);           
            resourceManager = new ResourceManager(Content);                       
            renderer = new Renderer(graphics, new Camera());
            world = new World();
            gameStateManager = new GameStateManager(this);
            entityFactory = new EntityFactory(world.EntityMgr, resourceManager);            
            world.AddSystem(new SpriteRenderSystem(renderer));
            world.AddSystem(new PlayerControlSystem());
            world.AddSystem(new CameraUpdateSystem(renderer));
            world.AddSystem(new MapBoundarySystem(renderer));
            world.AddSystem(new SpawnerSystem(entityFactory));          
            world.AddSystem(new GameLogicSystem());
            //world.AddSystem(new CollisionSystem());
            world.AddSystem(new AnimationSystem());
            world.AddSystem(new SeekMovementSystem());
            world.AddSystem(new FleeMovementSystem());
            map = new Map(graphics.GraphicsDevice);

            SpriteSheet playerSpriteSheet =  new SpriteSheet(resourceManager.GetResource<Texture2D>("playerSheet"), "Content\\PlayerAnimation.xml");
            SpriteSheet monsterSpriteSheet = new SpriteSheet(resourceManager.GetResource<Texture2D>("monsterSheet_JUNK"), "Content\\MonsterAnimation.xml");
            
            Entity player = entityFactory.CreatePlayerEntity(playerSpriteSheet);
            player.Refresh();

            Entity e = entityFactory.CreateFleeEntity(50, 50, monsterSpriteSheet, player);            
            e.Refresh();

            e = entityFactory.CreateChaseEntity(50, 50, monsterSpriteSheet, player);
            e.Refresh();
     
           // e = entityFactory.CreateSpawner(300, 100, monsterSpriteSheet, new Rectangle(0, 0, 46, 46), MAX_MONSTERS / 4, new TimeSpan(0, 0, 0, 0, 100));
            //e.Refresh();
            //e = entityFactory.CreateSpawner(100, 300, monsterSpriteSheet, new Rectangle(0, 0, 46, 46), MAX_MONSTERS / 4, new TimeSpan(0, 0, 0, 0, 100));
            //e.Refresh();
     

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
            Tileset tileset = new Tileset(texture, 25, 25);
            BSPMapGenerator mapGenerator = new BSPMapGenerator(tileset, 100, 160, 50, 15);            
            _gridMap = mapGenerator.GenerateMap3(new Point(0, 0));
            PathFinder p = new PathFinder();
            TileData tile1 = _gridMap.GetTile(Utillity.GetRandomPointInside(_gridMap.Bounds));
            while (tile1.Passable != true)
            {
                tile1 = _gridMap.GetTile(Utillity.GetRandomPointInside(_gridMap.Bounds));
            }
            TileData tile2 = _gridMap.GetTile(Utillity.GetRandomPointInside(_gridMap.Bounds));
            while (tile2.Passable != true)
            {
                tile2 = _gridMap.GetTile(Utillity.GetRandomPointInside(_gridMap.Bounds));
            }
            player.GetComponent<Spatial>().MoveTo(new Vector2(tile1.DstRect.Center.X, tile1.DstRect.Center.Y));
            p.GetPath2(_gridMap, tile1.DstRect.Center, tile2.DstRect.Center);
        }

        protected override void LoadContent()
        {
            map.LoadMap("Cave.tmx", graphics.GraphicsDevice, Content);
            renderer.ActiveMap = _gridMap;
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
