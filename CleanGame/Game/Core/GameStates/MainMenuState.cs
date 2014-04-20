using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreUI;
using CoreUI.Elements;
using Microsoft.Xna.Framework.Graphics;
using CleanGame.Game.Util;

namespace CleanGame.Game.Core.GameStates
{
    class MainMenuState : IGameState
    {
        private Dirty game;
        private Window menu;
        private Window selectMap;
        private Window settingsWindow;
        private CheckBox full;
        private CheckBox cap;
        private Label versionLbl;
        private bool isMapWindowShown = false;
        //string[] maps = { "Cave", "Forest", "Arena" };
        string[] maps = { "Arena" };

        public MainMenuState()
        {
            game = null;
           
        }

        public void OnEnter(Dirty game)
        {
            this.game = game;
            if (menu == null)
            {
                menu = new CoreUI.Window();
                menu.Style = Window.WindowStyle.None;
                menu.Size = new System.Drawing.Point(300, 400);
                menu.Position = new System.Drawing.Point(250, 50);

                CoreUI.Panel p = new CoreUI.Panel();
                p.SizeMode = SizeMode.Fill;
                menu.Content = p;

                CoreUI.Elements.Button b1 = new CoreUI.Elements.Button();
                b1.Position = new System.Drawing.Point(10, 10);
                b1.Size = new System.Drawing.Point(280, 50);
                b1.Text = "New Game";
                b1.Click += startGame;
                p.AddElement(b1);

                CoreUI.Elements.Button b2 = new CoreUI.Elements.Button();
                b2.Position = new System.Drawing.Point(10, 70);
                b2.Size = new System.Drawing.Point(280, 50);
                b2.Text = "Tutorial";
                b2.Click += startTutorial;
                p.AddElement(b2);

                CoreUI.Elements.Button b3 = new CoreUI.Elements.Button();
                b3.Position = new System.Drawing.Point(10, 190);
                b3.Size = new System.Drawing.Point(280, 50);
                b3.Text = "Options";
                b3.Click += options;
                p.AddElement(b3);

                CoreUI.Elements.Button b4 = new CoreUI.Elements.Button();
                b4.Position = new System.Drawing.Point(10, 250);
                b4.Size = new System.Drawing.Point(280, 50);
                b4.Text = "Exit";
                b4.Click += endGame;
                p.AddElement(b4);

                versionLbl = new Label();
                versionLbl.Position = new System.Drawing.Point(10, 375);
                versionLbl.Size = new System.Drawing.Point(100, 25);
                versionLbl.Text = App.PublishVersion;
                p.AddElement(versionLbl);
            }
            if (settingsWindow == null)
            {
                settingsWindow = new CoreUI.Window();
                settingsWindow.Style = Window.WindowStyle.None;
                settingsWindow.Size = new System.Drawing.Point(310, 150);
                settingsWindow.Position = new System.Drawing.Point(250, 50);

                CoreUI.Panel p = new CoreUI.Panel();
                p.SizeMode = SizeMode.Fill;
                settingsWindow.Content = p;

                Label title = new Label();
                title.Size = new System.Drawing.Point(300, 25);
                title.Position = new System.Drawing.Point(0, 0);
                //title.Foreground = new CoreUI.DrawEngines.MonoGameColor(Microsoft.Xna.Framework.Color.White);
                title.Text = "Settings";
                title.TextPosition = TextPosition.Center;
                p.AddElement(title);

#if DEBUG
                full = new CheckBox();
                full.Position = new System.Drawing.Point(20, 40);
                full.Size = new System.Drawing.Point(100, 20);
                full.Text = "Fullscreen";
                full.IsChecked = Settings.Instance.Global.Fullscreen;
                p.AddElement(full);
#endif

                cap = new CheckBox();
                cap.Position = new System.Drawing.Point(20, 60);
                cap.Size = new System.Drawing.Point(100, 20);
                cap.Text = "Capture Mouse";
                cap.IsChecked = Settings.Instance.Global.DefaultUser.CaptureMouse;
                p.AddElement(cap);

                CoreUI.Elements.Button s = new CoreUI.Elements.Button();
                s.Position = new System.Drawing.Point(20, 120);
                s.Size = new System.Drawing.Point(50, 25);
                s.Text = "Cancel";
                s.Click += back;
                p.AddElement(s);

                CoreUI.Elements.Button s2 = new CoreUI.Elements.Button();
                s2.Position = new System.Drawing.Point(250, 120);
                s2.Size = new System.Drawing.Point(50, 25);
                s2.Text = "Save";
                s2.Click += s2_Click;
                p.AddElement(s2);
            }
            if (selectMap == null)
            {
                selectMap = new CoreUI.Window();
                selectMap.Style = Window.WindowStyle.None;
                selectMap.Size = new System.Drawing.Point(310, 150);
                selectMap.Position = new System.Drawing.Point(250, 50);

                CoreUI.Panel p = new CoreUI.Panel();
                p.SizeMode = SizeMode.Fill;
                selectMap.Content = p;

                Label title = new Label();
                title.Size = new System.Drawing.Point(300, 25);
                title.Position = new System.Drawing.Point(0, 0);
                //title.Foreground = new CoreUI.DrawEngines.MonoGameColor(Microsoft.Xna.Framework.Color.White);
                title.Text = "Select a Map";
                title.TextPosition = TextPosition.Center;
                p.AddElement(title);

                
                for (int i = 0; i < maps.Length; i++)
                {
                    Panel mp = new Panel();
                    mp.Size = new System.Drawing.Point(90, 90);
                   // mp.Position = new System.Drawing.Point(10 + i * 100, 25);
                    mp.Position = new System.Drawing.Point(10 + 100, 25);
                    mp.Background = new CoreUI.DrawEngines.MonoGameColor(Microsoft.Xna.Framework.Color.Red);
                    mp.Tag = i;
                    mp.MouseDown += startMap;
                    p.AddElement(mp);

                    Texture2D t = game.resourceManager.GetResource<Texture2D>(maps[i] + "Preview");

                    CoreUI.Visuals.ImageBrush ib = new CoreUI.Visuals.ImageBrush();
                    ib.SizeMode = SizeMode.Fill;
                    ib.Texture = new CoreUI.DrawEngines.MonoGameTexture(t);
                    mp.BackgroundVisual = ib;

                    Label mapname = new Label();
                    mapname.Size = new System.Drawing.Point(90, 25);
                    //mapname.Position = new System.Drawing.Point(10 + i * 100, 70);
                    mapname.Position = new System.Drawing.Point(10 + 100, 70);
                    //title.Foreground = new CoreUI.DrawEngines.MonoGameColor(Microsoft.Xna.Framework.Color.White);
                    mapname.Text = maps[i];
                    mapname.TextPosition = TextPosition.Center;
                    p.AddElement(mapname);
                }

                CoreUI.Elements.Button s = new CoreUI.Elements.Button();
                s.Position = new System.Drawing.Point(130, 120);
                s.Size = new System.Drawing.Point(50, 25);
                s.Text = "Back";
                s.Click += back;
                p.AddElement(s);

                //CoreUI.Elements.Button c = new CoreUI.Elements.Button();
                //c.Position = new System.Drawing.Point(10, 190);
                //c.Size = new System.Drawing.Point(280, 50);
                //c.Text = "Exit";
                ////c.Click += endGame;
                //p.AddElement(c);
            }

            menu.Show();
        }

        void s2_Click(object sender)
        {
            //save settings
#if DEBUG
            if (game.graphics.IsFullScreen != (bool)full.IsChecked)
            {
                game.graphics.ToggleFullScreen();
                Settings.Instance.Global.Fullscreen = game.graphics.IsFullScreen;
            }
#endif
            Settings.Instance.Global.DefaultUser.CaptureMouse = (bool)cap.IsChecked;

            back(sender);
        }

        void startMap(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            int index = (int)((Panel)sender).Tag;
            game.StartSession(maps[index] + ".tmx");
        }
        void startTutorial(object sender)
        {
            game.StartTutorial();
        }
        void back(object sender)
        {
            if (isMapWindowShown)
            {
                isMapWindowShown = false;
                selectMap.Hide();
                menu.Show();
            }
            else
            {
                settingsWindow.Hide();
                menu.Show();
            }
        }
        void options(object sender)
        {
            menu.Hide();
            settingsWindow.Show();
        }
        void startGame(object sender)
        {
            menu.Hide();
            selectMap.Show();
            isMapWindowShown = true;
            
        }
        void endGame(object sender)
        {
            Events.Event endGame = new Events.Event();
            endGame.name = "GameState";
            EventManager.Instance.TriggerEvent(endGame);
        }

        public void OnExit()
        {
            menu.Hide();
            selectMap.Hide();
        }

        public void Update(float dt)
        {
        }
    }
}
