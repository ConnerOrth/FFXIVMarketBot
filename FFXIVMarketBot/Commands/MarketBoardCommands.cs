using Discord;
using Discord.Commands;
using Discord.WebSocket;
using FFXIVMarketBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FFXIVMarketBot.Commands
{
    // for commands to be available, and have the Context passed to them, we must inherit ModuleBase
    public class MarketBoardCommands : ModuleBase
    {
        private readonly HttpClient httpClient;
        private readonly MarketBoardDataDownloader marketBoardDataDownloader;

        public MarketBoardCommands(IHttpClientFactory httpClientFactory, MarketBoardDataDownloader marketBoardDataDownloader)
        {
            this.httpClient = httpClientFactory.CreateClient();
            this.marketBoardDataDownloader = marketBoardDataDownloader;
        }

        [Command("ping")]
        public async Task PingCommand()
        {
            int latency = ((DiscordSocketClient)Context.Client).Latency;

            await ReplyAsync($"pong ({latency}ms)");
        }

        [Command("clear")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task ClearCommand()
        {
            IEnumerable<IMessage> messages = await Context.Channel.GetMessagesAsync().FlattenAsync();
            await ((ITextChannel)Context.Channel).DeleteMessagesAsync(messages);
            const int delay = 3000;
            IUserMessage m = await ReplyAsync($"---I have deleted {messages.Count()} messages.---");
            await Task.Delay(delay);
            await m.DeleteAsync();
        }

        [Command("mb")]
        public async Task MarketBoardCommand([Remainder]string args = null)
        {
            string worldId;
            string itemName = args;
            string itemIcon;
            long itemId;
            string worldOrDataCenter = "Light";

            var content = Context.Message.Content;
            if (string.IsNullOrWhiteSpace(args))
            {
                //Crescent Spring Water
                await ReplyAsync("Please provide an item name in your command, (optionally followed by the World or DC name) to query.");
            }
            string[] itemWorld = args.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (itemWorld.Length > 1)
            {
                worldOrDataCenter = itemWorld.Last().Trim();
                itemName = string.Join(' ', itemWorld[0..^1]);
            }

            marketBoardDataDownloader.Worlds.TryGetValue(worldOrDataCenter, out worldId);
            if (worldId is null) await ReplyAsync("that isn't a valid World or DC name. Please check the spelling of the name entered.");

            string result = await httpClient.GetStringAsync("https://xivapi.com/search?string=" + itemName);

            ItemSearch itemSearch = ItemSearch.FromJson(result);
            if (itemSearch.Items.Count == 0)
            {
                await ReplyAsync("no results found for " + itemName + ", are you sure you spelled the item name correctly?");
            }

            Item item = itemSearch.Items.FirstOrDefault(i => i.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase) || i.Name.Contains(itemName, StringComparison.OrdinalIgnoreCase));
            if (item is null) await ReplyAsync("no results found for " + itemName + ", are you sure you spelled the item name correctly?");

            itemId = item.Id;
            itemName = item.Name;
            itemIcon = "https://xivapi.com" + item.Icon;

            var allListings = await httpClient.GetStringAsync($"https://universalis.app/api/{worldId}/{itemId}");
            var trimmedListings = ItemListings.FromJson(allListings)
                .Listings
                .Select(listing =>
                    new Listing()
                    {
                        Hq = listing.Hq,
                        PricePerUnit = listing.PricePerUnit,
                        Quantity = listing.Quantity,
                        RetainerName = listing.RetainerName,
                        RetainerCity = listing.RetainerCity,
                        Total = listing.Total,
                        WorldName = listing.WorldName
                    }
                )
                .OrderBy(l => l.PricePerUnit)
                .ThenBy(l => l.Total)
                .Take(24);

            EmbedBuilder embedBuilder = new EmbedBuilder();
            embedBuilder.WithTitle(itemName);
            embedBuilder.WithUrl("https://universalis.app/market/" + itemId);
            embedBuilder.WithThumbnailUrl(itemIcon);
            embedBuilder.WithColor(Color.Purple);
            embedBuilder.WithFooter("Above shows both HQ and NQ");

            foreach (var listing in trimmedListings)
            {
                embedBuilder.AddField(listing.WorldName, $"**${listing.PricePerUnit}**, x{listing.Quantity} @{listing.RetainerCityName}", true);
            }

            await ReplyAsync(null, false, embedBuilder.Build());
        }
    }
}
