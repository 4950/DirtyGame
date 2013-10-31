using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirtyGame.game.Core.GameStates
{
    public class GameStateManager
    {
        private IGameState currentState;
        private Dirty game;

        public IGameState CurrentState 
        { 
            get{ return currentState; }
        }

        private States nextState;
        enum States { Splash, Pause, Game, Exit };

        public GameStateManager(Dirty game)
        {
            currentState = new GamePlay();
            this.game = game;
            nextState = States.Game;
            SwitchState();
        }

        public void GameStateEvent()
        {
        }

        
        private void SwitchState()
        {
            States switchState = nextState;

            switch (switchState)
            {
                case States.Splash:
                   {
                       currentState = new SplashScreenState();
                       break;
                   }

                case States.Pause:
                   {
                       break;
                   }

                case States.Game:
                   {
                       currentState = new GamePlay();
                       break;
                   }

                case States.Exit:
                   {
                       break;
                   }

                default:
                   {
                       //Exit for default
                       break;
                   }

            }

            currentState.OnEnter(game);
        }
    }
}
