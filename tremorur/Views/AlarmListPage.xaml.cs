using tremorur.Models;
using tremorur.ViewModels;

namespace tremorur.Views
{
    public partial class AlarmListPage : ContentPage

    {
        private readonly AlarmListViewModel _viewModel;
        private ListView _alarmListView;

        public AlarmListPage(AlarmListViewModel viewModel)

        {
            _viewModel = viewModel;

            Title = "Medicin Alarmer";

            _alarmListView = new ListView
            {
                ItemsSource = _viewModel.Alarms,
                ItemTemplate = new DataTemplate(() =>
                {
                    var textCell = new TextCell();
                    textCell.SetBinding(TextCell.TextProperty, nameof(Alarm.TimeSpan));
                    return textCell;
                }),
                SelectionMode = ListViewSelectionMode.Single
            };

            _alarmListView.ItemSelected += (s, e) =>
            {
                _viewModel.SelectedAlarm = e.SelectedItem as Alarm;
            };

            Content = _alarmListView;
            var cancelButton = new Button
            {
                Text = "Cancel Alarm",
                BackgroundColor = Colors.Red,
                TextColor = Colors.White
            };

            bool isPressed = false;
            DateTime pressedTime = DateTime.Now;

            cancelButton.Pressed += (s, e) =>
            {
                isPressed = true;
                pressedTime = DateTime.Now;
            };

            cancelButton.Released += (s, e) =>
            {
                if (isPressed)
                {
                    isPressed = false;
                    var duration = (DateTime.Now - pressedTime).TotalMilliseconds;
                    if (duration >= 4000) // 4 seconds
                    {
                        _viewModel.DeleteSelectedAlarm();
                    }
                }
            };

            Content = new StackLayout
            {
                Padding = new Thickness(20),
                Children =
    {
        _alarmListView,
        cancelButton
    }
            };


        }


    }
    }


