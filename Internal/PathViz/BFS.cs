using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace trungdam.Internal.PathViz
{
    public static class BFS
    {
        public static void Run(Grid grid)
        {
            var path = new Dictionary<(int row, int col), (int row, int col)>();
            var queue = new Queue<(int row, int col)>();
            queue.Enqueue(grid.Start);
            var visited = new HashSet<(int row, int col)> { grid.Start };

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                visited.Remove(current);

                foreach (var neighbor in grid.Neighbors(current))
                {
                    if (neighbor == grid.Start || grid.GetState(neighbor) == NodeState.Searched)
                        continue;
                    path[neighbor] = current;

                    if (neighbor == grid.End)
                    {
                        grid.Backtrack(path);
                        return;
                    }

                    if (!visited.Contains(neighbor))
                    {
                        queue.Enqueue(neighbor);
                        visited.Add(neighbor);
                        grid.MakeQueue(neighbor);
                    }
                }

                if (current != grid.Start)
                {
                    grid.MakeSearched(current);
                }
            }
        }
    }
}
