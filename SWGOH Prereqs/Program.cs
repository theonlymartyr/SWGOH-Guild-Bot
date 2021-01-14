using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Exceptions;
using DSharpPlus.Interactivity;
using Newtonsoft.Json;

namespace SWGOH
{
    public class Program
    {
        private static bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool isOk = true;

            if (sslPolicyErrors != SslPolicyErrors.None)
            {
                foreach (X509ChainStatus cs in chain.ChainStatus)
                {
                    if (cs.Status != X509ChainStatusFlags.RevocationStatusUnknown)
                    {
                        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                        chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                        chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                        bool chainIsValid = chain.Build((X509Certificate2)certificate);
                        if (!chainIsValid)
                        {
                            isOk = false;
                        }
                    }
                }
            }

            return isOk;
        }
        //https://discordapp.com/api/oauth2/authorize?client_id=572518319991685120&permissions=68608&scope=bot
        public DiscordClient Client { get; set; }
        public InteractivityModule Interactivity { get; set; }
        public CommandsNextModule Commands { get; set; }
        public ConfigJson cfgjson;
        public static void Main(string[] args)
        {
            // since we cannot make the entry method asynchronous, let's pass the execution to asynchronous code
            var prog = new Program();
            prog.RunBotAsync().GetAwaiter().GetResult();
        }

        public async Task RunBotAsync()
        {
            ServicePointManager.ServerCertificateValidationCallback = (s, cert, chain, ssl) => true;
            // first, let's load our configuration file
            var json = "";
            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync();

            // next, let's load the values from that file to our client's configuration
            cfgjson = JsonConvert.DeserializeObject<ConfigJson>(json);
            var cfg = new DiscordConfiguration
            {
                Token = cfgjson.Token,
                TokenType = TokenType.Bot,

                AutoReconnect = true,
                LogLevel = LogLevel.Info,
                UseInternalLogHandler = true
            };



            // then we want to instantiate our client
            this.Client = new DiscordClient(cfg);

            // next, let's hook some events, so we know what's going on
            this.Client.Ready += this.Client_Ready;
            this.Client.GuildAvailable += this.Client_GuildAvailable;
            this.Client.ClientErrored += this.Client_ClientError;

            // let's enable interactivity, and set default options
            this.Client.UseInteractivity(new InteractivityConfiguration
            {
                // default pagination behaviour to just ignore the reactions
                PaginationBehaviour = TimeoutBehaviour.Ignore,

                // default pagination timeout to 5 minutes
                PaginationTimeout = TimeSpan.FromMinutes(5),

                // default timeout for other actions to 2 minutes
                Timeout = TimeSpan.FromMinutes(2)
            });

            // up next, let's set up our commands
            var ccfg = new CommandsNextConfiguration
            {
                // let's use the string prefix defined in config.json
                StringPrefix = cfgjson.CommandPrefix,

                // enable responding in direct messages
                EnableDms = true,

                // enable mentioning the bot as a command prefix
                EnableMentionPrefix = true,

                EnableDefaultHelp = false
            };

            // and hook them up
            this.Commands = this.Client.UseCommandsNext(ccfg);

            // let's hook some command events, so we know what's going on
            this.Commands.CommandExecuted += this.Commands_CommandExecuted;
            this.Commands.CommandErrored += this.Commands_CommandErrored;

            // up next, let's register our commands
            this.Commands.RegisterCommands<Commands>();
            this.Commands.RegisterCommands<TestCommands>();


            //await this.Client.UpdateStatusAsync(dg);

            // finally, let's connect and log in
            await this.Client.ConnectAsync();
            //test();
            while (true)
            {
                checkForReminders();
                //Check every minute for assigned tasks
                Thread.Sleep(60000);
            }

            // when the bot is running, try doing <prefix>help to see the list of registered commands, and <prefix>help <command> to see help about specific command.
            // and this is to prevent premature quitting
            await Task.Delay(-1);
        }



        private Task Client_Ready(ReadyEventArgs e)
        {
            // let's log the fact that this event occured
            Assembly.GetExecutingAssembly().GetName().Version.ToString();

            e.Client.DebugLogger.LogMessage(LogLevel.Info, "SWGOHBot", "Client is ready to process events.", DateTime.Now);
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "SWGOHBot", $"Running Client Version: {Assembly.GetExecutingAssembly().GetName().Version.ToString()}", DateTime.Now);
            DiscordGame dg = new DiscordGame(cfgjson.CommandPrefix + "help  |  version:" + Assembly.GetExecutingAssembly().GetName().Version.ToString());
            this.Client.UpdateStatusAsync(dg);
            // since this method is not async, let's return a completed task, so that no additional work is done
            return Task.CompletedTask;
        }

        private Task Client_GuildAvailable(GuildCreateEventArgs e)
        {
            // let's log the name of the guild that was just sent to our client
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "SWGOHBot", $"Guild available: {e.Guild.Name}", DateTime.Now);


            // since this method is not async, let's return a completed task, so that no additional work is done
            return Task.CompletedTask;
        }

        private Task Client_ClientError(ClientErrorEventArgs e)
        {
            // let's log the details of the error that just occured in our client
            e.Client.DebugLogger.LogMessage(LogLevel.Error, "SWGOHBot", $"Exception occured: {e.Exception.GetType()}: {e.Exception.Message}", DateTime.Now);

            // since this method is not async, let's return a completed task, so that no additional work is done
            return Task.CompletedTask;
        }

        private Task Commands_CommandExecuted(CommandExecutionEventArgs e)
        {
            // let's log the name of the command and user
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Info, "SWGOHBot", $"{e.Context.User.Username} successfully executed '{e.Command.QualifiedName}'", DateTime.Now);

            // since this method is not async, let's return a completed task, so that no additional work is done
            return Task.CompletedTask;
        }

        private async Task Commands_CommandErrored(CommandErrorEventArgs e)
        {
            loadJSON();
            // let's log the error details
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Error, "SWGOHBot", $"{e.Context.User.Username} tried executing '{e.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {e.Exception.GetType()}: {e.Exception.Message ?? "<no message>"}", DateTime.Now);

            // let's check if the error is a result of lack of required permissions
            if (e.Exception is ChecksFailedException ex)
            {

                // yes, the user lacks required permissions,  let them know
                var emoji = DiscordEmoji.FromName(e.Context.Client, ":no_entry:");

                // let's wrap the response into an embed
                var embed = new DiscordEmbedBuilder
                {
                    Title = "Access denied",
                    Description = $"{emoji} You do not have the permissions required to execute this command.",
                    Color = new DiscordColor(0xFF0000) // red
                };
                await e.Context.RespondAsync("", embed: embed);
            }
            if (e.Exception is NotFoundException nfe)
            {
                String s = $":scream:The command must be ran with either 2 arguements or a Discord user.";
                // let's wrap the response into an embed
                var embed = new DiscordEmbedBuilder
                {
                    Title = "Uh Oh! Looks like there was a problem!",
                    Description = s,//$"{emoji} I need to know who you want me to look for. Example: thrawn",
                    Color = new DiscordColor(0xFF0000) // red
                };
                await e.Context.RespondAsync("", embed: embed);
            }

            if (e.Exception is ArgumentException)
            {

                String s = "";

                if (!AllyCode.IsValid(Convert.ToUInt32(e.Exception.Data["allycode"])))
                {
                    s = $":scream:The command must be ran with either 2 arguements or a Discord user.";
                }
                else { }
                var emoji = DiscordEmoji.FromName(e.Context.Client, ":no_entry:");

                // let's wrap the response into an embed
                var embed = new DiscordEmbedBuilder
                {
                    Title = "Uh Oh! Looks like there was a problem!",
                    Description = s,//$"{emoji} I need to know who you want me to look for. Example: thrawn",
                    Color = new DiscordColor(0xFF0000) // red
                };
                await e.Context.RespondAsync("", embed: embed);
            }
        }
        public async void loadJSON()
        {
            var json = "";
            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync();

            // next, let's load the values from that file to our client's configuration
            cfgjson = JsonConvert.DeserializeObject<ConfigJson>(json);
        }
        public void test()
        {
            SWGOH_Prereqs.CharacterStrings d = new SWGOH_Prereqs.CharacterStrings();
            String toons = JsonConvert.SerializeObject(d.toonsList);

            File.WriteAllText((RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "" : $@"C:\Users\jake\Documents\swgoh\SWGOH Prereqs\SWGOH Prereqs") + @"\Data\toons.txt", toons.ToString());
        }
        public void checkForReminders()
        {
            Console.WriteLine("This is only a test");
        }
    }

    // this structure will hold data from config.json
    public struct ConfigJson
    {
        [JsonProperty("token")]
        public string Token { get; private set; }

        [JsonProperty("prefix")]
        public string CommandPrefix { get; private set; }
    }
}
