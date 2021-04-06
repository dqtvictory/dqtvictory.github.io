using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace trungdam.Internal.PathViz
{
    public static class AStar
    {
        static int ComputeH((int row, int col) node1, (int row, int col) node2) =>
            Math.Abs(node1.row - node2.row) + Math.Abs(node1.col - node2.col);
        
        public static void Run(Grid grid)
        {
            int count = 0;

            var path = new Dictionary<(int row, int col), (int row, int col)>();
            var queue = new List<(int f, int count, (int row, int col) node)> { (0, count, grid.Start) };
            var visited = new HashSet<(int row, int col)> { grid.Start };

            var G = new Dictionary<(int row, int col), int>(Grid.DimSq);
            var F = new Dictionary<(int row, int col), int>(Grid.DimSq);
            for (int row = 0; row < Grid.Dim; row++)
                for (int col = 0; col < Grid.Dim; col++)
                {
                    G[(row, col)] = 2 * Grid.Dim + 1;
                    F[(row, col)] = 2 * Grid.Dim + 1;
                }
            G[grid.Start] = 0;
            F[grid.Start] = ComputeH(grid.Start, grid.End);

            while (queue.Count > 0)
            {
                var item = queue.Min();
                var current = item.node;
                queue.Remove(item);
                visited.Remove(current);

                foreach (var neighbor in grid.Neighbors(current))
                {
                    var g = G[current] + 1;
                    if (g < G[neighbor])
                    {
                        path[neighbor] = current;
                        if (neighbor == grid.End)
                        {
                            grid.Backtrack(path);
                            return;
                        }

                        G[neighbor] = g;
                        F[neighbor] = g + ComputeH(neighbor, grid.End);

                        if (!visited.Contains(neighbor))
                        {
                            count++;
                            queue.Add((F[neighbor], count, neighbor));
                            visited.Add(neighbor);
                            grid.MakeQueue(neighbor);
                        }
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
