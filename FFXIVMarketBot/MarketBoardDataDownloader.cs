using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FFXIVMarketBot
{
    public class MarketBoardDataDownloader
    {
        public IDictionary<string, string> Worlds { get; private set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private readonly ILogger<MarketBot> logger;
        private readonly HttpClient httpClient;

        public MarketBoardDataDownloader(ILogger<MarketBot> logger, IHttpClientFactory httpClientFactory)
        {
            this.logger = logger;
            this.httpClient = httpClientFactory.CreateClient();            
        }

        public async Task ReloadWorlds(CancellationToken cancellationToken)
        {
            TimeSpan delay = TimeSpan.FromHours(1);
            while (!cancellationToken.IsCancellationRequested)
            {
                logger.LogInformation("MarketBoardDataDownloader running at: {time}", DateTimeOffset.Now);
                
                var content = await httpClient.GetStringAsync(new Uri("https://raw.githubusercontent.com/xivapi/ffxiv-datamining/master/csv/World.csv"));
                IDictionary<string, string> worldIds = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var line in content.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Skip(3))
                {
                    var columns = line.Split(new char[] { ',' });
                    if (!worldIds.ContainsKey(Regex.Replace(columns[1], @"[^a-zA-Z0-9]+", "")))
                    {
                        worldIds.Add(Regex.Replace(columns[1], @"[^a-zA-Z0-9]+", ""), columns[0]);
                    }
                }
                foreach (var server in new string[] { "Chaos", "Light", "Elemental", "Gaia", "Mana", "Aether", "Crystal", "Primal" })
                {
                    if (!worldIds.ContainsKey(server))
                    {
                        worldIds.Add(server, server);
                    }
                }
                
                Worlds = worldIds;

                logger.LogInformation("MarketBoardDataDownloader next run scheduled at: {time}", DateTimeOffset.Now.Add(delay));
                await Task.Delay(delay, cancellationToken);
            }
        }
    }
}