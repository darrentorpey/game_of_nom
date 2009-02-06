using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace Connecting
{
    public class RESTMessageSender
    {
        string _responseText;

        string _requestMethod;
        string _requestURL;
        private static string s_ContentType = "application/xml";

        public RESTMessageSender(string httpMethod, string requestURL)
        {
            _requestMethod = httpMethod;
            _requestURL = requestURL;
        }

		public void MakeRequest(string messageBody)
		{
			try
			{
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_requestURL);
				request.Method = _requestMethod;
                request.ContentType = s_ContentType;
                setBody(request, messageBody);

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                _responseText = convertResponseToString(response);
			}
			catch (Exception ex)
			{
                _responseText += "ERROR: " + ex.Message;
                Console.WriteLine("Event record fail: " + ex.Message);
                throw ex;
			}
		}

        // Get the response from the last request
        public string GetResponse()
        {
            return _responseText;
        }

        private
	        void setBody(HttpWebRequest request, string requestBody)
	        {
		        if (requestBody.Length > 0)
		        {
			        using (Stream requestStream = request.GetRequestStream())
			        using (StreamWriter writer = new StreamWriter(requestStream))
			        {
				        writer.Write(requestBody);
			        }
		        }
	        }

	        string convertResponseToString(HttpWebResponse response)
	        {
		        string result = "Status code: " + (int)response.StatusCode + " " + response.StatusCode + "\r\n";

		        foreach (string key in response.Headers.Keys)
		        {
			        result += string.Format("{0}: {1} \r\n", key, response.Headers[key]);
		        }

		        result += "\r\n";
		        result += new StreamReader(response.GetResponseStream()).ReadToEnd();

		        return result;
	        }
    }
}
