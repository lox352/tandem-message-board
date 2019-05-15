using System.Collections.Generic;
using Newtonsoft.Json;

namespace Tandem.MessageBoard.Api.Models
{
    public class Messages
    {
        [JsonProperty(PropertyName = "messages")]
        public List<Message> MessagesList { get; set; }
    }
}
