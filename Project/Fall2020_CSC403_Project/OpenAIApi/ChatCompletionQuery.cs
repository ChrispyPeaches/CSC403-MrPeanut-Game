using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Fall2020_CSC403_Project.OpenAIApi
{
    public class ChatCompletionQuery
    {
        [JsonProperty("messages")]
        public IList<ChatMessage> Messages { get; set; }

        public string Model { get; set; } = "gpt-3.5-turbo";

        public class ChatMessage
        {
            [JsonProperty(PropertyName = "role")]
            [JsonConverter(typeof(StringEnumConverter))]
            public RoleType Role { get; set; }

            [JsonProperty(PropertyName = "content")]
            public string Content { get; set; }
        }
        public enum RoleType
        {
            System,
            User,
            Assistant
        }
    }

    


}
