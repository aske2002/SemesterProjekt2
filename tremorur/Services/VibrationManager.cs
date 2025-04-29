
public class VibrationManager
{
    private readonly BluetoothService bluetoothService;
    public VibrationManager(BluetoothService bluetoothService)
    {
        this.bluetoothService = bluetoothService;
    }
    public async Task AddPatternAsync(string pattern)
    {
        // Implement the logic to add a vibration pattern
        // This is a placeholder implementation
        await Task.Delay(1000);
        Console.WriteLine($"Vibration pattern '{pattern}' added.");
    }
}