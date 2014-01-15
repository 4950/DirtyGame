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
using Microsoft.Xna.Framework.Input;


namespace DirtyGame.game.Core.GameStates
{
    class GamePlay : IGameState
    {
        private Dirty game;
        private Panel monsterHUD;
        private Label killLbl;
        private Label aliveLbl;
        private Label roundLbl;
        private Label scoreLbl;
        private Label cashLbl;
        private Window playerStuff;
        private Panel windowCont;
        private ImageBrush weaponImg;
        private Label weaponName;
        private Label weaponDamage;
        private ProgressBar weaponAmmo;
        private Label weaponAmmoLabel;

        private EntityRef curWeapon;

        public GamePlay()
        {
            game = null;
        }
        private void SaveGame(Keys key)
        {
            game.world.EntityMgr.SerializeEntities(App.Path + "test.xml");
        }
        private void LoadGame(Keys key)
        {
            game.world.EntityMgr.RemoveAllEntities();
            game.world.EntityMgr.DeserializeEntities(App.Path + "test.xml");

        }
        public void OnEnter(Dirty game)
        {
            game.baseContext.RegisterHandler(Microsoft.Xna.Framework.Input.Keys.F5, SaveGame, null);
            game.baseContext.RegisterHandler(Microsoft.Xna.Framework.Input.Keys.F6, LoadGame, null);

            //curWeapon = game.player.GetComponent<InventoryComponent>().CurrentWeapon;
            this.game = game;
            if (monsterHUD == null)
            {
                monsterHUD = new Panel();
                monsterHUD.Size = new System.Drawing.Point(120, 145);
                monsterHUD.Position = new System.Drawing.Point(680, 0);
                monsterHUD.Background = new CoreUI.DrawEngines.MonoGameColor(Microsoft.Xna.Framework.Color.Black);
                monsterHUD.Visibility = Visibility.Hidden;
                game.UIEngine.Children.AddElement(monsterHUD);

                scoreLbl = new Label();
                scoreLbl.Size = new System.Drawing.Point(120, 25);
                scoreLbl.Position = new System.Drawing.Point(680, 0);
                scoreLbl.Foreground = new CoreUI.DrawEngines.MonoGameColor(Microsoft.Xna.Framework.Color.White);
                scoreLbl.Text = "Score: 0";
                monsterHUD.AddElement(scoreLbl);

                cashLbl = new Label();
                cashLbl.Size = new System.Drawing.Point(120, 25);
                cashLbl.Position = new System.Drawing.Point(680, 25);
                cashLbl.Foreground = new CoreUI.DrawEngines.MonoGameColor(Microsoft.Xna.Framework.Color.White);
                cashLbl.Text = "Cash: 0";
                monsterHUD.AddElement(cashLbl);

                roundLbl = new Label();
                roundLbl.Size = new System.Drawing.Point(120, 25);
                roundLbl.Position = new System.Drawing.Point(680, 70);
                roundLbl.Foreground = new CoreUI.DrawEngines.MonoGameColor(Microsoft.Xna.Framework.Color.White);
                roundLbl.Text = "Round: 1";
                monsterHUD.AddElement(roundLbl);

                killLbl = new Label();
                killLbl.Size = new System.Drawing.Point(120, 25);
                killLbl.Position = new System.Drawing.Point(680, 95);
                killLbl.Foreground = new CoreUI.DrawEngines.MonoGameColor(Microsoft.Xna.Framework.Color.White);
                monsterHUD.AddElement(killLbl);

                aliveLbl = new Label();
                aliveLbl.Size = new System.Drawing.Point(120, 25);
                aliveLbl.Position = new System.Drawing.Point(680, 120);
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

                weaponAmmo = new ProgressBar();
                weaponAmmo.Size = new System.Drawing.Point(100, 20);
                weaponAmmo.Position = new System.Drawing.Point(42, 50);
                //weaponAmmo.Foreground = new MonoGameColor(Microsoft.Xna.Framework.Color.Black);
                windowCont.AddElement(weaponAmmo);

                weaponAmmoLabel = new Label();
                weaponAmmoLabel.Size = new System.Drawing.Point(100, 20);
                weaponAmmoLabel.Position = new System.Drawing.Point(42, 50);
                weaponAmmoLabel.Foreground = new MonoGameColor(Microsoft.Xna.Framework.Color.Black);
                weaponAmmoLabel.TextPosition = TextPosition.Center;
                windowCont.AddElement(weaponAmmoLabel);

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

            if (game.gameEntity.entity.GetComponent<PropertyComponent<int>>("GameRound").IsModified)
                roundLbl.Text = "Round: " + game.gameEntity.entity.GetComponent<PropertyComponent<int>>("GameRound").value;

            if (game.gameEntity.entity.GetComponent<PropertyComponent<int>>("GameCash").IsModified)
                cashLbl.Text = "Cash: " + game.gameEntity.entity.GetComponent<PropertyComponent<int>>("GameCash").value;

            if (game.gameEntity.entity.GetComponent<PropertyComponent<int>>("GameScore").IsModified)
                scoreLbl.Text = "Score: " + game.gameEntity.entity.GetComponent<PropertyComponent<int>>("GameScore").value;

            WeaponComponent wc = curWeapon == null || curWeapon.entity == null ? null : curWeapon.entity.GetComponent<WeaponComponent>();
            if (curWeapon == null || curWeapon.entity == null || curWeapon.entity != game.player.GetComponent<InventoryComponent>().CurrentWeapon)
            {
                curWeapon = game.player.GetComponent<InventoryComponent>().CurrentWeapon.reference;

                wc = curWeapon.entity.GetComponent<WeaponComponent>();

                weaponImg.ClearImage();
                weaponImg.LoadImage(curWeapon.entity.GetComponent<WeaponComponent>().Portrait);

                weaponName.Text = wc.Name;
                weaponDamage.Text = "Damage: " + wc.BaseDamage;

                if (wc.MaxAmmo == -1)
                {
                    weaponAmmo.Value = 1;
                    weaponAmmo.Maximum = 1;
                    weaponAmmoLabel.Text = "infinite";

                }

            }

            if (curWeapon.entity.GetComponent<WeaponComponent>().Ammo != weaponAmmo.Value && curWeapon.entity.GetComponent<WeaponComponent>().MaxAmmo != -1)
            {
                weaponAmmo.Maximum = wc.MaxAmmo;
                weaponAmmo.Value = wc.Ammo;
                weaponAmmoLabel.Text = wc.Ammo + "/" + wc.MaxAmmo;

            }
        }
    }
}
