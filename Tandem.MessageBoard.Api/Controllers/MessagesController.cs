﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Tandem.MessageBoard.Api.Models;
using Tandem.MessageBoard.Api.Services;

namespace Tandem.MessageBoard.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessagesService _messagesService;

        public MessagesController(IMessagesService messagesService)
        {
            _messagesService = messagesService;
        }

        // GET /messages
        [HttpGet]
        public ActionResult<IEnumerable<Message>> Get([FromQuery] string userId)
        {
            throw new NotImplementedException();
        }

        // POST /messages
        [HttpPost]
        public ActionResult<Message> Post([FromBody] Message message)
        {
            var response = _messagesService.AddMessage(message);
            return response;
        }

        // PUT /messages/{messageId}
        [HttpPut("{messageId}")]
        public void Put(int messageId, [FromBody] Message message) => throw new NotImplementedException();

        // DELETE /messages/{messageId}
        [HttpDelete("{messageId}")]
        public void Delete(Guid messageId) => throw new NotImplementedException();
    }
}
