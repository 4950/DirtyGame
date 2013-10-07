#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using ShittyPrototype.src.core;
using ShittyPrototype.src.graphics;
using ShittyPrototype.src.util;
using ShittyPrototype.src.application;
using System.Diagnostics;
using ShittyPrototype.src.application.core;
#endregion

namespace ShittyPrototype
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class ShittyPrototype : Game
    {
        GraphicsDeviceManager graphics;
        
        InputManager inputManager;
        SceneManager sceneManager;
        EntityFactory entityFactor;
        SpawnerManager spawnerManager;
        public Texture2D monsterTexture;
        Spawner[] spawnerList;
        List<Monster> monstersToSpawn;
        MonsterManager monsterManager;

        public ShittyPrototype()
            : base()
        {
            Content.RootDirectory = "Content";
            graphics = new GraphicsDeviceManager(this);
            inputManager = InputManager.GetSingleton();
            sceneManager = new SceneManager(graphics);
            entityFactor = new EntityFactory(graphics.GraphicsDevice);
            spawnerManager = new SpawnerManager();
            spawnerList = new Spawner[4];
            monstersToSpawn = new List<Monster>();
            monsterManager = new MonsterManager();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {

            Entity e = entityFactor.CreateTestEntity();
            sceneManager.Add(e);
            Spawner s = new Spawner(1000, 100, 100, 30, Content.Load<Texture2D>("monster"));
            Spawner s2 = new Spawner(2000, 400, 100, 30, Content.Load<Texture2D>("monster2"));
            Spawner s3 = new Spawner(3000, 100, 300, 30, Content.Load<Texture2D>("monster3"));
            Spawner s4 = new Spawner(500, 400, 300, 30, Content.Load<Texture2D>("monster4"));
            spawnerList[0] = s;
            spawnerList[1] = s2;
            spawnerList[2] = s3;
            spawnerList[3] = s4;

            //sceneManager.Add(spawnerList[0]);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            monsterTexture = Content.Load<Texture2D>("monster");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            inputManager.Update();
            //Debug.WriteLine(gameTime.TotalGameTime + " , " + gameTime.ElapsedGameTime);
            if (inputManager.IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            
            spawnerManager.spawners = spawnerList;
            monstersToSpawn = spawnerManager.Update(gameTime);
            foreach (Monster m in monstersToSpawn)
            {
                sceneManager.Add(m);
                monsterManager.Add(m);
            }
            
            monstersToSpawn.Clear();

            monsterManager.UpdateEntities(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            sceneManager.Render();
            //SpriteBatch spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            //Debug.WriteLine("Draw");
            //spawnerManager.Draw(spriteBatch, monster);
            
            base.Draw(gameTime);
        }
    }
}
