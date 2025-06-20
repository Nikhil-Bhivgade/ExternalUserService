using ExternalUserServiceLibrary.Clients;
using ExternalUserServiceLibrary.Configuration;
using ExternalUserServiceLibrary.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExternalUserServiceLibrary.Services
{
    public class ExternalUserService : IExternalUserService
    {
        private readonly IReqResClient _reqResClient;
        private readonly IMemoryCache _cache;
        private readonly int _cacheDurationSeconds;
        public ExternalUserService(IReqResClient reqResClient, IMemoryCache cache, IOptions<ApiSettings> settings)
        {
            _reqResClient = reqResClient;
            _cache = cache;
            _cacheDurationSeconds = settings.Value.CacheDurationSeconds;
        }
        public async Task<User> GetUserByIdAsync(int userId)
        {

            string cacheKey = $"user_{userId}";

            if (_cache.TryGetValue<User>(cacheKey, out var cachedUser))
            {
                return cachedUser;
            }

            var user = await _reqResClient.GetUserByIdAsync(userId);

            if (user != null)
            {
                _cache.Set(cacheKey, user, TimeSpan.FromSeconds(_cacheDurationSeconds));
            }

            return user;
        }
        public async Task<IEnumerable<User>> GetAllUsersAsync(int pageId)
        {
            string cacheKey = $"users_page_{pageId}";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<User> cachedUsers))
            {
                return cachedUsers;
            }

            var response = await _reqResClient.GetAllUsersAsync(pageId);

            if (response?.Data != null)
            {
                _cache.Set(cacheKey, response.Data, TimeSpan.FromSeconds(_cacheDurationSeconds));
                return response.Data;
            }

            return Enumerable.Empty<User>();
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            const string cacheKey = "users_all_pages";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<User> cachedAllUsers))
                return cachedAllUsers;

            var allUsers = new List<User>();
            int currentPage = 1;
            UserResponse response;

            do
            {
                response = await _reqResClient.GetAllUsersAsync(currentPage);
                if (response?.Data != null)
                    allUsers.AddRange(response.Data);
                currentPage++;
            }
            while (response != null && currentPage <= response.Total_Pages);

            if (allUsers != null)
            {
                _cache.Set(cacheKey, allUsers , TimeSpan.FromSeconds(_cacheDurationSeconds));
                return response.Data;
            }

            return allUsers;
        }

    }
}
