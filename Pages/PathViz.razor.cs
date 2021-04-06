using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using trungdam.Internal.PathViz;

namespace trungdam.Pages
{
    public partial class PathViz
    {
        Grid grid;
        Algo algoChoice;

        protected override async Task OnInitializedAsync()
        {
            NewGrid();
            algoChoice = Algo.AStar;
            TableOnMouseOut();
        }

        void NewGrid() => grid = new Grid();

        void Play()
        {
            if (algoChoice == Algo.AStar)
                AStar.Run(grid);
            else if (algoChoice == Algo.BFS)
                BFS.Run(grid);
        }
    }
}
