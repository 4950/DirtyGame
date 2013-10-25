using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirtyGame.game.Core.GameStates
{
    class GamePlay : IGameState
    {
        private Dirty game;

        public GamePlay()
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
            game.world.Update(dt);
        }
    }
}
