using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Tandem.MessageBoard.Api.Models;

namespace Tandem.MessageBoard.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        // GET /messages
        [HttpGet]
        public ActionResult<IEnumerable<Message>> Get([FromQuery] string userId)
        {
            throw new NotImplementedException();
        }

        // POST /messages
        [HttpPost]
        public void Post([FromBody] Message message)
        {
            throw new NotImplementedException();
        }

        // PUT /messages/{messageId}
        [HttpPut("{messageId}")]
        public void Put(int messageId, [FromBody] Message message) => throw new NotImplementedException();

        // DELETE /messages/{messageId}
        [HttpDelete("{messageId}")]
        public void Delete(Guid messageId) => throw new NotImplementedException();
    }
}
