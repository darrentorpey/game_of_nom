using System;
using System.Collections.Generic;
using System.Text;
using Connecting.Metrics;
using Connecting.Utils;
using System.IO;

namespace Connecting
{
    class PlayerManager
    {
        string _currentPlayerUUID;
        string _currentPlayerName;

        public struct Player
        {
            public string Name;
            public string UUID;
        }

        private Player _CurrentPlayer;
        public Player CurrentPlayer { get { return _CurrentPlayer; } }

        private static PlayerManager s_Instance = new PlayerManager();
        public static PlayerManager Instance { get { return s_Instance; } }

        private PlayerManager()
        {
            string defaultPlayerName = null;
            try
            {
                defaultPlayerName = FileManager.ReadLineFromFile("default_player.info");
            }
            catch (System.IO.FileNotFoundException)
            {
                defaultPlayerName = "Player_One";
            }
            if (defaultPlayerName != null)
            {
                LoadPlayer(defaultPlayerName);
            }
        }

        public void LoadPlayer(string playerName)
        {
            _currentPlayerName = playerName;

            if (File.Exists(configurationFileName()))
            {
                _currentPlayerUUID = FileManager.ReadLineFromFile(configurationFileName());
            }
            else
            {
                // If this player doesn't have a metrics configuration yet...

                // Get a new player ID for this previously unknown player
                _currentPlayerUUID = EventRecorder.Instance.GetNewPlayerID();

                // Record this player's info for future reference
                recordCurrentPlayerUUID();   
            }

            EventRecorder.Instance.PlayerID = _currentPlayerUUID;

            _CurrentPlayer.Name = _currentPlayerName;
            _CurrentPlayer.UUID = _currentPlayerUUID;
        }

        public string GetCurrentPlayerID()
        {
            return _currentPlayerUUID;
        }

        private void recordCurrentPlayerUUID() {
            FileManager.WriteLineToFile(configurationFileName(), _currentPlayerUUID);
        }

        private string configurationFileName()
        {
            return "player_" + _currentPlayerName + ".conf";
        }
    }
}
