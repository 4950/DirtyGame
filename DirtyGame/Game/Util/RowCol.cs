using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirtyGame.game.Util
{
    public struct RowCol
    {
        public int Row;
        public int Col;
   
        public RowCol(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public RowCol(RowCol rc)
        {
            Row = rc.Row;
            Col = rc.Col;
        }
    }
}
