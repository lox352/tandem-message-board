using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Collections.Generic;
using System;

namespace Tandem.MessageBoard.Api.Tests
{
    public class MessagesControllerTests : IClassFixture<WebApplicationFactory<Tandem.MessageBoard.Api.Startup>>
    {
        private readonly WebApplicationFactory<Tandem.MessageBoard.Api.Startup> _factory;
        public MessagesControllerTests(WebApplicationFactory<Tandem.MessageBoard.Api.Startup> factory)
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

            // Act
            var response = await client.PostAsync("/messages", stringContent);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            responseContent.Should().Contain("userId");
            responseContent.Should().Contain("message");
           

            string GeneratePostBody(string userId, string message) {
                var objectToPost = new Dictionary<string, string>();
                objectToPost.Add(userId, UserIdPropertyValue);
                objectToPost.Add(message, MessagePropertyValue);
                var serialisedObject = JsonConvert.SerializeObject(objectToPost);
                return serialisedObject;
            }
        }

        [Fact]
        public async Task Post_ResponseShouldBeSuccessWithCorrectMediaTypeAndCharacterEncoding()
        {
            // Arrange
            var serialisedObjectToPost = GenerateSerialisedMessageJson("some user", "some message");
            var stringContent = new StringContent(serialisedObjectToPost, Encoding.UTF8, "application/json");

            var client = _factory.CreateClient();

            // Act
            var response = await client.PostAsync("/messages", stringContent);

            // Assert
            response.IsSuccessStatusCode.Should().BeTrue();
            response.Content.Headers.ContentType.MediaType.Should().Be("application/json");
            response.Content.Headers.ContentType.CharSet.Should().Be("utf-8");
        }

        [Theory]
        [InlineData("simon", "G'day darkness")]
        [InlineData("garfunkel", "You're a great friend indeed")]
        public async Task Post_ResponseBodyShouldDeserialiseWithExpectedProperties(string userId, string message)
        {
            // Arrange
            var serialisedObjectToPost = GenerateSerialisedMessageJson(userId, message);
            var stringContent = new StringContent(serialisedObjectToPost, Encoding.UTF8, "application/json");

            var client = _factory.CreateClient();

            // Act
            var response = await client.PostAsync("/messages", stringContent);
            var responseContent = await response.Content.ReadAsStringAsync();
            var deserialisedResponseContent = JsonConvert.DeserializeObject<dynamic>(responseContent);

            // Assert
            deserialisedResponseContent.Message.Should().Be(message);

            deserialisedResponseContent.UserId.Should().Be(userId);

            deserialisedResponseContent.CreateDate.Should().Not().BeNull();
            deserialisedResponseContent.CreateDate.Should().Not().Be(default(DateTimeOffset).ToString());

            deserialisedResponseContent.MessageId.Should().Not().BeNull();
            deserialisedResponseContent.MessageId.Should().Not().Be("");
            deserialisedResponseContent.CreateDate.Should().Not().Be(default(Guid).ToString());
        }

        private string GenerateSerialisedMessageJson(string userId, string messageContent)
        {
            var objectToPost = new Dictionary<string, string>
            {
                { "userId", userId },
                { "message", messageContent }
            };
            var serialisedObject = JsonConvert.SerializeObject(objectToPost);
            return serialisedObject;
        }
    }
}
