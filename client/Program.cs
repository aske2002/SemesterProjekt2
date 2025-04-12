using System.Diagnostics;
using client.Services.Bluetooth;

var bluetoothService = new BluetoothService();
await bluetoothService.InitializeAsync();

Debug.WriteLine("Starting Bluetooth Discovery...");
await bluetoothService.StartDiscoveryAsync();

var isDiscovering = await bluetoothService.IsDiscoveringAsync();
Debug.WriteLine($"Discovering: {isDiscovering}");

await bluetoothService.WatchInterfacesAddedAsync(info =>
{
    Debug.WriteLine($"New interface added: {info.Item2}");
});

await Task.Delay(10000); // Let it discover for 10s

await bluetoothService.StopDiscoveryAsync();
Debug.WriteLine("Stopped Discovery");

await bluetoothService.DisposeAsync();