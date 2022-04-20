﻿using Puppy.SqlViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PuppyApp.Wpf.Templates
{
    internal class ParameterControlSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item,
         DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;
            if (element != null && item != null && item is SpParameterViewModel)
            {
                var spParam = (SpParameterViewModel)item;
                var template = (DataTemplate)element.TryFindResource($"{spParam.NetNature}.Template");
                return template ?? (DataTemplate)element.FindResource($"{typeof(System.String).Name}.Template");
            }
            return null;
        }
    }
}
