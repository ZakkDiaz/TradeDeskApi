using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TradeDeskBroker.Market;

namespace TradeDeskBroker
{
    public class MarketEvaluationService : BackgroundService
    {
        private readonly ILogger<MarketEvaluationService> _logger;
        private readonly IMarketService _marketService;
        private readonly List<TradeProfile> _tradeProfiles;
        private readonly TimeSpan _evaluationInterval;
        private readonly ITradeContext _tradeContext;

        public MarketEvaluationService(ILogger<MarketEvaluationService> logger, IMarketService marketService, ITradeContext tradeContext, List<TradeProfile> tradeProfiles)
        {
            _logger = logger;
            _marketService = marketService;
            _tradeProfiles = tradeProfiles;
            _tradeContext = tradeContext;
            _evaluationInterval = TimeSpan.FromSeconds(1); // Set the interval for evaluation
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var symbol = "BTCUSDT";

            var startTime = DateTime.UtcNow.AddHours(-1);
            var endTime = DateTime.UtcNow;

            try
            {
                var profileTasks = _tradeProfiles.Select(async profile =>
                {
                    DateTime currentTime = startTime;

                    while (currentTime < endTime)
                    {
                        try
                        {
                            var signal = await profile.EvaluateTradeSignalAsync(symbol, _marketService, currentTime);
                            if (signal != null)
                            {
                                signal.SignalTime = currentTime;
                                await _tradeContext.SubmitTrade(signal);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Error occurred during market evaluation for profile {profile.Name}.");
                        }

                        currentTime = currentTime.AddMinutes(1); // Increment time by 1 minute
                    }
                }).ToList();

                await Task.WhenAll(profileTasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during parallel processing of trade profiles.");
            }


            // Continue processing in real-time
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Market Evaluation Service running at: {time}", DateTimeOffset.Now);
                try
                {
                    // Use the current time as the offset for real-time processing
                    var currentTime = DateTime.UtcNow;
                    foreach (var profile in _tradeProfiles)
                    {
                        var signal = await profile.EvaluateTradeSignalAsync(symbol, _marketService, currentTime);
                        if (signal != null)
                        {
                            await _tradeContext.SubmitTrade(signal);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during real-time market evaluation.");
                }

                await Task.Delay(_evaluationInterval, stoppingToken);
            }
        }


    }

}
