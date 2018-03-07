using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Xpinn.SportsGo.Util.Portable;

namespace Xpinn.SportsGo.Util.Portable
{
    public static class JsonSerializerOptionsConfiguration
    {
        public static JsonSerializerSettings ReturnJsonSerializerSettings()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                ContractResolver = new WritablePropertiesOnlyResolver(),
            };

            return settings;
        }
    }
}
