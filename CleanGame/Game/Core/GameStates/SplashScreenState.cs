using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CleanGame.Game.Core.GameStates
{
    class SplashScreenState : IGameState
    {

        private Dirty game;

        public SplashScreenState()
        {
            game = null;
        }

        public void OnEnter(Dirty game)
        {
            this.game = game;
        }

        public void OnExit()
        {
            
        }

        public void Update(float dt)
        {
            
        }
    }
}
