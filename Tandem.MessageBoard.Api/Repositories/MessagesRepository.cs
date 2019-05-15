using System;
using System.Collections.Generic;
using Tandem.MessageBoard.Api.Models;

namespace Tandem.MessageBoard.Api.Repositories
{
    public interface IMessagesRepository
    {
        void SaveMessage(Message message);
    }

    public class MessageRepository : IMessagesRepository
    {
        private Dictionary<string, Dictionary<Guid, Message>> _messagesByUserId = new Dictionary<string, Dictionary<Guid, Message>>();

        public void SaveMessage(Message message)
        {
            AddMessageByUserId(message);
        }

        private void AddMessageByUserId(Message message)
        {
            if (_messagesByUserId.ContainsKey(message.UserId))
            {
                var messagesById = _messagesByUserId[message.UserId];
                messagesById[message.MessageId] = message;
            } 
            else
            {
                _messagesByUserId.Add(
                    message.UserId, 
                    new Dictionary<Guid, Message>() { { message.MessageId, message } }
                    );
            }
        }
    }
}