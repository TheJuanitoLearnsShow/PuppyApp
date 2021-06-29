using NJsonSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuppySqlWrapper
{
    public class SqlInputAsJsonProcessor
    {
        public async Task ValidateJson(string spname, string jsonPayload, string spOpenApiSchema)
        {
            var schema = await JsonSchema.FromJsonAsync(spOpenApiSchema);
            var errors = schema.Validate(jsonPayload);
            foreach (var error in errors)
                Console.WriteLine(error.Path + ": " + error.Kind);

        }
    }
}
