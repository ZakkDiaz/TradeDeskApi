using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TradeDeskBroker.Models;
using TradeDeskData;
using TradeDeskData.Entities;

namespace TradeDeskBroker.Market
{
    public class MarketService : IMarketService
    {
        private readonly Dictionary<string, LinkedList<MarketTick>> _data = new Dictionary<string, LinkedList<MarketTick>>();
        private readonly Dictionary<string, List<TimeWindow>> _queriedWindows = new Dictionary<string, List<TimeWindow>>();
        private readonly IFinancialRepository _repo;

        public MarketService(IFinancialRepository repo)
        {
            _repo = repo;
        }

        private void AddData(IEnumerable<DataStream> dataStreams)
        {
            if (!dataStreams.Any())
                return;
            var symbol = dataStreams.FirstOrDefault().Symbol;
            var dat = dataStreams.Select(data => new MarketTick
            {
                Price = data.Price,
                Volume = data.Quantity,
                Symbol = data.Symbol,
                TradedOn = DateTimeOffset.FromUnixTimeMilliseconds(data.DealTime).UtcDateTime,
                Id = data.Id,
                IsBuy = data.TradeType == 1
            });

            if (!_data.ContainsKey(symbol))
            {
                _data[symbol] = new LinkedList<MarketTick>();
            }
            _data[symbol].InsertSortedData(dat);
        }

        public async Task<IEnumerable<MarketTick>> GetDataInRange(string symbol, DateTime from, DateTime to)
        {
            if (!_data.ContainsKey(symbol))
                _data[symbol] = new LinkedList<MarketTick>();

            DateTime utcFrom = from.ToUniversalTime();
            DateTime utcTo = to.ToUniversalTime();
            var existingWindows = _queriedWindows.ContainsKey(symbol) ? _queriedWindows[symbol] : new List<TimeWindow>();
            var newWindow = new TimeWindow(utcFrom, utcTo);

            var rangesToQuery = DetermineRangesToQuery(existingWindows, newWindow);
            AddAndMergeWindow(symbol, newWindow);
            foreach (var range in rangesToQuery)
            {
                var newData = await _repo.GetTradesBetween(symbol, range.Start, range.End);
                AddData(newData);
            }

            TrimQueriedWindows(symbol);
            return _data[symbol].Where(tick => tick.TradedOn >= utcFrom && tick.TradedOn <= utcTo).ToList();
        }

        private void AddAndMergeWindow(string symbol, TimeWindow newWindow)
        {
            if (!_queriedWindows.ContainsKey(symbol))
                _queriedWindows[symbol] = new List<TimeWindow>();

            _queriedWindows[symbol].Add(newWindow);
            _queriedWindows[symbol] = MergeOverlappingWindows(_queriedWindows[symbol]);
        }
        private void TrimQueriedWindows(string symbol)
        {
            if (!_data.ContainsKey(symbol) || !_data[symbol].Any())
                return;

            var lastRecordTime = _data[symbol].Last.Value.TradedOn;
            if (_queriedWindows.ContainsKey(symbol))
            {
                var windows = _queriedWindows[symbol];
                for (int i = 0; i < windows.Count; i++)
                {
                    if (windows[i].End > lastRecordTime)
                    {
                        windows[i] = new TimeWindow(windows[i].Start, lastRecordTime);
                    }
                }
            }
        }


        private List<TimeWindow> MergeOverlappingWindows(List<TimeWindow> windows)
        {
            var mergedWindows = new List<TimeWindow>();
            foreach (var window in windows.OrderBy(w => w.Start))
            {
                if (!mergedWindows.Any() || mergedWindows.Last().End < window.Start)
                    mergedWindows.Add(window);
                else if (mergedWindows.Last().End < window.End)
                    mergedWindows.Last().End = window.End;
            }

            return mergedWindows;
        }

        private IEnumerable<TimeWindow> DetermineRangesToQuery(List<TimeWindow> existingWindows, TimeWindow newWindow)
        {
            var rangesToQuery = new List<TimeWindow>();
            DateTime start = newWindow.Start;
            DateTime end = newWindow.End;

            foreach (var window in existingWindows)
            {
                if (window.End <= start)
                    continue; // Current window is entirely before the new window's start

                if (window.Start <= start && window.End < end)
                {
                    // The start of the new window is covered by this window, but not the end
                    start = window.End;
                }
                else if (window.Start <= start && window.End >= end)
                {
                    // The new window is entirely covered by this window
                    return Enumerable.Empty<TimeWindow>();
                }
                else if (window.Start > start && window.Start < end)
                {
                    // There is an uncovered portion before this window starts
                    rangesToQuery.Add(new TimeWindow(start, window.Start));
                    start = window.End > end ? end : window.End;
                }
            }

            if (start < end)
            {
                // Add the remaining portion after the last window
                rangesToQuery.Add(new TimeWindow(start, end));
            }

            return rangesToQuery;
        }

        public async Task<IEnumerable<TradeSignal>> GetTradesInRange(string symbol, DateTime from, DateTime to)
        {
            var signals = await _repo.GetTradeSignals(symbol);
            if(signals == null)
                return new List<TradeSignal>();
            return signals.Select(s => new TradeSignal()
            {
                Confidence = s.Confidence,
                SignalTime = s.SignalTime.ToLocalTime(),
                IsBuy = s.IsBuy,
                Price = s.Price,
                RiskLevel = s.RiskLevel,
                SignalWeight = s.SignalWeight,
                Symbol = s.Symbol,
                Leverage = s.Leverage,
                StopLoss = s.StopLoss,
                TakeProfit = s.TakeProfit
            }).ToList();
        }
    }
}
public static class MarketHelpers
{
    public static void InsertSortedData(this LinkedList<MarketTick> list, IEnumerable<MarketTick> sortedData)
    {
        var currentNode = list.First;

        foreach (var tick in sortedData)
        {
            // Find the position where the new tick should be inserted
            while (currentNode != null && currentNode.Value.TradedOn <= tick.TradedOn)
            {
                currentNode = currentNode.Next;
            }

            // Insert the tick at the found position
            if (currentNode != null)
            {
                list.AddBefore(currentNode, tick);
            }
            else
            {
                list.AddLast(tick);
            }
        }
    }
}
