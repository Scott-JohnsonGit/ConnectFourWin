using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

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
        public PlayerNum PlayerNum { get { return _playerNum; } }
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
            :this(player, username, Directory.GetCurrentDirectory() + @"\\ConnectFourData")
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
            GetData();
            if (!PlayerAccount(username))
            {
                _currentAccount = "Guest";
            }
        }
        /// <summary>
        /// Gets or creates an account for leaderboard scores
        /// </summary>
        /// <param name="username">username inputed</param>
        /// <returns>Existing account found</returns>
        /// <exception cref="Exception">Inablity to create new account</exception>
        public bool PlayerAccount(string? username)
        {
            // Makes player a guest user
            if (username == "@Guest%" || username == null)
            {
                return false;
            }
            // Connects to database
            using (SQLiteConnection conn = new SQLiteConnection($"Data Source={_directory}{_dbName}"))
            {
                _currentAccount = username;
                conn.Open();
                // Signs in and gets all data for player wins
                using (SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM Accounts", conn))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (username == reader.GetString(1))
                            {
                                _gameWins = (ushort)reader.GetInt32(2);
                                conn.Close();
                                return true;
                            }
                        }
                    }
                }
                // Creates new account with that username if doesnt already exist
                using (SQLiteCommand cmd = new SQLiteCommand("INSERT INTO Accounts (AccountNames, WinAmount) VALUES (@AccountNames, @WinAmount)", conn))
                {
                    cmd.Parameters.AddWithValue("@AccountNames", username);
                    cmd.Parameters.AddWithValue("@WinAmount", 0);
                    // If no rows were updated (no account was created)
                    if (cmd.ExecuteNonQuery() < 1)
                    {
                        throw new Exception("Command Failed");
                    }
                }
                conn.Close();
            }
            return true;
        }
        /// <summary>
        /// Connects or creates database
        /// </summary>
        /// <exception cref="Exception">Failure to create file directory</exception>
        private void GetData()
        {
            // Create directory if doesnt exist
            if (!Directory.Exists(_directory))
            {
                try
                {
                    Directory.CreateDirectory(_directory);
                }
                catch 
                {
                    throw new Exception("Directory failed");
                }
            }
            // Open connection
            using (
                SQLiteConnection conn = new SQLiteConnection($"Data Source={_directory}{_dbName}"))
            {
                conn.Open();
                // Checks if database has table named Accounts
                try
                {
                    using (SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM Accounts", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                // Creates new table named Accounts if didnt exist
                catch (Exception ex)
                {
                    using (SQLiteCommand cmd = new SQLiteCommand("CREATE TABLE Accounts (ID INTEGER PRIMARY KEY AUTOINCREMENT, AccountNames TEXT, WinAmount INTEGER)", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                conn.Close();
            }
        }
        /// <summary>
        /// Updates players stats after each winning game
        /// </summary>
        /// <param name="winAmount">The new value of the players wins</param>
        /// <exception cref="Exception">Failed to update data</exception>
        public void UpdateData(int winAmount)
        {
            _gameWins++;
            if (_currentAccount != "Guest")
            {
                using (SQLiteConnection conn = new SQLiteConnection($"Data Source={_directory}{_dbName}"))
                {
                    conn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand("UPDATE Accounts SET WinAmount = @Wins WHERE AccountNames = @Account", conn))
                    {
                        cmd.Parameters.AddWithValue("@Wins", winAmount);
                        cmd.Parameters.AddWithValue("@Account", _currentAccount);
                        if (cmd.ExecuteNonQuery() < 1)
                        {
                            throw new Exception("Error sending data");
                        }
                    }
                    conn.Close();
                }
            }
        }
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
