using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using CoreUI;
using CoreUI.Elements;
using DirtyGame.game.Core.Events;

namespace DirtyGame.game.Core.GameStates
{
    class BossState : IGameState
    {
        private Dirty game;

        private Panel bossHUD;
        private Label clockLabel;

        private Stopwatch timer;
        private readonly long BOSS_TIME_IN_MILLISECONDS = 3000;

        public BossState()
        {
            game = null;
            timer = new Stopwatch();
        }

        public void OnEnter(Dirty game)
        {
            this.game = game;

            if (bossHUD == null)
            {
                bossHUD = new Panel();
                bossHUD.Size = new System.Drawing.Point(120, 50);
                bossHUD.Position = new System.Drawing.Point(680, 0);
                bossHUD.Background = new CoreUI.DrawEngines.MonoGameColor(Microsoft.Xna.Framework.Color.Black);
                bossHUD.Visibility = Visibility.Hidden;
                game.UIEngine.Children.AddElement(bossHUD);

                clockLabel = new Label();
                clockLabel.Size = new System.Drawing.Point(120, 25);
                clockLabel.Position = new System.Drawing.Point(680, 50);
                clockLabel.Foreground = new CoreUI.DrawEngines.MonoGameColor(Microsoft.Xna.Framework.Color.White);
                bossHUD.AddElement(clockLabel);
            }
            bossHUD.Visibility = Visibility.Visible;

            MessageBox.Show("Boss Time!");
            timer.Start();
        }

        public void OnExit()
        {
            bossHUD.Visibility = Visibility.Hidden;
        }

        public void Update(float dt)
        {
            clockLabel.Text = "Time remaining: " + Math.Ceiling((BOSS_TIME_IN_MILLISECONDS - timer.ElapsedMilliseconds) / 1000.0);

            long elapsedTime = timer.ElapsedMilliseconds;
            if (elapsedTime >= BOSS_TIME_IN_MILLISECONDS)
            {
                timer.Stop();

                Event purchaseStart = new Event();
                purchaseStart.name = "GameStatePurchase";
                EventManager.Instance.TriggerEvent(purchaseStart);
            }
        }

    }
}
