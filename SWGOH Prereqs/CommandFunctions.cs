using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SWGOH;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace SWGOH
{
    class CommandFunctions
    {
        public CommandFunctions() { }
        public uint parseAllycode(string allycode)
        {
            try
            {
                return Convert.ToUInt32(allycode.Replace("-", ""));
            }
            catch (Exception e)
            {
                return 1;
            }
        }
        public uint checkAllycode(CommandContext ctx, string allycode)
        {
            uint ss, setCode;
            if (allycode.Contains("-"))
            {
                setCode = parseAllycode(allycode);
            }
            else
            {
                if (UInt32.TryParse(allycode, out ss))
                {
                    setCode = parseAllycode(ss.ToString());
                }
                else
                {
                    setCode = parseAllycode(checkRegistered(ctx.Member.Id.ToString()));
                }
            }
            return setCode;
        }
        public swgohHelpApiHelper login()
        {
            // first, let's load our configuration file
            var json = "";
            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = sr.ReadToEnd();
            var cfgjson = JsonConvert.DeserializeObject<ConfigJson>(json);
            UserSettings test = new UserSettings();
            test.username = cfgjson.username;
            test.password = cfgjson.password;
            swgohHelpApiHelper helper = new swgohHelpApiHelper(test);
            if (!helper.loggedIn) { helper.login(); }
            return helper;
        }
        private struct ConfigJson
        {
            [JsonProperty("username")]
            public string username { get; private set; }

            [JsonProperty("password")]
            public string password { get; private set; }
        }
        public string checkRegistered(string userID)
        {
            JObject users;
            JArray user;
            bool found = false;
            Console.WriteLine("Checking users...");
            users = JObject.Parse(File.ReadAllText(detectOS() + @"Data/users.txt"));
            user = (JArray)users["users"];
            foreach (JObject obj in user.Children())
            {
                if (obj.Property("id").Value.ToString().Equals(userID))
                {
                    found = true;
                    return obj.Property("allycode").Value.ToString();
                }
            }
            return "";
        }
        public string detectOS()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return $@"C:\Users\jake\Documents\swgoh\SWGOH Prereqs\SWGOH Prereqs\";
            }
            return "";
        }

        public long backtrack(int i, long sum, long max, GuildParse.Roster[] r)
        {
            Console.WriteLine($"i:{i}  sum:{ sum}   max:{max}  size:{r.Length}");
            if (sum > max) { return 0; }
            if (i == r.Length) { return sum; }

            long pick = backtrack(i + 1, sum + r[i].Gp, max, r);
            long leave = backtrack(i + 1, sum, max, r);
            //Thread.Sleep(30000);
            return Math.Max(pick, leave);
        }
        public int findTargetSumWays(long[] nums, int S)
        {
            long sum = 0;
            foreach (long num in nums) sum += num;
            if (S > sum || -S < -sum || (S + sum) % 2 == 1) return 0;

            int[] dp = new int[(S + sum) / 2 + 1];
            dp[0] = 1;

            foreach (int num in nums)
            {
                for (int i = dp.Length - 1; i >= num; i--)
                {
                    dp[i] += dp[i - num]; // Crux
                }
            }

            return dp[dp.Length - 1];
        }
    }
}
