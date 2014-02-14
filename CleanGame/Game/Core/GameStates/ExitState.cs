using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CleanGame.Game.Core.GameStates
{
    class ExitState : IGameState
    {

        private Dirty game;

        public ExitState()
        {
            game = null;
        }

        public void OnEnter(Dirty game)
        {
            this.game = game;
            game.Exit();
        }

        public void OnExit()
        {
            
        }

        public void Update(float dt)
        {
           
        }
    }
}
