using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

namespace SWGOH
{
    public class swgohHelpApiHelper
    {
        string user = "";
        string token = "";
        string url = "";
        string signin = "";
        string data = "";
        string player = "";
        string units = "";
        string zetas = "";
        string squads = "";
        string events = "";
        string battles = "";
        string guild = "";
        DateTime signintime;
        public bool loggedIn = false;

        private HttpClient client = new HttpClient();

        public swgohHelpApiHelper(UserSettings settings)
        {

            user = "username=" + settings.username;
            user += "&password=" + settings.password;
            user += "&grant_type=password";
            user += "&client_id=" + settings.client_id;
            user += "&client_secret=" + settings.client_secret;

            this.token = null;

            string protocol = string.IsNullOrEmpty(settings.protocol) ? "https" : settings.protocol;
            string host = string.IsNullOrEmpty(settings.host) ? "api.swgoh.help" : settings.host;
            string port = string.IsNullOrEmpty(settings.port) ? "" : settings.port;

            url = protocol + "://" + host + port;
            signin = url + "/auth/signin/";
            data = url + "/swgoh/data/";
            player = url + "/swgoh/player/";
            guild = url + "/swgoh/guild/";
            units = url + "/swgoh/roster/";
            zetas = url + "/swgoh/zetas/";
            squads = url + "/swgoh/squads/";
            events = url + "/swgoh/events/";
            battles = url + "/swgoh/battles/";
        }

        public bool login()
        {
            try
            {
                //If we don't have a token, try to load them from the file
                if (token == null || signintime.Equals(null)) { loadTokens(); }
                //Check to see if token is still valid. Doesn't matter if file doesn't exist, signintime will default to a time that is >3600 seconds from DateTime.Now
                //So token sill be set to null and a  new token will be grabbed.
                checkToken();
                if (string.IsNullOrEmpty(token))
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(this.signin);
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    byte[] byteArray = Encoding.UTF8.GetBytes(user);
                    request.ContentLength = byteArray.Length;

                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();
                    WebResponse response = request.GetResponse();
                    dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    var loginresponse = reader.ReadToEnd();
                    reader.Close();
                    dataStream.Close();
                    response.Close();

                    var loginResponseObject = JsonConvert.DeserializeObject<LoginResponse>(loginresponse);
                    token = loginResponseObject.access_token;
                    writeValues(token);
                    loggedIn = true;
                    return loggedIn;
                }
                return loggedIn;
            }
            catch (Exception e) { Console.WriteLine(e.Message + "\n\n" + e.StackTrace); return loggedIn; }
        }
        public void checkToken()
        {
            DateTime dt = DateTime.Now;
            if (dt > signintime.AddSeconds(3600) || (signintime.AddSeconds(3600) - dt).Seconds > 45) { token = null; loggedIn = false; Console.WriteLine("Token not valid"); } else { loggedIn = true; Console.WriteLine("Token is still valid"); }
        }

        public void writeValues(string json)
        {
            try
            {
                string text = $"{json},{DateTime.Now}";
                // WriteAllText creates a file, writes the specified string to the file,
                // and then closes the file.    You do NOT need to call Flush() or Close().
                File.WriteAllText(@"C:\Users\jake\Documents\swgoh\SWGOH Prereqs\SWGOH Prereqs\values.txt", text);
            }
            catch (Exception e) { Console.WriteLine(e.StackTrace); }
        }

        public void loadTokens()
        {
            try
            {
                string text = File.ReadAllText(@"C:\Users\jake\Documents\swgoh\SWGOH Prereqs\SWGOH Prereqs\values.txt");
                string[] keys = text.Split(",");
                token = keys[0];
                signintime = Convert.ToDateTime(keys[1]);
            }
            catch (Exception e) { Console.WriteLine(e.StackTrace); }
        }

        public string fetchApi(string url, object param)
        {
            try
            {
                Console.WriteLine(url);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers.Add("Authorization", "Bearer " + token + "");
                request.Timeout = 300000;
                string json = JsonConvert.SerializeObject(param);

                byte[] byteArray = Encoding.UTF8.GetBytes(param.ToString());

                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(json);
                }

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
                Console.WriteLine(e.HResult + ":" + e.Message);
                throw e;
            }
        }

        public string fetchPlayer(uint[] allycodes, string language = null, bool? enums = null, object project = null)
        {
            dynamic obj = new ExpandoObject();
            obj.allycodes = allycodes;
            if (language != null)
                obj.language = "ENG_US";
            if (enums.HasValue)
                obj.enums = enums;
            if (project != null)
                obj.project = project;

            string response = fetchApi(player, obj);

            return response;
        }

        public string fetchZetas(object project = null)
        {
            dynamic obj = new ExpandoObject();
            if (project != null)
                obj.project = project;

            var response = fetchApi(zetas, obj);

            return response;
        }

        public string fetchSquads(object project = null)
        {
            dynamic obj = new ExpandoObject();
            if (project != null)
                obj.project = project;

            var response = fetchApi(squads, obj);

            return response;
        }

        public string fetchEvents(string language = null, bool? enums = null, object project = null)
        {
            dynamic obj = new ExpandoObject();
            if (language != null)
                obj.language = language;
            if (enums.HasValue)
                obj.enums = enums;
            if (project != null)
                obj.project = project;

            var response = fetchApi(events, obj);

            return response;
        }

        public string fetchBattles(string language = null, bool? enums = null, object project = null)
        {
            dynamic obj = new ExpandoObject();
            if (language != null)
                obj.language = language;
            if (enums.HasValue)
                obj.enums = enums;
            if (project != null)
                obj.project = project;

            var response = fetchApi(battles, obj);

            return response;
        }

        public string fetchData(string collection, string language = null, bool? enums = null, object match = null, object project = null)
        {
            dynamic obj = new ExpandoObject();
            obj.collection = collection;
            if (language != null)
                obj.language = language;
            if (enums.HasValue)
                obj.enums = enums;
            if (match != null)
                obj.match = match;
            if (project != null)
                obj.project = project;

            var response = fetchApi(battles, obj);

            return response;
        }

        public string fetchUnits(uint[] allycodes, string language = null, bool? enums = null, bool? mods = null, object project = null)
        {
            dynamic obj = new ExpandoObject();
            obj.allycodes = allycodes;
            if (language != null)
                obj.language = language;
            if (enums.HasValue)
                obj.enums = enums;
            if (mods.HasValue)
                obj.mods = mods;
            if (project != null)
                obj.project = project;

            var response = fetchApi(units, obj);

            return response;
        }

        public string fetchGuild(uint[] allycodes, string language = null, bool? enums = null, bool? roster = null, bool? units = null, bool? mods = null, object project = null)
        {
            dynamic obj = new ExpandoObject();
            obj.allycodes = allycodes;
            if (language != null)
                obj.language = "ENG_US";
            if (enums.HasValue)
                obj.enums = enums;
            if (roster.HasValue)
                obj.roster = roster;
            if (units.HasValue)
                obj.units = units;
            if (mods.HasValue)
                obj.mods = mods;
            if (project != null)
                obj.project = project;

            var response = fetchApi(guild, obj);

            return response;
        }

        public string getStats(string player)
        {
            string baseURL = "https://swgoh-stat-calc.glitch.me/api?flags=calcGP,gameStyle";
            try
            {
                //Console.WriteLine("Fetching Zetas");
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseURL);
                request.Method = "POST";
                request.ContentType = "application/json";
                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(player);
                }

                WebResponse response = request.GetResponse();
                Console.WriteLine(((HttpWebResponse)response).StatusDescription);
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

    }
}