using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreUI;

namespace CleanGame.Game.Core.GameStates
{
    class GameStateGameOver : IGameState
    {
        private Dirty game;


        public GameStateGameOver()
        {
            game = null;
        }

        public void OnEnter(Dirty game)
        {
            this.game = game;
            if(game.GameWon)
                MessageBox.Show("You Won!\nGame Will Now Exit", "Game Over", MessageBox.MessageBoxButttons.Ok).DialogResult += endGame;
            else
                MessageBox.Show("Your journey has ended.\nBetter luck next time!", "Game Over", MessageBox.MessageBoxButttons.Ok).DialogResult += endGame;
        }

        private void endGame(object sender, MessageBox.MessageBoxResultButtons ResultButton)
        {
            game.ResetToMainMenu();
        }

        public void OnExit()
        {
        }

        public void Update(float dt)
        {
        }
    }
}
