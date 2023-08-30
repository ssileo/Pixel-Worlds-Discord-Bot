using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
    class Utils
    {
        public static async void LockWorldDataDiscordMessage(string data, ulong channel)
        {
            string[] datas = data.Split('%');
            string worlddata = datas[0];
            string collectableData = datas[1];

            string[] LockWorldData = worlddata.Split('!');

            string owner = LockWorldData[0];
            string lastActive = LockWorldData[1];
            string timeNow = LockWorldData[2];
            string gems = LockWorldData[3];
            string world = LockWorldData[4];

            string[] collectables = collectableData.Split('=');
            List<string> collectable = new List<string>();
            foreach (string c in collectables)
            {
                try
                {
                    string[] cc = c.Split('+');

                    string toAdd = $"{cc[0]} - {cc[2]}";

                    collectable.Add(toAdd);
                }
                catch (Exception a)
                {
                    Console.WriteLine(a);
                }


            }
            collectable.Sort((x, y) => string.Compare(x, y));
            string path = $"{world}'z_Collectables.txt";


            DiscordChannel channell = await Bot.Client.GetChannelAsync(channel);

            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Red,
                Description = $"**Owner:**\n{owner}\n**Decay date:**\n{lastActive}\n**UTC now:**\n{timeNow}\n**Gems**\n{gems}",
                Title = $"**{world}'s World Lock Data**"
            };
            await channell.SendMessageAsync(embed: embed);

            File.WriteAllText(path, $"{world}'s collectables:\n" + string.Join("\n", collectable.ToArray()));

            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var msg = await new DiscordMessageBuilder()
                    .WithContent($"The dropped items in world {world}!")
                    .WithFiles(new Dictionary<string, Stream>() { { path, fs } })
                    .SendAsync(channell);
            }

            File.Delete(path);

        }

        public static async void Dumpids(string data, ulong channel)
        {
            string path = "PWitemIDSbykrak7305.txt";
            DiscordChannel channell = await Bot.Client.GetChannelAsync(channel);
            File.WriteAllText(path, data);
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var msg = await new DiscordMessageBuilder()
                    .WithContent($"Latest item id dump by @krak#7305")
                    .WithFiles(new Dictionary<string, Stream>() { { path, fs } })
                    .SendAsync(channell);
            }
            File.Delete(path);
        }

        public static async void WorldInfo(string data, ulong channel)
        {
            DiscordChannel channell = await Bot.Client.GetChannelAsync(channel);
            Console.WriteLine("Attempting to do world data : " + data);
            DiscordEmbedBuilder embed;
            if (data == "joiningalready")
            {
                embed = new DiscordEmbedBuilder
                {
                    Color = DiscordColor.Red,
                    Description = $"Try again in a few seconds.",
                    Title = $"**Already joining a world!**"
                }; await channell.SendMessageAsync(embed: embed);
                Console.WriteLine("Sent already joining");
            }
            else
            {

                string[] worldData = data.Split('!');
                string owner = worldData[0];
                string ownerId = worldData[1];
                string date = worldData[2];
                string cdate = worldData[3];
                string gems = worldData[4];
                string world = worldData[5];

                embed = new DiscordEmbedBuilder
                {
                    Color = DiscordColor.Blurple,
                    Description = $"**Owner:**\n{owner}\n**Owner's id:**\n{ownerId}\n**Decay date:**\n{date}\n**UTC now:**\n{cdate}\n**Gems:**\n{gems} <:Gem:1030555761232851068>",
                    Title = $"**World info for world \"{ world }\"**"
                }; await channell.SendMessageAsync(embed: embed);
                Console.WriteLine("Send world data " + data);
            }

        }
        public static async void GemInfo(string data, ulong channel)
        {
            DiscordChannel channell = await Bot.Client.GetChannelAsync(channel);
            Console.WriteLine("Attempting to do world data : " + data);
            DiscordEmbedBuilder embed;
            if (data == "joiningalready")
            {
                embed = new DiscordEmbedBuilder
                {
                    Color = DiscordColor.Red,
                    Description = $"Try again in a few seconds.",
                    Title = $"**Already joining a world!**"
                }; await channell.SendMessageAsync(embed: embed);
                Console.WriteLine("Sent already joining");
            }
            else
            {

                string[] worldData = data.Split(';');
                string world = worldData[0];
                string gemtotal = worldData[1];
                string mgem = worldData[2];
                string fgem = worldData[3];
                string pouchgem = worldData[4];

              

                embed = new DiscordEmbedBuilder
                {
                    Color = DiscordColor.Blurple,
                    Description = $"• gem stones - {mgem} <:HugeDiamond:1030555654777217125> \n • fish gems - {fgem} <:HugeTuna:1030555705389883443> \n • pouch gems - {pouchgem} <:LuxPouch:1030555559042236496> \n**Total gems: {gemtotal} <:Gem:1030555761232851068>**",
                    Title = $"**Gems in \"{ world }\"**"
                }; await channell.SendMessageAsync(embed: embed);
                Console.WriteLine("Send world data " + data);
            }
        }
    }
}
