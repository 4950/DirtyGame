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
using ShittyPrototype.src.Map;
using ShittyPrototype.src.application;
using System.Diagnostics;
using ShittyPrototype.src.application.core;
using CoreUI;
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
        CoreUIEngine uiEngine;
        CoreUI.DrawEngines.MonoGameDrawEngine uiDraw;
        SpriteFont defaultFont;
        Gamestate gamestate;


        public ShittyPrototype()
            : base()
        {
            Content.RootDirectory = "Content";
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferFormat = SurfaceFormat.Color;
            graphics.CreateDevice();
            inputManager = InputManager.GetSingleton();
            sceneManager = new SceneManager(graphics);
            entityFactor = new EntityFactory(graphics.GraphicsDevice);
            spawnerManager = new SpawnerManager();
            spawnerList = new Spawner[4];
//            spawnerList = new Spawner[1];
            monstersToSpawn = new List<Monster>();
            monsterManager = new MonsterManager();
            map = new Map(graphics.GraphicsDevice);
            gamestate = new Gamestate();

            uiDraw = new CoreUI.DrawEngines.MonoGameDrawEngine(graphics.GraphicsDevice, Content);
            uiEngine = new CoreUIEngine(uiDraw, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);
        }

        public static void LuaRegisteredFunc(string msg)
        {
            Console.WriteLine(msg);
            
        }
        #region UI Event Code
        void l_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            ((CoreUI.Elements.Label)sender).TextMode = CoreUI.Elements.LabelTextMode.SizeToContent;
            ((CoreUI.Elements.Label)sender).Text = "but can expand to fill content!\nEven if its multiple lines with different sizes";
        }

        void b4_Click(object sender)
        {
            CoreUI.MessageBox.Show("Is this a messagebox?", "Question!", CoreUI.MessageBox.MessageBoxButttons.YesNo);
        }

        void b_Click(object sender)
        {
            ((CoreUI.Elements.Button)sender).Text = "!&$@$%!(%$&@%$%&!*%$&@*$%*&!%&$@";
            ((Window)((CoreUI.Elements.Button)sender).Tag).Show();
        }

        void b3_Click(object sender)
        {
            Window w = new Window();
            w.Position = new System.Drawing.Point(50, 50);
            w.Size = new System.Drawing.Point(200, 100);
            w.Title = "OMG, Modality!";
            w.ShowDialog();

            CoreUI.Elements.Button b = new CoreUI.Elements.Button();
            b.Position = new System.Drawing.Point(10, 10);
            b.Size = new System.Drawing.Point(100, 23);
            b.Text = "Close";
            b.Click += new CoreUI.Elements.Button.ClickEventHandler(Modalb_click);
            w.Content = b;
        }
        void Modalb_click(object sender)
        {
            ((Window)((CoreUI.Elements.Button)sender).Parent).Close();
        }
        #endregion

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {

            defaultFont = Content.Load<SpriteFont>("NI7SEG");

#region Test UI

            uiDraw.setDefaultFont(defaultFont);

            Window w = new Window();
            w.Position = new System.Drawing.Point(200, 200);
            w.Size = new System.Drawing.Point(200, 200);
            w.Title = "Window";
            w.Show();

            CoreUI.Panel p = new CoreUI.Panel();
            p.SizeMode = SizeMode.Fill;
            w.Content = p;

            CoreUI.Elements.Button b2 = new CoreUI.Elements.Button();
            b2.Position = new System.Drawing.Point(10, 10);
            b2.Size = new System.Drawing.Point(100, 23);
            b2.Text = "Rawr!";
            //b2.Font = new Font("Viner Hand ITC", 24);
            p.AddElement(b2);

            CoreUI.Elements.Button b3 = new CoreUI.Elements.Button();
            b3.Position = new System.Drawing.Point(10, 70);
            b3.Size = new System.Drawing.Point(175, 23);
            b3.Text = "Show Modal Dialog";
            b3.Click += new CoreUI.Elements.Button.ClickEventHandler(b3_Click);
            p.AddElement(b3);

            CoreUI.Elements.Button b4 = new CoreUI.Elements.Button();
            b4.Position = new System.Drawing.Point(10, 140);
            b4.Size = new System.Drawing.Point(175, 23);
            b4.Text = "Show MessageBox";
            b4.Click += new CoreUI.Elements.Button.ClickEventHandler(b4_Click);
            p.AddElement(b4);
            
            Window ww = new Window();
            ww.Position = new System.Drawing.Point(250, 300);
            ww.Size = new System.Drawing.Point(300, 200);
            ww.Title = "Another Window!";
            ww.Show();

            CoreUI.Panel p2 = new CoreUI.Panel();
            p2.SizeMode = SizeMode.Fill;
            ww.Content = p2;

            CoreUI.Elements.Label l = new CoreUI.Elements.Label();
            l.Position = new System.Drawing.Point(5, 20);
            l.Size = new System.Drawing.Point(175, 23);
            l.Text = "(Click me)This label truncates";
            l.TextMode = CoreUI.Elements.LabelTextMode.Truncate;
            l.MouseUp += new System.Windows.Forms.MouseEventHandler(l_MouseUp);
            p2.AddElement(l);

            CoreUI.Elements.Listbox lb = new CoreUI.Elements.Listbox();
            lb.Position = new System.Drawing.Point(5, 50);
            lb.Size = new System.Drawing.Point(100, 100);
            lb.AddItem("This is");
            lb.AddItem("a listbox!");
            lb.AddItem("which");
            lb.AddItem("also truncates");
            lb.AddItem("in both directions");
            lb.AddItem("See?");
            lb.AddItem("!!!!!!!!!!");
            lb.AddItem("Scrolling!");
            p2.AddElement(lb);

            CoreUI.Elements.ComboBox cb = new CoreUI.Elements.ComboBox();
            cb.Position = new System.Drawing.Point(110, 50);
            cb.Size = new System.Drawing.Point(100, 10);
            cb.AddItem("ComboBox!");
            cb.AddItem("~Select Me~");
            p2.AddElement(cb);

            CoreUI.Elements.CheckBox chb = new CoreUI.Elements.CheckBox();
            chb.Position = new System.Drawing.Point(110, 70);
            chb.Size = new System.Drawing.Point(100, 15);
            chb.Text = "A Checkbox";
            chb.IsThreeState = true;
            p2.AddElement(chb);

            CoreUI.Elements.RadioButton rdb = new CoreUI.Elements.RadioButton();
            rdb.Position = new System.Drawing.Point(110, 90);
            rdb.Size = new System.Drawing.Point(100, 15);
            rdb.Text = "A RadioButton";
            rdb.IsThreeState = true;
            p2.AddElement(rdb);

            CoreUI.Elements.Button b = new CoreUI.Elements.Button();
            b.Position = new System.Drawing.Point(100, 100);
            b.Size = new System.Drawing.Point(130, 23);
            b.Text = "Relaunch Window";
            b.Tag = w;
            b.Click += new CoreUI.Elements.Button.ClickEventHandler(b_Click);
            uiEngine.Children.AddElement(b);

            CoreUI.MessageBox.Show("Warning, Virus Detected!\nEat it?", "Warning", CoreUI.MessageBox.MessageBoxButttons.OkCancel);

            Window www = new Window();
            www.Position = new System.Drawing.Point(400, 50);
            www.Size = new System.Drawing.Point(175, 50);
            www.Style = CoreUI.Window.WindowStyle.None;
            www.Show();

            CoreUI.Elements.Label l2 = new CoreUI.Elements.Label();
            l2.Position = new System.Drawing.Point(0, 0);
            l2.Text = "Window without chrome!\n(This is a label)";
            l2.TextMode = CoreUI.Elements.LabelTextMode.SizeToContent;
            www.Content = l2;

#endregion

            Entity e = entityFactor.CreateTestEntity();
            sceneManager.Add(e);
            Spawner s = new Spawner(1000, 100, 100, 10, Content.Load<Texture2D>("monster"));
            Spawner s2 = new Spawner(2000, 400, 100, 10, Content.Load<Texture2D>("monster2"));
            Spawner s3 = new Spawner(3000, 100, 300, 10, Content.Load<Texture2D>("monster3"));
            Spawner s4 = new Spawner(500, 400, 300, 10, Content.Load<Texture2D>("monster4"));
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
            Entity player = entityFactor.createPlayerEntity("Player", 50, 50, 12, Content);
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
            if (inputManager.IsKeyDown(Keys.Escape) || gamestate.Gameover())
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
            
            sceneManager.DetectCollision(monsterManager);
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
            
            //ui input
            uiEngine.GetInput(inputManager.mouseState.X, inputManager.mouseState.Y, inputManager.mouseState.LeftButton == ButtonState.Pressed, inputManager.mouseState.RightButton == ButtonState.Pressed, inputManager.mouseState.MiddleButton == ButtonState.Pressed);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
//            uiEngine.Update((float)gameTime.ElapsedGameTime.TotalMilliseconds);

            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            map.draw(sceneManager.camera);
            sceneManager.Render(gameTime);
            
   //         uiEngine.Render();
            //SpriteBatch spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            //Debug.WriteLine("Draw");
            //spawnerManager.Draw(spriteBatch, monster);
            
            base.Draw(gameTime);
        }
    }
}
