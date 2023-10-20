using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace Fall2020_CSC403_Project.OpenAIApi
{
    public class ChatCompletionQuery
    {
        [JsonProperty("messages")]
        public IList<ChatMessage> Messages { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; } = "gpt-3.5-turbo";

        public class ChatMessage
        {
            [JsonProperty(PropertyName = "role")]
            [JsonConverter(typeof(StringEnumConverter),
                converterParameters: typeof(CamelCaseNamingStrategy))]
            public RoleType Role { get; set; }

            [JsonProperty(PropertyName = "content")]
            public string Content { get; set; }
        }

        public enum RoleType
        {
            System,
            User,
            Assistant,
            Function
        }
    }
}
