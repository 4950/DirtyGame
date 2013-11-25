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
    class PurchaseState : IGameState
    {
        private Dirty game;

        private Panel purchaseHUD;
        private Label clockLabel;

        private Stopwatch timer;
        private readonly long PURCHASE_TIME_IN_MILLISECONDS = 3000;

        public PurchaseState()
        {
            game = null;
            timer = new Stopwatch();
        }

        public void OnEnter(Dirty game)
        {
            this.game = game;

            if (purchaseHUD == null)
            {
                purchaseHUD = new Panel();
                purchaseHUD.Size = new System.Drawing.Point(120, 50);
                purchaseHUD.Position = new System.Drawing.Point(680, 0);
                purchaseHUD.Background = new CoreUI.DrawEngines.MonoGameColor(Microsoft.Xna.Framework.Color.Black);
                purchaseHUD.Visibility = Visibility.Hidden;
                game.UIEngine.Children.AddElement(purchaseHUD);

                clockLabel = new Label();
                clockLabel.Size = new System.Drawing.Point(120, 25);
                clockLabel.Position = new System.Drawing.Point(680, 50);
                clockLabel.Foreground = new CoreUI.DrawEngines.MonoGameColor(Microsoft.Xna.Framework.Color.White);
                purchaseHUD.AddElement(clockLabel);
            }
            purchaseHUD.Visibility = Visibility.Visible;

            MessageBox.Show("Purchase Time!");
            timer.Start();
        }

        public void OnExit()
        {
            purchaseHUD.Visibility = Visibility.Hidden;
        }

        public void Update(float dt)
        {
            clockLabel.Text = "Time remaining: " + Math.Ceiling((PURCHASE_TIME_IN_MILLISECONDS - timer.ElapsedMilliseconds) / 1000.0);

            long elapsedTime = timer.ElapsedMilliseconds;
            if (elapsedTime >= PURCHASE_TIME_IN_MILLISECONDS)
            {
                timer.Stop();

                Event combatStart = new Event();
                combatStart.name = "GameStateGame";
                EventManager.Instance.TriggerEvent(combatStart);
            }
        }
    }
}
