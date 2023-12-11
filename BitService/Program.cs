// See https://aka.ms/new-console-template for more information
using BitServerClient;
using BitService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Sockets;
using TradeDeskData;

Console.WriteLine("Hello, World!");

IServiceCollection services = new ServiceCollection();
services.AddHttpClient();
services.AddHttpClient<BitServerClient.BitServerClient>();
var appSettings = Newtonsoft.Json.JsonConvert.DeserializeObject<DatabaseConfig>(System.IO.File.ReadAllText("appsettings.json"));
services.AddSingleton(Options.Create(appSettings));
services.AddSingleton<IFinancialRepository, FinancialRepository>();
var _serviceProvider = services.BuildServiceProvider();
var _client = _serviceProvider.GetRequiredService<BitServerClient.BitServerClient>();
var _repo = _serviceProvider.GetRequiredService<IFinancialRepository>();

// Use factory to create BitServerSocket
var _socket = BitServerSocketFactory.Create(_serviceProvider);

// Main code
var symbols = await _repo.GetTrackedSymbolsAsync();

foreach (var symbol in symbols)
{
    _socket.AddSymbol(symbol.Symbol);
}

await _socket.Connect();

while (true)
{
    System.Threading.Thread.Sleep(5000);
    var _newSymbols = await _repo.GetTrackedSymbolsAsync();
    if(_newSymbols != null)
    foreach (var _s in _newSymbols)
    {
        if (!symbols.Any(s => s.Symbol == _s.Symbol))
        {
            await _socket.AddSymbolAndConnect(_s.Symbol);
            symbols = symbols.Append(_s);
        }
    }
}