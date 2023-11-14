using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
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

        /// <summary>
        /// The functions available
        /// </summary>
        [JsonProperty(PropertyName = "tools")]
        public Tool Tools { get; set; }

        public class Tool
        {
            // Will always be "function"
            [JsonProperty("type")]
            public string Type { get; set; }


            [JsonProperty("function")]
            public FunctionModel Function { get; set; }

            public class FunctionModel
            {
                // Name of functions to call
                [JsonProperty("name")]
                public string Name { get; set; }

                // When the AI should call this function
                [JsonProperty("description")]
                public string Description { get; set; }

                // Parameters the function accepts
                [JsonProperty("parameters")]
                public ParameterModel Parameter { get; set; }

                public class ParameterModel
                {
                    [JsonProperty("type")]
                    public string Type { get; set; }

                    [JsonProperty("properties")]
                    public Array Properties { get; set; }
                }
            }
        }
    }
}
