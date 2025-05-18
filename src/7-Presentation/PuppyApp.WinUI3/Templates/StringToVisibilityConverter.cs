using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PuppyApp.WinUI3.Templates;

public class StringToVisibilityConverter : IValueConverter
{

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return string.IsNullOrEmpty(value as string) ? Visibility.Collapsed : Visibility.Visible;
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }
}