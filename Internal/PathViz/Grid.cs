using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace trungdam.Internal.PathViz
{
    public enum NodeState
    {
        Open,
        Wall,
        Start,
        End,
        Queue,
        Searched,
        Path
    }

    public enum Algo
    {
        AStar,
        BFS
    }

    public class Grid
    {
        public const int Dim = 30;
        public const int DimSq = Dim * Dim;
        static readonly (int row, int col) None = (-1, -1);

        internal (int row, int col) Start = None;
        internal (int row, int col) End = None;
        public bool HasStartNode => Start != None;
        public bool HasEndNode => End != None;

        NodeState[] states = new NodeState[DimSq];

        /// <summary>
        /// Get integer-based index from grid-based index
        /// </summary>
        /// <param name="pos">Grid-based position</param>
        /// <returns>An equivalent integer-based index</returns>
        static int GetIdx((int row, int col) pos) => Dim * pos.row + pos.col;

        public NodeState GetState((int row, int col) pos) => states[GetIdx(pos)];

        /// <summary>
        /// Turn a node into start node
        /// </summary>
        /// <param name="pos">Node's position</param>
        public void MakeStart((int row, int col) pos)
        {
            if (HasStartNode)
                states[GetIdx(Start)] = NodeState.Open;

            Start = pos;
            states[GetIdx(pos)] = NodeState.Start;
        }

        /// <summary>
        /// Turn a node into end node
        /// </summary>
        /// <param name="pos">Node's position</param>
        public void MakeEnd((int row, int col) pos)
        {
            if (HasEndNode)
                states[GetIdx(End)] = NodeState.Open;

            End = pos;
            states[GetIdx(pos)] = NodeState.End;
        }
        
        /// <summary>
        /// Turn a node into wall node
        /// </summary>
        /// <param name="pos">Node's position</param>
        public void MakeWall((int row, int col) pos)
        {
            int idx = GetIdx(pos);
            if (states[idx] == NodeState.Open)
                states[idx] = NodeState.Wall;
        }

        /// <summary>
        /// Turn a node into open node
        /// </summary>
        /// <param name="pos">Node's position</param>
        public void MakeOpen((int row, int col) pos)
        {
            int idx = GetIdx(pos);
            if (states[idx] != NodeState.Open)
            {
                if (Start == pos)
                    Start = None;
                else if (End == pos)
                    End = None;
                states[idx] = NodeState.Open;
            }
        }

        /// <summary>
        /// Turn a node into path node
        /// </summary>
        /// <param name="pos">Node's position</param>
        public void MakePath((int row, int col) pos)
        {
            int idx = GetIdx(pos);
            states[idx] = NodeState.Path;
        }

        /// <summary>
        /// Turn a node into queue node
        /// </summary>
        /// <param name="pos">Node's position</param>
        public void MakeQueue((int row, int col) pos)
        {
            int idx = GetIdx(pos);
            states[idx] = NodeState.Queue;
        }

        /// <summary>
        /// Turn a node into searched node
        /// </summary>
        /// <param name="pos">Node's position</param>
        public void MakeSearched((int row, int col) pos)
        {
            int idx = GetIdx(pos);
            states[idx] = NodeState.Searched;
        }

        /// <summary>
        /// Generate a random state of the grid
        /// </summary>
        public void Randomize()
        {
            Random random = new Random();

            if (HasStartNode)
                MakeOpen(Start);
            if (HasEndNode)
                MakeOpen(End);

            MakeStart((random.Next(Dim), random.Next(Dim)));
            do
            {
                MakeEnd((random.Next(Dim), random.Next(Dim)));
            }
            while (End == Start);

            double p;
            do { p = random.NextDouble(); }
            while (p < 0.2 || p > 0.6);
            for (int row = 0; row < Dim; row++)
                for (int col = 0; col < Dim; col++)
                {
                    var pos = (row, col);
                    if (pos == Start || pos == End)
                        continue;
                    if (random.NextDouble() < p)
                        states[GetIdx(pos)] = NodeState.Wall;
                    else
                        states[GetIdx(pos)] = NodeState.Open;
                }
        }

        /// <summary>
        /// Find all neighbors of a node
        /// </summary>
        /// <param name="pos">Node's position</param>
        /// <returns>List of node's neighbors</returns>
        internal List<(int row, int col)> Neighbors((int row, int col) pos)
        {
            var adjacents = new (int row, int col)[]
            {
                (pos.row - 1, pos.col), (pos.row + 1, pos.col),
                (pos.row, pos.col - 1), (pos.row, pos.col + 1),
            };
            var neighbors = new List<(int, int)>(4);
            var range = Enumerable.Range(0, Dim);

            foreach (var nei in adjacents)
                if (range.Contains(nei.row) && range.Contains(nei.col) && GetState(nei) != NodeState.Wall)
                    neighbors.Add(nei);
            return neighbors;
        }

        /// <summary>
        /// Backtrack the path found
        /// </summary>
        /// <param name="path">Dictionary of path</param>
        internal void Backtrack(Dictionary<(int row, int col), (int row, int col)> path)
        {
            var current = End;
            while (path.ContainsKey(current))
            {
                current = path[current];
                if (current != Start && current != End)
                {
                    MakePath(current);
                }
            }
        }
    }
}
