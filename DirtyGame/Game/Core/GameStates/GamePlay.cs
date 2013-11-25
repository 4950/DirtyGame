using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using CoreUI;
using CoreUI.Elements;
using CoreUI.DrawEngines;
using CoreUI.Visuals;
using EntityFramework;
using DirtyGame.game.Core.Events;
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
        private ProgressBar weaponAmmo;
        private Label weaponAmmoLabel;

        private Label clockLabel;
        private Stopwatch timer;
        private readonly long WAVE_TIME_IN_MILLISECONDS = 3000;    // 10 sec
        private readonly int WAVES_PER_ROUND = 5;

        private Label waveLabel;

        private Entity curWeapon;

        private static int currentWave = 0;

        public GamePlay()
        {
            game = null;
            timer = new Stopwatch();
        }

        public void OnEnter(Dirty game)
        {
            //curWeapon = game.player.GetComponent<InventoryComponent>().CurrentWeapon;
            this.game = game;
            timer.Start();
            currentWave++;

            if (currentWave > WAVES_PER_ROUND)
            {
                currentWave = 0;

                Event startBossWave = new Event();
                startBossWave.name = "GameStateBoss";
                EventManager.Instance.TriggerEvent(startBossWave);
            }

            if (monsterHUD == null)
            {
                monsterHUD = new Panel();
                monsterHUD.Size = new System.Drawing.Point(120, 50);
                monsterHUD.Position = new System.Drawing.Point(680, 0);
                monsterHUD.Background = new CoreUI.DrawEngines.MonoGameColor(Microsoft.Xna.Framework.Color.Black);
                monsterHUD.Visibility = Visibility.Hidden;
                game.UIEngine.Children.AddElement(monsterHUD);

                clockLabel = new Label();
                clockLabel.Size = new System.Drawing.Point(120, 25);
                clockLabel.Position = new System.Drawing.Point(680, 50);
                clockLabel.Foreground = new CoreUI.DrawEngines.MonoGameColor(Microsoft.Xna.Framework.Color.White);
                monsterHUD.AddElement(clockLabel);

                waveLabel = new Label();
                waveLabel.Size = new System.Drawing.Point(120, 25);
                waveLabel.Position = new System.Drawing.Point(400, 0);
                waveLabel.Foreground = new CoreUI.DrawEngines.MonoGameColor(Microsoft.Xna.Framework.Color.White);
                monsterHUD.AddElement(waveLabel);

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
            if (monsterHUD != null)
            {
                monsterHUD.Visibility = Visibility.Hidden;
            }
            if (playerStuff != null)
            {
                playerStuff.Hide();
            }
        }

        public void Update(float dt)
        {
            game.world.Update(dt);
            clockLabel.Text = "Time remaining: " + Math.Ceiling((WAVE_TIME_IN_MILLISECONDS - timer.ElapsedMilliseconds) / 1000.0);
            waveLabel.Text = "Wave: " + currentWave;
            aliveLbl.Text = "Monsters Left: " + game.gLogicSystem.monstersalive;
            killLbl.Text = "Monsters Killed: " + game.gLogicSystem.monstersdefeated;

            long elapsedTime = timer.ElapsedMilliseconds;
            if (elapsedTime >= WAVE_TIME_IN_MILLISECONDS)
            {
                timer.Stop();

                Event purchaseStart = new Event();
                purchaseStart.name = "GameStatePurchase";
                EventManager.Instance.TriggerEvent(purchaseStart);
            }

            WeaponComponent wc = curWeapon == null ? null : curWeapon.GetComponent<WeaponComponent>();
            if (curWeapon != game.player.GetComponent<InventoryComponent>().CurrentWeapon)
            {
                curWeapon = game.player.GetComponent<InventoryComponent>().CurrentWeapon;

                wc = curWeapon.GetComponent<WeaponComponent>();

                weaponImg.ClearImage();
                weaponImg.LoadImage(curWeapon.GetComponent<WeaponComponent>().Portrait);

                weaponName.Text = wc.Name;
                weaponDamage.Text = "Damage: " + wc.BaseDamage;

                if (wc.MaxAmmo == -1)
                {
                    weaponAmmo.Value = 1;
                    weaponAmmo.Maximum = 1;
                    weaponAmmoLabel.Text = "infinite";

                }

            }

            if (curWeapon.GetComponent<WeaponComponent>().Ammo != weaponAmmo.Value && curWeapon.GetComponent<WeaponComponent>().MaxAmmo != -1)
            {
                weaponAmmo.Maximum = wc.MaxAmmo;
                weaponAmmo.Value = wc.Ammo;
                weaponAmmoLabel.Text = wc.Ammo + "/" + wc.MaxAmmo;

            }
        }
    }
}
