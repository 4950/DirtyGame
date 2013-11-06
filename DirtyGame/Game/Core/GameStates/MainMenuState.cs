using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreUI;
using CoreUI.Elements;

namespace DirtyGame.game.Core.GameStates
{
    class MainMenuState : IGameState
    {
        private Dirty game;
        private Window menu;

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

                CoreUI.Elements.Button b2 = new CoreUI.Elements.Button();
                b2.Position = new System.Drawing.Point(10, 10);
                b2.Size = new System.Drawing.Point(280, 50);
                b2.Text = "New Game";
                b2.Click += startGame;
                p.AddElement(b2);

                CoreUI.Elements.Button b3 = new CoreUI.Elements.Button();
                b3.Position = new System.Drawing.Point(10, 70);
                b3.Size = new System.Drawing.Point(280, 50);
                b3.Text = "Options";
                p.AddElement(b3);

                CoreUI.Elements.Button b4 = new CoreUI.Elements.Button();
                b4.Position = new System.Drawing.Point(10, 190);
                b4.Size = new System.Drawing.Point(280, 50);
                b4.Text = "Exit";
                p.AddElement(b4);
            }

            menu.ShowDialog();
        }

        void startGame(object sender)
        {
            Events.Event startGame = new Events.Event();
            startGame.name = "GameStateGame";
            EventManager.Instance.TriggerEvent(startGame);
        }

        public void OnExit()
        {
            menu.Hide();
        }

        public void Update(float dt)
        {
        }
    }
}
