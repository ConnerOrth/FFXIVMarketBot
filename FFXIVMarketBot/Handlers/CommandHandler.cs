using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVMarketBot.Handlers
{
    public class CommandHandler
    {
        // setup fields to be set later in the constructor
        private readonly IConfiguration configuration;
        private readonly CommandService commandService;
        private readonly DiscordSocketClient client;
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger logger;

        public CommandHandler(IServiceProvider serviceProvider, IConfiguration configuration, CommandService commandService, DiscordSocketClient client, ILogger<CommandHandler> logger)
        {
            this.serviceProvider = serviceProvider;
            this.configuration = configuration;
            this.commandService = commandService;
            this.client = client;
            this.logger = logger;

            // take action when we execute a command
            this.commandService.CommandExecuted += CommandExecutedAsync;

            // take action when we receive a message (so we can process it, and see if it is a valid command)
            this.client.MessageReceived += MessageReceivedAsync;

        }

        public async Task InitializeAsync()
        {
            // register modules that are public and inherit ModuleBase<T>.
            await commandService.AddModulesAsync(Assembly.GetEntryAssembly(), serviceProvider);
        }

        // this class is where the magic starts, and takes actions upon receiving messages
        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            // ensures we don't process system/other bot messages
            if (!(rawMessage is SocketUserMessage message))
            {
                return;
            }

            if (message.Source != MessageSource.User)
            {
                return;
            }

            if (!message.Channel.Name.Equals("ffxiv-bot", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // sets the argument position away from the prefix we set
            var argPos = 0;

            // get prefix from the configuration file
            char prefix = char.Parse(configuration["Prefix"]);

            // determine if the message has a valid prefix, and adjust argPos based on prefix
            if (!(message.HasMentionPrefix(client.CurrentUser, ref argPos) || message.HasCharPrefix(prefix, ref argPos)))
            {
                return;
            }

            var context = new SocketCommandContext(client, message);

            // execute command if one is found that matches
            await commandService.ExecuteAsync(context, argPos, serviceProvider);
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            // if a command isn't found, log that info to console and exit this method
            if (!command.IsSpecified)
            {
                if (result.Error == CommandError.UnknownCommand)
                {
                    logger.LogError($"[{result.ErrorReason[0..^1]}] was called by [{context.User.Username}] <-> [{context.Message.Content}]");
                    return;
                }
                logger.LogError($"Command failed to execute for [{context.User.Username}] <-> [{result.ErrorReason}]!");
                return;
            }


            // log success to the console and exit this method
            if (result.IsSuccess)
            {
                logger.LogInformation($"Command [{command.Value.Name}] executed for [{context.User.Username}] on [{context.Guild.Name}]");
                return;
            }

            // failure scenario, let's let the user know
            await context.Channel.SendMessageAsync($"Sorry, {context.User.Username}... something went wrong -> [{result}]!");
        }
    }
}
