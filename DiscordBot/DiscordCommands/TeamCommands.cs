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

namespace DiscordBot.DiscordCommands
{
    class TeamCommands : BaseCommandModule
    {

        [Command("member")]
        public async Task member(CommandContext ctx)
        {
            var memberEmbed = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Red,
                Title = "**Would you like to receive member role?**",
                Description = "Gives you the **member role** which lets you acces more channels!"
                
            };

            

            var joinmsg = await ctx.Channel.SendMessageAsync(embed: memberEmbed).ConfigureAwait(false);

            var ThumbsUpEmoji = DiscordEmoji.FromName(ctx.Client, ":+1:");
            var ThumbsDownEmoji = DiscordEmoji.FromName(ctx.Client, ":-1:");

            await joinmsg.CreateReactionAsync(ThumbsUpEmoji).ConfigureAwait(false);
            await joinmsg.CreateReactionAsync(ThumbsDownEmoji).ConfigureAwait(false);

            var interactivy = ctx.Client.GetInteractivity();

            var result = await interactivy.WaitForReactionAsync(
                x => x.Message == joinmsg &&
                x.User == ctx.User &&
                (x.Emoji == ThumbsUpEmoji || x.Emoji == ThumbsDownEmoji)).ConfigureAwait(false);

            if (result.Result.Emoji == ThumbsUpEmoji)
            {
                var role = ctx.Guild.GetRole(1024493181393457212);
                await ctx.Member.GrantRoleAsync(role).ConfigureAwait(false);
            }
            else if (result.Result.Emoji == ThumbsDownEmoji)
            {
                // dont do anything
            }
            else
            {
                Console.WriteLine("something went wrong");
            }

            await joinmsg.DeleteAsync().ConfigureAwait(false);
            await ctx.Message.DeleteAsync().ConfigureAwait(false);
        }
    }
}
