using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using tremorur.Models;

namespace tremorur.Converters
{
    public class SelectedAlarmComparerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var currentAlarm = value as Alarm;
            var selectedAlarm = parameter as Alarm;

            if (currentAlarm == null || selectedAlarm == null)
                return Colors.Transparent;

            return currentAlarm.Id == selectedAlarm.Id ? Colors.White : Colors.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
