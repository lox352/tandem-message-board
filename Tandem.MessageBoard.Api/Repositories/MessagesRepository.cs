using System;
using Tandem.MessageBoard.Api.Models;

namespace Tandem.MessageBoard.Api.Repositories
{
    public interface IMessagesRepository
    {
        void SaveMessage(Message message);
    }

    public class MessageRepository : IMessagesRepository
    {
        public void SaveMessage(Message message)
        {
            throw new NotImplementedException();
        }
    }
}