using Connect4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Media.Animation;
using System.ComponentModel;

namespace ConnectFourWin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Game file being used
        /// </summary>
        private C4Game _game;
        /// <summary>
        /// Current player's turn
        /// </summary>
        private ushort _currentPlayer;
        /// <summary>
        /// If the game is over
        /// </summary>
        private bool _gameWon = false;
        /// <summary>
        /// Account names of each user
        /// </summary>
        private List<string> _usernames;
        /// <summary>
        /// Special rules being applied
        /// </summary>
        private SpecialRules _specialRules;
        /// <summary>
        /// Amount of players playing
        /// </summary>
        private int _maxplayers = 2;
        /// <summary>
        /// Start game of connect four
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            GetSettings();
            GetAccounts(out _usernames);
            StartGame();
        }
        /// <summary>
        /// Identifies rules and creates the game and its visual elements
        /// </summary>
        private void StartGame()
        {
            if (_specialRules == SpecialRules.ExtraPlayer)
            {
                _game = new C4Game(SpecialRules.ExtraPlayer, _usernames[0], _usernames[1], _usernames[2], Directory.GetCurrentDirectory() + @"ConnectFourData\\");
            }
            else
            {
                _game = new C4Game(_specialRules, _usernames[0], _usernames[1]);
            }
            _gameWon = false;
            _currentPlayer = _game.GameBoard.CurrentPlayer;
            UpdateScoreBoard();
            CreateVisualBoard();
            
        }
        /// <summary>
        /// Creates window for game settings
        /// </summary>
        private void GetSettings()
        {
            SettingsPage settings = new SettingsPage();
            settings.ShowDialog();
            _specialRules = settings.SelectedRules;
            if (_specialRules == SpecialRules.ExtraPlayer)
            {
                _maxplayers = 3;
            }
        }
        /// <summary>
        /// Creates windows for each player and gets usernames
        /// </summary>
        /// <param name="usernames">Returns list of usernames</param>
        private void GetAccounts(out List<string> usernames)
        {
            // Hide the game window
            this.Hide();
            List<LoginWin> logins = new List<LoginWin>();
            usernames = new List<string>();
            // Create new window for each player, data sent on app close
            for (int p = 0; p < _maxplayers; p++)
            {
                LoginWin login = new LoginWin(p);
                logins.Add(login);
                login.ShowDialog();
                _usernames.Add(login.Username);
            }
            this.Show();
        }
        /// <summary>
        /// Craetes visual elements for connect four
        /// </summary>
        public void CreateVisualBoard()
        {
            // Clear any elements in the grid
            XamlBoard.Children.Clear();
            XamlBoard.ColumnDefinitions.Clear();
            XamlBoard.RowDefinitions.Clear();
            // Create corrent amount of columns for game
            for (int x = 0; x < _game.GameBoard.Board.GetLength(1); x++)
            {
                ColumnDefinition column = new ColumnDefinition();
                XamlBoard.ColumnDefinitions.Add(column);
            }
            // Creates visual pieces and rows
            for (int y = 0; y < _game.GameBoard.Board.GetLength(0); y++)
            {
                RowDefinition row = new RowDefinition();
                XamlBoard.RowDefinitions.Add(row);
                for (int x = 0; x < _game.GameBoard.Board.GetLength(1); x++)
                {
                    // Create pieces
                    GameVisualPieces spaces = new GameVisualPieces(x, y)
                    {
                        Content = "🔴",
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        FontSize = 38,
                        Foreground = Brushes.LightGray
                    };
                    // Make font smaller for larger board
                    if (_game.CurentRules == SpecialRules.LargeBoard)
                    {
                        spaces.FontSize = 23;
                    }
                    // Add event handlers
                    spaces.MouseLeftButtonDown += SpaceClicked;
                    spaces.MouseEnter += HighlightNextPiece;
                    spaces.MouseMove += HighlightNextPiece;
                    // Add piece to grid and set columns & rows
                    XamlBoard.Children.Add(spaces);
                    Grid.SetColumn(spaces, x);
                    Grid.SetRow(spaces, y);
                }
            }
        }
        /// <summary>
        /// Called when a piece was clicked
        /// </summary>
        /// <param name="sender">Piece clicked</param>
        /// <param name="e"></param>
        public void SpaceClicked(object sender, MouseButtonEventArgs e)
        {
            // Only allow if game is still ongoing
            if (!_gameWon)
            {
                GameVisualPieces piece = sender as GameVisualPieces;
                // Find row
                int row = _game.GameBoard.LowestEmptySpace(piece.Column);
                // Assign current player before changing color to avoid changing to wrong color
                _currentPlayer = _game.GameBoard.CurrentPlayer;
                // Take the turn
                _game.TakeTurn(piece.Column + 1, out Connect4Player? winner, out WinType winCondition);
                // Find piece to adjust and change its color
                foreach (GameVisualPieces p in XamlBoard.Children)
                {
                    if (p.Row == row && p.Column == piece.Column)
                    {
                        ChangePieceColor(p, false);
                        break;
                    }
                }
                // Updates
                _currentPlayer = _game.GameBoard.CurrentPlayer;
                UpdateScoreBoard();
                // Highlight piece above it
                HighlightNextPiece(piece, null);
                // If player has won
                if (winner != null)
                {
                    _gameWon = true;
                    if (MessageBox.Show($"{winner.CurrentAccount} has won, they have won a total of {winner.GameWins} times. Restart?", "Game Over", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        StartGame();
                    }
                    else
                    {
                        this.Close();
                    }

                }
                // Board has been filled (restart)
                else if (winCondition == WinType.Draw)
                {
                    StartGame();
                }
            }
        }
        /// <summary>
        /// Highlight piece mouse is hovering over
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void HighlightNextPiece(object sender, MouseEventArgs? e)
        {
            // Get column from hovered piece
            GameVisualPieces hoveringPiece = sender as GameVisualPieces;
            // Get lowest empty row
            int row = _game.GameBoard.LowestEmptySpace(hoveringPiece.Column);
            // Change color of piece at lowest row and correct column
            foreach (GameVisualPieces p in XamlBoard.Children )
            {
                if (p.Row == row && p.Column == hoveringPiece.Column)
                {
                    ChangePieceColor(p, true);
                }
                // Reset all other pieces
                else if(_game.GameBoard.Board[p.Row, p.Column] == BoardSpace.Empty)
                {
                    p.ChangeColor(Brushes.LightGray);
                }
            }
        }
        /// <summary>
        /// Changes color of visual pieces
        /// </summary>
        /// <param name="piece">Piece to change</param>
        /// <param name="IsHighlight">If program is only highlighting a space</param>
        /// <exception cref="Exception">Impossible player</exception>
        private void ChangePieceColor(GameVisualPieces piece, bool IsHighlight)
        {
            List<SolidColorBrush> colors;
            // Diffrent color sets for highlighted spaces
            if (IsHighlight)
            {
                colors = new List<SolidColorBrush>()
                {
                    Brushes.IndianRed, Brushes.LightYellow, Brushes.LightGreen
                };
            }
            // Placed piece colors
            else
            {
                colors = new List<SolidColorBrush>()
                {
                    Brushes.Red, Brushes.Yellow, Brushes.Green
                };
            }
            // Change to correct players color
            switch (_currentPlayer)
            {
                case 0:
                    piece.ChangeColor(colors[0]);
                    break;
                case 1:
                    piece.ChangeColor(colors[1]);
                    break;
                case 2:
                    piece.ChangeColor(colors[2]);
                    break;
                default:
                    throw new Exception("Current player error");
            }
        }
        /// <summary>
        /// Updates scoreboard after each win
        /// </summary>
        private void UpdateScoreBoard()
        {
            ScoreBoard.Content = $"{_game.GameBoard.Players[0].CurrentAccount}: {_game.GameBoard.Players[0].GameWins} - {_game.GameBoard.Players[1].CurrentAccount}: {_game.GameBoard.Players[1].GameWins}";
            if (_game.CurentRules == SpecialRules.ExtraPlayer)
            {
                ScoreBoard.Content += $" - {_game.GameBoard.Players[2].CurrentAccount}: {_game.GameBoard.Players[2].GameWins}";
            }
        }
    }
}
