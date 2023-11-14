using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Windows.Forms;

namespace Fall2020_CSC403_Project.OpenAIApi
{
        /// <summary>
        /// A message formatted to the Open AI Api Chat Competion's requirements
        /// </summary>
        public class ChatMessage
        {
            /// <summary>
            /// The message author
            /// </summary>
            [JsonProperty(PropertyName = "role")]
            [JsonConverter(typeof(StringEnumConverter),
                converterParameters: typeof(CamelCaseNamingStrategy))]
            public RoleType Role { get; set; }

            /// <summary>
            /// The message
            /// </summary>
            [JsonProperty(PropertyName = "content")]
            public string Content { get; set; }
        }

        public class ChatMessageAssistant : ChatMessage
        {
            /// <summary>
            /// The function (if AI decides to call one)
            /// </summary>
            [JsonProperty(PropertyName = "tool_choice")]
            public Tool ToolChoice { get; set; }

            public class Tool
            {
                // Will always be "function"
                [JsonProperty("type")]
                public string Type { get; set; }

                // Function model
                [JsonProperty("function")]
                public FunctionModel Function { get; set; }

                public class FunctionModel
                {
                    // Name of function to call
                    [JsonProperty("name")]
                    public string Name { get; set; }
                }
            }
        }

        /// <summary>
        /// The type of author that conducted a message
        /// </summary>
        public enum RoleType
        {
            System,
            User,
            Assistant
        }
}
