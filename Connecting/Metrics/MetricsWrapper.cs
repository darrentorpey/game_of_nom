using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Connecting.Metrics
{
    // Example XML event POST message for IndieMetrics
    //<?xml version ="1.0" encoding="UTF-8"?>
    //<event>
    //  <event_type_id>1</event_type_id>
    //  <difficulty_mode>Easy</difficulty_mode>
    //  <timestamp>Tue Jan 20 15:59:05 -0500 2009</timestamp>
    //</event>

    class MetricsWrapper
    {
        static string s_URL_BASE = "http://alpha.indiemetrics.com/";
        //static string s_URL_BASE = "http://localhost:3000/";

        string _GameKey; // The unique, secret key for this game

        string _XMLBegin = "<?xml version =\"1.0\" encoding=\"UTF-8\"?>";

        SimpleRESTClient _EventPostSender = new SimpleRESTClient("POST", s_URL_BASE + "events.xml");

        public MetricsWrapper(string gameKey)
        {
            _GameKey = gameKey;
        }

        // Record an event with the given ID and message body
        public string RecordEvent(int eventID, string playerID, string messageBody)
        {
            string requestBody = makeEventPostMessage(eventID, playerID, messageBody);
            _EventPostSender.MakeRequest(requestBody);
            return _EventPostSender.GetResponse();
        }

        public string GetNewPlayerID()
        {
            SimpleRESTClient _PlayerIDFetcher = new SimpleRESTClient("GET", s_URL_BASE + "players/get_new_id");
            string newID = _PlayerIDFetcher.GetSimpleString();
            return newID;
        }

        // Creates the actual XML for the event message that will be POSTed to IndieMetrics
        private string makeEventPostMessage(int eventID, string playerID, string eventXML)
        {
            string timestamp = DateTime.Now.ToString("U"); // "example: Tue Jan 20 15:59:05 -0500 2009";
            return _XMLBegin + "<event><event_type_id>" + eventID + "</event_type_id><game_key>" + _GameKey + "</game_key><player_id>" + playerID + "</player_id>" + eventXML + "<timestamp>" + timestamp + "</timestamp></event>";
        }

    }
}
