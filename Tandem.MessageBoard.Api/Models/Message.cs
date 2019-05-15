using System;
using Newtonsoft.Json;

namespace Tandem.MessageBoard.Api.Models
{
    public class Message
    {
        public Guid MessageId { get; set; }

        public string UserId { get; set; }

        [JsonProperty(PropertyName = "Message")]
        public string MessageContent { get; set; }

        public DateTimeOffset CreatedDate { get; set; }
    }
}
