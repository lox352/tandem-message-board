using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Collections.Generic;

namespace Tandem.MessageBoard.Api.Tests
{
    public class MessageControllerTests : IClassFixture<WebApplicationFactory<Tandem.MessageBoard.Api.Startup>>
    {
        private readonly WebApplicationFactory<Tandem.MessageBoard.Api.Startup> _factory;
        public MessageControllerTests(WebApplicationFactory<Tandem.MessageBoard.Api.Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("userId", "message")]
        [InlineData("userid", "Message")]
        [InlineData("Userid", "MESSAGE")]
        [InlineData("UserId", "mEsSaGe")]
        [InlineData("USERID", "MeSsAgE")]
        [InlineData("uSeRiD", "mesSAGE")]
        public async Task Post_BodyPropertiesShouldBeCaseInsensitive(string userIdPropertyName, string messagePropertyName)
        {
            // Arrange
            const string UserIdPropertyValue = "simon";
            const string MessagePropertyValue = "Hello darkness, my old friend, I've come to talk with you again";

            var serialisedObjectToPost = GeneratePostBody(userIdPropertyName, messagePropertyName);
            var stringContent = new StringContent(serialisedObjectToPost, Encoding.UTF8, "application/json");

            var client = _factory.CreateClient();
            await client.PostAsync("/messages", stringContent);

            // Act
            var response = await client.GetAsync("/messages?userId=simon");
            var result = await response.Content.ReadAsStringAsync();

            // Assert
            result.Should().Contain("userId");
            result.Should().Contain("message");


            string GeneratePostBody(string userId, string message) {
                var objectToPost = new Dictionary<string, string>();
                objectToPost.Add(userId, UserIdPropertyValue);
                objectToPost.Add(message, MessagePropertyValue);
                var serialisedObject = JsonConvert.SerializeObject(objectToPost);
                return serialisedObject;
            }
        }
    }
}
