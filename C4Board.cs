using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect4
{
    /// <summary>
    /// A game of connect four
    /// </summary>
    internal class C4Board
    {
        #region Properties
        /// <summary>
        /// The current board state
        /// </summary>
        public BoardSpace[,] Board { get { return _board; } }
        /// <summary>
        /// Start template for the XL board special rule
        /// </summary>
        public readonly BoardSpace[,] XLboardTemplate = new BoardSpace[9, 10]
        {
            { BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty },
            { BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty },
            { BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty },
            { BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty },
            { BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty },
            { BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty },
            { BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty },
            { BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty },
            { BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty }
        };
        /// <summary>
        /// Board state displayed as values 0 - empty, 1 - player one, 2 - player two, 3 - player three, 4 - special type
        /// </summary>
        public string DebugBoard { get { return _debugBoard; } }
        /// <summary>
        /// Current player's turn
        /// </summary>
        public ushort CurrentPlayer { get { return PTurn; } }
        /// <summary>
        /// List of players participating in the game
        /// </summary>
        public List<Connect4Player> Players { get { return _currentPlayers; } }
        #endregion
        /// <summary>
        /// Editable state of the game board
        /// </summary>
        private BoardSpace[,] _board = new BoardSpace[6, 7]
        {
            { BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty },
            { BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty },
            { BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty },
            { BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty },
            { BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty },
            { BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty, BoardSpace.Empty }
        };
        /// <summary>
        /// Editable _debug board, updates per piece placed
        /// </summary>
        private string _debugBoard = "";
        /// <summary>
        /// Players in the game
        /// </summary>
        private ushort MaxPlayers = 2;
        /// <summary>
        /// The current players turn
        /// </summary>
        private ushort PTurn = 0;
        private List<Connect4Player> _currentPlayers;
        /// <summary>
        /// Create a game of Connect four 
        /// </summary>
        /// <param name="rule">Special rule to be played in</param>
        public C4Board(SpecialRules rule, List<Connect4Player> currentPlayers)
        {
            if (rule == SpecialRules.LargeBoard)
            {
                _board = XLboardTemplate;
            }
            else if (rule == SpecialRules.ExtraPlayer)
            {
                MaxPlayers = 3;
            }
            SetDebug();
            _currentPlayers = currentPlayers;
        }
        /// <summary>
        /// Updates and sets the DebugBoard string (for debugging and console printing)
        /// </summary>
        private void SetDebug()
        {
            _debugBoard = "";
            for (int y = 0; y < _board.GetLength(0); y++)
            {
                for (int x = 0; x < _board.GetLength(1); x++)
                {
                    _debugBoard += $"{(int)_board[y, x]}, "; // add each value to string indiviually
                }
                _debugBoard += "\n"; // add a new line after each row
            }
        }
        /// <summary>
        /// Finds the lowest unoccupied space in a game of connect four
        /// </summary>
        /// <param name="column">Column to search down</param>
        /// <returns>Lowest available space</returns>
        /// <exception cref="Exception">Inability to find suitable location</exception>
        public int LowestEmptySpace(int column)
        {
            // Ignore garbage column inputs
            if (column >= _board.GetLength(1) || column < 0 || _board[0, column] != BoardSpace.Empty)
            {
                return -1;
            }
            // If no pieces have been placed in column
            else if (_board[_board.GetLength(0) - 1, column] == (int)BoardSpace.Empty)
            {
                return _board.GetLength(0) - 1;
            }
            // Search down the specified column
            for (int i = 0; i < _board.GetLength(0); i++)
            {
                if (_board[i, column] != BoardSpace.Empty)
                {
                    return i - 1;
                }
            }
            throw new Exception("No valid space was found");
        }
        /// <summary>
        /// Attempt to change value of a board space
        /// </summary>
        /// <param name="row">X coordinate</param>
        /// <param name="column">Y coordinate</param>
        /// <returns>Success of attempt</returns>
        public bool TryPlacePiece(int row, int column)
        {
            if (row > -1)
            {
                _board[row, column] = (BoardSpace)PTurn + 1;
                // adjust debug string
                SetDebug();
                return true;
            }
            return false;
        }
        /// <summary>
        /// Change the current player turn
        /// </summary>
        public void ChangePlayer()
        {
            ChangePlayer(69);
        }
        /// <summary>
        /// Switches the current player by requested
        /// </summary>
        /// <param name="player">Requested player</param>
        /// <exception cref="Exception">Invalid player</exception>
        public void ChangePlayer(ushort player)
        {
            // 69 if no specified player to be chosen
            if (player != 69)
            {
                // Impossible player request
                if (player >= MaxPlayers)
                {
                    throw new Exception($"No player number {player}");
                }
                else
                {
                    PTurn = (ushort)(player - 1);
                    return;
                }
            }
            // no requested player
            if (PTurn < _currentPlayers.Count - 1)
            {
                PTurn++;
            }
            // Hits max players and restarts
            else
            {
                PTurn = 0;
            }
        }
        /// <summary>
        /// Look for consecutive pieces belonging to the same player
        /// </summary>
        /// <param name="player">Winning player number</param>
        /// <returns>True if a player has won the game, else false</returns>
        public bool CheckWin(out Connect4Player.PlayerNum player, out WinType winType)
        {
            // checks each player seperately
            for (ushort i = 0; i < _currentPlayers.Count; i++)
            {
                player = (Connect4Player.PlayerNum)(i + 1);
                if (HorizontalW(player, out WinType type))
                {
                    winType = type;
                    return true;
                }
                else if (VerticalW(player, out WinType type2))
                {
                    winType = type2;
                    return true;
                }
                else if (DiagonalW(player, out WinType type3))
                {
                    winType = type3;
                    return true;
                }
                else if (Draw(out WinType DrawType))
                {
                    player = Connect4Player.PlayerNum.None;
                    winType = WinType.Draw;
                    return true;
                }
            }
            // no player has won
            player = Connect4Player.PlayerNum.None;
            winType = WinType.None;
            return false;
        }
        /// <summary>
        /// Searches all horizontal rows
        /// </summary>
        /// <param name="player">current player being checked</param>
        /// <returns>True if player has won</returns>
        private bool HorizontalW(Connect4Player.PlayerNum player, out WinType type)
        {
            for (int y = 0; y < _board.GetLength(0); y++)
            {
                int ConsecutivePieces = 0;
                for (int x = 0; x < _board.GetLength(1); x++)
                {
                    if (_board[y, x] == (BoardSpace)player)
                    {
                        ConsecutivePieces++;
                    }
                    // Non current player piece interupts counting
                    else
                    {
                        ConsecutivePieces = 0;
                    }
                    // player has won
                    if (ConsecutivePieces > 3)
                    {
                        type = WinType.Horizontal;
                        return true;
                    }
                }
            }
            type = WinType.None;
            return false;
        }
        /// <summary>
        /// Searches all columns
        /// </summary>
        /// <param name="player">Current player being checked</param>
        /// <returns>True if player has won</returns>
        private bool VerticalW(Connect4Player.PlayerNum player, out WinType type)
        {
            for (int x = 0; x < _board.GetLength(1); x++)
            {
                int consecutivePieces = 0;
                for (int y = 0; y < _board.GetLength(0); y++)
                {
                    if (_board[y, x] == (BoardSpace)player)
                    {
                        consecutivePieces++;
                    }
                    // Non current player piece interupts counting
                    else
                    {
                        consecutivePieces = 0;
                    }
                    // player has won
                    if (consecutivePieces > 3)
                    {
                        type = WinType.Vertical;
                        return true;
                    }
                }
            }
            type = WinType.None;
            return false;
        }
        /// <summary>
        /// Checks both diagonals
        /// </summary>
        /// <param name="player">Current player being checked</param>
        /// <param name="type">Type of win if won</param>
        /// <returns>If a win was detected</returns>
        private bool DiagonalW(Connect4Player.PlayerNum player, out WinType type)
        {
            return (DiagonalL(player, out type) || DiagonalR(player, out type));
        }
        /// <summary>
        /// Checks for a diagonal going from top left to bottom right
        /// </summary>
        /// <param name="player">Current player being checked</param>
        /// <param name="type">Type of win if won</param>
        /// <returns>If a win was detected</returns>
        private bool DiagonalL(Connect4Player.PlayerNum player, out WinType type)
        {
            for (int x = _board.GetLength(1) - 1; x > 3; x--)
            {
                for (int y = _board.GetLength(0) - 1; y > 3; y--)
                {
                    if (_board[y, x] == (BoardSpace)player)
                    {
                        if (_board[y - 1, x - 1] == (BoardSpace)player && _board[y - 2, x - 2] == (BoardSpace)player && _board[y - 3, x - 3] == (BoardSpace)player)
                        {
                            type = WinType.DiagonalLeft;
                            return true;
                        }
                    }
                }
            }
            type = WinType.None;
            return false;
        }
        /// <summary>
        /// Checks for a diagonal going from top right to bottom left
        /// </summary>
        /// <param name="player">Current player being checked</param>
        /// <param name="type">Type of win if won</param>
        /// <returns>If a win was detected</returns>
        private bool DiagonalR(Connect4Player.PlayerNum player, out WinType type)
        {
            for (int x = 0; x < _board.GetLength(1) - 3; x++)
            {
                for (int y = _board.GetLength(0) - 1; y > 3; y--)
                {
                    if (_board[y, x] == (BoardSpace)player)
                    {
                        if (_board[y - 1, x + 1] == (BoardSpace)player && _board[y - 2, x + 2] == (BoardSpace)player && _board[y - 3, x + 3] == (BoardSpace)player)
                        {
                            type = WinType.DiagonalRight;
                            return true;
                        }
                    }
                }
            }
            type = WinType.None;
            return false;
        }
        /// <summary>
        /// Neither player has won and board is full
        /// </summary>
        /// <param name="player">Current player being checked</param>
        /// <param name="type">Type of win if won</param>
        /// <returns>If a draw was found</returns>
        private bool Draw(out WinType type)
        {
            type = WinType.Draw;
            foreach (BoardSpace space in _board)
            {
                // If it finds any empty space then no draw has been detected
                if (space == BoardSpace.Empty)
                {
                    return false;
                }
            }
            return true;
        }
    }
    /// <summary>
    /// Each type of acceptable space on each board position
    /// </summary>
    public enum BoardSpace
    {
        Empty = 0,
        PlayerOne = 1,
        PlayerTwo = 2,
        PlayerThree = 3,
        SpecialCase = 4,
    }
    /// <summary>
    /// Types of acceptable special rules for the game
    /// </summary>
    public enum SpecialRules
    {
        None = 0,
        ExtraPlayer = 1,
        LargeBoard = 2,
        DoubleTurn = 3,
    }
    /// <summary>
    /// Forms of wins/draw acceptable in game
    /// </summary>
    public enum WinType
    {
        None = 0,
        Horizontal = 1,
        Vertical = 2,
        DiagonalLeft = 3,
        DiagonalRight = 4,
        Draw = 5
    }
}
