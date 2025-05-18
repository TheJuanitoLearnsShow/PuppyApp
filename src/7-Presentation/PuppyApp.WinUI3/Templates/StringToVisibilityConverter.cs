

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using System.Globalization;

namespace PuppyApp.WinUI3.Templates;

public class StringToVisibilityConverter : IValueConverter
{

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value;
    }


    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return string.IsNullOrEmpty(value as string) ? Visibility.Collapsed : Visibility.Visible;
    }
}