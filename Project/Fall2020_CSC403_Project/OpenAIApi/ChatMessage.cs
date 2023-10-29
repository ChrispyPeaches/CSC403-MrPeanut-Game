using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

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
