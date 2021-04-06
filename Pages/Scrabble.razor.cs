using System.Collections.Generic;
using System.Threading.Tasks;
using trungdam.Internal.Scrabble;

namespace trungdam.Pages
{
    public partial class Scrabble
    {
        (int row, int col) selectedSquare = (-1, -1);
        bool enteringBlank = false;

        const string godUrl = "http://52.47.142.152:4001/scrabble";
        bool callingGod = false;
        List<(int, int)> moveSquares = new List<(int, int)>();

        string inputState;
        public string InputState
        {
            get => inputState;
            set
            {
                if (string.IsNullOrEmpty(value))
                    inputState = string.Empty;
                else
                {
                    int maxSize = Board.SizeSquare;
                    var buffer = new List<char>(maxSize);

                    foreach (char c in value)
                    {
                        if (buffer.Count == maxSize)
                            break;
                        else if (c == '0' || c != '?' && charKeys.Contains(c))
                            buffer.Add(c);
                    }
                    inputState = new string(buffer.ToArray());
                }
            }
        }

        string rack;
        public string Rack
        {
            get => rack;
            set
            {
                if (string.IsNullOrEmpty(value))
                    rack = null;
                else
                {
                    int maxSize = Board.RackSize;
                    var buffer = new List<char>(maxSize);

                    foreach (char c in value)
                    {
                        if (buffer.Count == maxSize)
                            break;
                        else if (charKeys.Contains(c))
                            buffer.Add(char.ToUpper(c));
                    }

                    rack = new string(buffer.ToArray());
                }
            }
        }

        Board board;

        protected override async Task OnInitializedAsync()
        {
            board = new Board();
            Snack.Configuration.SnackbarVariant = MudBlazor.Variant.Filled;
        }

        /// <summary>
        /// Load the Board State input into internal state
        /// </summary>
        void LoadState()
        {
            if (moveSquares.Count > 0)
                moveSquares = new List<(int, int)>();

            if (inputState.Length != Board.SizeSquare)
            {
                Snack.Add($"Input state must have exactly {Board.SizeSquare} characters consisting letters and zero digits", MudBlazor.Severity.Error);
                return;
            }
            board = new Board();
            for (int i = 0; i < Board.Size; i++)
                for (int j = 0; j < Board.Size; j++)
                {
                    char c = inputState[Board.Size * i + j];
                    if (c != '0')
                        board.SetTile((i, j), c);
                }
            Snack.Add("State has been successfully loaded", MudBlazor.Severity.Success);

        }

        /// <summary>
        /// Generate user-friendly state from current internal state
        /// </summary>
        void GenerateState()
        {
            if (moveSquares.Count > 0)
                moveSquares = new List<(int, int)>();

            char[] buffer = new char[Board.SizeSquare];
            for (int i = 0; i < Board.Size; i++)
                for (int j = 0; j < Board.Size; j++)
                {
                    char c = board.GetTile((i, j));
                    buffer[Board.Size * i + j] = c == 0 ? '0' : c;
                }
            inputState = new string(buffer);
        }

        /// <summary>
        /// Clear all tiles from the board
        /// </summary>
        void ClearBoard() 
        {
            if (moveSquares.Count > 0)
                moveSquares = new List<(int, int)>();

            board = new Board(); 
        }
        
        /// <summary>
        /// Ask for the Scrabble God's blessing
        /// </summary>
        async void CallGod()
        {
            // Some pre-checking before calling God
            if (board.IsEmpty && rack.Length < Board.RackSize)
            {
                Snack.Add($"Starting rack must contain exactly {Board.RackSize} tiles");
                return;
            }

            UnselectSquare();
            string message = "?message=";

            if (board.IsEmpty)
                message += $"start {rack}";
            else
            {
                GenerateState();
                message += $"help {rack} {inputState}";
            }

            Snack.Add("Asking for Scrabble God's blessing...", MudBlazor.Severity.Info);
            callingGod = true;
            try
            {
                string move = await Http.GetStringAsync(godUrl + message);
                InterpretMove(move);
            }
            catch
            {
                Snack.Add("An error seems to prevent God from hearing your prayer", MudBlazor.Severity.Error);
            }
            callingGod = false;
            StateHasChanged();
        }

        async void InterpretMove(string move)
        {
            if (moveSquares.Count > 0)
                moveSquares = new List<(int, int)>();

            string[] parts = move.Split('\n');
            int score = int.Parse(parts[0]);

            if (score == 0)
                Snack.Add("There is no good move at all... Better skip this turn");
            else
            {
                // Display the result on the UI
                string tiles = parts[1];
                for (int i = 2; i < parts.Length; i++)
                {
                    string[] rc = parts[i][1..^1].Split(", ");
                    var pos = (int.Parse(rc[0]), int.Parse(rc[1]));
                    moveSquares.Add(pos);
                    board.SetTile(pos, tiles[i - 2]);
                }

                // Remove used tiles from rack
                var remaining = new List<char>(rack);
                bool usedBlank = false;
                foreach (char tile in tiles)
                {
                    if (char.IsLower(tile))
                    {
                        usedBlank = true;
                        remaining.Remove('?');
                    }
                    else
                        remaining.Remove(tile);
                }
                rack = new string(remaining.ToArray());
                await Dialog.ShowMessageBox("Answer from Scrabble God", $"Play {tiles} for {score} points");

                // Display warning if there are blank used
                if (usedBlank)
                    Snack.Add("Look carefully at the solution as there is at least one blank (without face value)", MudBlazor.Severity.Warning);
            }
        }
    }
}