using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitService
{
    public class Deal
    {
        public string p { get; set; }
        public string v { get; set; }
        public int S { get; set; }
        public long t { get; set; }
    }

    public class DealData
    {
        public List<Deal> deals { get; set; }
        public string e { get; set; }
    }

    public class StreamMessage
    {
        public string c { get; set; }
        public DealData d { get; set; }
        public string s { get; set; }
        public long t { get; set; }
    }
}
