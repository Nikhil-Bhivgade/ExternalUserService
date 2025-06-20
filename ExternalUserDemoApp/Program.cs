using ExternalUserServiceLibrary.Clients;
using ExternalUserServiceLibrary.Configuration;
using ExternalUserServiceLibrary.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using System.Net.Http;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        services.Configure<ApiSettings>(context.Configuration.GetSection("ApiSettings"));

        services.AddMemoryCache();

        services.AddHttpClient<IReqResClient, ReqResClient>((sp, client) =>
        {
            var settings = sp.GetRequiredService<IOptions<ApiSettings>>().Value;

            client.BaseAddress = new Uri(settings.BaseUrl);
            client.DefaultRequestHeaders.Add("x-api-key", settings.ApiKey);

        }).AddPolicyHandler(GetRetryPolicy());


        services.AddScoped<IExternalUserService, ExternalUserService>();
    })
    .Build();

var userService = host.Services.GetRequiredService<IExternalUserService>();

Console.WriteLine("=== ReqRes User Viewer ===");
while (true)
{
    Console.WriteLine("\nOptions:");
    Console.WriteLine("1. Get user by ID");
    Console.WriteLine("2. Get all users from page");
    Console.WriteLine("0. Exit");
    Console.Write("Enter your choice: ");
    var choice = Console.ReadLine();

    if (choice == "1")
    {
        Console.Write("Enter user ID: ");
        if (int.TryParse(Console.ReadLine(), out int userId))
        {
            try
            {
                var user = await userService.GetUserByIdAsync(userId);
                if (user != null)
                {
                    Console.WriteLine($"{user.Id} - {user.First_Name} {user.Last_Name} - {user.Email}");
                }
                else
                {
                    Console.WriteLine("User not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
    else if (choice == "2")
    {
        Console.Write("Enter page number: ");
        if (int.TryParse(Console.ReadLine(), out int pageId))
        {
            try
            {
                var users = await userService.GetAllUsersAsync(pageId);
                foreach (var u in users)
                {
                    Console.WriteLine($"{u.Id} - {u.First_Name} {u.Last_Name}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
    else if (choice == "0")
    {
        break;
    }
    else
    {
        Console.WriteLine("Invalid choice.");
    }
}

// Retry policy with exponential backoff
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(3, retryAttempt =>
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
        );
}
