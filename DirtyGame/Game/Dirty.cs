﻿#region Using Statements
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
        public AISystem aiSystem;
        private readonly int MAX_MONSTERS = 20;
        public CoreUIEngine UIEngine;
        public GameLogicSystem gLogicSystem;
        public MonoGameDrawEngine UIDraw;

        public Dirty()
        {

            graphics = new GraphicsDeviceManager(this);
            resourceManager = new ResourceManager(Content);
            //init UI
            UIDraw = new MonoGameDrawEngine(graphics.GraphicsDevice, Content);
            UIEngine = new CoreUIEngine(UIDraw, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);
            SpriteFont defaultFont = Content.Load<SpriteFont>("default");
            UIDraw.setDefaultFont(defaultFont);
         
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
            gLogicSystem = new GameLogicSystem();
            world.AddSystem(gLogicSystem);
            world.AddSystem(new CollisionSystem());
            world.AddSystem(new AnimationSystem());
            map = new Map(graphics.GraphicsDevice);

            

            SpriteSheet playerSpriteSheet =  new SpriteSheet(resourceManager.GetResource<Texture2D>("playerSheet"), "Content\\PlayerAnimation.xml");
            SpriteSheet monsterSpriteSheet = new SpriteSheet(resourceManager.GetResource<Texture2D>("monsterSheet_JUNK"), "Content\\MonsterAnimation.xml");
            
            Entity e = entityFactory.CreatePlayerEntity(playerSpriteSheet);
            e.Refresh();

            e = entityFactory.CreateSpawner(100, 100, playerSpriteSheet, new Rectangle(0, 0, 46, 46), MAX_MONSTERS / 4, new TimeSpan(0, 0, 0, 0, 1000));
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            gameStateManager.CurrentState.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            MouseState ms = Mouse.GetState();
            UIEngine.GetInput(ms.X, ms.Y, ms.LeftButton == ButtonState.Pressed, ms.RightButton == ButtonState.Pressed, ms.MiddleButton == ButtonState.Pressed);
            UIEngine.Update((float)gameTime.ElapsedGameTime.TotalMilliseconds);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            
            renderer.Render();
            UIEngine.Render();
            base.Draw(gameTime);
        }
    }
}
