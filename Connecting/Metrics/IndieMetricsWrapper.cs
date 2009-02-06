using System;
using System.Collections.Generic;
using System.Text;

namespace Connecting.Metrics
{
    // Example XML event POST message for IndieMetrics
    //<?xml version ="1.0" encoding="UTF-8"?>
    //<event>
    //  <event_type_id>1</event_type_id>
    //  <difficulty_mode>Easy</difficulty_mode>
    //  <timestamp>Tue Jan 20 15:59:05 -0500 2009</timestamp>
    //</event>

    class IndieMetricsWrapper
    {
        string _GameKey; // The unique, secret key for this game
        string _GameShortname; // The 'shortname' of the game in our system

        string _XMLBegin = "<?xml version =\"1.0\" encoding=\"UTF-8\"?>";

        RESTMessageSender _EventPostSender = new RESTMessageSender("POST", "http://localhost:3000/events.xml");

        public IndieMetricsWrapper(string gameShortname, string gameKey)
        {
            _GameKey = gameKey;
            _GameShortname = gameShortname;
        }

        // Record an event with the given ID and message body
        public string RecordEvent(int eventID, string messageBody) {
            string requestBody = makeEventPostMessage(eventID, messageBody);
            _EventPostSender.MakeRequest(requestBody);
            return _EventPostSender.GetResponse();
        }

        // Creates the actual XML for the event message that will be POSTed to IndieMetrics
        private string makeEventPostMessage(int eventID, string eventXML)
        {
            string timestamp = DateTime.Now.ToString("U"); // "Tue Jan 20 15:59:05 -0500 2009";
            return _XMLBegin + "<event><event_type_id>" + eventID + "</event_type_id>" + eventXML + "<timestamp>" + timestamp + "</timestamp></event>";
        }

    }
}
