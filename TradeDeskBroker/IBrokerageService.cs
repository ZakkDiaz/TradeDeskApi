using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeDeskBroker.Models;

namespace TradeDeskBroker
{
    public interface IBrokerageService
    {
        Wallet GetWallet(string name);
        void AddFunds(string name, decimal funds);
    }
}
