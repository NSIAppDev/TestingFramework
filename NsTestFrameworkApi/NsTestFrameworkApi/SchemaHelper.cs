using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Serialization;

namespace NsTestFrameworkApi
{
    public static class SchemaHelper
    {
        public static bool IsSchemaValid(this string response, Type objectType)
        {
            var responseToken = JToken.Parse(response);
            var generatedSchema = new JSchemaGenerator { ContractResolver = new CamelCasePropertyNamesContractResolver() }.Generate(objectType);
            return responseToken.IsValid(generatedSchema, out IList<string> _);
        }
    }
}
