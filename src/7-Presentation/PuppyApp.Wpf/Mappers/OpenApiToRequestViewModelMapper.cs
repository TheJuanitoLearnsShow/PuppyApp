using Humanizer;
using NJsonSchema;
using PuppyApp.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuppyApp.Wpf.Mappers
{
    static internal class OpenApiToRequestViewModelMapper
    {
       // static public async Task<RequestViewModel> MapToRequest(string jsonSchema)
       //{
       //     var schema = await JsonSchema.FromJsonAsync(jsonSchema);
       //     var request = new RequestViewModel();
       //     IEnumerable<CallParameterViewModel> props = schema.Properties.Where(p => !p.Value.IsReadOnly).Select(p => MapProperty(p.Key, p.Value));
       //     request.LoadNewCallParameters(props);
       //     return request;
       //}

       // static private CallParameterViewModel MapProperty(string key, JsonSchemaProperty value)
       // {
       //     var p = new CallParameterViewModel(key.Humanize(), "", value.IsRequired, value.Type);
       //     return p;

       // }
    }
}
