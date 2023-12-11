using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeDeskBroker.Market
{
    public class TimeWindow
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public TimeWindow(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }

        // Method to check if two windows overlap
        public bool OverlapsWith(TimeWindow other)
        {
            return Start < other.End && End > other.Start;
        }

        // Method to merge this window with another, assuming they overlap
        public void MergeWith(TimeWindow other)
        {
            Start = new DateTime(Math.Min(Start.Ticks, other.Start.Ticks));
            End = new DateTime(Math.Max(End.Ticks, other.End.Ticks));
        }

        // Method to check if a specific DateTime falls within this window
        public bool Contains(DateTime time)
        {
            return Start <= time && time <= End;
        }
    }
}
