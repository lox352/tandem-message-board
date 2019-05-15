using System;
using Tandem.MessageBoard.Api.Models;

namespace Tandem.MessageBoard.Api.Services
{
    public interface IMessagesService
    {
        Message AddMessage(Message message);
    }

    public class MessagesService : IMessagesService
    {
        public Message AddMessage(Message message)
        {
            throw new NotImplementedException();
        }
    }
}