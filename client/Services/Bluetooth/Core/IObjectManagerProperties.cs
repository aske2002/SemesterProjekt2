using System.Collections.Generic;

namespace client.Services.Bluetooth.Core
{
    internal interface IObjectManagerProperties
    {
        IDictionary<string, IDictionary<string, object>> GetProperties();
    }
}