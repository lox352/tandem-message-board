using System;
using System.Collections.Generic;
using Tandem.MessageBoard.Api.Models;
using Tandem.MessageBoard.Api.Repositories;

namespace Tandem.MessageBoard.Api.Services
{
    public interface IMessagesService
    {
        Message AddMessage(Message message);
        Messages GetMessagesByUserId(string userId);
    }

    public class MessagesService : IMessagesService
    {
        private readonly IMessagesRepository _messagesRepository;

        public MessagesService(IMessagesRepository messagesRepository)
        {
            _messagesRepository = messagesRepository;
        }

        public Message AddMessage(Message message)
        {
            message.MessageId = Guid.NewGuid();
            message.CreatedDate = DateTimeOffset.Now;

            _messagesRepository.SaveMessage(message);

            return message;
        }

        public Messages GetMessagesByUserId(string userId)
        {
            var messages = _messagesRepository.GetMessagesByUserId(userId);
            foreach (var message in messages)
            {
                message.UserId = userId;
            }
            return new Messages { MessagesList = messages };
        }
    }
}