using Discord;
using Discord.WebSocket;
using FFXIVMarketBot.Handlers;
using FFXIVMarketBot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FFXIVMarketBot
{
    public class MarketBot : BackgroundService
    {
        private readonly ILogger<MarketBot> logger;
        private readonly DiscordSocketClient client;
        private readonly CommandHandler commandHandler;
        private readonly MarketBoardDataDownloader marketBoardDataDownloader;
        private readonly string token;

        public MarketBot(ILogger<MarketBot> logger, DiscordSocketClient client, CommandHandler commandHandler, IConfiguration configuration, MarketBoardDataDownloader marketBoardDataDownloader)
        {
            this.logger = logger;
            this.client = client;
            token = configuration["Token"];
            this.commandHandler = commandHandler;
            this.marketBoardDataDownloader = marketBoardDataDownloader;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("MarketBot starting at: {time}", DateTimeOffset.Now);
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();
            await commandHandler.InitializeAsync();
            await marketBoardDataDownloader.ReloadWorlds(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("MarketBot running at: {time}", DateTimeOffset.Now);
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
