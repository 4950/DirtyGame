using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CleanGame.Game.Core.Components;
using CleanGame.Game.Core.Components.Render;
using CleanGame.Game.Core.Systems.Monster;
using CleanGame.Game.Core.Systems.Util;
using CleanGame.Game.Core.Events;
using Microsoft.Xna.Framework;
using EntityFramework.Systems;
using EntityFramework;
using CleanGame.Game.SGraphics;
using CoreUI.Elements;
using CoreUI.DrawEngines;
using CleanGame.Game.Util;
using System.Xml;
using CleanGame.Game.Core.GameStates;
using CoreUI;
using Microsoft.Xna.Framework.Graphics;
using CleanGame.Game.Input;
using Microsoft.Xna.Framework.Input;

namespace CleanGame.Game.Core.Systems
{
    enum TutorialState
    {
        Start = 0,
        Welcome,
        Move,
        Cycle,
        Switch,
        Fire,
        End
    };
    class TutorialSystem : EntitySystem
    {
        private Dirty game;
        private InputContext ctx;
        private Label tutorialLbl;
        private Panel tutorialPnl;
        private TutorialState currentState = TutorialState.Start;
        private bool stateTransitioning;
        private float transitionTimer;
        private float stateEndTimer;
        private bool stateEnded;
        private MouseState prevMS;

        public TutorialSystem(Dirty game)
            : base(SystemDescriptions.TutorialSystem.Aspect, SystemDescriptions.TutorialSystem.Priority)
        {
            this.game = game;
            ctx = new InputContext();
            game.inputManager.AddInputContext(ctx);

            tutorialPnl = new Panel();
            tutorialPnl.Size = new System.Drawing.Point(game.currrentDisplayMode.Width, 100);
            tutorialPnl.Position = new System.Drawing.Point(0, 0);
            Color trans = Microsoft.Xna.Framework.Color.Black;
            trans.A = 200;
            tutorialPnl.Background = new MonoGameColor(trans);
            tutorialPnl.Visibility = CoreUI.Visibility.Visible;
            game.UIEngine.Children.AddElement(tutorialPnl);

            tutorialLbl = new Label();
            tutorialLbl.mFontInt = new MonoGameFont(game.resourceManager.GetResource<SpriteFont>("Tutorial"));
            tutorialLbl.Size = new System.Drawing.Point(game.currrentDisplayMode.Width, 100);
            tutorialLbl.Position = new System.Drawing.Point(0, 0);
            tutorialLbl.Foreground = new MonoGameColor(Microsoft.Xna.Framework.Color.White);
            tutorialLbl.TextPosition = TextPosition.Center;
            tutorialLbl.Visibility = CoreUI.Visibility.Visible;
            tutorialPnl.AddElement(tutorialLbl);
        }
        public override void Shutdown()
        {
            base.Shutdown();
            game.inputManager.RemoveInputContext(ctx);
        }
        private void NextState()
        {
            stateEnded = false;
            stateTransitioning = false;
            transitionTimer = 0;
            tutorialLbl.Visibility = Visibility.Visible;
            tutorialLbl.Text = "";
            currentState++;

            switch (currentState)
            {
                case TutorialState.Welcome:
                    tutorialLbl.Text = "Welcome to Tower Offense!";
                    stateEndTimer = 3;
                    break;
                case TutorialState.Move:
                    tutorialLbl.Text = "Use [WASD] or arrow keys to move";
                    ctx.RegisterHandler(Keys.Left, EndState, null);
                    ctx.RegisterHandler(Keys.Right, EndState, null);
                    ctx.RegisterHandler(Keys.Up, EndState, null);
                    ctx.RegisterHandler(Keys.Down, EndState, null);
                    ctx.RegisterHandler(Keys.U, EndState, null);
                    ctx.RegisterHandler(Keys.H, EndState, null);
                    ctx.RegisterHandler(Keys.J, EndState, null);
                    ctx.RegisterHandler(Keys.K, EndState, null);
                    ctx.RegisterHandler(Keys.W, EndState, null);
                    ctx.RegisterHandler(Keys.A, EndState, null);
                    ctx.RegisterHandler(Keys.S, EndState, null);
                    ctx.RegisterHandler(Keys.D, EndState, null);
                    break;
                case TutorialState.Cycle:
                    tutorialLbl.Text = "Cycle weapons with [Q]/[E] or [Tab]";
                    ctx.RegisterHandler(Keys.Tab, EndState, null);
                    ctx.RegisterHandler(Keys.Q, EndState, null);
                    ctx.RegisterHandler(Keys.E, EndState, null);

                    break;
                case TutorialState.Switch:
                    tutorialLbl.Text = "Select a specific weapon with number keys [0] - [9]";
                    for (int i = 0; i <= 9; i++)
                    {
                        Keys k = (Keys)Enum.Parse(typeof(Keys), "D" + i);
                        ctx.RegisterHandler(k, EndState, null);
                    }
                    break;
                case TutorialState.Fire:
                    tutorialLbl.Text = "Press [Space] or use mouse buttorns to fire your weapon!";
                    ctx.RegisterHandler(Keys.Space, EndState, null);
                    break;
                case TutorialState.End:
                    tutorialLbl.Text = "You have completed the tutorial. Press [Esc] end";
                    break;
            }
        }
        private void EndState(Microsoft.Xna.Framework.Input.Keys key)
        {
            stateEnded = true;

            ctx.UnregisterHandlers(Keys.Left);
            ctx.UnregisterHandlers(Keys.Right);
            ctx.UnregisterHandlers(Keys.Up);
            ctx.UnregisterHandlers(Keys.Down);
            ctx.UnregisterHandlers(Keys.U);
            ctx.UnregisterHandlers(Keys.H);
            ctx.UnregisterHandlers(Keys.J);
            ctx.UnregisterHandlers(Keys.K);
            ctx.UnregisterHandlers(Keys.W);
            ctx.UnregisterHandlers(Keys.A);
            ctx.UnregisterHandlers(Keys.S);
            ctx.UnregisterHandlers(Keys.D);

            ctx.UnregisterHandlers(Keys.Tab);
            ctx.UnregisterHandlers(Keys.Q);
            ctx.UnregisterHandlers(Keys.E);
            for (int i = 0; i <= 9; i++)
            {
                Keys k = (Keys)Enum.Parse(typeof(Keys), "D" + i);
                ctx.UnregisterHandlers(k);
            }

            ctx.UnregisterHandlers(Keys.Space);
        }
        private void CheckStateEnded()
        {
            switch (currentState)
            {
                case TutorialState.Start:
                    stateEnded = true;
                    break;
                case TutorialState.Fire:
                    MouseState ms = Mouse.GetState();
                    if (ms.RightButton == ButtonState.Pressed || ms.LeftButton == ButtonState.Pressed)//right mouse down or left mouse
                        EndState(Keys.None);
                    break;
                case TutorialState.Welcome:
                    if (stateEndTimer == 0)
                        stateEnded = true;
                    break;
            }
        }
        private void TransitionState()
        {
            tutorialLbl.Visibility = Visibility.Hidden;
            stateTransitioning = true;
            transitionTimer = 2;
        }
        public override void ProcessEntities(IEnumerable<Entity> entities, float dt)
        {
            if (stateEndTimer > 0)
            {
                stateEndTimer -= dt;
                if (stateEndTimer <= 0)
                {
                    stateEndTimer = 0;
                }
            }
            if (stateTransitioning)
            {
                transitionTimer -= dt;
                if (transitionTimer <= 0)
                {
                    NextState();
                }
            }
            else if (!stateEnded)
            {
                CheckStateEnded();
            }
            else
            {
                TransitionState();
            }
        }

        public override void OnEntityAdded(Entity e)
        {

        }

        public override void OnEntityRemoved(Entity e)
        {

        }
    }
}
