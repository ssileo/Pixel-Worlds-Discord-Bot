using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.CommandsNext;
using DSharpPlus.Net;
using DSharpPlus.Interactivity;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using DSharpPlus.Interactivity.Extensions;
using DiscordBot.DiscordCommands;

namespace DiscordBot
{
    class Bot
    {

        public static DiscordClient Client { get; private set; }
        public InteractivityExtension Interactivity { get; private set; }
        public CommandsNextExtension Commands { get; private set; }

        public async Task RunAsync()
        {
            var json = string.Empty;

            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync().ConfigureAwait(false);

            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            var config = new DiscordConfiguration()
            {
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Debug
                
            };

            Client = new DiscordClient(config);

            Client.Ready += OnClientReady;

            Client.UseInteractivity(new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromMinutes(2)
            }); 


            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { configJson.Prefix },
                EnableMentionPrefix = true,
                EnableDms = false,
                DmHelp = false,
                IgnoreExtraArguments= false
                
            };

            Commands = Client.UseCommandsNext(commandsConfig);

            Commands.RegisterCommands<Commands>();
            Commands.RegisterCommands<TeamCommands>();


            await Client.ConnectAsync();

            


            await Task.Delay(-1);
        }

        private Task OnClientReady(DiscordClient client, ReadyEventArgs e)
        {
            Console.WriteLine("Bot online");

           

            return Task.CompletedTask;
        }

        public static async void SendGemAmountEmbed(string data, ulong channell)
        {
            string[] k = data.Split('|');
            DiscordChannel channel = await Client.GetChannelAsync(channell);

            int gems = int.Parse(k[0]);

            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Red,
                Description = $"**{k[4].ToLower()} has {k[0]} gems**\n • {k[1]} Gem stones\n • {k[2]} Fish gems\n • {k[3]} Gem bags\n**Gem rates:**\n - {(gems / 1000) * 250} bytes at Rate (1000/250)\n - {(gems / 900) * 250} bytes at Rate (900/250)\n - {(gems / 800) * 250} bytes at Rate (800/250)\n**Use !gemrate if you amount is not here**",
                Title = $"World {k[4]}"
            };

            await channel.SendMessageAsync(embed: embed);
        }
    }
}
