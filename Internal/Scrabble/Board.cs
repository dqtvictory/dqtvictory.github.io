using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace trungdam.Internal.Scrabble
{
    public enum Square
    {
        Normal,
        DL,
        TL,
        DW,
        TW,
        Start,
        Selection,
        Solution
    }

    public class Board
    {
        public const int Size = 15;
        public const int SizeSquare = 15 * 15;
        public const int RackSize = 7;
        public static readonly (int row, int col) StartPos = (7, 7);

        static bool doneInit = false;
        static Square[] squares = new Square[SizeSquare];
        char[] state = new char[SizeSquare];

        public bool IsEmpty => state.All(c => c == 0);

        public Board()
        {
            if (!doneInit)
            {
                squares[GetIdx((0, 0))] = Square.TW;
                squares[GetIdx((1, 5))] = Square.TL;
                squares[GetIdx((5, 1))] = Square.TL;
                squares[GetIdx((5, 5))] = Square.TL;
                squares[GetIdx((1, 1))] = Square.DW;
                squares[GetIdx((2, 2))] = Square.DW;
                squares[GetIdx((3, 3))] = Square.DW;
                squares[GetIdx((4, 4))] = Square.DW;
                squares[GetIdx((0, 3))] = Square.DL;
                squares[GetIdx((3, 0))] = Square.DL;
                squares[GetIdx((2, 6))] = Square.DL;
                squares[GetIdx((6, 2))] = Square.DL;
                squares[GetIdx((6, 6))] = Square.DL;
                squares[GetIdx((0, 14))] = Square.TW;
                squares[GetIdx((1, 9))] = Square.TL;
                squares[GetIdx((5, 13))] = Square.TL;
                squares[GetIdx((5, 9))] = Square.TL;
                squares[GetIdx((1, 13))] = Square.DW;
                squares[GetIdx((2, 12))] = Square.DW;
                squares[GetIdx((3, 11))] = Square.DW;
                squares[GetIdx((4, 10))] = Square.DW;
                squares[GetIdx((0, 11))] = Square.DL;
                squares[GetIdx((3, 14))] = Square.DL;
                squares[GetIdx((2, 8))] = Square.DL;
                squares[GetIdx((6, 12))] = Square.DL;
                squares[GetIdx((6, 8))] = Square.DL;
                squares[GetIdx((14, 14))] = Square.TW;
                squares[GetIdx((13, 9))] = Square.TL;
                squares[GetIdx((9, 13))] = Square.TL;
                squares[GetIdx((9, 9))] = Square.TL;
                squares[GetIdx((13, 13))] = Square.DW;
                squares[GetIdx((12, 12))] = Square.DW;
                squares[GetIdx((11, 11))] = Square.DW;
                squares[GetIdx((10, 10))] = Square.DW;
                squares[GetIdx((14, 11))] = Square.DL;
                squares[GetIdx((11, 14))] = Square.DL;
                squares[GetIdx((12, 8))] = Square.DL;
                squares[GetIdx((8, 12))] = Square.DL;
                squares[GetIdx((8, 8))] = Square.DL;
                squares[GetIdx((14, 0))] = Square.TW;
                squares[GetIdx((13, 5))] = Square.TL;
                squares[GetIdx((9, 1))] = Square.TL;
                squares[GetIdx((9, 5))] = Square.TL;
                squares[GetIdx((13, 1))] = Square.DW;
                squares[GetIdx((12, 2))] = Square.DW;
                squares[GetIdx((11, 3))] = Square.DW;
                squares[GetIdx((10, 4))] = Square.DW;
                squares[GetIdx((14, 3))] = Square.DL;
                squares[GetIdx((11, 0))] = Square.DL;
                squares[GetIdx((12, 6))] = Square.DL;
                squares[GetIdx((8, 2))] = Square.DL;
                squares[GetIdx((8, 6))] = Square.DL;
                squares[GetIdx((7, 0))] = Square.TW;
                squares[GetIdx((0, 7))] = Square.TW;
                squares[GetIdx((14, 7))] = Square.TW;
                squares[GetIdx((7, 14))] = Square.TW;
                squares[GetIdx((3, 7))] = Square.DL;
                squares[GetIdx((7, 3))] = Square.DL;
                squares[GetIdx((7, 11))] = Square.DL;
                squares[GetIdx((11, 7))] = Square.DL;
                squares[GetIdx(StartPos)] = Square.Start;
                doneInit = true;
            }
        }

        static int GetIdx((int row, int col) pos) => Size * pos.row + pos.col;
        
        public static Square Kind((int row, int col) pos) => squares[GetIdx(pos)];

        public char GetTile((int row, int col) pos) => state[GetIdx(pos)];
        
        public void SetTile((int row, int col) pos, char tile) => state[GetIdx(pos)] = tile;
    }
}
