using Xunit;
using Moq;
using System.Threading.Tasks;
using ExternalUserServiceLibrary.Services;
using ExternalUserServiceLibrary.Clients;
using ExternalUserServiceLibrary.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace ExternalUserServiceLibrary.Tests.Services
{
    public class ExternalUserServiceTests
    {
        private readonly Mock<IReqResClient> _mockClient;
        private readonly IMemoryCache _memoryCache;
        private readonly IOptions<Configuration.ApiSettings> _mockOptions;
        private readonly ExternalUserService _service;

        public ExternalUserServiceTests()
        {
            _mockClient = new Mock<IReqResClient>();

            _memoryCache = new MemoryCache(new MemoryCacheOptions());

            _mockOptions = Options.Create(new Configuration.ApiSettings
            {
                CacheDurationSeconds = 10
            });

            _service = new ExternalUserService(_mockClient.Object, _memoryCache, _mockOptions);
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsUser_WhenUserExists()
        {
            // Arrange
            var userId = 2;
            var expectedUser = new User
            {
                Id = userId,
                First_Name = "Janet",
                Last_Name = "Weaver",
                Email = "janet.weaver@reqres.in"
            };

            _mockClient.Setup(c => c.GetUserByIdAsync(userId))
                       .ReturnsAsync(expectedUser);

            // Act
            var result = await _service.GetUserByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal("Janet", result.First_Name);
        }

        [Fact]
        public async Task GetUserByIdAsync_UsesCache_OnSecondCall()
        {
            // Arrange
            var userId = 5;
            var expectedUser = new User
            {
                Id = userId,
                First_Name = "Emma",
                Last_Name = "Wong",
                Email = "emma.wong@reqres.in"
            };

            _mockClient.Setup(c => c.GetUserByIdAsync(userId))
                       .ReturnsAsync(expectedUser);

            // Act
            var firstCall = await _service.GetUserByIdAsync(userId);
            var secondCall = await _service.GetUserByIdAsync(userId); // should hit cache

            // Assert
            Assert.Equal(expectedUser.Email, secondCall.Email);
            _mockClient.Verify(c => c.GetUserByIdAsync(userId), Times.Once); // Only one external call
        }
    }
}
