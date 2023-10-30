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
services.AddSingleton(Options.Create(new DatabaseConfig() { ConnectionString = System.IO.File.ReadAllText("conn.txt") }));
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
    await _socket.Connect(symbol.Symbol);
}

while (true)
{
    System.Threading.Thread.Sleep(100);
}