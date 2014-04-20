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
using CleanGame.Game.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameService;


namespace CleanGame.Game.Core.GameStates
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
        private Label curScenario;

        private EntityRef curWeapon;

        public GamePlay()
        {
            game = null;
        }
        private void SaveGame(Keys key)
        {
            game.world.EntityMgr.SerializeEntities(App.Path + "..\\quicksave.xml");
        }
        private void LoadGame(Keys key)
        {
            game.world.EntityMgr.RemoveAllEntities();
            game.world.EntityMgr.DeserializeEntities(App.Path + "..\\quicksave.xml");

        }
        private void ShowMenu(Keys key)
        {
            Events.Event startGame = new Events.Event();
            startGame.name = "GameStateGameMenu";
            EventManager.Instance.TriggerEvent(startGame);
        }
        public void OnEnter(Dirty game)
        {


            game.baseContext.RegisterHandler(Microsoft.Xna.Framework.Input.Keys.F5, SaveGame, null);
            game.baseContext.RegisterHandler(Microsoft.Xna.Framework.Input.Keys.F6, LoadGame, null);
            game.baseContext.RegisterHandler(Keys.Escape, ShowMenu, null);

            //curWeapon = game.player.GetComponent<InventoryComponent>().CurrentWeapon;
            this.game = game;
            if (monsterHUD == null)
            {
                monsterHUD = new Panel();
                monsterHUD.Size = new System.Drawing.Point(150, 145);
                monsterHUD.Position = new System.Drawing.Point(680, 0);
                //monsterHUD.Background = new CoreUI.DrawEngines.MonoGameColor(Microsoft.Xna.Framework.Color.Black);
                monsterHUD.Visibility = Visibility.Hidden;

                SpriteFont f = game.resourceManager.GetResource<SpriteFont>("HitsSmall");
                

                scoreLbl = new Label();
                scoreLbl.Size = new System.Drawing.Point(190, 25);
                scoreLbl.Position = new System.Drawing.Point(590, 0);
                scoreLbl.Foreground = new CoreUI.DrawEngines.MonoGameColor(Microsoft.Xna.Framework.Color.White);
                scoreLbl.TextPosition = TextPosition.Right;
                scoreLbl.mFontInt = new MonoGameFont(f);
                scoreLbl.Text = "Score: 0";
                monsterHUD.AddElement(scoreLbl);

                cashLbl = new Label();
                cashLbl.Size = new System.Drawing.Point(120, 25);
                cashLbl.Position = new System.Drawing.Point(680, 25);
                cashLbl.Foreground = new CoreUI.DrawEngines.MonoGameColor(Microsoft.Xna.Framework.Color.White);
                cashLbl.Text = "Cash: 0";
                cashLbl.Visibility = Visibility.Hidden;
                monsterHUD.AddElement(cashLbl);

                roundLbl = new Label();
                roundLbl.Size = new System.Drawing.Point(190, 25);
                roundLbl.Position = new System.Drawing.Point(590, 50);
                roundLbl.Foreground = new CoreUI.DrawEngines.MonoGameColor(Microsoft.Xna.Framework.Color.White);
                roundLbl.TextPosition = TextPosition.Right;
                roundLbl.mFontInt = new MonoGameFont(f);
                roundLbl.Text = "Round: 0";
                monsterHUD.AddElement(roundLbl);

                killLbl = new Label();
                killLbl.Size = new System.Drawing.Point(190, 25);
                killLbl.Position = new System.Drawing.Point(590, 25);
                killLbl.Foreground = new CoreUI.DrawEngines.MonoGameColor(Microsoft.Xna.Framework.Color.White);
                killLbl.TextPosition = TextPosition.Right;
                killLbl.mFontInt = new MonoGameFont(f);
                killLbl.Text = "Kills: 0";
                monsterHUD.AddElement(killLbl);

                aliveLbl = new Label();
                aliveLbl.Size = new System.Drawing.Point(120, 25);
                aliveLbl.Position = new System.Drawing.Point(680, 120);
                aliveLbl.Foreground = new CoreUI.DrawEngines.MonoGameColor(Microsoft.Xna.Framework.Color.White);
                aliveLbl.Visibility = Visibility.Hidden;
                monsterHUD.AddElement(aliveLbl);

                playerStuff = new Window();
                playerStuff.Size = new System.Drawing.Point(200, 100);
                playerStuff.Position = new System.Drawing.Point(0, game.currrentDisplayMode.Height - 100);
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

                curScenario = new Label();
                curScenario.Size = new System.Drawing.Point(200, 20);
                curScenario.Position = new System.Drawing.Point(10, 70);
                curScenario.Foreground = new MonoGameColor(Microsoft.Xna.Framework.Color.White);
                curScenario.TextPosition = TextPosition.Left;
                windowCont.AddElement(curScenario);

            }
            if (monsterHUD.Parent == null)
                game.UIEngine.Children.AddElement(monsterHUD);
            monsterHUD.Visibility = Visibility.Visible;
            playerStuff.Show();
        }

        public void OnExit()
        {
            monsterHUD.Visibility = Visibility.Hidden;
            playerStuff.Hide();
            game.baseContext.UnregisterHandlers(Microsoft.Xna.Framework.Input.Keys.F5);
            game.baseContext.UnregisterHandlers(Microsoft.Xna.Framework.Input.Keys.F6);
            game.baseContext.UnregisterHandlers(Keys.Escape);
        }

        public void Update(float dt)
        {
            game.world.Update(dt);
            game.physics.Update(dt);

            if (!game.graphics.IsFullScreen && Settings.Instance.Global.DefaultUser.CaptureMouse)//mouse locking
            {
                Vector2 pos = new Vector2(game.mouseState.X, game.mouseState.Y);
                Vector2 pos2 = pos;
                if (pos.X < 0)
                    pos2.X = 0;
                else if (pos.X > game.renderer.ActiveCamera.size.X)
                    pos2.X = game.renderer.ActiveCamera.size.X;
                if (pos.Y < 0)
                    pos2.Y = 0;
                else if (pos.Y > game.renderer.ActiveCamera.size.Y)
                    pos2.Y = game.renderer.ActiveCamera.size.Y;
                if (pos2 != pos)
                    Mouse.SetPosition((int)pos2.X, (int)pos2.Y);
            }

            aliveLbl.Text = "Monsters Left: " + game.gLogicSystem.monstersalive;
            

            if (game.gameEntity.entity.GetComponent<PropertyComponent<int>>("GameKills").IsModified)
                killLbl.Text = "Kills: " + game.gameEntity.entity.GetComponent<PropertyComponent<int>>("GameKills").value;

            if (game.gameEntity.entity.GetComponent<PropertyComponent<int>>("GameRound").IsModified)
            {
                roundLbl.Text = "Round: " + game.gameEntity.entity.GetComponent<PropertyComponent<int>>("GameRound").value;
#if DEBUG
                curScenario.Text = GameplayDataCaptureSystem.Instance.SessionID.ToString();
#endif
            }
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

                weaponName.Text = wc.WeaponName;
                weaponDamage.Text = "Damage: " + wc.BaseDamage;

                if (wc.MaxAmmo == -1)
                {
                    weaponAmmo.Value = 1;
                    weaponAmmo.Maximum = 1;
                    weaponAmmoLabel.Text = "infinite";

                }

            }
            if (wc.MaxAmmo == -1)//no ammo, show cooldown
            {
                int val = (int)((wc.Cooldown - wc.LastFire) / wc.Cooldown * 100);
                if (val != weaponAmmo.Value)
                {
                    weaponAmmo.Maximum = 100;
                    weaponAmmo.Value = val;
                    if (val < 100)
                        weaponAmmoLabel.Text = "Reloading";
                    else
                        weaponAmmoLabel.Text = "Ready";
                }
            }
            else if (wc.Ammo != weaponAmmo.Value)
            {
                weaponAmmo.Maximum = wc.MaxAmmo;
                weaponAmmo.Value = wc.Ammo;
                weaponAmmoLabel.Text = wc.Ammo + "/" + wc.MaxAmmo;

            }
        }
    }
}
