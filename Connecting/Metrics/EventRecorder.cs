using System;
using System.Collections.Generic;
using System.Text;

namespace Connecting.Metrics
{
    class EventRecorder
    {
        const string SECRET_GAME_KEY = "893c910a-532e-102c-a43c-0030487edd88";

        private static EventRecorder s_Instance = new EventRecorder(SECRET_GAME_KEY);
        public static EventRecorder Instance { get { return s_Instance; } }

        private bool _bAllowRecording = true;
        public bool Recording { get { return _bAllowRecording; } }

        public string _PlayerID;
        public string PlayerID { get { return _PlayerID; } set { _PlayerID = value; } }

        MetricsWrapper wrapper;

        // Fill these in with the IDs of the GameEvents you've created on IndieMetrics.com
        private enum Event
        {
            GameStart = 1,
            GameOver = 2,
            GameQuit = 3,
            GameVictory = 4
        }

        public void DisableRecording() {
            _bAllowRecording = false;
        }

        public void EnableRecording()
        {
            _bAllowRecording = true;
        }

        public void ToggleRecording()
        {
            if (_bAllowRecording)
            {
                _bAllowRecording = false;
            }
            else
            {
                _bAllowRecording = true;
            }
        }

        public EventRecorder(string gameKey)
        {
            wrapper = new MetricsWrapper(gameKey);
        }

        // Define helper methods to send event POST messages

        public void RecordGameStart() {
            recordEvent(Event.GameStart, "<difficulty_mode>Normal</difficulty_mode>");
        }

        public void RecordGameOver(int secondsPlayed, int groupsFormed)
        {
            recordEvent(Event.GameOver, "<seconds_played>" + secondsPlayed + "</seconds_played><groups_formed>" + groupsFormed + "</groups_formed>");
        }

        public void RecordGameQuit(int secondsPlayed)
        {
            recordEvent(Event.GameQuit, "<seconds_played>" + secondsPlayed + "</seconds_played>");
        }

        public void RecordVictory(int nomsDead)
        {
            recordEvent(Event.GameVictory, "<noms_dead>" + nomsDead + "</noms_dead>");
        }

        public string GetNewPlayerID()
        {
            return wrapper.GetNewPlayerID();
        } 

        // If recording is allowed, record an event from the data given
        private void recordEvent(Event eventType, string dataXML)
        {
            if (_bAllowRecording)
            {
                Console.WriteLine("Recording an event of type '" + eventType.ToString() + "'");
                try
                {
                    wrapper.RecordEvent((int)eventType, _PlayerID, dataXML);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Event recording failed! '" + ex.Message + "'");
                    DisableRecording();
                }
            }
        }
    }
}
