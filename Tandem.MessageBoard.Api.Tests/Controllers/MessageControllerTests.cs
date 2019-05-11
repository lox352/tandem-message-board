using System;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using FluentAssertions;

namespace Tandem.MessageBoard.Api.Tests
{
    public class MessageControllerTests : IClassFixture<WebApplicationFactory<Tandem.MessageBoard.Api.Startup>>
    {
        private readonly WebApplicationFactory<Tandem.MessageBoard.Api.Startup> _factory;
        public MessageControllerTests(WebApplicationFactory<Tandem.MessageBoard.Api.Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Test1()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/message");
            var result = await response.Content.ReadAsStringAsync();

            // Assert
            result.Should().Contain("value");
        }
    }
}
