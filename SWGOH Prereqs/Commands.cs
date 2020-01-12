using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using Newtonsoft.Json;
using SWGOH_Prereqs;

namespace SWGOH
{
    // note that in here we explicitly ask for duration. This is optional,
    // since we set the defaults.
    public class Commands
    {
        swgohHelpApiHelper helper;
        Dictionary<string, string> toons = new Dictionary<string, string>()
        {
            { "MAGMATROOPER", "Magma Trooper" },
            {"HERMITYODA", "Hermit Yoda"},
            {"ZEBS3", "Zeb" },
            {"BARRISSOFFEE", "Barriss Offee" },
            {"CT210408", "ECHO" },
            {"B1BATTLEDROIDV2", "B1" },
            {"COLONELSTARCK", "Colonel Starck" },
            {"TIEFIGHTERFOSF", "FOSF Tie Fighter" },
            {"BB8", "BB8" },
            {"POGGLETHELESSER", "Poggle" },
            {"FIRSTORDERSPECIALFORCESPILOT", "FOSF Tie Pilot" },
            {"GENERALKENOBI", "GK" },
            {"JAWASCAVENGER", "Jawa Scavenger" },
            {"L3_37", "L337" },
            {"DROIDEKA", "Droideka" },
            {"EWOKSCOUT", "Ewok Scout" },
            {"SITHINFILTRATOR", "Scimitar" },
            {"K2SO", "K2SO" },
            {"SITHTROOPER", "Sith Trooper" },
            {"IG86SENTINELDROID", "IG 86" },
            {"SITHMARAUDER", "Sith Marauder" },
            {"REYJEDITRAINING", "Rey (Jedi Training)" },
            {"KYLOREN", "Kylo Ren" },
            {"DENGAR", "Dengar" },
            {"ARC170REX", "Arc 170" },
            {"TIEFIGHTERFIRSTORDER", "FO Tie Fighter" },
            {"MAGNAGUARD", "Magnaguard" },
            {"MILLENNIUMFALCONEP7", "Rey's Falcon" },
            {"RANGETROOPER", "Range Trooper" },
            {"QUIGONJINN", "Qui-gon Jinn" },
            {"ROYALGUARD", "Royal Guard" },
            {"GEONOSIANSPY", "Geo Spy" },
            {"BASTILASHAN", "Bastila Shan" },
            {"JAWA", "Jawa" },
            {"SITHFIGHTER", "Sith Fighter" },
            {"XWINGRESISTANCE", "Resistance X-Wing" },
            {"QIRA", "Qi'ra" },
            {"POE", "Poe Dameron" },
            {"TALIA", "Talia" },
            {"NIGHTSISTERZOMBIE", "NS Zombie" },
            {"SCARIFREBEL", "Scarif Rebel Pathfinder" },
            {"GRANDADMIRALTHRAWN", "Thrawn" },
            {"JEDISTARFIGHTERANAKIN", "Eta 2" },
            {"IMPERIALPROBEDROID", "IPD" },
            {"SABINEWRENS3", "Sabine" },
            {"PAPLOO", "Paploo" },
            {"YOUNGHAN", "Young Han" },
            {"EWOKELDER", "Ewok Elder" },
            {"CHIEFCHIRPA", "Chief Chirpa" },
            {"GENERALHUX", "General Hux" },
            {"CT5555", "Fives" },
            {"VISASMARR", "Visas Marr" },
            {"LUKESKYWALKER", "Farmboy Luke" },
            {"KITFISTO", "Kit Fisto" },
            {"HOUNDSTOOTH", "Hound's Tooth" },
            {"ADMINISTRATORLANDO", "Lando" },
            {"ANAKINKNIGHT", "JKA" },
            {"IG2000", "IG-2000" },
            {"NIGHTSISTERSPIRIT", "NS Spirit" },
            {"XANADUBLOOD", "Xanadu Blood" },
            {"CLONESERGEANTPHASEI", "Clone Sergeant" },
            {"ADMIRALACKBAR", "Admiral Ackbar" },
            {"FULCRUMAHSOKA", "Ahsoka (Fulcrum)" },
            {"STORMTROOPER", "Stormtrooper" },
            {"LUMINARAUNDULI", "Luminara" },
            {"JANGOFETT", "Jango Fett" },
            {"HOTHREBELSCOUT", "Hoth Rebel Scout" },
            {"GHOST", "Ghost" },
            {"NUTEGUNRAY", "Nute Gunray" },
            {"SLAVE1", "Slave 1" },
            {"TIEADVANCED", "Tie Advanced" },
            {"VEERS", "General Veers" },
            {"PAO", "Pao" },
            {"IG88", "IG-88" },
            {"MILLENNIUMFALCONPRISTINE", "Lando's Falcon" },
            {"EBONHAWK", "Ebon Hawk" },
            {"REY", "Scavenger Rey" },
            {"BLADEOFDORIN", "Plo Koon's Star Fighter" },
            {"DARTHNIHILUS", "DN" },
            {"EMPERORPALPATINE", "Palpatine" },
            {"PLOKOON", "Plo Koon" },
            {"DAKA", "Daka" },
            {"GRANDMOFFTARKIN", "Tarkin" },
            {"LOGRAY", "Logray" },
            {"GEONOSIANSOLDIER", "Geo Soldier" },
            {"VULTUREDROID", "Vulture Droid" },
            {"RESISTANCETROOPER", "Resistance Trooper" },
            {"HK47", "HK-47" },
            {"TUSKENRAIDER", "Tusken Raider" },
            {"CASSIANANDOR", "Cassian" },
            {"DARTHTRAYA", "Traya" },
            {"BAZEMALBUS", "Baze" },
            {"EPIXFINN", "Res Hero Finn" },
            {"ZAALBAR", "Zaalbar" },
            {"KCLONEWARSCHEWBACCA2SO", "CW Chewbacca" },
            {"COMMANDERLUKESKYWALKER", "CLS" },
            {"B2SUPERBATTLEDROID", "B2" },
            {"UWINGROGUEONE", "Cassian's U-Wing" },
            {"DARTHSIDIOUS", "Sidious" },
            {"OLDBENKENOBI", "Old Ben" },
            {"KYLORENUNMASKED", "Kylo Ren (Unmasked)" },
            {"YOUNGCHEWBACCA", "Young Chewbacca" },
            {"YOUNGLANDO", "Young Lando" },
            {"HANSOLO", "Han Solo" },
            {"SMUGGLERHAN", "Vet Han" },
            {"UGNAUGHT", "Ugnaught" },
            {"FOSITHTROOPER", "FO Sith Trooper" },
            {"UMBARANSTARFIGHTER", "Umbaran" },
            {"WAMPA", "Wampa" },
            {"SMUGGLERCHEWBACCA", "Vet Chewbacca" },
            {"R2D2_LEGENDARY", "R2D2" },
            {"UWINGSCARIF", "Bistan's U-Wing" },
            {"HUMANTHUG", "Mob Enforcer" },
            {"GARSAXON", "Gar Saxon" },
            {"CORUSCANTUNDERWORLDPOLICE", "CUP" },
            {"ENFYSNEST", "Nest" },
            {"MOTHERTALZIN", "MT" },
            {"BIGGSDARKLIGHTER", "Biggs" },
            {"CC2224", "Cody" },
            {"C3POLEGENDARY", "C3PO" },
            {"JEDIKNIGHTREVAN", "JKR" },
            {"PADMEAMIDALA", "Padme" },
            {"NIGHTSISTERINITIATE", "NS Initiate" },
            {"CT7567", "Rex" },
            {"EMPERORSSHUTTLE", "Emperor's Shuttle" },
            {"ZAMWESELL", "Zam" },
            {"CAPITALMONCALAMARICRUISER", "Home One" },
            {"SUNFAC", "Sun Fac" },
            {"LOBOT", "Lobot" },
            {"EMBO", "Embo" },
            {"IMAGUNDI", "Ima Gun Di" },
            {"TIESILENCER", "Tie Silencer" },
            {"YWINGCLONEWARS", "Y-Wing" },
            {"XWINGRED3", "Biggs' X-Wing" },
            {"EETHKOTH", "Eeth Koth" },
            {"CAPITALSTARDESTROYER", "Executrix" },
            {"T3_M4", "T3-M4" },
            {"XWINGRED2", "Wedge's X-Wing" },
            {"JEDISTARFIGHTERAHSOKATANO", "Ahsoka's Star Fighter" },
            {"HOTHREBELSOLDIER", "Hoth Rebel Soldier" },
            {"ASAJVENTRESS", "Asajj Ventress" },
            {"GRIEVOUS", "GG" },
            {"CHIRRUTIMWE", "Chirrut" },
            {"JUHANI", "Juhani" },
            {"CAPITALNEGOTIATOR", "Negotiator" },
            {"KANANJARRUSS3", "Kanan" },
            {"ROSETICO", "Rose Tico" },
            {"JEDIKNIGHTCONSULAR", "Consular" },
            {"AHSOKATANO", "Ahsoka Tano" },
            {"DIRECTORKRENNIC", "Krennic" },
            {"TIEFIGHTERIMPERIAL", "Imperial Tie Fighter" },
            {"STORMTROOPERHAN", "ST Han" },
            {"HERASYNDULLAS3", "Hera" },
            {"BASTILASHANDARK", "BSF" },
            {"BOSSK", "Bossk" },
            {"FIRSTORDEREXECUTIONER", "FOX" },
            {"VADER", "Vader" },
            {"COUNTDOOKU", "Count Dooku" },
            {"MISSIONVAO", "Mission Vao" },
            {"ARC170CLONESERGEANT", "Sergeant's ARC 170" },
            {"GEONOSIANBROODALPHA", "GBA" },
            {"MAUL", "Darth Maul" },
            {"JOLEEBINDO", "Jolee" },
            {"CHIEFNEBIT", "Chief Nebit" },
            {"TUSKENSHAMAN", "Tusken Shaman" },
            {"IMPERIALSUPERCOMMANDO", "Imperial Super Commando" },
            {"FIRSTORDERTIEPILOT", "FO Tie Pilot" },
            {"DARTHSION", "Sion" },
            {"CAPITALJEDICRUISER", "Endurance" },
            {"CAPITALCHIMAERA", "Chimaera" },
            {"COMMANDSHUTTLE", "Command Shuttle" },
            {"FIRSTORDERTROOPER", "FO Stormtrooper" },
            {"PHANTOM2", "Phantom" },
            {"HOTHLEIA", "ROLO" },
            {"GREEDO", "Greedo" },
            {"BODHIROOK", "Bodhi Rook" },
            {"CHEWBACCALEGENDARY", "Chewbacca" },
            {"CHOPPERS3", "Chopper" },
            {"BOBAFETT", "Boba Fett" },
            {"WEDGEANTILLES", "Wedge" },
            {"DEATHTROOPER", "Death Trooper" },
            {"CARTHONASI", "Carth" },
            {"ARCTROOPER501ST", "ARC Trooper" },
            {"WICKET", "Wicket" },
            {"GEONOSIANSTARFIGHTER2", "Geo Soldier's Starfighter" },
            {"GEONOSIANSTARFIGHTER1", "Sun Fac's Starfighter" },
            {"GEONOSIANSTARFIGHTER3", "Geo Spy's Starfighter" },
            {"MACEWINDU", "Mace" },
            {"DATHCHA", "Datcha" },
            {"JYNERSO", "Jyn Erso" },
            {"TEEBO", "Teebo" },
            {"XWINGBLACKONE", "Poe's X-Wing" },
            {"SITHBOMBER", "B28" },
            {"AMILYNHOLDO", "Holdo" },
            {"GAUNTLETSTARFIGHTER", "Gauntlet Starfighter" },
            {"SITHASSASSIN", "Sith Assassin" },
            {"SNOWTROOPER", "Snowtrooper" },
            {"SHORETROOPER", "Shoretrooper" },
            {"PRINCESSLEIA", "Princess Leia" },
            {"SAVAGEOPRESS", "Savage" },
            {"FINN", "Finn" },
            {"PHASMA", "Captain Phasma" },
            {"EZRABRIDGERS3", "Ezra" },
            {"TIEREAPER", "Tie Reaper" },
            {"HYENABOMBER", "Hyena Bomber" },
            {"JEDISTARFIGHTERCONSULAR", "Consular's Starfighter" },
            {"FIRSTORDEROFFICERMALE", "FO Officer" },
            {"RESISTANCEPILOT", "Resistance Pilot" },
            {"AURRA_SING", "Aurra Sing" },
            {"JEDIKNIGHTGUARDIAN", "JKG" },
            {"CANDEROUSORDO", "Canderous" },
            {"CADBANE", "Cad Bane" },
            {"TIEFIGHTERPILOT", "TFP" },
            {"GRANDMASTERYODA", "GMY" },
            {"SHAAKTI", "Shaak Ti" },
            {"BISTAN", "Bistan" },
            {"DARTHMALAK", "Malak" },
            {"JAWAENGINEER", "Jawa Engineer" },
            {"AAYLASECURA", "Aayla Secura" },
            {"WATTAMBOR", "Wat Tambor" },
            {"DARTHREVAN", "DR" },
            {"URORRURRR", "URoRRuR'R'R" },
            {"NIGHTSISTERACOLYTE", "NS Acolyte" },
            {"GENERALSKYWALKER", "Gen Skywalker" },
            {"HOTHHAN", "Captain Han" },
            {"GAMORREANGUARD", "Gamorrean Guard" }

        };
        [Command("gs"), Description("Gets guild stats")]
        public async Task getPrereqs(CommandContext ctx, [Description("Ally Code to lookup")] uint allycode)
        {
            try
            {
                await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":stopwatch:"));
                DateTime start = DateTime.Now;
                // first retrieve the interactivity module from the client
                var interactivity = ctx.Client.GetInteractivityModule();
                String s = "";
                //login to the API
                login();

                GuildParse.Guild guilds = getGuild(new uint[] { allycode });
                if (guilds.guild.Length > 0)
                {
                    int maxWidth = 75;
                    string header = $"{guilds.guild[0].Name} Stats ";
                    int space = maxWidth - header.Length;

                    PlayerParse.Player players1 = getGuildMembers(buildMemberArray(guilds.guild[0].Roster));
                    GuildParse.GuildMember guild = guilds.guild[0];
                    //Build our output
                    s += "**" + header + "**\n";
                    s += "======= Overview =======```\n";
                    s += createLine("Members:", guild.Members.ToString());
                    s += createLine("Total GP:", (guild.Gp / 1000000).ToString() + "M");
                    s += createLine("Character GP:", buildCharGP(guild.Roster, "Char").ToString() + "M");
                    s += createLine("Fleet GP:", buildCharGP(guild.Roster, "Fleet").ToString() + "M");
                    s += "```";
                    await ctx.RespondAsync(ctx.Member.Mention + " Here is the information you requested:");
                    await ctx.RespondAsync(s);
                    s = "";
                    var embed = new DiscordEmbedBuilder
                    {
                        Title = "Stats",
                        Color = new DiscordColor(0xFF0000) // red
                    };
                    var embed2 = new DiscordEmbedBuilder
                    {
                        Title = "Stats",
                        Color = new DiscordColor(0xFF0000) // red
                    };

                    buildPlayerStats(players1, embed, embed2);

                    await ctx.RespondAsync("", embed: embed);
                    await ctx.RespondAsync("", embed: embed2);
                }
                DateTime end = DateTime.Now;
                Console.WriteLine((end - start).TotalSeconds);
                await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":thumbsup:"));
            }
            catch (Exception e) { Console.WriteLine(e.StackTrace); await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":thumbsdown:")); }

        }
        [Command("tw"), Description("Compares 2 guilds for TW")]
        public async Task TWCompare(CommandContext ctx, [Description("Ally code of one guild")] uint allycode1, [Description("Ally Code of other guild")] uint allycode2)
        {
            try
            {
                await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":stopwatch:"));
                DateTime start = DateTime.Now;
                // first retrieve the interactivity module from the client
                var interactivity = ctx.Client.GetInteractivityModule();
                String s = "", title = "", embeds = "";
                //login to the API
                login();
                //Retreive the 2 guilds to compare using an allycode for a member of each guild
                //If both allycodes are from the same guild, only one guild will return, make sure to check for that
                GuildParse.Guild guilds = getGuild(new uint[] { allycode1, allycode2 });
                if (guilds.guild.Length > 1)
                {
                    var embed = new DiscordEmbedBuilder
                    {
                        Title = "Stats",
                        Color = new DiscordColor(0xFF0000) // red
                    };
                    int maxWidth = 75;
                    string header = $"{guilds.guild[0].Name}  VS  {guilds.guild[1].Name}";
                    int space = maxWidth - header.Length;

                    PlayerParse.Player players1 = getGuildMembers(buildMemberArray(guilds.guild[0].Roster)), players2 = getGuildMembers(buildMemberArray(guilds.guild[1].Roster));
                    GuildParse.GuildMember guild1 = guilds.guild[0], guild2 = guilds.guild[1];
                    //Build our output
                    s += "**" + header + "**\n";
                    s += "======= Overview =======```\n";
                    s += createHeaderLine("Members:", guild1.Members.ToString(), guild2.Members.ToString());
                    s += createHeaderLine("Total GP:", (guild1.Gp / 1000000).ToString() + "M", (guild2.Gp / 1000000).ToString() + "M");
                    s += createHeaderLine("Character GP:", buildCharGP(guild1.Roster, "Char").ToString() + "M", buildCharGP(guild2.Roster, "Char").ToString() + "M");
                    s += createHeaderLine("Fleet GP:", buildCharGP(guild1.Roster, "Fleet").ToString() + "M", buildCharGP(guild2.Roster, "Fleet").ToString() + "M");
                    s += "```";
                    //await ctx.RespondAsync(ctx.Member.Mention + " Here is the information you requested:");
                    // await ctx.RespondAsync(s);
                    embed.Description = s;
                    ToonStats ts1 = new ToonStats(), ts2 = new ToonStats();
                    getGuildStats(players1, ts1);
                    getGuildStats(players2, ts2);
                    title = "==Gear==\n";
                    embeds += "```CSS\n";
                    embeds += createLine("G11:", ts1.G11.ToString(), ts2.G11.ToString());
                    embeds += createLine("G12:", ts1.G12.ToString(), ts2.G12.ToString());
                    embeds += createLine("G12+1:", ts1.G121.ToString(), ts2.G121.ToString());
                    embeds += createLine("G12+2:", ts1.G122.ToString(), ts2.G122.ToString());
                    embeds += createLine("G12+3:", ts1.G123.ToString(), ts2.G123.ToString());
                    embeds += createLine("G12+4:", ts1.G124.ToString(), ts2.G124.ToString());
                    embeds += createLine("G12+5:", ts1.G125.ToString(), ts2.G125.ToString());
                    embeds += createLine("G13:", ts1.G13.ToString(), ts2.G13.ToString());
                    embeds += "```";
                    embed.AddField($"{title}", embeds, true);
                    title = "==Relics==";
                    embeds = "```CSS\n";
                    embeds += createLine("Total Relics:", ts1.TotalRelics.ToString(), ts2.TotalRelics.ToString());
                    embeds += createLine("Relic 0:", ts1.relics[0].ToString(), ts2.relics[0].ToString());
                    embeds += createLine("Relic 1:", ts1.relics[1].ToString(), ts2.relics[1].ToString());
                    embeds += createLine("Relic 2:", ts1.relics[2].ToString(), ts2.relics[2].ToString());
                    embeds += createLine("Relic 3:", ts1.relics[3].ToString(), ts2.relics[3].ToString());
                    embeds += createLine("Relic 4:", ts1.relics[4].ToString(), ts2.relics[4].ToString());
                    embeds += createLine("Relic 5:", ts1.relics[5].ToString(), ts2.relics[5].ToString());
                    embeds += createLine("Relic 6:", ts1.relics[6].ToString(), ts2.relics[6].ToString());
                    embeds += createLine("Relic 7:", ts1.relics[7].ToString(), ts2.relics[7].ToString());
                    embeds += "```";
                    embed.AddField($"{title}", embeds, true);
                    title = "=Malak=";
                    embeds = "```CSS\n";
                    embeds += createLine("Total:", ts1.dm.Total.ToString(), ts2.dm.Total.ToString());
                    embeds += createLine("7*:", ts1.dm.stars[2].ToString(), ts2.dm.stars[2].ToString());
                    embeds += createLine("6*:", ts1.dm.stars[1].ToString(), ts2.dm.stars[1].ToString());
                    embeds += createLine("5*:", ts1.dm.stars[0].ToString(), ts2.dm.stars[0].ToString());
                    embeds += createLine("G11:", ts1.dm.stars[0].ToString(), ts2.dm.stars[0].ToString());
                    embeds += createLine("G12:", ts1.dm.stars[1].ToString(), ts2.dm.stars[1].ToString());
                    embeds += createLine("G13:", ts1.dm.stars[2].ToString(), ts2.dm.stars[2].ToString());
                    embeds += createLine("Relic 4+:", (ts1.dm.relics[4] + ts1.dm.relics[5] + ts1.dm.relics[6] + ts1.dm.relics[7]).ToString(), (ts2.dm.relics[4] + ts2.dm.relics[5] + ts2.dm.relics[6] + ts2.dm.relics[7]).ToString());
                    embeds += createLine("Relic 7:", ts1.dm.relics[7].ToString(), ts2.dm.relics[7].ToString());
                    embeds += createLine("GP 16K+:", ts1.dm.gp16.ToString(), ts2.dm.gp16.ToString());
                    embeds += createLine("GP 20K+:", ts1.dm.gp20.ToString(), ts2.dm.gp20.ToString());
                    embeds += "```";
                    embed.AddField($"{title}", embeds, true);
                    title = "=Padme=";
                    embeds = "```CSS\n";
                    embeds += createLine("Total:", ts1.padme.Total.ToString(), ts2.padme.Total.ToString());
                    embeds += createLine("7*:", ts1.padme.stars[2].ToString(), ts2.padme.stars[2].ToString());
                    embeds += createLine("6*:", ts1.padme.stars[1].ToString(), ts2.padme.stars[1].ToString());
                    embeds += createLine("5*:", ts1.padme.stars[0].ToString(), ts2.padme.stars[0].ToString());
                    embeds += createLine("G11:", ts1.padme.stars[0].ToString(), ts2.padme.stars[0].ToString());
                    embeds += createLine("G12:", ts1.padme.stars[1].ToString(), ts2.padme.stars[1].ToString());
                    embeds += createLine("G13:", ts1.padme.stars[2].ToString(), ts2.padme.stars[2].ToString());
                    embeds += createLine("Relic 4+:", (ts1.padme.relics[4] + ts1.padme.relics[5] + ts1.padme.relics[6] + ts1.padme.relics[7]).ToString(), (ts2.padme.relics[4] + ts2.padme.relics[5] + ts2.padme.relics[6] + ts2.padme.relics[7]).ToString());
                    embeds += createLine("Relic 7:", ts1.padme.relics[7].ToString(), ts2.padme.relics[7].ToString());
                    embeds += createLine("GP 16K+:", ts1.padme.gp16.ToString(), ts2.padme.gp16.ToString());
                    embeds += createLine("GP 20K+:", ts1.padme.gp20.ToString(), ts2.padme.gp20.ToString());
                    embeds += "```";
                    embed.AddField($"{title}", embeds, true);
                    title = "=JKR=";
                    embeds = "```CSS\n";
                    embeds += createLine("Total:", ts1.jkr.Total.ToString(), ts2.padme.Total.ToString());
                    embeds += createLine("7*:", ts1.jkr.stars[2].ToString(), ts2.jkr.stars[2].ToString());
                    embeds += createLine("6*:", ts1.jkr.stars[1].ToString(), ts2.jkr.stars[1].ToString());
                    embeds += createLine("5*:", ts1.jkr.stars[0].ToString(), ts2.jkr.stars[0].ToString());
                    embeds += createLine("G11:", ts1.jkr.stars[0].ToString(), ts2.jkr.stars[0].ToString());
                    embeds += createLine("G12:", ts1.jkr.stars[1].ToString(), ts2.jkr.stars[1].ToString());
                    embeds += createLine("G13:", ts1.jkr.stars[2].ToString(), ts2.jkr.stars[2].ToString());
                    embeds += createLine("Relic 4+:", (ts1.jkr.relics[4] + ts1.jkr.relics[5] + ts1.jkr.relics[6] + ts1.jkr.relics[7]).ToString(), (ts2.jkr.relics[4] + ts2.jkr.relics[5] + ts2.jkr.relics[6] + ts2.jkr.relics[7]).ToString());
                    embeds += createLine("Relic 7:", ts1.jkr.relics[7].ToString(), ts2.jkr.relics[7].ToString());
                    embeds += createLine("GP 16K+:", ts1.jkr.gp16.ToString(), ts2.jkr.gp16.ToString());
                    embeds += createLine("GP 20K+:", ts1.jkr.gp20.ToString(), ts2.jkr.gp20.ToString());
                    embeds += "```";
                    embed.AddField($"{title}", embeds, true);
                    title = "=Grievous=";
                    embeds = "```CSS\n";
                    embeds += createLine("Total:", ts1.grievous.Total.ToString(), ts2.grievous.Total.ToString());
                    embeds += createLine("7*:", ts1.grievous.stars[2].ToString(), ts2.grievous.stars[2].ToString());
                    embeds += createLine("6*:", ts1.grievous.stars[1].ToString(), ts2.grievous.stars[1].ToString());
                    embeds += createLine("5*:", ts1.grievous.stars[0].ToString(), ts2.grievous.stars[0].ToString());
                    embeds += createLine("G11:", ts1.grievous.stars[0].ToString(), ts2.grievous.stars[0].ToString());
                    embeds += createLine("G12:", ts1.grievous.stars[1].ToString(), ts2.grievous.stars[1].ToString());
                    embeds += createLine("G13:", ts1.grievous.stars[2].ToString(), ts2.grievous.stars[2].ToString());
                    embeds += createLine("Relic 4+:", (ts1.grievous.relics[4] + ts1.grievous.relics[5] + ts1.grievous.relics[6] + ts1.grievous.relics[7]).ToString(), (ts2.grievous.relics[4] + ts2.grievous.relics[5] + ts2.grievous.relics[6] + ts2.grievous.relics[7]).ToString());
                    embeds += createLine("Relic 7:", ts1.grievous.relics[7].ToString(), ts2.grievous.relics[7].ToString());
                    embeds += createLine("GP 16K+:", ts1.grievous.gp16.ToString(), ts2.grievous.gp16.ToString());
                    embeds += createLine("GP 20K+:", ts1.grievous.gp20.ToString(), ts2.grievous.gp20.ToString());
                    embeds += "```";
                    embed.AddField($"{title}", embeds, true);
                    title = "=Nest=";
                    embeds = "```CSS\n";
                    embeds += createLine("Total:", ts1.en.Total.ToString(), ts2.en.Total.ToString());
                    embeds += createLine("7*:", ts1.en.stars[2].ToString(), ts2.en.stars[2].ToString());
                    embeds += createLine("6*:", ts1.en.stars[1].ToString(), ts2.en.stars[1].ToString());
                    embeds += createLine("5*:", ts1.en.stars[0].ToString(), ts2.en.stars[0].ToString());
                    embeds += createLine("G11:", ts1.en.stars[0].ToString(), ts2.en.stars[0].ToString());
                    embeds += createLine("G12:", ts1.en.stars[1].ToString(), ts2.en.stars[1].ToString());
                    embeds += createLine("G13:", ts1.en.stars[2].ToString(), ts2.en.stars[2].ToString());
                    embeds += createLine("Relic 4+:", (ts1.en.relics[4] + ts1.en.relics[5] + ts1.en.relics[6] + ts1.en.relics[7]).ToString(), (ts2.en.relics[4] + ts2.en.relics[5] + ts2.en.relics[6] + ts2.en.relics[7]).ToString());
                    embeds += createLine("Relic 7:", ts1.en.relics[7].ToString(), ts2.en.relics[7].ToString());
                    embeds += createLine("GP 16K+:", ts1.en.gp16.ToString(), ts2.en.gp16.ToString());
                    embeds += createLine("GP 20K+:", ts1.en.gp20.ToString(), ts2.en.gp20.ToString());
                    embeds += "```";
                    embed.AddField($"{title}", embeds, true);
                    title = "=BSF=";
                    embeds = "```CSS\n";
                    embeds += createLine("Total:", ts1.bsf.Total.ToString(), ts2.bsf.Total.ToString());
                    embeds += createLine("7*:", ts1.bsf.stars[2].ToString(), ts2.bsf.stars[2].ToString());
                    embeds += createLine("6*:", ts1.bsf.stars[1].ToString(), ts2.bsf.stars[1].ToString());
                    embeds += createLine("5*:", ts1.bsf.stars[0].ToString(), ts2.bsf.stars[0].ToString());
                    embeds += createLine("G11:", ts1.bsf.stars[0].ToString(), ts2.bsf.stars[0].ToString());
                    embeds += createLine("G12:", ts1.bsf.stars[1].ToString(), ts2.bsf.stars[1].ToString());
                    embeds += createLine("G13:", ts1.bsf.stars[2].ToString(), ts2.bsf.stars[2].ToString());
                    embeds += createLine("Relic 4+:", (ts1.bsf.relics[4] + ts1.bsf.relics[5] + ts1.bsf.relics[6] + ts1.bsf.relics[7]).ToString(), (ts2.bsf.relics[4] + ts2.bsf.relics[5] + ts2.bsf.relics[6] + ts2.bsf.relics[7]).ToString());
                    embeds += createLine("Relic 7:", ts1.bsf.relics[7].ToString(), ts2.bsf.relics[7].ToString());
                    embeds += createLine("GP 16K+:", ts1.bsf.gp16.ToString(), ts2.bsf.gp16.ToString());
                    embeds += createLine("GP 20K+:", ts1.bsf.gp20.ToString(), ts2.bsf.gp20.ToString());
                    embeds += "```";
                    embed.AddField($"{title}", embeds, true);
                    title = " =DR=";
                    embeds = "```CSS\n";
                    embeds += createLine("Total:", ts1.dr.Total.ToString(), ts2.dr.Total.ToString());
                    embeds += createLine("7*:", ts1.dr.stars[2].ToString(), ts2.dr.stars[2].ToString());
                    embeds += createLine("6*:", ts1.dr.stars[1].ToString(), ts2.dr.stars[1].ToString());
                    embeds += createLine("5*:", ts1.dr.stars[0].ToString(), ts2.dr.stars[0].ToString());
                    embeds += createLine("G11:", ts1.dr.stars[0].ToString(), ts2.dr.stars[0].ToString());
                    embeds += createLine("G12:", ts1.dr.stars[1].ToString(), ts2.dr.stars[1].ToString());
                    embeds += createLine("G13:", ts1.dr.stars[2].ToString(), ts2.dr.stars[2].ToString());
                    embeds += createLine("Relic 4+:", (ts1.dr.relics[4] + ts1.dr.relics[5] + ts1.dr.relics[6] + ts1.dr.relics[7]).ToString(), (ts2.dr.relics[4] + ts2.dr.relics[5] + ts2.dr.relics[6] + ts2.dr.relics[7]).ToString());
                    embeds += createLine("Relic 7:", ts1.dr.relics[7].ToString(), ts2.dr.relics[7].ToString());
                    embeds += createLine("GP 16K+:", ts1.dr.gp16.ToString(), ts2.dr.gp16.ToString());
                    embeds += createLine("GP 20K+:", ts1.dr.gp20.ToString(), ts2.dr.gp20.ToString());
                    embeds += "```";
                    embed.AddField($"{title}", embeds, true);
                    title = "=Traya=";
                    embeds = "```CSS\n";
                    embeds += createLine("Total:", ts1.traya.Total.ToString(), ts2.traya.Total.ToString());
                    embeds += createLine("7*:", ts1.traya.stars[2].ToString(), ts2.traya.stars[2].ToString());
                    embeds += createLine("6*:", ts1.traya.stars[1].ToString(), ts2.traya.stars[1].ToString());
                    embeds += createLine("5*:", ts1.traya.stars[0].ToString(), ts2.traya.stars[0].ToString());
                    embeds += createLine("G11:", ts1.traya.stars[0].ToString(), ts2.traya.stars[0].ToString());
                    embeds += createLine("G12:", ts1.traya.stars[1].ToString(), ts2.traya.stars[1].ToString());
                    embeds += createLine("G13:", ts1.traya.stars[2].ToString(), ts2.traya.stars[2].ToString());
                    embeds += createLine("Relic 4+:", (ts1.traya.relics[4] + ts1.traya.relics[5] + ts1.traya.relics[6] + ts1.traya.relics[7]).ToString(), (ts2.traya.relics[4] + ts2.traya.relics[5] + ts2.traya.relics[6] + ts2.traya.relics[7]).ToString());
                    embeds += createLine("Relic 7:", ts1.traya.relics[7].ToString(), ts2.traya.relics[7].ToString());
                    embeds += createLine("GP 16K+:", ts1.traya.gp16.ToString(), ts2.traya.gp16.ToString());
                    embeds += createLine("GP 20K+:", ts1.traya.gp20.ToString(), ts2.traya.gp20.ToString());
                    embeds += "```";
                    embed.AddField($"{title}", embeds, true);
                    title = "=GBA=";
                    embeds = "```CSS\n";
                    embeds += createLine("Total:", ts1.gba.Total.ToString(), ts2.gba.Total.ToString());
                    embeds += createLine("7*:", ts1.gba.stars[2].ToString(), ts2.gba.stars[2].ToString());
                    embeds += createLine("6*:", ts1.gba.stars[1].ToString(), ts2.gba.stars[1].ToString());
                    embeds += createLine("5*:", ts1.gba.stars[0].ToString(), ts2.gba.stars[0].ToString());
                    embeds += createLine("G11:", ts1.gba.stars[0].ToString(), ts2.gba.stars[0].ToString());
                    embeds += createLine("G12:", ts1.gba.stars[1].ToString(), ts2.gba.stars[1].ToString());
                    embeds += createLine("G13:", ts1.gba.stars[2].ToString(), ts2.gba.stars[2].ToString());
                    embeds += createLine("Relic 4+:", (ts1.gba.relics[4] + ts1.gba.relics[5] + ts1.gba.relics[6] + ts1.gba.relics[7]).ToString(), (ts2.gba.relics[4] + ts2.gba.relics[5] + ts2.gba.relics[6] + ts2.gba.relics[7]).ToString());
                    embeds += createLine("Relic 7:", ts1.gba.relics[7].ToString(), ts2.gba.relics[7].ToString());
                    embeds += createLine("GP 16K+:", ts1.gba.gp16.ToString(), ts2.gba.gp16.ToString());
                    embeds += createLine("GP 20K+:", ts1.gba.gp20.ToString(), ts2.gba.gp20.ToString());
                    embeds += "```";
                    embed.AddField($"{title}", embeds, true);
                    title = "=Bossk=";
                    embeds = "```CSS\n";
                    embeds += createLine("Total:", ts1.bossk.Total.ToString(), ts2.bossk.Total.ToString());
                    embeds += createLine("7*:", ts1.bossk.stars[2].ToString(), ts2.bossk.stars[2].ToString());
                    embeds += createLine("6*:", ts1.bossk.stars[1].ToString(), ts2.bossk.stars[1].ToString());
                    embeds += createLine("5*:", ts1.bossk.stars[0].ToString(), ts2.bossk.stars[0].ToString());
                    embeds += createLine("G11:", ts1.bossk.stars[0].ToString(), ts2.bossk.stars[0].ToString());
                    embeds += createLine("G12:", ts1.bossk.stars[1].ToString(), ts2.bossk.stars[1].ToString());
                    embeds += createLine("G13:", ts1.bossk.stars[2].ToString(), ts2.bossk.stars[2].ToString());
                    embeds += createLine("Relic 4+:", (ts1.bossk.relics[4] + ts1.bossk.relics[5] + ts1.bossk.relics[6] + ts1.bossk.relics[7]).ToString(), (ts2.bossk.relics[4] + ts2.bossk.relics[5] + ts2.bossk.relics[6] + ts2.bossk.relics[7]).ToString());
                    embeds += createLine("Relic 7:", ts1.bossk.relics[7].ToString(), ts2.bossk.relics[7].ToString());
                    embeds += createLine("GP 16K+:", ts1.bossk.gp16.ToString(), ts2.bossk.gp16.ToString());
                    embeds += createLine("GP 20K+:", ts1.bossk.gp20.ToString(), ts2.bossk.gp20.ToString());
                    embeds += "```";
                    embed.AddField($"{title}", embeds, true);
                    title = "=Hound's Tooth=";
                    embeds = "```CSS\n";
                    embeds += createLine("Total:", ts1.ht.Total.ToString(), ts2.ht.Total.ToString());
                    embeds += createLine("7*:", ts1.ht.stars[2].ToString(), ts2.ht.stars[2].ToString());
                    embeds += createLine("6*:", ts1.ht.stars[1].ToString(), ts2.ht.stars[1].ToString());
                    embeds += createLine("5*:", ts1.ht.stars[0].ToString(), ts2.ht.stars[0].ToString());
                    embeds += "```";
                    embed.AddField($"{title}", embeds, true);
                    title = "=Han's Falcon=";
                    embeds = "```CSS\n";
                    embeds += createLine("Total:", ts1.mf.Total.ToString(), ts2.mf.Total.ToString());
                    embeds += createLine("7*:", ts1.mf.stars[2].ToString(), ts2.mf.stars[2].ToString());
                    embeds += createLine("6*:", ts1.mf.stars[1].ToString(), ts2.mf.stars[1].ToString());
                    embeds += createLine("5*:", ts1.mf.stars[0].ToString(), ts2.mf.stars[0].ToString());
                    embeds += "```";
                    embed.AddField($"{title}", embeds, true);
                    title = "=Negotiator=";
                    embeds = "```CSS\n";
                    embeds += createLine("Total:", ts1.nego.Total.ToString(), ts2.nego.Total.ToString());
                    embeds += createLine("7*:", ts1.nego.stars[2].ToString(), ts2.nego.stars[2].ToString());
                    embeds += createLine("6*:", ts1.nego.stars[1].ToString(), ts2.nego.stars[1].ToString());
                    embeds += createLine("5*:", ts1.nego.stars[0].ToString(), ts2.nego.stars[0].ToString());
                    embeds += "```";
                    embed.AddField($"{title}", embeds, true);
                    title = "=Malevolence=";
                    embeds = "```CSS\n";
                    embeds += createLine("Total:", ts1.mal.Total.ToString(), ts2.mal.Total.ToString());
                    embeds += createLine("7*:", ts1.mal.stars[2].ToString(), ts2.mal.stars[2].ToString());
                    embeds += createLine("6*:", ts1.mal.stars[1].ToString(), ts2.mal.stars[1].ToString());
                    embeds += createLine("5*:", ts1.mal.stars[0].ToString(), ts2.mal.stars[0].ToString());
                    embeds += "```";
                    embed.AddField($"{title}", embeds, true);

                    await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":thumbsup:"));
                    DateTime end = DateTime.Now;
                    Console.WriteLine((end - start).TotalSeconds);
                    await ctx.RespondAsync(ctx.Member.Mention + " Here is the information you requested:", embed: embed);
                }
                else
                {
                    await ctx.RespondAsync(ctx.User.Mention + " it appears you either supplied 2 allycodes from the same guild or did not supply 2 allycodes. Please try again.");
                }
            }
            catch (Exception e) { Console.WriteLine(e.StackTrace); }
            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":thumbsup:"));
        }

        public List<string> buildPlayerStats(PlayerParse.Player r, DiscordEmbedBuilder b, DiscordEmbedBuilder b2)
        {
            List<string> players = new List<string>();
            int i = 0;

            foreach (PlayerParse.PlayerElement pe in r.PlayerList)
            {
                String s = "", r7 = "";
                int g13 = 0, g12 = 0, relics = 0;
                int[] rLevel = new int[8];
                foreach (PlayerParse.Roster rost in pe.Roster)
                {
                    if (rost.Gear == 12) { g12++; }
                    if (rost.Gear == 13)
                    {
                        g13++;
                        if (rost.Relic.CurrentTier > 1)
                        {
                            relics++; rLevel[rost.Relic.CurrentTier - 2]++;

                            if ((rost.Relic.CurrentTier - 2) == 7) { r7 += "." + toons.GetValueOrDefault(rost.DefId) + "\n"; }
                        }
                    }
                }
                s += "```CSS\n";
                s += createLine("GP:", String.Format(CultureInfo.InvariantCulture, "{0:#,##,M}", (double)pe.Stats[0].Value));
                s += createLine("Toon GP:", pe.Stats[1].Value.ToString("#,##,M", CultureInfo.InvariantCulture));
                s += createLine("Ship GP:", pe.Stats[2].Value.ToString("#,##,M", CultureInfo.InvariantCulture));
                s += createLine("R7:", rLevel[7].ToString());
                if (r7.Length > 0) { s += createLine("", r7.TrimEnd('\n')); }
                s += createLine("R4+:", (rLevel[7] + rLevel[6] + rLevel[5] + rLevel[4]).ToString());
                s += createLine("G13:", g13.ToString());
                s += createLine("G12:", g12.ToString());
                s += "```";
                if (i < 25) { b.AddField($"={pe.Name}=", s.Replace(",", "."), true); }
                else { b2.AddField($"={pe.Name}=", s.Replace(",", "."), true); }
                s = "";
                i++;
            }

            return players;
        }

        public void getGuildStats(PlayerParse.Player guild, ToonStats ts)
        {
            try
            {
                foreach (PlayerParse.PlayerElement pe in guild.PlayerList)
                {
                    foreach (PlayerParse.Roster r in pe.Roster)
                    {
                        switch (r.DefId)
                        {
                            case "HOUNDSTOOTH":
                                if (r.Rarity >= 5) { ts.ht.stars[r.Rarity - 5]++; }
                                ts.ht.Total++;
                                break;
                            case "DARTHTRAYA":
                                if (r.Rarity >= 5) { ts.traya.stars[r.Rarity - 5]++; }
                                ts.traya.Total++;
                                if (r.Relic.CurrentTier > 1) { ts.traya.relics[r.Relic.CurrentTier - 2]++; ts.traya.totRel++; }
                                if (r.Gp >= 16000)
                                {
                                    if (r.Gp >= 20000)
                                        ts.traya.gp20++;
                                    else
                                        ts.traya.gp16++;
                                }
                                break;

                            case "BASTILASHANDARK":
                                if (r.Rarity >= 5) { ts.bsf.stars[r.Rarity - 5]++; }
                                ts.bsf.Total++;
                                if (r.Relic.CurrentTier > 1) { ts.bsf.relics[r.Relic.CurrentTier - 2]++; ts.bsf.totRel++; }
                                if (r.Gp >= 16000)
                                {
                                    if (r.Gp >= 20000)
                                        ts.bsf.gp20++;
                                    else
                                        ts.bsf.gp16++;
                                }
                                break;
                            case "ENFYSNEST":
                                if (r.Rarity >= 5) { ts.en.stars[r.Rarity - 5]++; }
                                ts.en.Total++;
                                if (r.Relic.CurrentTier > 1) { ts.en.relics[r.Relic.CurrentTier - 2]++; ts.en.totRel++; }
                                if (r.Gp >= 16000)
                                {
                                    if (r.Gp >= 20000)
                                        ts.en.gp20++;
                                    else
                                        ts.en.gp16++;
                                }
                                break;
                            case "PADMEAMIDALA":
                                if (r.Rarity >= 5) { ts.padme.stars[r.Rarity - 5]++; }
                                ts.padme.Total++;
                                if (r.Relic.CurrentTier > 1) { ts.padme.relics[r.Relic.CurrentTier - 2]++; ts.padme.totRel++; }
                                if (r.Gp >= 16000)
                                {
                                    if (r.Gp >= 20000)
                                        ts.padme.gp20++;
                                    else
                                        ts.padme.gp16++;
                                }
                                break;
                            case "JEDIKNIGHTREVAN":
                                if (r.Rarity >= 5) { ts.jkr.stars[r.Rarity - 5]++; }
                                ts.jkr.Total++;
                                if (r.Relic.CurrentTier > 1) { ts.jkr.relics[r.Relic.CurrentTier - 2]++; ts.jkr.totRel++; }
                                if (r.Gp >= 16000)
                                {
                                    if (r.Gp >= 20000)
                                        ts.jkr.gp20++;
                                    else
                                        ts.jkr.gp16++;
                                }
                                break;
                            case "GRIEVOUS":
                                if (r.Rarity >= 5) { ts.grievous.stars[r.Rarity - 5]++; }
                                ts.grievous.Total++;
                                if (r.Relic.CurrentTier > 1) { ts.grievous.relics[r.Relic.CurrentTier - 2]++; ts.grievous.totRel++; }
                                if (r.Gp >= 16000)
                                {
                                    if (r.Gp >= 20000)
                                        ts.grievous.gp20++;
                                    else
                                        ts.grievous.gp16++;
                                }
                                break;
                            case "BOSSK":
                                if (r.Rarity >= 5) { ts.bossk.stars[r.Rarity - 5]++; }
                                ts.bossk.Total++;
                                if (r.Relic.CurrentTier > 1) { ts.bossk.relics[r.Relic.CurrentTier - 2]++; ts.bossk.totRel++; }
                                if (r.Gp >= 16000)
                                {
                                    if (r.Gp >= 20000)
                                        ts.bossk.gp20++;
                                    else
                                        ts.bossk.gp16++;
                                }
                                break;
                            case "GEONOSIANBROODALPHA":
                                if (r.Rarity >= 5) { ts.gba.stars[r.Rarity - 5]++; }
                                ts.gba.Total++;
                                if (r.Relic.CurrentTier > 1) { ts.gba.relics[r.Relic.CurrentTier - 2]++; ts.gba.totRel++; }
                                if (r.Gp >= 16000)
                                {
                                    if (r.Gp >= 20000)
                                        ts.gba.gp20++;
                                    else
                                        ts.gba.gp16++;
                                }
                                break;
                            case "DARTHREVAN":
                                if (r.Rarity >= 5) { ts.dr.stars[r.Rarity - 5]++; }
                                ts.dr.Total++;
                                if (r.Relic.CurrentTier > 1) { ts.dr.relics[r.Relic.CurrentTier - 2]++; ts.dr.totRel++; }
                                if (r.Gp >= 16000)
                                {
                                    if (r.Gp >= 20000)
                                        ts.dr.gp20++;
                                    else
                                        ts.dr.gp16++;
                                }
                                break;
                            case "DARTHMALAK":
                                if (r.Rarity >= 5) { ts.dm.stars[r.Rarity - 5]++; }
                                ts.dm.Total++;
                                if (r.Relic.CurrentTier > 1) { ts.dm.relics[r.Relic.CurrentTier - 2]++; ts.dm.totRel++; }
                                if (r.Gp >= 16000)
                                {
                                    if (r.Gp >= 20000)
                                        ts.dm.gp20++;
                                    else
                                        ts.dm.gp16++;
                                }
                                break;
                            case "CAPITALNEGOTIATOR":
                                if (r.Rarity >= 5) { ts.nego.stars[r.Rarity - 5]++; }
                                ts.nego.Total++;
                                break;
                            case "CAPITALMALEVOLENCE":
                                if (r.Rarity >= 5) { ts.mal.stars[r.Rarity - 5]++; }
                                ts.mal.Total++;
                                break;
                            case "MILLENNIUMFALCON":
                                if (r.Rarity >= 5) { ts.mf.stars[r.Rarity - 5]++; }
                                ts.mf.Total++;
                                break;
                        }

                        switch (r.Gear)
                        {
                            case 11:
                                ts.G11++;
                                break;
                            case 12:
                                switch (r.Equipped.Length)
                                {
                                    case 0:
                                        ts.G12++;
                                        break;
                                    case 1:
                                        ts.G121++;
                                        break;
                                    case 2:
                                        ts.G122++;
                                        break;
                                    case 3:
                                        ts.G123++;
                                        break;
                                    case 4:
                                        ts.G124++;
                                        break;
                                    case 5:
                                        ts.G125++;
                                        break;
                                }
                                break;
                            case 13:
                                ts.G13++;
                                break;
                        }
                        if (r.Relic != null)
                        {
                            if (r.Relic.CurrentTier - 2 >= 0) { ts.TotalRelics++; ts.relics[r.Relic.CurrentTier - 2]++; }
                        }
                    }
                }
            }
            catch (Exception e) { Console.WriteLine(e.StackTrace); }
        }
        public uint[][] buildMemberArray(GuildParse.Roster[] r)
        {

            uint[] members1 = new uint[25], members2 = new uint[r.Length - 25];
            uint[][] totalMembers = new uint[2][];
            for (int i = 0; i < r.Length; i++)
            {
                if (i < 25) { members1[i] = r[i].AllyCode; }
                else { members2[i - 25] = r[i].AllyCode; }
            }
            totalMembers[0] = members1;
            totalMembers[1] = members2;
            Console.WriteLine("# allycodes sent: " + members1.Length + " - " + members2.Length);
            return totalMembers;
        }
        public long buildCharGP(GuildParse.Roster[] r, string toonType)
        {
            long GP = 0;
            if (toonType.Equals("Char")) { foreach (GuildParse.Roster rost in r) { GP += rost.GpChar; } }
            if (toonType.Equals("Fleet")) { foreach (GuildParse.Roster rost in r) { GP += rost.GpShip; } }
            return GP / 1000000;
        }
        public string createLine(string category, string left)
        {
            if (category.Length > 0) { return category + addDots(10 - category.Length) + addDots(6 - left.Length) + left + addPadding(2) + "\n"; }
            else { return category + left + addPadding(2) + "\n"; }
        }
        public string createHeaderLine(string category, string left, string right)
        {
            return category + addDots(13 - category.Length) + addDots(10 - left.Length) + left + addPadding(2) + "::" + addPadding(2) + right + "\n";
        }
        public string createLine(string category, string left, string right)
        {
            return category + addDots(13 - category.Length - left.Length) + left + "::" + right + "\n";
        }
        public string addPadding(int space)
        {
            string s = "";
            for (int i = 0; i < space; i++) { s += " "; }
            return s;
        }
        public string addDots(int space)
        {
            string s = "";
            for (int i = 0; i < space; i++) { s += "."; }
            return s;
        }
        private void login()
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
            helper = new swgohHelpApiHelper(test);
            if (!helper.loggedIn) { helper.login(); }
        }
        public PlayerParse.Player getGuildMembers(uint[][] ac)
        {
            dynamic obj = new ExpandoObject();
            obj.name = 1;
            obj.roster = 1;
            obj.stats = 1;

            string guild = helper.fetchPlayer(ac[0], null, null, obj);
            guild = guild.Remove(guild.LastIndexOf("]")) + "," + helper.fetchPlayer(ac[1], null, null, obj).Substring(1);

            guild = "{\"players\":" + guild + "}";
            File.WriteAllText(@"C:\Users\jake\Documents\swgoh\SWGOH Prereqs\SWGOH Prereqs\players.txt", guild);
            PlayerParse.Player player = JsonConvert.DeserializeObject<PlayerParse.Player>(guild);
            return player;
        }
        public GuildParse.Guild getGuild(uint[] ac)
        {
            string guild = helper.fetchGuild(ac);
            guild = "{\"guild\":" + guild + "}";
            GuildParse.Guild gi = JsonConvert.DeserializeObject<GuildParse.Guild>(guild);
            return gi;
        }
        private struct ConfigJson
        {
            [JsonProperty("username")]
            public string username { get; private set; }

            [JsonProperty("password")]
            public string password { get; private set; }
        }
    }
}
