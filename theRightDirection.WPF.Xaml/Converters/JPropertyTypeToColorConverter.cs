﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Newtonsoft.Json.Linq;

namespace theRightDirection.WPF.Xaml.Converters
{
    public sealed class JPropertyTypeToColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var jprop = values[0] as JProperty;
            if (jprop != null)
            {
                if(values.Length != 5)
                {
                    return Brushes.Black;
                }
                switch (jprop.Value.Type)
                {
                    case JTokenType.String:
                        return Library.BrushHelper.HexCodeToSolidColorBrush(values[1].ToString());
                    case JTokenType.Integer:
                        return Library.BrushHelper.HexCodeToSolidColorBrush(values[2].ToString());
                    case JTokenType.Boolean:
                        return Library.BrushHelper.HexCodeToSolidColorBrush(values[3].ToString());
                    case JTokenType.Null:
                        return Library.BrushHelper.HexCodeToSolidColorBrush(values[4].ToString());
                }
            }
            return Brushes.Black;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
