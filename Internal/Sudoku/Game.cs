using System;
using System.Collections.Generic;
using System.Linq;

namespace trungdam.Internal.Sudoku
{
    public class Game
    {
        static bool init = false;
        static List<int[]> rows, cols, blocks;
        static List<(int row, int col, int block)> squares;

        int[] state = new int[81];
        List<bool[]> conflictTable = new List<bool[]>(81);

        public HashSet<int> Conflicts { get
            {
                var confl = new HashSet<int>(81);
                for (int i = 0; i < 81; i++)
                    for (int j = i + 1; j < 81; j++)
                        if (conflictTable[i][j - i - 1])
                        {
                            confl.Add(i);
                            confl.Add(j);
                        }
                return confl;
            } 
        }

        public Game()
        {
            if (!init)
            {
                Init();
                init = true;
            }

            for (int count = 80; count >= 0; count--)
                conflictTable.Add(new bool[count]);
        }

        static void Init()
        {
            rows = new List<int[]>(9);
            cols = new List<int[]>(9);
            blocks = new List<int[]>(9);
            squares = new List<(int row, int col, int block)>(81);

            for (int i = 0; i < 9; i++)
            {
                int[] row = new int[9];
                int[] col = new int[9];

                for (int j = 0; j < 9; j++)
                {
                    row[j] = 9 * i + j;
                    col[j] = 9 * j + i;
                }

                rows.Add(row);
                cols.Add(col);
            }

            int[] blockStarts = { 0, 3, 6, 27, 30, 33, 54, 57, 60 };
            int[] blockOffsets = { 0, 1, 2, 9, 10, 11, 18, 19, 20 };
            foreach (int i in blockStarts)
            {
                var block = new List<int>(9);
                foreach (int x in blockOffsets)
                    block.Add(i + x);
                blocks.Add(block.ToArray());
            }

            for (int idx = 0; idx < 81; idx++)
            {
                int r = -1, c = -1, b = -1;
                for (int i = 0; i < 9; i++)
                {
                    if (r > -1 && c > -1 && b > -1)
                        break;
                    if (r == -1 && rows[i].Contains(idx))
                        r = i;
                    if (c == -1 && cols[i].Contains(idx))
                        c = i;
                    if (b == -1 && blocks[i].Contains(idx))
                        b = i;
                }
                squares.Add((r, c, b));
            }
        }

        /// <summary>
        /// Get integer index from tuple of row and column position
        /// </summary>
        /// <param name="pos">Tuple of position</param>
        /// <returns>Index as integer</returns>
        public static int GetIdx((int row, int col) pos) => 9 * pos.row + pos.col;

        /// <summary>
        /// Get position as tuple of row and column from index number
        /// </summary>
        /// <param name="idx">Index number</param>
        /// <returns>Tuple of position</returns>
        public static (int row, int col) GetPos(int idx) => (idx / 9, idx % 9);

        /// <summary>
        /// Get value from a square
        /// </summary>
        /// <param name="pos">Square's position</param>
        /// <returns>Value of square</returns>
        public int GetSquare((int row, int col) pos) => state[GetIdx(pos)];

        /// <summary>
        /// Set a value to a square and update conflict table at the same time
        /// </summary>
        /// <param name="value">Square's new value</param>
        /// <param name="pos">Square's position</param>
        public void SetSquare(int value, (int row, int col) pos)
        {
            int current = state[GetIdx(pos)];
            int idx = GetIdx(pos);

            // Update conflict table
            if (value == 0)
            {
                // Reset current conflict with others to False
                for (int i = 0; i < idx; i++)
                    conflictTable[i][idx - i - 1] = false;
                Array.Fill(conflictTable[idx], false);
            }
            else
            {
                // Find and update conflict if any
                (int idx, int val)[] row = GetRow(pos), col = GetCol(pos), block = GetBlock(pos);
                
                for (int i = 0; i < 9; i++)
                {
                    (int idx, int val) r = row[i], c = col[i], b = block[i];

                    if (idx != r.idx)
                    {
                        if (current == r.val)
                            SetConflict(idx, r.idx, false);
                        else if (value == r.val)
                            SetConflict(idx, r.idx, true);
                    }
                    if (idx != c.idx)
                    {
                        if (current == c.val)
                            SetConflict(idx, c.idx, false);
                        else if (value == c.val)
                            SetConflict(idx, c.idx, true);
                    }
                    if (idx != b.idx)
                    {
                        if (current == b.val)
                            SetConflict(idx, b.idx, false);
                        else if (value == b.val)
                            SetConflict(idx, b.idx, true);
                    }
                }
            }
            state[GetIdx(pos)] = value;
        }

        (int idx, int val)[] GetRow((int row, int col) pos)
        {
            int r = squares[GetIdx(pos)].row;
            int[] row = rows[r];
            return row.Zip(row.Select(idx => state[idx])).ToArray();
        }

        (int idx, int val)[] GetCol((int row, int col) pos)
        {
            int c = squares[GetIdx(pos)].col;
            int[] col = cols[c];
            return col.Zip(col.Select(idx => state[idx])).ToArray();
        }

        (int idx, int val)[] GetBlock((int row, int col) pos)
        {
            int b = squares[GetIdx(pos)].block;
            int[] block = blocks[b];
            return block.Zip(block.Select(idx => state[idx])).ToArray();
        }

        void SetConflict(int idx1, int idx2, bool value)
        {
            int min, max;
            if (idx1 < idx2)
            {
                min = idx1;
                max = idx2;
            }
            else
            {
                min = idx2;
                max = idx1;
            }
            conflictTable[min][max - min - 1] = value;
        }
    }
}
