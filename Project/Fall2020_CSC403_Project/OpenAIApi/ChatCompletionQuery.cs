﻿using Newtonsoft.Json;
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

        [JsonProperty("tool_choice")]
        public string ToolChoice { get; set; } = "auto";

        /// <summary>
        /// The functions available
        /// </summary>
        [JsonProperty(PropertyName = "tools")]
        public List<Tool> Tools { get; set; }

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
                    // Always object
                    [JsonProperty("type")]
                    public string Type { get; set; }

                    [JsonProperty("properties")]
                    public Property Properties { get; set; }

                    // Remain empty for our purposes
                    public class Property
                    {
                        // Name of property
                        [JsonProperty("response")]
                        public PropertyStuff Response { get; set; }

                        public class PropertyStuff
                        {
                            // Describes parameter type
                            [JsonProperty("type")]
                            public string Type { get; set; }

                            // Describes what AI should fill in this parameter with
                            [JsonProperty("description")]
                            public string Description { get; set; }
                        }
                    }

                    // Defines what properties are required
                    [JsonProperty("required")]
                    public List<string> Required { get; set; }
                }
            }
        }
    }
}
