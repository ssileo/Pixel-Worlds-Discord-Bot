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
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity.Extensions;
using TcpServer.API;

namespace DiscordBot
{
    public class Commands : BaseCommandModule
    {
        [Command("ping")]
        [RequireRoles(RoleCheckMode.Any, "Pirate", "Owner")]
        [Description("says pong back lol")]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync($"Pong, latency {ctx.Client.Ping} ms").ConfigureAwait(false);
            
        }


        [Command("Join")]
        [RequireRoles(RoleCheckMode.Any, "member", "Pirate", "Owner")]
        [Description("Joins the world you want to join")]
        public async Task JoinWorld(CommandContext ctx, string world)
        {
            char[] ok = world.ToCharArray();
            char[] invalidStartingChars = { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
            string[] restrictedWorlds = { "buggedcrash", "both", "buy", "mineworld", "netherworld", "jetrace", "raven" };

            DiscordChannel channel = ctx.Channel;

            foreach (char c in invalidStartingChars)
            {
                if (world.StartsWith(c.ToString()))
                {
                    DiscordEmbedBuilder embed = new DiscordEmbedBuilder
                    {
                        Color = DiscordColor.Red,
                        Description = $"Worlds can't start with numbers {ctx.Member.Mention}!",
                        Title = $"Error"
                    };
                    await channel.SendMessageAsync(embed: embed);
                    Console.WriteLine("Worlds can't start with numbers error given");
                    return;
                }
            }

            if (ok.Length > 15 || ok.Length < 2)
            {
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder
                {
                    Color = DiscordColor.Red,
                    Description = $"Length of world is invalid {ctx.Member.Mention}!",
                    Title = $"Error"
                };
                await channel.SendMessageAsync(embed: embed);
                Console.WriteLine($"{ctx.User} entered an invalid world");
                return;
            }
            

            foreach (string s in restrictedWorlds)
            {
                if (world == s)
                {
                    DiscordEmbedBuilder embed = new DiscordEmbedBuilder
                    {
                        Color = DiscordColor.Red,
                        Description = $"That world is restricted {ctx.Member.Mention}!",
                        Title = $"Error"
                    };
                    await channel.SendMessageAsync(embed: embed);
                    return;
                }
            }
                

            await ctx.Channel.SendMessageAsync($"{ctx.User.Mention} I'm joining {world}!");

            Program.Write(Program._client.GetStream(), new APIRequest() { opcode = Opcode.JoinWorld, Data = $"{world}" });
            Console.WriteLine("packets for join world sent!");

        }

        [Command("gems")]
        [RequireRoles(RoleCheckMode.Any, "member", "Pirate", "Owner")]
        [Description("gets gem amount of gems in world")]
        public async Task Gems(CommandContext ctx, string world)
        {
            Program.Write(Program._client.GetStream(), new APIRequest() { opcode = Opcode.Gems,Data = world, Channel = ctx.Channel.Id });
            Console.WriteLine("Sent get gems to client");

        }

        [Command("restart")]
        [RequireRoles(RoleCheckMode.Any, "member", "Admin", "Owner")]
        [Description("if bot is stuck or bugged it should fix it")]
        public async Task RestartPW(CommandContext ctx)
        {
            Program.Write(Program._client.GetStream(), new APIRequest() { opcode = Opcode.Restart });
            DiscordChannel channel = ctx.Channel;

            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Red,
                Description = $"Pixel worlds is being restarted on the bot",
                Title = $"Restarting!"
            };

            await channel.SendMessageAsync(embed: embed);


        }

        [Command("Leave")]
        [RequireRoles(RoleCheckMode.Any, "member", "Admin", "Owner")]
        [Description("if bot is stuck or bugged it should fix it")]
        public async Task GoToMainMenu(CommandContext ctx)
        {
            Program.Write(Program._client.GetStream(), new APIRequest() { opcode = Opcode.Leave });
            DiscordChannel channel = ctx.Channel;

            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Red,
                Title = $"joining main menu"
            };

            await channel.SendMessageAsync(embed: embed);


        }

        [Command("info")]
        [RequireRoles(RoleCheckMode.Any, "member", "Admin", "Owner")]
        [Description("gives world info")]
        public async Task LockWorldinfo(CommandContext ctx)
        {
            
            Program.Write(Program._client.GetStream(), new APIRequest() { opcode = Opcode.LockWorldData, Channel = ctx.Channel.Id });
            Console.WriteLine("Packets for lock world data were sent!");
        }

        [Command("dump")]
        [RequireRoles(RoleCheckMode.Any, "member", "Admin", "Owner")]
        [Description("dumps latest pw ids")]
        public async Task DumpIds(CommandContext ctx)
        {

            Program.Write(Program._client.GetStream(), new APIRequest() { opcode = Opcode.itemIDs, Channel = ctx.Channel.Id });
            Console.WriteLine("Packets for id dump were sent!");
        }

        [Command("gemrate")]
        [RequireRoles(RoleCheckMode.Any, "member", "Admin", "Owner")]
        [Description("workout gem rates fast")]
        public async Task rateofgems(CommandContext ctx, int amount, int rate)
        {
            double bytes = (amount / rate) * 250;
            double bytess = Math.Round(bytes, 2);

            DiscordChannel channel = ctx.Channel;

            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Red,
                Description = $"({amount} / {rate}) x 250 = {bytess} bytes",
                Title = $"{bytess} bytes!"
            };
            await channel.SendMessageAsync(embed: embed);
        }


        [Command("coin")]
        [RequireRoles(RoleCheckMode.Any, "member", "Pirate", "Owner")]
        [Description("do a coin flip")]
        public async Task coinflip(CommandContext ctx,string guess, int amount)
        {
            if (guess.ToLower() == "heads" || guess.ToLower() == "tails")
            {
                Console.WriteLine($"{ctx.Member.Username} made a valid guess");
            }
            else
            {
                DiscordEmbedBuilder invalid = new DiscordEmbedBuilder
                {
                    Color = DiscordColor.Red,
                    Title = $"{ctx.Member.Username}, **{guess}** is an invalid guess!"
                };
                await ctx.Channel.SendMessageAsync(invalid);
                return;
            }

            string coinflipstat = Casino.Logic.CoinflipLogic.CoinflippLogic(guess, amount);

            string[] coindata = coinflipstat.Split('|');
            string coinflip = $"{ctx.User.Username} you {coindata[0]} {coindata[2]} bytes, because it was {coindata[1]}!";
            DiscordEmbedBuilder embedd = new DiscordEmbedBuilder();
            if (coindata[0] == "won")
            {
                DiscordEmbedBuilder won = new DiscordEmbedBuilder
                {
                    Color = DiscordColor.Green,
                    Title = $"{coinflip}"
                };

                embedd = won;
            }
            else
            {

                DiscordEmbedBuilder lost = new DiscordEmbedBuilder
                {
                    Color = DiscordColor.Red,
                    Title = $"{coinflip}"
                };
                embedd = lost;
            }


            await ctx.Channel.SendMessageAsync(embedd);


        }

        [Command("say")]
        [RequireRoles(RoleCheckMode.Any, "member", "Pirate", "Owner")]
        [Description("Get the bot to say something")]
        public async Task say(CommandContext ctx, params string[] message)
        {
            string text = string.Join(" ", message);
            Program.Write(Program._client.GetStream(), new APIRequest() { opcode = Opcode.Say, Data = text});
            Console.WriteLine($"Attempting to say\"{text}\".");

            await ctx.Message.DeleteAsync();
        }

        [Command("move")]
        [RequireRoles(RoleCheckMode.Any,"member", "Pirate", "Owner")]
        [Description("Get the bot to say something")]
        public async Task say(CommandContext ctx, string direction)
        {
            string[] directions = {"left", "right"};
            
            if (direction == "left" || direction == "right")
            {
                // hi
            }
            else
            {
                DiscordEmbedBuilder failure = new DiscordEmbedBuilder
                {
                    Color = DiscordColor.Red,
                    Title = $"{direction} is not a direction!"
                };
                await ctx.Channel.SendMessageAsync(embed: failure).ConfigureAwait(false);
                return;
            }

            Program.Write(Program._client.GetStream(), new APIRequest() { opcode = Opcode.Move, Data = direction });

            await ctx.Channel.DeleteMessageAsync(ctx.Message).ConfigureAwait(false);
        }

        [Command("world")]
        [RequireRoles(RoleCheckMode.Any, "member", "Admin", "Owner")]
        [Description("Get gems and info of worlds!")]
        public async Task WorldInfo(CommandContext ctx, string worldName)
        {
            Program.Write(Program._client.GetStream(), new APIRequest() { opcode = Opcode.World, Data = worldName, Channel = ctx.Channel.Id });
            Console.WriteLine("Packets for world info were sent!");
            await ctx.Channel.SendMessageAsync("Attempting to join " + worldName + $" {ctx.Member.Mention}");
        }
    }
}
