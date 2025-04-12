using client.Services;
using client.Services.Bluetooth;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<BluetoothService>();

var host = builder.Build();
host.Run();
