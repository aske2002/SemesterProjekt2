using System;
using System.Threading.Tasks;
using client.Services.Bluetooth.Gatt.BlueZModel;
using Tmds.DBus;

namespace client.Services.Bluetooth.Core
{
    public class ServerContext : IDisposable
    {
        public readonly PropertyListener Listener = new PropertyListener();
        public ServerContext()
        {
            Connection = new Connection(Address.System);
        }

        public async Task Connect()
        {
            await Connection.ConnectAsync();
            await Listener.InitializeAsync(Connection);
        }

        public Connection Connection { get; }

        public void Dispose()
        {
            Connection.Dispose();
        }
    }
}