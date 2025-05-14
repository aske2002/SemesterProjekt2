﻿using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace Converters
{
    public class SelectedAlarmComparerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Equals(value, parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
