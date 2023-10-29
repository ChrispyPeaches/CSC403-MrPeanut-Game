using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace Fall2020_CSC403_Project.OpenAIApi
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ChatCompletionQuery
    {
        /// <summary>
        /// A list of previous messages in a dialogue
        /// </summary>
        [JsonProperty("messages")]
        public IList<ChatMessage> Messages { get; set; }

        /// <summary>
        /// The OpenAI model to use for chat compltion
        /// </summary>
        [JsonProperty("model")]
        public string Model { get; set; } = "gpt-3.5-turbo";
    }
}
