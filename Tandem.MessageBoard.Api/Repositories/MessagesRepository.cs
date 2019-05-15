using System;
using System.Collections.Generic;
using Tandem.MessageBoard.Api.Models;

namespace Tandem.MessageBoard.Api.Repositories
{
    public interface IMessagesRepository
    {
        void SaveMessage(Message message);
        List<Message> GetMessagesByUserId(string userId);
    }

    public class MessageRepository : IMessagesRepository
    {
        private Dictionary<string, List<Message>> _messagesByUserId = new Dictionary<string, List<Message>>();

        public void SaveMessage(Message message)
        {
            AddMessageByUserId(message);
        }

        public List<Message> GetMessagesByUserId(string userId)
        {
            return RetrieveMessagesByUserId(userId);
        }

        private List<Message> RetrieveMessagesByUserId(string queryUserId)
        {
            var userId = queryUserId.ToLowerInvariant();
            if (_messagesByUserId.ContainsKey(userId))
            {
                var messages = _messagesByUserId[userId];
                return messages;
            }
            else
            {
                return new List<Message>();
            }
        }

        private void AddMessageByUserId(Message message)
        {
            var userId = message.UserId.ToLowerInvariant();
            if (_messagesByUserId.ContainsKey(userId))
            {
                var messages = _messagesByUserId[userId];
                messages.Add(message);
            } 
            else
            {
                var messages = new List<Message>() { message };
                _messagesByUserId.Add(userId, messages);
            }
        }
    }
}