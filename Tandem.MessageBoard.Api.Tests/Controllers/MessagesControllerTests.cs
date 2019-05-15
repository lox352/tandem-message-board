using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Collections.Generic;
using System;
using Newtonsoft.Json.Linq;

namespace Tandem.MessageBoard.Api.Tests
{
    public class MessagesControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        public MessagesControllerTests(WebApplicationFactory<Startup> factory)
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
            var client = _factory.CreateClient();

            const string UserIdPropertyValue = "simon";
            const string MessagePropertyValue = "Hello darkness, my old friend, I've come to talk with you again";

            var serialisedObjectToPost = GeneratePostBody(userIdPropertyName, messagePropertyName);
            var stringContent = new StringContent(serialisedObjectToPost, Encoding.UTF8, "application/json");

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
            var client = _factory.CreateClient();

            var stringContent = GenerateSerialisedMessageStringContent("some user", "some message");

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
            var client = _factory.CreateClient();

            var stringContent = GenerateSerialisedMessageStringContent(userId, message);

            // Act
            var response = await client.PostAsync("/messages", stringContent);
            var responseContent = await response.Content.ReadAsStringAsync();
            JObject deserialisedResponseContent = JObject.Parse(responseContent);

            // Assert
            ((string)deserialisedResponseContent["message"])
                .Should().Be(message);
            ((string)deserialisedResponseContent["userId"])
                .Should().Be(userId);
            ((DateTimeOffset)deserialisedResponseContent["createdDate"])
                .Should().NotBe(default(DateTimeOffset));
            ((Guid)deserialisedResponseContent["messageId"])
                .Should().NotBe(default(Guid));
        }

        [Theory]
        [InlineData("simon", "SIMON")]
        [InlineData("ßimon", "Ssimon")]
        [InlineData("GARFUNKEL", "garfunkel")]
        public async Task Get_UserIdQueryParameterShouldBeCaseInsensitive(string savedUserId, string queryUserId)
        {
            // Arrange
            var client = _factory.CreateClient();

            const string Message = "This is the sound of silence";

            var stringContent = GenerateSerialisedMessageStringContent(savedUserId, Message);

            await client.PostAsync("/messages", stringContent);

            // Act
            var response = await client.GetAsync($"messages?userId={queryUserId}");
            var responseContent = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(responseContent);

            // Assert
            var messages = (JArray)result["messages"];
            messages.Count.Should().Be(1);
            ((string)((JObject)messages[0])["message"]).Should().Be(Message);
            ((string)((JObject)messages[0])["userId"]).Should().Be(queryUserId);
        }

        [Fact]
        public async Task Get_UserIdWithSingleMessage()
        {
            // Arrange
            var client = _factory.CreateClient();

            var stringContent = GenerateSerialisedMessageStringContent("simon", "Just a bridge...");

            await client.PostAsync("/messages", stringContent);

            // Act
            var response = await client.GetAsync($"messages?userId={queryUserId}");
            var responseContent = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(responseContent);

            // Assert
            var messages = (JArray)result["messages"];
            messages.Count.Should().Be(1);

            var message = (JObject)messages[0];
            ((string)(message)["message"]).Should().Be("Just a bridge...");
            ((string)(message)["userId"]).Should().Be("simon");
            ((Guid)(message)["messageId"]).Should().NotBe(default(Guid));

            const string Iso8601Regex = "^([\\+-]?\\d{4}(?!\\d{2}\\b))((-?)((0[1-9]|1[0-2])(\\3([12]\\d|0[1-9]|3[01]))?|W([0-4]\\d|5[0-2])(-?[1-7])?|(00[1-9]|0[1-9]\\d|[12]\\d{2}|3([0-5]\\d|6[1-6])))([T\\s]((([01]\\d|2[0-3])((:?)[0-5]\\d)?|24\\:?00)([\\.,]\\d+(?!:))?)?(\\17[0-5]\\d([\\.,]\\d+)?)?([zZ]|([\\+-])([01]\\d|2[0-3]):?([0-5]\\d)?)?)?)?$\n\n";
            ((string)(message)["createdDate"]).Should().MatchRegex(Iso8601Regex);
            ((Guid)(message)["createdDate"]).Should().NotBe(default(DateTimeOffset));
        }

        [Fact]
        public async Task Get_UserIdWithMultipleMessages()
        {
            // Arrange
            var client = _factory.CreateClient();

            const string UserId = "simon";

            var firstStringContent = GenerateSerialisedMessageStringContent(UserId, "Here's to you");
            var secondStringContent = GenerateSerialisedMessageStringContent(UserId, "Mrs. Robinson");

            await client.PostAsync("/messages", firstStringContent);
            await client.PostAsync("/messages", secondStringContent);

            // Act
            var response = await client.GetAsync($"messages?userId={UserId}");
            var responseContent = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(responseContent);

            // Assert
            var messages = (JArray)result["messages"];
            messages.Count.Should().Be(2);
            ((string)((JObject)messages[0])["message"]).Should().Be("Here's to you");
            ((string)((JObject)messages[1])["message"]).Should().Be("Mrs. Robinson");
        }

        private StringContent GenerateSerialisedMessageStringContent(string userId, string messageContent)
        {
            var objectToPost = new Dictionary<string, string>
            {
                { "userId", userId },
                { "message", messageContent }
            };
            var serialisedObject = JsonConvert.SerializeObject(objectToPost);
            return new StringContent(serialisedObject, Encoding.UTF8, "application/json"); ;
        }
    }
}
