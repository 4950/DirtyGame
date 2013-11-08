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
        enum States { Splash, Pause, Game, MainMenu, GameOver, Exit };

        public void GameStateEventCallback(Event e)
        {
            nextState = States.Exit;
            SwitchState();
        }
        public void GameStateGameOverCallback(Event e)
        {
            nextState = States.GameOver;
            SwitchState();
        }
        public void GameStateGameCallback(Event e)
        {
            nextState = States.Game;
            SwitchState();
        }
        public GameStateManager(Dirty game)
        {
            currentState = null;
            this.game = game;
            nextState = States.MainMenu;
            SwitchState();
            EventManager.Instance.AddListener("GameState", GameStateEventCallback);
            EventManager.Instance.AddListener("GameStateGameOver", GameStateGameOverCallback);
            EventManager.Instance.AddListener("GameStateGame", GameStateGameCallback);
        }
        
        private void SwitchState()
        {
            States switchState = nextState;

            if (currentState != null)
                currentState.OnExit();

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
                case States.MainMenu:
                   currentState = new MainMenuState();
                   break;
                case States.GameOver:
                   currentState = new GameStateGameOver();
                   break;
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
