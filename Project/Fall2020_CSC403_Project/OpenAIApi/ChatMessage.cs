using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Fall2020_CSC403_Project.OpenAIApi
{
    public partial class ChatCompletionQuery
    {
        public class ChatMessage
        {
            [JsonProperty(PropertyName = "role")]
            [JsonConverter(typeof(StringEnumConverter),
                converterParameters: typeof(CamelCaseNamingStrategy))]
            public RoleType Role { get; set; }

            [JsonProperty(PropertyName = "content")]
            public string Content { get; set; }
        }
    }
}
