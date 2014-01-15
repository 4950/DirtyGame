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
    class BuyState : IGameState
    {
        private Panel BuyPanel;
        private Label TitleLbl;
        private Button DoneBtn;
        private Listbox BuyList;
        private Dirty game;

        public void OnEnter(Dirty game)
        {
            this.game = game;
            if (BuyPanel == null)
            {
                BuyPanel = new Panel();
                BuyPanel.Size = new System.Drawing.Point(200, 200);
                BuyPanel.Position = new System.Drawing.Point(100, 100);
                BuyPanel.Background = new CoreUI.DrawEngines.MonoGameColor(Microsoft.Xna.Framework.Color.Black);
                BuyPanel.Visibility = Visibility.Hidden;
                game.UIEngine.Children.AddElement(BuyPanel);

                TitleLbl = new Label();
                TitleLbl.Size = new System.Drawing.Point(200, 25);
                TitleLbl.Position = new System.Drawing.Point(100, 100);
                TitleLbl.Foreground = new CoreUI.DrawEngines.MonoGameColor(Microsoft.Xna.Framework.Color.White);
                TitleLbl.Text = "Buy Your Shit Here";
                TitleLbl.TextPosition = TextPosition.Center;
                BuyPanel.AddElement(TitleLbl);

                DoneBtn = new Button();
                DoneBtn.Size = new System.Drawing.Point(50, 25);
                DoneBtn.Position = new System.Drawing.Point(250, 275);
                DoneBtn.Text = "Done";
                DoneBtn.Click += DoneBtn_Click;
                BuyPanel.AddElement(DoneBtn);

                BuyList = new Listbox();
                BuyList.Size = new System.Drawing.Point(200, 150);
                BuyList.Position = new System.Drawing.Point(100, 125);
                BuyPanel.AddElement(BuyList);
            }

            BuyList.RemoveAllItems();
            PopulateList();


            BuyPanel.Visibility = Visibility.Visible;
        }
        private void PopulateList()
        {
            foreach (Entity weapon in game.player.GetComponent<InventoryComponent>().WeaponList)
            {
                WeaponComponent wc = weapon.GetComponent<WeaponComponent>();

                if (wc.MaxAmmo != -1)
                {
                    string txt = wc.Name + " Ammo   " + wc.Ammo + "/" + wc.MaxAmmo + "   $" + wc.AmmoPrice + "ea";
                    BuyList.AddItem(txt);
                }
            }
        }

        void DoneBtn_Click(object sender)
        {
            Events.Event startGame = new Events.Event();
            startGame.name = "GameStateGame";
            EventManager.Instance.TriggerEvent(startGame);
        }

        public void OnExit()
        {
            BuyPanel.Visibility = Visibility.Hidden;
        }

        public void Update(float dt)
        {
        }
    }
}
