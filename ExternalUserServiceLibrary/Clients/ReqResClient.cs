using ExternalUserServiceLibrary.Configuration;
using ExternalUserServiceLibrary.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ExternalUserServiceLibrary.Clients
{
    public class ReqResClient : IReqResClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        public ReqResClient(HttpClient httpClient, IOptions<ApiSettings> options)
        {
            _httpClient = httpClient;
            _baseUrl = options.Value.BaseUrl;
        }

        public async Task<UserResponse> GetAllUsersAsync(int page)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/users?page={page}");

                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException($"Failed to fetch users for page {page}");

                var contentStream = await response.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<UserResponse>(contentStream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result;
            }
            catch (HttpRequestException hex)
            {
                throw new ApplicationException($"Network error while contacting the user API: {hex.Message}", hex);
            }
            catch (JsonException jex)
            {
                throw new InvalidOperationException("Failed to parse user data from API.", jex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Unhandled error while fetching page {page}.", ex);
            }
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/users/{userId}");

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new KeyNotFoundException($"User with ID {userId} was not found.");

                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException($"Failed to fetch user with ID {userId}");

                var contentStream = await response.Content.ReadAsStreamAsync();

                using var jsonDoc = await JsonDocument.ParseAsync(contentStream);
                if (jsonDoc.RootElement.TryGetProperty("data", out var userElement))
                {
                    var user = JsonSerializer.Deserialize<User>(userElement.GetRawText(), new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (user is null)
                        throw new InvalidOperationException("Deserialization returned null for user data.");

                    return user;
                }

                return null;
            }
            catch (HttpRequestException hex)
            {
                throw new ApplicationException($"Network error while contacting the user API: {hex.Message}", hex);
            }
            catch (JsonException jex)
            {
                throw new InvalidOperationException("Failed to parse user data from API.", jex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Unhandled error while fetching user {userId}.", ex);
            }
        }
    }
}
