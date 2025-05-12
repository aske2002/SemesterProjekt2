using tremorur.Models.Bluetooth;

namespace tremorur.Messages;

public record DeviceConnected(IBluetoothPeripheral Peripheral);
public record DeviceDisconnected();