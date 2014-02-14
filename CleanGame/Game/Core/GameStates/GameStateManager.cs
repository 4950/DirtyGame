using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CleanGame.Game.Core.Events;


namespace CleanGame.Game.Core.GameStates
{
    public class GameStateManager
    {

        private Dictionary<Type, IGameState> states = new Dictionary<Type, IGameState>();
        private IGameState currentState;
        private Dirty game;

        public IGameState CurrentState 
        { 
            get{ return currentState; }
        }

        private States nextState;
        enum States { Splash, Pause, Game, MainMenu, GameOver, GameMenu, Buy, Exit };

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
        public void GameStateBuyEventCallback(Event e)
        {
            nextState = States.Buy;
            SwitchState();
        }
        public void GameStateMainMenuEventCallback(Event e)
        {
            nextState = States.MainMenu;
            SwitchState();
        }
        public void GameStateGameMenuEventCallback(Event e)
        {
            nextState = States.GameMenu;
            SwitchState();
        }
        public GameStateManager(Dirty game)
        {
            currentState = null;
            this.game = game;
            nextState = States.MainMenu;
            SwitchState();
            EventManager.Instance.AddListener("GameState", GameStateEventCallback);
            EventManager.Instance.AddListener("GameStateBuy", GameStateBuyEventCallback);
            EventManager.Instance.AddListener("GameStateGameOver", GameStateGameOverCallback);
            EventManager.Instance.AddListener("GameStateGame", GameStateGameCallback);
            EventManager.Instance.AddListener("GameStateMainMenu", GameStateMainMenuEventCallback);
            EventManager.Instance.AddListener("GameStateGameMenu", GameStateGameMenuEventCallback);
        }
        private void SwitchToState<T>() where T : IGameState, new()
        {
            if (states.Keys.Contains(typeof(T)))
                currentState = states[typeof(T)];
            else
            {
                T state = new T();
                states.Add(typeof(T), state);
                currentState = state;
            }
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
                case States.GameMenu:
                   currentState = new GameMenuState();
                   break;
                case States.GameOver:
                   currentState = new GameStateGameOver();
                   break;
                case States.Game:
                   {
                       SwitchToState<GamePlay>();
                       break;
                   }
                case States.Buy:
                   SwitchToState<BuyState>();
                   break;
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
