using System;
using System.Collections.Generic;
using Tandem.MessageBoard.Api.Models;
using Tandem.MessageBoard.Api.Repositories;

namespace Tandem.MessageBoard.Api.Services
{
    public interface IMessagesService
    {
        Message AddMessage(Message message);
        List<Message> GetMessagesByUserId(string userId);
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

        public List<Message> GetMessagesByUserId(string userId)
        {
            throw new NotImplementedException();
        }
    }
}