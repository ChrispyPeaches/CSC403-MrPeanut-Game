using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Fall2020_CSC403_Project.OpenAIApi
{
    public class ChatMessage
    {
        [JsonProperty(PropertyName = "role")]
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
