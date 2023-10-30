namespace BitServerClient
{
    public class Kline
    {
        public bool success;
        public int code;
        public KData data;
    }
    public class KData
    {
        public long[] time;
        public decimal[] open;
        public decimal[] close;
        public decimal[] high;
        public decimal[] low;
        public decimal[] vol;
        public decimal[] amount;
    }
}
