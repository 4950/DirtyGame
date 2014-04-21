using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreUI;
using CoreUI.Elements;
using CoreUI.DrawEngines;
using CoreUI.Visuals;
using EntityFramework;
using CleanGame.Game.Core.Components;
using Microsoft.Xna.Framework.Input;
namespace CleanGame.Game.Core.GameStates
{
    class GameMenuState : IGameState
    {
        private Window menuWindow;
        private Dirty game;
        private Panel windowPnl;
        private Button ExitMenuBtn;
        private Button ResumeBtn;
        private Button ExitGameBtn;

        public void OnEnter(Dirty game)
        {
            this.game = game;
            if (menuWindow == null)
            {
                menuWindow = new Window();
                menuWindow.Size = new System.Drawing.Point(200, 160);
                menuWindow.Position = new System.Drawing.Point(game.currrentDisplayMode.Width / 2 - menuWindow.Size.X / 2, game.currrentDisplayMode.Height / 2 - menuWindow.Size.Y / 2);
                menuWindow.Style = Window.WindowStyle.None;

                windowPnl = new Panel();
                windowPnl.Size = new System.Drawing.Point(200, 300);
                windowPnl.Position = new System.Drawing.Point(0, 0);
                menuWindow.Content = windowPnl;

                ResumeBtn = new Button();
                ResumeBtn.Size = new System.Drawing.Point(180, 40);
                ResumeBtn.Position = new System.Drawing.Point(10, 10);
                ResumeBtn.Text = "Resume";
                ResumeBtn.Click += ResumeBtn_Click;
                windowPnl.AddElement(ResumeBtn);

                ExitMenuBtn = new Button();
                ExitMenuBtn.Size = new System.Drawing.Point(180, 40);
                ExitMenuBtn.Position = new System.Drawing.Point(10, 60);
                ExitMenuBtn.Text = "Exit to Menu";
                ExitMenuBtn.Click += ExitMenuBtn_Click;
                windowPnl.AddElement(ExitMenuBtn);

                ExitGameBtn = new Button();
                ExitGameBtn.Size = new System.Drawing.Point(180, 40);
                ExitGameBtn.Position = new System.Drawing.Point(10, 110);
                ExitGameBtn.Text = "Exit to Desktop";
                ExitGameBtn.Click += ExitGameBtn_Click;
                windowPnl.AddElement(ExitGameBtn);
            }
            menuWindow.Show();
        }

        void ExitGameBtn_Click(object sender)
        {
            game.Exit();
        }

        void ExitMenuBtn_Click(object sender)
        {
            game.ResetToMainMenu();
        }

        void ResumeBtn_Click(object sender)
        {
            Events.Event startGame = new Events.Event();
            startGame.name = "GameStateGame";
            EventManager.Instance.TriggerEvent(startGame);
        }

        public void OnExit()
        {
            menuWindow.Hide();
        }

        public void Update(float dt)
        {
        }
    }
}
