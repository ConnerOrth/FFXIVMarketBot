using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVMarketBot.Services
{
    public class LoggingService
    {
        // declare the fields used later in this class
        private readonly ILogger<LoggingService> logger;
        private readonly DiscordSocketClient client;
        private readonly CommandService commands;

        //public LoggingService(ILogger<LoggingService> logger)
        public LoggingService(ILogger<LoggingService> logger, DiscordSocketClient client, CommandService commands)
        {
            this.logger = logger;
            this.client = client;
            this.commands = commands;
            //hook into these events with the methods provided below
            client.Ready += OnReadyAsync;
            client.Log += OnLogAsync;
            this.commands.Log += OnLogAsync;
        }

        // this method executes on the bot being connected/ready
        public Task OnReadyAsync()
        {
            logger.LogInformation($"Connected as -> [{client.CurrentUser}] :)");
            logger.LogInformation($"We are on [{client.Guilds.Count}] servers");
            return Task.CompletedTask;
        }

        // this method switches out the severity level from Discord.Net's API, and logs appropriately
        public Task OnLogAsync(LogMessage msg)
        {
            string logText = $"{msg.Source}: {msg.Exception?.ToString() ?? msg.Message}";
            switch (msg.Severity.ToString())
            {
                case "Critical":
                    {
                        logger.LogCritical(logText);
                        break;
                    }
                case "Warning":
                    {
                        logger.LogWarning(logText);
                        break;
                    }
                case "Info":
                    {
                        logger.LogInformation(logText);
                        break;
                    }
                case "Verbose":
                    {
                        logger.LogInformation(logText);
                        break;
                    }
                case "Debug":
                    {
                        logger.LogDebug(logText);
                        break;
                    }
                case "Error":
                    {
                        logger.LogError(logText);
                        break;
                    }
            }
            return Task.CompletedTask;
        }
    }
}
