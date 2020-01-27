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
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using SWGOH_Prereqs;
using System.Data.SqlClient;
using System.Collections;
using System.Text.RegularExpressions;

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
        //Mod unit stat 5 is speed
        #region guildstats
        [Command("gs"), Description("Gets guild stats"), Aliases("guildstats", "stats")]
        public async Task getPrereqs(CommandContext ctx, [Description("Ally Code to lookup")] uint allycode, [RemainingText][Description("Sort Type. Can be gp or name, optionally followed by asc or desc. gp desc is default")] string sort)
        {
            DataHelper dh = new DataHelper();
            try
            {
                string sortedBy = "GP DESCENDING";
                await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":stopwatch:"));
                DateTime start = DateTime.Now;
                // first retrieve the interactivity module from the client
                var interactivity = ctx.Client.GetInteractivityModule();
                String s = "";
                //login to the API
                login();

                GuildParse.Guild guilds = dh.getGuild(new uint[] { allycode }, helper);
                if (guilds.guild.Length > 0)
                {
                    Database db = new Database();
                    SqlDataReader reader = db.GetRecords("*", "guilds", "GuildIGID = '" + guilds.guild[0].Id + "'");
                    int id = 0;
                    if (!reader.Read())
                    {
                        Hashtable hash = new Hashtable();
                        hash.Add("GuildName", guilds.guild[0].Name);
                        hash.Add("GuildIGID", guilds.guild[0].Id.ToString());
                        id = db.InsertRow(hash, "guilds");
                        hash.Clear();
                        Console.WriteLine(id);
                        DateTime myDateTime = DateTime.Now;
                        string sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                        hash.Add("GuildID", id);
                        hash.Add("TimeUpdated", sqlFormattedDate);
                        hash.Add("GuildData", guilds.ToJson().ToString());
                        hash.Add("RequestingUserDiscordID", ctx.Member.Mention.ToString());

                        db.InsertRow(hash, "guilddata");
                    }
                    int maxWidth = 75;
                    string header = $"{guilds.guild[0].Name} Stats ";
                    int space = maxWidth - header.Length;

                    PlayerParse.Player players1 = dh.getGuildMembers(dh.buildMemberArray(guilds.guild[0].Roster), helper);
                    GuildParse.GuildMember guild = guilds.guild[0];
                    //Build our output

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
                    await ctx.Message.DeleteOwnReactionAsync(DiscordEmoji.FromName(ctx.Client, ":stopwatch:"));
                    sortedBy = dh.buildPlayerStats(players1, embed, embed2, sort, id);
                    s += "**" + header + sortedBy + "**\n";
                    s += "======= Overview =======\n";
                    s += dh.createLine("Members:", guild.Members.ToString());
                    s += dh.createLine("Total GP:", (guild.Gp / 1000000).ToString() + "M");
                    s += dh.createLine("Character GP:", dh.buildCharGP(guild.Roster, "Char").ToString() + "M");
                    s += dh.createLine("Fleet GP:", dh.buildCharGP(guild.Roster, "Fleet").ToString() + "M");
                    s += "";
                    embed.Title = s;
                    if (ctx.Guild != null) { await ctx.RespondAsync(ctx.Member.Mention + " Here is the information you requested:", embed: embed); }
                    else { await ctx.RespondAsync(" Here is the information you requested:", embed: embed); }
                    await ctx.RespondAsync("", embed: embed2);
                }
                DateTime end = DateTime.Now;
                Console.WriteLine((end - start).TotalSeconds);
                await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":thumbsup:"));
            }
            catch (Exception e) { Console.WriteLine(e.StackTrace); await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":thumbsdown:")); }
        }
        #endregion

        #region TW
        [Command("tw"), Description("Compares 2 guilds for TW")]
        public async Task TWCompare(CommandContext ctx, [Description("Ally code of one guild")] uint allycode1, [Description("Ally Code of other guild")] uint allycode2)
        {
            DataHelper dh = new DataHelper();
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
                GuildParse.Guild guilds = dh.getGuild(new uint[] { allycode1, allycode2 }, helper);
                if (guilds.guild.Length > 1)
                {
                    var embed = new DiscordEmbedBuilder
                    {
                        Title = "TW Comparison",
                        Color = new DiscordColor(0xFF0000) // red
                    };
                    int maxWidth = 75;
                    string header = $"{guilds.guild[0].Name}  VS  {guilds.guild[1].Name}";
                    int space = maxWidth - header.Length;
                    //Get guilds and build arrays of players
                    PlayerParse.Player players1 = dh.getGuildMembers(dh.buildMemberArray(guilds.guild[0].Roster), helper), players2 = dh.getGuildMembers(dh.buildMemberArray(guilds.guild[1].Roster), helper);
                    GuildParse.GuildMember guild1 = guilds.guild[0], guild2 = guilds.guild[1];
                    //Build our output
                    s += "**" + header + "**\n";
                    s += "======= Overview =======```\n";
                    s += dh.createHeaderLine("Members:", guild1.Members.ToString(), guild2.Members.ToString());
                    s += dh.createHeaderLine("Total GP:", (guild1.Gp / 1000000).ToString() + "M", (guild2.Gp / 1000000).ToString() + "M");
                    s += dh.createHeaderLine("Character GP:", dh.buildCharGP(guild1.Roster, "Char").ToString() + "M", dh.buildCharGP(guild2.Roster, "Char").ToString() + "M");
                    s += dh.createHeaderLine("Fleet GP:", dh.buildCharGP(guild1.Roster, "Fleet").ToString() + "M", dh.buildCharGP(guild2.Roster, "Fleet").ToString() + "M");
                    s += "```";
                    embed.Description = s;
                    ToonStats ts1 = new ToonStats(), ts2 = new ToonStats();
                    dh.getGuildStats(players1, ts1);
                    dh.getGuildStats(players2, ts2);
                    #region gear
                    title = "==Gear==\n";
                    embeds += "```CSS\n";
                    embeds += dh.createLongLine("G11:", ts1.G11.ToString(), ts2.G11.ToString());
                    embeds += dh.createLongLine("G12:", ts1.G12.ToString(), ts2.G12.ToString());
                    embeds += dh.createLongLine("G12+1:", ts1.G121.ToString(), ts2.G121.ToString());
                    embeds += dh.createLongLine("G12+2:", ts1.G122.ToString(), ts2.G122.ToString());
                    embeds += dh.createLongLine("G12+3:", ts1.G123.ToString(), ts2.G123.ToString());
                    embeds += dh.createLongLine("G12+4:", ts1.G124.ToString(), ts2.G124.ToString());
                    embeds += dh.createLongLine("G12+5:", ts1.G125.ToString(), ts2.G125.ToString());
                    embeds += dh.createLongLine("G13:", ts1.G13.ToString(), ts2.G13.ToString());
                    embeds += "```";
                    embed.AddField($"{title}", embeds, false);
                    #endregion
                    #region relics
                    title = "==Relics==";
                    embeds = "```CSS\n";
                    embeds += dh.createLongLine("Total Relics:", ts1.TotalRelics.ToString(), ts2.TotalRelics.ToString());
                    embeds += dh.createLongLine("Relic 0:", ts1.relics[0].ToString(), ts2.relics[0].ToString());
                    embeds += dh.createLongLine("Relic 1:", ts1.relics[1].ToString(), ts2.relics[1].ToString());
                    embeds += dh.createLongLine("Relic 2:", ts1.relics[2].ToString(), ts2.relics[2].ToString());
                    embeds += dh.createLongLine("Relic 3:", ts1.relics[3].ToString(), ts2.relics[3].ToString());
                    embeds += dh.createLongLine("Relic 4:", ts1.relics[4].ToString(), ts2.relics[4].ToString());
                    embeds += dh.createLongLine("Relic 5:", ts1.relics[5].ToString(), ts2.relics[5].ToString());
                    embeds += dh.createLongLine("Relic 6:", ts1.relics[6].ToString(), ts2.relics[6].ToString());
                    embeds += dh.createLongLine("Relic 7:", ts1.relics[7].ToString(), ts2.relics[7].ToString());
                    embeds += "```";
                    embed.AddField($"{title}", embeds, false);
                    #endregion
                    #region mods
                    title = "==Mods==";
                    embeds = "```CSS\n";
                    embeds += dh.createLongLine("6 Dot Mods:", ts1.sixStarMods.ToString(), ts2.sixStarMods.ToString());
                    embeds += dh.createLongLine("10+ Speed:", ts1.speedMods[0].ToString(), ts2.speedMods[0].ToString());
                    embeds += dh.createLongLine("15+ Speed:", ts1.speedMods[1].ToString(), ts2.speedMods[1].ToString());
                    embeds += dh.createLongLine("20+ Speed:", ts1.speedMods[2].ToString(), ts2.speedMods[2].ToString());
                    embeds += dh.createLongLine("25+ Speed:", ts1.speedMods[3].ToString(), ts2.speedMods[3].ToString());
                    embeds += dh.createLongLine("100+ Off:", ts1.off100.ToString(), ts2.off100.ToString());
                    embeds += "```";
                    embed.AddField($"{title}", embeds, false);
                    #endregion
                    #region GAS
                    title = "=Gen Skywalker=";
                    embeds = "```CSS\n";
                    embeds += dh.createLine("Total:", ts1.gas.Total.ToString(), ts2.gas.Total.ToString());
                    embeds += dh.createLine("7*:", ts1.gas.stars[2].ToString(), ts2.gas.stars[2].ToString());
                    embeds += dh.createLine("6*:", ts1.gas.stars[1].ToString(), ts2.gas.stars[1].ToString());
                    embeds += dh.createLine("5*:", ts1.gas.stars[0].ToString(), ts2.gas.stars[0].ToString());
                    embeds += dh.createLine("G11:", ts1.gas.gear[0].ToString(), ts2.gas.gear[0].ToString());
                    embeds += dh.createLine("G12:", ts1.gas.gear[1].ToString(), ts2.gas.gear[1].ToString());
                    embeds += dh.createLine("G13:", ts1.gas.gear[2].ToString(), ts2.gas.gear[2].ToString());
                    embeds += dh.createLine("Relic 4+:", (ts1.gas.relics[4] + ts1.gas.relics[5] + ts1.gas.relics[6] + ts1.gas.relics[7]).ToString(), (ts2.gas.relics[4] + ts2.gas.relics[5] + ts2.gas.relics[6] + ts2.gas.relics[7]).ToString());
                    embeds += dh.createLine("Relic 7:", ts1.gas.relics[7].ToString(), ts2.gas.relics[7].ToString());
                    embeds += dh.createLine("GP 16K+:", ts1.gas.gp16.ToString(), ts2.gas.gp16.ToString());
                    embeds += dh.createLine("GP 20K+:", ts1.gas.gp20.ToString(), ts2.gas.gp20.ToString());
                    embeds += dh.createLine("Z:", ts1.gas.z.ToString(), ts2.gas.z.ToString());
                    embeds += dh.createLine("ZZ:", ts1.gas.zz.ToString(), ts2.gas.zz.ToString());
                    embeds += dh.createLine("ZZZ:", ts1.gas.zzz.ToString(), ts2.gas.zzz.ToString());
                    embeds += dh.createLine("ZZZZ:", ts1.gas.zzzz.ToString(), ts2.gas.zzzz.ToString());
                    embeds += "```";
                    embed.AddField($"{title}", embeds, true);
                    #endregion
                    #region Malak
                    title = "=Malak=";
                    embeds = "```CSS\n";
                    embeds += dh.createLine("Total:", ts1.dm.Total.ToString(), ts2.dm.Total.ToString());
                    embeds += dh.createLine("7*:", ts1.dm.stars[2].ToString(), ts2.dm.stars[2].ToString());
                    embeds += dh.createLine("6*:", ts1.dm.stars[1].ToString(), ts2.dm.stars[1].ToString());
                    embeds += dh.createLine("5*:", ts1.dm.stars[0].ToString(), ts2.dm.stars[0].ToString());
                    embeds += dh.createLine("G11:", ts1.dm.gear[0].ToString(), ts2.dm.gear[0].ToString());
                    embeds += dh.createLine("G12:", ts1.dm.gear[1].ToString(), ts2.dm.gear[1].ToString());
                    embeds += dh.createLine("G13:", ts1.dm.gear[2].ToString(), ts2.dm.gear[2].ToString());
                    embeds += dh.createLine("Relic 4+:", (ts1.dm.relics[4] + ts1.dm.relics[5] + ts1.dm.relics[6] + ts1.dm.relics[7]).ToString(), (ts2.dm.relics[4] + ts2.dm.relics[5] + ts2.dm.relics[6] + ts2.dm.relics[7]).ToString());
                    embeds += dh.createLine("Relic 7:", ts1.dm.relics[7].ToString(), ts2.dm.relics[7].ToString());
                    embeds += dh.createLine("GP 16K+:", ts1.dm.gp16.ToString(), ts2.dm.gp16.ToString());
                    embeds += dh.createLine("GP 20K+:", ts1.dm.gp20.ToString(), ts2.dm.gp20.ToString());
                    embeds += dh.createLine("Z:", ts1.dm.z.ToString(), ts2.dm.z.ToString());
                    embeds += dh.createLine("ZZ:", ts1.dm.zz.ToString(), ts2.dm.zz.ToString());
                    embeds += "```";
                    embed.AddField($"{title}", embeds, true);
                    #endregion
                    #region Padme
                    title = "=Padme=";
                    embeds = "```CSS\n";
                    embeds += dh.createLine("Total:", ts1.padme.Total.ToString(), ts2.padme.Total.ToString());
                    embeds += dh.createLine("7*:", ts1.padme.stars[2].ToString(), ts2.padme.stars[2].ToString());
                    embeds += dh.createLine("6*:", ts1.padme.stars[1].ToString(), ts2.padme.stars[1].ToString());
                    embeds += dh.createLine("5*:", ts1.padme.stars[0].ToString(), ts2.padme.stars[0].ToString());
                    embeds += dh.createLine("G11:", ts1.padme.gear[0].ToString(), ts2.padme.gear[0].ToString());
                    embeds += dh.createLine("G12:", ts1.padme.gear[1].ToString(), ts2.padme.gear[1].ToString());
                    embeds += dh.createLine("G13:", ts1.padme.gear[2].ToString(), ts2.padme.gear[2].ToString());
                    embeds += dh.createLine("Relic 4+:", (ts1.padme.relics[4] + ts1.padme.relics[5] + ts1.padme.relics[6] + ts1.padme.relics[7]).ToString(), (ts2.padme.relics[4] + ts2.padme.relics[5] + ts2.padme.relics[6] + ts2.padme.relics[7]).ToString());
                    embeds += dh.createLine("Relic 7:", ts1.padme.relics[7].ToString(), ts2.padme.relics[7].ToString());
                    embeds += dh.createLine("GP 16K+:", ts1.padme.gp16.ToString(), ts2.padme.gp16.ToString());
                    embeds += dh.createLine("GP 20K+:", ts1.padme.gp20.ToString(), ts2.padme.gp20.ToString());
                    embeds += dh.createLine("Z:", ts1.padme.z.ToString(), ts2.padme.z.ToString());
                    embeds += dh.createLine("ZZ:", ts1.padme.zz.ToString(), ts2.padme.zz.ToString());
                    embeds += "```";
                    embed.AddField($"{title}", embeds, true);
                    #endregion
                    #region JKR
                    title = "=JKR=";
                    embeds = "```CSS\n";
                    embeds += dh.createLine("Total:", ts1.jkr.Total.ToString(), ts2.padme.Total.ToString());
                    embeds += dh.createLine("7*:", ts1.jkr.stars[2].ToString(), ts2.jkr.stars[2].ToString());
                    embeds += dh.createLine("6*:", ts1.jkr.stars[1].ToString(), ts2.jkr.stars[1].ToString());
                    embeds += dh.createLine("5*:", ts1.jkr.stars[0].ToString(), ts2.jkr.stars[0].ToString());
                    embeds += dh.createLine("G11:", ts1.jkr.gear[0].ToString(), ts2.jkr.gear[0].ToString());
                    embeds += dh.createLine("G12:", ts1.jkr.gear[1].ToString(), ts2.jkr.gear[1].ToString());
                    embeds += dh.createLine("G13:", ts1.jkr.gear[2].ToString(), ts2.jkr.gear[2].ToString());
                    embeds += dh.createLine("Relic 4+:", (ts1.jkr.relics[4] + ts1.jkr.relics[5] + ts1.jkr.relics[6] + ts1.jkr.relics[7]).ToString(), (ts2.jkr.relics[4] + ts2.jkr.relics[5] + ts2.jkr.relics[6] + ts2.jkr.relics[7]).ToString());
                    embeds += dh.createLine("Relic 7:", ts1.jkr.relics[7].ToString(), ts2.jkr.relics[7].ToString());
                    embeds += dh.createLine("GP 16K+:", ts1.jkr.gp16.ToString(), ts2.jkr.gp16.ToString());
                    embeds += dh.createLine("GP 20K+:", ts1.jkr.gp20.ToString(), ts2.jkr.gp20.ToString());
                    embeds += dh.createLine("Z:", ts1.jkr.z.ToString(), ts2.jkr.z.ToString());
                    embeds += dh.createLine("ZZ:", ts1.jkr.zz.ToString(), ts2.jkr.zz.ToString());
                    embeds += dh.createLine("ZZZ:", ts1.jkr.zzz.ToString(), ts2.jkr.zzz.ToString());
                    embeds += "```";
                    embed.AddField($"{title}", embeds, true);
                    #endregion
                    #region Grievous
                    title = "=Grievous=";
                    embeds = "```CSS\n";
                    embeds += dh.createLine("Total:", ts1.grievous.Total.ToString(), ts2.grievous.Total.ToString());
                    embeds += dh.createLine("7*:", ts1.grievous.stars[2].ToString(), ts2.grievous.stars[2].ToString());
                    embeds += dh.createLine("6*:", ts1.grievous.stars[1].ToString(), ts2.grievous.stars[1].ToString());
                    embeds += dh.createLine("5*:", ts1.grievous.stars[0].ToString(), ts2.grievous.stars[0].ToString());
                    embeds += dh.createLine("G11:", ts1.grievous.gear[0].ToString(), ts2.grievous.gear[0].ToString());
                    embeds += dh.createLine("G12:", ts1.grievous.gear[1].ToString(), ts2.grievous.gear[1].ToString());
                    embeds += dh.createLine("G13:", ts1.grievous.gear[2].ToString(), ts2.grievous.gear[2].ToString());
                    embeds += dh.createLine("Relic 4+:", (ts1.grievous.relics[4] + ts1.grievous.relics[5] + ts1.grievous.relics[6] + ts1.grievous.relics[7]).ToString(), (ts2.grievous.relics[4] + ts2.grievous.relics[5] + ts2.grievous.relics[6] + ts2.grievous.relics[7]).ToString());
                    embeds += dh.createLine("Relic 7:", ts1.grievous.relics[7].ToString(), ts2.grievous.relics[7].ToString());
                    embeds += dh.createLine("GP 16K+:", ts1.grievous.gp16.ToString(), ts2.grievous.gp16.ToString());
                    embeds += dh.createLine("GP 20K+:", ts1.grievous.gp20.ToString(), ts2.grievous.gp20.ToString());
                    embeds += dh.createLine("Z:", ts1.grievous.z.ToString(), ts2.grievous.z.ToString());
                    embeds += dh.createLine("ZZ:", ts1.grievous.zz.ToString(), ts2.grievous.zz.ToString());
                    embeds += "```";
                    embed.AddField($"{title}", embeds, true);
                    #endregion
                    #region Nest
                    title = "=Nest=";
                    embeds = "```CSS\n";
                    embeds += dh.createLine("Total:", ts1.en.Total.ToString(), ts2.en.Total.ToString());
                    embeds += dh.createLine("7*:", ts1.en.stars[2].ToString(), ts2.en.stars[2].ToString());
                    embeds += dh.createLine("6*:", ts1.en.stars[1].ToString(), ts2.en.stars[1].ToString());
                    embeds += dh.createLine("5*:", ts1.en.stars[0].ToString(), ts2.en.stars[0].ToString());
                    embeds += dh.createLine("G11:", ts1.en.gear[0].ToString(), ts2.en.gear[0].ToString());
                    embeds += dh.createLine("G12:", ts1.en.gear[1].ToString(), ts2.en.gear[1].ToString());
                    embeds += dh.createLine("G13:", ts1.en.gear[2].ToString(), ts2.en.gear[2].ToString());
                    embeds += dh.createLine("Relic 4+:", (ts1.en.relics[4] + ts1.en.relics[5] + ts1.en.relics[6] + ts1.en.relics[7]).ToString(), (ts2.en.relics[4] + ts2.en.relics[5] + ts2.en.relics[6] + ts2.en.relics[7]).ToString());
                    embeds += dh.createLine("Relic 7:", ts1.en.relics[7].ToString(), ts2.en.relics[7].ToString());
                    embeds += dh.createLine("GP 16K+:", ts1.en.gp16.ToString(), ts2.en.gp16.ToString());
                    embeds += dh.createLine("GP 20K+:", ts1.en.gp20.ToString(), ts2.en.gp20.ToString());
                    embeds += "```";
                    embed.AddField($"{title}", embeds, true);
                    #endregion
                    #region BSF
                    title = "=BSF=";
                    embeds = "```CSS\n";
                    embeds += dh.createLine("Total:", ts1.bsf.Total.ToString(), ts2.bsf.Total.ToString());
                    embeds += dh.createLine("7*:", ts1.bsf.stars[2].ToString(), ts2.bsf.stars[2].ToString());
                    embeds += dh.createLine("6*:", ts1.bsf.stars[1].ToString(), ts2.bsf.stars[1].ToString());
                    embeds += dh.createLine("5*:", ts1.bsf.stars[0].ToString(), ts2.bsf.stars[0].ToString());
                    embeds += dh.createLine("G11:", ts1.bsf.gear[0].ToString(), ts2.bsf.gear[0].ToString());
                    embeds += dh.createLine("G12:", ts1.bsf.gear[1].ToString(), ts2.bsf.gear[1].ToString());
                    embeds += dh.createLine("G13:", ts1.bsf.gear[2].ToString(), ts2.bsf.gear[2].ToString());
                    embeds += dh.createLine("Relic 4+:", (ts1.bsf.relics[4] + ts1.bsf.relics[5] + ts1.bsf.relics[6] + ts1.bsf.relics[7]).ToString(), (ts2.bsf.relics[4] + ts2.bsf.relics[5] + ts2.bsf.relics[6] + ts2.bsf.relics[7]).ToString());
                    embeds += dh.createLine("Relic 7:", ts1.bsf.relics[7].ToString(), ts2.bsf.relics[7].ToString());
                    embeds += dh.createLine("GP 16K+:", ts1.bsf.gp16.ToString(), ts2.bsf.gp16.ToString());
                    embeds += dh.createLine("GP 20K+:", ts1.bsf.gp20.ToString(), ts2.bsf.gp20.ToString());
                    embeds += dh.createLine("Z:", ts1.bsf.z.ToString(), ts2.bsf.z.ToString());
                    embeds += "```";
                    embed.AddField($"{title}", embeds, true);
                    #endregion
                    #region DR
                    title = " =DR=";
                    embeds = "```CSS\n";
                    embeds += dh.createLine("Total:", ts1.dr.Total.ToString(), ts2.dr.Total.ToString());
                    embeds += dh.createLine("7*:", ts1.dr.stars[2].ToString(), ts2.dr.stars[2].ToString());
                    embeds += dh.createLine("6*:", ts1.dr.stars[1].ToString(), ts2.dr.stars[1].ToString());
                    embeds += dh.createLine("5*:", ts1.dr.stars[0].ToString(), ts2.dr.stars[0].ToString());
                    embeds += dh.createLine("G11:", ts1.dr.gear[0].ToString(), ts2.dr.gear[0].ToString());
                    embeds += dh.createLine("G12:", ts1.dr.gear[1].ToString(), ts2.dr.gear[1].ToString());
                    embeds += dh.createLine("G13:", ts1.dr.gear[2].ToString(), ts2.dr.gear[2].ToString());
                    embeds += dh.createLine("Relic 4+:", (ts1.dr.relics[4] + ts1.dr.relics[5] + ts1.dr.relics[6] + ts1.dr.relics[7]).ToString(), (ts2.dr.relics[4] + ts2.dr.relics[5] + ts2.dr.relics[6] + ts2.dr.relics[7]).ToString());
                    embeds += dh.createLine("Relic 7:", ts1.dr.relics[7].ToString(), ts2.dr.relics[7].ToString());
                    embeds += dh.createLine("GP 16K+:", ts1.dr.gp16.ToString(), ts2.dr.gp16.ToString());
                    embeds += dh.createLine("GP 20K+:", ts1.dr.gp20.ToString(), ts2.dr.gp20.ToString());
                    embeds += dh.createLine("Z:", ts1.gas.z.ToString(), ts2.gas.z.ToString());
                    embeds += dh.createLine("ZZ:", ts1.gas.zz.ToString(), ts2.gas.zz.ToString());
                    embeds += dh.createLine("ZZZ:", ts1.gas.zzz.ToString(), ts2.gas.zzz.ToString());
                    embeds += "```";
                    embed.AddField($"{title}", embeds, true);
                    #endregion
                    #region Traya
                    title = "=Traya=";
                    embeds = "```CSS\n";
                    embeds += dh.createLine("Total:", ts1.traya.Total.ToString(), ts2.traya.Total.ToString());
                    embeds += dh.createLine("7*:", ts1.traya.stars[2].ToString(), ts2.traya.stars[2].ToString());
                    embeds += dh.createLine("6*:", ts1.traya.stars[1].ToString(), ts2.traya.stars[1].ToString());
                    embeds += dh.createLine("5*:", ts1.traya.stars[0].ToString(), ts2.traya.stars[0].ToString());
                    embeds += dh.createLine("G11:", ts1.traya.gear[0].ToString(), ts2.traya.gear[0].ToString());
                    embeds += dh.createLine("G12:", ts1.traya.gear[1].ToString(), ts2.traya.gear[1].ToString());
                    embeds += dh.createLine("G13:", ts1.traya.gear[2].ToString(), ts2.traya.gear[2].ToString());
                    embeds += dh.createLine("Relic 4+:", (ts1.traya.relics[4] + ts1.traya.relics[5] + ts1.traya.relics[6] + ts1.traya.relics[7]).ToString(), (ts2.traya.relics[4] + ts2.traya.relics[5] + ts2.traya.relics[6] + ts2.traya.relics[7]).ToString());
                    embeds += dh.createLine("Relic 7:", ts1.traya.relics[7].ToString(), ts2.traya.relics[7].ToString());
                    embeds += dh.createLine("GP 16K+:", ts1.traya.gp16.ToString(), ts2.traya.gp16.ToString());
                    embeds += dh.createLine("GP 20K+:", ts1.traya.gp20.ToString(), ts2.traya.gp20.ToString());
                    embeds += dh.createLine("Z:", ts1.traya.z.ToString(), ts2.traya.z.ToString());
                    embeds += dh.createLine("ZZ:", ts1.traya.zz.ToString(), ts2.traya.zz.ToString());
                    embeds += "```";
                    embed.AddField($"{title}", embeds, true);
                    #endregion
                    #region GBA
                    title = "=GBA=";
                    embeds = "```CSS\n";
                    embeds += dh.createLine("Total:", ts1.gba.Total.ToString(), ts2.gba.Total.ToString());
                    embeds += dh.createLine("7*:", ts1.gba.stars[2].ToString(), ts2.gba.stars[2].ToString());
                    embeds += dh.createLine("6*:", ts1.gba.stars[1].ToString(), ts2.gba.stars[1].ToString());
                    embeds += dh.createLine("5*:", ts1.gba.stars[0].ToString(), ts2.gba.stars[0].ToString());
                    embeds += dh.createLine("G11:", ts1.gba.gear[0].ToString(), ts2.gba.gear[0].ToString());
                    embeds += dh.createLine("G12:", ts1.gba.gear[1].ToString(), ts2.gba.gear[1].ToString());
                    embeds += dh.createLine("G13:", ts1.gba.gear[2].ToString(), ts2.gba.gear[2].ToString());
                    embeds += dh.createLine("Relic 4+:", (ts1.gba.relics[4] + ts1.gba.relics[5] + ts1.gba.relics[6] + ts1.gba.relics[7]).ToString(), (ts2.gba.relics[4] + ts2.gba.relics[5] + ts2.gba.relics[6] + ts2.gba.relics[7]).ToString());
                    embeds += dh.createLine("Relic 7:", ts1.gba.relics[7].ToString(), ts2.gba.relics[7].ToString());
                    embeds += dh.createLine("GP 16K+:", ts1.gba.gp16.ToString(), ts2.gba.gp16.ToString());
                    embeds += dh.createLine("GP 20K+:", ts1.gba.gp20.ToString(), ts2.gba.gp20.ToString());
                    embeds += dh.createLine("Z:", ts1.gba.z.ToString(), ts2.gba.z.ToString());
                    embeds += dh.createLine("ZZ:", ts1.gba.zz.ToString(), ts2.gba.zz.ToString());
                    embeds += "```";
                    embed.AddField($"{title}", embeds, true);
                    #endregion
                    #region Bossk
                    title = "=Bossk=";
                    embeds = "```CSS\n";
                    embeds += dh.createLine("Total:", ts1.bossk.Total.ToString(), ts2.bossk.Total.ToString());
                    embeds += dh.createLine("7*:", ts1.bossk.stars[2].ToString(), ts2.bossk.stars[2].ToString());
                    embeds += dh.createLine("6*:", ts1.bossk.stars[1].ToString(), ts2.bossk.stars[1].ToString());
                    embeds += dh.createLine("5*:", ts1.bossk.stars[0].ToString(), ts2.bossk.stars[0].ToString());
                    embeds += dh.createLine("G11:", ts1.bossk.gear[0].ToString(), ts2.bossk.gear[0].ToString());
                    embeds += dh.createLine("G12:", ts1.bossk.gear[1].ToString(), ts2.bossk.gear[1].ToString());
                    embeds += dh.createLine("G13:", ts1.bossk.gear[2].ToString(), ts2.bossk.gear[2].ToString());
                    embeds += dh.createLine("Relic 4+:", (ts1.bossk.relics[4] + ts1.bossk.relics[5] + ts1.bossk.relics[6] + ts1.bossk.relics[7]).ToString(), (ts2.bossk.relics[4] + ts2.bossk.relics[5] + ts2.bossk.relics[6] + ts2.bossk.relics[7]).ToString());
                    embeds += dh.createLine("Relic 7:", ts1.bossk.relics[7].ToString(), ts2.bossk.relics[7].ToString());
                    embeds += dh.createLine("GP 16K+:", ts1.bossk.gp16.ToString(), ts2.bossk.gp16.ToString());
                    embeds += dh.createLine("GP 20K+:", ts1.bossk.gp20.ToString(), ts2.bossk.gp20.ToString());
                    embeds += dh.createLine("Z:", ts1.bossk.z.ToString(), ts2.bossk.z.ToString());
                    embeds += dh.createLine("ZZ:", ts1.bossk.zz.ToString(), ts2.bossk.zz.ToString());
                    embeds += "```";
                    embed.AddField($"{title}", embeds, true);
                    #endregion
                    #region HT
                    title = "=Hound's Tooth=";
                    embeds = "```CSS\n";
                    embeds += dh.createLine("Total:", ts1.ht.Total.ToString(), ts2.ht.Total.ToString());
                    embeds += dh.createLine("7*:", ts1.ht.stars[2].ToString(), ts2.ht.stars[2].ToString());
                    embeds += dh.createLine("6*:", ts1.ht.stars[1].ToString(), ts2.ht.stars[1].ToString());
                    embeds += dh.createLine("5*:", ts1.ht.stars[0].ToString(), ts2.ht.stars[0].ToString());
                    embeds += "```";
                    embed.AddField($"{title}", embeds, true);
                    #endregion
                    #region MilFal
                    title = "=Han's Falcon=";
                    embeds = "```CSS\n";
                    embeds += dh.createLine("Total:", ts1.mf.Total.ToString(), ts2.mf.Total.ToString());
                    embeds += dh.createLine("7*:", ts1.mf.stars[2].ToString(), ts2.mf.stars[2].ToString());
                    embeds += dh.createLine("6*:", ts1.mf.stars[1].ToString(), ts2.mf.stars[1].ToString());
                    embeds += dh.createLine("5*:", ts1.mf.stars[0].ToString(), ts2.mf.stars[0].ToString());
                    embeds += "```";
                    embed.AddField($"{title}", embeds, true);
                    #endregion
                    #region Negotiator    
                    title = "=Negotiator=";
                    embeds = "```CSS\n";
                    embeds += dh.createLine("Total:", ts1.nego.Total.ToString(), ts2.nego.Total.ToString());
                    embeds += dh.createLine("7*:", ts1.nego.stars[2].ToString(), ts2.nego.stars[2].ToString());
                    embeds += dh.createLine("6*:", ts1.nego.stars[1].ToString(), ts2.nego.stars[1].ToString());
                    embeds += dh.createLine("5*:", ts1.nego.stars[0].ToString(), ts2.nego.stars[0].ToString());
                    embeds += "```";
                    embed.AddField($"{title}", embeds, true);
                    #endregion
                    #region Malevolence
                    title = "=Malevolence=";
                    embeds = "```CSS\n";
                    embeds += dh.createLine("Total:", ts1.mal.Total.ToString(), ts2.mal.Total.ToString());
                    embeds += dh.createLine("7*:", ts1.mal.stars[2].ToString(), ts2.mal.stars[2].ToString());
                    embeds += dh.createLine("6*:", ts1.mal.stars[1].ToString(), ts2.mal.stars[1].ToString());
                    embeds += dh.createLine("5*:", ts1.mal.stars[0].ToString(), ts2.mal.stars[0].ToString());
                    embeds += "```";
                    embed.AddField($"{title}", embeds, true);
                    #endregion
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
        }
        #endregion
        [Command("register"), Description("register allycode to discord handle"), Aliases("r"), Hidden]
        public async Task registerMember(CommandContext ctx, [Description("Ally Code to register")] uint allycode, [RemainingText][Description("Discord User to register to ally code")] string user)
        {
            try
            {
                String added = "";
                Database db = new Database();
                SqlDataReader reader = db.GetRecords("*", "Users", "Allycode = " + allycode);
                Console.WriteLine(reader.FieldCount);
                var embed = new DiscordEmbedBuilder();
                if (!reader.Read())
                {
                    Hashtable hash = new Hashtable();
                    if (user != null)
                    {
                        DiscordUser ds = ctx.Message.MentionedUsers[0];
                        added = user;
                        hash.Add("DiscordID", user);
                        hash.Add("Username", ds.Username);
                    }
                    else
                    {
                        added = ctx.Member.Mention;
                        hash.Add("DiscordID", ctx.Member.Mention);
                        hash.Add("Username", ctx.Member.DisplayName);
                    }
                    hash.Add("Allycode", allycode);
                    hash.Add("UserLevel", 1);
                    db.InsertRow(hash, "Users");
                    embed.Title = "User Registered";
                    embed.Color = new DiscordColor(0x00b300);
                    embed.Description = added + " has been registered with ASN-121 with allycode " + allycode;
                }
                else
                {
                    embed.Title = "User Exists";
                    embed.Color = new DiscordColor(0xFF0000);
                    embed.Description = "This user already exists in my Databank under username: " + reader[2].ToString() + " and Allycode: " + allycode;
                }
                await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":thumbsup:"));
                await ctx.RespondAsync("", embed: embed);
            }
            catch (Exception e)
            {
                var embed = new DiscordEmbedBuilder()
                {
                    Title = "Error",
                    Description = "Something went wrong, please try again.",
                    Color = new DiscordColor(0xFF0000)
                };
                await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":thumbsdown:"));
                await ctx.RespondAsync("", embed: embed);
                Console.WriteLine(e.StackTrace);
            }
        }
        [Command("checkusers"), Description("register allycode to discord handle"), Aliases("cu"), Hidden]
        public async Task checkMember(CommandContext ctx, [Description("Ally Code to check")] uint allycode)
        {
            var client = new MongoClient("mongodb+srv://the_only_martyr:swgohpassword@cluster0-9qh8n.mongodb.net/test?retryWrites=true&w=majority");
            var database = client.GetDatabase("BotUsers");
            var collection = database.GetCollection<BsonDocument>("Users");

            await ctx.RespondAsync(": Checking allycode: " + allycode);
        }

        [Command("help"), Description("This Command List")]
        public async Task Help(CommandContext ctx, [RemainingText][Description("")] string user)
        {
            DataHelper dh = new DataHelper();
            string title = "", embeds = "", prefix = "";

            var embed = new DiscordEmbedBuilder
            {
                Title = "ASN-121 Help",
                Color = new DiscordColor(0xFF0000) // red
            };
            IReadOnlyDictionary<string, Command> commands = ctx.CommandsNext.RegisteredCommands;
            if (user == null)
            {
                DiscordMessage m = ctx.Message;
                prefix = m.Content.Replace(ctx.Command.Name, "");
                title = "```asciidoc\n= Command List =\n";
                embeds += title + $"Use {prefix}help commandName :: to get more information for each command\n\n";
                String comName = "";
                int i = 0;
                foreach (Command c in commands.Values)
                {
                    if (!comName.Equals(c.Name))
                    {
                        comName = c.Name;
                        i = 0;
                    }
                    else { i++; }

                    if (i < 1)
                    {
                        if (!c.IsHidden)
                        {
                            if (c.Name.Equals("tw") || c.Name.Equals("gs") || c.Name.Equals("register") || c.Name.Equals("checkusers") || c.Name.Equals("help"))
                            {
                                string aliases = "";
                                if (c.Aliases != null)
                                {
                                    if (c.Aliases.Count == 1) { foreach (string s in c.Aliases) { aliases += s; } }
                                    else
                                    {
                                        foreach (string s in c.Aliases) { aliases += s + ", "; }
                                        aliases = aliases.Substring(0, aliases.LastIndexOf(','));
                                    }
                                    embeds += dh.createLineDocs(prefix + c.Name, c.Description + " (aliases: " + aliases + ")");
                                }
                                else { embeds += dh.createLineDocs(prefix + c.Name, c.Description); }
                            }
                        }
                    }
                }
                embeds += "```";
                embed.Description = embeds;
            }
            else
            {
                if (dh.checkCommand(user, commands))
                {
                    DiscordMessage m = ctx.Message;
                    prefix = m.Content.Substring(0, m.Content.IndexOf(ctx.Command.Name));
                    switch (user)
                    {
                        case "gs":
                        case "guildstats":
                        case "stats":
                            title = "```asciidoc\n= Guild Stats =\n";
                            embeds += title + "";
                            embeds += "Diplays data for every member of the guild of the provided allycode. \n\n" +
                                "Usage: gs {allycode} {sort} ::  allycode is required. Sort is optional, defaults to GP Descending.\n\n" +
                                "Sorts::  gp desc, gp asc, relics desc, relics asc, name desc and name asc\n\n" +
                                $"Example:: {prefix}gs 729778685 relics desc";
                            break;
                        case "tw":
                            title = "```asciidoc\n= TW Comparison =\n";
                            embeds += title + "";
                            embeds += "Displays a comparison of 2 guilds for TW. Both allycodes must be entered and represent 2 different guilds.\n\n" +
                                $"Example:: {prefix}tw 729778685 632295218";
                            break;
                        case " register":
                        case "r":
                            title = "```asciidoc\n= Registration =\n";
                            embeds += title + "";
                            embeds += "Registers a user to the bot. This feature is still under development.\n\n" +
                                $"Example:: {prefix}r 729778685 @the_only_martyr";
                            break;
                        case "checkusers":
                        case "cu":
                            title = "```asciidoc\n= Check Registration =\n";
                            embeds += title + "";
                            embeds += "Checks to see if a user is registered with the bot.\n\n" +
                                $"Example:: {prefix}cu 729778685";
                            break;
                    }
                    Console.WriteLine(prefix);
                    embeds += "```";
                    embed.Description = embeds;
                }
                else { embed.Description = "You must enter a valid command for me to assist you."; }
            }
            await ctx.RespondAsync("", embed: embed);
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
        private struct ConfigJson
        {
            [JsonProperty("username")]
            public string username { get; private set; }

            [JsonProperty("password")]
            public string password { get; private set; }
        }
    }
}
