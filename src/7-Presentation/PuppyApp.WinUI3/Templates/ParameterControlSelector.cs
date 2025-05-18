using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Puppy.Types;
using PuppyApp.ReactiveViewModels;

namespace PuppyApp.WinUI3.Templates
{
    public class ParameterControlSelector : DataTemplateSelector
    {
        public DataTemplate StringParameterTemplate { get; set; }
        public DataTemplate NumberParameterTemplate { get; set; }
        public DataTemplate BooleanParameterTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is CallParameterViewModel viewModel)
            {
                return viewModel.PropertyDescriptor switch
                {
                    IntPropertyDescriptor => NumberParameterTemplate,
                    _ => StringParameterTemplate // fallback to string template
                };
            }
            return StringParameterTemplate;
        }
    }
}
