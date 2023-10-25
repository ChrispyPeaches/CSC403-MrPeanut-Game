using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace Fall2020_CSC403_Project.OpenAIApi
{
    public class ChatCompletionResponse
    {
        [JsonProperty("choices")]
        public  List<Choice> Choices { get; set; }

        public class Choice
        {
            [JsonProperty("message")]
            public ChatCompletionQuery.ChatMessage Message { get; set; }
        }
    }
}
