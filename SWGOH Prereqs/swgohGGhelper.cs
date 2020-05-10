using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace SWGOH_Prereqs
{
    class swgohGGhelper
    {
        public string getGGinfo(uint allycode)
        {
            try
            {
                string url = "https://swgoh.gg/api/player/" + allycode + "/";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "application/json";
                request.Timeout = 300000;
                WebResponse response = request.GetResponse();
                var dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                var apiResponse = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
                response.Close();

                return apiResponse;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string getGGguild(long guildID)
        {
            string url = "https://swgoh.gg/api/guild/" + guildID + "/";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Timeout = 300000;
            WebResponse response = request.GetResponse();
            var dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            var apiResponse = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();

            return apiResponse;
        }
        public string syncPLayer(uint allycode)
        {
            string url = $"http://swgoh.gg/api/players/{allycode}/trigger-sync/";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Timeout = 300000;
            WebResponse response = request.GetResponse();
            var dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            var apiResponse = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();

            return apiResponse;
        }
        public string token()
        {
            string url = $"http://swgoh.gg/api/token-balance";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Timeout = 300000;
            WebResponse response = request.GetResponse();
            var dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            var apiResponse = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();

            return apiResponse;
        }
    }
}
