# ExternalUserService Demo – .NET 8 Console App

This project demonstrates a .NET 8 Console Application that interacts with the [ReqRes](https://reqres.in) public API to fetch and display user data.

**Features:**
- Clean service-based architecture
- API client using `HttpClientFactory`
- In-memory caching with configurable expiration
- Retry policy using Polly
- Dependency Injection & Options pattern
- Unit tests using xUnit and Moq

---

## 🧾 Prerequisites

- [.NET SDK 8+](https://dotnet.microsoft.com/download/dotnet/8.0)
- Git CLI or Visual Studio Code / Visual Studio (optional)

---

## 📦 Solution Structure

```
/ExternalUserServiceLibrary        # Class Library with client & services  
/ExternalUserConsoleApp            # Console App (entry point)  
/ExternalUserServiceLibrary.Tests  # xUnit Test Project  
```

---

## 🚀 How to Run the Console Application

```bash
dotnet build
dotnet run --project ExternalUserConsoleApp
```

You’ll see:

```
=== ReqRes User Viewer ===
1. Get user by ID
2. Get all users from page
0. Exit
```

---

## 🧪 How to Run Unit Tests

```bash
dotnet test ExternalUserServiceLibrary.Tests
```

- Tests are located in `ExternalUserServiceTests.cs`
- Covers service behavior, caching, and API call mocking

---

## ⚙️ Configuration

App settings are managed via `appsettings.json` in the console app:

```json
{
  "ApiSettings": {
    "BaseUrl": "https://reqres.in/api",
    "ApiKey": "reqres-free-v1",
    "CacheDurationSeconds": 60
  }
}
```

---

## 📄 License

[MIT](LICENSE) (or your license here)
