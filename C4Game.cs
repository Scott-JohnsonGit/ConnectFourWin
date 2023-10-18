using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Connect4
{
    /// <summary>
    /// Template initalizer for game of connect four
    /// </summary>
    internal class C4Game
    {
        #region Properties/Fields
        /// <summary>
        /// Current board class created to run template game of connect four
        /// </summary>
        public C4Board GameBoard { get { return _gameBoard; } }
        /// <summary>
        /// Rules being applied to the current game
        /// </summary>
        public SpecialRules CurentRules { get {  return _currentRules; } }
        /// <summary>
        /// Field setting rules property
        /// </summary>
        private SpecialRules _currentRules = SpecialRules.None;
        /// <summary>
        /// Field setting current gameboard property
        /// </summary>
        private C4Board _gameBoard;
        /// <summary>
        /// List of player classes for each player participating in a connect four game
        /// </summary>
        private List<Connect4Player> connect4Players = new List<Connect4Player>();
        /// <summary>
        /// Number of times TakeTurn has been called in game instance
        /// </summary>
        private int turnsTaken = 0;
        #endregion
        #region Constructors
        /// <summary>
        /// Construtor creating game of connect four
        /// </summary>
        /// <param name="rule">Current ruleset being used</param>
        /// <param name="User1">Account associated with the first user</param>
        /// <param name="User2">Account associated with the second user</param>
        public C4Game(SpecialRules rule, string User1, string User2)
            :this(rule, User1, User2, null, $@"{Directory.GetCurrentDirectory()}\ConnectFourData\")
        {

        }
        /// <summary>
        /// Construtor creating game of connect four
        /// </summary>
        /// <param name="rule">Current ruleset being used</param>
        /// <param name="User1">Account associated with the first user</param>
        /// <param name="User2">Account associated with the second user</param>
        /// <param name="Path">File directory to create database containing player stats</param>
        /// <exception cref="Exception">"ExtraPlayer" rules enabled without assigning a third user</exception>
        public C4Game(SpecialRules rule, string User1, string User2, string Path)
    : this(rule, User1, User2, null, Path)
        {
            if (rule == SpecialRules.ExtraPlayer)
            {
                throw new Exception("No 3rd player specified");
            }
        }
        /// <summary>
        /// Construtor creating game of connect four
        /// </summary>
        /// <param name="rule">Current ruleset being used</param>
        /// <param name="User1">Account associated with the first user</param>
        /// <param name="User2">Account associated with the second user</param>
        /// <param name="User3">Third possible user with "ExtraPlayer" ruleset</param>
        /// <param name="Path">File directory to create database containing player stats</param>
        /// <exception cref="Exception">User 3 left undefined with "ExtraPlayer" enabled</exception>
        public C4Game(SpecialRules rule, string User1, string User2, string? User3, string Path)
        {
            // Create and add first 2 required players to player list
            Connect4Player Player1 = new Connect4Player(PlayerNum.One, User1, Path);
            Connect4Player Player2 = new Connect4Player(PlayerNum.One, User2, Path);
            connect4Players.Add(Player1);
            connect4Players.Add(Player2);
            // Update rules
            _currentRules = rule;
            // Require 3rd player if ruleset selected, create the user
            if (rule == SpecialRules.ExtraPlayer)
            {
                // Errors if player not provided
                if (User3 == null)
                {
                    throw new Exception("User 3 is not defined");
                }
                // Creates and adds player
                Connect4Player Player3 = new Connect4Player(PlayerNum.One, User3, Path);
                connect4Players.Add(Player3);
            }
            // Creates the board
            _gameBoard = new C4Board(rule, connect4Players);
        }
        #endregion
        /// <summary>
        /// Takes turn adjusted for rules selected
        /// </summary>
        /// <param name="column">Column to drop piece</param>
        /// <param name="winner">Return winner of the game if game won on turn</param>
        /// <param name="winCondition">Return how game was won if game won on turn</param>
        public void TakeTurn(int column, out Connect4Player? winner, out WinType winCondition)
        {
            // Track turns taken (for double turn ruleset)
            turnsTaken++;
            // Adjust column so index starts at 0
            column--;
            // Try to place piece and the lowest availaible space in column
            if (_gameBoard.TryPlacePiece(_gameBoard.LowestEmptySpace(column), column))
            {
                if (_currentRules != SpecialRules.DoubleTurn)
                {
                    _gameBoard.ChangePlayer();
                }
                // Switch only after 2 pieces placed (for double turn rules)
                else if (turnsTaken % 2 == 0)
                {
                    _gameBoard.ChangePlayer();
                }
            }
            // Check if any player won game on turn
            if (_gameBoard.CheckWin(out PlayerNum player, out WinType winType))
            {
                // If a player has won update database and return winning player
                if (player != PlayerNum.None)
                {
                    connect4Players[(int)player - 1].UpdateData(connect4Players[(int)player - 1].GameWins + 1);
                    winner = connect4Players[(int)player - 1];
                }
                // Draws (board is full with no winner)
                else
                {
                    winner = null;
                }
            }
            // No win or draw detected
            else
            {
                winner = null;
            }
            // Set win type (including no win)
            winCondition = winType;
        }
    }
}
