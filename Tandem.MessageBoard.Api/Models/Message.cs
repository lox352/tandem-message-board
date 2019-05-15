using System;
namespace Tandem.MessageBoard.Api.Models
{
    public class Message
    {
        public Guid MessageId { get; set; }
        public string UserId { get; set; }
        public string Message { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }
}
