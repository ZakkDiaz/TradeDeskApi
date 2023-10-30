using BitServerClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using TradeDeskData;

namespace BitService
{
    public static class BitServerSocketFactory
    {
        private static BitServerSocket _socket;
        public static BitServerSocket Create(IServiceProvider serviceProvider)
        {
            var _repo = serviceProvider.GetRequiredService<IFinancialRepository>();

            void OnUpdate(string message)
            {
                Console.WriteLine(message);
                if (message.Contains("{\"id\":0,\"code\":0,\"msg\""))
                    return;
                var bmessage = BalanceBrackets(message);
                try
                {
                    var streamMessage = JsonConvert.DeserializeObject<StreamMessage>(bmessage);
                    if (streamMessage != null && streamMessage.d != null && streamMessage.d.deals != null)
                        foreach (var deal in streamMessage.d.deals)
                        {
                            // Parse and validate the data
                            if (decimal.TryParse(deal.p, out decimal price) &&
                                decimal.TryParse(deal.v, out decimal quantity))
                            {
                                // Insert the data into the database
                                var result = _repo.InsertDataStreamAsync(
                                    tradeType: deal.S,
                                    price: price,
                                    dealTime: deal.t,
                                    quantity: quantity,
                                    eventType: streamMessage.d.e,
                                    symbol: streamMessage.s,
                                    eventTime: streamMessage.t
                                ).Result;

                                Console.WriteLine($"Inserted data with ID: {result}");
                            }
                            else
                            {
                                Console.WriteLine("Failed to parse deal data.");
                            }
                        }
                }
                catch (Exception ex)
                {
                    _socket.Reconnect().Wait();
                }
            }

            _socket = new BitServerSocket(OnUpdate);
            return _socket;
        }
        private static string BalanceBrackets(string message)
        {
            int openCount = 0;
            int closeCount = 0;
            int lastBalancedCloseIndex = -1;

            StringBuilder sb = new StringBuilder();

            // Loop through the message to count open and close brackets
            for (int i = 0; i < message.Length; i++)
            {
                if (message[i] == '{')
                {
                    openCount++;
                }
                else if (message[i] == '}')
                {
                    closeCount++;
                }

                // Record the position where the brackets are balanced
                if (openCount == closeCount)
                {
                    sb.Append(message[i]);
                    return sb.ToString();
                }
                sb.Append(message[i]);
            }

            //// Truncate the message at the last balanced closing bracket, if one exists
            //if (lastBalancedCloseIndex != -1)
            //{
            //    var _m =  message.Substring(0, lastBalancedCloseIndex + 1);
            //    return _m;
            //}

            return message;
        }
    }
}

