using System;
using System.Collections.Generic;
using System.Text;

namespace Connecting.Metrics
{
    class EventRecorder
    {
        const string GAME_SHORTNAME = "nom";
        const string GAME_KEY = "5";

        private static EventRecorder s_Instance = new EventRecorder(GAME_SHORTNAME, GAME_KEY);
        public static EventRecorder Instance { get { return s_Instance; } }

        private bool _bAllowRecording = true;
        public bool Recording { get { return _bAllowRecording; } }

        MetricsWrapper wrapper;

        // Fill these in with the IDs of the GameEvents you've created on IndieMetrics.com
        private enum Event
        {
            GameStart = 21,
            GameOver = 22,
            GameQuit = 23,
            GameVictory = 24
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

        public EventRecorder(string gameShortname, string gameKey)
        {
            wrapper = new MetricsWrapper(gameShortname, gameKey);
        }

        // Define helper methods to send event POST messages

        public void recordGameStart() {
            recordEvent(Event.GameStart, "<difficulty_mode>Normal</difficulty_mode>");
        }

        public void recordGameOver(int secondsPlayed, int groupsFormed)
        {
            recordEvent(Event.GameOver, "<seconds_played>" + secondsPlayed + "</seconds_played><groups_formed>" + groupsFormed + "</groups_formed>");
        }

        public void recordGameQuit(int secondsPlayed)
        {
            recordEvent(Event.GameQuit, "<seconds_played>" + secondsPlayed + "</seconds_played>");
        }

        public void recordVictory(int nomsDead)
        {
            recordEvent(Event.GameVictory, "<noms_dead>" + nomsDead + "</noms_dead>");
        }

        private 

        // If recording is allowed, record an event from the data given
        void recordEvent(Event eventType, string dataXML)
        {
            if (_bAllowRecording)
            {
                Console.WriteLine("Recording an event of type '" + eventType.ToString() + "'");
                try
                {
                    wrapper.RecordEvent((int)eventType, dataXML);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Event recording failed!");
                    DisableRecording();
                }
            }
        }
    }
}
