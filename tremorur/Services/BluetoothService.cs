namespace tremorur.Services;

public partial class BluetoothService
{
    private bool _shouldScan = false;
    public void Start()
    {
        StartScan();
    }
    public bool IsScanning => _isScanning;

    private partial bool _isScanning { get; } // Defined per platform

    partial void StartScan(); // Defined per platform
}