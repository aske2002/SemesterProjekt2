using Microsoft.Maui.Controls;
using tremorur.ViewModels;
using tremorur.Models.Bluetooth;
using System.Diagnostics;

namespace tremorur.Views
{
    public partial class BluetoothConnectPage : ContentPage
    {
        private BluetoothConnectViewModel _viewModel;
        public BluetoothConnectPage(BluetoothConnectViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = viewModel;
        }
    }
}
