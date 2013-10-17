﻿#region Using Statements
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
using ShittyPrototype.src.Map;
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
        Map map;
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
            map = new Map(graphics.GraphicsDevice);
        }

        public static void LuaRegisteredFunc(string msg)
        {
            Console.WriteLine(msg);
            
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
            GlobalLua.lua.DoFile("scripts\\HelloWorld.lua");

            GlobalLua.lua.DoString("myvar = 25");
            double myvar = (double)GlobalLua.lua["myvar"];
            Console.WriteLine(myvar);

            // Possible way of passing parameters. There could be a better way to do it, I just don't know it.
            double x = 10;
            GlobalLua.lua["x"] = x;
            GlobalLua.DoFile("IncrementX");     // using the convenience method rather than directly calling the LuaInterface one
            x = (double) GlobalLua.lua["x"];
            Console.WriteLine(x);

            // Example of registering a C# function so that Lua scripts can call it.
            // Just prints a message.
            GlobalLua.lua.RegisterFunction("LuaRegisteredFunc", this, this.GetType().GetMethod("LuaRegisteredFunc"));
            GlobalLua.lua.DoFile("scripts\\UseRegisteredFunc.lua");
            Entity player = entityFactor.createPlayerEntity();
            sceneManager.Add(player);
            sceneManager.CenterOnPlayer();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            map.LoadMap("Cave.tmx", graphics.GraphicsDevice, Content);
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

            if (inputManager.IsKeyDown(Keys.Left))
            {
                sceneManager.MovePlayer(-5,0);
            }
            if (inputManager.IsKeyDown(Keys.Right))
            {
                sceneManager.MovePlayer(5,0);
            }
            if (inputManager.IsKeyDown(Keys.Up))
            {
                sceneManager.MovePlayer(0,-5); //(0,0) is TOP left
            }
            if (inputManager.IsKeyDown(Keys.Down))
            {
                sceneManager.MovePlayer(0,5);
            }

            Entity p = new Entity();
            foreach (Entity entity in sceneManager.getEntities())
            {
                if (entity.HasComponent<InputComponent>()) 
                {
                    p = entity;
                }
            }

            PositionComponent playerPosition = (PositionComponent)p.GetComponent<PositionComponent>();
            int playerX = playerPosition.x;
            int playerY = playerPosition.y;

            foreach (Monster m in monstersToSpawn)
            {
                if ((playerX == m.pos.x) && (playerY == m.pos.y))
                {
                    sceneManager.Remove(m);
                }
            }




            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            map.draw(sceneManager.camera);
            sceneManager.Render();
            //SpriteBatch spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            //Debug.WriteLine("Draw");
            //spawnerManager.Draw(spriteBatch, monster);
            
            base.Draw(gameTime);
        }
    }
}
