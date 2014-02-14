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
    class BuyState : IGameState
    {
        private Panel BuyPanel;
        private Label TitleLbl;
        private Button DoneBtn;
        private Listbox BuyList;
        private Dirty game;
        private Label AmtLbl;
        private Button PlusBtn;
        private Button SubBtn;
        private int amount = 0;
        private int index = 0;
        private Label CostLbl;
        private Button BuyBtn;
        private Label CashLbl;
        private List<WeaponComponent> saleList = new List<WeaponComponent>();

        public void OnEnter(Dirty game)
        {
            this.game = game;
            if (BuyPanel == null)
            {
                BuyPanel = new Panel();
                BuyPanel.Size = new System.Drawing.Point(200, 225);
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
                DoneBtn.Position = new System.Drawing.Point(250, 300);
                DoneBtn.Text = "Done";
                DoneBtn.Click += DoneBtn_Click;
                BuyPanel.AddElement(DoneBtn);

                BuyList = new Listbox();
                BuyList.Size = new System.Drawing.Point(200, 150);
                BuyList.Position = new System.Drawing.Point(100, 125);
                BuyList.SelectedIndexChanged += BuyList_SelectedIndexChanged;
                BuyPanel.AddElement(BuyList);

                AmtLbl = new Label();
                AmtLbl.Size = new System.Drawing.Point(75, 25);
                AmtLbl.Position = new System.Drawing.Point(125, 275);
                AmtLbl.Foreground = new CoreUI.DrawEngines.MonoGameColor(Microsoft.Xna.Framework.Color.White);
                AmtLbl.Text = "Amount: 0";
                BuyPanel.AddElement(AmtLbl);

                SubBtn = new Button();
                SubBtn.Size = new System.Drawing.Point(25, 25);
                SubBtn.Position = new System.Drawing.Point(100, 275);
                SubBtn.Text = "-";
                SubBtn.Click += SubBtn_Click;
                BuyPanel.AddElement(SubBtn);

                PlusBtn = new Button();
                PlusBtn.Size = new System.Drawing.Point(25, 25);
                PlusBtn.Position = new System.Drawing.Point(200, 275);
                PlusBtn.Text = "+";
                PlusBtn.Click += PlusBtn_Click;
                BuyPanel.AddElement(PlusBtn);

                CostLbl = new Label();
                CostLbl.Size = new System.Drawing.Point(75, 25);
                CostLbl.Position = new System.Drawing.Point(225, 275);
                CostLbl.Foreground = new CoreUI.DrawEngines.MonoGameColor(Microsoft.Xna.Framework.Color.White);
                CostLbl.Text = "Cost: $0";
                CostLbl.TextPosition = TextPosition.Center;
                BuyPanel.AddElement(CostLbl);

                BuyBtn = new Button();
                BuyBtn.Size = new System.Drawing.Point(50, 25);
                BuyBtn.Position = new System.Drawing.Point(100, 300);
                BuyBtn.Text = "Buy!";
                BuyBtn.Click += BuyBtn_Click;
                BuyPanel.AddElement(BuyBtn);

                CashLbl = new Label();
                CashLbl.Size = new System.Drawing.Point(75, 25);
                CashLbl.Position = new System.Drawing.Point(150, 300);
                CashLbl.Foreground = new CoreUI.DrawEngines.MonoGameColor(Microsoft.Xna.Framework.Color.White);
                CashLbl.Text = "Cash: $0";
                CashLbl.TextPosition = TextPosition.Center;
                BuyPanel.AddElement(CashLbl);
            }
            
            PopulateList();

            CashLbl.Text = "Cash: $" + game.gameEntity.entity.GetComponent<PropertyComponent<int>>("GameCash").value;
            BuyPanel.Visibility = Visibility.Visible;
        }

        void BuyBtn_Click(object sender)
        {
            if (index >= 0 && index < saleList.Count && amount > 0)
            {
                WeaponComponent wc = saleList[index];

                float cash = game.gameEntity.entity.GetComponent<PropertyComponent<int>>("GameCash").value;
                float price = amount * wc.AmmoPrice;

                if (price > cash)
                {
                    amount = (int)Math.Floor(cash / wc.AmmoPrice);
                    price = amount * wc.AmmoPrice;
                }
                wc.Ammo += amount;
                game.gameEntity.entity.GetComponent<PropertyComponent<int>>("GameCash").value -= (int)price;

                CashLbl.Text = "Cash: $" + game.gameEntity.entity.GetComponent<PropertyComponent<int>>("GameCash").value;
            }
            PopulateList();
            updateLabels();
        }

        void SubBtn_Click(object sender)
        {
            amount--;
            updateLabels();
        }

        void PlusBtn_Click(object sender)
        {
            amount++;
            updateLabels();
        }
        private void updateLabels()
        {
            if (index >= 0 && index < saleList.Count)
            {
                WeaponComponent wc = saleList[index];
                if (amount < 0) amount = 0;
                if (amount > (wc.MaxAmmo - wc.Ammo)) amount = (wc.MaxAmmo - wc.Ammo);

                AmtLbl.Text = "Amount: " + amount;
                CostLbl.Text = "Cost: $" + amount * wc.AmmoPrice;
            }
            else
            {
                AmtLbl.Text = "Amount: 0";
                CostLbl.Text = "Cost: $0";
            }
        }
        void BuyList_SelectedIndexChanged(object sender)
        {
            if (BuyList.SelectedIndex != -1)
            {
                index = BuyList.SelectedIndex;
                WeaponComponent wc = saleList[index];
                amount = (wc.MaxAmmo - wc.Ammo);
                updateLabels();
            }
        }
        private void PopulateList()
        {
            index = 0;
            BuyList.RemoveAllItems();
            saleList.Clear();

            foreach (Entity weapon in game.player.GetComponent<InventoryComponent>().WeaponList)
            {
                WeaponComponent wc = weapon.GetComponent<WeaponComponent>();

                if (wc.MaxAmmo != -1)
                {
                    string txt = wc.WeaponName + " Ammo   " + wc.Ammo + "/" + wc.MaxAmmo + "   $" + wc.AmmoPrice + "ea";
                    BuyList.AddItem(txt);
                    saleList.Add(wc);
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
