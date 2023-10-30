namespace BitServerClient
{
    public class KlineSpot
    {
        public long Start;
        public long End;
        public decimal Open;
        public decimal High;
        public decimal Low;
        public decimal Close;
        public decimal Volume;
        public decimal QuoteVolume;

        // Add DateTime property for the Start timestamp
        public DateTime StartTime => DateTimeOffset.FromUnixTimeMilliseconds(Start).UtcDateTime;

        // Add DateTime property for the End timestamp
        public DateTime EndTime => DateTimeOffset.FromUnixTimeMilliseconds(End).UtcDateTime;

        // Add a property to calculate the time interval in minutes
        //public int TimeIntervalMinutes
        //{
        //    get
        //    {
        //        if (_nextKlineSpot == null) return 0;
        //        return (int)(_nextKlineSpot.StartTime - StartTime).TotalMinutes;
        //    }
        //}

        private KlineSpot _nextKlineSpot;

        public KlineSpot Copy()
        {
            return new KlineSpot()
            {
                Start = Start,
                Open = Open,
                High = High,
                Low = Low,
                Close = Close,
                Volume = Volume,
                End = End,
                QuoteVolume = QuoteVolume
            };
        }
        public bool IsBullish() => (Close > Open);
        public bool IsBearish() => !IsBullish();
        public decimal Middle() => (High + Low) / 2;
    }
}
