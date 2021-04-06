using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using trungdam.Internal.Sudoku;

namespace trungdam.Pages
{
    public partial class Sudoku
    {
        (int row, int col) selectedSquare = (-1, -1);
        Game game;

        HashSet<int> sameColIdx = new HashSet<int>();
        HashSet<int> conflicts = new HashSet<int>();

        bool boardUpdated = false;

        string GetSquare((int row, int col) pos)
        {
            int value = game.GetSquare(pos);
            return value == 0 ? string.Empty : value.ToString();
        }

        void SetSquare(int value, (int row, int col) pos)
        {
            int current = game.GetSquare(pos);
            if (current != value)
            {
                game.SetSquare(value, pos);
                boardUpdated = true;
            }
        }
    }
}
