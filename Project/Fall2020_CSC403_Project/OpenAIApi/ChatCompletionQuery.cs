using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace Fall2020_CSC403_Project.OpenAIApi
{
    public partial class ChatCompletionQuery
    {
        [JsonProperty("messages")]
        public IList<ChatMessage> Messages { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; } = "gpt-3.5-turbo";

        public enum RoleType
        {
            System,
            User,
            Assistant,
            Function
        }
    }
}
