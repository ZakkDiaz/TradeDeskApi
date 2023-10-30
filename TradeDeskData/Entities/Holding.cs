using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeDeskData.Entities
{

    public class Holding
    {
        public int Id { get; set; }
        public int WalletId { get; set; }
        public int TrackedSymbolId { get; set; }
        public decimal Amount { get; set; }
    }
}
