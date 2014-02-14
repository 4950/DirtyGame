using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirtyGame.game.Core.GameStates
{
    public interface IGameState
    {
        void OnEnter(Dirty game);

        void OnExit();

        void Update(float dt);
    }
}
