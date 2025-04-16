using System.Diagnostics;
using client.Services.Bluetooth;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);
var messenger = new WeakReferenceMessenger();

builder.Services.AddSingleton<IMessenger>(messenger);
builder.Services.AddHostedService<VibrationManager>();
builder.Services.AddHostedService<BluetoothService>();

Console.WriteLine("==Application started==");
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddLogging(configure => configure.AddConsole());

    Console.WriteLine("Waiting for debugger to attach");
    while (!Debugger.IsAttached)
    {
        Thread.Sleep(1000);
    }
    Console.WriteLine("Debugger attached.");
}

var host = builder.Build();
host.Run();

