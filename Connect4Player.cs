using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Connect4
{
    /// <summary>
    /// A player associated with a game of connect four
    /// </summary>
    internal class Connect4Player
    {
        /// <summary>
        /// Games won by the player
        /// </summary>
        public ushort GameWins { get { return _gameWins; } }
        /// <summary>
        /// Account sent to database to track stats
        /// </summary>
        public string CurrentAccount { get { return _currentAccount; } }
        /// <summary>
        /// Directory path taken to database
        /// </summary>
        public readonly string Path;
        /// <summary>
        /// Number player this instance is associated with
        /// </summary>
        public PlayerNum Playernum { get { return _playerNum; } }
        /// <summary>
        /// Number player this instance is associated with
        /// </summary>
        private PlayerNum _playerNum = PlayerNum.None;
        /// <summary>
        /// Wins associated with account
        /// </summary>
        private ushort _gameWins = 0;
        /// <summary>
        /// Full directory path and name of database
        /// </summary>
        private readonly string _directory;
        /// <summary>
        /// Name of database
        /// </summary>
        private const string _dbName = "Data.db";
        /// <summary>
        /// Account sent to database to track stats
        /// </summary>
        private string _currentAccount = "@Guest%";
        /// <summary>
        /// Creates player and establishes database
        /// </summary>
        /// <param name="player">Player associated with instance</param>
        /// <param name="username">Username for leaderboard account</param>
        public Connect4Player(PlayerNum player, string username)
            : this(player, username, Directory.GetCurrentDirectory() + @"\\ConnectFourData")
        {
            // Uses current directory of running application
        }
        /// <summary>
        /// Creates player and establishes database
        /// </summary>
        /// <param name="player">Player associated with instance</param>
        /// <param name="username">Username for leaderboard account</param>
        /// <param name="path">Custom path for the database</param>
        public Connect4Player(PlayerNum player, string username, string path)
        {
            _directory = path;
            Path = @$"{_directory}\\{_dbName}";
            _playerNum = player;
        }
        
        /// <summary>
        /// Enum associated with players turns
        /// </summary>
        public enum PlayerNum
        {
            None = 0,
            One = 1,
            Two = 2,
            Extra = 3
        }
    }
}
