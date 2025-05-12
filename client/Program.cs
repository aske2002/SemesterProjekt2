using System.Diagnostics;
using System.Globalization;
using client.Services.Bluetooth;
using client.Services.Button;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using shared.Models;

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

var builder = Host.CreateApplicationBuilder(args);
var loggingProvider = new CustomLoggingProvider();
builder.Logging.ClearProviders();
builder.Logging.AddProvider(loggingProvider);
builder.Services.AddSingleton<ILoggerProvider>(loggingProvider);
var logger = loggingProvider.CreateLogger("Program");

var messenger = new WeakReferenceMessenger();

builder.Services.AddSingleton<IMessenger>(messenger);
builder.Services.AddHostedService<VibrationManager>();
builder.Services.AddHostedService<BluetoothService>();
builder.Services.AddHostedService<HardwareButtonManager>();


logger.LogInformation("==Application starting==");
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddLogging(configure => configure.AddConsole());

    logger.LogInformation("Waiting for debugger to attach");
    while (!Debugger.IsAttached)
    {
        Thread.Sleep(1000);
    }
    logger.LogInformation("Debugger attached.");
}

var host = builder.Build();
host.Run();

