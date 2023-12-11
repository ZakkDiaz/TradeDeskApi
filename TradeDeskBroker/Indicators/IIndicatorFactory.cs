using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeDeskBroker
{
    public interface IIndicatorFactory
    {
        Indicator CreateIndicator(string type, int periodSeconds);
    }

}
