﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows;

namespace theRightDirection.WPF.Xaml.Converters
{
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visibilityState = Visibility.Collapsed;
            if(parameter != null)
            {
                visibilityState = (Visibility)parameter;
            }
            return !(bool)value ? Visibility.Visible : visibilityState;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visibility = (Visibility)value;
            return visibility == Visibility.Visible ? true : false;
        }
    }
}
