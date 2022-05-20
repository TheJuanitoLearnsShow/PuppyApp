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
            if (element != null && item != null)
            {
                var itemTypeName = item.GetType().FullName;
                if (itemTypeName != null &&
                    !itemTypeName.StartsWith("Elmish.WPF.ViewModel`2[[System.Tuple`2[[SpRequestMvu+Model, Puppy.SqlViewModels"))
                    return (DataTemplate)element.FindResource($"{typeof(System.String).Name}.Template");
                dynamic spParam = item;
                if (spParam.HasLookup)
                    return (DataTemplate)element.TryFindResource("Lookup.Template");
                var template = (DataTemplate)element.TryFindResource($"{spParam.NetNature}.Template");
                return template ?? (DataTemplate)element.FindResource($"{typeof(System.String).Name}.Template");
                //    
            }
            return (DataTemplate)element.FindResource($"{typeof(System.String).Name}.Template");
        }
    }
}
