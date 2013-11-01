using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirtyGame.game.Core.Events;


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

        public void GameStateEventCallback(Event e)
        {
            nextState = States.Exit;
            SwitchState();
        }

        public GameStateManager(Dirty game)
        {
            currentState = new GamePlay();
            this.game = game;
            nextState = States.Game;
            SwitchState();
            EventManager.Instance.AddListener("GameState", GameStateEventCallback);
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
                       currentState = new ExitState();
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
