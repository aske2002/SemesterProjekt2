using Microsoft.Maui.Controls;
using tremorur.ViewModels;
using tremorur.Models.Bluetooth;
using System.Diagnostics;

namespace tremorur.Views
{
    public partial class BluetoothTestPage : ContentPage
    {
        private BluetoothTestViewModel _viewModel;
        public BluetoothTestPage(BluetoothTestViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = viewModel;
        }

        public async void NotifyCharacteristic(object? sender, ToggledEventArgs e)
        {
            if (sender is Microsoft.Maui.Controls.Switch toggleSwitch && toggleSwitch.BindingContext is BluetoothPeripheralCharacteristic characteristic)
            {
                await _viewModel.ToggleNotify(characteristic, e.Value);
            }
        }
    }
}
