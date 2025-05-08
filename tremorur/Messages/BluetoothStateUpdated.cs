namespace tremorur.Messages;

public enum BluetoothState {
    Available,
    NotAvailable,
}
public record BluetoothStateUpdated(BluetoothState State);
