using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreUI;
using CoreUI.Elements;

namespace DirtyGame.game.Core.GameStates
{
    class GamePlay : IGameState
    {
        private Dirty game;
        private Panel monsterHUD;
        private Label killLbl;
        private Label aliveLbl;

        public GamePlay()
        {
            game = null;
        }

        public void OnEnter(Dirty game)
        {
            this.game = game;
            if (monsterHUD == null)
            {
                monsterHUD = new Panel();
                monsterHUD.Size = new System.Drawing.Point(120, 50);
                monsterHUD.Position = new System.Drawing.Point(680, 0);
                monsterHUD.Background = new CoreUI.DrawEngines.MonoGameColor(Microsoft.Xna.Framework.Color.Black);
                monsterHUD.Visibility = Visibility.Hidden;
                game.UIEngine.Children.AddElement(monsterHUD);

                killLbl = new Label();
                killLbl.Size = new System.Drawing.Point(120, 25);
                killLbl.Position = new System.Drawing.Point(680, 0);
                killLbl.Foreground = new CoreUI.DrawEngines.MonoGameColor(Microsoft.Xna.Framework.Color.White);
                monsterHUD.AddElement(killLbl);

                aliveLbl = new Label();
                aliveLbl.Size = new System.Drawing.Point(120, 25);
                aliveLbl.Position = new System.Drawing.Point(680, 25);
                aliveLbl.Foreground = new CoreUI.DrawEngines.MonoGameColor(Microsoft.Xna.Framework.Color.White);
                monsterHUD.AddElement(aliveLbl);
            }
            monsterHUD.Visibility = Visibility.Visible;
        }

        public void OnExit()
        {
            monsterHUD.Visibility = Visibility.Hidden;
        }

        public void Update(float dt)
        {
            game.world.Update(dt);
            aliveLbl.Text = "Monsters Left: " + game.gLogicSystem.monstersalive;
            killLbl.Text = "Monsters Killed: " + game.gLogicSystem.monstersdefeated;
        }
    }
}
