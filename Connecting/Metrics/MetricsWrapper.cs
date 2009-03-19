using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Connecting.Metrics
{
    class MetricsWrapper
    {
        string _ServerBaseURL; // The base URL to the metrics web service

        string _GameKey; // The unique, secret key for this game

        SimpleRESTClient _EventPostSender; // Our client for handling REST messaging

        static string _XMLBegin = "<?xml version =\"1.0\" encoding=\"UTF-8\"?>";
        
        // Initialize a metrics wrapper
        //  serverBaseURL - The base URL to the metrics web service, such as http://api.indiemetrics.com
        //  gameKey - The secret game key for the game for which this wrapper will recording information
        public MetricsWrapper(string serverBaseURL, string gameKey)
        {
            _ServerBaseURL = serverBaseURL;
            _GameKey = gameKey;
            _EventPostSender = new SimpleRESTClient("POST", _ServerBaseURL + "events");
        }

        // Record an event with the given ID and message body
        public string RecordEvent(int eventID, string playerID, string messageBody)
        {
            string requestBody = makeEventPostMessage(eventID, playerID, messageBody);
            _EventPostSender.MakeRequest(requestBody);
            return _EventPostSender.GetResponse();
        }

        // Gets a new, unique player ID from the metrics web service
        public string GetNewPlayerID()
        {
            SimpleRESTClient _PlayerIDFetcher = new SimpleRESTClient("GET", _ServerBaseURL + "players/get_new_id");
            return _PlayerIDFetcher.GetSimpleString();
        }

        // Creates the actual XML for the event message that will be POSTed to the metrics web service
        private string makeEventPostMessage(int eventID, string playerID, string eventXML)
        {
            string timestamp = DateTime.Now.ToString("U"); // "example: Tue Jan 20 15:59:05 -0500 2009";
            return _XMLBegin + "<event><event_type_id>" + eventID + "</event_type_id><game_key>" + _GameKey + "</game_key><player_id>" + playerID + "</player_id>" + eventXML + "<timestamp>" + timestamp + "</timestamp></event>";
        }

        // Example XML event POST message for IndieMetrics
        //<?xml version ="1.0" encoding="UTF-8"?>
        //<event>
        //  <event_type_id>1</event_type_id>
        //  ... BEGIN EVENT-TYPE-SPECIFIC DATA FIELDS ...
        //  <difficulty_mode>Easy</difficulty_mode>
        //  ... END EVENT-TYPE-SPECIFIC DATA FIELDS ...
        //  <timestamp>Tue Jan 20 15:59:05 -0500 2009</timestamp>
        //</event>
    }
}
