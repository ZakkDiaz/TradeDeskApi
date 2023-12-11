using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace BitServerClient
{
    public class BitServerSocket
    {
        private static string _apiUrl = "wss://wbs.mexc.com/ws";
        private ClientWebSocket _ws;
        private Task _wsT;
        private CancellationToken _cancellationTokenSource;
        private int _maxBuf = 1024;
        private Action<string> _onMessage;
        private List<string> _tickers = new List<string>();  // To keep track of all tickers

        public BitServerSocket(Action<string> onMessage)
        {
            _onMessage = onMessage;
            _wsT = Task.Run(ProcessMessages);
            if (_wsT.Status == TaskStatus.Created)
                _wsT.Start();
        }

        public void AddSymbol(string ticker)
        {
            _tickers.Add(ticker);  // Add the ticker to the list
        }

        public async Task Connect()
        {
            await connect();
        }

        public async Task Reconnect()
        {
            _ws?.Abort();  // Close the existing WebSocket connection
            _ws = null;  // Reset the WebSocket object
            await connect();  // Re-initialize the WebSocket connection
        }

        List<string> tickers = new List<string>();
        private async Task connect()
        {
            try
            {
                if (_ws == null)
                {
                    _ws = new ClientWebSocket();
                    await _ws.ConnectAsync(new Uri(_apiUrl), CancellationToken.None);
                }

                foreach (var ticker in _tickers)
                {
                    if (tickers.Contains(ticker))
                        continue;
                    await Send($"{{ \"method\":\"SUBSCRIPTION\", \"params\":[\"spot@public.deals.v3.api@{ticker.Replace("_", "")}\"] }}");
                    tickers.Add(ticker);
                }
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log it)
            }
        }

        private async Task Send(string message)
        {
            await _ws.SendAsync(new ArraySegment<byte>(Encoding.ASCII.GetBytes(message)), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private Regex failRgx = new Regex("no subscription success");

        private async Task ProcessMessages()
        {
            var _buf = new byte[_maxBuf];
            StringBuilder _res = new StringBuilder();

            while (true)
            {
                while (_ws == null || _ws.State != WebSocketState.Open) { Thread.Sleep(100); }

                try
                {
                    var result = await _ws.ReceiveAsync(new ArraySegment<byte>(_buf), _cancellationTokenSource);
                    _res.Append(Encoding.UTF8.GetString(_buf));

                    while (!result.EndOfMessage)
                    {
                        result = await _ws.ReceiveAsync(new ArraySegment<byte>(_buf), _cancellationTokenSource);
                        _res.Append(Encoding.UTF8.GetString(_buf));
                    }

                    var txt = _res.ToString();
                    if (!failRgx.IsMatch(txt))
                        _onMessage(txt);

                    _res.Clear();
                }
                catch
                {
                    _ws = null;
                    await connect();
                }

                Thread.Sleep(5);
            }
        }

        public async Task AddSymbolAndConnect(string symbol)
        {
            AddSymbol(symbol);
            await connect();
        }
    }
}