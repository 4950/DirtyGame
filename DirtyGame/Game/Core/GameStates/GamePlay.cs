using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreUI;
using CoreUI.Elements;
using CoreUI.DrawEngines;
using CoreUI.Visuals;
using EntityFramework;
using DirtyGame.game.Core.Components;


namespace DirtyGame.game.Core.GameStates
{
    class GamePlay : IGameState
    {
        private Dirty game;
        private Panel monsterHUD;
        private Label killLbl;
        private Label aliveLbl;
        private Window playerStuff;
        private Panel windowCont;
        private ImageBrush weaponImg;
        private Label weaponName;
        private Label weaponDamage;
        

        private Entity curWeapon;

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

                playerStuff = new Window();
                playerStuff.Size = new System.Drawing.Point(200, 100);
                playerStuff.Position = new System.Drawing.Point(0, 400);
                playerStuff.Style = Window.WindowStyle.None;
                playerStuff.Background = new MonoGameColor(new Microsoft.Xna.Framework.Color(0, 0, 0, .5f));

                windowCont = new Panel();
                playerStuff.Content = windowCont;

                Panel img = new Panel();
                img.Position = new System.Drawing.Point(10, 10);
                img.Size = new System.Drawing.Point(32, 32);
                windowCont.AddElement(img);

                weaponImg = new ImageBrush();
                weaponImg.SizeMode = SizeMode.Fill;
                weaponImg.Visibility = Visibility.Visible;
                img.BackgroundVisual = weaponImg;

                weaponName = new Label();
                weaponName.Size = new System.Drawing.Point(100, 20);
                weaponName.Position = new System.Drawing.Point(42, 10);
                weaponName.Foreground = new MonoGameColor(Microsoft.Xna.Framework.Color.White);
                windowCont.AddElement(weaponName);

                weaponDamage = new Label();
                weaponDamage.Size = new System.Drawing.Point(100, 20);
                weaponDamage.Position = new System.Drawing.Point(42, 30);
                weaponDamage.Foreground = new MonoGameColor(Microsoft.Xna.Framework.Color.White);
                windowCont.AddElement(weaponDamage);

            }
            monsterHUD.Visibility = Visibility.Visible;
            playerStuff.Show();
        }

        public void OnExit()
        {
            monsterHUD.Visibility = Visibility.Hidden;
            playerStuff.Hide();
        }

        public void Update(float dt)
        {
            game.world.Update(dt);
            aliveLbl.Text = "Monsters Left: " + game.gLogicSystem.monstersalive;
            killLbl.Text = "Monsters Killed: " + game.gLogicSystem.monstersdefeated;

            if (curWeapon != game.player.GetComponent<InventoryComponent>().CurrentWeapon)
            {
                curWeapon = game.player.GetComponent<InventoryComponent>().CurrentWeapon;

                WeaponComponent wc = curWeapon.GetComponent<WeaponComponent>();

                weaponImg.ClearImage();
                weaponImg.LoadImage(curWeapon.GetComponent<WeaponComponent>().Portrait);

                weaponName.Text = wc.Name;
                weaponDamage.Text = "Damage: " + wc.BaseDamage;
            }
        }
    }
}
