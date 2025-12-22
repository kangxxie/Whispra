# Whispra - Development Roadmap (Step-by-Step Implementation Guide)

This document provides a comprehensive, step-by-step guide to build Whispra from scratch. Each phase includes specific tasks, files to create, technologies involved, and learning resources where needed.

---

## Prerequisites & Learning Path

Before starting, you should have basic understanding of:

- Git basics (clone, commit, push, branches)
- Command line/terminal usage
- Basic programming concepts

**Technologies you'll learn throughout this project:**

- TypeScript (frontend & shared code)
- C# (backend)
- ASP.NET Core (web API framework)
- MongoDB (NoSQL database)
- Redis (caching & real-time)
- SignalR (real-time communication)
- React Native/Expo (mobile development)
- React + Vite (desktop web view)
- Tauri (desktop app wrapper)
- Docker (containerization)

---

# PHASE 0: Environment Setup & Prerequisites

**Goal**: Set up your development machine with all necessary tools.

## Step 0.1: Install Core Tools

### What to install:

1. **Node.js (LTS version)**

   - Download from: https://nodejs.org/
   - Check installation: `node --version` (should be 20.x or higher)
   - 📚 **Study needed**: Basic understanding of npm/package management (1-2 hours)
   - Resource: https://nodejs.org/en/learn/getting-started/introduction-to-nodejs

2. **.NET SDK (latest stable, 8.0+)**

   - Download from: https://dotnet.microsoft.com/download
   - Check installation: `dotnet --version`
   - 📚 **Study needed**: C# basics and .NET CLI commands (4-6 hours)
   - Resource: https://learn.microsoft.com/en-us/dotnet/csharp/tour-of-csharp/
   - Resource: https://learn.microsoft.com/en-us/dotnet/core/tools/

3. **Docker Desktop**

   - Download from: https://www.docker.com/products/docker-desktop
   - Check installation: `docker --version`
   - 📚 **Study needed**: Docker basics (containers, images, docker-compose) (2-3 hours)
   - Resource: https://docs.docker.com/get-started/

4. **Git**

   - Download from: https://git-scm.com/
   - Check installation: `git --version`

5. **VS Code** (recommended IDE)
   - Download from: https://code.visualstudio.com/
   - Install extensions:
     - C# Dev Kit
     - Docker
     - ESLint
     - Prettier
     - MongoDB for VS Code
     - REST Client (for API testing)

## Step 0.2: Create GitHub Repository

```bash
# Navigate to your projects folder
cd c:\Users\hibab\Desktop\Coding\GitHub\

# Initialize git repository
git init Whispra
cd Whispra

# Create initial commit
git add .
git commit -m "Initial commit"

# Add remote (create repo on GitHub first)
git remote add origin https://github.com/YOUR_USERNAME/Whispra.git
git push -u origin main
```

📚 **Study needed**: Git basics if unfamiliar (2-3 hours)

- Resource: https://www.atlassian.com/git/tutorials

---

# PHASE 1: Repository Structure & Infrastructure Setup

**Goal**: Create the monorepo structure and set up local development infrastructure (MongoDB + Redis).

## Step 1.1: Create Monorepo Folder Structure

Create the following folders and files:

```bash
# Run these commands in the Whispra root directory

# Frontend apps
mkdir -p apps/mobile
mkdir -p apps/desktop
mkdir -p apps/web

# Shared packages
mkdir -p packages/ui
mkdir -p packages/shared

# Backend
mkdir -p backend/Whispra.Api
mkdir -p backend/Whispra.Application
mkdir -p backend/Whispra.Domain
mkdir -p backend/Whispra.Infrastructure

# Root config
touch .gitignore
touch docker-compose.yml
touch README.md
```

**Files created**: Folder structure
**Technologies**: None yet, just organization
**Study needed**: ✅ None, just file system organization

## Step 1.2: Create Root .gitignore

Create `.gitignore` in the root with:

```gitignore
# Dependencies
node_modules/
**/node_modules/

# Build outputs
dist/
build/
out/
**/bin/
**/obj/

# Environment files
.env
.env.local
.env.*.local
**/.env

# IDE
.vscode/
.idea/
*.swp
*.swo
*~

# OS
.DS_Store
Thumbs.db

# Logs
*.log
npm-debug.log*

# Temporary files
*.tmp
.cache/

# ASP.NET
**/appsettings.Development.json
**/appsettings.*.json
!**/appsettings.json

# Expo
.expo/
.expo-shared/

# Tauri
**/target/
**/WixTools/
```

**Files created**: `.gitignore`
**Technologies**: None
**Study needed**: ✅ None

## Step 1.3: Set Up Docker Infrastructure (MongoDB + Redis)

Create `docker-compose.yml` in the root:

```yaml
version: "3.8"

services:
  mongodb:
    image: mongo:7
    container_name: whispra-mongodb
    restart: unless-stopped
    environment:
      MONGO_INITDB_ROOT_USERNAME: admin
      MONGO_INITDB_ROOT_PASSWORD: devpassword123
    ports:
      - "27017:27017"
    volumes:
      - mongodb_data:/data/db
    networks:
      - whispra-network

  redis:
    image: redis:7-alpine
    container_name: whispra-redis
    restart: unless-stopped
    command: redis-server --requirepass devredispassword123
    ports:
      - "6379:6379"
    volumes:
      - redis_data:/data
    networks:
      - whispra-network

volumes:
  mongodb_data:
  redis_data:

networks:
  whispra-network:
    driver: bridge
```

**Files created**: `docker-compose.yml`
**Technologies**: Docker, MongoDB, Redis
**Study needed**:

- 📚 Docker Compose basics (1-2 hours)
  - Resource: https://docs.docker.com/compose/gettingstarted/
- 📚 MongoDB concepts (NoSQL, documents, collections) (2-3 hours)
  - Resource: https://learn.mongodb.com/learning-paths/introduction-to-mongodb
- 📚 Redis basics (key-value store, caching) (1-2 hours)
  - Resource: https://redis.io/docs/getting-started/

### Start the infrastructure:

```bash
# Start MongoDB and Redis in background
docker compose up -d

# Check they're running
docker compose ps

# View logs if needed
docker compose logs mongodb
docker compose logs redis
```

## Step 1.4: Verify Infrastructure

Test MongoDB connection:

1. Install MongoDB Compass (GUI): https://www.mongodb.com/try/download/compass
2. Connect to: `mongodb://admin:devpassword123@localhost:27017/`
3. You should see the connection successful

Test Redis connection:

```bash
# Install Redis CLI tool or use Docker
docker exec -it whispra-redis redis-cli -a devredispassword123
# Type: PING
# Should respond: PONG
# Type: exit
```

**Study needed**: ✅ Just verification steps, no additional study

---

# PHASE 2: Backend Foundation (ASP.NET Core Setup)

**Goal**: Create the basic ASP.NET Core Web API project structure with clean architecture layers.

## Step 2.1: Understanding Clean Architecture

Before creating the backend, understand the layer separation:

- **Whispra.Domain**: Core business entities and rules (no dependencies on other layers)
- **Whispra.Application**: Use cases and business logic (depends only on Domain)
- **Whispra.Infrastructure**: External services (database, storage, email) (depends on Application & Domain)
- **Whispra.Api**: HTTP endpoints and SignalR hubs (depends on all layers)

📚 **Study needed**: Clean Architecture concepts (3-4 hours)

- Resource: https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures
- Resource: https://www.youtube.com/watch?v=lkmvnjypENw (Clean Architecture in ASP.NET Core)

## Step 2.2: Create Domain Layer

```bash
cd backend

# Create Domain project (class library)
dotnet new classlib -n Whispra.Domain
cd Whispra.Domain

# Remove default Class1.cs
rm Class1.cs

# Create folder structure
mkdir -p Entities/Users
mkdir -p Entities/Communities
mkdir -p Entities/Posts
mkdir -p Entities/Messages
mkdir -p ValueObjects
mkdir -p Enums
mkdir -p Exceptions
```

**Files to create**:

`Whispra.Domain/Entities/BaseEntity.cs`:

```csharp
namespace Whispra.Domain.Entities;

public abstract class BaseEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
}
```

`Whispra.Domain/Entities/Users/User.cs`:

```csharp
namespace Whispra.Domain.Entities.Users;

public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? Bio { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsEmailVerified { get; set; } = false;
}
```

`Whispra.Domain/Exceptions/DomainException.cs`:

```csharp
namespace Whispra.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }

    public DomainException(string message, Exception innerException)
        : base(message, innerException) { }
}
```

**Technologies**: C# 12, .NET 8 class library
**Study needed**:

- 📚 C# classes, properties, inheritance (4-5 hours if new to C#)
  - Resource: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/classes
- 📚 Entity design patterns (2-3 hours)
  - Resource: https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/

## Step 2.3: Create Application Layer

The Application layer contains use cases (business logic) and interfaces that Infrastructure will implement.

```bash
cd backend

# Create Application project (class library)
dotnet new classlib -n Whispra.Application
cd Whispra.Application

# Remove default Class1.cs
rm Class1.cs

# Add reference to Domain project
dotnet add reference ../Whispra.Domain/Whispra.Domain.csproj

# Create folder structure
mkdir -p UseCases/Users/Register
mkdir -p UseCases/Users/Login
mkdir -p Interfaces/Repositories
mkdir -p Interfaces/Services
mkdir -p DTOs/Users
mkdir -p Common
```

**Install NuGet packages**:

```bash
# Still in Whispra.Application folder
dotnet add package FluentValidation
dotnet add package Microsoft.Extensions.DependencyInjection.Abstractions
```

**Files to create**:

`Whispra.Application/Interfaces/Repositories/IUserRepository.cs`:

```csharp
using Whispra.Domain.Entities.Users;

namespace Whispra.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<User> CreateAsync(User user, CancellationToken cancellationToken = default);
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default);
}
```

`Whispra.Application/Interfaces/Services/IPasswordHasher.cs`:

```csharp
namespace Whispra.Application.Interfaces.Services;

public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}
```

`Whispra.Application/DTOs/Users/RegisterUserDto.cs`:

```csharp
namespace Whispra.Application.DTOs.Users;

public record RegisterUserDto(
    string Username,
    string Email,
    string Password
);
```

`Whispra.Application/DTOs/Users/UserResponseDto.cs`:

```csharp
namespace Whispra.Domain.Entities.Users;

public record UserResponseDto(
    string Id,
    string Username,
    string Email,
    string? DisplayName,
    string? Bio,
    string? ProfilePictureUrl,
    DateTime CreatedAt
);
```

`Whispra.Application/UseCases/Users/Register/RegisterUserUseCase.cs`:

```csharp
using Whispra.Application.DTOs.Users;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Application.Interfaces.Services;
using Whispra.Domain.Entities.Users;

namespace Whispra.Application.UseCases.Users.Register;

public class RegisterUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserUseCase(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserResponseDto> ExecuteAsync(
        RegisterUserDto dto,
        CancellationToken cancellationToken = default)
    {
        // Check if email already exists
        var existingUser = await _userRepository.GetByEmailAsync(dto.Email, cancellationToken);
        if (existingUser != null)
        {
            throw new InvalidOperationException("Email already registered");
        }

        // Check if username already exists
        existingUser = await _userRepository.GetByUsernameAsync(dto.Username, cancellationToken);
        if (existingUser != null)
        {
            throw new InvalidOperationException("Username already taken");
        }

        // Create new user
        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = _passwordHasher.HashPassword(dto.Password)
        };

        var createdUser = await _userRepository.CreateAsync(user, cancellationToken);

        return new UserResponseDto(
            createdUser.Id,
            createdUser.Username,
            createdUser.Email,
            createdUser.DisplayName,
            createdUser.Bio,
            createdUser.ProfilePictureUrl,
            createdUser.CreatedAt
        );
    }
}
```

`Whispra.Application/UseCases/Users/Register/RegisterUserValidator.cs`:

```csharp
using FluentValidation;
using Whispra.Application.DTOs.Users;

namespace Whispra.Application.UseCases.Users.Register;

public class RegisterUserValidator : AbstractValidator<RegisterUserDto>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters")
            .MaximumLength(30).WithMessage("Username must not exceed 30 characters")
            .Matches("^[a-zA-Z0-9_]+$").WithMessage("Username can only contain letters, numbers, and underscores");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches("[0-9]").WithMessage("Password must contain at least one number");
    }
}
```

**Technologies**: C# dependency injection, FluentValidation, DTOs, Use Cases pattern
**Study needed**:

- 📚 FluentValidation library (1-2 hours)
  - Resource: https://docs.fluentvalidation.net/en/latest/
- 📚 DTOs and Records in C# (1 hour)
  - Resource: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/record
- 📚 Dependency Injection in .NET (2-3 hours)
  - Resource: https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection

## Step 2.4: Create Infrastructure Layer

Infrastructure implements the interfaces defined in Application and handles external dependencies.

```bash
cd backend

# Create Infrastructure project
dotnet new classlib -n Whispra.Infrastructure
cd Whispra.Infrastructure

# Remove default Class1.cs
rm Class1.cs

# Add references
dotnet add reference ../Whispra.Domain/Whispra.Domain.csproj
dotnet add reference ../Whispra.Application/Whispra.Application.csproj

# Create folder structure
mkdir -p Persistence/MongoDB
mkdir -p Persistence/Repositories
mkdir -p Services
mkdir -p Configuration
```

**Install NuGet packages**:

```bash
# Still in Whispra.Infrastructure folder
dotnet add package MongoDB.Driver
dotnet add package Microsoft.Extensions.Options.ConfigurationExtensions
dotnet add package BCrypt.Net-Next
```

**Files to create**:

`Whispra.Infrastructure/Configuration/MongoDbSettings.cs`:

```csharp
namespace Whispra.Infrastructure.Configuration;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
}
```

`Whispra.Infrastructure/Persistence/MongoDB/MongoDbContext.cs`:

```csharp
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Whispra.Domain.Entities.Users;
using Whispra.Infrastructure.Configuration;

namespace Whispra.Infrastructure.Persistence.MongoDB;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<User> Users => _database.GetCollection<User>("users");

    // Future collections will be added here:
    // public IMongoCollection<Community> Communities => _database.GetCollection<Community>("communities");
    // public IMongoCollection<Post> Posts => _database.GetCollection<Post>("posts");
    // etc.
}
```

`Whispra.Infrastructure/Persistence/Repositories/UserRepository.cs`:

```csharp
using MongoDB.Driver;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Domain.Entities.Users;
using Whispra.Infrastructure.Persistence.MongoDB;

namespace Whispra.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly MongoDbContext _context;

    public UserRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Find(u => u.Id == id && !u.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Find(u => u.Email == email && !u.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Find(u => u.Username == username && !u.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.InsertOneAsync(user, cancellationToken: cancellationToken);
        return user;
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        user.UpdatedAt = DateTime.UtcNow;
        await _context.Users.ReplaceOneAsync(
            u => u.Id == user.Id,
            user,
            cancellationToken: cancellationToken);
    }

    public async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Find(u => u.Id == id && !u.IsDeleted)
            .AnyAsync(cancellationToken);
    }
}
```

`Whispra.Infrastructure/Services/PasswordHasher.cs`:

```csharp
using Whispra.Application.Interfaces.Services;

namespace Whispra.Infrastructure.Services;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, 12);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
```

**Technologies**: MongoDB.Driver, BCrypt for password hashing, Options pattern
**Study needed**:

- 📚 MongoDB C# Driver basics (3-4 hours)
  - Resource: https://www.mongodb.com/docs/drivers/csharp/current/quick-start/
- 📚 Options pattern in .NET (1-2 hours)
  - Resource: https://learn.microsoft.com/en-us/dotnet/core/extensions/options
- 📚 BCrypt password hashing (1 hour)
  - Resource: https://github.com/BcryptNet/bcrypt.net

## Step 2.5: Create API Layer

The API layer exposes HTTP endpoints and will later include SignalR hubs.

```bash
cd backend

# Create ASP.NET Core Web API project
dotnet new webapi -n Whispra.Api
cd Whispra.Api

# Add references to all other layers
dotnet add reference ../Whispra.Domain/Whispra.Domain.csproj
dotnet add reference ../Whispra.Application/Whispra.Application.csproj
dotnet add reference ../Whispra.Infrastructure/Whispra.Infrastructure.csproj

# Create folder structure
mkdir -p Controllers
mkdir -p Middleware
mkdir -p Extensions
```

**Install NuGet packages**:

```bash
# Still in Whispra.Api folder
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Swashbuckle.AspNetCore
```

**Files to create**:

`Whispra.Api/appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "MongoDbSettings": {
    "ConnectionString": "mongodb://admin:devpassword123@localhost:27017",
    "DatabaseName": "whispra_dev"
  }
}
```

`Whispra.Api/Extensions/ServiceCollectionExtensions.cs`:

```csharp
using Whispra.Application.Interfaces.Repositories;
using Whispra.Application.Interfaces.Services;
using Whispra.Application.UseCases.Users.Register;
using Whispra.Infrastructure.Configuration;
using Whispra.Infrastructure.Persistence.MongoDB;
using Whispra.Infrastructure.Persistence.Repositories;
using Whispra.Infrastructure.Services;

namespace Whispra.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register use cases
        services.AddScoped<RegisterUserUseCase>();

        return services;
    }

    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure MongoDB
        services.Configure<MongoDbSettings>(
            configuration.GetSection(nameof(MongoDbSettings)));
        services.AddSingleton<MongoDbContext>();

        // Register repositories
        services.AddScoped<IUserRepository, UserRepository>();

        // Register services
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        return services;
    }
}
```

`Whispra.Api/Controllers/UsersController.cs`:

```csharp
using Microsoft.AspNetCore.Mvc;
using Whispra.Application.DTOs.Users;
using Whispra.Application.UseCases.Users.Register;

namespace Whispra.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly RegisterUserUseCase _registerUserUseCase;

    public UsersController(RegisterUserUseCase registerUserUseCase)
    {
        _registerUserUseCase = registerUserUseCase;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserResponseDto>> Register(
        [FromBody] RegisterUserDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _registerUserUseCase.ExecuteAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(Register), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
```

`Whispra.Api/Program.cs`:

```csharp
using Whispra.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add application and infrastructure services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();
```

**Technologies**: ASP.NET Core Web API, Swagger/OpenAPI, Dependency Injection
**Study needed**:

- 📚 ASP.NET Core Web API basics (4-5 hours)
  - Resource: https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api
- 📚 Controllers and routing (2-3 hours)
  - Resource: https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/routing
- 📚 Swagger/OpenAPI documentation (1 hour)
  - Resource: https://learn.microsoft.com/en-us/aspnet/core/tutorials/web-api-help-pages-using-swagger

## Step 2.6: Create Backend Solution File

Create a solution file to manage all backend projects:

```bash
cd backend

# Create solution
dotnet new sln -n Whispra

# Add all projects to solution
dotnet sln add Whispra.Domain/Whispra.Domain.csproj
dotnet sln add Whispra.Application/Whispra.Application.csproj
dotnet sln add Whispra.Infrastructure/Whispra.Infrastructure.csproj
dotnet sln add Whispra.Api/Whispra.Api.csproj
```

## Step 2.7: Test the Backend

```bash
# Make sure Docker is running with MongoDB
docker compose up -d

# Navigate to API project
cd backend/Whispra.Api

# Run the API
dotnet run
```

You should see output like:

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7001
      Now listening on: http://localhost:5000
```

Open your browser to: `https://localhost:7001/swagger`

You'll see Swagger UI with your API endpoints!

**Test the registration endpoint**:

1. In Swagger UI, expand `POST /api/Users/register`
2. Click "Try it out"
3. Enter test data:

```json
{
  "username": "testuser",
  "email": "test@example.com",
  "password": "Test123456"
}
```

4. Click "Execute"
5. You should get a 201 Created response with the user data!

**Verify in MongoDB**:

1. Open MongoDB Compass
2. Connect to `mongodb://admin:devpassword123@localhost:27017/`
3. Look for database `whispra_dev` → collection `users`
4. You should see your registered user!

**Study needed**: ✅ Just testing

---

# PHASE 3: JWT Authentication & Login

**Goal**: Implement secure authentication using JWT tokens with refresh token rotation for session management.

## Step 3.1: Understanding JWT Authentication

**What is JWT?**

- **JWT (JSON Web Token)** is a compact, self-contained token for securely transmitting information
- Contains 3 parts: Header (algorithm), Payload (claims/user data), Signature (verification)
- **Access Token**: Short-lived (15 minutes), used for API requests
- **Refresh Token**: Long-lived (7 days), used to get new access tokens

**Why Refresh Token Rotation?**

- Improves security by limiting token lifetime
- If access token is stolen, it expires quickly
- Refresh tokens are stored securely and rotated on each use

📚 **Study needed**: JWT concepts and authentication flow (2-3 hours)

- Resource: https://jwt.io/introduction
- Resource: https://learn.microsoft.com/en-us/aspnet/core/security/authentication/

## Step 3.2: Add JWT Configuration to Domain Layer

Add token-related entities to the Domain layer.

```bash
cd backend/Whispra.Domain

# Create folder for auth entities
mkdir -p Entities/Auth
```

`Whispra.Domain/Entities/Auth/RefreshToken.cs`:

```csharp
namespace Whispra.Domain.Entities.Auth;

public class RefreshToken : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; } = false;
    public DateTime? RevokedAt { get; set; }
    public string? ReplacedByToken { get; set; }
}
```

**Study needed**: ✅ None, just entity definition

## Step 3.3: Update Application Layer for Authentication

Add DTOs and use cases for login and token management.

```bash
cd backend/Whispra.Application

# Create folders
mkdir -p DTOs/Auth
mkdir -p UseCases/Auth/Login
mkdir -p UseCases/Auth/RefreshToken
mkdir -p Interfaces/Services
```

**Install additional package**:

```bash
cd backend/Whispra.Application
dotnet add package System.IdentityModel.Tokens.Jwt
```

**Files to create**:

`Whispra.Application/DTOs/Auth/LoginDto.cs`:

```csharp
namespace Whispra.Application.DTOs.Auth;

public record LoginDto(
    string Email,
    string Password
);
```

`Whispra.Application/DTOs/Auth/AuthResponseDto.cs`:

```csharp
using Whispra.Application.DTOs.Users;

namespace Whispra.Application.DTOs.Auth;

public record AuthResponseDto(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt,
    DateTime RefreshTokenExpiresAt,
    UserResponseDto User
);
```

`Whispra.Application/DTOs/Auth/RefreshTokenDto.cs`:

```csharp
namespace Whispra.Application.DTOs.Auth;

public record RefreshTokenDto(
    string RefreshToken
);
```

`Whispra.Application/Interfaces/Services/IJwtTokenService.cs`:

```csharp
using Whispra.Domain.Entities.Users;

namespace Whispra.Application.Interfaces.Services;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    DateTime GetAccessTokenExpiration();
    DateTime GetRefreshTokenExpiration();
}
```

`Whispra.Application/Interfaces/Repositories/IRefreshTokenRepository.cs`:

```csharp
using Whispra.Domain.Entities.Auth;

namespace Whispra.Application.Interfaces.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<RefreshToken> CreateAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    Task UpdateAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    Task RevokeAllUserTokensAsync(string userId, CancellationToken cancellationToken = default);
}
```

`Whispra.Application/UseCases/Auth/Login/LoginUseCase.cs`:

```csharp
using Whispra.Application.DTOs.Auth;
using Whispra.Application.DTOs.Users;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Application.Interfaces.Services;
using Whispra.Domain.Entities.Auth;

namespace Whispra.Application.UseCases.Auth.Login;

public class LoginUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginUseCase(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponseDto> ExecuteAsync(
        LoginDto dto,
        CancellationToken cancellationToken = default)
    {
        // Find user by email
        var user = await _userRepository.GetByEmailAsync(dto.Email, cancellationToken);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Verify password
        if (!_passwordHasher.VerifyPassword(dto.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user, cancellationToken);

        // Generate tokens
        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        // Store refresh token
        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = _jwtTokenService.GetRefreshTokenExpiration()
        };
        await _refreshTokenRepository.CreateAsync(refreshTokenEntity, cancellationToken);

        var userResponse = new UserResponseDto(
            user.Id,
            user.Username,
            user.Email,
            user.DisplayName,
            user.Bio,
            user.ProfilePictureUrl,
            user.CreatedAt
        );

        return new AuthResponseDto(
            accessToken,
            refreshToken,
            _jwtTokenService.GetAccessTokenExpiration(),
            _jwtTokenService.GetRefreshTokenExpiration(),
            userResponse
        );
    }
}
```

`Whispra.Application/UseCases/Auth/RefreshToken/RefreshTokenUseCase.cs`:

```csharp
using Whispra.Application.DTOs.Auth;
using Whispra.Application.DTOs.Users;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Application.Interfaces.Services;
using Whispra.Domain.Entities.Auth;

namespace Whispra.Application.UseCases.Auth.RefreshToken;

public class RefreshTokenUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtTokenService _jwtTokenService;

    public RefreshTokenUseCase(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponseDto> ExecuteAsync(
        RefreshTokenDto dto,
        CancellationToken cancellationToken = default)
    {
        // Find refresh token
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(
            dto.RefreshToken, cancellationToken);

        if (storedToken == null || storedToken.IsRevoked)
        {
            throw new UnauthorizedAccessException("Invalid refresh token");
        }

        // Check if expired
        if (storedToken.ExpiresAt < DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Refresh token expired");
        }

        // Get user
        var user = await _userRepository.GetByIdAsync(storedToken.UserId, cancellationToken);
        if (user == null)
        {
            throw new UnauthorizedAccessException("User not found");
        }

        // Generate new tokens
        var newAccessToken = _jwtTokenService.GenerateAccessToken(user);
        var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

        // Revoke old refresh token and create new one (rotation)
        storedToken.IsRevoked = true;
        storedToken.RevokedAt = DateTime.UtcNow;
        storedToken.ReplacedByToken = newRefreshToken;
        await _refreshTokenRepository.UpdateAsync(storedToken, cancellationToken);

        var newRefreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = newRefreshToken,
            ExpiresAt = _jwtTokenService.GetRefreshTokenExpiration()
        };
        await _refreshTokenRepository.CreateAsync(newRefreshTokenEntity, cancellationToken);

        var userResponse = new UserResponseDto(
            user.Id,
            user.Username,
            user.Email,
            user.DisplayName,
            user.Bio,
            user.ProfilePictureUrl,
            user.CreatedAt
        );

        return new AuthResponseDto(
            newAccessToken,
            newRefreshToken,
            _jwtTokenService.GetAccessTokenExpiration(),
            _jwtTokenService.GetRefreshTokenExpiration(),
            userResponse
        );
    }
}
```

**Technologies**: JWT tokens, System.IdentityModel.Tokens.Jwt
**Study needed**:

- 📚 JWT token generation and validation (2-3 hours)
  - Resource: https://learn.microsoft.com/en-us/dotnet/api/system.identitymodel.tokens.jwt
- 📚 Refresh token rotation pattern (1-2 hours)
  - Resource: https://auth0.com/docs/secure/tokens/refresh-tokens/refresh-token-rotation

## Step 3.4: Implement JWT Service in Infrastructure

```bash
cd backend/Whispra.Infrastructure

# Create folder
mkdir -p Services/Auth
```

**Install packages**:

```bash
cd backend/Whispra.Infrastructure
dotnet add package System.IdentityModel.Tokens.Jwt
dotnet add package Microsoft.IdentityModel.Tokens
```

`Whispra.Infrastructure/Configuration/JwtSettings.cs`:

```csharp
namespace Whispra.Infrastructure.Configuration;

public class JwtSettings
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessTokenExpirationMinutes { get; set; } = 15;
    public int RefreshTokenExpirationDays { get; set; } = 7;
}
```

`Whispra.Infrastructure/Services/Auth/JwtTokenService.cs`:

```csharp
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Whispra.Application.Interfaces.Services;
using Whispra.Domain.Entities.Users;
using Whispra.Infrastructure.Configuration;

namespace Whispra.Infrastructure.Services.Auth;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _jwtSettings;

    public JwtTokenService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    public string GenerateAccessToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: GetAccessTokenExpiration(),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public DateTime GetAccessTokenExpiration()
    {
        return DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes);
    }

    public DateTime GetRefreshTokenExpiration()
    {
        return DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);
    }
}
```

`Whispra.Infrastructure/Persistence/Repositories/RefreshTokenRepository.cs`:

```csharp
using MongoDB.Driver;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Domain.Entities.Auth;
using Whispra.Infrastructure.Persistence.MongoDB;

namespace Whispra.Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly MongoDbContext _context;

    public RefreshTokenRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _context.RefreshTokens
            .Find(rt => rt.Token == token)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<RefreshToken> CreateAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        await _context.RefreshTokens.InsertOneAsync(refreshToken, cancellationToken: cancellationToken);
        return refreshToken;
    }

    public async Task UpdateAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        refreshToken.UpdatedAt = DateTime.UtcNow;
        await _context.RefreshTokens.ReplaceOneAsync(
            rt => rt.Id == refreshToken.Id,
            refreshToken,
            cancellationToken: cancellationToken);
    }

    public async Task RevokeAllUserTokensAsync(string userId, CancellationToken cancellationToken = default)
    {
        var update = Builders<RefreshToken>.Update
            .Set(rt => rt.IsRevoked, true)
            .Set(rt => rt.RevokedAt, DateTime.UtcNow);

        await _context.RefreshTokens.UpdateManyAsync(
            rt => rt.UserId == userId && !rt.IsRevoked,
            update,
            cancellationToken: cancellationToken);
    }
}
```

Update `Whispra.Infrastructure/Persistence/MongoDB/MongoDbContext.cs` to include RefreshTokens collection:

```csharp
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Whispra.Domain.Entities.Auth;
using Whispra.Domain.Entities.Users;
using Whispra.Infrastructure.Configuration;

namespace Whispra.Infrastructure.Persistence.MongoDB;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<User> Users => _database.GetCollection<User>("users");
    public IMongoCollection<RefreshToken> RefreshTokens => _database.GetCollection<RefreshToken>("refresh_tokens");
}
```

**Technologies**: JWT generation, symmetric key encryption, claims-based identity
**Study needed**:

- 📚 Symmetric key cryptography basics (1 hour)
  - Resource: https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.symmetricsecuritykey
- 📚 Claims in .NET (1-2 hours)
  - Resource: https://learn.microsoft.com/en-us/dotnet/api/system.security.claims.claim

## Step 3.5: Update API Layer for Authentication

Add JWT authentication configuration and auth controller.

```bash
cd backend/Whispra.Api

# Create auth controller folder
mkdir -p Controllers
```

Update `Whispra.Api/appsettings.json` to include JWT settings:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "MongoDbSettings": {
    "ConnectionString": "mongodb://admin:devpassword123@localhost:27017",
    "DatabaseName": "whispra_dev"
  },
  "JwtSettings": {
    "Secret": "YourVeryLongSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "WhispraApi",
    "Audience": "WhispraClients",
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
  }
}
```

⚠️ **IMPORTANT**: In production, never hardcode the Secret! Use environment variables or Azure Key Vault.

`Whispra.Api/Controllers/AuthController.cs`:

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Whispra.Application.DTOs.Auth;
using Whispra.Application.UseCases.Auth.Login;
using Whispra.Application.UseCases.Auth.RefreshToken;

namespace Whispra.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly LoginUseCase _loginUseCase;
    private readonly RefreshTokenUseCase _refreshTokenUseCase;

    public AuthController(
        LoginUseCase loginUseCase,
        RefreshTokenUseCase refreshTokenUseCase)
    {
        _loginUseCase = loginUseCase;
        _refreshTokenUseCase = refreshTokenUseCase;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(
        [FromBody] LoginDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _loginUseCase.ExecuteAsync(dto, cancellationToken);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponseDto>> RefreshToken(
        [FromBody] RefreshTokenDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _refreshTokenUseCase.ExecuteAsync(dto, cancellationToken);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }

    [Authorize]
    [HttpGet("me")]
    public ActionResult<object> GetCurrentUser()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var username = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

        return Ok(new
        {
            userId,
            username,
            email,
            message = "You are authenticated!"
        });
    }
}
```

Update `Whispra.Api/Extensions/ServiceCollectionExtensions.cs`:

```csharp
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Application.Interfaces.Services;
using Whispra.Application.UseCases.Auth.Login;
using Whispra.Application.UseCases.Auth.RefreshToken;
using Whispra.Application.UseCases.Users.Register;
using Whispra.Infrastructure.Configuration;
using Whispra.Infrastructure.Persistence.MongoDB;
using Whispra.Infrastructure.Persistence.Repositories;
using Whispra.Infrastructure.Services;
using Whispra.Infrastructure.Services.Auth;

namespace Whispra.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register use cases
        services.AddScoped<RegisterUserUseCase>();
        services.AddScoped<LoginUseCase>();
        services.AddScoped<RefreshTokenUseCase>();

        return services;
    }

    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure MongoDB
        services.Configure<MongoDbSettings>(
            configuration.GetSection(nameof(MongoDbSettings)));
        services.AddSingleton<MongoDbContext>();

        // Configure JWT
        services.Configure<JwtSettings>(
            configuration.GetSection(nameof(JwtSettings)));

        // Register repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        // Register services
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();

        return services;
    }

    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings!.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                ClockSkew = TimeSpan.Zero // Remove default 5 minute tolerance
            };
        });

        return services;
    }
}
```

Update `Whispra.Api/Program.cs`:

```csharp
using Whispra.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add application and infrastructure services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Authentication & Authorization middleware (order matters!)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
```

**Technologies**: JWT Bearer authentication, ASP.NET Core middleware
**Study needed**:

- 📚 ASP.NET Core authentication middleware (2-3 hours)
  - Resource: https://learn.microsoft.com/en-us/aspnet/core/security/authentication/
- 📚 JWT Bearer authentication in ASP.NET (2 hours)
  - Resource: https://learn.microsoft.com/en-us/aspnet/core/security/authentication/jwt-authn

## Step 3.6: Test Authentication

```bash
# Make sure MongoDB is running
docker compose up -d

# Run the API
cd backend/Whispra.Api
dotnet run
```

Open Swagger: `https://localhost:7001/swagger`

### Test Flow:

**1. Register a new user** (if you haven't already):

```json
POST /api/Users/register
{
  "username": "authtest",
  "email": "authtest@example.com",
  "password": "Test123456"
}
```

**2. Login**:

```json
POST /api/Auth/login
{
  "email": "authtest@example.com",
  "password": "Test123456"
}
```

Response will include:

```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "ZXlKaGJHY2lPaUpJVXpJMU5pSXNJblI1Y0NJNklrcFhWQ0o5...",
  "accessTokenExpiresAt": "2025-12-22T14:15:00Z",
  "refreshTokenExpiresAt": "2025-12-29T13:00:00Z",
  "user": {
    "id": "...",
    "username": "authtest",
    "email": "authtest@example.com",
    ...
  }
}
```

**3. Test protected endpoint**:

- In Swagger, click the 🔒 "Authorize" button at the top
- Enter: `Bearer YOUR_ACCESS_TOKEN_HERE` (replace with actual token from login response)
- Click "Authorize"
- Now try `GET /api/Auth/me`
- You should get your user info!

**4. Test refresh token**:

```json
POST /api/Auth/refresh
{
  "refreshToken": "YOUR_REFRESH_TOKEN_FROM_LOGIN"
}
```

You'll get new access and refresh tokens!

**Study needed**: ✅ Just testing,

---

# PHASE 4: Communities (Groups with Roles & Permissions)

**Goal**: Implement community creation, membership management, role-based permissions, and invite system.

## Step 4.1: Understanding Communities Architecture

**What are Communities?**

- Communities are groups (like Facebook groups or Discord servers)
- Can be **public** (anyone can see and join) or **private** (invite-only)
- Members have **roles**: Owner, Moderator, Member
- Each role has different **permissions** (create posts, invite members, moderate, etc.)

**Key Entities**:

- **Community**: The group itself (name, description, privacy settings)
- **CommunityMember**: Join table linking users to communities with their role
- **CommunityInvite**: Invitation links/codes for private communities

📚 **Study needed**: Role-based access control (RBAC) concepts (2-3 hours)

- Resource: https://learn.microsoft.com/en-us/azure/role-based-access-control/overview
- Resource: https://auth0.com/docs/manage-users/access-control/rbac

## Step 4.2: Add Community Entities to Domain Layer

```bash
cd backend/Whispra.Domain

# Create folders
mkdir -p Entities/Communities
mkdir -p Enums
```

`Whispra.Domain/Enums/CommunityPrivacy.cs`:

```csharp
namespace Whispra.Domain.Enums;

public enum CommunityPrivacy
{
    Public = 0,    // Anyone can see and join
    Private = 1    // Invite-only, hidden from public search
}
```

`Whispra.Domain/Enums/CommunityRole.cs`:

```csharp
namespace Whispra.Domain.Enums;

public enum CommunityRole
{
    Member = 0,      // Can view posts, comment, create posts
    Moderator = 1,   // + Can moderate content, invite members
    Owner = 2        // + Can change settings, manage roles, delete community
}
```

`Whispra.Domain/Entities/Communities/Community.cs`:

```csharp
using Whispra.Domain.Enums;

namespace Whispra.Domain.Entities.Communities;

public class Community : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public CommunityPrivacy Privacy { get; set; } = CommunityPrivacy.Public;
    public string OwnerId { get; set; } = string.Empty;
    public int MemberCount { get; set; } = 0;
    public List<string> Tags { get; set; } = new();
}
```

`Whispra.Domain/Entities/Communities/CommunityMember.cs`:

```csharp
using Whispra.Domain.Enums;

namespace Whispra.Domain.Entities.Communities;

public class CommunityMember : BaseEntity
{
    public string CommunityId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public CommunityRole Role { get; set; } = CommunityRole.Member;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}
```

`Whispra.Domain/Entities/Communities/CommunityInvite.cs`:

```csharp
namespace Whispra.Domain.Entities.Communities;

public class CommunityInvite : BaseEntity
{
    public string CommunityId { get; set; } = string.Empty;
    public string InviteCode { get; set; } = string.Empty;
    public string CreatedByUserId { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public int? MaxUses { get; set; } // null = unlimited
    public int UsesCount { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}
```

**Technologies**: Enums, entity relationships
**Study needed**:

- 📚 C# Enums (1 hour)
  - Resource: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/enum
- ✅ Entity relationships are straightforward here

## Step 4.3: Add Community DTOs and Interfaces to Application Layer

```bash
cd backend/Whispra.Application

# Create folders
mkdir -p DTOs/Communities
mkdir -p UseCases/Communities/Create
mkdir -p UseCases/Communities/Join
mkdir -p UseCases/Communities/Leave
mkdir -p UseCases/Communities/UpdateRole
mkdir -p UseCases/Communities/CreateInvite
mkdir -p Interfaces/Repositories
```

`Whispra.Application/DTOs/Communities/CreateCommunityDto.cs`:

```csharp
using Whispra.Domain.Enums;

namespace Whispra.Application.DTOs.Communities;

public record CreateCommunityDto(
    string Name,
    string? Description,
    CommunityPrivacy Privacy,
    List<string>? Tags
);
```

`Whispra.Application/DTOs/Communities/CommunityResponseDto.cs`:

```csharp
using Whispra.Domain.Enums;

namespace Whispra.Application.DTOs.Communities;

public record CommunityResponseDto(
    string Id,
    string Name,
    string? Description,
    string? CoverImageUrl,
    CommunityPrivacy Privacy,
    string OwnerId,
    int MemberCount,
    List<string> Tags,
    DateTime CreatedAt,
    CommunityRole? CurrentUserRole = null // Will be populated if user is a member
);
```

`Whispra.Application/DTOs/Communities/JoinCommunityDto.cs`:

```csharp
namespace Whispra.Application.DTOs.Communities;

public record JoinCommunityDto(
    string? InviteCode = null // Required for private communities
);
```

`Whispra.Application/DTOs/Communities/UpdateMemberRoleDto.cs`:

```csharp
using Whispra.Domain.Enums;

namespace Whispra.Application.DTOs.Communities;

public record UpdateMemberRoleDto(
    string UserId,
    CommunityRole NewRole
);
```

`Whispra.Application/DTOs/Communities/CreateInviteDto.cs`:

```csharp
namespace Whispra.Application.DTOs.Communities;

public record CreateInviteDto(
    int? MaxUses = null,
    int? ExpiresInDays = 7
);
```

`Whispra.Application/DTOs/Communities/InviteResponseDto.cs`:

```csharp
namespace Whispra.Application.DTOs.Communities;

public record InviteResponseDto(
    string Id,
    string InviteCode,
    string CommunityId,
    DateTime ExpiresAt,
    int? MaxUses,
    int UsesCount,
    bool IsActive
);
```

`Whispra.Application/Interfaces/Repositories/ICommunityRepository.cs`:

```csharp
using Whispra.Domain.Entities.Communities;
using Whispra.Domain.Enums;

namespace Whispra.Application.Interfaces.Repositories;

public interface ICommunityRepository
{
    Task<Community?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Community> CreateAsync(Community community, CancellationToken cancellationToken = default);
    Task UpdateAsync(Community community, CancellationToken cancellationToken = default);
    Task<List<Community>> GetPublicCommunitiesAsync(int skip, int limit, CancellationToken cancellationToken = default);
    Task<List<Community>> GetUserCommunitiesAsync(string userId, int skip, int limit, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default);
}
```

`Whispra.Application/Interfaces/Repositories/ICommunityMemberRepository.cs`:

```csharp
using Whispra.Domain.Entities.Communities;
using Whispra.Domain.Enums;

namespace Whispra.Application.Interfaces.Repositories;

public interface ICommunityMemberRepository
{
    Task<CommunityMember?> GetMembershipAsync(string communityId, string userId, CancellationToken cancellationToken = default);
    Task<CommunityMember> CreateAsync(CommunityMember member, CancellationToken cancellationToken = default);
    Task UpdateAsync(CommunityMember member, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<List<CommunityMember>> GetCommunityMembersAsync(string communityId, CancellationToken cancellationToken = default);
    Task<int> GetMemberCountAsync(string communityId, CancellationToken cancellationToken = default);
    Task<bool> IsMemberAsync(string communityId, string userId, CancellationToken cancellationToken = default);
}
```

`Whispra.Application/Interfaces/Repositories/ICommunityInviteRepository.cs`:

```csharp
using Whispra.Domain.Entities.Communities;

namespace Whispra.Application.Interfaces.Repositories;

public interface ICommunityInviteRepository
{
    Task<CommunityInvite?> GetByCodeAsync(string inviteCode, CancellationToken cancellationToken = default);
    Task<CommunityInvite> CreateAsync(CommunityInvite invite, CancellationToken cancellationToken = default);
    Task UpdateAsync(CommunityInvite invite, CancellationToken cancellationToken = default);
    Task<List<CommunityInvite>> GetCommunityInvitesAsync(string communityId, CancellationToken cancellationToken = default);
}
```

**Technologies**: DTOs with optional parameters, repository interfaces
**Study needed**: ✅ None, these follow patterns you've already learned

## Step 4.4: Create Community Use Cases

`Whispra.Application/UseCases/Communities/Create/CreateCommunityUseCase.cs`:

```csharp
using Whispra.Application.DTOs.Communities;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Domain.Entities.Communities;
using Whispra.Domain.Enums;

namespace Whispra.Application.UseCases.Communities.Create;

public class CreateCommunityUseCase
{
    private readonly ICommunityRepository _communityRepository;
    private readonly ICommunityMemberRepository _memberRepository;

    public CreateCommunityUseCase(
        ICommunityRepository communityRepository,
        ICommunityMemberRepository memberRepository)
    {
        _communityRepository = communityRepository;
        _memberRepository = memberRepository;
    }

    public async Task<CommunityResponseDto> ExecuteAsync(
        CreateCommunityDto dto,
        string currentUserId,
        CancellationToken cancellationToken = default)
    {
        // Create community
        var community = new Community
        {
            Name = dto.Name,
            Description = dto.Description,
            Privacy = dto.Privacy,
            OwnerId = currentUserId,
            MemberCount = 1,
            Tags = dto.Tags ?? new List<string>()
        };

        var createdCommunity = await _communityRepository.CreateAsync(community, cancellationToken);

        // Add creator as owner/member
        var membership = new CommunityMember
        {
            CommunityId = createdCommunity.Id,
            UserId = currentUserId,
            Role = CommunityRole.Owner
        };

        await _memberRepository.CreateAsync(membership, cancellationToken);

        return new CommunityResponseDto(
            createdCommunity.Id,
            createdCommunity.Name,
            createdCommunity.Description,
            createdCommunity.CoverImageUrl,
            createdCommunity.Privacy,
            createdCommunity.OwnerId,
            createdCommunity.MemberCount,
            createdCommunity.Tags,
            createdCommunity.CreatedAt,
            CommunityRole.Owner
        );
    }
}
```

`Whispra.Application/UseCases/Communities/Join/JoinCommunityUseCase.cs`:

```csharp
using Whispra.Application.DTOs.Communities;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Domain.Entities.Communities;
using Whispra.Domain.Enums;

namespace Whispra.Application.UseCases.Communities.Join;

public class JoinCommunityUseCase
{
    private readonly ICommunityRepository _communityRepository;
    private readonly ICommunityMemberRepository _memberRepository;
    private readonly ICommunityInviteRepository _inviteRepository;

    public JoinCommunityUseCase(
        ICommunityRepository communityRepository,
        ICommunityMemberRepository memberRepository,
        ICommunityInviteRepository inviteRepository)
    {
        _communityRepository = communityRepository;
        _memberRepository = memberRepository;
        _inviteRepository = inviteRepository;
    }

    public async Task<CommunityResponseDto> ExecuteAsync(
        string communityId,
        string currentUserId,
        JoinCommunityDto dto,
        CancellationToken cancellationToken = default)
    {
        var community = await _communityRepository.GetByIdAsync(communityId, cancellationToken);
        if (community == null)
        {
            throw new InvalidOperationException("Community not found");
        }

        // Check if already a member
        var existingMembership = await _memberRepository.GetMembershipAsync(
            communityId, currentUserId, cancellationToken);
        if (existingMembership != null)
        {
            throw new InvalidOperationException("Already a member of this community");
        }

        // If private, validate invite code
        if (community.Privacy == CommunityPrivacy.Private)
        {
            if (string.IsNullOrEmpty(dto.InviteCode))
            {
                throw new InvalidOperationException("Invite code required for private community");
            }

            var invite = await _inviteRepository.GetByCodeAsync(dto.InviteCode, cancellationToken);
            if (invite == null || !invite.IsActive || invite.CommunityId != communityId)
            {
                throw new InvalidOperationException("Invalid invite code");
            }

            if (invite.ExpiresAt < DateTime.UtcNow)
            {
                throw new InvalidOperationException("Invite code expired");
            }

            if (invite.MaxUses.HasValue && invite.UsesCount >= invite.MaxUses.Value)
            {
                throw new InvalidOperationException("Invite code has reached max uses");
            }

            // Increment invite uses
            invite.UsesCount++;
            await _inviteRepository.UpdateAsync(invite, cancellationToken);
        }

        // Add member
        var membership = new CommunityMember
        {
            CommunityId = communityId,
            UserId = currentUserId,
            Role = CommunityRole.Member
        };

        await _memberRepository.CreateAsync(membership, cancellationToken);

        // Update member count
        community.MemberCount++;
        await _communityRepository.UpdateAsync(community, cancellationToken);

        return new CommunityResponseDto(
            community.Id,
            community.Name,
            community.Description,
            community.CoverImageUrl,
            community.Privacy,
            community.OwnerId,
            community.MemberCount,
            community.Tags,
            community.CreatedAt,
            CommunityRole.Member
        );
    }
}
```

`Whispra.Application/UseCases/Communities/Leave/LeaveCommunityUseCase.cs`:

```csharp
using Whispra.Application.Interfaces.Repositories;
using Whispra.Domain.Enums;

namespace Whispra.Application.UseCases.Communities.Leave;

public class LeaveCommunityUseCase
{
    private readonly ICommunityRepository _communityRepository;
    private readonly ICommunityMemberRepository _memberRepository;

    public LeaveCommunityUseCase(
        ICommunityRepository communityRepository,
        ICommunityMemberRepository memberRepository)
    {
        _communityRepository = communityRepository;
        _memberRepository = memberRepository;
    }

    public async Task ExecuteAsync(
        string communityId,
        string currentUserId,
        CancellationToken cancellationToken = default)
    {
        var community = await _communityRepository.GetByIdAsync(communityId, cancellationToken);
        if (community == null)
        {
            throw new InvalidOperationException("Community not found");
        }

        var membership = await _memberRepository.GetMembershipAsync(
            communityId, currentUserId, cancellationToken);
        if (membership == null)
        {
            throw new InvalidOperationException("Not a member of this community");
        }

        // Owner cannot leave (must transfer ownership or delete community first)
        if (membership.Role == CommunityRole.Owner)
        {
            throw new InvalidOperationException("Owner cannot leave community. Transfer ownership or delete the community first.");
        }

        // Remove membership
        await _memberRepository.DeleteAsync(membership.Id, cancellationToken);

        // Update member count
        community.MemberCount--;
        await _communityRepository.UpdateAsync(community, cancellationToken);
    }
}
```

`Whispra.Application/UseCases/Communities/UpdateRole/UpdateMemberRoleUseCase.cs`:

```csharp
using Whispra.Application.DTOs.Communities;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Domain.Enums;

namespace Whispra.Application.UseCases.Communities.UpdateRole;

public class UpdateMemberRoleUseCase
{
    private readonly ICommunityMemberRepository _memberRepository;

    public UpdateMemberRoleUseCase(ICommunityMemberRepository memberRepository)
    {
        _memberRepository = memberRepository;
    }

    public async Task ExecuteAsync(
        string communityId,
        UpdateMemberRoleDto dto,
        string currentUserId,
        CancellationToken cancellationToken = default)
    {
        // Check if current user is owner or moderator
        var currentUserMembership = await _memberRepository.GetMembershipAsync(
            communityId, currentUserId, cancellationToken);

        if (currentUserMembership == null)
        {
            throw new UnauthorizedAccessException("You are not a member of this community");
        }

        if (currentUserMembership.Role != CommunityRole.Owner &&
            currentUserMembership.Role != CommunityRole.Moderator)
        {
            throw new UnauthorizedAccessException("Only owners and moderators can change roles");
        }

        // Cannot change owner role (would need ownership transfer use case)
        if (dto.NewRole == CommunityRole.Owner)
        {
            throw new InvalidOperationException("Cannot assign owner role. Use transfer ownership instead.");
        }

        // Get target member
        var targetMembership = await _memberRepository.GetMembershipAsync(
            communityId, dto.UserId, cancellationToken);

        if (targetMembership == null)
        {
            throw new InvalidOperationException("User is not a member of this community");
        }

        // Moderators cannot change other moderators' or owner's roles
        if (currentUserMembership.Role == CommunityRole.Moderator &&
            targetMembership.Role != CommunityRole.Member)
        {
            throw new UnauthorizedAccessException("Moderators can only change regular members' roles");
        }

        // Update role
        targetMembership.Role = dto.NewRole;
        await _memberRepository.UpdateAsync(targetMembership, cancellationToken);
    }
}
```

`Whispra.Application/UseCases/Communities/CreateInvite/CreateInviteUseCase.cs`:

```csharp
using System.Security.Cryptography;
using Whispra.Application.DTOs.Communities;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Domain.Entities.Communities;
using Whispra.Domain.Enums;

namespace Whispra.Application.UseCases.Communities.CreateInvite;

public class CreateInviteUseCase
{
    private readonly ICommunityRepository _communityRepository;
    private readonly ICommunityMemberRepository _memberRepository;
    private readonly ICommunityInviteRepository _inviteRepository;

    public CreateInviteUseCase(
        ICommunityRepository communityRepository,
        ICommunityMemberRepository memberRepository,
        ICommunityInviteRepository inviteRepository)
    {
        _communityRepository = communityRepository;
        _memberRepository = memberRepository;
        _inviteRepository = inviteRepository;
    }

    public async Task<InviteResponseDto> ExecuteAsync(
        string communityId,
        CreateInviteDto dto,
        string currentUserId,
        CancellationToken cancellationToken = default)
    {
        var community = await _communityRepository.GetByIdAsync(communityId, cancellationToken);
        if (community == null)
        {
            throw new InvalidOperationException("Community not found");
        }

        // Check if user can create invites (moderator or owner)
        var membership = await _memberRepository.GetMembershipAsync(
            communityId, currentUserId, cancellationToken);

        if (membership == null)
        {
            throw new UnauthorizedAccessException("You are not a member of this community");
        }

        if (membership.Role != CommunityRole.Owner &&
            membership.Role != CommunityRole.Moderator)
        {
            throw new UnauthorizedAccessException("Only owners and moderators can create invites");
        }

        // Generate invite code
        var inviteCode = GenerateInviteCode();

        var invite = new CommunityInvite
        {
            CommunityId = communityId,
            InviteCode = inviteCode,
            CreatedByUserId = currentUserId,
            ExpiresAt = DateTime.UtcNow.AddDays(dto.ExpiresInDays ?? 7),
            MaxUses = dto.MaxUses
        };

        var createdInvite = await _inviteRepository.CreateAsync(invite, cancellationToken);

        return new InviteResponseDto(
            createdInvite.Id,
            createdInvite.InviteCode,
            createdInvite.CommunityId,
            createdInvite.ExpiresAt,
            createdInvite.MaxUses,
            createdInvite.UsesCount,
            createdInvite.IsActive
        );
    }

    private static string GenerateInviteCode()
    {
        var bytes = new byte[8];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes)
            .Replace("+", "")
            .Replace("/", "")
            .Replace("=", "")
            .ToUpper();
    }
}
```

**Technologies**: Business logic, authorization checks, validation
**Study needed**:

- 📚 Authorization patterns (2-3 hours) - Important for understanding role checks!
  - Resource: https://learn.microsoft.com/en-us/aspnet/core/security/authorization/introduction

## Step 4.5: Implement Infrastructure Layer for Communities

```bash
cd backend/Whispra.Infrastructure/Persistence/Repositories
```

`Whispra.Infrastructure/Persistence/Repositories/CommunityRepository.cs`:

```csharp
using MongoDB.Driver;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Domain.Entities.Communities;
using Whispra.Infrastructure.Persistence.MongoDB;

namespace Whispra.Infrastructure.Persistence.Repositories;

public class CommunityRepository : ICommunityRepository
{
    private readonly MongoDbContext _context;

    public CommunityRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<Community?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.Communities
            .Find(c => c.Id == id && !c.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Community> CreateAsync(Community community, CancellationToken cancellationToken = default)
    {
        await _context.Communities.InsertOneAsync(community, cancellationToken: cancellationToken);
        return community;
    }

    public async Task UpdateAsync(Community community, CancellationToken cancellationToken = default)
    {
        community.UpdatedAt = DateTime.UtcNow;
        await _context.Communities.ReplaceOneAsync(
            c => c.Id == community.Id,
            community,
            cancellationToken: cancellationToken);
    }

    public async Task<List<Community>> GetPublicCommunitiesAsync(
        int skip, int limit, CancellationToken cancellationToken = default)
    {
        return await _context.Communities
            .Find(c => !c.IsDeleted && c.Privacy == Domain.Enums.CommunityPrivacy.Public)
            .SortByDescending(c => c.CreatedAt)
            .Skip(skip)
            .Limit(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Community>> GetUserCommunitiesAsync(
        string userId, int skip, int limit, CancellationToken cancellationToken = default)
    {
        // This requires joining with CommunityMember collection
        // For simplicity, we'll implement a basic version
        // In production, you'd use aggregation pipeline
        var membershipIds = await _context.CommunityMembers
            .Find(m => m.UserId == userId)
            .Project(m => m.CommunityId)
            .ToListAsync(cancellationToken);

        return await _context.Communities
            .Find(c => membershipIds.Contains(c.Id) && !c.IsDeleted)
            .SortByDescending(c => c.CreatedAt)
            .Skip(skip)
            .Limit(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.Communities
            .Find(c => c.Id == id && !c.IsDeleted)
            .AnyAsync(cancellationToken);
    }
}
```

`Whispra.Infrastructure/Persistence/Repositories/CommunityMemberRepository.cs`:

```csharp
using MongoDB.Driver;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Domain.Entities.Communities;
using Whispra.Infrastructure.Persistence.MongoDB;

namespace Whispra.Infrastructure.Persistence.Repositories;

public class CommunityMemberRepository : ICommunityMemberRepository
{
    private readonly MongoDbContext _context;

    public CommunityMemberRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<CommunityMember?> GetMembershipAsync(
        string communityId, string userId, CancellationToken cancellationToken = default)
    {
        return await _context.CommunityMembers
            .Find(m => m.CommunityId == communityId && m.UserId == userId && !m.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<CommunityMember> CreateAsync(
        CommunityMember member, CancellationToken cancellationToken = default)
    {
        await _context.CommunityMembers.InsertOneAsync(member, cancellationToken: cancellationToken);
        return member;
    }

    public async Task UpdateAsync(CommunityMember member, CancellationToken cancellationToken = default)
    {
        member.UpdatedAt = DateTime.UtcNow;
        await _context.CommunityMembers.ReplaceOneAsync(
            m => m.Id == member.Id,
            member,
            cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var update = Builders<CommunityMember>.Update
            .Set(m => m.IsDeleted, true)
            .Set(m => m.DeletedAt, DateTime.UtcNow);

        await _context.CommunityMembers.UpdateOneAsync(
            m => m.Id == id,
            update,
            cancellationToken: cancellationToken);
    }

    public async Task<List<CommunityMember>> GetCommunityMembersAsync(
        string communityId, CancellationToken cancellationToken = default)
    {
        return await _context.CommunityMembers
            .Find(m => m.CommunityId == communityId && !m.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetMemberCountAsync(
        string communityId, CancellationToken cancellationToken = default)
    {
        return (int)await _context.CommunityMembers
            .CountDocumentsAsync(
                m => m.CommunityId == communityId && !m.IsDeleted,
                cancellationToken: cancellationToken);
    }

    public async Task<bool> IsMemberAsync(
        string communityId, string userId, CancellationToken cancellationToken = default)
    {
        return await _context.CommunityMembers
            .Find(m => m.CommunityId == communityId && m.UserId == userId && !m.IsDeleted)
            .AnyAsync(cancellationToken);
    }
}
```

`Whispra.Infrastructure/Persistence/Repositories/CommunityInviteRepository.cs`:

```csharp
using MongoDB.Driver;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Domain.Entities.Communities;
using Whispra.Infrastructure.Persistence.MongoDB;

namespace Whispra.Infrastructure.Persistence.Repositories;

public class CommunityInviteRepository : ICommunityInviteRepository
{
    private readonly MongoDbContext _context;

    public CommunityInviteRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<CommunityInvite?> GetByCodeAsync(
        string inviteCode, CancellationToken cancellationToken = default)
    {
        return await _context.CommunityInvites
            .Find(i => i.InviteCode == inviteCode && !i.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<CommunityInvite> CreateAsync(
        CommunityInvite invite, CancellationToken cancellationToken = default)
    {
        await _context.CommunityInvites.InsertOneAsync(invite, cancellationToken: cancellationToken);
        return invite;
    }

    public async Task UpdateAsync(CommunityInvite invite, CancellationToken cancellationToken = default)
    {
        invite.UpdatedAt = DateTime.UtcNow;
        await _context.CommunityInvites.ReplaceOneAsync(
            i => i.Id == invite.Id,
            invite,
            cancellationToken: cancellationToken);
    }

    public async Task<List<CommunityInvite>> GetCommunityInvitesAsync(
        string communityId, CancellationToken cancellationToken = default)
    {
        return await _context.CommunityInvites
            .Find(i => i.CommunityId == communityId && !i.IsDeleted && i.IsActive)
            .ToListAsync(cancellationToken);
    }
}
```

Update `Whispra.Infrastructure/Persistence/MongoDB/MongoDbContext.cs`:

```csharp
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Whispra.Domain.Entities.Auth;
using Whispra.Domain.Entities.Communities;
using Whispra.Domain.Entities.Users;
using Whispra.Infrastructure.Configuration;

namespace Whispra.Infrastructure.Persistence.MongoDB;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<User> Users => _database.GetCollection<User>("users");
    public IMongoCollection<RefreshToken> RefreshTokens => _database.GetCollection<RefreshToken>("refresh_tokens");
    public IMongoCollection<Community> Communities => _database.GetCollection<Community>("communities");
    public IMongoCollection<CommunityMember> CommunityMembers => _database.GetCollection<CommunityMember>("community_members");
    public IMongoCollection<CommunityInvite> CommunityInvites => _database.GetCollection<CommunityInvite>("community_invites");
}
```

**Technologies**: MongoDB queries, soft deletes, counting
**Study needed**: ✅ None, builds on MongoDB patterns from Phase 2

## Step 4.6: Create Communities Controller

```bash
cd backend/Whispra.Api/Controllers
```

`Whispra.Api/Controllers/CommunitiesController.cs`:

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Whispra.Application.DTOs.Communities;
using Whispra.Application.UseCases.Communities.Create;
using Whispra.Application.UseCases.Communities.CreateInvite;
using Whispra.Application.UseCases.Communities.Join;
using Whispra.Application.UseCases.Communities.Leave;
using Whispra.Application.UseCases.Communities.UpdateRole;

namespace Whispra.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CommunitiesController : ControllerBase
{
    private readonly CreateCommunityUseCase _createCommunityUseCase;
    private readonly JoinCommunityUseCase _joinCommunityUseCase;
    private readonly LeaveCommunityUseCase _leaveCommunityUseCase;
    private readonly UpdateMemberRoleUseCase _updateMemberRoleUseCase;
    private readonly CreateInviteUseCase _createInviteUseCase;

    public CommunitiesController(
        CreateCommunityUseCase createCommunityUseCase,
        JoinCommunityUseCase joinCommunityUseCase,
        LeaveCommunityUseCase leaveCommunityUseCase,
        UpdateMemberRoleUseCase updateMemberRoleUseCase,
        CreateInviteUseCase createInviteUseCase)
    {
        _createCommunityUseCase = createCommunityUseCase;
        _joinCommunityUseCase = joinCommunityUseCase;
        _leaveCommunityUseCase = leaveCommunityUseCase;
        _updateMemberRoleUseCase = updateMemberRoleUseCase;
        _createInviteUseCase = createInviteUseCase;
    }

    private string GetCurrentUserId()
    {
        return User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("User not authenticated");
    }

    [HttpPost]
    public async Task<ActionResult<CommunityResponseDto>> CreateCommunity(
        [FromBody] CreateCommunityDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _createCommunityUseCase.ExecuteAsync(
                dto, GetCurrentUserId(), cancellationToken);
            return CreatedAtAction(nameof(CreateCommunity), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{communityId}/join")]
    public async Task<ActionResult<CommunityResponseDto>> JoinCommunity(
        string communityId,
        [FromBody] JoinCommunityDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _joinCommunityUseCase.ExecuteAsync(
                communityId, GetCurrentUserId(), dto, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{communityId}/leave")]
    public async Task<ActionResult> LeaveCommunity(
        string communityId,
        CancellationToken cancellationToken)
    {
        try
        {
            await _leaveCommunityUseCase.ExecuteAsync(
                communityId, GetCurrentUserId(), cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{communityId}/members/role")]
    public async Task<ActionResult> UpdateMemberRole(
        string communityId,
        [FromBody] UpdateMemberRoleDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            await _updateMemberRoleUseCase.ExecuteAsync(
                communityId, dto, GetCurrentUserId(), cancellationToken);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{communityId}/invites")]
    public async Task<ActionResult<InviteResponseDto>> CreateInvite(
        string communityId,
        [FromBody] CreateInviteDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _createInviteUseCase.ExecuteAsync(
                communityId, dto, GetCurrentUserId(), cancellationToken);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
```

Update `Whispra.Api/Extensions/ServiceCollectionExtensions.cs` to register new use cases:

```csharp
public static IServiceCollection AddApplicationServices(this IServiceCollection services)
{
    // Register use cases - Users
    services.AddScoped<RegisterUserUseCase>();

    // Register use cases - Auth
    services.AddScoped<LoginUseCase>();
    services.AddScoped<RefreshTokenUseCase>();

    // Register use cases - Communities
    services.AddScoped<CreateCommunityUseCase>();
    services.AddScoped<JoinCommunityUseCase>();
    services.AddScoped<LeaveCommunityUseCase>();
    services.AddScoped<UpdateMemberRoleUseCase>();
    services.AddScoped<CreateInviteUseCase>();

    return services;
}

public static IServiceCollection AddInfrastructureServices(
    this IServiceCollection services,
    IConfiguration configuration)
{
    // Configure MongoDB
    services.Configure<MongoDbSettings>(
        configuration.GetSection(nameof(MongoDbSettings)));
    services.AddSingleton<MongoDbContext>();

    // Configure JWT
    services.Configure<JwtSettings>(
        configuration.GetSection(nameof(JwtSettings)));

    // Register repositories - Users
    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

    // Register repositories - Communities
    services.AddScoped<ICommunityRepository, CommunityRepository>();
    services.AddScoped<ICommunityMemberRepository, CommunityMemberRepository>();
    services.AddScoped<ICommunityInviteRepository, CommunityInviteRepository>();

    // Register services
    services.AddSingleton<IPasswordHasher, PasswordHasher>();
    services.AddSingleton<IJwtTokenService, JwtTokenService>();

    return services;
}
```

**Technologies**: Controller actions, HTTP verbs, authorization
**Study needed**: ✅ None, follows established patterns

## Step 4.7: Test Communities

```bash
# Run the API
cd backend/Whispra.Api
dotnet run
```

Open Swagger: `https://localhost:7001/swagger`

### Test Flow:

**1. Login** (get your access token):

```json
POST /api/Auth/login
{
  "email": "authtest@example.com",
  "password": "Test123456"
}
```

**2. Authorize in Swagger** with Bearer token

**3. Create a public community**:

```json
POST /api/Communities
{
  "name": "Tech Enthusiasts",
  "description": "A community for tech lovers",
  "privacy": 0,
  "tags": ["technology", "programming"]
}
```

**4. Create a private community**:

```json
POST /api/Communities
{
  "name": "Secret Club",
  "description": "Private group",
  "privacy": 1,
  "tags": ["private"]
}
```

**5. Create an invite for the private community**:

```json
POST /api/Communities/{communityId}/invites
{
  "maxUses": 10,
  "expiresInDays": 7
}
```

Save the `inviteCode` from the response!

**6. Register a second user** and login to get their token

**7. Try to join the public community** (as second user):

```json
POST /api/Communities/{publicCommunityId}/join
{}
```

**8. Try to join the private community with invite code**:

```json
POST /api/Communities/{privateCommunityId}/join
{
  "inviteCode": "ABC123XYZ"
}
```

**9. As the owner, promote the second user to moderator**:

```json
PUT /api/Communities/{communityId}/members/role
{
  "userId": "second-user-id",
  "newRole": 1
}
```

**10. Leave a community** (as second user):

```json
POST /api/Communities/{communityId}/leave
```

Check MongoDB Compass to see:

- `communities` collection
- `community_members` collection with roles
- `community_invites` collection

**Study needed**: ✅ Just testing

---

# PHASE 5: Posts & Feed (Content Creation & Discovery)

**Goal**: Implement post creation with different types (text/photo/video), comments, reactions, and feed system with visibility rules.

## Step 5.1: Understanding Posts & Feed Architecture

**What are Posts?**

- Posts can contain text, photos, or videos
- Can be posted to: User's profile, Community feed, or both
- Have **visibility rules**: Public, FollowersOnly, CommunityOnly, Private
- Support **comments** and **reactions** (like Instagram/Facebook)

**Feed System**:

- **Home Feed**: Posts from followed users + communities you're in
- **Community Feed**: Posts within a specific community
- **User Profile Feed**: Posts by a specific user (filtered by visibility)

**Key Entities**:

- **Post**: The content (text, media URLs, visibility, author, community)
- **Comment**: Comments on posts (nested comments supported)
- **Reaction**: Emoji reactions (❤️, 👍, 😂, etc.)

📚 **Study needed**: Feed architecture patterns (2-3 hours)

- Resource: https://www.infoq.com/articles/scaling-instagram-infrastructure/
- Resource: https://engineering.fb.com/2015/03/23/production-engineering/news-feed-fyi-bringing-people-closer-together/

## Step 5.2: Add Post Entities to Domain Layer

```bash
cd backend/Whispra.Domain

# Create folders
mkdir -p Entities/Posts
mkdir -p Enums
```

`Whispra.Domain/Enums/PostVisibility.cs`:

```csharp
namespace Whispra.Domain.Enums;

public enum PostVisibility
{
    Public = 0,           // Everyone can see
    FollowersOnly = 1,    // Only followers can see
    CommunityOnly = 2,    // Only community members can see (for community posts)
    Private = 3           // Only author can see (drafts)
}
```

`Whispra.Domain/Enums/PostType.cs`:

```csharp
namespace Whispra.Domain.Enums;

public enum PostType
{
    Text = 0,
    Photo = 1,
    Video = 2
}
```

`Whispra.Domain/Enums/ReactionType.cs`:

```csharp
namespace Whispra.Domain.Enums;

public enum ReactionType
{
    Like = 0,      // ❤️
    Love = 1,      // 😍
    Haha = 2,      // 😂
    Wow = 3,       // 😮
    Sad = 4,       // 😢
    Angry = 5      // 😡
}
```

`Whispra.Domain/Entities/Posts/Post.cs`:

```csharp
using Whispra.Domain.Enums;

namespace Whispra.Domain.Entities.Posts;

public class Post : BaseEntity
{
    public string AuthorId { get; set; } = string.Empty;
    public string? CommunityId { get; set; } // null = user profile post
    public PostType Type { get; set; } = PostType.Text;
    public string? TextContent { get; set; }
    public List<string> MediaUrls { get; set; } = new(); // Photo/video URLs
    public PostVisibility Visibility { get; set; } = PostVisibility.Public;
    public int CommentCount { get; set; } = 0;
    public int ReactionCount { get; set; } = 0;
    public Dictionary<string, int> ReactionCounts { get; set; } = new(); // {"Like": 10, "Love": 5}
}
```

`Whispra.Domain/Entities/Posts/Comment.cs`:

```csharp
namespace Whispra.Domain.Entities.Posts;

public class Comment : BaseEntity
{
    public string PostId { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? ParentCommentId { get; set; } // For nested replies
    public int ReplyCount { get; set; } = 0;
    public int ReactionCount { get; set; } = 0;
}
```

`Whispra.Domain/Entities/Posts/Reaction.cs`:

```csharp
using Whispra.Domain.Enums;

namespace Whispra.Domain.Entities.Posts;

public class Reaction : BaseEntity
{
    public string TargetId { get; set; } = string.Empty; // PostId or CommentId
    public string TargetType { get; set; } = string.Empty; // "Post" or "Comment"
    public string UserId { get; set; } = string.Empty;
    public ReactionType Type { get; set; } = ReactionType.Like;
}
```

**Technologies**: Enums, nested entities, dictionaries
**Study needed**:

- 📚 C# Dictionaries (1 hour)
  - Resource: https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2

## Step 5.3: Add Post DTOs and Interfaces to Application Layer

```bash
cd backend/Whispra.Application

# Create folders
mkdir -p DTOs/Posts
mkdir -p DTOs/Comments
mkdir -p DTOs/Reactions
mkdir -p UseCases/Posts/Create
mkdir -p UseCases/Posts/GetFeed
mkdir -p UseCases/Posts/Update
mkdir -p UseCases/Posts/Delete
mkdir -p UseCases/Comments/Create
mkdir -p UseCases/Reactions/Add
mkdir -p Interfaces/Repositories
```

`Whispra.Application/DTOs/Posts/CreatePostDto.cs`:

```csharp
using Whispra.Domain.Enums;

namespace Whispra.Application.DTOs.Posts;

public record CreatePostDto(
    string? TextContent,
    PostType Type,
    List<string>? MediaUrls,
    PostVisibility Visibility,
    string? CommunityId = null
);
```

`Whispra.Application/DTOs/Posts/UpdatePostDto.cs`:

```csharp
using Whispra.Domain.Enums;

namespace Whispra.Application.DTOs.Posts;

public record UpdatePostDto(
    string? TextContent,
    PostVisibility? Visibility
);
```

`Whispra.Application/DTOs/Posts/PostResponseDto.cs`:

```csharp
using Whispra.Domain.Enums;

namespace Whispra.Application.DTOs.Posts;

public record PostResponseDto(
    string Id,
    string AuthorId,
    string AuthorUsername,
    string? CommunityId,
    string? CommunityName,
    PostType Type,
    string? TextContent,
    List<string> MediaUrls,
    PostVisibility Visibility,
    int CommentCount,
    int ReactionCount,
    Dictionary<string, int> ReactionCounts,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    bool IsAuthor,
    string? CurrentUserReaction = null // "Like", "Love", etc.
);
```

`Whispra.Application/DTOs/Comments/CreateCommentDto.cs`:

```csharp
namespace Whispra.Application.DTOs.Comments;

public record CreateCommentDto(
    string Content,
    string? ParentCommentId = null
);
```

`Whispra.Application/DTOs/Comments/CommentResponseDto.cs`:

```csharp
namespace Whispra.Application.DTOs.Comments;

public record CommentResponseDto(
    string Id,
    string PostId,
    string AuthorId,
    string AuthorUsername,
    string Content,
    string? ParentCommentId,
    int ReplyCount,
    int ReactionCount,
    DateTime CreatedAt,
    bool IsAuthor,
    string? CurrentUserReaction = null
);
```

`Whispra.Application/DTOs/Reactions/AddReactionDto.cs`:

```csharp
using Whispra.Domain.Enums;

namespace Whispra.Application.DTOs.Reactions;

public record AddReactionDto(
    ReactionType Type
);
```

`Whispra.Application/Interfaces/Repositories/IPostRepository.cs`:

```csharp
using Whispra.Domain.Entities.Posts;

namespace Whispra.Application.Interfaces.Repositories;

public interface IPostRepository
{
    Task<Post?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Post> CreateAsync(Post post, CancellationToken cancellationToken = default);
    Task UpdateAsync(Post post, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<List<Post>> GetUserPostsAsync(string userId, int skip, int limit, CancellationToken cancellationToken = default);
    Task<List<Post>> GetCommunityPostsAsync(string communityId, int skip, int limit, CancellationToken cancellationToken = default);
    Task<List<Post>> GetHomeFeedAsync(string userId, List<string> followingUserIds, List<string> communityIds, int skip, int limit, CancellationToken cancellationToken = default);
}
```

`Whispra.Application/Interfaces/Repositories/ICommentRepository.cs`:

```csharp
using Whispra.Domain.Entities.Posts;

namespace Whispra.Application.Interfaces.Repositories;

public interface ICommentRepository
{
    Task<Comment?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Comment> CreateAsync(Comment comment, CancellationToken cancellationToken = default);
    Task UpdateAsync(Comment comment, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<List<Comment>> GetPostCommentsAsync(string postId, int skip, int limit, CancellationToken cancellationToken = default);
    Task<List<Comment>> GetCommentRepliesAsync(string parentCommentId, int skip, int limit, CancellationToken cancellationToken = default);
}
```

`Whispra.Application/Interfaces/Repositories/IReactionRepository.cs`:

```csharp
using Whispra.Domain.Entities.Posts;
using Whispra.Domain.Enums;

namespace Whispra.Application.Interfaces.Repositories;

public interface IReactionRepository
{
    Task<Reaction?> GetUserReactionAsync(string targetId, string userId, CancellationToken cancellationToken = default);
    Task<Reaction> CreateAsync(Reaction reaction, CancellationToken cancellationToken = default);
    Task UpdateAsync(Reaction reaction, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<Dictionary<string, int>> GetReactionCountsAsync(string targetId, CancellationToken cancellationToken = default);
}
```

**Technologies**: DTOs with complex types, feed queries
**Study needed**: ✅ None, follows established patterns

## Step 5.4: Create Post Use Cases

`Whispra.Application/UseCases/Posts/Create/CreatePostUseCase.cs`:

```csharp
using Whispra.Application.DTOs.Posts;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Domain.Entities.Posts;
using Whispra.Domain.Enums;

namespace Whispra.Application.UseCases.Posts.Create;

public class CreatePostUseCase
{
    private readonly IPostRepository _postRepository;
    private readonly ICommunityMemberRepository _communityMemberRepository;
    private readonly IUserRepository _userRepository;

    public CreatePostUseCase(
        IPostRepository postRepository,
        ICommunityMemberRepository communityMemberRepository,
        IUserRepository userRepository)
    {
        _postRepository = postRepository;
        _communityMemberRepository = communityMemberRepository;
        _userRepository = userRepository;
    }

    public async Task<PostResponseDto> ExecuteAsync(
        CreatePostDto dto,
        string currentUserId,
        CancellationToken cancellationToken = default)
    {
        // Validate content
        if (string.IsNullOrWhiteSpace(dto.TextContent) && (dto.MediaUrls == null || !dto.MediaUrls.Any()))
        {
            throw new InvalidOperationException("Post must contain text or media");
        }

        // If posting to community, verify membership
        if (!string.IsNullOrEmpty(dto.CommunityId))
        {
            var membership = await _communityMemberRepository.GetMembershipAsync(
                dto.CommunityId, currentUserId, cancellationToken);

            if (membership == null)
            {
                throw new UnauthorizedAccessException("You are not a member of this community");
            }

            // Community posts must be CommunityOnly visibility
            if (dto.Visibility != PostVisibility.CommunityOnly)
            {
                throw new InvalidOperationException("Community posts must have CommunityOnly visibility");
            }
        }

        // Create post
        var post = new Post
        {
            AuthorId = currentUserId,
            CommunityId = dto.CommunityId,
            Type = dto.Type,
            TextContent = dto.TextContent,
            MediaUrls = dto.MediaUrls ?? new List<string>(),
            Visibility = dto.Visibility
        };

        var createdPost = await _postRepository.CreateAsync(post, cancellationToken);

        // Get author username
        var author = await _userRepository.GetByIdAsync(currentUserId, cancellationToken);

        return new PostResponseDto(
            createdPost.Id,
            createdPost.AuthorId,
            author?.Username ?? "Unknown",
            createdPost.CommunityId,
            null, // CommunityName would need community lookup
            createdPost.Type,
            createdPost.TextContent,
            createdPost.MediaUrls,
            createdPost.Visibility,
            createdPost.CommentCount,
            createdPost.ReactionCount,
            createdPost.ReactionCounts,
            createdPost.CreatedAt,
            createdPost.UpdatedAt,
            true,
            null
        );
    }
}
```

`Whispra.Application/UseCases/Posts/Update/UpdatePostUseCase.cs`:

```csharp
using Whispra.Application.DTOs.Posts;
using Whispra.Application.Interfaces.Repositories;

namespace Whispra.Application.UseCases.Posts.Update;

public class UpdatePostUseCase
{
    private readonly IPostRepository _postRepository;

    public UpdatePostUseCase(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task ExecuteAsync(
        string postId,
        UpdatePostDto dto,
        string currentUserId,
        CancellationToken cancellationToken = default)
    {
        var post = await _postRepository.GetByIdAsync(postId, cancellationToken);
        if (post == null)
        {
            throw new InvalidOperationException("Post not found");
        }

        // Only author can update
        if (post.AuthorId != currentUserId)
        {
            throw new UnauthorizedAccessException("You can only edit your own posts");
        }

        // Update fields
        if (!string.IsNullOrEmpty(dto.TextContent))
        {
            post.TextContent = dto.TextContent;
        }

        if (dto.Visibility.HasValue)
        {
            post.Visibility = dto.Visibility.Value;
        }

        await _postRepository.UpdateAsync(post, cancellationToken);
    }
}
```

`Whispra.Application/UseCases/Posts/Delete/DeletePostUseCase.cs`:

```csharp
using Whispra.Application.Interfaces.Repositories;
using Whispra.Domain.Enums;

namespace Whispra.Application.UseCases.Posts.Delete;

public class DeletePostUseCase
{
    private readonly IPostRepository _postRepository;
    private readonly ICommunityMemberRepository _communityMemberRepository;

    public DeletePostUseCase(
        IPostRepository postRepository,
        ICommunityMemberRepository communityMemberRepository)
    {
        _postRepository = postRepository;
        _communityMemberRepository = communityMemberRepository;
    }

    public async Task ExecuteAsync(
        string postId,
        string currentUserId,
        CancellationToken cancellationToken = default)
    {
        var post = await _postRepository.GetByIdAsync(postId, cancellationToken);
        if (post == null)
        {
            throw new InvalidOperationException("Post not found");
        }

        // Check permissions: author OR community moderator/owner
        bool canDelete = post.AuthorId == currentUserId;

        if (!canDelete && !string.IsNullOrEmpty(post.CommunityId))
        {
            var membership = await _communityMemberRepository.GetMembershipAsync(
                post.CommunityId, currentUserId, cancellationToken);

            if (membership != null &&
                (membership.Role == CommunityRole.Owner || membership.Role == CommunityRole.Moderator))
            {
                canDelete = true;
            }
        }

        if (!canDelete)
        {
            throw new UnauthorizedAccessException("You don't have permission to delete this post");
        }

        await _postRepository.DeleteAsync(postId, cancellationToken);
    }
}
```

`Whispra.Application/UseCases/Posts/GetFeed/GetHomeFeedUseCase.cs`:

```csharp
using Whispra.Application.DTOs.Posts;
using Whispra.Application.Interfaces.Repositories;

namespace Whispra.Application.UseCases.Posts.GetFeed;

public class GetHomeFeedUseCase
{
    private readonly IPostRepository _postRepository;
    private readonly ICommunityMemberRepository _communityMemberRepository;
    private readonly IUserRepository _userRepository;
    private readonly IReactionRepository _reactionRepository;

    public GetHomeFeedUseCase(
        IPostRepository postRepository,
        ICommunityMemberRepository communityMemberRepository,
        IUserRepository userRepository,
        IReactionRepository reactionRepository)
    {
        _postRepository = postRepository;
        _communityMemberRepository = communityMemberRepository;
        _userRepository = userRepository;
        _reactionRepository = reactionRepository;
    }

    public async Task<List<PostResponseDto>> ExecuteAsync(
        string currentUserId,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        // TODO: In a real app, you'd have a "Follow" relationship
        // For now, we'll just get posts from communities the user is in
        var userMemberships = await _communityMemberRepository.GetCommunityMembersAsync(
            currentUserId, cancellationToken);
        var communityIds = userMemberships.Select(m => m.CommunityId).ToList();

        var skip = (page - 1) * pageSize;
        var posts = await _postRepository.GetHomeFeedAsync(
            currentUserId, new List<string>(), communityIds, skip, pageSize, cancellationToken);

        var response = new List<PostResponseDto>();

        foreach (var post in posts)
        {
            var author = await _userRepository.GetByIdAsync(post.AuthorId, cancellationToken);
            var userReaction = await _reactionRepository.GetUserReactionAsync(
                post.Id, currentUserId, cancellationToken);

            response.Add(new PostResponseDto(
                post.Id,
                post.AuthorId,
                author?.Username ?? "Unknown",
                post.CommunityId,
                null,
                post.Type,
                post.TextContent,
                post.MediaUrls,
                post.Visibility,
                post.CommentCount,
                post.ReactionCount,
                post.ReactionCounts,
                post.CreatedAt,
                post.UpdatedAt,
                post.AuthorId == currentUserId,
                userReaction?.Type.ToString()
            ));
        }

        return response;
    }
}
```

`Whispra.Application/UseCases/Comments/Create/CreateCommentUseCase.cs`:

```csharp
using Whispra.Application.DTOs.Comments;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Domain.Entities.Posts;

namespace Whispra.Application.UseCases.Comments.Create;

public class CreateCommentUseCase
{
    private readonly IPostRepository _postRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly IUserRepository _userRepository;

    public CreateCommentUseCase(
        IPostRepository postRepository,
        ICommentRepository commentRepository,
        IUserRepository userRepository)
    {
        _postRepository = postRepository;
        _commentRepository = commentRepository;
        _userRepository = userRepository;
    }

    public async Task<CommentResponseDto> ExecuteAsync(
        string postId,
        CreateCommentDto dto,
        string currentUserId,
        CancellationToken cancellationToken = default)
    {
        var post = await _postRepository.GetByIdAsync(postId, cancellationToken);
        if (post == null)
        {
            throw new InvalidOperationException("Post not found");
        }

        // TODO: Add visibility check (can user see this post?)

        // If replying to a comment, verify parent exists
        if (!string.IsNullOrEmpty(dto.ParentCommentId))
        {
            var parentComment = await _commentRepository.GetByIdAsync(
                dto.ParentCommentId, cancellationToken);
            if (parentComment == null || parentComment.PostId != postId)
            {
                throw new InvalidOperationException("Parent comment not found");
            }

            // Increment parent reply count
            parentComment.ReplyCount++;
            await _commentRepository.UpdateAsync(parentComment, cancellationToken);
        }

        var comment = new Comment
        {
            PostId = postId,
            AuthorId = currentUserId,
            Content = dto.Content,
            ParentCommentId = dto.ParentCommentId
        };

        var createdComment = await _commentRepository.CreateAsync(comment, cancellationToken);

        // Increment post comment count
        post.CommentCount++;
        await _postRepository.UpdateAsync(post, cancellationToken);

        var author = await _userRepository.GetByIdAsync(currentUserId, cancellationToken);

        return new CommentResponseDto(
            createdComment.Id,
            createdComment.PostId,
            createdComment.AuthorId,
            author?.Username ?? "Unknown",
            createdComment.Content,
            createdComment.ParentCommentId,
            createdComment.ReplyCount,
            createdComment.ReactionCount,
            createdComment.CreatedAt,
            true,
            null
        );
    }
}
```

`Whispra.Application/UseCases/Reactions/Add/AddReactionUseCase.cs`:

```csharp
using Whispra.Application.DTOs.Reactions;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Domain.Entities.Posts;

namespace Whispra.Application.UseCases.Reactions.Add;

public class AddReactionUseCase
{
    private readonly IPostRepository _postRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly IReactionRepository _reactionRepository;

    public AddReactionUseCase(
        IPostRepository postRepository,
        ICommentRepository commentRepository,
        IReactionRepository reactionRepository)
    {
        _postRepository = postRepository;
        _commentRepository = commentRepository;
        _reactionRepository = reactionRepository;
    }

    public async Task ExecuteAsync(
        string targetId,
        string targetType,
        AddReactionDto dto,
        string currentUserId,
        CancellationToken cancellationToken = default)
    {
        // Validate target exists
        if (targetType == "Post")
        {
            var post = await _postRepository.GetByIdAsync(targetId, cancellationToken);
            if (post == null)
            {
                throw new InvalidOperationException("Post not found");
            }
        }
        else if (targetType == "Comment")
        {
            var comment = await _commentRepository.GetByIdAsync(targetId, cancellationToken);
            if (comment == null)
            {
                throw new InvalidOperationException("Comment not found");
            }
        }
        else
        {
            throw new InvalidOperationException("Invalid target type");
        }

        // Check if user already reacted
        var existingReaction = await _reactionRepository.GetUserReactionAsync(
            targetId, currentUserId, cancellationToken);

        if (existingReaction != null)
        {
            // Update reaction type
            existingReaction.Type = dto.Type;
            await _reactionRepository.UpdateAsync(existingReaction, cancellationToken);
        }
        else
        {
            // Create new reaction
            var reaction = new Reaction
            {
                TargetId = targetId,
                TargetType = targetType,
                UserId = currentUserId,
                Type = dto.Type
            };

            await _reactionRepository.CreateAsync(reaction, cancellationToken);

            // Increment reaction count
            if (targetType == "Post")
            {
                var post = await _postRepository.GetByIdAsync(targetId, cancellationToken);
                if (post != null)
                {
                    post.ReactionCount++;
                    var reactionKey = dto.Type.ToString();
                    if (post.ReactionCounts.ContainsKey(reactionKey))
                    {
                        post.ReactionCounts[reactionKey]++;
                    }
                    else
                    {
                        post.ReactionCounts[reactionKey] = 1;
                    }
                    await _postRepository.UpdateAsync(post, cancellationToken);
                }
            }
            else if (targetType == "Comment")
            {
                var comment = await _commentRepository.GetByIdAsync(targetId, cancellationToken);
                if (comment != null)
                {
                    comment.ReactionCount++;
                    await _commentRepository.UpdateAsync(comment, cancellationToken);
                }
            }
        }
    }
}
```

**Technologies**: Feed algorithms, nested comments, reaction aggregation
**Study needed**:

- 📚 Feed ranking basics (2-3 hours) - Optional for now, we're doing chronological
  - Resource: https://www.reforge.com/blog/how-the-facebook-news-feed-works

## Step 5.5: Implement Infrastructure Layer for Posts

```bash
cd backend/Whispra.Infrastructure/Persistence/Repositories
```

`Whispra.Infrastructure/Persistence/Repositories/PostRepository.cs`:

```csharp
using MongoDB.Driver;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Domain.Entities.Posts;
using Whispra.Domain.Enums;
using Whispra.Infrastructure.Persistence.MongoDB;

namespace Whispra.Infrastructure.Persistence.Repositories;

public class PostRepository : IPostRepository
{
    private readonly MongoDbContext _context;

    public PostRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<Post?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.Posts
            .Find(p => p.Id == id && !p.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Post> CreateAsync(Post post, CancellationToken cancellationToken = default)
    {
        await _context.Posts.InsertOneAsync(post, cancellationToken: cancellationToken);
        return post;
    }

    public async Task UpdateAsync(Post post, CancellationToken cancellationToken = default)
    {
        post.UpdatedAt = DateTime.UtcNow;
        await _context.Posts.ReplaceOneAsync(
            p => p.Id == post.Id,
            post,
            cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var update = Builders<Post>.Update
            .Set(p => p.IsDeleted, true)
            .Set(p => p.DeletedAt, DateTime.UtcNow);

        await _context.Posts.UpdateOneAsync(
            p => p.Id == id,
            update,
            cancellationToken: cancellationToken);
    }

    public async Task<List<Post>> GetUserPostsAsync(
        string userId, int skip, int limit, CancellationToken cancellationToken = default)
    {
        return await _context.Posts
            .Find(p => p.AuthorId == userId && !p.IsDeleted)
            .SortByDescending(p => p.CreatedAt)
            .Skip(skip)
            .Limit(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Post>> GetCommunityPostsAsync(
        string communityId, int skip, int limit, CancellationToken cancellationToken = default)
    {
        return await _context.Posts
            .Find(p => p.CommunityId == communityId && !p.IsDeleted)
            .SortByDescending(p => p.CreatedAt)
            .Skip(skip)
            .Limit(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Post>> GetHomeFeedAsync(
        string userId,
        List<string> followingUserIds,
        List<string> communityIds,
        int skip,
        int limit,
        CancellationToken cancellationToken = default)
    {
        // Get posts from:
        // 1. User's own posts
        // 2. Posts from followed users (when we add follow feature)
        // 3. Posts from communities user is in
        var filter = Builders<Post>.Filter.And(
            Builders<Post>.Filter.Eq(p => p.IsDeleted, false),
            Builders<Post>.Filter.Or(
                Builders<Post>.Filter.Eq(p => p.AuthorId, userId),
                Builders<Post>.Filter.In(p => p.CommunityId, communityIds)
            )
        );

        return await _context.Posts
            .Find(filter)
            .SortByDescending(p => p.CreatedAt)
            .Skip(skip)
            .Limit(limit)
            .ToListAsync(cancellationToken);
    }
}
```

`Whispra.Infrastructure/Persistence/Repositories/CommentRepository.cs`:

```csharp
using MongoDB.Driver;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Domain.Entities.Posts;
using Whispra.Infrastructure.Persistence.MongoDB;

namespace Whispra.Infrastructure.Persistence.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly MongoDbContext _context;

    public CommentRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<Comment?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.Comments
            .Find(c => c.Id == id && !c.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Comment> CreateAsync(Comment comment, CancellationToken cancellationToken = default)
    {
        await _context.Comments.InsertOneAsync(comment, cancellationToken: cancellationToken);
        return comment;
    }

    public async Task UpdateAsync(Comment comment, CancellationToken cancellationToken = default)
    {
        comment.UpdatedAt = DateTime.UtcNow;
        await _context.Comments.ReplaceOneAsync(
            c => c.Id == comment.Id,
            comment,
            cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var update = Builders<Comment>.Update
            .Set(c => c.IsDeleted, true)
            .Set(c => c.DeletedAt, DateTime.UtcNow);

        await _context.Comments.UpdateOneAsync(
            c => c.Id == id,
            update,
            cancellationToken: cancellationToken);
    }

    public async Task<List<Comment>> GetPostCommentsAsync(
        string postId, int skip, int limit, CancellationToken cancellationToken = default)
    {
        return await _context.Comments
            .Find(c => c.PostId == postId && c.ParentCommentId == null && !c.IsDeleted)
            .SortByDescending(c => c.CreatedAt)
            .Skip(skip)
            .Limit(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Comment>> GetCommentRepliesAsync(
        string parentCommentId, int skip, int limit, CancellationToken cancellationToken = default)
    {
        return await _context.Comments
            .Find(c => c.ParentCommentId == parentCommentId && !c.IsDeleted)
            .SortBy(c => c.CreatedAt)
            .Skip(skip)
            .Limit(limit)
            .ToListAsync(cancellationToken);
    }
}
```

`Whispra.Infrastructure/Persistence/Repositories/ReactionRepository.cs`:

```csharp
using MongoDB.Driver;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Domain.Entities.Posts;
using Whispra.Infrastructure.Persistence.MongoDB;

namespace Whispra.Infrastructure.Persistence.Repositories;

public class ReactionRepository : IReactionRepository
{
    private readonly MongoDbContext _context;

    public ReactionRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<Reaction?> GetUserReactionAsync(
        string targetId, string userId, CancellationToken cancellationToken = default)
    {
        return await _context.Reactions
            .Find(r => r.TargetId == targetId && r.UserId == userId && !r.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Reaction> CreateAsync(Reaction reaction, CancellationToken cancellationToken = default)
    {
        await _context.Reactions.InsertOneAsync(reaction, cancellationToken: cancellationToken);
        return reaction;
    }

    public async Task UpdateAsync(Reaction reaction, CancellationToken cancellationToken = default)
    {
        reaction.UpdatedAt = DateTime.UtcNow;
        await _context.Reactions.ReplaceOneAsync(
            r => r.Id == reaction.Id,
            reaction,
            cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var update = Builders<Reaction>.Update
            .Set(r => r.IsDeleted, true)
            .Set(r => r.DeletedAt, DateTime.UtcNow);

        await _context.Reactions.UpdateOneAsync(
            r => r.Id == id,
            update,
            cancellationToken: cancellationToken);
    }

    public async Task<Dictionary<string, int>> GetReactionCountsAsync(
        string targetId, CancellationToken cancellationToken = default)
    {
        var reactions = await _context.Reactions
            .Find(r => r.TargetId == targetId && !r.IsDeleted)
            .ToListAsync(cancellationToken);

        return reactions
            .GroupBy(r => r.Type.ToString())
            .ToDictionary(g => g.Key, g => g.Count());
    }
}
```

Update `Whispra.Infrastructure/Persistence/MongoDB/MongoDbContext.cs`:

```csharp
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Whispra.Domain.Entities.Auth;
using Whispra.Domain.Entities.Communities;
using Whispra.Domain.Entities.Posts;
using Whispra.Domain.Entities.Users;
using Whispra.Infrastructure.Configuration;

namespace Whispra.Infrastructure.Persistence.MongoDB;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<User> Users => _database.GetCollection<User>("users");
    public IMongoCollection<RefreshToken> RefreshTokens => _database.GetCollection<RefreshToken>("refresh_tokens");
    public IMongoCollection<Community> Communities => _database.GetCollection<Community>("communities");
    public IMongoCollection<CommunityMember> CommunityMembers => _database.GetCollection<CommunityMember>("community_members");
    public IMongoCollection<CommunityInvite> CommunityInvites => _database.GetCollection<CommunityInvite>("community_invites");
    public IMongoCollection<Post> Posts => _database.GetCollection<Post>("posts");
    public IMongoCollection<Comment> Comments => _database.GetCollection<Comment>("comments");
    public IMongoCollection<Reaction> Reactions => _database.GetCollection<Reaction>("reactions");
}
```

**Technologies**: MongoDB aggregation (basic), filtering, sorting
**Study needed**: ✅ None, builds on previous MongoDB knowledge

## Step 5.6: Create Posts Controller

```bash
cd backend/Whispra.Api/Controllers
```

`Whispra.Api/Controllers/PostsController.cs`:

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Whispra.Application.DTOs.Comments;
using Whispra.Application.DTOs.Posts;
using Whispra.Application.DTOs.Reactions;
using Whispra.Application.UseCases.Comments.Create;
using Whispra.Application.UseCases.Posts.Create;
using Whispra.Application.UseCases.Posts.Delete;
using Whispra.Application.UseCases.Posts.GetFeed;
using Whispra.Application.UseCases.Posts.Update;
using Whispra.Application.UseCases.Reactions.Add;

namespace Whispra.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly CreatePostUseCase _createPostUseCase;
    private readonly UpdatePostUseCase _updatePostUseCase;
    private readonly DeletePostUseCase _deletePostUseCase;
    private readonly GetHomeFeedUseCase _getHomeFeedUseCase;
    private readonly CreateCommentUseCase _createCommentUseCase;
    private readonly AddReactionUseCase _addReactionUseCase;

    public PostsController(
        CreatePostUseCase createPostUseCase,
        UpdatePostUseCase updatePostUseCase,
        DeletePostUseCase deletePostUseCase,
        GetHomeFeedUseCase getHomeFeedUseCase,
        CreateCommentUseCase createCommentUseCase,
        AddReactionUseCase addReactionUseCase)
    {
        _createPostUseCase = createPostUseCase;
        _updatePostUseCase = updatePostUseCase;
        _deletePostUseCase = deletePostUseCase;
        _getHomeFeedUseCase = getHomeFeedUseCase;
        _createCommentUseCase = createCommentUseCase;
        _addReactionUseCase = addReactionUseCase;
    }

    private string GetCurrentUserId()
    {
        return User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("User not authenticated");
    }

    [HttpPost]
    public async Task<ActionResult<PostResponseDto>> CreatePost(
        [FromBody] CreatePostDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _createPostUseCase.ExecuteAsync(
                dto, GetCurrentUserId(), cancellationToken);
            return CreatedAtAction(nameof(CreatePost), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [HttpPut("{postId}")]
    public async Task<ActionResult> UpdatePost(
        string postId,
        [FromBody] UpdatePostDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            await _updatePostUseCase.ExecuteAsync(
                postId, dto, GetCurrentUserId(), cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [HttpDelete("{postId}")]
    public async Task<ActionResult> DeletePost(
        string postId,
        CancellationToken cancellationToken)
    {
        try
        {
            await _deletePostUseCase.ExecuteAsync(
                postId, GetCurrentUserId(), cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [HttpGet("feed")]
    public async Task<ActionResult<List<PostResponseDto>>> GetHomeFeed(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _getHomeFeedUseCase.ExecuteAsync(
                GetCurrentUserId(), page, pageSize, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{postId}/comments")]
    public async Task<ActionResult<CommentResponseDto>> CreateComment(
        string postId,
        [FromBody] CreateCommentDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _createCommentUseCase.ExecuteAsync(
                postId, dto, GetCurrentUserId(), cancellationToken);
            return CreatedAtAction(nameof(CreateComment), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{postId}/reactions")]
    public async Task<ActionResult> AddReactionToPost(
        string postId,
        [FromBody] AddReactionDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            await _addReactionUseCase.ExecuteAsync(
                postId, "Post", dto, GetCurrentUserId(), cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
```

Update `Whispra.Api/Extensions/ServiceCollectionExtensions.cs`:

```csharp
public static IServiceCollection AddApplicationServices(this IServiceCollection services)
{
    // Register use cases - Users
    services.AddScoped<RegisterUserUseCase>();

    // Register use cases - Auth
    services.AddScoped<LoginUseCase>();
    services.AddScoped<RefreshTokenUseCase>();

    // Register use cases - Communities
    services.AddScoped<CreateCommunityUseCase>();
    services.AddScoped<JoinCommunityUseCase>();
    services.AddScoped<LeaveCommunityUseCase>();
    services.AddScoped<UpdateMemberRoleUseCase>();
    services.AddScoped<CreateInviteUseCase>();

    // Register use cases - Posts
    services.AddScoped<CreatePostUseCase>();
    services.AddScoped<UpdatePostUseCase>();
    services.AddScoped<DeletePostUseCase>();
    services.AddScoped<GetHomeFeedUseCase>();
    services.AddScoped<CreateCommentUseCase>();
    services.AddScoped<AddReactionUseCase>();

    return services;
}

public static IServiceCollection AddInfrastructureServices(
    this IServiceCollection services,
    IConfiguration configuration)
{
    // Configure MongoDB
    services.Configure<MongoDbSettings>(
        configuration.GetSection(nameof(MongoDbSettings)));
    services.AddSingleton<MongoDbContext>();

    // Configure JWT
    services.Configure<JwtSettings>(
        configuration.GetSection(nameof(JwtSettings)));

    // Register repositories - Users
    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

    // Register repositories - Communities
    services.AddScoped<ICommunityRepository, CommunityRepository>();
    services.AddScoped<ICommunityMemberRepository, CommunityMemberRepository>();
    services.AddScoped<ICommunityInviteRepository, CommunityInviteRepository>();

    // Register repositories - Posts
    services.AddScoped<IPostRepository, PostRepository>();
    services.AddScoped<ICommentRepository, CommentRepository>();
    services.AddScoped<IReactionRepository, ReactionRepository>();

    // Register services
    services.AddSingleton<IPasswordHasher, PasswordHasher>();
    services.AddSingleton<IJwtTokenService, JwtTokenService>();

    return services;
}
```

**Technologies**: RESTful API design, query parameters
**Study needed**: ✅ None, follows established patterns

## Step 5.7: Test Posts & Feed

```bash
# Run the API
cd backend/Whispra.Api
dotnet run
```

Open Swagger: `https://localhost:7001/swagger`

### Test Flow:

**1. Login and authorize**

**2. Create a text post**:

```json
POST /api/Posts
{
  "textContent": "Hello Whispra! This is my first post.",
  "type": 0,
  "mediaUrls": [],
  "visibility": 0
}
```

**3. Create a post in a community**:

```json
POST /api/Posts
{
  "textContent": "Posting in Tech Enthusiasts community!",
  "type": 0,
  "mediaUrls": [],
  "visibility": 2,
  "communityId": "your-community-id"
}
```

**4. Get home feed**:

```
GET /api/Posts/feed?page=1&pageSize=20
```

**5. Add a comment to a post**:

```json
POST /api/Posts/{postId}/comments
{
  "content": "Great post! I agree completely.",
  "parentCommentId": null
}
```

**6. Reply to a comment**:

```json
POST /api/Posts/{postId}/comments
{
  "content": "Thanks for your comment!",
  "parentCommentId": "first-comment-id"
}
```

**7. Add a reaction to a post**:

```json
POST /api/Posts/{postId}/reactions
{
  "type": 0
}
```

Types: 0=Like, 1=Love, 2=Haha, 3=Wow, 4=Sad, 5=Angry

**8. Update a post**:

```json
PUT /api/Posts/{postId}
{
  "textContent": "Updated post content!",
  "visibility": 1
}
```

**9. Delete a post**:

```
DELETE /api/Posts/{postId}
```

Check MongoDB Compass to see:

- `posts` collection with reactions counts
- `comments` collection with nested replies
- `reactions` collection

**Study needed**: ✅ Just testing

---

# PHASE 6: Media Upload (S3-Compatible Object Storage)

**Goal**: Implement secure media file uploads for images and videos using S3-compatible storage with presigned URLs, file validation, and metadata management.

## Step 6.1: Understanding Media Upload Architecture

**Why S3-Compatible Storage?**

- **Scalable**: Can store millions of files without performance degradation
- **Cost-effective**: Pay only for storage used
- **Distributed**: Files served from CDN/edge locations worldwide
- **Secure**: Presigned URLs provide temporary, scoped access
- **Compatible**: Works with AWS S3, Azure Blob, Google Cloud Storage, MinIO (local dev)

**How Presigned URLs Work**:

1. Frontend requests upload URL from backend
2. Backend generates a **presigned URL** (valid for 5-10 minutes)
3. Frontend uploads file **directly to S3** (not through backend!)
4. Frontend sends file metadata to backend
5. Backend stores metadata in MongoDB

**Benefits**:

- ✅ Backend doesn't handle large files (saves bandwidth/memory)
- ✅ Upload happens directly to S3 (faster for users)
- ✅ Temporary URLs expire automatically (security)

📚 **Study needed**: S3 concepts and presigned URLs (2-3 hours)

- Resource: https://docs.aws.amazon.com/AmazonS3/latest/userguide/PresignedUrlUploadObject.html
- Resource: https://min.io/docs/minio/linux/developers/dotnet/API.html

## Step 6.2: Set Up MinIO for Local Development

We'll use **MinIO** (S3-compatible storage) for local dev. In production, you'd use AWS S3, Azure Blob, or Google Cloud Storage.

Update `docker-compose.yml` in the root:

```yaml
version: "3.8"

services:
  mongodb:
    image: mongo:7
    container_name: whispra-mongodb
    restart: unless-stopped
    environment:
      MONGO_INITDB_ROOT_USERNAME: admin
      MONGO_INITDB_ROOT_PASSWORD: devpassword123
    ports:
      - "27017:27017"
    volumes:
      - mongodb_data:/data/db
    networks:
      - whispra-network

  redis:
    image: redis:7-alpine
    container_name: whispra-redis
    restart: unless-stopped
    command: redis-server --requirepass devredispassword123
    ports:
      - "6379:6379"
    volumes:
      - redis_data:/data
    networks:
      - whispra-network

  minio:
    image: minio/minio:latest
    container_name: whispra-minio
    restart: unless-stopped
    environment:
      MINIO_ROOT_USER: minioadmin
      MINIO_ROOT_PASSWORD: minioadmin123
    ports:
      - "9000:9000" # API port
      - "9001:9001" # Console UI port
    volumes:
      - minio_data:/data
    command: server /data --console-address ":9001"
    networks:
      - whispra-network

volumes:
  mongodb_data:
  redis_data:
  minio_data:

networks:
  whispra-network:
    driver: bridge
```

**Start MinIO**:

```bash
# Restart Docker Compose with MinIO
docker compose up -d

# Check all services are running
docker compose ps
```

**Access MinIO Console**:

1. Open browser: `http://localhost:9001`
2. Login:
   - Username: `minioadmin`
   - Password: `minioadmin123`
3. Create a bucket named `whispra-media`
4. Set bucket to **public read** (for this tutorial; in production use CloudFront/CDN)

**Technologies**: MinIO, S3 API, Docker Compose
**Study needed**:

- 📚 MinIO basics (1-2 hours)
  - Resource: https://min.io/docs/minio/linux/index.html

## Step 6.3: Add Media Entity to Domain Layer

```bash
cd backend/Whispra.Domain

# Create folder
mkdir -p Entities/Media
```

`Whispra.Domain/Enums/MediaType.cs`:

```csharp
namespace Whispra.Domain.Enums;

public enum MediaType
{
    Image = 0,
    Video = 1
}
```

`Whispra.Domain/Entities/Media/Media.cs`:

```csharp
using Whispra.Domain.Enums;

namespace Whispra.Domain.Entities.Media;

public class Media : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string S3Key { get; set; } = string.Empty; // Path in S3 bucket
    public string Url { get; set; } = string.Empty; // Public URL to access file
    public MediaType Type { get; set; }
    public long FileSizeBytes { get; set; }
    public string? MimeType { get; set; }
    public int? Width { get; set; } // For images
    public int? Height { get; set; } // For images
    public int? DurationSeconds { get; set; } // For videos
    public string? ThumbnailUrl { get; set; } // For videos
    public bool IsProcessed { get; set; } = false; // For async processing (thumbnails, transcoding)
}
```

**Technologies**: Media metadata, S3 keys
**Study needed**: ✅ None, straightforward entity

## Step 6.4: Add Media DTOs and Interfaces to Application Layer

```bash
cd backend/Whispra.Application

# Create folders
mkdir -p DTOs/Media
mkdir -p UseCases/Media/GetUploadUrl
mkdir -p UseCases/Media/ConfirmUpload
mkdir -p UseCases/Media/Delete
mkdir -p Interfaces/Services
mkdir -p Interfaces/Repositories
```

`Whispra.Application/DTOs/Media/GetUploadUrlDto.cs`:

```csharp
using Whispra.Domain.Enums;

namespace Whispra.Application.DTOs.Media;

public record GetUploadUrlDto(
    string FileName,
    MediaType Type,
    long FileSizeBytes,
    string MimeType
);
```

`Whispra.Application/DTOs/Media/UploadUrlResponseDto.cs`:

```csharp
namespace Whispra.Application.DTOs.Media;

public record UploadUrlResponseDto(
    string UploadUrl,       // Presigned URL for uploading
    string MediaId,         // ID to use when confirming upload
    string S3Key,           // S3 key (path in bucket)
    DateTime ExpiresAt      // When presigned URL expires
);
```

`Whispra.Application/DTOs/Media/ConfirmUploadDto.cs`:

```csharp
namespace Whispra.Application.DTOs.Media;

public record ConfirmUploadDto(
    string MediaId,
    int? Width = null,      // For images
    int? Height = null      // For images
);
```

`Whispra.Application/DTOs/Media/MediaResponseDto.cs`:

```csharp
using Whispra.Domain.Enums;

namespace Whispra.Application.DTOs.Media;

public record MediaResponseDto(
    string Id,
    string FileName,
    string Url,
    MediaType Type,
    long FileSizeBytes,
    string? MimeType,
    int? Width,
    int? Height,
    int? DurationSeconds,
    string? ThumbnailUrl,
    bool IsProcessed,
    DateTime CreatedAt
);
```

`Whispra.Application/Interfaces/Services/IS3Service.cs`:

```csharp
namespace Whispra.Application.Interfaces.Services;

public interface IS3Service
{
    Task<string> GeneratePresignedUploadUrlAsync(
        string s3Key,
        string contentType,
        int expirationMinutes = 10);

    Task<string> GetPublicUrlAsync(string s3Key);

    Task<bool> DeleteFileAsync(string s3Key);
}
```

`Whispra.Application/Interfaces/Repositories/IMediaRepository.cs`:

```csharp
using Whispra.Domain.Entities.Media;

namespace Whispra.Application.Interfaces.Repositories;

public interface IMediaRepository
{
    Task<Media?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Media> CreateAsync(Media media, CancellationToken cancellationToken = default);
    Task UpdateAsync(Media media, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<List<Media>> GetUserMediaAsync(string userId, int skip, int limit, CancellationToken cancellationToken = default);
}
```

**Technologies**: Presigned URLs, file metadata
**Study needed**: ✅ None, builds on previous patterns

## Step 6.5: Create Media Use Cases

`Whispra.Application/UseCases/Media/GetUploadUrl/GetUploadUrlUseCase.cs`:

```csharp
using Whispra.Application.DTOs.Media;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Application.Interfaces.Services;
using Whispra.Domain.Entities.Media;

namespace Whispra.Application.UseCases.Media.GetUploadUrl;

public class GetUploadUrlUseCase
{
    private readonly IMediaRepository _mediaRepository;
    private readonly IS3Service _s3Service;

    public GetUploadUrlUseCase(
        IMediaRepository mediaRepository,
        IS3Service s3Service)
    {
        _mediaRepository = mediaRepository;
        _s3Service = s3Service;
    }

    public async Task<UploadUrlResponseDto> ExecuteAsync(
        GetUploadUrlDto dto,
        string currentUserId,
        CancellationToken cancellationToken = default)
    {
        // Validate file size (max 100MB for images, 500MB for videos)
        var maxSizeBytes = dto.Type == Domain.Enums.MediaType.Image
            ? 100 * 1024 * 1024  // 100 MB
            : 500 * 1024 * 1024; // 500 MB

        if (dto.FileSizeBytes > maxSizeBytes)
        {
            throw new InvalidOperationException(
                $"File size exceeds maximum allowed ({maxSizeBytes / 1024 / 1024} MB)");
        }

        // Validate MIME type
        var allowedImageTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
        var allowedVideoTypes = new[] { "video/mp4", "video/quicktime", "video/webm" };

        var allowedTypes = dto.Type == Domain.Enums.MediaType.Image
            ? allowedImageTypes
            : allowedVideoTypes;

        if (!allowedTypes.Contains(dto.MimeType?.ToLowerInvariant()))
        {
            throw new InvalidOperationException(
                $"File type not allowed. Allowed types: {string.Join(", ", allowedTypes)}");
        }

        // Generate S3 key: {userId}/{type}/{timestamp}_{guid}_{filename}
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var guid = Guid.NewGuid().ToString("N")[..8]; // Short GUID
        var typeFolder = dto.Type == Domain.Enums.MediaType.Image ? "images" : "videos";
        var sanitizedFileName = Path.GetFileNameWithoutExtension(dto.FileName)
            .Replace(" ", "_")
            .Replace("-", "_");
        var extension = Path.GetExtension(dto.FileName);
        var s3Key = $"{currentUserId}/{typeFolder}/{timestamp}_{guid}_{sanitizedFileName}{extension}";

        // Create media metadata record (not yet uploaded)
        var media = new Domain.Entities.Media.Media
        {
            UserId = currentUserId,
            FileName = dto.FileName,
            S3Key = s3Key,
            Url = await _s3Service.GetPublicUrlAsync(s3Key),
            Type = dto.Type,
            FileSizeBytes = dto.FileSizeBytes,
            MimeType = dto.MimeType,
            IsProcessed = false
        };

        var createdMedia = await _mediaRepository.CreateAsync(media, cancellationToken);

        // Generate presigned upload URL (valid for 10 minutes)
        var uploadUrl = await _s3Service.GeneratePresignedUploadUrlAsync(
            s3Key,
            dto.MimeType,
            expirationMinutes: 10);

        return new UploadUrlResponseDto(
            uploadUrl,
            createdMedia.Id,
            s3Key,
            DateTime.UtcNow.AddMinutes(10)
        );
    }
}
```

`Whispra.Application/UseCases/Media/ConfirmUpload/ConfirmUploadUseCase.cs`:

```csharp
using Whispra.Application.DTOs.Media;
using Whispra.Application.Interfaces.Repositories;

namespace Whispra.Application.UseCases.Media.ConfirmUpload;

public class ConfirmUploadUseCase
{
    private readonly IMediaRepository _mediaRepository;

    public ConfirmUploadUseCase(IMediaRepository mediaRepository)
    {
        _mediaRepository = mediaRepository;
    }

    public async Task<MediaResponseDto> ExecuteAsync(
        ConfirmUploadDto dto,
        string currentUserId,
        CancellationToken cancellationToken = default)
    {
        var media = await _mediaRepository.GetByIdAsync(dto.MediaId, cancellationToken);
        if (media == null)
        {
            throw new InvalidOperationException("Media not found");
        }

        // Verify ownership
        if (media.UserId != currentUserId)
        {
            throw new UnauthorizedAccessException("You don't own this media");
        }

        // Update metadata
        media.IsProcessed = true; // Mark as uploaded
        if (dto.Width.HasValue) media.Width = dto.Width.Value;
        if (dto.Height.HasValue) media.Height = dto.Height.Value;

        await _mediaRepository.UpdateAsync(media, cancellationToken);

        return new MediaResponseDto(
            media.Id,
            media.FileName,
            media.Url,
            media.Type,
            media.FileSizeBytes,
            media.MimeType,
            media.Width,
            media.Height,
            media.DurationSeconds,
            media.ThumbnailUrl,
            media.IsProcessed,
            media.CreatedAt
        );
    }
}
```

`Whispra.Application/UseCases/Media/Delete/DeleteMediaUseCase.cs`:

```csharp
using Whispra.Application.Interfaces.Repositories;
using Whispra.Application.Interfaces.Services;

namespace Whispra.Application.UseCases.Media.Delete;

public class DeleteMediaUseCase
{
    private readonly IMediaRepository _mediaRepository;
    private readonly IS3Service _s3Service;

    public DeleteMediaUseCase(
        IMediaRepository mediaRepository,
        IS3Service s3Service)
    {
        _mediaRepository = mediaRepository;
        _s3Service = s3Service;
    }

    public async Task ExecuteAsync(
        string mediaId,
        string currentUserId,
        CancellationToken cancellationToken = default)
    {
        var media = await _mediaRepository.GetByIdAsync(mediaId, cancellationToken);
        if (media == null)
        {
            throw new InvalidOperationException("Media not found");
        }

        // Verify ownership
        if (media.UserId != currentUserId)
        {
            throw new UnauthorizedAccessException("You don't own this media");
        }

        // Delete from S3
        await _s3Service.DeleteFileAsync(media.S3Key);

        // Soft delete from database
        await _mediaRepository.DeleteAsync(mediaId, cancellationToken);
    }
}
```

**Technologies**: File validation, presigned URL generation, metadata updates
**Study needed**:

- 📚 File upload security best practices (1-2 hours)
  - Resource: https://owasp.org/www-community/vulnerabilities/Unrestricted_File_Upload

## Step 6.6: Implement S3 Service in Infrastructure

```bash
cd backend/Whispra.Infrastructure

# Install MinIO SDK
dotnet add package Minio
```

`Whispra.Infrastructure/Configuration/S3Settings.cs`:

```csharp
namespace Whispra.Infrastructure.Configuration;

public class S3Settings
{
    public string Endpoint { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string BucketName { get; set; } = string.Empty;
    public string Region { get; set; } = "us-east-1";
    public bool UseSSL { get; set; } = false; // false for local MinIO
}
```

`Whispra.Infrastructure/Services/Storage/S3Service.cs`:

```csharp
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Whispra.Application.Interfaces.Services;
using Whispra.Infrastructure.Configuration;

namespace Whispra.Infrastructure.Services.Storage;

public class S3Service : IS3Service
{
    private readonly S3Settings _settings;
    private readonly IMinioClient _minioClient;

    public S3Service(IOptions<S3Settings> settings)
    {
        _settings = settings.Value;

        _minioClient = new MinioClient()
            .WithEndpoint(_settings.Endpoint)
            .WithCredentials(_settings.AccessKey, _settings.SecretKey)
            .WithSSL(_settings.UseSSL)
            .Build();
    }

    public async Task<string> GeneratePresignedUploadUrlAsync(
        string s3Key,
        string contentType,
        int expirationMinutes = 10)
    {
        var args = new PresignedPutObjectArgs()
            .WithBucket(_settings.BucketName)
            .WithObject(s3Key)
            .WithExpiry(expirationMinutes * 60);

        return await _minioClient.PresignedPutObjectAsync(args);
    }

    public async Task<string> GetPublicUrlAsync(string s3Key)
    {
        // For local development, construct the URL manually
        // In production with AWS S3, you'd use CloudFront URL or S3 public URL
        var protocol = _settings.UseSSL ? "https" : "http";
        return $"{protocol}://{_settings.Endpoint}/{_settings.BucketName}/{s3Key}";
    }

    public async Task<bool> DeleteFileAsync(string s3Key)
    {
        try
        {
            var args = new RemoveObjectArgs()
                .WithBucket(_settings.BucketName)
                .WithObject(s3Key);

            await _minioClient.RemoveObjectAsync(args);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
```

`Whispra.Infrastructure/Persistence/Repositories/MediaRepository.cs`:

```csharp
using MongoDB.Driver;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Domain.Entities.Media;
using Whispra.Infrastructure.Persistence.MongoDB;

namespace Whispra.Infrastructure.Persistence.Repositories;

public class MediaRepository : IMediaRepository
{
    private readonly MongoDbContext _context;

    public MediaRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<Media?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.Media
            .Find(m => m.Id == id && !m.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Media> CreateAsync(Media media, CancellationToken cancellationToken = default)
    {
        await _context.Media.InsertOneAsync(media, cancellationToken: cancellationToken);
        return media;
    }

    public async Task UpdateAsync(Media media, CancellationToken cancellationToken = default)
    {
        media.UpdatedAt = DateTime.UtcNow;
        await _context.Media.ReplaceOneAsync(
            m => m.Id == media.Id,
            media,
            cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var update = Builders<Media>.Update
            .Set(m => m.IsDeleted, true)
            .Set(m => m.DeletedAt, DateTime.UtcNow);

        await _context.Media.UpdateOneAsync(
            m => m.Id == id,
            update,
            cancellationToken: cancellationToken);
    }

    public async Task<List<Media>> GetUserMediaAsync(
        string userId, int skip, int limit, CancellationToken cancellationToken = default)
    {
        return await _context.Media
            .Find(m => m.UserId == userId && !m.IsDeleted)
            .SortByDescending(m => m.CreatedAt)
            .Skip(skip)
            .Limit(limit)
            .ToListAsync(cancellationToken);
    }
}
```

Update `Whispra.Infrastructure/Persistence/MongoDB/MongoDbContext.cs`:

```csharp
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Whispra.Domain.Entities.Auth;
using Whispra.Domain.Entities.Communities;
using Whispra.Domain.Entities.Media;
using Whispra.Domain.Entities.Posts;
using Whispra.Domain.Entities.Users;
using Whispra.Infrastructure.Configuration;

namespace Whispra.Infrastructure.Persistence.MongoDB;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<User> Users => _database.GetCollection<User>("users");
    public IMongoCollection<RefreshToken> RefreshTokens => _database.GetCollection<RefreshToken>("refresh_tokens");
    public IMongoCollection<Community> Communities => _database.GetCollection<Community>("communities");
    public IMongoCollection<CommunityMember> CommunityMembers => _database.GetCollection<CommunityMember>("community_members");
    public IMongoCollection<CommunityInvite> CommunityInvites => _database.GetCollection<CommunityInvite>("community_invites");
    public IMongoCollection<Post> Posts => _database.GetCollection<Post>("posts");
    public IMongoCollection<Comment> Comments => _database.GetCollection<Comment>("comments");
    public IMongoCollection<Reaction> Reactions => _database.GetCollection<Reaction>("reactions");
    public IMongoCollection<Media> Media => _database.GetCollection<Media>("media");
}
```

**Technologies**: MinIO SDK, presigned URLs, S3 operations
**Study needed**:

- 📚 MinIO .NET SDK (1-2 hours)
  - Resource: https://min.io/docs/minio/linux/developers/dotnet/minio-dotnet.html

## Step 6.7: Create Media Controller and Update Configuration

Create `backend/Whispra.Api/Controllers/MediaController.cs`:

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Whispra.Application.DTOs.Media;
using Whispra.Application.UseCases.Media.ConfirmUpload;
using Whispra.Application.UseCases.Media.Delete;
using Whispra.Application.UseCases.Media.GetUploadUrl;

namespace Whispra.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MediaController : ControllerBase
{
    private readonly GetUploadUrlUseCase _getUploadUrlUseCase;
    private readonly ConfirmUploadUseCase _confirmUploadUseCase;
    private readonly DeleteMediaUseCase _deleteMediaUseCase;

    public MediaController(
        GetUploadUrlUseCase getUploadUrlUseCase,
        ConfirmUploadUseCase confirmUploadUseCase,
        DeleteMediaUseCase deleteMediaUseCase)
    {
        _getUploadUrlUseCase = getUploadUrlUseCase;
        _confirmUploadUseCase = confirmUploadUseCase;
        _deleteMediaUseCase = deleteMediaUseCase;
    }

    private string GetCurrentUserId()
    {
        return User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("User not authenticated");
    }

    [HttpPost("upload-url")]
    public async Task<ActionResult<UploadUrlResponseDto>> GetUploadUrl(
        [FromBody] GetUploadUrlDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _getUploadUrlUseCase.ExecuteAsync(
                dto, GetCurrentUserId(), cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("confirm")]
    public async Task<ActionResult<MediaResponseDto>> ConfirmUpload(
        [FromBody] ConfirmUploadDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _confirmUploadUseCase.ExecuteAsync(
                dto, GetCurrentUserId(), cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [HttpDelete("{mediaId}")]
    public async Task<ActionResult> DeleteMedia(
        string mediaId,
        CancellationToken cancellationToken)
    {
        try
        {
            await _deleteMediaUseCase.ExecuteAsync(
                mediaId, GetCurrentUserId(), cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}
```

Update `backend/Whispra.Api/appsettings.json` to add S3 settings:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "MongoDbSettings": {
    "ConnectionString": "mongodb://admin:devpassword123@localhost:27017",
    "DatabaseName": "whispra_dev"
  },
  "JwtSettings": {
    "Secret": "YourVeryLongSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "WhispraApi",
    "Audience": "WhispraClients",
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
  },
  "S3Settings": {
    "Endpoint": "localhost:9000",
    "AccessKey": "minioadmin",
    "SecretKey": "minioadmin123",
    "BucketName": "whispra-media",
    "Region": "us-east-1",
    "UseSSL": false
  }
}
```

Update `backend/Whispra.Api/Extensions/ServiceCollectionExtensions.cs` to register media services:

```csharp
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Application.Interfaces.Services;
using Whispra.Application.UseCases.Auth.Login;
using Whispra.Application.UseCases.Auth.RefreshToken;
using Whispra.Application.UseCases.Comments.Create;
using Whispra.Application.UseCases.Communities.Create;
using Whispra.Application.UseCases.Communities.CreateInvite;
using Whispra.Application.UseCases.Communities.Join;
using Whispra.Application.UseCases.Communities.Leave;
using Whispra.Application.UseCases.Communities.UpdateRole;
using Whispra.Application.UseCases.Media.ConfirmUpload;
using Whispra.Application.UseCases.Media.Delete;
using Whispra.Application.UseCases.Media.GetUploadUrl;
using Whispra.Application.UseCases.Posts.Create;
using Whispra.Application.UseCases.Posts.Delete;
using Whispra.Application.UseCases.Posts.GetFeed;
using Whispra.Application.UseCases.Posts.Update;
using Whispra.Application.UseCases.Reactions.Add;
using Whispra.Application.UseCases.Users.Register;
using Whispra.Infrastructure.Configuration;
using Whispra.Infrastructure.Persistence.MongoDB;
using Whispra.Infrastructure.Persistence.Repositories;
using Whispra.Infrastructure.Services;
using Whispra.Infrastructure.Services.Auth;
using Whispra.Infrastructure.Services.Storage;

namespace Whispra.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register use cases - Users
        services.AddScoped<RegisterUserUseCase>();

        // Register use cases - Auth
        services.AddScoped<LoginUseCase>();
        services.AddScoped<RefreshTokenUseCase>();

        // Register use cases - Communities
        services.AddScoped<CreateCommunityUseCase>();
        services.AddScoped<JoinCommunityUseCase>();
        services.AddScoped<LeaveCommunityUseCase>();
        services.AddScoped<UpdateMemberRoleUseCase>();
        services.AddScoped<CreateInviteUseCase>();

        // Register use cases - Posts
        services.AddScoped<CreatePostUseCase>();
        services.AddScoped<UpdatePostUseCase>();
        services.AddScoped<DeletePostUseCase>();
        services.AddScoped<GetHomeFeedUseCase>();
        services.AddScoped<CreateCommentUseCase>();
        services.AddScoped<AddReactionUseCase>();

        // Register use cases - Media
        services.AddScoped<GetUploadUrlUseCase>();
        services.AddScoped<ConfirmUploadUseCase>();
        services.AddScoped<DeleteMediaUseCase>();

        return services;
    }

    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure MongoDB
        services.Configure<MongoDbSettings>(
            configuration.GetSection(nameof(MongoDbSettings)));
        services.AddSingleton<MongoDbContext>();

        // Configure JWT
        services.Configure<JwtSettings>(
            configuration.GetSection(nameof(JwtSettings)));

        // Configure S3
        services.Configure<S3Settings>(
            configuration.GetSection(nameof(S3Settings)));

        // Register repositories - Users
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        // Register repositories - Communities
        services.AddScoped<ICommunityRepository, CommunityRepository>();
        services.AddScoped<ICommunityMemberRepository, CommunityMemberRepository>();
        services.AddScoped<ICommunityInviteRepository, CommunityInviteRepository>();

        // Register repositories - Posts
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IReactionRepository, ReactionRepository>();

        // Register repositories - Media
        services.AddScoped<IMediaRepository, MediaRepository>();

        // Register services
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<IS3Service, S3Service>();

        return services;
    }

    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings!.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                ClockSkew = TimeSpan.Zero
            };
        });

        return services;
    }
}
```

**Technologies**: REST API for uploads, presigned URL handling
**Study needed**: ✅ None, follows established patterns

## Step 6.8: Test Media Upload

```bash
# Make sure all services are running
docker compose up -d

# Run the API
cd backend/Whispra.Api
dotnet run
```

Open Swagger: `https://localhost:7001/swagger`

### Test Flow:

**1. Login and authorize** (get Bearer token)

**2. Request upload URL**:

```json
POST /api/Media/upload-url
{
  "fileName": "profile-pic.jpg",
  "type": 0,
  "fileSizeBytes": 2048000,
  "mimeType": "image/jpeg"
}
```

Response:

```json
{
  "uploadUrl": "http://localhost:9000/whispra-media/userId/images/1234567890_abc12345_profile-pic.jpg?X-Amz-Algorithm=...",
  "mediaId": "abc123...",
  "s3Key": "userId/images/1234567890_abc12345_profile-pic.jpg",
  "expiresAt": "2025-12-22T14:10:00Z"
}
```

**3. Upload file to presigned URL** (use Postman or cURL):

```bash
curl -X PUT "PRESIGNED_URL_FROM_RESPONSE" \
     -H "Content-Type: image/jpeg" \
     --upload-file /path/to/your/image.jpg
```

Or use Postman:

- Method: `PUT`
- URL: Paste the `uploadUrl` from step 2
- Body: Binary → Select your image file
- Headers: `Content-Type: image/jpeg`
- Click Send

**4. Confirm upload**:

```json
POST /api/Media/confirm
{
  "mediaId": "abc123...",
  "width": 1920,
  "height": 1080
}
```

Response:

```json
{
  "id": "abc123...",
  "fileName": "profile-pic.jpg",
  "url": "http://localhost:9000/whispra-media/userId/images/1234567890_abc12345_profile-pic.jpg",
  "type": 0,
  "fileSizeBytes": 2048000,
  "mimeType": "image/jpeg",
  "width": 1920,
  "height": 1080,
  "durationSeconds": null,
  "thumbnailUrl": null,
  "isProcessed": true,
  "createdAt": "2025-12-22T13:00:00Z"
}
```

**5. Verify in MinIO**:

- Open `http://localhost:9001`
- Navigate to `whispra-media` bucket
- You should see your uploaded file!

**6. Delete media**:

```
DELETE /api/Media/{mediaId}
```

Check MongoDB Compass for `media` collection!

**Study needed**: ✅ Just testing

---

**🔄 CHECKPOINT - Media Upload Complete!**

You now have:

- ✅ S3-compatible storage with MinIO (local dev)
- ✅ Presigned URL generation for secure uploads
- ✅ Direct upload to S3 (not through backend)
- ✅ File validation (size, MIME type)
- ✅ Media metadata storage in MongoDB
- ✅ Upload confirmation flow
- ✅ Media deletion (S3 + database)

Before moving to Phase 7 (Real-time Messaging with SignalR), confirm:

1. ✅ Can you request an upload URL?
2. ✅ Can you upload a file using the presigned URL?
3. ✅ Can you confirm the upload with metadata?
4. ✅ Can you see the file in MinIO console?
5. ✅ Do you understand the upload flow?

**Next Phase Preview**: We'll implement Real-time Messaging with SignalR for direct messages, group chats, typing indicators, and online presence.

Ready to continue with Phase 7 (Real-time Messaging)?

---

# PHASE 7: Real-time Messaging with SignalR

**Goal**: Implement real-time direct messaging, group chats, typing indicators, online presence tracking, and message read receipts using SignalR WebSocket connections.

## Step 7.1: Understanding SignalR and Real-time Messaging

**What is SignalR?**

- **SignalR** is a library for ASP.NET that enables real-time bi-directional communication between server and clients
- Uses **WebSockets** (falls back to Server-Sent Events or Long Polling if WebSockets unavailable)
- Perfect for: Chat apps, live notifications, collaborative editing, real-time dashboards

**How SignalR Works**:

1. Client connects to SignalR **Hub** (WebSocket endpoint)
2. Server can **push** messages to specific users or groups
3. Clients can **invoke** server methods
4. Connection stays open for instant bi-directional communication

**Messaging Features We'll Build**:

- **Direct Messages (DMs)**: 1-on-1 private conversations
- **Group Chats**: Multi-user conversations (like WhatsApp groups)
- **Typing Indicators**: "User is typing..." real-time notifications
- **Online Presence**: See who's online/offline
- **Read Receipts**: Track when messages are read
- **Message History**: Paginated message loading

📚 **Study needed**: SignalR concepts and WebSockets (3-4 hours)

- Resource: https://learn.microsoft.com/en-us/aspnet/core/signalr/introduction
- Resource: https://learn.microsoft.com/en-us/aspnet/core/tutorials/signalr
- Resource: https://www.youtube.com/watch?v=RaXx_f3bIRU (SignalR tutorial)

## Step 7.2: Add Messaging Entities to Domain Layer

```bash
cd backend/Whispra.Domain

# Create folders
mkdir -p Entities/Messages
mkdir -p Enums
```

`Whispra.Domain/Enums/ConversationType.cs`:

```csharp
namespace Whispra.Domain.Enums;

public enum ConversationType
{
    Direct = 0,     // 1-on-1 conversation
    Group = 1       // Group chat
}
```

`Whispra.Domain/Enums/MessageStatus.cs`:

```csharp
namespace Whispra.Domain.Enums;

public enum MessageStatus
{
    Sent = 0,       // Message sent to server
    Delivered = 1,  // Message delivered to recipient(s)
    Read = 2        // Message read by recipient(s)
}
```

`Whispra.Domain/Entities/Messages/Conversation.cs`:

```csharp
using Whispra.Domain.Enums;

namespace Whispra.Domain.Entities.Messages;

public class Conversation : BaseEntity
{
    public ConversationType Type { get; set; } = ConversationType.Direct;
    public List<string> ParticipantIds { get; set; } = new(); // User IDs in conversation
    public string? GroupName { get; set; } // For group chats only
    public string? GroupAvatarUrl { get; set; } // For group chats only
    public string? LastMessageId { get; set; }
    public DateTime? LastMessageAt { get; set; }
    public Dictionary<string, DateTime> LastReadAt { get; set; } = new(); // UserId -> LastReadTimestamp
    public Dictionary<string, bool> TypingStatus { get; set; } = new(); // UserId -> IsTyping
}
```

`Whispra.Domain/Entities/Messages/Message.cs`:

```csharp
using Whispra.Domain.Enums;

namespace Whispra.Domain.Entities.Messages;

public class Message : BaseEntity
{
    public string ConversationId { get; set; } = string.Empty;
    public string SenderId { get; set; } = string.Empty;
    public string TextContent { get; set; } = string.Empty;
    public List<string> MediaUrls { get; set; } = new(); // Links to uploaded media
    public MessageStatus Status { get; set; } = MessageStatus.Sent;
    public DateTime? DeliveredAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public List<string> ReadByUserIds { get; set; } = new(); // For group chats
    public string? ReplyToMessageId { get; set; } // For threaded replies
    public bool IsEdited { get; set; } = false;
    public DateTime? EditedAt { get; set; }
}
```

`Whispra.Domain/Entities/Users/UserPresence.cs`:

```csharp
namespace Whispra.Domain.Entities.Users;

public class UserPresence : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public bool IsOnline { get; set; } = false;
    public DateTime LastSeenAt { get; set; } = DateTime.UtcNow;
    public string? ConnectionId { get; set; } // SignalR connection ID
    public List<string> ActiveConversationIds { get; set; } = new(); // Conversations user is actively viewing
}
```

**Technologies**: Real-time entities, presence tracking
**Study needed**: ✅ None, straightforward entities

## Step 7.3: Add Messaging DTOs and Interfaces to Application Layer

```bash
cd backend/Whispra.Application

# Create folders
mkdir -p DTOs/Messages
mkdir -p UseCases/Messages/SendMessage
mkdir -p UseCases/Messages/GetConversations
mkdir -p UseCases/Messages/GetMessages
mkdir -p UseCases/Messages/CreateConversation
mkdir -p UseCases/Messages/MarkAsRead
mkdir -p Interfaces/Hubs
mkdir -p Interfaces/Repositories
```

`Whispra.Application/DTOs/Messages/SendMessageDto.cs`:

```csharp
namespace Whispra.Application.DTOs.Messages;

public record SendMessageDto(
    string ConversationId,
    string TextContent,
    List<string>? MediaUrls = null,
    string? ReplyToMessageId = null
);
```

`Whispra.Application/DTOs/Messages/CreateConversationDto.cs`:

```csharp
using Whispra.Domain.Enums;

namespace Whispra.Application.DTOs.Messages;

public record CreateConversationDto(
    ConversationType Type,
    List<string> ParticipantIds,
    string? GroupName = null,
    string? GroupAvatarUrl = null
);
```

`Whispra.Application/DTOs/Messages/MessageResponseDto.cs`:

```csharp
using Whispra.Domain.Enums;

namespace Whispra.Application.DTOs.Messages;

public record MessageResponseDto(
    string Id,
    string ConversationId,
    string SenderId,
    string TextContent,
    List<string> MediaUrls,
    MessageStatus Status,
    DateTime? DeliveredAt,
    DateTime? ReadAt,
    List<string> ReadByUserIds,
    string? ReplyToMessageId,
    bool IsEdited,
    DateTime? EditedAt,
    DateTime CreatedAt
);
```

`Whispra.Application/DTOs/Messages/ConversationResponseDto.cs`:

```csharp
using Whispra.Domain.Enums;

namespace Whispra.Application.DTOs.Messages;

public record ConversationResponseDto(
    string Id,
    ConversationType Type,
    List<string> ParticipantIds,
    string? GroupName,
    string? GroupAvatarUrl,
    MessageResponseDto? LastMessage,
    DateTime? LastMessageAt,
    int UnreadCount,
    List<string> TypingUserIds,
    DateTime CreatedAt
);
```

`Whispra.Application/DTOs/Messages/TypingIndicatorDto.cs`:

```csharp
namespace Whispra.Application.DTOs.Messages;

public record TypingIndicatorDto(
    string ConversationId,
    bool IsTyping
);
```

`Whispra.Application/DTOs/Messages/UserPresenceDto.cs`:

```csharp
namespace Whispra.Application.DTOs.Messages;

public record UserPresenceDto(
    string UserId,
    bool IsOnline,
    DateTime LastSeenAt
);
```

`Whispra.Application/Interfaces/Repositories/IConversationRepository.cs`:

```csharp
using Whispra.Domain.Entities.Messages;

namespace Whispra.Application.Interfaces.Repositories;

public interface IConversationRepository
{
    Task<Conversation?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Conversation?> GetDirectConversationAsync(string userId1, string userId2, CancellationToken cancellationToken = default);
    Task<List<Conversation>> GetUserConversationsAsync(string userId, int skip, int limit, CancellationToken cancellationToken = default);
    Task<Conversation> CreateAsync(Conversation conversation, CancellationToken cancellationToken = default);
    Task UpdateAsync(Conversation conversation, CancellationToken cancellationToken = default);
    Task UpdateTypingStatusAsync(string conversationId, string userId, bool isTyping, CancellationToken cancellationToken = default);
    Task UpdateLastReadAsync(string conversationId, string userId, DateTime timestamp, CancellationToken cancellationToken = default);
}
```

`Whispra.Application/Interfaces/Repositories/IMessageRepository.cs`:

```csharp
using Whispra.Domain.Entities.Messages;

namespace Whispra.Application.Interfaces.Repositories;

public interface IMessageRepository
{
    Task<Message?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<List<Message>> GetConversationMessagesAsync(string conversationId, int skip, int limit, CancellationToken cancellationToken = default);
    Task<Message> CreateAsync(Message message, CancellationToken cancellationToken = default);
    Task UpdateAsync(Message message, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountAsync(string conversationId, string userId, CancellationToken cancellationToken = default);
    Task MarkAsReadAsync(string conversationId, string userId, CancellationToken cancellationToken = default);
}
```

`Whispra.Application/Interfaces/Repositories/IUserPresenceRepository.cs`:

```csharp
using Whispra.Domain.Entities.Users;

namespace Whispra.Application.Interfaces.Repositories;

public interface IUserPresenceRepository
{
    Task<UserPresence?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<List<UserPresence>> GetByUserIdsAsync(List<string> userIds, CancellationToken cancellationToken = default);
    Task UpsertAsync(UserPresence presence, CancellationToken cancellationToken = default);
    Task SetOnlineAsync(string userId, string connectionId, CancellationToken cancellationToken = default);
    Task SetOfflineAsync(string userId, CancellationToken cancellationToken = default);
}
```

`Whispra.Application/Interfaces/Hubs/IChatClient.cs`:

```csharp
using Whispra.Application.DTOs.Messages;

namespace Whispra.Application.Interfaces.Hubs;

// Interface for client-side methods that the server can invoke
public interface IChatClient
{
    Task ReceiveMessage(MessageResponseDto message);
    Task UserTyping(string conversationId, string userId, bool isTyping);
    Task UserPresenceChanged(string userId, bool isOnline, DateTime lastSeenAt);
    Task MessageRead(string conversationId, string messageId, string userId);
    Task ConversationUpdated(ConversationResponseDto conversation);
}
```

**Technologies**: SignalR hub interfaces, real-time DTOs
**Study needed**:

- 📚 SignalR strongly-typed hubs (1-2 hours)
  - Resource: https://learn.microsoft.com/en-us/aspnet/core/signalr/hubs#strongly-typed-hubs

## Step 7.4: Create Messaging Use Cases

`Whispra.Application/UseCases/Messages/CreateConversation/CreateConversationUseCase.cs`:

```csharp
using Whispra.Application.DTOs.Messages;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Domain.Entities.Messages;
using Whispra.Domain.Enums;

namespace Whispra.Application.UseCases.Messages.CreateConversation;

public class CreateConversationUseCase
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IUserRepository _userRepository;

    public CreateConversationUseCase(
        IConversationRepository conversationRepository,
        IUserRepository userRepository)
    {
        _conversationRepository = conversationRepository;
        _userRepository = userRepository;
    }

    public async Task<ConversationResponseDto> ExecuteAsync(
        CreateConversationDto dto,
        string currentUserId,
        CancellationToken cancellationToken = default)
    {
        // Validate participants exist
        var participantIds = dto.ParticipantIds.Distinct().ToList();

        if (!participantIds.Contains(currentUserId))
        {
            participantIds.Add(currentUserId);
        }

        foreach (var participantId in participantIds)
        {
            var user = await _userRepository.GetByIdAsync(participantId, cancellationToken);
            if (user == null)
            {
                throw new InvalidOperationException($"User with ID {participantId} not found");
            }
        }

        // For direct conversations, check if one already exists
        if (dto.Type == ConversationType.Direct)
        {
            if (participantIds.Count != 2)
            {
                throw new InvalidOperationException("Direct conversations must have exactly 2 participants");
            }

            var existingConversation = await _conversationRepository.GetDirectConversationAsync(
                participantIds[0], participantIds[1], cancellationToken);

            if (existingConversation != null)
            {
                return MapToDto(existingConversation, currentUserId);
            }
        }

        // Create new conversation
        var conversation = new Conversation
        {
            Type = dto.Type,
            ParticipantIds = participantIds,
            GroupName = dto.GroupName,
            GroupAvatarUrl = dto.GroupAvatarUrl
        };

        var created = await _conversationRepository.CreateAsync(conversation, cancellationToken);
        return MapToDto(created, currentUserId);
    }

    private ConversationResponseDto MapToDto(Conversation conversation, string currentUserId)
    {
        return new ConversationResponseDto(
            conversation.Id,
            conversation.Type,
            conversation.ParticipantIds,
            conversation.GroupName,
            conversation.GroupAvatarUrl,
            null, // LastMessage will be populated separately
            conversation.LastMessageAt,
            0, // UnreadCount will be calculated separately
            conversation.TypingStatus.Where(ts => ts.Value).Select(ts => ts.Key).ToList(),
            conversation.CreatedAt
        );
    }
}
```

`Whispra.Application/UseCases/Messages/SendMessage/SendMessageUseCase.cs`:

```csharp
using Whispra.Application.DTOs.Messages;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Domain.Entities.Messages;

namespace Whispra.Application.UseCases.Messages.SendMessage;

public class SendMessageUseCase
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IMessageRepository _messageRepository;

    public SendMessageUseCase(
        IConversationRepository conversationRepository,
        IMessageRepository messageRepository)
    {
        _conversationRepository = conversationRepository;
        _messageRepository = messageRepository;
    }

    public async Task<MessageResponseDto> ExecuteAsync(
        SendMessageDto dto,
        string currentUserId,
        CancellationToken cancellationToken = default)
    {
        // Verify conversation exists and user is a participant
        var conversation = await _conversationRepository.GetByIdAsync(dto.ConversationId, cancellationToken);
        if (conversation == null)
        {
            throw new InvalidOperationException("Conversation not found");
        }

        if (!conversation.ParticipantIds.Contains(currentUserId))
        {
            throw new UnauthorizedAccessException("You are not a participant in this conversation");
        }

        // Create message
        var message = new Message
        {
            ConversationId = dto.ConversationId,
            SenderId = currentUserId,
            TextContent = dto.TextContent,
            MediaUrls = dto.MediaUrls ?? new List<string>(),
            ReplyToMessageId = dto.ReplyToMessageId
        };

        var created = await _messageRepository.CreateAsync(message, cancellationToken);

        // Update conversation's last message
        conversation.LastMessageId = created.Id;
        conversation.LastMessageAt = created.CreatedAt;
        await _conversationRepository.UpdateAsync(conversation, cancellationToken);

        return MapToDto(created);
    }

    private MessageResponseDto MapToDto(Message message)
    {
        return new MessageResponseDto(
            message.Id,
            message.ConversationId,
            message.SenderId,
            message.TextContent,
            message.MediaUrls,
            message.Status,
            message.DeliveredAt,
            message.ReadAt,
            message.ReadByUserIds,
            message.ReplyToMessageId,
            message.IsEdited,
            message.EditedAt,
            message.CreatedAt
        );
    }
}
```

`Whispra.Application/UseCases/Messages/GetConversations/GetConversationsUseCase.cs`:

```csharp
using Whispra.Application.DTOs.Messages;
using Whispra.Application.Interfaces.Repositories;

namespace Whispra.Application.UseCases.Messages.GetConversations;

public class GetConversationsUseCase
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IMessageRepository _messageRepository;

    public GetConversationsUseCase(
        IConversationRepository conversationRepository,
        IMessageRepository messageRepository)
    {
        _conversationRepository = conversationRepository;
        _messageRepository = messageRepository;
    }

    public async Task<List<ConversationResponseDto>> ExecuteAsync(
        string currentUserId,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var skip = (page - 1) * pageSize;
        var conversations = await _conversationRepository.GetUserConversationsAsync(
            currentUserId, skip, pageSize, cancellationToken);

        var result = new List<ConversationResponseDto>();

        foreach (var conversation in conversations)
        {
            MessageResponseDto? lastMessage = null;

            if (!string.IsNullOrEmpty(conversation.LastMessageId))
            {
                var message = await _messageRepository.GetByIdAsync(conversation.LastMessageId, cancellationToken);
                if (message != null)
                {
                    lastMessage = new MessageResponseDto(
                        message.Id,
                        message.ConversationId,
                        message.SenderId,
                        message.TextContent,
                        message.MediaUrls,
                        message.Status,
                        message.DeliveredAt,
                        message.ReadAt,
                        message.ReadByUserIds,
                        message.ReplyToMessageId,
                        message.IsEdited,
                        message.EditedAt,
                        message.CreatedAt
                    );
                }
            }

            var unreadCount = await _messageRepository.GetUnreadCountAsync(
                conversation.Id, currentUserId, cancellationToken);

            var typingUserIds = conversation.TypingStatus
                .Where(ts => ts.Value && ts.Key != currentUserId)
                .Select(ts => ts.Key)
                .ToList();

            result.Add(new ConversationResponseDto(
                conversation.Id,
                conversation.Type,
                conversation.ParticipantIds,
                conversation.GroupName,
                conversation.GroupAvatarUrl,
                lastMessage,
                conversation.LastMessageAt,
                unreadCount,
                typingUserIds,
                conversation.CreatedAt
            ));
        }

        return result;
    }
}
```

`Whispra.Application/UseCases/Messages/GetMessages/GetMessagesUseCase.cs`:

```csharp
using Whispra.Application.DTOs.Messages;
using Whispra.Application.Interfaces.Repositories;

namespace Whispra.Application.UseCases.Messages.GetMessages;

public class GetMessagesUseCase
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IMessageRepository _messageRepository;

    public GetMessagesUseCase(
        IConversationRepository conversationRepository,
        IMessageRepository messageRepository)
    {
        _conversationRepository = conversationRepository;
        _messageRepository = messageRepository;
    }

    public async Task<List<MessageResponseDto>> ExecuteAsync(
        string conversationId,
        string currentUserId,
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        // Verify user is participant
        var conversation = await _conversationRepository.GetByIdAsync(conversationId, cancellationToken);
        if (conversation == null)
        {
            throw new InvalidOperationException("Conversation not found");
        }

        if (!conversation.ParticipantIds.Contains(currentUserId))
        {
            throw new UnauthorizedAccessException("You are not a participant in this conversation");
        }

        var skip = (page - 1) * pageSize;
        var messages = await _messageRepository.GetConversationMessagesAsync(
            conversationId, skip, pageSize, cancellationToken);

        return messages.Select(m => new MessageResponseDto(
            m.Id,
            m.ConversationId,
            m.SenderId,
            m.TextContent,
            m.MediaUrls,
            m.Status,
            m.DeliveredAt,
            m.ReadAt,
            m.ReadByUserIds,
            m.ReplyToMessageId,
            m.IsEdited,
            m.EditedAt,
            m.CreatedAt
        )).ToList();
    }
}
```

`Whispra.Application/UseCases/Messages/MarkAsRead/MarkAsReadUseCase.cs`:

```csharp
using Whispra.Application.Interfaces.Repositories;

namespace Whispra.Application.UseCases.Messages.MarkAsRead;

public class MarkAsReadUseCase
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IMessageRepository _messageRepository;

    public MarkAsReadUseCase(
        IConversationRepository conversationRepository,
        IMessageRepository messageRepository)
    {
        _conversationRepository = conversationRepository;
        _messageRepository = messageRepository;
    }

    public async Task ExecuteAsync(
        string conversationId,
        string currentUserId,
        CancellationToken cancellationToken = default)
    {
        // Verify user is participant
        var conversation = await _conversationRepository.GetByIdAsync(conversationId, cancellationToken);
        if (conversation == null)
        {
            throw new InvalidOperationException("Conversation not found");
        }

        if (!conversation.ParticipantIds.Contains(currentUserId))
        {
            throw new UnauthorizedAccessException("You are not a participant in this conversation");
        }

        // Mark all messages in conversation as read
        await _messageRepository.MarkAsReadAsync(conversationId, currentUserId, cancellationToken);

        // Update conversation's last read timestamp
        await _conversationRepository.UpdateLastReadAsync(conversationId, currentUserId, DateTime.UtcNow, cancellationToken);
    }
}
```

**Technologies**: Real-time messaging logic, conversation management
**Study needed**: ✅ None, builds on established patterns

## Step 7.5: Implement Infrastructure Layer for Messaging

```bash
cd backend/Whispra.Infrastructure/Persistence/Repositories
```

`Whispra.Infrastructure/Persistence/Repositories/ConversationRepository.cs`:

```csharp
using MongoDB.Driver;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Domain.Entities.Messages;
using Whispra.Domain.Enums;
using Whispra.Infrastructure.Persistence.MongoDB;

namespace Whispra.Infrastructure.Persistence.Repositories;

public class ConversationRepository : IConversationRepository
{
    private readonly MongoDbContext _context;

    public ConversationRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<Conversation?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.Conversations
            .Find(c => c.Id == id && !c.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Conversation?> GetDirectConversationAsync(
        string userId1, string userId2, CancellationToken cancellationToken = default)
    {
        return await _context.Conversations
            .Find(c => c.Type == ConversationType.Direct
                && c.ParticipantIds.Contains(userId1)
                && c.ParticipantIds.Contains(userId2)
                && !c.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<Conversation>> GetUserConversationsAsync(
        string userId, int skip, int limit, CancellationToken cancellationToken = default)
    {
        return await _context.Conversations
            .Find(c => c.ParticipantIds.Contains(userId) && !c.IsDeleted)
            .SortByDescending(c => c.LastMessageAt)
            .ThenByDescending(c => c.CreatedAt)
            .Skip(skip)
            .Limit(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<Conversation> CreateAsync(Conversation conversation, CancellationToken cancellationToken = default)
    {
        await _context.Conversations.InsertOneAsync(conversation, cancellationToken: cancellationToken);
        return conversation;
    }

    public async Task UpdateAsync(Conversation conversation, CancellationToken cancellationToken = default)
    {
        conversation.UpdatedAt = DateTime.UtcNow;
        await _context.Conversations.ReplaceOneAsync(
            c => c.Id == conversation.Id,
            conversation,
            cancellationToken: cancellationToken);
    }

    public async Task UpdateTypingStatusAsync(
        string conversationId, string userId, bool isTyping, CancellationToken cancellationToken = default)
    {
        var update = Builders<Conversation>.Update
            .Set($"TypingStatus.{userId}", isTyping)
            .Set(c => c.UpdatedAt, DateTime.UtcNow);

        await _context.Conversations.UpdateOneAsync(
            c => c.Id == conversationId,
            update,
            cancellationToken: cancellationToken);
    }

    public async Task UpdateLastReadAsync(
        string conversationId, string userId, DateTime timestamp, CancellationToken cancellationToken = default)
    {
        var update = Builders<Conversation>.Update
            .Set($"LastReadAt.{userId}", timestamp)
            .Set(c => c.UpdatedAt, DateTime.UtcNow);

        await _context.Conversations.UpdateOneAsync(
            c => c.Id == conversationId,
            update,
            cancellationToken: cancellationToken);
    }
}
```

`Whispra.Infrastructure/Persistence/Repositories/MessageRepository.cs`:

```csharp
using MongoDB.Driver;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Domain.Entities.Messages;
using Whispra.Infrastructure.Persistence.MongoDB;

namespace Whispra.Infrastructure.Persistence.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly MongoDbContext _context;

    public MessageRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<Message?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.Messages
            .Find(m => m.Id == id && !m.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<Message>> GetConversationMessagesAsync(
        string conversationId, int skip, int limit, CancellationToken cancellationToken = default)
    {
        return await _context.Messages
            .Find(m => m.ConversationId == conversationId && !m.IsDeleted)
            .SortByDescending(m => m.CreatedAt)
            .Skip(skip)
            .Limit(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<Message> CreateAsync(Message message, CancellationToken cancellationToken = default)
    {
        await _context.Messages.InsertOneAsync(message, cancellationToken: cancellationToken);
        return message;
    }

    public async Task UpdateAsync(Message message, CancellationToken cancellationToken = default)
    {
        message.UpdatedAt = DateTime.UtcNow;
        await _context.Messages.ReplaceOneAsync(
            m => m.Id == message.Id,
            message,
            cancellationToken: cancellationToken);
    }

    public async Task<int> GetUnreadCountAsync(
        string conversationId, string userId, CancellationToken cancellationToken = default)
    {
        // Get conversation to find last read timestamp
        var conversation = await _context.Conversations
            .Find(c => c.Id == conversationId)
            .FirstOrDefaultAsync(cancellationToken);

        if (conversation == null)
            return 0;

        var lastReadTimestamp = conversation.LastReadAt.ContainsKey(userId)
            ? conversation.LastReadAt[userId]
            : DateTime.MinValue;

        // Count messages after last read that weren't sent by the user
        var count = await _context.Messages
            .CountDocumentsAsync(
                m => m.ConversationId == conversationId
                    && m.SenderId != userId
                    && m.CreatedAt > lastReadTimestamp
                    && !m.IsDeleted,
                cancellationToken: cancellationToken);

        return (int)count;
    }

    public async Task MarkAsReadAsync(
        string conversationId, string userId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Message>.Filter.And(
            Builders<Message>.Filter.Eq(m => m.ConversationId, conversationId),
            Builders<Message>.Filter.Ne(m => m.SenderId, userId),
            Builders<Message>.Filter.Eq(m => m.IsDeleted, false)
        );

        var update = Builders<Message>.Update
            .AddToSet(m => m.ReadByUserIds, userId)
            .Set(m => m.ReadAt, DateTime.UtcNow)
            .Set(m => m.UpdatedAt, DateTime.UtcNow);

        await _context.Messages.UpdateManyAsync(filter, update, cancellationToken: cancellationToken);
    }
}
```

`Whispra.Infrastructure/Persistence/Repositories/UserPresenceRepository.cs`:

```csharp
using MongoDB.Driver;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Domain.Entities.Users;
using Whispra.Infrastructure.Persistence.MongoDB;

namespace Whispra.Infrastructure.Persistence.Repositories;

public class UserPresenceRepository : IUserPresenceRepository
{
    private readonly MongoDbContext _context;

    public UserPresenceRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<UserPresence?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserPresences
            .Find(p => p.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<UserPresence>> GetByUserIdsAsync(
        List<string> userIds, CancellationToken cancellationToken = default)
    {
        return await _context.UserPresences
            .Find(p => userIds.Contains(p.UserId))
            .ToListAsync(cancellationToken);
    }

    public async Task UpsertAsync(UserPresence presence, CancellationToken cancellationToken = default)
    {
        var filter = Builders<UserPresence>.Filter.Eq(p => p.UserId, presence.UserId);
        var options = new ReplaceOptions { IsUpsert = true };

        presence.UpdatedAt = DateTime.UtcNow;
        await _context.UserPresences.ReplaceOneAsync(filter, presence, options, cancellationToken);
    }

    public async Task SetOnlineAsync(string userId, string connectionId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<UserPresence>.Filter.Eq(p => p.UserId, userId);
        var update = Builders<UserPresence>.Update
            .Set(p => p.IsOnline, true)
            .Set(p => p.ConnectionId, connectionId)
            .Set(p => p.LastSeenAt, DateTime.UtcNow)
            .Set(p => p.UpdatedAt, DateTime.UtcNow);

        await _context.UserPresences.UpdateOneAsync(
            filter, update, new UpdateOptions { IsUpsert = true }, cancellationToken);
    }

    public async Task SetOfflineAsync(string userId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<UserPresence>.Filter.Eq(p => p.UserId, userId);
        var update = Builders<UserPresence>.Update
            .Set(p => p.IsOnline, false)
            .Set(p => p.LastSeenAt, DateTime.UtcNow)
            .Set(p => p.ConnectionId, null)
            .Set(p => p.UpdatedAt, DateTime.UtcNow);

        await _context.UserPresences.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
    }
}
```

Update `Whispra.Infrastructure/Persistence/MongoDB/MongoDbContext.cs`:

```csharp
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Whispra.Domain.Entities.Auth;
using Whispra.Domain.Entities.Communities;
using Whispra.Domain.Entities.Media;
using Whispra.Domain.Entities.Messages;
using Whispra.Domain.Entities.Posts;
using Whispra.Domain.Entities.Users;
using Whispra.Infrastructure.Configuration;

namespace Whispra.Infrastructure.Persistence.MongoDB;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<User> Users => _database.GetCollection<User>("users");
    public IMongoCollection<RefreshToken> RefreshTokens => _database.GetCollection<RefreshToken>("refresh_tokens");
    public IMongoCollection<Community> Communities => _database.GetCollection<Community>("communities");
    public IMongoCollection<CommunityMember> CommunityMembers => _database.GetCollection<CommunityMember>("community_members");
    public IMongoCollection<CommunityInvite> CommunityInvites => _database.GetCollection<CommunityInvite>("community_invites");
    public IMongoCollection<Post> Posts => _database.GetCollection<Post>("posts");
    public IMongoCollection<Comment> Comments => _database.GetCollection<Comment>("comments");
    public IMongoCollection<Reaction> Reactions => _database.GetCollection<Reaction>("reactions");
    public IMongoCollection<Media> Media => _database.GetCollection<Media>("media");
    public IMongoCollection<Conversation> Conversations => _database.GetCollection<Conversation>("conversations");
    public IMongoCollection<Message> Messages => _database.GetCollection<Message>("messages");
    public IMongoCollection<UserPresence> UserPresences => _database.GetCollection<UserPresence>("user_presences");
}
```

**Technologies**: MongoDB queries for real-time data, upsert operations
**Study needed**: ✅ None, builds on MongoDB patterns

## Step 7.6: Create SignalR Chat Hub

```bash
cd backend/Whispra.Api

# Install SignalR package (already included in ASP.NET Core)
# Create folder for hubs
mkdir -p Hubs
```

`Whispra.Api/Hubs/ChatHub.cs`:

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Whispra.Application.DTOs.Messages;
using Whispra.Application.Interfaces.Hubs;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Application.UseCases.Messages.SendMessage;
using Whispra.Application.UseCases.Messages.MarkAsRead;

namespace Whispra.Api.Hubs;

[Authorize]
public class ChatHub : Hub<IChatClient>
{
    private readonly ISendMessageUseCase _sendMessageUseCase;
    private readonly IMarkAsReadUseCase _markAsReadUseCase;
    private readonly IConversationRepository _conversationRepository;
    private readonly IUserPresenceRepository _presenceRepository;

    public ChatHub(
        SendMessageUseCase sendMessageUseCase,
        MarkAsReadUseCase markAsReadUseCase,
        IConversationRepository conversationRepository,
        IUserPresenceRepository presenceRepository)
    {
        _sendMessageUseCase = sendMessageUseCase;
        _markAsReadUseCase = markAsReadUseCase;
        _conversationRepository = conversationRepository;
        _presenceRepository = presenceRepository;
    }

    private string GetCurrentUserId()
    {
        return Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? throw new HubException("User not authenticated");
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetCurrentUserId();

        // Set user as online
        await _presenceRepository.SetOnlineAsync(userId, Context.ConnectionId);

        // Notify other users about online status
        await Clients.Others.UserPresenceChanged(userId, true, DateTime.UtcNow);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetCurrentUserId();

        // Set user as offline
        await _presenceRepository.SetOfflineAsync(userId);

        // Notify other users about offline status
        await Clients.Others.UserPresenceChanged(userId, false, DateTime.UtcNow);

        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinConversation(string conversationId)
    {
        var userId = GetCurrentUserId();

        // Verify user is participant
        var conversation = await _conversationRepository.GetByIdAsync(conversationId);
        if (conversation == null || !conversation.ParticipantIds.Contains(userId))
        {
            throw new HubException("Conversation not found or access denied");
        }

        // Join SignalR group for this conversation
        await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
    }

    public async Task LeaveConversation(string conversationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);
    }

    public async Task SendMessage(SendMessageDto dto)
    {
        var userId = GetCurrentUserId();

        // Send message using use case
        var message = await _sendMessageUseCase.ExecuteAsync(dto, userId);

        // Get conversation to find other participants
        var conversation = await _conversationRepository.GetByIdAsync(dto.ConversationId);
        if (conversation != null)
        {
            // Notify all participants in the conversation
            await Clients.Group(dto.ConversationId).ReceiveMessage(message);

            // Also send to specific user connections (in case they're not in the group yet)
            foreach (var participantId in conversation.ParticipantIds)
            {
                if (participantId != userId)
                {
                    await Clients.User(participantId).ReceiveMessage(message);
                }
            }
        }
    }

    public async Task StartTyping(string conversationId)
    {
        var userId = GetCurrentUserId();

        // Update typing status
        await _conversationRepository.UpdateTypingStatusAsync(conversationId, userId, true);

        // Notify other participants
        await Clients.OthersInGroup(conversationId).UserTyping(conversationId, userId, true);
    }

    public async Task StopTyping(string conversationId)
    {
        var userId = GetCurrentUserId();

        // Update typing status
        await _conversationRepository.UpdateTypingStatusAsync(conversationId, userId, false);

        // Notify other participants
        await Clients.OthersInGroup(conversationId).UserTyping(conversationId, userId, false);
    }

    public async Task MarkConversationAsRead(string conversationId)
    {
        var userId = GetCurrentUserId();

        // Mark messages as read
        await _markAsReadUseCase.ExecuteAsync(conversationId, userId);

        // Notify other participants
        await Clients.OthersInGroup(conversationId).MessageRead(conversationId, "", userId);
    }
}
```

**Technologies**: SignalR Hub, WebSocket groups, real-time events
**Study needed**:

- 📚 SignalR Hubs in depth (2-3 hours)
  - Resource: https://learn.microsoft.com/en-us/aspnet/core/signalr/hubs
- 📚 SignalR Groups for targeted messaging (1-2 hours)
  - Resource: https://learn.microsoft.com/en-us/aspnet/core/signalr/groups

## Step 7.7: Create Messages Controller and Update Configuration

`Whispra.Api/Controllers/MessagesController.cs`:

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Whispra.Application.DTOs.Messages;
using Whispra.Application.UseCases.Messages.CreateConversation;
using Whispra.Application.UseCases.Messages.GetConversations;
using Whispra.Application.UseCases.Messages.GetMessages;

namespace Whispra.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly CreateConversationUseCase _createConversationUseCase;
    private readonly GetConversationsUseCase _getConversationsUseCase;
    private readonly GetMessagesUseCase _getMessagesUseCase;

    public MessagesController(
        CreateConversationUseCase createConversationUseCase,
        GetConversationsUseCase getConversationsUseCase,
        GetMessagesUseCase getMessagesUseCase)
    {
        _createConversationUseCase = createConversationUseCase;
        _getConversationsUseCase = getConversationsUseCase;
        _getMessagesUseCase = getMessagesUseCase;
    }

    private string GetCurrentUserId()
    {
        return User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("User not authenticated");
    }

    [HttpPost("conversations")]
    public async Task<ActionResult<ConversationResponseDto>> CreateConversation(
        [FromBody] CreateConversationDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _createConversationUseCase.ExecuteAsync(
                dto, GetCurrentUserId(), cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("conversations")]
    public async Task<ActionResult<List<ConversationResponseDto>>> GetConversations(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _getConversationsUseCase.ExecuteAsync(
            GetCurrentUserId(), page, pageSize, cancellationToken);
        return Ok(result);
    }

    [HttpGet("conversations/{conversationId}/messages")]
    public async Task<ActionResult<List<MessageResponseDto>>> GetMessages(
        string conversationId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _getMessagesUseCase.ExecuteAsync(
                conversationId, GetCurrentUserId(), page, pageSize, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}
```

Update `Whispra.Api/Extensions/ServiceCollectionExtensions.cs` to register messaging use cases:

```csharp
public static IServiceCollection AddApplicationServices(this IServiceCollection services)
{
    // Register use cases - Users
    services.AddScoped<RegisterUserUseCase>();

    // Register use cases - Auth
    services.AddScoped<LoginUseCase>();
    services.AddScoped<RefreshTokenUseCase>();

    // Register use cases - Communities
    services.AddScoped<CreateCommunityUseCase>();
    services.AddScoped<JoinCommunityUseCase>();
    services.AddScoped<LeaveCommunityUseCase>();
    services.AddScoped<UpdateMemberRoleUseCase>();
    services.AddScoped<CreateInviteUseCase>();

    // Register use cases - Posts
    services.AddScoped<CreatePostUseCase>();
    services.AddScoped<UpdatePostUseCase>();
    services.AddScoped<DeletePostUseCase>();
    services.AddScoped<GetHomeFeedUseCase>();
    services.AddScoped<CreateCommentUseCase>();
    services.AddScoped<AddReactionUseCase>();

    // Register use cases - Media
    services.AddScoped<GetUploadUrlUseCase>();
    services.AddScoped<ConfirmUploadUseCase>();
    services.AddScoped<DeleteMediaUseCase>();

    // Register use cases - Messages
    services.AddScoped<CreateConversationUseCase>();
    services.AddScoped<GetConversationsUseCase>();
    services.AddScoped<GetMessagesUseCase>();
    services.AddScoped<SendMessageUseCase>();
    services.AddScoped<MarkAsReadUseCase>();

    return services;
}

public static IServiceCollection AddInfrastructureServices(
    this IServiceCollection services,
    IConfiguration configuration)
{
    // Configure MongoDB
    services.Configure<MongoDbSettings>(
        configuration.GetSection(nameof(MongoDbSettings)));
    services.AddSingleton<MongoDbContext>();

    // Configure JWT
    services.Configure<JwtSettings>(
        configuration.GetSection(nameof(JwtSettings)));

    // Configure S3
    services.Configure<S3Settings>(
        configuration.GetSection(nameof(S3Settings)));

    // Register repositories - Users
    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

    // Register repositories - Communities
    services.AddScoped<ICommunityRepository, CommunityRepository>();
    services.AddScoped<ICommunityMemberRepository, CommunityMemberRepository>();
    services.AddScoped<ICommunityInviteRepository, CommunityInviteRepository>();

    // Register repositories - Posts
    services.AddScoped<IPostRepository, PostRepository>();
    services.AddScoped<ICommentRepository, CommentRepository>();
    services.AddScoped<IReactionRepository, ReactionRepository>();

    // Register repositories - Media
    services.AddScoped<IMediaRepository, MediaRepository>();

    // Register repositories - Messages
    services.AddScoped<IConversationRepository, ConversationRepository>();
    services.AddScoped<IMessageRepository, MessageRepository>();
    services.AddScoped<IUserPresenceRepository, UserPresenceRepository>();

    // Register services
    services.AddSingleton<IPasswordHasher, PasswordHasher>();
    services.AddSingleton<IJwtTokenService, JwtTokenService>();
    services.AddSingleton<IS3Service, S3Service>();

    return services;
}
```

Update `Whispra.Api/Program.cs` to add SignalR:

```csharp
using Whispra.Api.Extensions;
using Whispra.Api.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add SignalR
builder.Services.AddSignalR();

// Add application and infrastructure services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
              .WithOrigins("http://localhost:3000", "http://localhost:19006") // Frontend origins
              .AllowCredentials(); // Required for SignalR
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Authentication & Authorization middleware (order matters!)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Map SignalR hub
app.MapHub<ChatHub>("/hubs/chat");

app.Run();
```

**Technologies**: SignalR hub mapping, CORS for WebSockets
**Study needed**:

- 📚 SignalR CORS configuration (1 hour)
  - Resource: https://learn.microsoft.com/en-us/aspnet/core/signalr/security

## Step 7.8: Test Real-time Messaging

```bash
# Make sure all services are running
docker compose up -d

# Run the API
cd backend/Whispra.Api
dotnet run
```

Open Swagger: `https://localhost:7001/swagger`

### Test Flow (REST API):

**1. Login as two different users** to get two JWT tokens

**2. Create a direct conversation** (User 1):

```json
POST /api/Messages/conversations
{
  "type": 0,
  "participantIds": ["user2-id"]
}
```

**3. Get conversations** (User 1):

```
GET /api/Messages/conversations?page=1&pageSize=20
```

**4. Get messages in conversation**:

```
GET /api/Messages/conversations/{conversationId}/messages?page=1&pageSize=50
```

### Test SignalR (Using Postman or JavaScript client):

For testing SignalR, you'll need a SignalR client. Here's a simple JavaScript example:

**Create `test-signalr.html`**:

```html
<!DOCTYPE html>
<html>
  <head>
    <title>Whispra Chat Test</title>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/7.0.0/signalr.min.js"></script>
  </head>
  <body>
    <h1>Whispra Chat Test</h1>
    <div>
      <input
        type="text"
        id="token"
        placeholder="JWT Token"
        style="width: 500px;"
      />
      <button onclick="connect()">Connect</button>
      <button onclick="disconnect()">Disconnect</button>
    </div>
    <div>
      <input type="text" id="conversationId" placeholder="Conversation ID" />
      <button onclick="joinConversation()">Join</button>
    </div>
    <div>
      <input type="text" id="message" placeholder="Message" />
      <button onclick="sendMessage()">Send</button>
      <button onclick="startTyping()">Start Typing</button>
      <button onclick="stopTyping()">Stop Typing</button>
    </div>
    <div id="messages"></div>

    <script>
      let connection = null;

      function connect() {
        const token = document.getElementById("token").value;

        connection = new signalR.HubConnectionBuilder()
          .withUrl("https://localhost:7001/hubs/chat", {
            accessTokenFactory: () => token,
          })
          .build();

        connection.on("ReceiveMessage", (message) => {
          addLog("New message: " + JSON.stringify(message));
        });

        connection.on("UserTyping", (conversationId, userId, isTyping) => {
          addLog(
            `User ${userId} is ${
              isTyping ? "typing" : "stopped typing"
            } in ${conversationId}`
          );
        });

        connection.on("UserPresenceChanged", (userId, isOnline, lastSeenAt) => {
          addLog(`User ${userId} is now ${isOnline ? "online" : "offline"}`);
        });

        connection
          .start()
          .then(() => addLog("Connected!"))
          .catch((err) => addLog("Error: " + err));
      }

      function disconnect() {
        if (connection) {
          connection.stop();
          addLog("Disconnected");
        }
      }

      function joinConversation() {
        const conversationId = document.getElementById("conversationId").value;
        connection
          .invoke("JoinConversation", conversationId)
          .then(() => addLog("Joined conversation"))
          .catch((err) => addLog("Error: " + err));
      }

      function sendMessage() {
        const conversationId = document.getElementById("conversationId").value;
        const text = document.getElementById("message").value;

        connection
          .invoke("SendMessage", {
            conversationId: conversationId,
            textContent: text,
            mediaUrls: [],
            replyToMessageId: null,
          })
          .then(() => {
            addLog("Message sent");
            document.getElementById("message").value = "";
          })
          .catch((err) => addLog("Error: " + err));
      }

      function startTyping() {
        const conversationId = document.getElementById("conversationId").value;
        connection.invoke("StartTyping", conversationId);
      }

      function stopTyping() {
        const conversationId = document.getElementById("conversationId").value;
        connection.invoke("StopTyping", conversationId);
      }

      function addLog(message) {
        const div = document.getElementById("messages");
        div.innerHTML += `<p>${new Date().toLocaleTimeString()}: ${message}</p>`;
      }
    </script>
  </body>
</html>
```

**How to test**:

1. Open `test-signalr.html` in two browser tabs/windows
2. Login with two different users and copy their JWT tokens
3. Paste tokens in each tab and click "Connect"
4. Create a conversation via REST API and get the conversation ID
5. In both tabs, paste the conversation ID and click "Join"
6. Send messages from one tab and watch them appear in real-time in the other!
7. Test typing indicators
8. Close one tab and watch the presence change to offline

**Check MongoDB Compass**:

- `conversations` collection with participants and typing status
- `messages` collection with real-time messages
- `user_presences` collection tracking online/offline status

**Study needed**: ✅ Just testing

---

**🔄 CHECKPOINT - Real-time Messaging Complete!**

You now have:

- ✅ SignalR WebSocket connections
- ✅ Real-time direct messaging (DMs)
- ✅ Group conversations
- ✅ Typing indicators
- ✅ Online/offline presence tracking
- ✅ Message read receipts
- ✅ Conversation management (create, join, leave)
- ✅ Message history with pagination

Before moving to Phase 8, confirm:

1. ✅ Can you create conversations?
2. ✅ Can you send messages in real-time?
3. ✅ Do typing indicators work?
4. ✅ Does presence tracking show online/offline status?
5. ✅ Can you see unread message counts?

**Next Phase Preview**: We'll implement Notifications system (in-app notifications, push notifications preparation, notification preferences).

Ready to continue with Phase 8 (Notifications)?

---

# PHASE 8: Notifications System

**Goal**: Implement a comprehensive notifications system with in-app notifications, notification preferences, real-time delivery via SignalR, and preparation for push notifications (mobile/desktop).

## Step 8.1: Understanding Notifications Architecture

**What are Notifications?**

- **In-App Notifications**: Messages shown within the app (new follower, comment reply, mention, etc.)
- **Real-time Delivery**: Instant notification via SignalR when user is online
- **Persistent Storage**: Notifications saved in database for later retrieval
- **Notification Preferences**: User can control what notifications they receive
- **Push Notifications**: (Future) Mobile/desktop push when user is offline

**Notification Types We'll Build**:

- **NewFollower**: Someone followed you
- **PostLike**: Someone liked your post
- **PostComment**: Someone commented on your post
- **CommentReply**: Someone replied to your comment
- **Mention**: Someone mentioned you in a post/comment
- **CommunityInvite**: Someone invited you to a community
- **NewMessage**: New direct message (if not in conversation)

**Notification Flow**:

1. Action occurs (e.g., user likes a post)
2. System creates notification in database
3. If recipient is online → send via SignalR immediately
4. If recipient is offline → notification waits in database
5. User retrieves unread notifications on next login
6. User can mark notifications as read/unread

📚 **Study needed**: Notification system patterns (2-3 hours)

- Resource: https://www.youtube.com/watch?v=bBF0cE4Duv8 (Notification System Design)
- Resource: https://firebase.google.com/docs/cloud-messaging (For future push notifications)

## Step 8.2: Add Notification Entities to Domain Layer

```bash
cd backend/Whispra.Domain

# Create folders
mkdir -p Entities/Notifications
mkdir -p Enums
```

`Whispra.Domain/Enums/NotificationType.cs`:

```csharp
namespace Whispra.Domain.Enums;

public enum NotificationType
{
    NewFollower = 0,
    PostLike = 1,
    PostComment = 2,
    CommentReply = 3,
    Mention = 4,
    CommunityInvite = 5,
    NewMessage = 6,
    CommunityRoleChanged = 7,
    PostShared = 8
}
```

`Whispra.Domain/Entities/Notifications/Notification.cs`:

```csharp
using Whispra.Domain.Enums;

namespace Whispra.Domain.Entities.Notifications;

public class Notification : BaseEntity
{
    public string RecipientId { get; set; } = string.Empty;
    public string SenderId { get; set; } = string.Empty; // User who triggered the notification
    public NotificationType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }

    // Metadata for linking to related entities
    public string? RelatedEntityId { get; set; } // PostId, CommentId, CommunityId, etc.
    public string? RelatedEntityType { get; set; } // "Post", "Comment", "Community", etc.
    public Dictionary<string, string> AdditionalData { get; set; } = new(); // Extra JSON data

    // For grouping similar notifications
    public string? GroupKey { get; set; } // e.g., "post_likes_123" to group all likes on same post
}
```

`Whispra.Domain/Entities/Notifications/NotificationPreference.cs`:

```csharp
using Whispra.Domain.Enums;

namespace Whispra.Domain.Entities.Notifications;

public class NotificationPreference : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public Dictionary<NotificationType, bool> InAppEnabled { get; set; } = new();
    public Dictionary<NotificationType, bool> PushEnabled { get; set; } = new();
    public Dictionary<NotificationType, bool> EmailEnabled { get; set; } = new();

    // General settings
    public bool MuteAll { get; set; } = false;
    public List<string> MutedUserIds { get; set; } = new(); // Don't receive notifications from these users
    public List<string> MutedCommunityIds { get; set; } = new(); // Don't receive notifications from these communities
}
```

**Technologies**: Notification entities, user preferences
**Study needed**: ✅ None, straightforward entities

## Step 8.3: Add Notification DTOs and Interfaces to Application Layer

```bash
cd backend/Whispra.Application

# Create folders
mkdir -p DTOs/Notifications
mkdir -p UseCases/Notifications/Create
mkdir -p UseCases/Notifications/GetNotifications
mkdir -p UseCases/Notifications/MarkAsRead
mkdir -p UseCases/Notifications/UpdatePreferences
mkdir -p Interfaces/Repositories
mkdir -p Interfaces/Services
```

`Whispra.Application/DTOs/Notifications/NotificationResponseDto.cs`:

```csharp
using Whispra.Domain.Enums;

namespace Whispra.Application.DTOs.Notifications;

public record NotificationResponseDto(
    string Id,
    string RecipientId,
    string SenderId,
    NotificationType Type,
    string Title,
    string Message,
    bool IsRead,
    DateTime? ReadAt,
    string? RelatedEntityId,
    string? RelatedEntityType,
    Dictionary<string, string> AdditionalData,
    DateTime CreatedAt
);
```

`Whispra.Application/DTOs/Notifications/CreateNotificationDto.cs`:

```csharp
using Whispra.Domain.Enums;

namespace Whispra.Application.DTOs.Notifications;

public record CreateNotificationDto(
    string RecipientId,
    string SenderId,
    NotificationType Type,
    string Title,
    string Message,
    string? RelatedEntityId = null,
    string? RelatedEntityType = null,
    Dictionary<string, string>? AdditionalData = null,
    string? GroupKey = null
);
```

`Whispra.Application/DTOs/Notifications/NotificationPreferenceDto.cs`:

```csharp
using Whispra.Domain.Enums;

namespace Whispra.Application.DTOs.Notifications;

public record NotificationPreferenceDto(
    Dictionary<NotificationType, bool> InAppEnabled,
    Dictionary<NotificationType, bool> PushEnabled,
    Dictionary<NotificationType, bool> EmailEnabled,
    bool MuteAll,
    List<string> MutedUserIds,
    List<string> MutedCommunityIds
);
```

`Whispra.Application/DTOs/Notifications/UpdatePreferencesDto.cs`:

```csharp
using Whispra.Domain.Enums;

namespace Whispra.Application.DTOs.Notifications;

public record UpdatePreferencesDto(
    Dictionary<NotificationType, bool>? InAppEnabled = null,
    Dictionary<NotificationType, bool>? PushEnabled = null,
    Dictionary<NotificationType, bool>? EmailEnabled = null,
    bool? MuteAll = null,
    List<string>? MutedUserIds = null,
    List<string>? MutedCommunityIds = null
);
```

`Whispra.Application/Interfaces/Repositories/INotificationRepository.cs`:

```csharp
using Whispra.Domain.Entities.Notifications;
using Whispra.Domain.Enums;

namespace Whispra.Application.Interfaces.Repositories;

public interface INotificationRepository
{
    Task<Notification?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<List<Notification>> GetUserNotificationsAsync(string userId, bool? isRead, int skip, int limit, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountAsync(string userId, CancellationToken cancellationToken = default);
    Task<Notification> CreateAsync(Notification notification, CancellationToken cancellationToken = default);
    Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default);
    Task MarkAsReadAsync(string notificationId, CancellationToken cancellationToken = default);
    Task MarkAllAsReadAsync(string userId, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task DeleteOldNotificationsAsync(DateTime olderThan, CancellationToken cancellationToken = default);
}
```

`Whispra.Application/Interfaces/Repositories/INotificationPreferenceRepository.cs`:

```csharp
using Whispra.Domain.Entities.Notifications;

namespace Whispra.Application.Interfaces.Repositories;

public interface INotificationPreferenceRepository
{
    Task<NotificationPreference?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<NotificationPreference> CreateAsync(NotificationPreference preference, CancellationToken cancellationToken = default);
    Task UpdateAsync(NotificationPreference preference, CancellationToken cancellationToken = default);
    Task<NotificationPreference> GetOrCreateAsync(string userId, CancellationToken cancellationToken = default);
}
```

`Whispra.Application/Interfaces/Services/INotificationService.cs`:

```csharp
using Whispra.Application.DTOs.Notifications;

namespace Whispra.Application.Interfaces.Services;

public interface INotificationService
{
    Task SendNotificationAsync(CreateNotificationDto dto, CancellationToken cancellationToken = default);
    Task SendRealTimeNotificationAsync(string userId, NotificationResponseDto notification);
}
```

`Whispra.Application/Interfaces/Hubs/INotificationClient.cs`:

```csharp
using Whispra.Application.DTOs.Notifications;

namespace Whispra.Application.Interfaces.Hubs;

// Interface for client-side methods that the server can invoke
public interface INotificationClient
{
    Task ReceiveNotification(NotificationResponseDto notification);
    Task NotificationRead(string notificationId);
    Task UnreadCountChanged(int count);
}
```

**Technologies**: Notification DTOs, preference management
**Study needed**: ✅ None, follows established patterns

## Step 8.4: Create Notification Use Cases

`Whispra.Application/UseCases/Notifications/Create/CreateNotificationUseCase.cs`:

```csharp
using Whispra.Application.DTOs.Notifications;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Application.Interfaces.Services;
using Whispra.Domain.Entities.Notifications;

namespace Whispra.Application.UseCases.Notifications.Create;

public class CreateNotificationUseCase
{
    private readonly INotificationRepository _notificationRepository;
    private readonly INotificationPreferenceRepository _preferenceRepository;
    private readonly INotificationService _notificationService;

    public CreateNotificationUseCase(
        INotificationRepository notificationRepository,
        INotificationPreferenceRepository preferenceRepository,
        INotificationService notificationService)
    {
        _notificationRepository = notificationRepository;
        _preferenceRepository = preferenceRepository;
        _notificationService = notificationService;
    }

    public async Task<NotificationResponseDto?> ExecuteAsync(
        CreateNotificationDto dto,
        CancellationToken cancellationToken = default)
    {
        // Check user preferences
        var preferences = await _preferenceRepository.GetOrCreateAsync(dto.RecipientId, cancellationToken);

        // Check if notifications are muted
        if (preferences.MuteAll)
            return null;

        if (preferences.MutedUserIds.Contains(dto.SenderId))
            return null;

        // Check if this notification type is enabled
        if (preferences.InAppEnabled.ContainsKey(dto.Type) && !preferences.InAppEnabled[dto.Type])
            return null;

        // Create notification
        var notification = new Notification
        {
            RecipientId = dto.RecipientId,
            SenderId = dto.SenderId,
            Type = dto.Type,
            Title = dto.Title,
            Message = dto.Message,
            RelatedEntityId = dto.RelatedEntityId,
            RelatedEntityType = dto.RelatedEntityType,
            AdditionalData = dto.AdditionalData ?? new Dictionary<string, string>(),
            GroupKey = dto.GroupKey
        };

        var created = await _notificationRepository.CreateAsync(notification, cancellationToken);

        var response = new NotificationResponseDto(
            created.Id,
            created.RecipientId,
            created.SenderId,
            created.Type,
            created.Title,
            created.Message,
            created.IsRead,
            created.ReadAt,
            created.RelatedEntityId,
            created.RelatedEntityType,
            created.AdditionalData,
            created.CreatedAt
        );

        // Send real-time notification if user is online
        await _notificationService.SendRealTimeNotificationAsync(dto.RecipientId, response);

        return response;
    }
}
```

`Whispra.Application/UseCases/Notifications/GetNotifications/GetNotificationsUseCase.cs`:

```csharp
using Whispra.Application.DTOs.Notifications;
using Whispra.Application.Interfaces.Repositories;

namespace Whispra.Application.UseCases.Notifications.GetNotifications;

public class GetNotificationsUseCase
{
    private readonly INotificationRepository _notificationRepository;

    public GetNotificationsUseCase(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<(List<NotificationResponseDto> Notifications, int UnreadCount)> ExecuteAsync(
        string userId,
        bool? isRead = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var skip = (page - 1) * pageSize;
        var notifications = await _notificationRepository.GetUserNotificationsAsync(
            userId, isRead, skip, pageSize, cancellationToken);

        var unreadCount = await _notificationRepository.GetUnreadCountAsync(userId, cancellationToken);

        var response = notifications.Select(n => new NotificationResponseDto(
            n.Id,
            n.RecipientId,
            n.SenderId,
            n.Type,
            n.Title,
            n.Message,
            n.IsRead,
            n.ReadAt,
            n.RelatedEntityId,
            n.RelatedEntityType,
            n.AdditionalData,
            n.CreatedAt
        )).ToList();

        return (response, unreadCount);
    }
}
```

`Whispra.Application/UseCases/Notifications/MarkAsRead/MarkAsReadUseCase.cs`:

```csharp
using Whispra.Application.Interfaces.Repositories;
using Whispra.Application.Interfaces.Services;

namespace Whispra.Application.UseCases.Notifications.MarkAsRead;

public class MarkNotificationAsReadUseCase
{
    private readonly INotificationRepository _notificationRepository;

    public MarkNotificationAsReadUseCase(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task ExecuteAsync(
        string notificationId,
        string currentUserId,
        CancellationToken cancellationToken = default)
    {
        var notification = await _notificationRepository.GetByIdAsync(notificationId, cancellationToken);
        if (notification == null)
        {
            throw new InvalidOperationException("Notification not found");
        }

        if (notification.RecipientId != currentUserId)
        {
            throw new UnauthorizedAccessException("You don't own this notification");
        }

        await _notificationRepository.MarkAsReadAsync(notificationId, cancellationToken);
    }

    public async Task MarkAllAsReadAsync(
        string currentUserId,
        CancellationToken cancellationToken = default)
    {
        await _notificationRepository.MarkAllAsReadAsync(currentUserId, cancellationToken);
    }
}
```

`Whispra.Application/UseCases/Notifications/UpdatePreferences/UpdatePreferencesUseCase.cs`:

```csharp
using Whispra.Application.DTOs.Notifications;
using Whispra.Application.Interfaces.Repositories;

namespace Whispra.Application.UseCases.Notifications.UpdatePreferences;

public class UpdatePreferencesUseCase
{
    private readonly INotificationPreferenceRepository _preferenceRepository;

    public UpdatePreferencesUseCase(INotificationPreferenceRepository preferenceRepository)
    {
        _preferenceRepository = preferenceRepository;
    }

    public async Task<NotificationPreferenceDto> ExecuteAsync(
        UpdatePreferencesDto dto,
        string currentUserId,
        CancellationToken cancellationToken = default)
    {
        var preferences = await _preferenceRepository.GetOrCreateAsync(currentUserId, cancellationToken);

        // Update only provided fields
        if (dto.InAppEnabled != null)
        {
            foreach (var kvp in dto.InAppEnabled)
            {
                preferences.InAppEnabled[kvp.Key] = kvp.Value;
            }
        }

        if (dto.PushEnabled != null)
        {
            foreach (var kvp in dto.PushEnabled)
            {
                preferences.PushEnabled[kvp.Key] = kvp.Value;
            }
        }

        if (dto.EmailEnabled != null)
        {
            foreach (var kvp in dto.EmailEnabled)
            {
                preferences.EmailEnabled[kvp.Key] = kvp.Value;
            }
        }

        if (dto.MuteAll.HasValue)
            preferences.MuteAll = dto.MuteAll.Value;

        if (dto.MutedUserIds != null)
            preferences.MutedUserIds = dto.MutedUserIds;

        if (dto.MutedCommunityIds != null)
            preferences.MutedCommunityIds = dto.MutedCommunityIds;

        await _preferenceRepository.UpdateAsync(preferences, cancellationToken);

        return new NotificationPreferenceDto(
            preferences.InAppEnabled,
            preferences.PushEnabled,
            preferences.EmailEnabled,
            preferences.MuteAll,
            preferences.MutedUserIds,
            preferences.MutedCommunityIds
        );
    }

    public async Task<NotificationPreferenceDto> GetPreferencesAsync(
        string currentUserId,
        CancellationToken cancellationToken = default)
    {
        var preferences = await _preferenceRepository.GetOrCreateAsync(currentUserId, cancellationToken);

        return new NotificationPreferenceDto(
            preferences.InAppEnabled,
            preferences.PushEnabled,
            preferences.EmailEnabled,
            preferences.MuteAll,
            preferences.MutedUserIds,
            preferences.MutedCommunityIds
        );
    }
}
```

**Technologies**: Notification creation logic, preference filtering
**Study needed**: ✅ None, builds on established patterns

## Step 8.5: Implement Infrastructure Layer for Notifications

```bash
cd backend/Whispra.Infrastructure/Persistence/Repositories
```

`Whispra.Infrastructure/Persistence/Repositories/NotificationRepository.cs`:

```csharp
using MongoDB.Driver;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Domain.Entities.Notifications;
using Whispra.Infrastructure.Persistence.MongoDB;

namespace Whispra.Infrastructure.Persistence.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly MongoDbContext _context;

    public NotificationRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<Notification?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.Notifications
            .Find(n => n.Id == id && !n.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<Notification>> GetUserNotificationsAsync(
        string userId, bool? isRead, int skip, int limit, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Notification>.Filter.And(
            Builders<Notification>.Filter.Eq(n => n.RecipientId, userId),
            Builders<Notification>.Filter.Eq(n => n.IsDeleted, false)
        );

        if (isRead.HasValue)
        {
            filter = Builders<Notification>.Filter.And(
                filter,
                Builders<Notification>.Filter.Eq(n => n.IsRead, isRead.Value)
            );
        }

        return await _context.Notifications
            .Find(filter)
            .SortByDescending(n => n.CreatedAt)
            .Skip(skip)
            .Limit(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetUnreadCountAsync(string userId, CancellationToken cancellationToken = default)
    {
        var count = await _context.Notifications
            .CountDocumentsAsync(
                n => n.RecipientId == userId && !n.IsRead && !n.IsDeleted,
                cancellationToken: cancellationToken);

        return (int)count;
    }

    public async Task<Notification> CreateAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        await _context.Notifications.InsertOneAsync(notification, cancellationToken: cancellationToken);
        return notification;
    }

    public async Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        notification.UpdatedAt = DateTime.UtcNow;
        await _context.Notifications.ReplaceOneAsync(
            n => n.Id == notification.Id,
            notification,
            cancellationToken: cancellationToken);
    }

    public async Task MarkAsReadAsync(string notificationId, CancellationToken cancellationToken = default)
    {
        var update = Builders<Notification>.Update
            .Set(n => n.IsRead, true)
            .Set(n => n.ReadAt, DateTime.UtcNow)
            .Set(n => n.UpdatedAt, DateTime.UtcNow);

        await _context.Notifications.UpdateOneAsync(
            n => n.Id == notificationId,
            update,
            cancellationToken: cancellationToken);
    }

    public async Task MarkAllAsReadAsync(string userId, CancellationToken cancellationToken = default)
    {
        var update = Builders<Notification>.Update
            .Set(n => n.IsRead, true)
            .Set(n => n.ReadAt, DateTime.UtcNow)
            .Set(n => n.UpdatedAt, DateTime.UtcNow);

        await _context.Notifications.UpdateManyAsync(
            n => n.RecipientId == userId && !n.IsRead && !n.IsDeleted,
            update,
            cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var update = Builders<Notification>.Update
            .Set(n => n.IsDeleted, true)
            .Set(n => n.DeletedAt, DateTime.UtcNow);

        await _context.Notifications.UpdateOneAsync(
            n => n.Id == id,
            update,
            cancellationToken: cancellationToken);
    }

    public async Task DeleteOldNotificationsAsync(DateTime olderThan, CancellationToken cancellationToken = default)
    {
        var update = Builders<Notification>.Update
            .Set(n => n.IsDeleted, true)
            .Set(n => n.DeletedAt, DateTime.UtcNow);

        await _context.Notifications.UpdateManyAsync(
            n => n.CreatedAt < olderThan && n.IsRead,
            update,
            cancellationToken: cancellationToken);
    }
}
```

`Whispra.Infrastructure/Persistence/Repositories/NotificationPreferenceRepository.cs`:

```csharp
using MongoDB.Driver;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Domain.Entities.Notifications;
using Whispra.Domain.Enums;
using Whispra.Infrastructure.Persistence.MongoDB;

namespace Whispra.Infrastructure.Persistence.Repositories;

public class NotificationPreferenceRepository : INotificationPreferenceRepository
{
    private readonly MongoDbContext _context;

    public NotificationPreferenceRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<NotificationPreference?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.NotificationPreferences
            .Find(p => p.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<NotificationPreference> CreateAsync(NotificationPreference preference, CancellationToken cancellationToken = default)
    {
        await _context.NotificationPreferences.InsertOneAsync(preference, cancellationToken: cancellationToken);
        return preference;
    }

    public async Task UpdateAsync(NotificationPreference preference, CancellationToken cancellationToken = default)
    {
        preference.UpdatedAt = DateTime.UtcNow;
        await _context.NotificationPreferences.ReplaceOneAsync(
            p => p.UserId == preference.UserId,
            preference,
            cancellationToken: cancellationToken);
    }

    public async Task<NotificationPreference> GetOrCreateAsync(string userId, CancellationToken cancellationToken = default)
    {
        var existing = await GetByUserIdAsync(userId, cancellationToken);
        if (existing != null)
            return existing;

        // Create default preferences (all enabled)
        var defaultPreferences = new NotificationPreference
        {
            UserId = userId,
            InAppEnabled = Enum.GetValues<NotificationType>()
                .ToDictionary(t => t, t => true),
            PushEnabled = Enum.GetValues<NotificationType>()
                .ToDictionary(t => t, t => true),
            EmailEnabled = Enum.GetValues<NotificationType>()
                .ToDictionary(t => t, t => false) // Email off by default
        };

        return await CreateAsync(defaultPreferences, cancellationToken);
    }
}
```

`Whispra.Infrastructure/Services/NotificationService.cs`:

```csharp
using Microsoft.AspNetCore.SignalR;
using Whispra.Application.DTOs.Notifications;
using Whispra.Application.Interfaces.Hubs;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Application.Interfaces.Services;

namespace Whispra.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly INotificationPreferenceRepository _preferenceRepository;
    private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;

    public NotificationService(
        INotificationRepository notificationRepository,
        INotificationPreferenceRepository preferenceRepository,
        IHubContext<NotificationHub, INotificationClient> hubContext)
    {
        _notificationRepository = notificationRepository;
        _preferenceRepository = preferenceRepository;
        _hubContext = hubContext;
    }

    public async Task SendNotificationAsync(CreateNotificationDto dto, CancellationToken cancellationToken = default)
    {
        // Check preferences
        var preferences = await _preferenceRepository.GetOrCreateAsync(dto.RecipientId, cancellationToken);

        if (preferences.MuteAll || preferences.MutedUserIds.Contains(dto.SenderId))
            return;

        if (preferences.InAppEnabled.ContainsKey(dto.Type) && !preferences.InAppEnabled[dto.Type])
            return;

        // Create notification entity
        var notification = new Domain.Entities.Notifications.Notification
        {
            RecipientId = dto.RecipientId,
            SenderId = dto.SenderId,
            Type = dto.Type,
            Title = dto.Title,
            Message = dto.Message,
            RelatedEntityId = dto.RelatedEntityId,
            RelatedEntityType = dto.RelatedEntityType,
            AdditionalData = dto.AdditionalData ?? new Dictionary<string, string>(),
            GroupKey = dto.GroupKey
        };

        var created = await _notificationRepository.CreateAsync(notification, cancellationToken);

        var response = new NotificationResponseDto(
            created.Id,
            created.RecipientId,
            created.SenderId,
            created.Type,
            created.Title,
            created.Message,
            created.IsRead,
            created.ReadAt,
            created.RelatedEntityId,
            created.RelatedEntityType,
            created.AdditionalData,
            created.CreatedAt
        );

        // Send real-time
        await SendRealTimeNotificationAsync(dto.RecipientId, response);
    }

    public async Task SendRealTimeNotificationAsync(string userId, NotificationResponseDto notification)
    {
        try
        {
            await _hubContext.Clients.User(userId).ReceiveNotification(notification);
        }
        catch
        {
            // User not connected, notification is already saved in DB
        }
    }
}
```

Update `Whispra.Infrastructure/Persistence/MongoDB/MongoDbContext.cs`:

```csharp
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Whispra.Domain.Entities.Auth;
using Whispra.Domain.Entities.Communities;
using Whispra.Domain.Entities.Media;
using Whispra.Domain.Entities.Messages;
using Whispra.Domain.Entities.Notifications;
using Whispra.Domain.Entities.Posts;
using Whispra.Domain.Entities.Users;
using Whispra.Infrastructure.Configuration;

namespace Whispra.Infrastructure.Persistence.MongoDB;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<User> Users => _database.GetCollection<User>("users");
    public IMongoCollection<RefreshToken> RefreshTokens => _database.GetCollection<RefreshToken>("refresh_tokens");
    public IMongoCollection<Community> Communities => _database.GetCollection<Community>("communities");
    public IMongoCollection<CommunityMember> CommunityMembers => _database.GetCollection<CommunityMember>("community_members");
    public IMongoCollection<CommunityInvite> CommunityInvites => _database.GetCollection<CommunityInvite>("community_invites");
    public IMongoCollection<Post> Posts => _database.GetCollection<Post>("posts");
    public IMongoCollection<Comment> Comments => _database.GetCollection<Comment>("comments");
    public IMongoCollection<Reaction> Reactions => _database.GetCollection<Reaction>("reactions");
    public IMongoCollection<Media> Media => _database.GetCollection<Media>("media");
    public IMongoCollection<Conversation> Conversations => _database.GetCollection<Conversation>("conversations");
    public IMongoCollection<Message> Messages => _database.GetCollection<Message>("messages");
    public IMongoCollection<UserPresence> UserPresences => _database.GetCollection<UserPresence>("user_presences");
    public IMongoCollection<Notification> Notifications => _database.GetCollection<Notification>("notifications");
    public IMongoCollection<NotificationPreference> NotificationPreferences => _database.GetCollection<NotificationPreference>("notification_preferences");
}
```

**Technologies**: MongoDB queries for notifications, real-time delivery via SignalR
**Study needed**: ✅ None, builds on established patterns

## Step 8.6: Create Notification Hub and Controller

```bash
cd backend/Whispra.Api/Hubs
```

`Whispra.Api/Hubs/NotificationHub.cs`:

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Whispra.Application.Interfaces.Hubs;
using Whispra.Application.Interfaces.Repositories;

namespace Whispra.Api.Hubs;

[Authorize]
public class NotificationHub : Hub<INotificationClient>
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationHub(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    private string GetCurrentUserId()
    {
        return Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? throw new HubException("User not authenticated");
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetCurrentUserId();

        // Send unread count on connection
        var unreadCount = await _notificationRepository.GetUnreadCountAsync(userId);
        await Clients.Caller.UnreadCountChanged(unreadCount);

        await base.OnConnectedAsync();
    }

    public async Task<int> GetUnreadCount()
    {
        var userId = GetCurrentUserId();
        return await _notificationRepository.GetUnreadCountAsync(userId);
    }
}
```

```bash
cd backend/Whispra.Api/Controllers
```

`Whispra.Api/Controllers/NotificationsController.cs`:

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Whispra.Application.DTOs.Notifications;
using Whispra.Application.UseCases.Notifications.GetNotifications;
using Whispra.Application.UseCases.Notifications.MarkAsRead;
using Whispra.Application.UseCases.Notifications.UpdatePreferences;

namespace Whispra.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly GetNotificationsUseCase _getNotificationsUseCase;
    private readonly MarkNotificationAsReadUseCase _markAsReadUseCase;
    private readonly UpdatePreferencesUseCase _updatePreferencesUseCase;

    public NotificationsController(
        GetNotificationsUseCase getNotificationsUseCase,
        MarkNotificationAsReadUseCase markAsReadUseCase,
        UpdatePreferencesUseCase updatePreferencesUseCase)
    {
        _getNotificationsUseCase = getNotificationsUseCase;
        _markAsReadUseCase = markAsReadUseCase;
        _updatePreferencesUseCase = updatePreferencesUseCase;
    }

    private string GetCurrentUserId()
    {
        return User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("User not authenticated");
    }

    [HttpGet]
    public async Task<ActionResult<object>> GetNotifications(
        [FromQuery] bool? isRead = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var (notifications, unreadCount) = await _getNotificationsUseCase.ExecuteAsync(
            GetCurrentUserId(), isRead, page, pageSize, cancellationToken);

        return Ok(new
        {
            notifications,
            unreadCount,
            page,
            pageSize,
            hasMore = notifications.Count == pageSize
        });
    }

    [HttpPut("{notificationId}/read")]
    public async Task<ActionResult> MarkAsRead(
        string notificationId,
        CancellationToken cancellationToken)
    {
        try
        {
            await _markAsReadUseCase.ExecuteAsync(notificationId, GetCurrentUserId(), cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [HttpPut("read-all")]
    public async Task<ActionResult> MarkAllAsRead(CancellationToken cancellationToken)
    {
        await _markAsReadUseCase.MarkAllAsReadAsync(GetCurrentUserId(), cancellationToken);
        return NoContent();
    }

    [HttpGet("preferences")]
    public async Task<ActionResult<NotificationPreferenceDto>> GetPreferences(
        CancellationToken cancellationToken)
    {
        var preferences = await _updatePreferencesUseCase.GetPreferencesAsync(
            GetCurrentUserId(), cancellationToken);
        return Ok(preferences);
    }

    [HttpPut("preferences")]
    public async Task<ActionResult<NotificationPreferenceDto>> UpdatePreferences(
        [FromBody] UpdatePreferencesDto dto,
        CancellationToken cancellationToken)
    {
        var preferences = await _updatePreferencesUseCase.ExecuteAsync(
            dto, GetCurrentUserId(), cancellationToken);
        return Ok(preferences);
    }
}
```

**Technologies**: SignalR notification hub, REST API for notification management
**Study needed**: ✅ None, follows established patterns

## Step 8.7: Update Service Registration and Program.cs

Update `Whispra.Api/Extensions/ServiceCollectionExtensions.cs`:

```csharp
// In AddApplicationServices method, add:
services.AddScoped<CreateNotificationUseCase>();
services.AddScoped<GetNotificationsUseCase>();
services.AddScoped<MarkNotificationAsReadUseCase>();
services.AddScoped<UpdatePreferencesUseCase>();

// In AddInfrastructureServices method, add:
services.AddScoped<INotificationRepository, NotificationRepository>();
services.AddScoped<INotificationPreferenceRepository, NotificationPreferenceRepository>();
services.AddSingleton<INotificationService, NotificationService>();
```

Update `Whispra.Api/Program.cs`:

```csharp
using Whispra.Api.Extensions;
using Whispra.Api.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add SignalR
builder.Services.AddSignalR();

// Add application and infrastructure services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
              .WithOrigins("http://localhost:3000", "http://localhost:19006")
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Authentication & Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Map SignalR hubs
app.MapHub<ChatHub>("/hubs/chat");
app.MapHub<NotificationHub>("/hubs/notifications");

app.Run();
```

**Technologies**: Service registration, SignalR hub mapping
**Study needed**: ✅ None

## Step 8.8: Test Notifications System

```bash
# Make sure all services are running
docker compose up -d

# Run the API
cd backend/Whispra.Api
dotnet run
```

Open Swagger: `https://localhost:7001/swagger`

### Test Flow:

**1. Get notification preferences**:

```
GET /api/Notifications/preferences
```

**2. Update preferences** (mute all notifications):

```json
PUT /api/Notifications/preferences
{
  "muteAll": true
}
```

**3. Update preferences** (enable only certain types):

```json
PUT /api/Notifications/preferences
{
  "inAppEnabled": {
    "1": true,
    "2": true,
    "3": false
  }
}
```

**4. Get notifications**:

```
GET /api/Notifications?page=1&pageSize=20
```

**5. Get only unread notifications**:

```
GET /api/Notifications?isRead=false&page=1&pageSize=20
```

**6. Mark notification as read**:

```
PUT /api/Notifications/{notificationId}/read
```

**7. Mark all as read**:

```
PUT /api/Notifications/read-all
```

### Test Real-time Notifications (HTML Client):

**Create `test-notifications.html`**:

```html
<!DOCTYPE html>
<html>
  <head>
    <title>Whispra Notifications Test</title>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/7.0.0/signalr.min.js"></script>
  </head>
  <body>
    <h1>Whispra Notifications Test</h1>
    <div>
      <input
        type="text"
        id="token"
        placeholder="JWT Token"
        style="width: 500px;"
      />
      <button onclick="connect()">Connect</button>
      <button onclick="disconnect()">Disconnect</button>
    </div>
    <div>
      <h3>Unread Count: <span id="unreadCount">0</span></h3>
    </div>
    <div id="notifications"></div>

    <script>
      let connection = null;

      function connect() {
        const token = document.getElementById("token").value;

        connection = new signalR.HubConnectionBuilder()
          .withUrl("https://localhost:7001/hubs/notifications", {
            accessTokenFactory: () => token,
          })
          .build();

        connection.on("ReceiveNotification", (notification) => {
          addNotification(notification);
        });

        connection.on("UnreadCountChanged", (count) => {
          document.getElementById("unreadCount").textContent = count;
        });

        connection
          .start()
          .then(() => addLog("Connected to notifications!"))
          .catch((err) => addLog("Error: " + err));
      }

      function disconnect() {
        if (connection) {
          connection.stop();
          addLog("Disconnected");
        }
      }

      function addNotification(notification) {
        const div = document.getElementById("notifications");
        const notifHtml = `
                <div style="border: 1px solid #ccc; padding: 10px; margin: 10px 0; border-radius: 5px;">
                    <strong>${notification.title}</strong><br/>
                    ${notification.message}<br/>
                    <small>Type: ${notification.type} | ${new Date(
          notification.createdAt
        ).toLocaleString()}</small>
                </div>
            `;
        div.innerHTML = notifHtml + div.innerHTML;
      }

      function addLog(message) {
        const div = document.getElementById("notifications");
        div.innerHTML += `<p><em>${new Date().toLocaleTimeString()}: ${message}</em></p>`;
      }
    </script>
  </body>
</html>
```

**How to test**:

1. Open `test-notifications.html` in a browser
2. Login and paste your JWT token
3. Click "Connect"
4. In another tab, trigger actions that create notifications (like a post, comment, etc.)
5. Watch notifications appear in real-time!
6. Check the unread count update automatically

**Check MongoDB Compass**:

- `notifications` collection with all notifications
- `notification_preferences` collection with user preferences

**Study needed**: ✅ Just testing

---

**🔄 CHECKPOINT - Notifications System Complete!**

You now have:

- ✅ In-app notifications stored in MongoDB
- ✅ Real-time notification delivery via SignalR
- ✅ Notification preferences (mute all, mute users, enable/disable by type)
- ✅ Unread count tracking
- ✅ Mark as read/unread functionality
- ✅ Notification types for all major events
- ✅ Ready for push notification integration (Firebase/APNS)

Before moving to Phase 9, confirm:

1. ✅ Can you receive notifications in real-time?
2. ✅ Can you update notification preferences?
3. ✅ Does the unread count update correctly?
4. ✅ Can you mark notifications as read?
5. ✅ Do muted users/communities work?

**Next Phase Preview**: We'll implement Moderation & Safety features (content reporting, user blocking, spam detection, rate limiting).

---

# PHASE 9: Moderation & Safety

**Goal**: Implement comprehensive moderation and safety features including content reporting, user blocking, content moderation tools, basic spam detection, and rate limiting to keep the platform safe.

## Step 9.1: Understanding Moderation & Safety Architecture

**Why Moderation is Critical?**

- **User Safety**: Protect users from harassment, spam, and harmful content
- **Content Quality**: Maintain community standards and guidelines
- **Legal Compliance**: Remove illegal content (hate speech, CSAM, etc.)
- **Platform Trust**: Users feel safe = users stay engaged

**Key Features We'll Build**:

1. **Content Reporting**: Users can report posts, comments, users, or communities
2. **User Blocking**: Users can block others (prevents all interactions)
3. **Moderation Queue**: Moderators/admins review reports and take action
4. **Automated Detection**: Basic spam/profanity detection
5. **Rate Limiting**: Prevent spam and abuse (e.g., max 10 posts per hour)
6. **Moderation Actions**: Warn, mute, suspend, ban users; hide/delete content

**Moderation Roles**:

- **User**: Can report content and block users
- **Moderator**: Can review reports in their communities and take action
- **Admin**: Can review all reports and take platform-wide action

📚 **Study needed**: Content moderation patterns (2-3 hours)

- Resource: https://www.nngroup.com/articles/content-moderation-ux/
- Resource: https://transparency.fb.com/policies/community-standards/
- Resource: https://blog.cloudflare.com/rate-limiting-with-cloudflare-workers/

## Step 9.2: Add Moderation Entities to Domain Layer

```bash
cd backend/Whispra.Domain

# Create folders
mkdir -p Entities/Moderation
mkdir -p Enums
```

`Whispra.Domain/Enums/ReportReason.cs`:

```csharp
namespace Whispra.Domain.Enums;

public enum ReportReason
{
    Spam = 0,
    Harassment = 1,
    HateSpeech = 2,
    Violence = 3,
    SexualContent = 4,
    Misinformation = 5,
    IntellectualProperty = 6,
    SelfHarm = 7,
    Other = 99
}
```

`Whispra.Domain/Enums/ReportStatus.cs`:

```csharp
namespace Whispra.Domain.Enums;

public enum ReportStatus
{
    Pending = 0,      // Awaiting review
    UnderReview = 1,  // Being reviewed by moderator
    Resolved = 2,     // Action taken
    Dismissed = 3     // No action needed
}
```

`Whispra.Domain/Enums/ReportTargetType.cs`:

```csharp
namespace Whispra.Domain.Enums;

public enum ReportTargetType
{
    Post = 0,
    Comment = 1,
    User = 2,
    Community = 3,
    Message = 4
}
```

`Whispra.Domain/Enums/ModerationActionType.cs`:

```csharp
namespace Whispra.Domain.Enums;

public enum ModerationActionType
{
    NoAction = 0,
    Warning = 1,         // Send warning to user
    ContentHidden = 2,   // Hide content (soft delete)
    ContentDeleted = 3,  // Permanently delete content
    UserMuted = 4,       // Mute user for X hours (can't post/comment)
    UserSuspended = 5,   // Suspend user for X days (can't login)
    UserBanned = 6       // Permanently ban user
}
```

`Whispra.Domain/Entities/Moderation/Report.cs`:

```csharp
using Whispra.Domain.Enums;

namespace Whispra.Domain.Entities.Moderation;

public class Report : BaseEntity
{
    public string ReporterId { get; set; } = string.Empty; // User who filed the report
    public ReportTargetType TargetType { get; set; }
    public string TargetId { get; set; } = string.Empty; // PostId, CommentId, UserId, etc.
    public string? TargetOwnerId { get; set; } // Owner of the reported content/user
    public ReportReason Reason { get; set; }
    public string? Description { get; set; } // Additional details from reporter
    public ReportStatus Status { get; set; } = ReportStatus.Pending;

    // Review details
    public string? ReviewedBy { get; set; } // ModeratorId
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewNotes { get; set; }
    public ModerationActionType? ActionTaken { get; set; }

    // Metadata
    public string? CommunityId { get; set; } // If report is community-specific
    public Dictionary<string, string> Metadata { get; set; } = new(); // Extra context
}
```

`Whispra.Domain/Entities/Moderation/UserBlock.cs`:

```csharp
namespace Whispra.Domain.Entities.Moderation;

public class UserBlock : BaseEntity
{
    public string BlockerId { get; set; } = string.Empty; // User who blocked
    public string BlockedUserId { get; set; } = string.Empty; // User being blocked
    public DateTime BlockedAt { get; set; } = DateTime.UtcNow;
}
```

`Whispra.Domain/Entities/Moderation/ModerationAction.cs`:

```csharp
using Whispra.Domain.Enums;

namespace Whispra.Domain.Entities.Moderation;

public class ModerationAction : BaseEntity
{
    public string ModeratorId { get; set; } = string.Empty;
    public string TargetUserId { get; set; } = string.Empty; // User being moderated
    public ModerationActionType ActionType { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }

    // For temporary actions
    public DateTime? ExpiresAt { get; set; } // For mutes/suspensions
    public bool IsActive { get; set; } = true;

    // Links to report if action was from report
    public string? ReportId { get; set; }
    public string? CommunityId { get; set; } // If community-specific moderation
}
```

**Technologies**: Moderation entities, enums for safety
**Study needed**: ✅ None, straightforward entities

## Step 9.3: Add Moderation DTOs and Interfaces to Application Layer

```bash
cd backend/Whispra.Application

# Create folders
mkdir -p DTOs/Moderation
mkdir -p UseCases/Moderation/CreateReport
mkdir -p UseCases/Moderation/ReviewReport
mkdir -p UseCases/Moderation/BlockUser
mkdir -p UseCases/Moderation/GetReports
mkdir -p Interfaces/Repositories
mkdir -p Interfaces/Services
```

`Whispra.Application/DTOs/Moderation/CreateReportDto.cs`:

```csharp
using Whispra.Domain.Enums;

namespace Whispra.Application.DTOs.Moderation;

public record CreateReportDto(
    ReportTargetType TargetType,
    string TargetId,
    ReportReason Reason,
    string? Description = null
);
```

`Whispra.Application/DTOs/Moderation/ReportResponseDto.cs`:

```csharp
using Whispra.Domain.Enums;

namespace Whispra.Application.DTOs.Moderation;

public record ReportResponseDto(
    string Id,
    string ReporterId,
    ReportTargetType TargetType,
    string TargetId,
    string? TargetOwnerId,
    ReportReason Reason,
    string? Description,
    ReportStatus Status,
    string? ReviewedBy,
    DateTime? ReviewedAt,
    string? ReviewNotes,
    ModerationActionType? ActionTaken,
    DateTime CreatedAt
);
```

`Whispra.Application/DTOs/Moderation/ReviewReportDto.cs`:

```csharp
using Whispra.Domain.Enums;

namespace Whispra.Application.DTOs.Moderation;

public record ReviewReportDto(
    ReportStatus Status,
    ModerationActionType ActionTaken,
    string? ReviewNotes = null,
    int? ActionDurationHours = null // For temporary mutes/suspensions
);
```

`Whispra.Application/DTOs/Moderation/BlockUserDto.cs`:

```csharp
namespace Whispra.Application.DTOs.Moderation;

public record BlockUserDto(
    string UserId
);
```

`Whispra.Application/DTOs/Moderation/ModerationActionResponseDto.cs`:

```csharp
using Whispra.Domain.Enums;

namespace Whispra.Application.DTOs.Moderation;

public record ModerationActionResponseDto(
    string Id,
    string ModeratorId,
    string TargetUserId,
    ModerationActionType ActionType,
    string Reason,
    string? Notes,
    DateTime? ExpiresAt,
    bool IsActive,
    DateTime CreatedAt
);
```

`Whispra.Application/Interfaces/Repositories/IReportRepository.cs`:

```csharp
using Whispra.Domain.Entities.Moderation;
using Whispra.Domain.Enums;

namespace Whispra.Application.Interfaces.Repositories;

public interface IReportRepository
{
    Task<Report?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Report> CreateAsync(Report report, CancellationToken cancellationToken = default);
    Task UpdateAsync(Report report, CancellationToken cancellationToken = default);
    Task<List<Report>> GetReportsByStatusAsync(ReportStatus status, int skip, int limit, CancellationToken cancellationToken = default);
    Task<List<Report>> GetReportsByTargetAsync(ReportTargetType targetType, string targetId, CancellationToken cancellationToken = default);
    Task<List<Report>> GetCommunityReportsAsync(string communityId, ReportStatus? status, int skip, int limit, CancellationToken cancellationToken = default);
    Task<bool> HasUserReportedAsync(string reporterId, string targetId, CancellationToken cancellationToken = default);
    Task<int> GetPendingCountAsync(CancellationToken cancellationToken = default);
}
```

`Whispra.Application/Interfaces/Repositories/IUserBlockRepository.cs`:

```csharp
using Whispra.Domain.Entities.Moderation;

namespace Whispra.Application.Interfaces.Repositories;

public interface IUserBlockRepository
{
    Task<UserBlock?> GetBlockAsync(string blockerId, string blockedUserId, CancellationToken cancellationToken = default);
    Task<UserBlock> CreateAsync(UserBlock block, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<List<UserBlock>> GetUserBlocksAsync(string userId, int skip, int limit, CancellationToken cancellationToken = default);
    Task<bool> IsBlockedAsync(string blockerId, string blockedUserId, CancellationToken cancellationToken = default);
    Task<List<string>> GetBlockedUserIdsAsync(string userId, CancellationToken cancellationToken = default);
}
```

`Whispra.Application/Interfaces/Repositories/IModerationActionRepository.cs`:

```csharp
using Whispra.Domain.Entities.Moderation;
using Whispra.Domain.Enums;

namespace Whispra.Application.Interfaces.Repositories;

public interface IModerationActionRepository
{
    Task<ModerationAction?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<ModerationAction> CreateAsync(ModerationAction action, CancellationToken cancellationToken = default);
    Task UpdateAsync(ModerationAction action, CancellationToken cancellationToken = default);
    Task<List<ModerationAction>> GetUserActionsAsync(string userId, bool onlyActive, CancellationToken cancellationToken = default);
    Task<ModerationAction?> GetActiveActionAsync(string userId, ModerationActionType actionType, CancellationToken cancellationToken = default);
    Task ExpireOldActionsAsync(CancellationToken cancellationToken = default);
}
```

`Whispra.Application/Interfaces/Services/IModerationService.cs`:

```csharp
using Whispra.Domain.Enums;

namespace Whispra.Application.Interfaces.Services;

public interface IModerationService
{
    Task<bool> IsUserMutedAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> IsUserSuspendedAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> IsUserBannedAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> IsContentAllowedAsync(string content, CancellationToken cancellationToken = default);
    Task<bool> CheckRateLimitAsync(string userId, string action, int maxActions, TimeSpan window, CancellationToken cancellationToken = default);
}
```

**Technologies**: Moderation DTOs, repository interfaces
**Study needed**: ✅ None, follows established patterns

## Step 9.4: Create Moderation Use Cases

`Whispra.Application/UseCases/Moderation/CreateReport/CreateReportUseCase.cs`:

```csharp
using Whispra.Application.DTOs.Moderation;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Domain.Entities.Moderation;
using Whispra.Domain.Enums;

namespace Whispra.Application.UseCases.Moderation.CreateReport;

public class CreateReportUseCase
{
    private readonly IReportRepository _reportRepository;
    private readonly IPostRepository _postRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly IUserRepository _userRepository;

    public CreateReportUseCase(
        IReportRepository reportRepository,
        IPostRepository postRepository,
        ICommentRepository commentRepository,
        IUserRepository userRepository)
    {
        _reportRepository = reportRepository;
        _postRepository = postRepository;
        _commentRepository = commentRepository;
        _userRepository = userRepository;
    }

    public async Task<ReportResponseDto> ExecuteAsync(
        CreateReportDto dto,
        string currentUserId,
        CancellationToken cancellationToken = default)
    {
        // Check if user already reported this target
        var alreadyReported = await _reportRepository.HasUserReportedAsync(
            currentUserId, dto.TargetId, cancellationToken);

        if (alreadyReported)
        {
            throw new InvalidOperationException("You have already reported this content");
        }

        // Validate target exists and get owner
        string? targetOwnerId = null;
        string? communityId = null;

        switch (dto.TargetType)
        {
            case ReportTargetType.Post:
                var post = await _postRepository.GetByIdAsync(dto.TargetId, cancellationToken);
                if (post == null)
                    throw new InvalidOperationException("Post not found");
                targetOwnerId = post.AuthorId;
                communityId = post.CommunityId;
                break;

            case ReportTargetType.Comment:
                var comment = await _commentRepository.GetByIdAsync(dto.TargetId, cancellationToken);
                if (comment == null)
                    throw new InvalidOperationException("Comment not found");
                targetOwnerId = comment.AuthorId;
                break;

            case ReportTargetType.User:
                var user = await _userRepository.GetByIdAsync(dto.TargetId, cancellationToken);
                if (user == null)
                    throw new InvalidOperationException("User not found");
                targetOwnerId = dto.TargetId;
                break;

            // Add more cases as needed
        }

        // Can't report yourself
        if (targetOwnerId == currentUserId)
        {
            throw new InvalidOperationException("You cannot report your own content");
        }

        var report = new Report
        {
            ReporterId = currentUserId,
            TargetType = dto.TargetType,
            TargetId = dto.TargetId,
            TargetOwnerId = targetOwnerId,
            Reason = dto.Reason,
            Description = dto.Description,
            Status = ReportStatus.Pending,
            CommunityId = communityId
        };

        var created = await _reportRepository.CreateAsync(report, cancellationToken);

        return new ReportResponseDto(
            created.Id,
            created.ReporterId,
            created.TargetType,
            created.TargetId,
            created.TargetOwnerId,
            created.Reason,
            created.Description,
            created.Status,
            created.ReviewedBy,
            created.ReviewedAt,
            created.ReviewNotes,
            created.ActionTaken,
            created.CreatedAt
        );
    }
}
```

`Whispra.Application/UseCases/Moderation/ReviewReport/ReviewReportUseCase.cs`:

```csharp
using Whispra.Application.DTOs.Moderation;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Domain.Entities.Moderation;
using Whispra.Domain.Enums;

namespace Whispra.Application.UseCases.Moderation.ReviewReport;

public class ReviewReportUseCase
{
    private readonly IReportRepository _reportRepository;
    private readonly IModerationActionRepository _moderationActionRepository;
    private readonly IPostRepository _postRepository;
    private readonly ICommentRepository _commentRepository;

    public ReviewReportUseCase(
        IReportRepository reportRepository,
        IModerationActionRepository moderationActionRepository,
        IPostRepository postRepository,
        ICommentRepository commentRepository)
    {
        _reportRepository = reportRepository;
        _moderationActionRepository = moderationActionRepository;
        _postRepository = postRepository;
        _commentRepository = commentRepository;
    }

    public async Task<ReportResponseDto> ExecuteAsync(
        string reportId,
        ReviewReportDto dto,
        string currentUserId,
        CancellationToken cancellationToken = default)
    {
        var report = await _reportRepository.GetByIdAsync(reportId, cancellationToken);
        if (report == null)
        {
            throw new InvalidOperationException("Report not found");
        }

        // Update report
        report.Status = dto.Status;
        report.ReviewedBy = currentUserId;
        report.ReviewedAt = DateTime.UtcNow;
        report.ReviewNotes = dto.ReviewNotes;
        report.ActionTaken = dto.ActionTaken;

        await _reportRepository.UpdateAsync(report, cancellationToken);

        // Take moderation action if needed
        if (dto.ActionTaken != ModerationActionType.NoAction && report.TargetOwnerId != null)
        {
            await ExecuteModerationActionAsync(
                report,
                dto.ActionTaken,
                currentUserId,
                dto.ActionDurationHours,
                cancellationToken);
        }

        return new ReportResponseDto(
            report.Id,
            report.ReporterId,
            report.TargetType,
            report.TargetId,
            report.TargetOwnerId,
            report.Reason,
            report.Description,
            report.Status,
            report.ReviewedBy,
            report.ReviewedAt,
            report.ReviewNotes,
            report.ActionTaken,
            report.CreatedAt
        );
    }

    private async Task ExecuteModerationActionAsync(
        Report report,
        ModerationActionType actionType,
        string moderatorId,
        int? durationHours,
        CancellationToken cancellationToken)
    {
        // Handle content-specific actions
        if (actionType == ModerationActionType.ContentHidden)
        {
            if (report.TargetType == ReportTargetType.Post)
            {
                var post = await _postRepository.GetByIdAsync(report.TargetId, cancellationToken);
                if (post != null)
                {
                    post.IsDeleted = true;
                    post.DeletedAt = DateTime.UtcNow;
                    await _postRepository.UpdateAsync(post, cancellationToken);
                }
            }
            else if (report.TargetType == ReportTargetType.Comment)
            {
                var comment = await _commentRepository.GetByIdAsync(report.TargetId, cancellationToken);
                if (comment != null)
                {
                    comment.IsDeleted = true;
                    comment.DeletedAt = DateTime.UtcNow;
                    await _commentRepository.UpdateAsync(comment, cancellationToken);
                }
            }
        }

        // Handle user-specific actions
        if (report.TargetOwnerId != null)
        {
            var action = new ModerationAction
            {
                ModeratorId = moderatorId,
                TargetUserId = report.TargetOwnerId,
                ActionType = actionType,
                Reason = $"Report: {report.Reason}",
                Notes = report.ReviewNotes,
                ReportId = report.Id,
                CommunityId = report.CommunityId,
                IsActive = true
            };

            // Set expiration for temporary actions
            if (durationHours.HasValue &&
                (actionType == ModerationActionType.UserMuted ||
                 actionType == ModerationActionType.UserSuspended))
            {
                action.ExpiresAt = DateTime.UtcNow.AddHours(durationHours.Value);
            }

            await _moderationActionRepository.CreateAsync(action, cancellationToken);
        }
    }
}
```

`Whispra.Application/UseCases/Moderation/BlockUser/BlockUserUseCase.cs`:

```csharp
using Whispra.Application.DTOs.Moderation;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Domain.Entities.Moderation;

namespace Whispra.Application.UseCases.Moderation.BlockUser;

public class BlockUserUseCase
{
    private readonly IUserBlockRepository _blockRepository;
    private readonly IUserRepository _userRepository;

    public BlockUserUseCase(
        IUserBlockRepository blockRepository,
        IUserRepository userRepository)
    {
        _blockRepository = blockRepository;
        _userRepository = userRepository;
    }

    public async Task ExecuteAsync(
        BlockUserDto dto,
        string currentUserId,
        CancellationToken cancellationToken = default)
    {
        // Can't block yourself
        if (dto.UserId == currentUserId)
        {
            throw new InvalidOperationException("You cannot block yourself");
        }

        // Verify target user exists
        var targetUser = await _userRepository.GetByIdAsync(dto.UserId, cancellationToken);
        if (targetUser == null)
        {
            throw new InvalidOperationException("User not found");
        }

        // Check if already blocked
        var existing = await _blockRepository.GetBlockAsync(currentUserId, dto.UserId, cancellationToken);
        if (existing != null)
        {
            throw new InvalidOperationException("User is already blocked");
        }

        var block = new UserBlock
        {
            BlockerId = currentUserId,
            BlockedUserId = dto.UserId
        };

        await _blockRepository.CreateAsync(block, cancellationToken);
    }

    public async Task UnblockUserAsync(
        string blockedUserId,
        string currentUserId,
        CancellationToken cancellationToken = default)
    {
        var block = await _blockRepository.GetBlockAsync(currentUserId, blockedUserId, cancellationToken);
        if (block == null)
        {
            throw new InvalidOperationException("User is not blocked");
        }

        await _blockRepository.DeleteAsync(block.Id, cancellationToken);
    }

    public async Task<List<string>> GetBlockedUsersAsync(
        string currentUserId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var skip = (page - 1) * pageSize;
        var blocks = await _blockRepository.GetUserBlocksAsync(currentUserId, skip, pageSize, cancellationToken);
        return blocks.Select(b => b.BlockedUserId).ToList();
    }
}
```

`Whispra.Application/UseCases/Moderation/GetReports/GetReportsUseCase.cs`:

```csharp
using Whispra.Application.DTOs.Moderation;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Domain.Enums;

namespace Whispra.Application.UseCases.Moderation.GetReports;

public class GetReportsUseCase
{
    private readonly IReportRepository _reportRepository;
    private readonly ICommunityMemberRepository _communityMemberRepository;

    public GetReportsUseCase(
        IReportRepository reportRepository,
        ICommunityMemberRepository communityMemberRepository)
    {
        _reportRepository = reportRepository;
        _communityMemberRepository = communityMemberRepository;
    }

    public async Task<(List<ReportResponseDto> Reports, int PendingCount)> ExecuteAsync(
        ReportStatus? status,
        string? communityId,
        string currentUserId,
        bool isAdmin,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var skip = (page - 1) * pageSize;
        List<Domain.Entities.Moderation.Report> reports;

        if (communityId != null)
        {
            // Community-specific reports (must be moderator/owner)
            var membership = await _communityMemberRepository.GetMembershipAsync(
                communityId, currentUserId, cancellationToken);

            if (membership == null ||
                (membership.Role != CommunityRole.Owner && membership.Role != CommunityRole.Moderator))
            {
                throw new UnauthorizedAccessException("Only community moderators can view reports");
            }

            reports = await _reportRepository.GetCommunityReportsAsync(
                communityId, status, skip, pageSize, cancellationToken);
        }
        else if (isAdmin)
        {
            // Platform-wide reports (admin only)
            if (status.HasValue)
            {
                reports = await _reportRepository.GetReportsByStatusAsync(
                    status.Value, skip, pageSize, cancellationToken);
            }
            else
            {
                // Get all pending by default
                reports = await _reportRepository.GetReportsByStatusAsync(
                    ReportStatus.Pending, skip, pageSize, cancellationToken);
            }
        }
        else
        {
            throw new UnauthorizedAccessException("You don't have permission to view reports");
        }

        var pendingCount = await _reportRepository.GetPendingCountAsync(cancellationToken);

        var response = reports.Select(r => new ReportResponseDto(
            r.Id,
            r.ReporterId,
            r.TargetType,
            r.TargetId,
            r.TargetOwnerId,
            r.Reason,
            r.Description,
            r.Status,
            r.ReviewedBy,
            r.ReviewedAt,
            r.ReviewNotes,
            r.ActionTaken,
            r.CreatedAt
        )).ToList();

        return (response, pendingCount);
    }
}
```

**Technologies**: Moderation logic, authorization checks
**Study needed**: ✅ None, builds on established patterns

## Step 9.5: Implement Infrastructure Layer for Moderation

```bash
cd backend/Whispra.Infrastructure

# Create services folder
mkdir -p Services/Moderation
```

`Whispra.Infrastructure/Persistence/Repositories/ReportRepository.cs`:

```csharp
using MongoDB.Driver;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Domain.Entities.Moderation;
using Whispra.Domain.Enums;
using Whispra.Infrastructure.Persistence.MongoDB;

namespace Whispra.Infrastructure.Persistence.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly MongoDbContext _context;

    public ReportRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<Report?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.Reports
            .Find(r => r.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Report> CreateAsync(Report report, CancellationToken cancellationToken = default)
    {
        await _context.Reports.InsertOneAsync(report, cancellationToken: cancellationToken);
        return report;
    }

    public async Task UpdateAsync(Report report, CancellationToken cancellationToken = default)
    {
        report.UpdatedAt = DateTime.UtcNow;
        await _context.Reports.ReplaceOneAsync(
            r => r.Id == report.Id,
            report,
            cancellationToken: cancellationToken);
    }

    public async Task<List<Report>> GetReportsByStatusAsync(
        ReportStatus status, int skip, int limit, CancellationToken cancellationToken = default)
    {
        return await _context.Reports
            .Find(r => r.Status == status)
            .SortByDescending(r => r.CreatedAt)
            .Skip(skip)
            .Limit(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Report>> GetReportsByTargetAsync(
        ReportTargetType targetType, string targetId, CancellationToken cancellationToken = default)
    {
        return await _context.Reports
            .Find(r => r.TargetType == targetType && r.TargetId == targetId)
            .SortByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Report>> GetCommunityReportsAsync(
        string communityId, ReportStatus? status, int skip, int limit, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Report>.Filter.Eq(r => r.CommunityId, communityId);

        if (status.HasValue)
        {
            filter = Builders<Report>.Filter.And(
                filter,
                Builders<Report>.Filter.Eq(r => r.Status, status.Value)
            );
        }

        return await _context.Reports
            .Find(filter)
            .SortByDescending(r => r.CreatedAt)
            .Skip(skip)
            .Limit(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasUserReportedAsync(
        string reporterId, string targetId, CancellationToken cancellationToken = default)
    {
        var count = await _context.Reports
            .CountDocumentsAsync(
                r => r.ReporterId == reporterId && r.TargetId == targetId,
                cancellationToken: cancellationToken);

        return count > 0;
    }

    public async Task<int> GetPendingCountAsync(CancellationToken cancellationToken = default)
    {
        var count = await _context.Reports
            .CountDocumentsAsync(
                r => r.Status == ReportStatus.Pending,
                cancellationToken: cancellationToken);

        return (int)count;
    }
}
```

`Whispra.Infrastructure/Persistence/Repositories/UserBlockRepository.cs`:

```csharp
using MongoDB.Driver;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Domain.Entities.Moderation;
using Whispra.Infrastructure.Persistence.MongoDB;

namespace Whispra.Infrastructure.Persistence.Repositories;

public class UserBlockRepository : IUserBlockRepository
{
    private readonly MongoDbContext _context;

    public UserBlockRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<UserBlock?> GetBlockAsync(
        string blockerId, string blockedUserId, CancellationToken cancellationToken = default)
    {
        return await _context.UserBlocks
            .Find(b => b.BlockerId == blockerId && b.BlockedUserId == blockedUserId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<UserBlock> CreateAsync(UserBlock block, CancellationToken cancellationToken = default)
    {
        await _context.UserBlocks.InsertOneAsync(block, cancellationToken: cancellationToken);
        return block;
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        await _context.UserBlocks.DeleteOneAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<List<UserBlock>> GetUserBlocksAsync(
        string userId, int skip, int limit, CancellationToken cancellationToken = default)
    {
        return await _context.UserBlocks
            .Find(b => b.BlockerId == userId)
            .SortByDescending(b => b.BlockedAt)
            .Skip(skip)
            .Limit(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsBlockedAsync(
        string blockerId, string blockedUserId, CancellationToken cancellationToken = default)
    {
        var count = await _context.UserBlocks
            .CountDocumentsAsync(
                b => b.BlockerId == blockerId && b.BlockedUserId == blockedUserId,
                cancellationToken: cancellationToken);

        return count > 0;
    }

    public async Task<List<string>> GetBlockedUserIdsAsync(
        string userId, CancellationToken cancellationToken = default)
    {
        var blocks = await _context.UserBlocks
            .Find(b => b.BlockerId == userId)
            .ToListAsync(cancellationToken);

        return blocks.Select(b => b.BlockedUserId).ToList();
    }
}
```

`Whispra.Infrastructure/Persistence/Repositories/ModerationActionRepository.cs`:

```csharp
using MongoDB.Driver;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Domain.Entities.Moderation;
using Whispra.Domain.Enums;
using Whispra.Infrastructure.Persistence.MongoDB;

namespace Whispra.Infrastructure.Persistence.Repositories;

public class ModerationActionRepository : IModerationActionRepository
{
    private readonly MongoDbContext _context;

    public ModerationActionRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<ModerationAction?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.ModerationActions
            .Find(a => a.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ModerationAction> CreateAsync(
        ModerationAction action, CancellationToken cancellationToken = default)
    {
        await _context.ModerationActions.InsertOneAsync(action, cancellationToken: cancellationToken);
        return action;
    }

    public async Task UpdateAsync(ModerationAction action, CancellationToken cancellationToken = default)
    {
        action.UpdatedAt = DateTime.UtcNow;
        await _context.ModerationActions.ReplaceOneAsync(
            a => a.Id == action.Id,
            action,
            cancellationToken: cancellationToken);
    }

    public async Task<List<ModerationAction>> GetUserActionsAsync(
        string userId, bool onlyActive, CancellationToken cancellationToken = default)
    {
        var filter = Builders<ModerationAction>.Filter.Eq(a => a.TargetUserId, userId);

        if (onlyActive)
        {
            filter = Builders<ModerationAction>.Filter.And(
                filter,
                Builders<ModerationAction>.Filter.Eq(a => a.IsActive, true)
            );
        }

        return await _context.ModerationActions
            .Find(filter)
            .SortByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<ModerationAction?> GetActiveActionAsync(
        string userId, ModerationActionType actionType, CancellationToken cancellationToken = default)
    {
        return await _context.ModerationActions
            .Find(a => a.TargetUserId == userId &&
                       a.ActionType == actionType &&
                       a.IsActive)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task ExpireOldActionsAsync(CancellationToken cancellationToken = default)
    {
        var filter = Builders<ModerationAction>.Filter.And(
            Builders<ModerationAction>.Filter.Eq(a => a.IsActive, true),
            Builders<ModerationAction>.Filter.Lt(a => a.ExpiresAt, DateTime.UtcNow),
            Builders<ModerationAction>.Filter.Ne(a => a.ExpiresAt, null)
        );

        var update = Builders<ModerationAction>.Update
            .Set(a => a.IsActive, false)
            .Set(a => a.UpdatedAt, DateTime.UtcNow);

        await _context.ModerationActions.UpdateManyAsync(filter, update, cancellationToken: cancellationToken);
    }
}
```

`Whispra.Infrastructure/Services/Moderation/ModerationService.cs`:

```csharp
using Whispra.Application.Interfaces.Repositories;
using Whispra.Application.Interfaces.Services;
using Whispra.Domain.Enums;

namespace Whispra.Infrastructure.Services.Moderation;

public class ModerationService : IModerationService
{
    private readonly IModerationActionRepository _moderationActionRepository;
    private static readonly string[] _profanityWords = { "badword1", "badword2" }; // Add actual profanity list

    public ModerationService(IModerationActionRepository moderationActionRepository)
    {
        _moderationActionRepository = moderationActionRepository;
    }

    public async Task<bool> IsUserMutedAsync(string userId, CancellationToken cancellationToken = default)
    {
        await _moderationActionRepository.ExpireOldActionsAsync(cancellationToken);

        var action = await _moderationActionRepository.GetActiveActionAsync(
            userId, ModerationActionType.UserMuted, cancellationToken);

        return action != null;
    }

    public async Task<bool> IsUserSuspendedAsync(string userId, CancellationToken cancellationToken = default)
    {
        await _moderationActionRepository.ExpireOldActionsAsync(cancellationToken);

        var action = await _moderationActionRepository.GetActiveActionAsync(
            userId, ModerationActionType.UserSuspended, cancellationToken);

        return action != null;
    }

    public async Task<bool> IsUserBannedAsync(string userId, CancellationToken cancellationToken = default)
    {
        var action = await _moderationActionRepository.GetActiveActionAsync(
            userId, ModerationActionType.UserBanned, cancellationToken);

        return action != null;
    }

    public Task<bool> IsContentAllowedAsync(string content, CancellationToken cancellationToken = default)
    {
        // Basic profanity filter
        var lowerContent = content.ToLower();
        var hasProfanity = _profanityWords.Any(word => lowerContent.Contains(word));

        if (hasProfanity)
            return Task.FromResult(false);

        // Add more checks (spam patterns, links, etc.)
        // For production, use a proper content moderation API (Azure Content Moderator, Perspective API, etc.)

        return Task.FromResult(true);
    }

    public Task<bool> CheckRateLimitAsync(
        string userId,
        string action,
        int maxActions,
        TimeSpan window,
        CancellationToken cancellationToken = default)
    {
        // This is a simplified implementation
        // In production, use Redis with sliding window or token bucket algorithm

        // For now, just return true (no rate limiting)
        // TODO: Implement proper rate limiting with Redis

        return Task.FromResult(true);
    }
}
```

Update `Whispra.Infrastructure/Persistence/MongoDB/MongoDbContext.cs`:

```csharp
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Whispra.Domain.Entities.Auth;
using Whispra.Domain.Entities.Communities;
using Whispra.Domain.Entities.Media;
using Whispra.Domain.Entities.Messages;
using Whispra.Domain.Entities.Moderation;
using Whispra.Domain.Entities.Notifications;
using Whispra.Domain.Entities.Posts;
using Whispra.Domain.Entities.Users;
using Whispra.Infrastructure.Configuration;

namespace Whispra.Infrastructure.Persistence.MongoDB;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<User> Users => _database.GetCollection<User>("users");
    public IMongoCollection<RefreshToken> RefreshTokens => _database.GetCollection<RefreshToken>("refresh_tokens");
    public IMongoCollection<Community> Communities => _database.GetCollection<Community>("communities");
    public IMongoCollection<CommunityMember> CommunityMembers => _database.GetCollection<CommunityMember>("community_members");
    public IMongoCollection<CommunityInvite> CommunityInvites => _database.GetCollection<CommunityInvite>("community_invites");
    public IMongoCollection<Post> Posts => _database.GetCollection<Post>("posts");
    public IMongoCollection<Comment> Comments => _database.GetCollection<Comment>("comments");
    public IMongoCollection<Reaction> Reactions => _database.GetCollection<Reaction>("reactions");
    public IMongoCollection<Media> Media => _database.GetCollection<Media>("media");
    public IMongoCollection<Conversation> Conversations => _database.GetCollection<Conversation>("conversations");
    public IMongoCollection<Message> Messages => _database.GetCollection<Message>("messages");
    public IMongoCollection<UserPresence> UserPresences => _database.GetCollection<UserPresence>("user_presences");
    public IMongoCollection<Notification> Notifications => _database.GetCollection<Notification>("notifications");
    public IMongoCollection<NotificationPreference> NotificationPreferences => _database.GetCollection<NotificationPreference>("notification_preferences");
    public IMongoCollection<Report> Reports => _database.GetCollection<Report>("reports");
    public IMongoCollection<UserBlock> UserBlocks => _database.GetCollection<UserBlock>("user_blocks");
    public IMongoCollection<ModerationAction> ModerationActions => _database.GetCollection<ModerationAction>("moderation_actions");
}
```

**Technologies**: MongoDB queries for moderation, basic content filtering
**Study needed**:

- 📚 Content moderation APIs (Optional, 1-2 hours)
  - Resource: https://azure.microsoft.com/en-us/products/ai-services/ai-content-safety
  - Resource: https://perspectiveapi.com/

## Step 9.6: Create Moderation Controllers

```bash
cd backend/Whispra.Api/Controllers
```

`Whispra.Api/Controllers/ModerationController.cs`:

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Whispra.Application.DTOs.Moderation;
using Whispra.Application.UseCases.Moderation.BlockUser;
using Whispra.Application.UseCases.Moderation.CreateReport;
using Whispra.Application.UseCases.Moderation.GetReports;
using Whispra.Application.UseCases.Moderation.ReviewReport;
using Whispra.Domain.Enums;

namespace Whispra.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ModerationController : ControllerBase
{
    private readonly CreateReportUseCase _createReportUseCase;
    private readonly ReviewReportUseCase _reviewReportUseCase;
    private readonly GetReportsUseCase _getReportsUseCase;
    private readonly BlockUserUseCase _blockUserUseCase;

    public ModerationController(
        CreateReportUseCase createReportUseCase,
        ReviewReportUseCase reviewReportUseCase,
        GetReportsUseCase getReportsUseCase,
        BlockUserUseCase blockUserUseCase)
    {
        _createReportUseCase = createReportUseCase;
        _reviewReportUseCase = reviewReportUseCase;
        _getReportsUseCase = getReportsUseCase;
        _blockUserUseCase = blockUserUseCase;
    }

    private string GetCurrentUserId()
    {
        return User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("User not authenticated");
    }

    private bool IsAdmin()
    {
        // TODO: Implement proper admin role check
        // For now, check if user has admin claim
        return User.HasClaim("role", "admin");
    }

    // ========== REPORTS ==========

    [HttpPost("reports")]
    public async Task<ActionResult<ReportResponseDto>> CreateReport(
        [FromBody] CreateReportDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var report = await _createReportUseCase.ExecuteAsync(dto, GetCurrentUserId(), cancellationToken);
            return Created($"/api/Moderation/reports/{report.Id}", report);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("reports")]
    public async Task<ActionResult<object>> GetReports(
        [FromQuery] ReportStatus? status = null,
        [FromQuery] string? communityId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var (reports, pendingCount) = await _getReportsUseCase.ExecuteAsync(
                status, communityId, GetCurrentUserId(), IsAdmin(), page, pageSize, cancellationToken);

            return Ok(new
            {
                reports,
                pendingCount,
                page,
                pageSize,
                hasMore = reports.Count == pageSize
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [HttpPut("reports/{reportId}/review")]
    public async Task<ActionResult<ReportResponseDto>> ReviewReport(
        string reportId,
        [FromBody] ReviewReportDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            // TODO: Check if user is moderator/admin
            var report = await _reviewReportUseCase.ExecuteAsync(
                reportId, dto, GetCurrentUserId(), cancellationToken);
            return Ok(report);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    // ========== BLOCKING ==========

    [HttpPost("blocks")]
    public async Task<ActionResult> BlockUser(
        [FromBody] BlockUserDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            await _blockUserUseCase.ExecuteAsync(dto, GetCurrentUserId(), cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("blocks/{userId}")]
    public async Task<ActionResult> UnblockUser(
        string userId,
        CancellationToken cancellationToken)
    {
        try
        {
            await _blockUserUseCase.UnblockUserAsync(userId, GetCurrentUserId(), cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpGet("blocks")]
    public async Task<ActionResult<List<string>>> GetBlockedUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var blockedUserIds = await _blockUserUseCase.GetBlockedUsersAsync(
            GetCurrentUserId(), page, pageSize, cancellationToken);
        return Ok(blockedUserIds);
    }
}
```

**Technologies**: Moderation REST API, authorization
**Study needed**: ✅ None, follows established patterns

## Step 9.7: Add Moderation Middleware (Optional Enhancement)

Create middleware to check user status before allowing actions.

```bash
cd backend/Whispra.Api
mkdir -p Middleware
```

`Whispra.Api/Middleware/ModerationMiddleware.cs`:

```csharp
using System.Security.Claims;
using Whispra.Application.Interfaces.Services;

namespace Whispra.Api.Middleware;

public class ModerationMiddleware
{
    private readonly RequestDelegate _next;

    public ModerationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IModerationService moderationService)
    {
        // Skip for non-authenticated requests
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            await _next(context);
            return;
        }

        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            await _next(context);
            return;
        }

        // Check if user is banned
        if (await moderationService.IsUserBannedAsync(userId))
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Your account has been permanently banned"
            });
            return;
        }

        // Check if user is suspended
        if (await moderationService.IsUserSuspendedAsync(userId))
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Your account is temporarily suspended"
            });
            return;
        }

        // Check if user is muted (only for write operations)
        if (context.Request.Method != "GET" && context.Request.Method != "HEAD")
        {
            if (await moderationService.IsUserMutedAsync(userId))
            {
                // Allow reading but not posting
                if (context.Request.Path.StartsWithSegments("/api/Posts") ||
                    context.Request.Path.StartsWithSegments("/api/Comments") ||
                    context.Request.Path.StartsWithSegments("/api/Messages"))
                {
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        error = "You are temporarily muted and cannot post content"
                    });
                    return;
                }
            }
        }

        await _next(context);
    }
}
```

**Technologies**: ASP.NET Core middleware, request interception
**Study needed**:

- 📚 ASP.NET Core Middleware (1-2 hours)
  - Resource: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/

## Step 9.8: Update Service Registration and Program.cs

Update `Whispra.Api/Extensions/ServiceCollectionExtensions.cs`:

```csharp
// In AddApplicationServices method, add:
services.AddScoped<CreateReportUseCase>();
services.AddScoped<ReviewReportUseCase>();
services.AddScoped<GetReportsUseCase>();
services.AddScoped<BlockUserUseCase>();

// In AddInfrastructureServices method, add:
services.AddScoped<IReportRepository, ReportRepository>();
services.AddScoped<IUserBlockRepository, UserBlockRepository>();
services.AddScoped<IModerationActionRepository, ModerationActionRepository>();
services.AddSingleton<IModerationService, ModerationService>();
```

Update `Whispra.Api/Program.cs` to add moderation middleware:

```csharp
using Whispra.Api.Extensions;
using Whispra.Api.Hubs;
using Whispra.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add SignalR
builder.Services.AddSignalR();

// Add application and infrastructure services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
              .WithOrigins("http://localhost:3000", "http://localhost:19006")
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Authentication & Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Moderation middleware (after auth)
app.UseMiddleware<ModerationMiddleware>();

app.MapControllers();

// Map SignalR hubs
app.MapHub<ChatHub>("/hubs/chat");
app.MapHub<NotificationHub>("/hubs/notifications");

app.Run();
```

**Technologies**: Service registration, middleware pipeline
**Study needed**: ✅ None

## Step 9.9: Test Moderation & Safety Features

```bash
# Make sure all services are running
docker compose up -d

# Run the API
cd backend/Whispra.Api
dotnet run
```

Open Swagger: `https://localhost:7001/swagger`

### Test Flow:

**1. Report a post**:

```json
POST /api/Moderation/reports
{
  "targetType": 0,
  "targetId": "post-id-here",
  "reason": 0,
  "description": "This post contains spam"
}
```

**2. Get pending reports** (as moderator/admin):

```
GET /api/Moderation/reports?status=0&page=1&pageSize=20
```

**3. Review a report** (as moderator/admin):

```json
PUT /api/Moderation/reports/{reportId}/review
{
  "status": 2,
  "actionTaken": 2,
  "reviewNotes": "Content violates spam policy",
  "actionDurationHours": 24
}
```

**4. Block a user**:

```json
POST /api/Moderation/blocks
{
  "userId": "user-id-to-block"
}
```

**5. Get blocked users**:

```
GET /api/Moderation/blocks?page=1&pageSize=20
```

**6. Unblock a user**:

```
DELETE /api/Moderation/blocks/{userId}
```

**7. Test moderation actions**:

- Try to post content as a muted user → Should get 403 error
- Try to login as a suspended user → Should get 403 error
- Try to login as a banned user → Should get 403 error

**Check MongoDB Compass**:

- `reports` collection with all reports
- `user_blocks` collection with blocked relationships
- `moderation_actions` collection with moderation history

**Study needed**: ✅ Just testing

---

**🔄 CHECKPOINT - Moderation & Safety Complete!**

You now have:

- ✅ Content reporting system (posts, comments, users)
- ✅ User blocking (prevent interactions)
- ✅ Moderation queue for reviewing reports
- ✅ Moderation actions (warn, mute, suspend, ban)
- ✅ Basic content filtering (profanity detection)
- ✅ Middleware to enforce moderation actions
- ✅ Community moderators can review community reports
- ✅ Admins can review all platform reports

Before moving to the next phase, confirm:

1. ✅ Can users report content?
2. ✅ Can users block other users?
3. ✅ Can moderators review reports?
4. ✅ Do moderation actions take effect?
5. ✅ Are muted/suspended/banned users blocked from actions?

**Next Phase Preview**: We'll implement comprehensive Testing (unit tests, integration tests, E2E tests) and then move to Frontend development (React Native mobile app, Tauri desktop app).

---

# PHASE 10: Testing & Quality Assurance

**Goal**: Implement comprehensive testing strategy including unit tests, integration tests, and API testing to ensure code quality, catch bugs early, and enable confident refactoring.

## Step 10.1: Understanding Testing Fundamentals

**Why Testing is Critical?**

- **Catch bugs early**: Find issues before they reach production
- **Confident refactoring**: Change code without fear of breaking things
- **Documentation**: Tests serve as living documentation of how code should work
- **Faster development**: Automated tests run faster than manual testing
- **Regression prevention**: Ensure old bugs don't come back

**Testing Pyramid**:

```
        /\
       /  \  E2E Tests (Few)
      /____\
     /      \  Integration Tests (Some)
    /________\
   /          \  Unit Tests (Many)
  /__________\
```

**Test Types We'll Implement**:

1. **Unit Tests**: Test individual classes/methods in isolation (Use Cases, Services)
2. **Integration Tests**: Test how components work together (Repositories with MongoDB)
3. **API Tests**: Test HTTP endpoints end-to-end (Controllers)

📚 **Study needed**: Testing concepts and xUnit framework (3-4 hours)

- Resource: https://learn.microsoft.com/en-us/dotnet/core/testing/
- Resource: https://xunit.net/docs/getting-started/netcore/cmdline
- Resource: https://martinfowler.com/articles/practical-test-pyramid.html

## Step 10.2: Set Up Testing Projects

```bash
cd backend

# Create test projects
dotnet new xunit -n Whispra.UnitTests
dotnet new xunit -n Whispra.IntegrationTests

# Add test projects to solution
dotnet sln add Whispra.UnitTests/Whispra.UnitTests.csproj
dotnet sln add Whispra.IntegrationTests/Whispra.IntegrationTests.csproj

# Add references from test projects to main projects
cd Whispra.UnitTests
dotnet add reference ../Whispra.Domain/Whispra.Domain.csproj
dotnet add reference ../Whispra.Application/Whispra.Application.csproj
dotnet add reference ../Whispra.Infrastructure/Whispra.Infrastructure.csproj

cd ../Whispra.IntegrationTests
dotnet add reference ../Whispra.Domain/Whispra.Domain.csproj
dotnet add reference ../Whispra.Application/Whispra.Application.csproj
dotnet add reference ../Whispra.Infrastructure/Whispra.Infrastructure.csproj
dotnet add reference ../Whispra.Api/Whispra.Api.csproj
```

**Install testing NuGet packages**:

```bash
# In Whispra.UnitTests
cd ../Whispra.UnitTests
dotnet add package xunit
dotnet add package xunit.runner.visualstudio
dotnet add package Moq
dotnet add package FluentAssertions
dotnet add package Microsoft.NET.Test.Sdk

# In Whispra.IntegrationTests
cd ../Whispra.IntegrationTests
dotnet add package xunit
dotnet add package xunit.runner.visualstudio
dotnet add package FluentAssertions
dotnet add package Microsoft.AspNetCore.Mvc.Testing
dotnet add package Microsoft.NET.Test.Sdk
dotnet add package Testcontainers.MongoDb
```

**Technologies**: xUnit (test framework), Moq (mocking), FluentAssertions (readable assertions), Testcontainers (Docker for tests)

**Study needed**:

- 📚 xUnit basics (2-3 hours)
  - Resource: https://xunit.net/
- 📚 Moq for mocking (2-3 hours)
  - Resource: https://github.com/moq/moq4
- 📚 FluentAssertions syntax (1 hour)
  - Resource: https://fluentassertions.com/introduction

## Step 10.3: Write Unit Tests for Use Cases

Unit tests test individual classes in isolation by mocking dependencies.

**Example: Test RegisterUserUseCase**

`Whispra.UnitTests/Application/UseCases/Users/RegisterUserUseCaseTests.cs`:

```csharp
using FluentAssertions;
using Moq;
using Whispra.Application.DTOs.Users;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Application.Interfaces.Services;
using Whispra.Application.UseCases.Users.Register;
using Whispra.Domain.Entities.Users;
using Xunit;

namespace Whispra.UnitTests.Application.UseCases.Users;

public class RegisterUserUseCaseTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly RegisterUserUseCase _useCase;

    public RegisterUserUseCaseTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _useCase = new RegisterUserUseCase(_userRepositoryMock.Object, _passwordHasherMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidData_ShouldCreateUser()
    {
        // Arrange
        var dto = new RegisterUserDto(
            Username: "testuser",
            Email: "test@example.com",
            FullName: "Test User",
            Password: "Password123"
        );

        _userRepositoryMock.Setup(x => x.ExistsByUsernameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _userRepositoryMock.Setup(x => x.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _passwordHasherMock.Setup(x => x.HashPassword(It.IsAny<string>()))
            .Returns("hashedPassword123");

        _userRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User user, CancellationToken ct) => user);

        // Act
        var result = await _useCase.ExecuteAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Username.Should().Be("testuser");
        result.Email.Should().Be("test@example.com");
        result.FullName.Should().Be("Test User");

        _passwordHasherMock.Verify(x => x.HashPassword("Password123"), Times.Once);
        _userRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithExistingUsername_ShouldThrowException()
    {
        // Arrange
        var dto = new RegisterUserDto(
            Username: "existinguser",
            Email: "test@example.com",
            FullName: "Test User",
            Password: "Password123"
        );

        _userRepositoryMock.Setup(x => x.ExistsByUsernameAsync("existinguser", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        Func<Task> act = async () => await _useCase.ExecuteAsync(dto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Username already exists");
    }

    [Fact]
    public async Task ExecuteAsync_WithExistingEmail_ShouldThrowException()
    {
        // Arrange
        var dto = new RegisterUserDto(
            Username: "newuser",
            Email: "existing@example.com",
            FullName: "Test User",
            Password: "Password123"
        );

        _userRepositoryMock.Setup(x => x.ExistsByUsernameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _userRepositoryMock.Setup(x => x.ExistsByEmailAsync("existing@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        Func<Task> act = async () => await _useCase.ExecuteAsync(dto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Email already exists");
    }

    [Theory]
    [InlineData("")]
    [InlineData("ab")]
    [InlineData("a")]
    public async Task ExecuteAsync_WithInvalidUsername_ShouldThrowException(string username)
    {
        // Arrange
        var dto = new RegisterUserDto(
            Username: username,
            Email: "test@example.com",
            FullName: "Test User",
            Password: "Password123"
        );

        // Act
        Func<Task> act = async () => await _useCase.ExecuteAsync(dto);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }
}
```

**More Unit Test Examples**:

`Whispra.UnitTests/Application/UseCases/Auth/LoginUseCaseTests.cs`:

```csharp
using FluentAssertions;
using Moq;
using Whispra.Application.DTOs.Auth;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Application.Interfaces.Services;
using Whispra.Application.UseCases.Auth.Login;
using Whispra.Domain.Entities.Auth;
using Whispra.Domain.Entities.Users;
using Xunit;

namespace Whispra.UnitTests.Application.UseCases.Auth;

public class LoginUseCaseTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
    private readonly LoginUseCase _useCase;

    public LoginUseCaseTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _jwtTokenServiceMock = new Mock<IJwtTokenService>();
        _useCase = new LoginUseCase(
            _userRepositoryMock.Object,
            _refreshTokenRepositoryMock.Object,
            _passwordHasherMock.Object,
            _jwtTokenServiceMock.Object
        );
    }

    [Fact]
    public async Task ExecuteAsync_WithValidCredentials_ShouldReturnAuthResponse()
    {
        // Arrange
        var dto = new LoginDto("test@example.com", "Password123");
        var user = new User
        {
            Id = "user123",
            Email = "test@example.com",
            Username = "testuser",
            PasswordHash = "hashedPassword"
        };

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(dto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock.Setup(x => x.VerifyPassword(dto.Password, user.PasswordHash))
            .Returns(true);

        _jwtTokenServiceMock.Setup(x => x.GenerateAccessToken(user))
            .Returns("access.token.here");

        _jwtTokenServiceMock.Setup(x => x.GenerateRefreshToken())
            .Returns("refresh.token.here");

        _jwtTokenServiceMock.Setup(x => x.GetRefreshTokenExpiration())
            .Returns(DateTime.UtcNow.AddDays(7));

        _refreshTokenRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((RefreshToken token, CancellationToken ct) => token);

        // Act
        var result = await _useCase.ExecuteAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().Be("access.token.here");
        result.RefreshToken.Should().Be("refresh.token.here");
        result.User.Id.Should().Be("user123");
        result.User.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidEmail_ShouldThrowException()
    {
        // Arrange
        var dto = new LoginDto("nonexistent@example.com", "Password123");

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(dto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        Func<Task> act = async () => await _useCase.ExecuteAsync(dto);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid email or password");
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidPassword_ShouldThrowException()
    {
        // Arrange
        var dto = new LoginDto("test@example.com", "WrongPassword");
        var user = new User
        {
            Email = "test@example.com",
            PasswordHash = "hashedPassword"
        };

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(dto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock.Setup(x => x.VerifyPassword(dto.Password, user.PasswordHash))
            .Returns(false);

        // Act
        Func<Task> act = async () => await _useCase.ExecuteAsync(dto);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid email or password");
    }
}
```

`Whispra.UnitTests/Application/UseCases/Posts/CreatePostUseCaseTests.cs`:

```csharp
using FluentAssertions;
using Moq;
using Whispra.Application.DTOs.Posts;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Application.UseCases.Posts.Create;
using Whispra.Domain.Entities.Posts;
using Whispra.Domain.Enums;
using Xunit;

namespace Whispra.UnitTests.Application.UseCases.Posts;

public class CreatePostUseCaseTests
{
    private readonly Mock<IPostRepository> _postRepositoryMock;
    private readonly Mock<ICommunityMemberRepository> _communityMemberRepositoryMock;
    private readonly CreatePostUseCase _useCase;

    public CreatePostUseCaseTests()
    {
        _postRepositoryMock = new Mock<IPostRepository>();
        _communityMemberRepositoryMock = new Mock<ICommunityMemberRepository>();
        _useCase = new CreatePostUseCase(_postRepositoryMock.Object, _communityMemberRepositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithTextPost_ShouldCreatePost()
    {
        // Arrange
        var dto = new CreatePostDto(
            TextContent: "This is a test post",
            Type: PostType.Text,
            Visibility: PostVisibility.Public,
            CommunityId: null
        );

        _postRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<Post>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Post post, CancellationToken ct) => post);

        // Act
        var result = await _useCase.ExecuteAsync(dto, "user123");

        // Assert
        result.Should().NotBeNull();
        result.TextContent.Should().Be("This is a test post");
        result.Type.Should().Be(PostType.Text);
        result.Visibility.Should().Be(PostVisibility.Public);
        result.AuthorId.Should().Be("user123");

        _postRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Post>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithCommunityPost_ShouldVerifyMembership()
    {
        // Arrange
        var dto = new CreatePostDto(
            TextContent: "Community post",
            Type: PostType.Text,
            Visibility: PostVisibility.CommunityOnly,
            CommunityId: "community123"
        );

        _communityMemberRepositoryMock.Setup(x => x.IsMemberAsync("community123", "user123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _postRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<Post>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Post post, CancellationToken ct) => post);

        // Act
        var result = await _useCase.ExecuteAsync(dto, "user123");

        // Assert
        result.CommunityId.Should().Be("community123");
        _communityMemberRepositoryMock.Verify(x => x.IsMemberAsync("community123", "user123", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithNonMemberCommunityPost_ShouldThrowException()
    {
        // Arrange
        var dto = new CreatePostDto(
            TextContent: "Community post",
            Type: PostType.Text,
            Visibility: PostVisibility.CommunityOnly,
            CommunityId: "community123"
        );

        _communityMemberRepositoryMock.Setup(x => x.IsMemberAsync("community123", "user123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        Func<Task> act = async () => await _useCase.ExecuteAsync(dto, "user123");

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("You must be a member of this community to post");
    }
}
```

**Technologies**: xUnit [Fact] and [Theory], Moq mocking, FluentAssertions
**Study needed**: ✅ Just practice writing tests

## Step 10.4: Write Integration Tests for Repositories

Integration tests test how components work with real external dependencies (MongoDB in our case).

**Set up Testcontainers for MongoDB**:

`Whispra.IntegrationTests/Infrastructure/MongoDbFixture.cs`:

```csharp
using MongoDB.Driver;
using Testcontainers.MongoDb;
using Whispra.Infrastructure.Configuration;
using Whispra.Infrastructure.Persistence.MongoDB;
using Microsoft.Extensions.Options;
using Xunit;

namespace Whispra.IntegrationTests.Infrastructure;

public class MongoDbFixture : IAsyncLifetime
{
    private readonly MongoDbContainer _mongoContainer = new MongoDbBuilder()
        .WithImage("mongo:7.0")
        .WithUsername("testuser")
        .WithPassword("testpassword")
        .Build();

    public MongoDbContext Context { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _mongoContainer.StartAsync();

        var settings = Options.Create(new MongoDbSettings
        {
            ConnectionString = _mongoContainer.GetConnectionString(),
            DatabaseName = "whispra_test"
        });

        Context = new MongoDbContext(settings);
    }

    public async Task DisposeAsync()
    {
        await _mongoContainer.DisposeAsync();
    }

    public async Task CleanupAsync()
    {
        // Drop all collections between tests
        var client = new MongoClient(_mongoContainer.GetConnectionString());
        await client.DropDatabaseAsync("whispra_test");
    }
}
```

**Example: Test UserRepository**:

`Whispra.IntegrationTests/Infrastructure/Repositories/UserRepositoryTests.cs`:

```csharp
using FluentAssertions;
using Whispra.Domain.Entities.Users;
using Whispra.Infrastructure.Persistence.Repositories;
using Xunit;

namespace Whispra.IntegrationTests.Infrastructure.Repositories;

public class UserRepositoryTests : IClassFixture<MongoDbFixture>
{
    private readonly MongoDbFixture _fixture;
    private readonly UserRepository _repository;

    public UserRepositoryTests(MongoDbFixture fixture)
    {
        _fixture = fixture;
        _repository = new UserRepository(fixture.Context);
    }

    [Fact]
    public async Task CreateAsync_ShouldPersistUser()
    {
        // Arrange
        var user = new User
        {
            Username = "testuser",
            Email = "test@example.com",
            FullName = "Test User",
            PasswordHash = "hashedPassword"
        };

        // Act
        var result = await _repository.CreateAsync(user);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeNullOrEmpty();
        result.Username.Should().Be("testuser");
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        // Verify it was actually saved
        var retrieved = await _repository.GetByIdAsync(result.Id);
        retrieved.Should().NotBeNull();
        retrieved!.Email.Should().Be("test@example.com");

        // Cleanup
        await _fixture.CleanupAsync();
    }

    [Fact]
    public async Task GetByUsernameAsync_WithExistingUser_ShouldReturnUser()
    {
        // Arrange
        var user = new User
        {
            Username = "findme",
            Email = "findme@example.com",
            PasswordHash = "hashed"
        };
        await _repository.CreateAsync(user);

        // Act
        var result = await _repository.GetByUsernameAsync("findme");

        // Assert
        result.Should().NotBeNull();
        result!.Username.Should().Be("findme");
        result.Email.Should().Be("findme@example.com");

        // Cleanup
        await _fixture.CleanupAsync();
    }

    [Fact]
    public async Task GetByUsernameAsync_WithNonExistingUser_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByUsernameAsync("nonexistent");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ExistsByEmailAsync_WithExistingEmail_ShouldReturnTrue()
    {
        // Arrange
        var user = new User
        {
            Username = "user1",
            Email = "existing@example.com",
            PasswordHash = "hashed"
        };
        await _repository.CreateAsync(user);

        // Act
        var exists = await _repository.ExistsByEmailAsync("existing@example.com");

        // Assert
        exists.Should().BeTrue();

        // Cleanup
        await _fixture.CleanupAsync();
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyUser()
    {
        // Arrange
        var user = new User
        {
            Username = "oldname",
            Email = "old@example.com",
            FullName = "Old Name",
            PasswordHash = "hashed"
        };
        var created = await _repository.CreateAsync(user);

        // Act
        created.FullName = "New Name";
        created.Bio = "Updated bio";
        await _repository.UpdateAsync(created);

        // Assert
        var updated = await _repository.GetByIdAsync(created.Id);
        updated.Should().NotBeNull();
        updated!.FullName.Should().Be("New Name");
        updated.Bio.Should().Be("Updated bio");
        updated.UpdatedAt.Should().BeAfter(updated.CreatedAt);

        // Cleanup
        await _fixture.CleanupAsync();
    }
}
```

**More Integration Test Examples**:

`Whispra.IntegrationTests/Infrastructure/Repositories/PostRepositoryTests.cs`:

```csharp
using FluentAssertions;
using Whispra.Domain.Entities.Posts;
using Whispra.Domain.Enums;
using Whispra.Infrastructure.Persistence.Repositories;
using Xunit;

namespace Whispra.IntegrationTests.Infrastructure.Repositories;

public class PostRepositoryTests : IClassFixture<MongoDbFixture>
{
    private readonly MongoDbFixture _fixture;
    private readonly PostRepository _repository;

    public PostRepositoryTests(MongoDbFixture fixture)
    {
        _fixture = fixture;
        _repository = new PostRepository(fixture.Context);
    }

    [Fact]
    public async Task GetUserPostsAsync_ShouldReturnUserPosts()
    {
        // Arrange
        var userId = "user123";
        var post1 = new Post
        {
            AuthorId = userId,
            TextContent = "First post",
            Type = PostType.Text,
            Visibility = PostVisibility.Public
        };
        var post2 = new Post
        {
            AuthorId = userId,
            TextContent = "Second post",
            Type = PostType.Text,
            Visibility = PostVisibility.Public
        };
        var post3 = new Post
        {
            AuthorId = "otheruser",
            TextContent = "Other user post",
            Type = PostType.Text,
            Visibility = PostVisibility.Public
        };

        await _repository.CreateAsync(post1);
        await _repository.CreateAsync(post2);
        await _repository.CreateAsync(post3);

        // Act
        var result = await _repository.GetUserPostsAsync(userId, 0, 10);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(p => p.AuthorId == userId);
        result.Should().BeInDescendingOrder(p => p.CreatedAt);

        // Cleanup
        await _fixture.CleanupAsync();
    }

    [Fact]
    public async Task GetCommunityPostsAsync_WithPagination_ShouldReturnCorrectPage()
    {
        // Arrange
        var communityId = "community123";
        for (int i = 0; i < 25; i++)
        {
            var post = new Post
            {
                AuthorId = "user123",
                TextContent = $"Post {i}",
                Type = PostType.Text,
                Visibility = PostVisibility.CommunityOnly,
                CommunityId = communityId
            };
            await _repository.CreateAsync(post);
            await Task.Delay(10); // Ensure different timestamps
        }

        // Act - Get second page (skip 10, take 10)
        var result = await _repository.GetCommunityPostsAsync(communityId, 10, 10);

        // Assert
        result.Should().HaveCount(10);
        result.Should().OnlyContain(p => p.CommunityId == communityId);

        // Cleanup
        await _fixture.CleanupAsync();
    }
}
```

**Technologies**: Testcontainers (Docker-based testing), real MongoDB instance
**Study needed**:

- 📚 Testcontainers (1-2 hours)
  - Resource: https://dotnet.testcontainers.org/

## Step 10.5: Write API Integration Tests

Test the entire HTTP request/response cycle with a test server.

`Whispra.IntegrationTests/Api/CustomWebApplicationFactory.cs`:

```csharp
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Testcontainers.MongoDb;
using Whispra.Infrastructure.Configuration;
using Whispra.Infrastructure.Persistence.MongoDB;

namespace Whispra.IntegrationTests.Api;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MongoDbContainer _mongoContainer = new MongoDbBuilder()
        .WithImage("mongo:7.0")
        .WithUsername("testuser")
        .WithPassword("testpassword")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // Remove existing MongoDB configuration
            services.RemoveAll<MongoDbContext>();
            services.RemoveAll<IOptions<MongoDbSettings>>();

            // Add test MongoDB configuration
            services.Configure<MongoDbSettings>(options =>
            {
                options.ConnectionString = _mongoContainer.GetConnectionString();
                options.DatabaseName = "whispra_test";
            });

            services.AddSingleton<MongoDbContext>();
        });
    }

    public async Task InitializeAsync()
    {
        await _mongoContainer.StartAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _mongoContainer.DisposeAsync();
    }
}
```

`Whispra.IntegrationTests/Api/Controllers/UsersControllerTests.cs`:

```csharp
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Whispra.Application.DTOs.Users;
using Xunit;

namespace Whispra.IntegrationTests.Api.Controllers;

public class UsersControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public UsersControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var registerDto = new RegisterUserDto(
            Username: $"testuser_{Guid.NewGuid():N}",
            Email: $"test_{Guid.NewGuid():N}@example.com",
            FullName: "Test User",
            Password: "Password123"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/Users/register", registerDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var user = await response.Content.ReadFromJsonAsync<UserResponseDto>();
        user.Should().NotBeNull();
        user!.Username.Should().Be(registerDto.Username);
        user.Email.Should().Be(registerDto.Email);
    }

    [Fact]
    public async Task Register_WithDuplicateUsername_ShouldReturnBadRequest()
    {
        // Arrange
        var username = $"duplicate_{Guid.NewGuid():N}";
        var registerDto1 = new RegisterUserDto(
            Username: username,
            Email: $"email1_{Guid.NewGuid():N}@example.com",
            FullName: "User 1",
            Password: "Password123"
        );
        var registerDto2 = new RegisterUserDto(
            Username: username, // Same username
            Email: $"email2_{Guid.NewGuid():N}@example.com",
            FullName: "User 2",
            Password: "Password123"
        );

        // Act
        await _client.PostAsJsonAsync("/api/Users/register", registerDto1);
        var response = await _client.PostAsJsonAsync("/api/Users/register", registerDto2);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WithInvalidEmail_ShouldReturnBadRequest()
    {
        // Arrange
        var registerDto = new RegisterUserDto(
            Username: $"user_{Guid.NewGuid():N}",
            Email: "invalid-email",
            FullName: "Test User",
            Password: "Password123"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/Users/register", registerDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
```

`Whispra.IntegrationTests/Api/Controllers/AuthControllerTests.cs`:

```csharp
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Whispra.Application.DTOs.Auth;
using Whispra.Application.DTOs.Users;
using Xunit;

namespace Whispra.IntegrationTests.Api.Controllers;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnTokens()
    {
        // Arrange - Register a user first
        var email = $"login_{Guid.NewGuid():N}@example.com";
        var password = "Password123";
        var registerDto = new RegisterUserDto(
            Username: $"loginuser_{Guid.NewGuid():N}",
            Email: email,
            FullName: "Login Test",
            Password: password
        );
        await _client.PostAsJsonAsync("/api/Users/register", registerDto);

        var loginDto = new LoginDto(email, password);

        // Act
        var response = await _client.PostAsJsonAsync("/api/Auth/login", loginDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        authResponse.Should().NotBeNull();
        authResponse!.AccessToken.Should().NotBeNullOrEmpty();
        authResponse.RefreshToken.Should().NotBeNullOrEmpty();
        authResponse.User.Email.Should().Be(email);
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ShouldReturnUnauthorized()
    {
        // Arrange - Register a user
        var email = $"wrongpwd_{Guid.NewGuid():N}@example.com";
        var registerDto = new RegisterUserDto(
            Username: $"user_{Guid.NewGuid():N}",
            Email: email,
            FullName: "Test",
            Password: "CorrectPassword123"
        );
        await _client.PostAsJsonAsync("/api/Users/register", registerDto);

        var loginDto = new LoginDto(email, "WrongPassword");

        // Act
        var response = await _client.PostAsJsonAsync("/api/Auth/login", loginDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCurrentUser_WithValidToken_ShouldReturnUser()
    {
        // Arrange - Register and login
        var email = $"getme_{Guid.NewGuid():N}@example.com";
        var registerDto = new RegisterUserDto(
            Username: $"getmeuser_{Guid.NewGuid():N}",
            Email: email,
            FullName: "Get Me Test",
            Password: "Password123"
        );
        await _client.PostAsJsonAsync("/api/Users/register", registerDto);

        var loginResponse = await _client.PostAsJsonAsync("/api/Auth/login", new LoginDto(email, "Password123"));
        var authResponse = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>();

        // Set authorization header
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponse!.AccessToken);

        // Act
        var response = await _client.GetAsync("/api/Auth/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var user = await response.Content.ReadFromJsonAsync<UserResponseDto>();
        user.Should().NotBeNull();
        user!.Email.Should().Be(email);
    }

    [Fact]
    public async Task GetCurrentUser_WithoutToken_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/Auth/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
```

**Technologies**: WebApplicationFactory, in-memory test server, HTTP client testing
**Study needed**:

- 📚 ASP.NET Core integration testing (2-3 hours)
  - Resource: https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests

## Step 10.6: Run Tests and Check Coverage

```bash
# Run all tests
cd backend
dotnet test

# Run tests with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run tests with coverage (install coverlet first)
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# Generate coverage report (install ReportGenerator)
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:**/coverage.opencover.xml -targetdir:coverage-report
```

**Install coverage tools**:

```bash
# In each test project
cd Whispra.UnitTests
dotnet add package coverlet.collector

cd ../Whispra.IntegrationTests
dotnet add package coverlet.collector
```

**Check test results**:

- ✅ All tests should pass
- ✅ Aim for >80% code coverage on Use Cases
- ✅ Aim for >70% overall coverage

**Technologies**: Coverlet (coverage tool), ReportGenerator (HTML reports)
**Study needed**:

- 📚 Code coverage concepts (1 hour)
  - Resource: https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-code-coverage

## Step 10.7: Testing Best Practices

**Follow AAA Pattern**:

```csharp
[Fact]
public async Task TestName_Condition_ExpectedBehavior()
{
    // Arrange - Set up test data and mocks
    var input = new SomeDto(...);
    _mockRepo.Setup(x => x.Method()).Returns(value);

    // Act - Execute the code under test
    var result = await _useCase.ExecuteAsync(input);

    // Assert - Verify the results
    result.Should().NotBeNull();
    result.Property.Should().Be(expectedValue);
}
```

**Test Naming Convention**:

- **MethodName_StateUnderTest_ExpectedBehavior**
- Examples:
  - `ExecuteAsync_WithValidData_ShouldCreateUser`
  - `ExecuteAsync_WithExistingUsername_ShouldThrowException`
  - `GetByIdAsync_WithNonExistentId_ShouldReturnNull`

**What to Test**:

✅ **DO Test**:

- Happy path (valid inputs, successful execution)
- Error cases (invalid inputs, exceptions)
- Edge cases (null, empty, boundary values)
- Business logic in Use Cases
- Repository data access operations
- API endpoints (status codes, response bodies)

❌ **DON'T Test**:

- DTOs (just data holders)
- Entity properties (auto-properties)
- Third-party libraries (trust they work)
- Framework code (ASP.NET Core, MongoDB driver)

**Mock vs Real Dependencies**:

- **Unit Tests**: Mock ALL dependencies (fast, isolated)
- **Integration Tests**: Use real external services (MongoDB, S3, etc.)
- **API Tests**: Use real HTTP client + test database

**Test Data Management**:

```csharp
// Use test builders for complex objects
public class UserBuilder
{
    private string _username = "testuser";
    private string _email = "test@example.com";

    public UserBuilder WithUsername(string username)
    {
        _username = username;
        return this;
    }

    public UserBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public User Build()
    {
        return new User
        {
            Username = _username,
            Email = _email,
            PasswordHash = "hashedPassword"
        };
    }
}

// Usage in tests
var user = new UserBuilder()
    .WithUsername("customuser")
    .WithEmail("custom@example.com")
    .Build();
```

**Technologies**: Test patterns, builder pattern, test data factories
**Study needed**: ✅ Practice and experience

## Step 10.8: Continuous Integration (CI) Setup

Create a GitHub Actions workflow for automated testing.

`.github/workflows/backend-tests.yml`:

```yaml
name: Backend Tests

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main, develop]

jobs:
  test:
    runs-on: ubuntu-latest

    steps:
      - uses: actions/checkout@v3

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: "8.0.x"

      - name: Restore dependencies
        run: dotnet restore
        working-directory: ./backend

      - name: Build
        run: dotnet build --no-restore --configuration Release
        working-directory: ./backend

      - name: Run Unit Tests
        run: dotnet test Whispra.UnitTests/Whispra.UnitTests.csproj --no-build --configuration Release --logger "trx;LogFileName=unit-test-results.trx"
        working-directory: ./backend

      - name: Run Integration Tests
        run: dotnet test Whispra.IntegrationTests/Whispra.IntegrationTests.csproj --no-build --configuration Release --logger "trx;LogFileName=integration-test-results.trx"
        working-directory: ./backend

      - name: Generate Coverage Report
        run: dotnet test --no-build --configuration Release /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
        working-directory: ./backend

      - name: Upload Coverage to Codecov
        uses: codecov/codecov-action@v3
        with:
          files: ./backend/**/coverage.opencover.xml
          flags: backend
          name: backend-coverage

      - name: Publish Test Results
        uses: EnricoMi/publish-unit-test-result-action@v2
        if: always()
        with:
          files: |
            backend/**/*.trx
```

**Set up branch protection**:

1. Go to GitHub repository → Settings → Branches
2. Add branch protection rule for `main`
3. Require status checks to pass: ✅ Backend Tests
4. Require pull request reviews before merging
5. No force pushes

**Technologies**: GitHub Actions, CI/CD, automated testing
**Study needed**:

- 📚 GitHub Actions basics (2-3 hours)
  - Resource: https://docs.github.com/en/actions

## Step 10.9: Test the Testing Setup

```bash
# Navigate to backend
cd backend

# Run unit tests only
dotnet test Whispra.UnitTests/Whispra.UnitTests.csproj

# Run integration tests only (make sure Docker is running!)
docker compose up -d
dotnet test Whispra.IntegrationTests/Whispra.IntegrationTests.csproj

# Run all tests
dotnet test

# Run tests with coverage
dotnet test /p:CollectCoverage=true

# Run tests in watch mode (re-runs on file changes)
dotnet watch test --project Whispra.UnitTests/Whispra.UnitTests.csproj
```

**Expected output**:

```
Starting test execution, please wait...
A total of 1 test files matched the specified pattern.

Passed!  - Failed:     0, Passed:    25, Skipped:     0, Total:    25, Duration: 2s
```

**Study needed**: ✅ Just testing

---

**🔄 CHECKPOINT - Testing & Quality Assurance Complete!**

You now have:

- ✅ Unit tests for Use Cases (business logic)
- ✅ Integration tests for Repositories (database operations)
- ✅ API integration tests (end-to-end HTTP testing)
- ✅ Test fixtures and helpers (MongoDbFixture, WebApplicationFactory)
- ✅ Code coverage measurement
- ✅ CI/CD pipeline with GitHub Actions
- ✅ Automated testing on every push/PR
- ✅ Testing best practices and patterns

**Test Coverage Summary**:

- Unit Tests: ~50+ tests
- Integration Tests: ~20+ tests
- API Tests: ~15+ tests
- Total: ~85+ tests
- Coverage: 70-80% (should increase as you add more tests)

Before moving to frontend development, confirm:

1. ✅ Can you run all tests successfully?
2. ✅ Do tests pass in CI/CD?
3. ✅ Is code coverage >70%?
4. ✅ Are you following AAA pattern?
5. ✅ Do you understand mocking vs real dependencies?

**Next Phase Preview**: We'll start building the Frontend (React Native mobile app with Expo, Tauri desktop app with React+Vite). This is where users will actually interact with our backend!

---

# PHASE 11: Frontend - React Native Mobile App

**Goal**: Build a cross-platform mobile app (iOS & Android) using React Native and Expo that connects to our backend API, providing users with authentication, feed browsing, posting, messaging, and all other features.

## Step 11.1: Understanding React Native & Expo

**What is React Native?**

- **Cross-platform framework**: Write once, run on iOS and Android
- **JavaScript/TypeScript**: Use React knowledge to build mobile apps
- **Native components**: Renders to actual native UI components
- **Hot reload**: See changes instantly without recompiling

**What is Expo?**

- **Developer toolchain** built on top of React Native
- **Easier setup**: No need for Xcode or Android Studio initially
- **Built-in features**: Camera, location, notifications, file system
- **Expo Go**: Test app on real device without building
- **Easy deployment**: Build and publish with simple commands

**App Architecture**:

```
Mobile App (React Native + Expo)
    ↓
API Client (Axios)
    ↓
Backend API (ASP.NET Core)
    ↓
MongoDB + MinIO + Redis
```

📚 **Study needed**: React Native and Expo basics (4-6 hours)

- Resource: https://reactnative.dev/docs/getting-started
- Resource: https://docs.expo.dev/tutorial/introduction/
- Resource: https://www.youtube.com/watch?v=0-S5a0eXPoc (React Native Crash Course)

## Step 11.2: Set Up Expo Project

**Prerequisites**:

```bash
# Make sure Node.js is installed
node --version  # Should be v18 or higher

# Install Expo CLI globally
npm install -g expo-cli

# Or use npx (doesn't require global install)
npx expo --version
```

**Create the project**:

```bash
# Navigate to apps folder
cd apps

# Create Expo app with TypeScript
npx create-expo-app mobile --template expo-template-blank-typescript

# Navigate into the project
cd mobile

# Install additional dependencies
npm install @react-navigation/native @react-navigation/native-stack @react-navigation/bottom-tabs
npm install react-native-screens react-native-safe-area-context
npm install axios @microsoft/signalr
npm install @react-native-async-storage/async-storage
npm install react-native-gesture-handler react-native-reanimated
npm install expo-image-picker expo-secure-store

# Install dev dependencies
npm install -D @types/react @types/react-native
```

**Project structure**:

```
mobile/
├── app/                    # App screens (Expo Router)
├── components/             # Reusable components
├── services/               # API services
├── contexts/               # React Context for state
├── types/                  # TypeScript types
├── utils/                  # Helper functions
├── assets/                 # Images, fonts
├── App.tsx                 # Root component
├── app.json                # Expo configuration
├── tsconfig.json           # TypeScript config
└── package.json
```

**Technologies**: React Native, Expo, TypeScript, React Navigation

**Study needed**:

- 📚 React Navigation (2-3 hours)
  - Resource: https://reactnavigation.org/docs/getting-started
- 📚 TypeScript with React (2-3 hours)
  - Resource: https://react-typescript-cheatsheet.netlify.app/

## Step 11.3: Create TypeScript Types

Create shared types for API responses.

`mobile/types/api.ts`:

```typescript
// User types
export interface User {
  id: string;
  username: string;
  email: string;
  fullName: string;
  bio?: string;
  avatarUrl?: string;
  followersCount: number;
  followingCount: number;
  createdAt: string;
}

// Auth types
export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  email: string;
  fullName: string;
  password: string;
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  user: User;
}

// Post types
export enum PostType {
  Text = 0,
  Photo = 1,
  Video = 2,
}

export enum PostVisibility {
  Public = 0,
  FollowersOnly = 1,
  CommunityOnly = 2,
  Private = 3,
}

export interface Post {
  id: string;
  authorId: string;
  authorUsername: string;
  authorAvatarUrl?: string;
  textContent?: string;
  mediaUrls: string[];
  type: PostType;
  visibility: PostVisibility;
  communityId?: string;
  communityName?: string;
  commentCount: number;
  reactionCounts: Record<string, number>;
  currentUserReaction?: string;
  createdAt: string;
}

export interface CreatePostRequest {
  textContent?: string;
  type: PostType;
  visibility: PostVisibility;
  communityId?: string;
  mediaUrls?: string[];
}

// Comment types
export interface Comment {
  id: string;
  postId: string;
  authorId: string;
  authorUsername: string;
  authorAvatarUrl?: string;
  content: string;
  parentCommentId?: string;
  replyCount: number;
  reactionCount: number;
  currentUserReaction?: string;
  createdAt: string;
}

// Community types
export enum CommunityPrivacy {
  Public = 0,
  Private = 1,
}

export interface Community {
  id: string;
  name: string;
  description: string;
  privacy: CommunityPrivacy;
  memberCount: number;
  currentUserRole?: number; // 0=Member, 1=Moderator, 2=Owner
  coverImageUrl?: string;
  createdAt: string;
}

// Message types
export interface Message {
  id: string;
  conversationId: string;
  senderId: string;
  senderUsername: string;
  content: string;
  status: number; // 0=Sent, 1=Delivered, 2=Read
  createdAt: string;
}

export interface Conversation {
  id: string;
  type: number; // 0=Direct, 1=Group
  participants: User[];
  lastMessage?: Message;
  unreadCount: number;
  updatedAt: string;
}

// Notification types
export interface Notification {
  id: string;
  type: number;
  title: string;
  message: string;
  isRead: boolean;
  relatedEntityId?: string;
  relatedEntityType?: string;
  createdAt: string;
}

// API Response wrapper
export interface ApiResponse<T> {
  data?: T;
  error?: string;
  message?: string;
}

// Pagination
export interface PaginatedResponse<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  hasMore: boolean;
}
```

**Technologies**: TypeScript interfaces and enums

**Study needed**: ✅ None, straightforward type definitions

## Step 11.4: Create API Service

Set up Axios client with interceptors for authentication.

`mobile/services/api.ts`:

```typescript
import axios, { AxiosInstance, AxiosError } from "axios";
import * as SecureStore from "expo-secure-store";

const API_BASE_URL = "http://localhost:5000/api"; // Change for production

class ApiService {
  private client: AxiosInstance;

  constructor() {
    this.client = axios.create({
      baseURL: API_BASE_URL,
      headers: {
        "Content-Type": "application/json",
      },
      timeout: 10000,
    });

    // Request interceptor - add auth token
    this.client.interceptors.request.use(
      async (config) => {
        const token = await SecureStore.getItemAsync("accessToken");
        if (token) {
          config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
      },
      (error) => Promise.reject(error)
    );

    // Response interceptor - handle token refresh
    this.client.interceptors.response.use(
      (response) => response,
      async (error: AxiosError) => {
        const originalRequest = error.config;

        // If 401 and not already retrying, try to refresh token
        if (
          error.response?.status === 401 &&
          originalRequest &&
          !originalRequest._retry
        ) {
          originalRequest._retry = true;

          try {
            const refreshToken = await SecureStore.getItemAsync("refreshToken");
            if (refreshToken) {
              const response = await axios.post(
                `${API_BASE_URL}/Auth/refresh`,
                {
                  refreshToken,
                }
              );

              const { accessToken, refreshToken: newRefreshToken } =
                response.data;

              await SecureStore.setItemAsync("accessToken", accessToken);
              await SecureStore.setItemAsync("refreshToken", newRefreshToken);

              originalRequest.headers.Authorization = `Bearer ${accessToken}`;
              return this.client(originalRequest);
            }
          } catch (refreshError) {
            // Refresh failed, logout user
            await this.clearTokens();
            // Navigate to login screen (handled by AuthContext)
            return Promise.reject(refreshError);
          }
        }

        return Promise.reject(error);
      }
    );
  }

  private async clearTokens() {
    await SecureStore.deleteItemAsync("accessToken");
    await SecureStore.deleteItemAsync("refreshToken");
  }

  // Auth endpoints
  async login(email: string, password: string) {
    const response = await this.client.post("/Auth/login", { email, password });
    return response.data;
  }

  async register(data: {
    username: string;
    email: string;
    fullName: string;
    password: string;
  }) {
    const response = await this.client.post("/Users/register", data);
    return response.data;
  }

  async getCurrentUser() {
    const response = await this.client.get("/Auth/me");
    return response.data;
  }

  async logout() {
    await this.clearTokens();
  }

  // Posts endpoints
  async getPosts(page = 1, pageSize = 20) {
    const response = await this.client.get("/Posts/feed", {
      params: { page, pageSize },
    });
    return response.data;
  }

  async createPost(data: {
    textContent?: string;
    type: number;
    visibility: number;
    communityId?: string;
  }) {
    const response = await this.client.post("/Posts", data);
    return response.data;
  }

  async likePost(postId: string) {
    const response = await this.client.post(`/Posts/${postId}/reactions`, {
      type: 0,
    }); // 0 = Like
    return response.data;
  }

  async getComments(postId: string, page = 1, pageSize = 20) {
    const response = await this.client.get(`/Posts/${postId}/comments`, {
      params: { page, pageSize },
    });
    return response.data;
  }

  async addComment(postId: string, content: string) {
    const response = await this.client.post(`/Posts/${postId}/comments`, {
      content,
    });
    return response.data;
  }

  // Communities endpoints
  async getCommunities(page = 1, pageSize = 20) {
    const response = await this.client.get("/Communities", {
      params: { page, pageSize },
    });
    return response.data;
  }

  async getCommunity(id: string) {
    const response = await this.client.get(`/Communities/${id}`);
    return response.data;
  }

  async joinCommunity(id: string, inviteCode?: string) {
    const response = await this.client.post(`/Communities/${id}/join`, {
      inviteCode,
    });
    return response.data;
  }

  // Messages endpoints
  async getConversations(page = 1, pageSize = 20) {
    const response = await this.client.get("/Messages/conversations", {
      params: { page, pageSize },
    });
    return response.data;
  }

  async getMessages(conversationId: string, page = 1, pageSize = 50) {
    const response = await this.client.get(
      `/Messages/conversations/${conversationId}/messages`,
      {
        params: { page, pageSize },
      }
    );
    return response.data;
  }

  // Notifications endpoints
  async getNotifications(isRead?: boolean, page = 1, pageSize = 20) {
    const response = await this.client.get("/Notifications", {
      params: { isRead, page, pageSize },
    });
    return response.data;
  }

  async markNotificationAsRead(id: string) {
    const response = await this.client.put(`/Notifications/${id}/read`);
    return response.data;
  }

  async markAllNotificationsAsRead() {
    const response = await this.client.put("/Notifications/read-all");
    return response.data;
  }

  // Get raw axios instance for custom requests
  getClient(): AxiosInstance {
    return this.client;
  }
}

export default new ApiService();
```

**Technologies**: Axios, Expo SecureStore, JWT token management

**Study needed**:

- 📚 Axios interceptors (1-2 hours)
  - Resource: https://axios-http.com/docs/interceptors

## Step 11.5: Create Authentication Context

Manage auth state globally with React Context.

`mobile/contexts/AuthContext.tsx`:

```typescript
import React, { createContext, useState, useContext, useEffect } from "react";
import * as SecureStore from "expo-secure-store";
import apiService from "../services/api";
import { User, AuthResponse } from "../types/api";

interface AuthContextData {
  user: User | null;
  loading: boolean;
  isAuthenticated: boolean;
  login: (email: string, password: string) => Promise<void>;
  register: (
    username: string,
    email: string,
    fullName: string,
    password: string
  ) => Promise<void>;
  logout: () => Promise<void>;
}

const AuthContext = createContext<AuthContextData>({} as AuthContextData);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadUser();
  }, []);

  const loadUser = async () => {
    try {
      const token = await SecureStore.getItemAsync("accessToken");
      if (token) {
        const userData = await apiService.getCurrentUser();
        setUser(userData);
      }
    } catch (error) {
      console.error("Failed to load user:", error);
      await SecureStore.deleteItemAsync("accessToken");
      await SecureStore.deleteItemAsync("refreshToken");
    } finally {
      setLoading(false);
    }
  };

  const login = async (email: string, password: string) => {
    try {
      const response: AuthResponse = await apiService.login(email, password);
      await SecureStore.setItemAsync("accessToken", response.accessToken);
      await SecureStore.setItemAsync("refreshToken", response.refreshToken);
      setUser(response.user);
    } catch (error) {
      throw error;
    }
  };

  const register = async (
    username: string,
    email: string,
    fullName: string,
    password: string
  ) => {
    try {
      await apiService.register({ username, email, fullName, password });
      // After registration, login automatically
      await login(email, password);
    } catch (error) {
      throw error;
    }
  };

  const logout = async () => {
    try {
      await apiService.logout();
      setUser(null);
    } catch (error) {
      console.error("Logout error:", error);
    }
  };

  return (
    <AuthContext.Provider
      value={{
        user,
        loading,
        isAuthenticated: !!user,
        login,
        register,
        logout,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used within AuthProvider");
  }
  return context;
};
```

**Technologies**: React Context API, hooks

**Study needed**:

- 📚 React Context API (2-3 hours)
  - Resource: https://react.dev/learn/passing-data-deeply-with-context

## Step 11.6: Create Authentication Screens

**Login Screen**:

`mobile/screens/auth/LoginScreen.tsx`:

```typescript
import React, { useState } from "react";
import {
  View,
  Text,
  TextInput,
  TouchableOpacity,
  StyleSheet,
  Alert,
  ActivityIndicator,
  KeyboardAvoidingView,
  Platform,
} from "react-native";
import { useAuth } from "../../contexts/AuthContext";

export default function LoginScreen({ navigation }: any) {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [loading, setLoading] = useState(false);
  const { login } = useAuth();

  const handleLogin = async () => {
    if (!email || !password) {
      Alert.alert("Error", "Please fill in all fields");
      return;
    }

    setLoading(true);
    try {
      await login(email, password);
      // Navigation handled by AuthContext
    } catch (error: any) {
      Alert.alert(
        "Login Failed",
        error.response?.data?.error || "Invalid email or password"
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <KeyboardAvoidingView
      behavior={Platform.OS === "ios" ? "padding" : "height"}
      style={styles.container}
    >
      <View style={styles.content}>
        <Text style={styles.title}>Welcome to Whispra</Text>
        <Text style={styles.subtitle}>Sign in to continue</Text>

        <TextInput
          style={styles.input}
          placeholder="Email"
          value={email}
          onChangeText={setEmail}
          autoCapitalize="none"
          keyboardType="email-address"
          editable={!loading}
        />

        <TextInput
          style={styles.input}
          placeholder="Password"
          value={password}
          onChangeText={setPassword}
          secureTextEntry
          editable={!loading}
        />

        <TouchableOpacity
          style={styles.button}
          onPress={handleLogin}
          disabled={loading}
        >
          {loading ? (
            <ActivityIndicator color="#fff" />
          ) : (
            <Text style={styles.buttonText}>Sign In</Text>
          )}
        </TouchableOpacity>

        <TouchableOpacity
          onPress={() => navigation.navigate("Register")}
          disabled={loading}
        >
          <Text style={styles.link}>Don't have an account? Sign up</Text>
        </TouchableOpacity>
      </View>
    </KeyboardAvoidingView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: "#fff",
  },
  content: {
    flex: 1,
    justifyContent: "center",
    padding: 20,
  },
  title: {
    fontSize: 32,
    fontWeight: "bold",
    marginBottom: 8,
    textAlign: "center",
  },
  subtitle: {
    fontSize: 16,
    color: "#666",
    marginBottom: 40,
    textAlign: "center",
  },
  input: {
    height: 50,
    borderWidth: 1,
    borderColor: "#ddd",
    borderRadius: 8,
    paddingHorizontal: 16,
    marginBottom: 16,
    fontSize: 16,
  },
  button: {
    height: 50,
    backgroundColor: "#007AFF",
    borderRadius: 8,
    justifyContent: "center",
    alignItems: "center",
    marginTop: 8,
  },
  buttonText: {
    color: "#fff",
    fontSize: 16,
    fontWeight: "600",
  },
  link: {
    color: "#007AFF",
    textAlign: "center",
    marginTop: 20,
    fontSize: 14,
  },
});
```

**Register Screen**:

`mobile/screens/auth/RegisterScreen.tsx`:

```typescript
import React, { useState } from "react";
import {
  View,
  Text,
  TextInput,
  TouchableOpacity,
  StyleSheet,
  Alert,
  ActivityIndicator,
  KeyboardAvoidingView,
  Platform,
  ScrollView,
} from "react-native";
import { useAuth } from "../../contexts/AuthContext";

export default function RegisterScreen({ navigation }: any) {
  const [username, setUsername] = useState("");
  const [email, setEmail] = useState("");
  const [fullName, setFullName] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [loading, setLoading] = useState(false);
  const { register } = useAuth();

  const handleRegister = async () => {
    if (!username || !email || !fullName || !password || !confirmPassword) {
      Alert.alert("Error", "Please fill in all fields");
      return;
    }

    if (password !== confirmPassword) {
      Alert.alert("Error", "Passwords do not match");
      return;
    }

    if (password.length < 6) {
      Alert.alert("Error", "Password must be at least 6 characters");
      return;
    }

    setLoading(true);
    try {
      await register(username, email, fullName, password);
      // Navigation handled by AuthContext
    } catch (error: any) {
      Alert.alert(
        "Registration Failed",
        error.response?.data?.error || "Please try again"
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <KeyboardAvoidingView
      behavior={Platform.OS === "ios" ? "padding" : "height"}
      style={styles.container}
    >
      <ScrollView contentContainerStyle={styles.scrollContent}>
        <View style={styles.content}>
          <Text style={styles.title}>Create Account</Text>
          <Text style={styles.subtitle}>Join Whispra today</Text>

          <TextInput
            style={styles.input}
            placeholder="Username"
            value={username}
            onChangeText={setUsername}
            autoCapitalize="none"
            editable={!loading}
          />

          <TextInput
            style={styles.input}
            placeholder="Full Name"
            value={fullName}
            onChangeText={setFullName}
            editable={!loading}
          />

          <TextInput
            style={styles.input}
            placeholder="Email"
            value={email}
            onChangeText={setEmail}
            autoCapitalize="none"
            keyboardType="email-address"
            editable={!loading}
          />

          <TextInput
            style={styles.input}
            placeholder="Password"
            value={password}
            onChangeText={setPassword}
            secureTextEntry
            editable={!loading}
          />

          <TextInput
            style={styles.input}
            placeholder="Confirm Password"
            value={confirmPassword}
            onChangeText={setConfirmPassword}
            secureTextEntry
            editable={!loading}
          />

          <TouchableOpacity
            style={styles.button}
            onPress={handleRegister}
            disabled={loading}
          >
            {loading ? (
              <ActivityIndicator color="#fff" />
            ) : (
              <Text style={styles.buttonText}>Sign Up</Text>
            )}
          </TouchableOpacity>

          <TouchableOpacity
            onPress={() => navigation.goBack()}
            disabled={loading}
          >
            <Text style={styles.link}>Already have an account? Sign in</Text>
          </TouchableOpacity>
        </View>
      </ScrollView>
    </KeyboardAvoidingView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: "#fff",
  },
  scrollContent: {
    flexGrow: 1,
  },
  content: {
    flex: 1,
    justifyContent: "center",
    padding: 20,
  },
  title: {
    fontSize: 32,
    fontWeight: "bold",
    marginBottom: 8,
    textAlign: "center",
  },
  subtitle: {
    fontSize: 16,
    color: "#666",
    marginBottom: 40,
    textAlign: "center",
  },
  input: {
    height: 50,
    borderWidth: 1,
    borderColor: "#ddd",
    borderRadius: 8,
    paddingHorizontal: 16,
    marginBottom: 16,
    fontSize: 16,
  },
  button: {
    height: 50,
    backgroundColor: "#007AFF",
    borderRadius: 8,
    justifyContent: "center",
    alignItems: "center",
    marginTop: 8,
  },
  buttonText: {
    color: "#fff",
    fontSize: 16,
    fontWeight: "600",
  },
  link: {
    color: "#007AFF",
    textAlign: "center",
    marginTop: 20,
    fontSize: 14,
  },
});
```

**Technologies**: React Native components, form handling

**Study needed**:

- 📚 React Native UI components (2-3 hours)
  - Resource: https://reactnative.dev/docs/components-and-apis

## Step 11.7: Set Up Navigation

`mobile/navigation/AppNavigator.tsx`:

```typescript
import React from "react";
import { NavigationContainer } from "@react-navigation/native";
import { createNativeStackNavigator } from "@react-navigation/native-stack";
import { createBottomTabNavigator } from "@react-navigation/bottom-tabs";
import { Ionicons } from "@expo/vector-icons";

import { useAuth } from "../contexts/AuthContext";

// Auth screens
import LoginScreen from "../screens/auth/LoginScreen";
import RegisterScreen from "../screens/auth/RegisterScreen";

// Main screens
import HomeScreen from "../screens/main/HomeScreen";
import CommunitiesScreen from "../screens/main/CommunitiesScreen";
import MessagesScreen from "../screens/main/MessagesScreen";
import NotificationsScreen from "../screens/main/NotificationsScreen";
import ProfileScreen from "../screens/main/ProfileScreen";

const Stack = createNativeStackNavigator();
const Tab = createBottomTabNavigator();

function AuthStack() {
  return (
    <Stack.Navigator screenOptions={{ headerShown: false }}>
      <Stack.Screen name="Login" component={LoginScreen} />
      <Stack.Screen name="Register" component={RegisterScreen} />
    </Stack.Navigator>
  );
}

function MainTabs() {
  return (
    <Tab.Navigator
      screenOptions={({ route }) => ({
        tabBarIcon: ({ focused, color, size }) => {
          let iconName: any;

          if (route.name === "Home") {
            iconName = focused ? "home" : "home-outline";
          } else if (route.name === "Communities") {
            iconName = focused ? "people" : "people-outline";
          } else if (route.name === "Messages") {
            iconName = focused ? "chatbubbles" : "chatbubbles-outline";
          } else if (route.name === "Notifications") {
            iconName = focused ? "notifications" : "notifications-outline";
          } else if (route.name === "Profile") {
            iconName = focused ? "person" : "person-outline";
          }

          return <Ionicons name={iconName} size={size} color={color} />;
        },
        tabBarActiveTintColor: "#007AFF",
        tabBarInactiveTintColor: "gray",
      })}
    >
      <Tab.Screen name="Home" component={HomeScreen} />
      <Tab.Screen name="Communities" component={CommunitiesScreen} />
      <Tab.Screen name="Messages" component={MessagesScreen} />
      <Tab.Screen name="Notifications" component={NotificationsScreen} />
      <Tab.Screen name="Profile" component={ProfileScreen} />
    </Tab.Navigator>
  );
}

export default function AppNavigator() {
  const { isAuthenticated, loading } = useAuth();

  if (loading) {
    return null; // Or a loading screen
  }

  return (
    <NavigationContainer>
      {isAuthenticated ? <MainTabs /> : <AuthStack />}
    </NavigationContainer>
  );
}
```

**Technologies**: React Navigation, tab navigator, stack navigator

**Study needed**: ✅ None if you've studied React Navigation basics

## Step 11.8: Create Main Screens (Home Feed)

`mobile/screens/main/HomeScreen.tsx`:

```typescript
import React, { useState, useEffect } from "react";
import {
  View,
  FlatList,
  StyleSheet,
  RefreshControl,
  ActivityIndicator,
  Text,
} from "react-native";
import apiService from "../../services/api";
import PostCard from "../../components/PostCard";
import { Post } from "../../types/api";

export default function HomeScreen() {
  const [posts, setPosts] = useState<Post[]>([]);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);

  useEffect(() => {
    loadPosts();
  }, []);

  const loadPosts = async (pageNum = 1) => {
    try {
      const response = await apiService.getPosts(pageNum, 20);
      if (pageNum === 1) {
        setPosts(response.items);
      } else {
        setPosts([...posts, ...response.items]);
      }
      setHasMore(response.hasMore);
      setPage(pageNum);
    } catch (error) {
      console.error("Failed to load posts:", error);
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  };

  const handleRefresh = () => {
    setRefreshing(true);
    loadPosts(1);
  };

  const handleLoadMore = () => {
    if (!loading && hasMore) {
      loadPosts(page + 1);
    }
  };

  const handleLike = async (postId: string) => {
    try {
      await apiService.likePost(postId);
      // Update local state
      setPosts(
        posts.map((p) =>
          p.id === postId ? { ...p, currentUserReaction: "Like" } : p
        )
      );
    } catch (error) {
      console.error("Failed to like post:", error);
    }
  };

  if (loading) {
    return (
      <View style={styles.center}>
        <ActivityIndicator size="large" color="#007AFF" />
      </View>
    );
  }

  return (
    <View style={styles.container}>
      <FlatList
        data={posts}
        keyExtractor={(item) => item.id}
        renderItem={({ item }) => (
          <PostCard post={item} onLike={() => handleLike(item.id)} />
        )}
        refreshControl={
          <RefreshControl refreshing={refreshing} onRefresh={handleRefresh} />
        }
        onEndReached={handleLoadMore}
        onEndReachedThreshold={0.5}
        ListFooterComponent={() =>
          loading && hasMore ? (
            <ActivityIndicator style={styles.loader} />
          ) : null
        }
        ListEmptyComponent={
          <View style={styles.empty}>
            <Text style={styles.emptyText}>No posts yet</Text>
          </View>
        }
      />
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: "#f5f5f5",
  },
  center: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center",
  },
  loader: {
    marginVertical: 20,
  },
  empty: {
    padding: 40,
    alignItems: "center",
  },
  emptyText: {
    fontSize: 16,
    color: "#999",
  },
});
```

**Post Card Component**:

`mobile/components/PostCard.tsx`:

```typescript
import React from "react";
import { View, Text, Image, TouchableOpacity, StyleSheet } from "react";
import { Ionicons } from "@expo/vector-icons";
import { Post } from "../types/api";

interface PostCardProps {
  post: Post;
  onLike: () => void;
}

export default function PostCard({ post, onLike }: PostCardProps) {
  const formatTime = (dateString: string) => {
    const date = new Date(dateString);
    const now = new Date();
    const diffMs = now.getTime() - date.getTime();
    const diffMins = Math.floor(diffMs / 60000);
    const diffHours = Math.floor(diffMs / 3600000);
    const diffDays = Math.floor(diffMs / 86400000);

    if (diffMins < 1) return "Just now";
    if (diffMins < 60) return `${diffMins}m ago`;
    if (diffHours < 24) return `${diffHours}h ago`;
    if (diffDays < 7) return `${diffDays}d ago`;
    return date.toLocaleDateString();
  };

  return (
    <View style={styles.card}>
      {/* Author info */}
      <View style={styles.header}>
        {post.authorAvatarUrl ? (
          <Image source={{ uri: post.authorAvatarUrl }} style={styles.avatar} />
        ) : (
          <View style={[styles.avatar, styles.avatarPlaceholder]}>
            <Text style={styles.avatarText}>
              {post.authorUsername[0].toUpperCase()}
            </Text>
          </View>
        )}
        <View style={styles.authorInfo}>
          <Text style={styles.authorName}>{post.authorUsername}</Text>
          <Text style={styles.timestamp}>{formatTime(post.createdAt)}</Text>
        </View>
      </View>

      {/* Post content */}
      {post.textContent && (
        <Text style={styles.content}>{post.textContent}</Text>
      )}

      {/* Media */}
      {post.mediaUrls.length > 0 && (
        <Image
          source={{ uri: post.mediaUrls[0] }}
          style={styles.media}
          resizeMode="cover"
        />
      )}

      {/* Actions */}
      <View style={styles.actions}>
        <TouchableOpacity style={styles.action} onPress={onLike}>
          <Ionicons
            name={
              post.currentUserReaction === "Like" ? "heart" : "heart-outline"
            }
            size={24}
            color={post.currentUserReaction === "Like" ? "#ff3b30" : "#666"}
          />
          <Text style={styles.actionText}>
            {post.reactionCounts?.Like || 0}
          </Text>
        </TouchableOpacity>

        <TouchableOpacity style={styles.action}>
          <Ionicons name="chatbubble-outline" size={22} color="#666" />
          <Text style={styles.actionText}>{post.commentCount}</Text>
        </TouchableOpacity>

        <TouchableOpacity style={styles.action}>
          <Ionicons name="share-outline" size={24} color="#666" />
        </TouchableOpacity>
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  card: {
    backgroundColor: "#fff",
    marginBottom: 8,
    padding: 16,
  },
  header: {
    flexDirection: "row",
    alignItems: "center",
    marginBottom: 12,
  },
  avatar: {
    width: 40,
    height: 40,
    borderRadius: 20,
    marginRight: 12,
  },
  avatarPlaceholder: {
    backgroundColor: "#007AFF",
    justifyContent: "center",
    alignItems: "center",
  },
  avatarText: {
    color: "#fff",
    fontSize: 18,
    fontWeight: "bold",
  },
  authorInfo: {
    flex: 1,
  },
  authorName: {
    fontSize: 16,
    fontWeight: "600",
    marginBottom: 2,
  },
  timestamp: {
    fontSize: 12,
    color: "#999",
  },
  content: {
    fontSize: 15,
    lineHeight: 20,
    marginBottom: 12,
  },
  media: {
    width: "100%",
    height: 250,
    borderRadius: 8,
    marginBottom: 12,
  },
  actions: {
    flexDirection: "row",
    borderTopWidth: 1,
    borderTopColor: "#f0f0f0",
    paddingTop: 12,
  },
  action: {
    flexDirection: "row",
    alignItems: "center",
    marginRight: 24,
  },
  actionText: {
    marginLeft: 6,
    fontSize: 14,
    color: "#666",
  },
});
```

**Technologies**: FlatList, pull-to-refresh, infinite scroll

**Study needed**:

- 📚 FlatList optimization (1-2 hours)
  - Resource: https://reactnative.dev/docs/optimizing-flatlist-configuration

---

**🔄 CHECKPOINT - Mobile App Foundation Complete!**

You now have:

- ✅ Expo + React Native project set up
- ✅ TypeScript types for all API entities
- ✅ API service with Axios and token refresh
- ✅ Authentication context and screens
- ✅ Navigation (tab + stack navigators)
- ✅ Home feed with posts
- ✅ Pull-to-refresh and infinite scroll

**Next steps** (to be added in future phases):

- Create remaining screens (Communities, Messages, Notifications, Profile)
- Implement SignalR for real-time messaging
- Add image picker and media upload
- Create post creation screen
- Build comment threads
- Add push notifications
- Optimize performance
- Test on iOS and Android devices

Before testing, confirm:

1. ✅ Backend API is running (localhost:5000)
2. ✅ MongoDB, Redis, MinIO are running (docker compose up)
3. ✅ Updated API_BASE_URL in api.ts to your computer's IP (not localhost for physical devices)

**Run the app**:

```bash
cd apps/mobile
npm start

# Then:
# - Press 'i' for iOS simulator
# - Press 'a' for Android emulator
# - Scan QR code with Expo Go app on your phone
```

**Study needed**: ✅ Just testing and refinement

---

# PHASE 12: Mobile App - Advanced Features

**Goal**: Complete the mobile app by implementing Communities, Messages, Notifications, Profile screens, real-time messaging with SignalR, media upload, post creation, and push notifications.

## Step 12.1: Create Communities Screen

Display list of communities with join functionality.

`mobile/screens/main/CommunitiesScreen.tsx`:

```typescript
import React, { useState, useEffect } from "react";
import {
  View,
  FlatList,
  StyleSheet,
  RefreshControl,
  ActivityIndicator,
  Text,
  TouchableOpacity,
  Image,
} from "react-native";
import { Ionicons } from "@expo/vector-icons";
import apiService from "../../services/api";
import { Community } from "../../types/api";

export default function CommunitiesScreen({ navigation }: any) {
  const [communities, setCommunities] = useState<Community[]>([]);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);

  useEffect(() => {
    loadCommunities();
  }, []);

  const loadCommunities = async () => {
    try {
      const response = await apiService.getCommunities(1, 50);
      setCommunities(response.items);
    } catch (error) {
      console.error("Failed to load communities:", error);
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  };

  const handleRefresh = () => {
    setRefreshing(true);
    loadCommunities();
  };

  const handleJoinCommunity = async (communityId: string) => {
    try {
      await apiService.joinCommunity(communityId);
      // Refresh communities to update membership status
      loadCommunities();
    } catch (error: any) {
      console.error("Failed to join community:", error);
    }
  };

  const renderCommunity = ({ item }: { item: Community }) => (
    <TouchableOpacity
      style={styles.communityCard}
      onPress={() => navigation.navigate("CommunityDetail", { id: item.id })}
    >
      <View style={styles.communityHeader}>
        {item.coverImageUrl ? (
          <Image
            source={{ uri: item.coverImageUrl }}
            style={styles.coverImage}
          />
        ) : (
          <View style={[styles.coverImage, styles.coverPlaceholder]}>
            <Ionicons name="people" size={40} color="#fff" />
          </View>
        )}
      </View>

      <View style={styles.communityInfo}>
        <View style={styles.nameRow}>
          <Text style={styles.communityName}>{item.name}</Text>
          {item.privacy === 1 && (
            <Ionicons name="lock-closed" size={16} color="#999" />
          )}
        </View>

        <Text style={styles.communityDescription} numberOfLines={2}>
          {item.description}
        </Text>

        <View style={styles.communityStats}>
          <Ionicons name="people-outline" size={16} color="#666" />
          <Text style={styles.statsText}>{item.memberCount} members</Text>
        </View>

        {!item.currentUserRole && (
          <TouchableOpacity
            style={styles.joinButton}
            onPress={() => handleJoinCommunity(item.id)}
          >
            <Text style={styles.joinButtonText}>Join</Text>
          </TouchableOpacity>
        )}

        {item.currentUserRole !== undefined && (
          <View style={styles.memberBadge}>
            <Ionicons name="checkmark-circle" size={16} color="#34C759" />
            <Text style={styles.memberBadgeText}>Member</Text>
          </View>
        )}
      </View>
    </TouchableOpacity>
  );

  if (loading) {
    return (
      <View style={styles.center}>
        <ActivityIndicator size="large" color="#007AFF" />
      </View>
    );
  }

  return (
    <View style={styles.container}>
      <FlatList
        data={communities}
        keyExtractor={(item) => item.id}
        renderItem={renderCommunity}
        refreshControl={
          <RefreshControl refreshing={refreshing} onRefresh={handleRefresh} />
        }
        contentContainerStyle={styles.list}
        ListEmptyComponent={
          <View style={styles.empty}>
            <Text style={styles.emptyText}>No communities found</Text>
          </View>
        }
      />
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: "#f5f5f5",
  },
  center: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center",
  },
  list: {
    padding: 16,
  },
  communityCard: {
    backgroundColor: "#fff",
    borderRadius: 12,
    marginBottom: 16,
    overflow: "hidden",
  },
  communityHeader: {
    height: 120,
  },
  coverImage: {
    width: "100%",
    height: "100%",
  },
  coverPlaceholder: {
    backgroundColor: "#007AFF",
    justifyContent: "center",
    alignItems: "center",
  },
  communityInfo: {
    padding: 16,
  },
  nameRow: {
    flexDirection: "row",
    alignItems: "center",
    gap: 8,
    marginBottom: 8,
  },
  communityName: {
    fontSize: 18,
    fontWeight: "bold",
  },
  communityDescription: {
    fontSize: 14,
    color: "#666",
    marginBottom: 12,
    lineHeight: 20,
  },
  communityStats: {
    flexDirection: "row",
    alignItems: "center",
    gap: 6,
    marginBottom: 12,
  },
  statsText: {
    fontSize: 14,
    color: "#666",
  },
  joinButton: {
    backgroundColor: "#007AFF",
    paddingVertical: 8,
    paddingHorizontal: 16,
    borderRadius: 6,
    alignSelf: "flex-start",
  },
  joinButtonText: {
    color: "#fff",
    fontWeight: "600",
    fontSize: 14,
  },
  memberBadge: {
    flexDirection: "row",
    alignItems: "center",
    gap: 6,
  },
  memberBadgeText: {
    color: "#34C759",
    fontWeight: "600",
    fontSize: 14,
  },
  empty: {
    padding: 40,
    alignItems: "center",
  },
  emptyText: {
    fontSize: 16,
    color: "#999",
  },
});
```

**Technologies**: FlatList, image handling, community UI

**Study needed**: ✅ None, follows established patterns

## Step 12.2: Create Messages Screen with SignalR

Set up SignalR for real-time messaging.

**First, create SignalR service:**

`mobile/services/signalr.ts`:

```typescript
import * as SignalR from "@microsoft/signalr";
import * as SecureStore from "expo-secure-store";

const HUB_URL = "http://localhost:5000/hubs/chat"; // Change for production

class SignalRService {
  private connection: SignalR.HubConnection | null = null;
  private messageHandlers: ((message: any) => void)[] = [];
  private typingHandlers: ((data: any) => void)[] = [];
  private readReceiptHandlers: ((data: any) => void)[] = [];

  async connect() {
    if (this.connection?.state === SignalR.HubConnectionState.Connected) {
      return;
    }

    const token = await SecureStore.getItemAsync("accessToken");
    if (!token) {
      throw new Error("No access token available");
    }

    this.connection = new SignalR.HubConnectionBuilder()
      .withUrl(HUB_URL, {
        accessTokenFactory: () => token,
      })
      .withAutomaticReconnect()
      .build();

    // Set up event handlers
    this.connection.on("ReceiveMessage", (message) => {
      this.messageHandlers.forEach((handler) => handler(message));
    });

    this.connection.on("UserTyping", (data) => {
      this.typingHandlers.forEach((handler) => handler(data));
    });

    this.connection.on("MessageRead", (data) => {
      this.readReceiptHandlers.forEach((handler) => handler(data));
    });

    try {
      await this.connection.start();
      console.log("SignalR connected");
    } catch (error) {
      console.error("SignalR connection error:", error);
      throw error;
    }
  }

  async disconnect() {
    if (this.connection) {
      await this.connection.stop();
      this.connection = null;
    }
  }

  async joinConversation(conversationId: string) {
    if (this.connection?.state === SignalR.HubConnectionState.Connected) {
      await this.connection.invoke("JoinConversation", conversationId);
    }
  }

  async leaveConversation(conversationId: string) {
    if (this.connection?.state === SignalR.HubConnectionState.Connected) {
      await this.connection.invoke("LeaveConversation", conversationId);
    }
  }

  async sendMessage(conversationId: string, content: string) {
    if (this.connection?.state === SignalR.HubConnectionState.Connected) {
      await this.connection.invoke("SendMessage", conversationId, content);
    }
  }

  async sendTypingIndicator(conversationId: string) {
    if (this.connection?.state === SignalR.HubConnectionState.Connected) {
      await this.connection.invoke("SendTypingIndicator", conversationId);
    }
  }

  async markAsRead(conversationId: string, messageId: string) {
    if (this.connection?.state === SignalR.HubConnectionState.Connected) {
      await this.connection.invoke("MarkAsRead", conversationId, messageId);
    }
  }

  onMessage(handler: (message: any) => void) {
    this.messageHandlers.push(handler);
  }

  onTyping(handler: (data: any) => void) {
    this.typingHandlers.push(handler);
  }

  onReadReceipt(handler: (data: any) => void) {
    this.readReceiptHandlers.push(handler);
  }

  removeMessageHandler(handler: (message: any) => void) {
    this.messageHandlers = this.messageHandlers.filter((h) => h !== handler);
  }

  removeTypingHandler(handler: (data: any) => void) {
    this.typingHandlers = this.typingHandlers.filter((h) => h !== handler);
  }

  removeReadReceiptHandler(handler: (data: any) => void) {
    this.readReceiptHandlers = this.readReceiptHandlers.filter(
      (h) => h !== handler
    );
  }
}

export default new SignalRService();
```

**Messages List Screen:**

`mobile/screens/main/MessagesScreen.tsx`:

```typescript
import React, { useState, useEffect } from "react";
import {
  View,
  FlatList,
  StyleSheet,
  Text,
  TouchableOpacity,
  Image,
  ActivityIndicator,
} from "react-native";
import apiService from "../../services/api";
import { Conversation } from "../../types/api";
import { useAuth } from "../../contexts/AuthContext";

export default function MessagesScreen({ navigation }: any) {
  const [conversations, setConversations] = useState<Conversation[]>([]);
  const [loading, setLoading] = useState(true);
  const { user } = useAuth();

  useEffect(() => {
    loadConversations();
  }, []);

  const loadConversations = async () => {
    try {
      const response = await apiService.getConversations(1, 50);
      setConversations(response.items);
    } catch (error) {
      console.error("Failed to load conversations:", error);
    } finally {
      setLoading(false);
    }
  };

  const getOtherParticipant = (conversation: Conversation) => {
    return conversation.participants.find((p) => p.id !== user?.id);
  };

  const formatTime = (dateString: string) => {
    const date = new Date(dateString);
    const now = new Date();
    const diffMs = now.getTime() - date.getTime();
    const diffHours = Math.floor(diffMs / 3600000);
    const diffDays = Math.floor(diffMs / 86400000);

    if (diffHours < 24) {
      return date.toLocaleTimeString("en-US", {
        hour: "numeric",
        minute: "2-digit",
      });
    } else if (diffDays < 7) {
      return date.toLocaleDateString("en-US", { weekday: "short" });
    } else {
      return date.toLocaleDateString("en-US", {
        month: "short",
        day: "numeric",
      });
    }
  };

  const renderConversation = ({ item }: { item: Conversation }) => {
    const otherUser = getOtherParticipant(item);
    const unreadCount = item.unreadCount;

    return (
      <TouchableOpacity
        style={styles.conversationCard}
        onPress={() => navigation.navigate("Chat", { conversationId: item.id })}
      >
        {otherUser?.avatarUrl ? (
          <Image source={{ uri: otherUser.avatarUrl }} style={styles.avatar} />
        ) : (
          <View style={[styles.avatar, styles.avatarPlaceholder]}>
            <Text style={styles.avatarText}>
              {otherUser?.username[0].toUpperCase()}
            </Text>
          </View>
        )}

        <View style={styles.conversationInfo}>
          <View style={styles.topRow}>
            <Text style={styles.username}>
              {otherUser?.username || "Group"}
            </Text>
            {item.lastMessage && (
              <Text style={styles.timestamp}>
                {formatTime(item.lastMessage.createdAt)}
              </Text>
            )}
          </View>

          {item.lastMessage && (
            <View style={styles.messageRow}>
              <Text style={styles.lastMessage} numberOfLines={1}>
                {item.lastMessage.senderId === user?.id && "You: "}
                {item.lastMessage.content}
              </Text>
              {unreadCount > 0 && (
                <View style={styles.unreadBadge}>
                  <Text style={styles.unreadText}>{unreadCount}</Text>
                </View>
              )}
            </View>
          )}
        </View>
      </TouchableOpacity>
    );
  };

  if (loading) {
    return (
      <View style={styles.center}>
        <ActivityIndicator size="large" color="#007AFF" />
      </View>
    );
  }

  return (
    <View style={styles.container}>
      <FlatList
        data={conversations}
        keyExtractor={(item) => item.id}
        renderItem={renderConversation}
        ListEmptyComponent={
          <View style={styles.empty}>
            <Text style={styles.emptyText}>No conversations yet</Text>
          </View>
        }
      />
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: "#fff",
  },
  center: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center",
  },
  conversationCard: {
    flexDirection: "row",
    padding: 16,
    borderBottomWidth: 1,
    borderBottomColor: "#f0f0f0",
  },
  avatar: {
    width: 50,
    height: 50,
    borderRadius: 25,
    marginRight: 12,
  },
  avatarPlaceholder: {
    backgroundColor: "#007AFF",
    justifyContent: "center",
    alignItems: "center",
  },
  avatarText: {
    color: "#fff",
    fontSize: 20,
    fontWeight: "bold",
  },
  conversationInfo: {
    flex: 1,
  },
  topRow: {
    flexDirection: "row",
    justifyContent: "space-between",
    marginBottom: 4,
  },
  username: {
    fontSize: 16,
    fontWeight: "600",
  },
  timestamp: {
    fontSize: 12,
    color: "#999",
  },
  messageRow: {
    flexDirection: "row",
    alignItems: "center",
  },
  lastMessage: {
    flex: 1,
    fontSize: 14,
    color: "#666",
  },
  unreadBadge: {
    backgroundColor: "#007AFF",
    borderRadius: 10,
    minWidth: 20,
    height: 20,
    justifyContent: "center",
    alignItems: "center",
    paddingHorizontal: 6,
    marginLeft: 8,
  },
  unreadText: {
    color: "#fff",
    fontSize: 12,
    fontWeight: "bold",
  },
  empty: {
    padding: 40,
    alignItems: "center",
  },
  emptyText: {
    fontSize: 16,
    color: "#999",
  },
});
```

**Chat Screen with Real-time Messages:**

`mobile/screens/chat/ChatScreen.tsx`:

```typescript
import React, { useState, useEffect, useRef } from "react";
import {
  View,
  FlatList,
  TextInput,
  TouchableOpacity,
  StyleSheet,
  Text,
  KeyboardAvoidingView,
  Platform,
} from "react-native";
import { Ionicons } from "@expo/vector-icons";
import apiService from "../../services/api";
import signalRService from "../../services/signalr";
import { Message } from "../../types/api";
import { useAuth } from "../../contexts/AuthContext";

export default function ChatScreen({ route }: any) {
  const { conversationId } = route.params;
  const [messages, setMessages] = useState<Message[]>([]);
  const [newMessage, setNewMessage] = useState("");
  const [loading, setLoading] = useState(true);
  const { user } = useAuth();
  const flatListRef = useRef<FlatList>(null);

  useEffect(() => {
    loadMessages();
    connectSignalR();

    return () => {
      signalRService.leaveConversation(conversationId);
    };
  }, [conversationId]);

  const loadMessages = async () => {
    try {
      const response = await apiService.getMessages(conversationId, 1, 50);
      setMessages(response.items.reverse()); // Show oldest first
    } catch (error) {
      console.error("Failed to load messages:", error);
    } finally {
      setLoading(false);
    }
  };

  const connectSignalR = async () => {
    try {
      await signalRService.connect();
      await signalRService.joinConversation(conversationId);

      // Listen for new messages
      const messageHandler = (message: Message) => {
        if (message.conversationId === conversationId) {
          setMessages((prev) => [...prev, message]);
          // Auto-scroll to bottom
          setTimeout(() => {
            flatListRef.current?.scrollToEnd({ animated: true });
          }, 100);
        }
      };

      signalRService.onMessage(messageHandler);
    } catch (error) {
      console.error("SignalR connection error:", error);
    }
  };

  const handleSend = async () => {
    if (!newMessage.trim()) return;

    try {
      await signalRService.sendMessage(conversationId, newMessage.trim());
      setNewMessage("");
    } catch (error) {
      console.error("Failed to send message:", error);
    }
  };

  const renderMessage = ({ item }: { item: Message }) => {
    const isOwnMessage = item.senderId === user?.id;

    return (
      <View
        style={[
          styles.messageContainer,
          isOwnMessage && styles.ownMessageContainer,
        ]}
      >
        <View
          style={[
            styles.messageBubble,
            isOwnMessage && styles.ownMessageBubble,
          ]}
        >
          {!isOwnMessage && (
            <Text style={styles.senderName}>{item.senderUsername}</Text>
          )}
          <Text
            style={[styles.messageText, isOwnMessage && styles.ownMessageText]}
          >
            {item.content}
          </Text>
          <Text
            style={[styles.messageTime, isOwnMessage && styles.ownMessageTime]}
          >
            {new Date(item.createdAt).toLocaleTimeString("en-US", {
              hour: "numeric",
              minute: "2-digit",
            })}
          </Text>
        </View>
      </View>
    );
  };

  return (
    <KeyboardAvoidingView
      style={styles.container}
      behavior={Platform.OS === "ios" ? "padding" : undefined}
      keyboardVerticalOffset={90}
    >
      <FlatList
        ref={flatListRef}
        data={messages}
        keyExtractor={(item) => item.id}
        renderItem={renderMessage}
        contentContainerStyle={styles.messagesList}
        onContentSizeChange={() =>
          flatListRef.current?.scrollToEnd({ animated: true })
        }
      />

      <View style={styles.inputContainer}>
        <TextInput
          style={styles.input}
          placeholder="Type a message..."
          value={newMessage}
          onChangeText={setNewMessage}
          multiline
          maxLength={1000}
        />
        <TouchableOpacity style={styles.sendButton} onPress={handleSend}>
          <Ionicons name="send" size={24} color="#007AFF" />
        </TouchableOpacity>
      </View>
    </KeyboardAvoidingView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: "#f5f5f5",
  },
  messagesList: {
    padding: 16,
  },
  messageContainer: {
    marginBottom: 12,
    alignItems: "flex-start",
  },
  ownMessageContainer: {
    alignItems: "flex-end",
  },
  messageBubble: {
    maxWidth: "75%",
    padding: 12,
    borderRadius: 16,
    backgroundColor: "#fff",
  },
  ownMessageBubble: {
    backgroundColor: "#007AFF",
  },
  senderName: {
    fontSize: 12,
    fontWeight: "600",
    color: "#666",
    marginBottom: 4,
  },
  messageText: {
    fontSize: 15,
    color: "#000",
    marginBottom: 4,
  },
  ownMessageText: {
    color: "#fff",
  },
  messageTime: {
    fontSize: 11,
    color: "#999",
  },
  ownMessageTime: {
    color: "rgba(255, 255, 255, 0.7)",
  },
  inputContainer: {
    flexDirection: "row",
    padding: 12,
    backgroundColor: "#fff",
    borderTopWidth: 1,
    borderTopColor: "#ddd",
    alignItems: "center",
  },
  input: {
    flex: 1,
    backgroundColor: "#f5f5f5",
    borderRadius: 20,
    paddingHorizontal: 16,
    paddingVertical: 8,
    fontSize: 15,
    maxHeight: 100,
    marginRight: 12,
  },
  sendButton: {
    width: 44,
    height: 44,
    justifyContent: "center",
    alignItems: "center",
  },
});
```

**Technologies**: SignalR real-time messaging, chat UI

**Study needed**:

- 📚 SignalR with React Native (2-3 hours)
  - Resource: https://learn.microsoft.com/en-us/aspnet/core/signalr/javascript-client

## Step 12.3: Create Notifications Screen

`mobile/screens/main/NotificationsScreen.tsx`:

```typescript
import React, { useState, useEffect } from "react";
import {
  View,
  FlatList,
  StyleSheet,
  Text,
  TouchableOpacity,
  RefreshControl,
  ActivityIndicator,
} from "react-native";
import { Ionicons } from "@expo/vector-icons";
import apiService from "../../services/api";
import { Notification } from "../../types/api";

export default function NotificationsScreen() {
  const [notifications, setNotifications] = useState<Notification[]>([]);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);

  useEffect(() => {
    loadNotifications();
  }, []);

  const loadNotifications = async () => {
    try {
      const response = await apiService.getNotifications(undefined, 1, 50);
      setNotifications(response.items);
    } catch (error) {
      console.error("Failed to load notifications:", error);
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  };

  const handleRefresh = () => {
    setRefreshing(true);
    loadNotifications();
  };

  const handleMarkAsRead = async (id: string) => {
    try {
      await apiService.markNotificationAsRead(id);
      setNotifications(
        notifications.map((n) => (n.id === id ? { ...n, isRead: true } : n))
      );
    } catch (error) {
      console.error("Failed to mark as read:", error);
    }
  };

  const handleMarkAllAsRead = async () => {
    try {
      await apiService.markAllNotificationsAsRead();
      setNotifications(notifications.map((n) => ({ ...n, isRead: true })));
    } catch (error) {
      console.error("Failed to mark all as read:", error);
    }
  };

  const getNotificationIcon = (type: number) => {
    switch (type) {
      case 0:
        return "heart"; // Like
      case 1:
        return "chatbubble"; // Comment
      case 2:
        return "person-add"; // Follow
      case 3:
        return "people"; // CommunityInvite
      case 4:
        return "mail"; // Message
      default:
        return "notifications";
    }
  };

  const formatTime = (dateString: string) => {
    const date = new Date(dateString);
    const now = new Date();
    const diffMs = now.getTime() - date.getTime();
    const diffMins = Math.floor(diffMs / 60000);
    const diffHours = Math.floor(diffMs / 3600000);
    const diffDays = Math.floor(diffMs / 86400000);

    if (diffMins < 1) return "Just now";
    if (diffMins < 60) return `${diffMins}m ago`;
    if (diffHours < 24) return `${diffHours}h ago`;
    if (diffDays < 7) return `${diffDays}d ago`;
    return date.toLocaleDateString();
  };

  const renderNotification = ({ item }: { item: Notification }) => (
    <TouchableOpacity
      style={[styles.notificationCard, !item.isRead && styles.unreadCard]}
      onPress={() => handleMarkAsRead(item.id)}
    >
      <View style={[styles.iconContainer, !item.isRead && styles.unreadIcon]}>
        <Ionicons
          name={getNotificationIcon(item.type)}
          size={24}
          color={!item.isRead ? "#007AFF" : "#666"}
        />
      </View>

      <View style={styles.notificationContent}>
        <Text style={styles.notificationTitle}>{item.title}</Text>
        <Text style={styles.notificationMessage} numberOfLines={2}>
          {item.message}
        </Text>
        <Text style={styles.notificationTime}>
          {formatTime(item.createdAt)}
        </Text>
      </View>

      {!item.isRead && <View style={styles.unreadDot} />}
    </TouchableOpacity>
  );

  if (loading) {
    return (
      <View style={styles.center}>
        <ActivityIndicator size="large" color="#007AFF" />
      </View>
    );
  }

  const unreadCount = notifications.filter((n) => !n.isRead).length;

  return (
    <View style={styles.container}>
      {unreadCount > 0 && (
        <View style={styles.header}>
          <Text style={styles.headerText}>{unreadCount} unread</Text>
          <TouchableOpacity onPress={handleMarkAllAsRead}>
            <Text style={styles.markAllButton}>Mark all as read</Text>
          </TouchableOpacity>
        </View>
      )}

      <FlatList
        data={notifications}
        keyExtractor={(item) => item.id}
        renderItem={renderNotification}
        refreshControl={
          <RefreshControl refreshing={refreshing} onRefresh={handleRefresh} />
        }
        ListEmptyComponent={
          <View style={styles.empty}>
            <Ionicons name="notifications-outline" size={64} color="#ddd" />
            <Text style={styles.emptyText}>No notifications yet</Text>
          </View>
        }
      />
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: "#f5f5f5",
  },
  center: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center",
  },
  header: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
    padding: 16,
    backgroundColor: "#fff",
    borderBottomWidth: 1,
    borderBottomColor: "#f0f0f0",
  },
  headerText: {
    fontSize: 16,
    fontWeight: "600",
  },
  markAllButton: {
    color: "#007AFF",
    fontSize: 14,
  },
  notificationCard: {
    flexDirection: "row",
    backgroundColor: "#fff",
    padding: 16,
    marginBottom: 1,
    alignItems: "flex-start",
  },
  unreadCard: {
    backgroundColor: "#f0f8ff",
  },
  iconContainer: {
    width: 44,
    height: 44,
    borderRadius: 22,
    backgroundColor: "#f5f5f5",
    justifyContent: "center",
    alignItems: "center",
    marginRight: 12,
  },
  unreadIcon: {
    backgroundColor: "#e3f2fd",
  },
  notificationContent: {
    flex: 1,
  },
  notificationTitle: {
    fontSize: 15,
    fontWeight: "600",
    marginBottom: 4,
  },
  notificationMessage: {
    fontSize: 14,
    color: "#666",
    marginBottom: 4,
    lineHeight: 20,
  },
  notificationTime: {
    fontSize: 12,
    color: "#999",
  },
  unreadDot: {
    width: 8,
    height: 8,
    borderRadius: 4,
    backgroundColor: "#007AFF",
    marginLeft: 8,
    marginTop: 8,
  },
  empty: {
    padding: 60,
    alignItems: "center",
  },
  emptyText: {
    fontSize: 16,
    color: "#999",
    marginTop: 16,
  },
});
```

**Technologies**: Notification list UI, mark as read

**Study needed**: ✅ None, straightforward UI

## Step 12.4: Create Profile Screen

`mobile/screens/main/ProfileScreen.tsx`:

```typescript
import React from "react";
import {
  View,
  Text,
  StyleSheet,
  TouchableOpacity,
  ScrollView,
  Image,
  Alert,
} from "react-native";
import { Ionicons } from "@expo/vector-icons";
import { useAuth } from "../../contexts/AuthContext";

export default function ProfileScreen({ navigation }: any) {
  const { user, logout } = useAuth();

  const handleLogout = () => {
    Alert.alert("Logout", "Are you sure you want to logout?", [
      { text: "Cancel", style: "cancel" },
      {
        text: "Logout",
        style: "destructive",
        onPress: async () => {
          await logout();
        },
      },
    ]);
  };

  return (
    <ScrollView style={styles.container}>
      <View style={styles.header}>
        {user?.avatarUrl ? (
          <Image source={{ uri: user.avatarUrl }} style={styles.avatar} />
        ) : (
          <View style={[styles.avatar, styles.avatarPlaceholder]}>
            <Text style={styles.avatarText}>
              {user?.username[0].toUpperCase()}
            </Text>
          </View>
        )}

        <Text style={styles.username}>@{user?.username}</Text>
        <Text style={styles.fullName}>{user?.fullName}</Text>
        {user?.bio && <Text style={styles.bio}>{user.bio}</Text>}

        <View style={styles.stats}>
          <View style={styles.stat}>
            <Text style={styles.statValue}>{user?.followersCount || 0}</Text>
            <Text style={styles.statLabel}>Followers</Text>
          </View>
          <View style={styles.stat}>
            <Text style={styles.statValue}>{user?.followingCount || 0}</Text>
            <Text style={styles.statLabel}>Following</Text>
          </View>
        </View>

        <TouchableOpacity
          style={styles.editButton}
          onPress={() => navigation.navigate("EditProfile")}
        >
          <Text style={styles.editButtonText}>Edit Profile</Text>
        </TouchableOpacity>
      </View>

      <View style={styles.section}>
        <TouchableOpacity style={styles.menuItem}>
          <Ionicons name="settings-outline" size={24} color="#666" />
          <Text style={styles.menuText}>Settings</Text>
          <Ionicons name="chevron-forward" size={24} color="#ccc" />
        </TouchableOpacity>

        <TouchableOpacity style={styles.menuItem}>
          <Ionicons name="shield-outline" size={24} color="#666" />
          <Text style={styles.menuText}>Privacy & Safety</Text>
          <Ionicons name="chevron-forward" size={24} color="#ccc" />
        </TouchableOpacity>

        <TouchableOpacity style={styles.menuItem}>
          <Ionicons name="notifications-outline" size={24} color="#666" />
          <Text style={styles.menuText}>Notification Preferences</Text>
          <Ionicons name="chevron-forward" size={24} color="#ccc" />
        </TouchableOpacity>

        <TouchableOpacity style={styles.menuItem}>
          <Ionicons name="help-circle-outline" size={24} color="#666" />
          <Text style={styles.menuText}>Help & Support</Text>
          <Ionicons name="chevron-forward" size={24} color="#ccc" />
        </TouchableOpacity>

        <TouchableOpacity style={styles.menuItem}>
          <Ionicons name="information-circle-outline" size={24} color="#666" />
          <Text style={styles.menuText}>About</Text>
          <Ionicons name="chevron-forward" size={24} color="#ccc" />
        </TouchableOpacity>
      </View>

      <TouchableOpacity style={styles.logoutButton} onPress={handleLogout}>
        <Ionicons name="log-out-outline" size={24} color="#ff3b30" />
        <Text style={styles.logoutText}>Logout</Text>
      </TouchableOpacity>
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: "#f5f5f5",
  },
  header: {
    backgroundColor: "#fff",
    padding: 24,
    alignItems: "center",
    borderBottomWidth: 1,
    borderBottomColor: "#f0f0f0",
  },
  avatar: {
    width: 100,
    height: 100,
    borderRadius: 50,
    marginBottom: 16,
  },
  avatarPlaceholder: {
    backgroundColor: "#007AFF",
    justifyContent: "center",
    alignItems: "center",
  },
  avatarText: {
    color: "#fff",
    fontSize: 40,
    fontWeight: "bold",
  },
  username: {
    fontSize: 18,
    color: "#666",
    marginBottom: 4,
  },
  fullName: {
    fontSize: 24,
    fontWeight: "bold",
    marginBottom: 8,
  },
  bio: {
    fontSize: 14,
    color: "#666",
    textAlign: "center",
    marginBottom: 16,
  },
  stats: {
    flexDirection: "row",
    gap: 40,
    marginVertical: 16,
  },
  stat: {
    alignItems: "center",
  },
  statValue: {
    fontSize: 20,
    fontWeight: "bold",
  },
  statLabel: {
    fontSize: 14,
    color: "#666",
  },
  editButton: {
    backgroundColor: "#007AFF",
    paddingVertical: 10,
    paddingHorizontal: 32,
    borderRadius: 8,
    marginTop: 8,
  },
  editButtonText: {
    color: "#fff",
    fontSize: 16,
    fontWeight: "600",
  },
  section: {
    backgroundColor: "#fff",
    marginTop: 16,
  },
  menuItem: {
    flexDirection: "row",
    alignItems: "center",
    padding: 16,
    borderBottomWidth: 1,
    borderBottomColor: "#f0f0f0",
  },
  menuText: {
    flex: 1,
    fontSize: 16,
    marginLeft: 16,
  },
  logoutButton: {
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "center",
    backgroundColor: "#fff",
    padding: 16,
    marginTop: 16,
    marginBottom: 32,
  },
  logoutText: {
    fontSize: 16,
    color: "#ff3b30",
    marginLeft: 8,
    fontWeight: "600",
  },
});
```

**Technologies**: Profile UI, settings menu

**Study needed**: ✅ None

## Step 12.5: Create Post Creation Screen

`mobile/screens/create/CreatePostScreen.tsx`:

```typescript
import React, { useState } from "react";
import {
  View,
  Text,
  TextInput,
  TouchableOpacity,
  StyleSheet,
  Alert,
  ActivityIndicator,
  ScrollView,
  Image,
} from "react-native";
import { Ionicons } from "@expo/vector-icons";
import * as ImagePicker from "expo-image-picker";
import apiService from "../../services/api";
import { PostType, PostVisibility } from "../../types/api";

export default function CreatePostScreen({ navigation }: any) {
  const [content, setContent] = useState("");
  const [selectedImage, setSelectedImage] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const [visibility, setVisibility] = useState(PostVisibility.Public);

  const handlePickImage = async () => {
    const { status } = await ImagePicker.requestMediaLibraryPermissionsAsync();
    if (status !== "granted") {
      Alert.alert("Permission needed", "Please grant camera roll permission");
      return;
    }

    const result = await ImagePicker.launchImageLibraryAsync({
      mediaTypes: ImagePicker.MediaTypeOptions.Images,
      allowsEditing: true,
      aspect: [4, 3],
      quality: 0.8,
    });

    if (!result.canceled) {
      setSelectedImage(result.assets[0].uri);
    }
  };

  const handlePost = async () => {
    if (!content.trim() && !selectedImage) {
      Alert.alert("Error", "Please add some content or an image");
      return;
    }

    setLoading(true);
    try {
      // For now, just create text post (media upload would require additional backend setup)
      const postData = {
        textContent: content.trim(),
        type: selectedImage ? PostType.Photo : PostType.Text,
        visibility: visibility,
      };

      await apiService.createPost(postData);
      Alert.alert("Success", "Post created!");
      navigation.goBack();
    } catch (error: any) {
      Alert.alert(
        "Error",
        error.response?.data?.error || "Failed to create post"
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <View style={styles.container}>
      <View style={styles.header}>
        <TouchableOpacity onPress={() => navigation.goBack()}>
          <Text style={styles.cancelButton}>Cancel</Text>
        </TouchableOpacity>
        <TouchableOpacity
          style={styles.postButton}
          onPress={handlePost}
          disabled={loading}
        >
          {loading ? (
            <ActivityIndicator color="#fff" />
          ) : (
            <Text style={styles.postButtonText}>Post</Text>
          )}
        </TouchableOpacity>
      </View>

      <ScrollView style={styles.content}>
        <TextInput
          style={styles.textInput}
          placeholder="What's on your mind?"
          value={content}
          onChangeText={setContent}
          multiline
          maxLength={5000}
          autoFocus
        />

        {selectedImage && (
          <View style={styles.imageContainer}>
            <Image
              source={{ uri: selectedImage }}
              style={styles.selectedImage}
            />
            <TouchableOpacity
              style={styles.removeImage}
              onPress={() => setSelectedImage(null)}
            >
              <Ionicons name="close-circle" size={32} color="#fff" />
            </TouchableOpacity>
          </View>
        )}

        <View style={styles.visibilitySection}>
          <Text style={styles.sectionTitle}>Who can see this?</Text>
          <TouchableOpacity
            style={[
              styles.visibilityOption,
              visibility === PostVisibility.Public && styles.selectedOption,
            ]}
            onPress={() => setVisibility(PostVisibility.Public)}
          >
            <Ionicons
              name="globe-outline"
              size={24}
              color={visibility === PostVisibility.Public ? "#007AFF" : "#666"}
            />
            <Text style={styles.optionText}>Public</Text>
          </TouchableOpacity>
          <TouchableOpacity
            style={[
              styles.visibilityOption,
              visibility === PostVisibility.FollowersOnly &&
                styles.selectedOption,
            ]}
            onPress={() => setVisibility(PostVisibility.FollowersOnly)}
          >
            <Ionicons
              name="people-outline"
              size={24}
              color={
                visibility === PostVisibility.FollowersOnly ? "#007AFF" : "#666"
              }
            />
            <Text style={styles.optionText}>Followers Only</Text>
          </TouchableOpacity>
        </View>
      </ScrollView>

      <View style={styles.toolbar}>
        <TouchableOpacity
          style={styles.toolbarButton}
          onPress={handlePickImage}
        >
          <Ionicons name="image-outline" size={28} color="#007AFF" />
        </TouchableOpacity>
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: "#fff",
  },
  header: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
    padding: 16,
    borderBottomWidth: 1,
    borderBottomColor: "#f0f0f0",
  },
  cancelButton: {
    fontSize: 16,
    color: "#666",
  },
  postButton: {
    backgroundColor: "#007AFF",
    paddingVertical: 8,
    paddingHorizontal: 20,
    borderRadius: 20,
    minWidth: 70,
    alignItems: "center",
  },
  postButtonText: {
    color: "#fff",
    fontSize: 16,
    fontWeight: "600",
  },
  content: {
    flex: 1,
    padding: 16,
  },
  textInput: {
    fontSize: 18,
    minHeight: 150,
    textAlignVertical: "top",
  },
  imageContainer: {
    position: "relative",
    marginTop: 16,
  },
  selectedImage: {
    width: "100%",
    height: 250,
    borderRadius: 12,
  },
  removeImage: {
    position: "absolute",
    top: 8,
    right: 8,
    backgroundColor: "rgba(0, 0, 0, 0.5)",
    borderRadius: 16,
  },
  visibilitySection: {
    marginTop: 24,
  },
  sectionTitle: {
    fontSize: 16,
    fontWeight: "600",
    marginBottom: 12,
  },
  visibilityOption: {
    flexDirection: "row",
    alignItems: "center",
    padding: 12,
    borderRadius: 8,
    borderWidth: 1,
    borderColor: "#ddd",
    marginBottom: 8,
  },
  selectedOption: {
    borderColor: "#007AFF",
    backgroundColor: "#f0f8ff",
  },
  optionText: {
    fontSize: 16,
    marginLeft: 12,
  },
  toolbar: {
    flexDirection: "row",
    padding: 16,
    borderTopWidth: 1,
    borderTopColor: "#f0f0f0",
  },
  toolbarButton: {
    padding: 8,
  },
});
```

**Technologies**: Image picker, post creation UI

**Study needed**:

- 📚 Expo Image Picker (1 hour)
  - Resource: https://docs.expo.dev/versions/latest/sdk/imagepicker/

## Step 12.6: Update Navigation with Create Post Button

Update `AppNavigator.tsx` to add a floating action button for creating posts:

```typescript
function MainTabs() {
  return (
    <Tab.Navigator
      screenOptions={({ route }) => ({
        tabBarIcon: ({ focused, color, size }) => {
          let iconName: any;

          if (route.name === "Home") {
            iconName = focused ? "home" : "home-outline";
          } else if (route.name === "Communities") {
            iconName = focused ? "people" : "people-outline";
          } else if (route.name === "Create") {
            iconName = "add-circle";
            size = 48; // Larger icon for create button
          } else if (route.name === "Messages") {
            iconName = focused ? "chatbubbles" : "chatbubbles-outline";
          } else if (route.name === "Profile") {
            iconName = focused ? "person" : "person-outline";
          }

          return <Ionicons name={iconName} size={size} color={color} />;
        },
        tabBarActiveTintColor: "#007AFF",
        tabBarInactiveTintColor: "gray",
        tabBarShowLabel: route.name !== "Create",
      })}
    >
      <Tab.Screen name="Home" component={HomeScreen} />
      <Tab.Screen name="Communities" component={CommunitiesScreen} />
      <Tab.Screen
        name="Create"
        component={CreatePostScreen}
        options={{
          tabBarButton: (props) => (
            <TouchableOpacity
              {...props}
              style={{
                top: -10,
                justifyContent: "center",
                alignItems: "center",
              }}
            />
          ),
        }}
      />
      <Tab.Screen name="Messages" component={MessagesScreen} />
      <Tab.Screen name="Profile" component={ProfileScreen} />
    </Tab.Navigator>
  );
}
```

**Technologies**: Custom tab bar button

**Study needed**: ✅ None

## Step 12.7: Update Root App.tsx

Update the main `App.tsx` file:

```typescript
import React from "react";
import { StatusBar } from "expo-status-bar";
import { SafeAreaProvider } from "react-native-safe-area-context";
import { AuthProvider } from "./contexts/AuthContext";
import AppNavigator from "./navigation/AppNavigator";

export default function App() {
  return (
    <SafeAreaProvider>
      <AuthProvider>
        <AppNavigator />
        <StatusBar style="auto" />
      </AuthProvider>
    </SafeAreaProvider>
  );
}
```

**Technologies**: React Native app structure

**Study needed**: ✅ None

---

**🔄 CHECKPOINT - Mobile App Advanced Features Complete!**

You now have:

- ✅ Communities screen with join functionality
- ✅ Real-time messaging with SignalR
- ✅ Messages list and chat screen
- ✅ Notifications screen with mark as read
- ✅ Profile screen with logout
- ✅ Create post screen with image picker
- ✅ Custom tab bar with create button
- ✅ Complete navigation flow

**Next steps to enhance the app:**

1. **Push Notifications**: Set up Expo push notifications
2. **Image Upload to MinIO**: Implement media upload to backend
3. **Comment Threads**: Add comment viewing/posting
4. **Search**: Add user/post/community search
5. **Performance**: Optimize with React.memo, useMemo, useCallback
6. **Offline Support**: Add offline caching with AsyncStorage
7. **Dark Mode**: Implement dark theme
8. **Animations**: Add smooth transitions with Reanimated

**Testing the app:**

```bash
# Start the backend API
cd backend/Whispra.Api
dotnet run

# Start MongoDB, Redis, MinIO
docker compose up -d

# Start the mobile app
cd apps/mobile
npm start

# Then:
# - Press 'i' for iOS simulator
# - Press 'a' for Android emulator
# - Scan QR code with Expo Go on your phone
```

**Important for physical device testing:**

Update `API_BASE_URL` in `mobile/services/api.ts`:

```typescript
// Find your computer's IP address:
// Windows: ipconfig
// Mac: ifconfig | grep "inet "
// Linux: ip addr show

const API_BASE_URL = "http://192.168.1.XXX:5000/api"; // Replace with your IP
```

**Study needed**: ✅ Just testing and refinement

**Common Issues & Solutions:**

1. **SignalR connection fails**: Check backend CORS settings, verify hub URL
2. **Images not loading**: Check API_BASE_URL, verify MinIO is accessible
3. **Token refresh fails**: Check JWT configuration, verify refresh token endpoint
4. **Slow performance**: Use React DevTools Profiler, optimize re-renders

Before moving to Phase 13, confirm:

1. ✅ Can you login and register?
2. ✅ Do posts load in the feed?
3. ✅ Does real-time messaging work?
4. ✅ Can you create posts?
5. ✅ Do notifications display?
6. ✅ Can you join communities?
7. ✅ Does the profile screen load user data?

**Next Phase Preview**: We'll build the Tauri desktop app with React+Vite, then move to Deployment & Production (Docker, cloud hosting, monitoring, CI/CD)!

---

# PHASE 13: Desktop App - Tauri

**Goal**: Build a cross-platform desktop application (Windows, macOS, Linux) using Tauri with React and Vite that connects to the backend API, providing a native desktop experience with system integration.

## Step 13.1: Understanding Tauri

**What is Tauri?**

- **Lightweight desktop framework**: Build desktop apps with web technologies (HTML, CSS, JavaScript)
- **Small bundle size**: Apps are ~3-5 MB (vs 100+ MB with Electron)
- **Native performance**: Uses system webview instead of bundling Chromium
- **Rust backend**: Secure, fast backend with access to OS APIs
- **Cross-platform**: Single codebase for Windows, macOS, Linux
- **Security-first**: Strong security model, permissions-based API access

**Tauri vs Electron**:

| Feature      | Tauri            | Electron         |
| ------------ | ---------------- | ---------------- |
| Bundle Size  | ~3-5 MB          | ~100-200 MB      |
| Memory Usage | ~50-100 MB       | ~200-500 MB      |
| Backend      | Rust             | Node.js          |
| Webview      | System           | Bundled Chromium |
| Security     | High (sandboxed) | Medium           |
| Startup Time | Fast             | Slower           |

**App Architecture**:

```
Desktop App (Tauri)
├── Frontend (React + Vite)
│   ├── UI Components
│   ├── API Client (Axios)
│   └── State Management
└── Backend (Rust)
    ├── System APIs
    ├── File System
    └── Window Management
        ↓
Backend API (ASP.NET Core)
```

📚 **Study needed**: Tauri fundamentals (3-4 hours)

- Resource: https://tauri.app/v1/guides/
- Resource: https://www.youtube.com/watch?v=kRoGYgAuZQE (Tauri Crash Course)
- Resource: https://tauri.app/v1/guides/getting-started/prerequisites

## Step 13.2: Set Up Tauri Project

**Prerequisites**:

```bash
# Install Rust (required for Tauri)
# Windows: Download from https://www.rust-lang.org/tools/install
# macOS/Linux:
curl --proto '=https' --tlsv1.2 -sSf https://sh.rustup.rs | sh

# Verify Rust installation
rustc --version
cargo --version

# Windows-specific: Install Visual Studio C++ Build Tools
# Download from: https://visualstudio.microsoft.com/visual-cpp-build-tools/

# macOS-specific: Install Xcode Command Line Tools
xcode-select --install

# Linux-specific: Install dependencies
sudo apt update
sudo apt install libwebkit2gtk-4.0-dev \
    build-essential \
    curl \
    wget \
    libssl-dev \
    libgtk-3-dev \
    libayatana-appindicator3-dev \
    librsvg2-dev
```

**Create the project**:

```bash
# Navigate to apps folder
cd apps

# Create Vite + React + TypeScript project
npm create vite@latest desktop -- --template react-ts

# Navigate into the project
cd desktop

# Install dependencies
npm install

# Install Tauri CLI
npm install -D @tauri-apps/cli

# Initialize Tauri
npm run tauri init

# When prompted:
# - App name: Whispra
# - Window title: Whispra
# - Web assets location: ../dist
# - Dev server URL: http://localhost:5173
# - Frontend dev command: npm run dev
# - Frontend build command: npm run build
```

**Install additional dependencies**:

```bash
# Tauri API for frontend
npm install @tauri-apps/api

# React Router
npm install react-router-dom

# Axios for API calls
npm install axios

# State management
npm install zustand

# UI components
npm install @headlessui/react @heroicons/react

# SignalR
npm install @microsoft/signalr

# Tailwind CSS for styling
npm install -D tailwindcss postcss autoprefixer
npx tailwindcss init -p
```

**Project structure**:

```
desktop/
├── src/                    # React frontend
│   ├── components/         # UI components
│   ├── pages/              # Page components
│   ├── services/           # API services
│   ├── stores/             # Zustand stores
│   ├── types/              # TypeScript types
│   ├── App.tsx             # Root component
│   └── main.tsx            # Entry point
├── src-tauri/              # Rust backend
│   ├── src/
│   │   └── main.rs         # Tauri main file
│   ├── Cargo.toml          # Rust dependencies
│   ├── tauri.conf.json     # Tauri configuration
│   └── icons/              # App icons
├── index.html
├── vite.config.ts
├── tailwind.config.js
└── package.json
```

**Technologies**: Tauri, Rust, React, Vite, TypeScript, Tailwind CSS

**Study needed**:

- 📚 Vite basics (1-2 hours)
  - Resource: https://vitejs.dev/guide/
- 📚 Tailwind CSS (2-3 hours)
  - Resource: https://tailwindcss.com/docs/installation

## Step 13.3: Configure Tailwind CSS

Update `tailwind.config.js`:

```javascript
/** @type {import('tailwindcss').Config} */
export default {
  content: ["./index.html", "./src/**/*.{js,ts,jsx,tsx}"],
  theme: {
    extend: {
      colors: {
        primary: "#007AFF",
        secondary: "#5856D6",
        success: "#34C759",
        warning: "#FF9500",
        error: "#FF3B30",
      },
    },
  },
  plugins: [],
};
```

Update `src/index.css`:

```css
@tailwind base;
@tailwind components;
@tailwind utilities;

* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

body {
  font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", "Roboto",
    "Oxygen", "Ubuntu", "Cantarell", "Fira Sans", "Droid Sans",
    "Helvetica Neue", sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
}

code {
  font-family: source-code-pro, Menlo, Monaco, Consolas, "Courier New",
    monospace;
}

/* Custom scrollbar */
::-webkit-scrollbar {
  width: 8px;
  height: 8px;
}

::-webkit-scrollbar-track {
  background: #f1f1f1;
}

::-webkit-scrollbar-thumb {
  background: #888;
  border-radius: 4px;
}

::-webkit-scrollbar-thumb:hover {
  background: #555;
}
```

**Technologies**: Tailwind CSS utility-first styling

**Study needed**: ✅ None, standard configuration

## Step 13.4: Create TypeScript Types

Create shared types (similar to mobile app).

`src/types/api.ts`:

```typescript
// User types
export interface User {
  id: string;
  username: string;
  email: string;
  fullName: string;
  bio?: string;
  avatarUrl?: string;
  followersCount: number;
  followingCount: number;
  createdAt: string;
}

// Auth types
export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  user: User;
}

// Post types
export enum PostType {
  Text = 0,
  Photo = 1,
  Video = 2,
}

export enum PostVisibility {
  Public = 0,
  FollowersOnly = 1,
  CommunityOnly = 2,
  Private = 3,
}

export interface Post {
  id: string;
  authorId: string;
  authorUsername: string;
  authorAvatarUrl?: string;
  textContent?: string;
  mediaUrls: string[];
  type: PostType;
  visibility: PostVisibility;
  communityId?: string;
  communityName?: string;
  commentCount: number;
  reactionCounts: Record<string, number>;
  currentUserReaction?: string;
  createdAt: string;
}

// Community types
export interface Community {
  id: string;
  name: string;
  description: string;
  privacy: number;
  memberCount: number;
  currentUserRole?: number;
  coverImageUrl?: string;
  createdAt: string;
}

// Message types
export interface Message {
  id: string;
  conversationId: string;
  senderId: string;
  senderUsername: string;
  content: string;
  status: number;
  createdAt: string;
}

export interface Conversation {
  id: string;
  type: number;
  participants: User[];
  lastMessage?: Message;
  unreadCount: number;
  updatedAt: string;
}

// Notification types
export interface Notification {
  id: string;
  type: number;
  title: string;
  message: string;
  isRead: boolean;
  relatedEntityId?: string;
  relatedEntityType?: string;
  createdAt: string;
}

// Pagination
export interface PaginatedResponse<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  hasMore: boolean;
}
```

**Technologies**: TypeScript interfaces

**Study needed**: ✅ None

## Step 13.5: Create API Service with Zustand Store

**API Service**:

`src/services/api.ts`:

```typescript
import axios, { AxiosInstance } from "axios";

const API_BASE_URL = "http://localhost:5000/api";

class ApiService {
  private client: AxiosInstance;

  constructor() {
    this.client = axios.create({
      baseURL: API_BASE_URL,
      headers: {
        "Content-Type": "application/json",
      },
      timeout: 10000,
    });

    // Request interceptor
    this.client.interceptors.request.use(
      async (config) => {
        const token = localStorage.getItem("accessToken");
        if (token) {
          config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
      },
      (error) => Promise.reject(error)
    );

    // Response interceptor
    this.client.interceptors.response.use(
      (response) => response,
      async (error) => {
        const originalRequest = error.config;

        if (error.response?.status === 401 && !originalRequest._retry) {
          originalRequest._retry = true;

          try {
            const refreshToken = localStorage.getItem("refreshToken");
            if (refreshToken) {
              const response = await axios.post(
                `${API_BASE_URL}/Auth/refresh`,
                {
                  refreshToken,
                }
              );

              const { accessToken, refreshToken: newRefreshToken } =
                response.data;
              localStorage.setItem("accessToken", accessToken);
              localStorage.setItem("refreshToken", newRefreshToken);

              originalRequest.headers.Authorization = `Bearer ${accessToken}`;
              return this.client(originalRequest);
            }
          } catch (refreshError) {
            localStorage.removeItem("accessToken");
            localStorage.removeItem("refreshToken");
            window.location.href = "/login";
            return Promise.reject(refreshError);
          }
        }

        return Promise.reject(error);
      }
    );
  }

  // Auth
  async login(email: string, password: string) {
    const response = await this.client.post("/Auth/login", { email, password });
    return response.data;
  }

  async register(data: {
    username: string;
    email: string;
    fullName: string;
    password: string;
  }) {
    const response = await this.client.post("/Users/register", data);
    return response.data;
  }

  async getCurrentUser() {
    const response = await this.client.get("/Auth/me");
    return response.data;
  }

  // Posts
  async getPosts(page = 1, pageSize = 20) {
    const response = await this.client.get("/Posts/feed", {
      params: { page, pageSize },
    });
    return response.data;
  }

  async createPost(data: any) {
    const response = await this.client.post("/Posts", data);
    return response.data;
  }

  async likePost(postId: string) {
    const response = await this.client.post(`/Posts/${postId}/reactions`, {
      type: 0,
    });
    return response.data;
  }

  async getComments(postId: string, page = 1, pageSize = 20) {
    const response = await this.client.get(`/Posts/${postId}/comments`, {
      params: { page, pageSize },
    });
    return response.data;
  }

  async addComment(postId: string, content: string) {
    const response = await this.client.post(`/Posts/${postId}/comments`, {
      content,
    });
    return response.data;
  }

  // Communities
  async getCommunities(page = 1, pageSize = 20) {
    const response = await this.client.get("/Communities", {
      params: { page, pageSize },
    });
    return response.data;
  }

  async joinCommunity(id: string, inviteCode?: string) {
    const response = await this.client.post(`/Communities/${id}/join`, {
      inviteCode,
    });
    return response.data;
  }

  // Messages
  async getConversations(page = 1, pageSize = 20) {
    const response = await this.client.get("/Messages/conversations", {
      params: { page, pageSize },
    });
    return response.data;
  }

  async getMessages(conversationId: string, page = 1, pageSize = 50) {
    const response = await this.client.get(
      `/Messages/conversations/${conversationId}/messages`,
      {
        params: { page, pageSize },
      }
    );
    return response.data;
  }

  // Notifications
  async getNotifications(isRead?: boolean, page = 1, pageSize = 20) {
    const response = await this.client.get("/Notifications", {
      params: { isRead, page, pageSize },
    });
    return response.data;
  }

  async markNotificationAsRead(id: string) {
    const response = await this.client.put(`/Notifications/${id}/read`);
    return response.data;
  }

  async markAllNotificationsAsRead() {
    const response = await this.client.put("/Notifications/read-all");
    return response.data;
  }
}

export default new ApiService();
```

**Auth Store with Zustand**:

`src/stores/authStore.ts`:

```typescript
import { create } from "zustand";
import apiService from "../services/api";
import { User, AuthResponse } from "../types/api";

interface AuthState {
  user: User | null;
  isAuthenticated: boolean;
  loading: boolean;
  login: (email: string, password: string) => Promise<void>;
  register: (
    username: string,
    email: string,
    fullName: string,
    password: string
  ) => Promise<void>;
  logout: () => void;
  loadUser: () => Promise<void>;
}

export const useAuthStore = create<AuthState>((set) => ({
  user: null,
  isAuthenticated: false,
  loading: true,

  login: async (email: string, password: string) => {
    const response: AuthResponse = await apiService.login(email, password);
    localStorage.setItem("accessToken", response.accessToken);
    localStorage.setItem("refreshToken", response.refreshToken);
    set({ user: response.user, isAuthenticated: true });
  },

  register: async (
    username: string,
    email: string,
    fullName: string,
    password: string
  ) => {
    await apiService.register({ username, email, fullName, password });
    // Auto-login after registration
    const response: AuthResponse = await apiService.login(email, password);
    localStorage.setItem("accessToken", response.accessToken);
    localStorage.setItem("refreshToken", response.refreshToken);
    set({ user: response.user, isAuthenticated: true });
  },

  logout: () => {
    localStorage.removeItem("accessToken");
    localStorage.removeItem("refreshToken");
    set({ user: null, isAuthenticated: false });
  },

  loadUser: async () => {
    try {
      const token = localStorage.getItem("accessToken");
      if (token) {
        const user = await apiService.getCurrentUser();
        set({ user, isAuthenticated: true, loading: false });
      } else {
        set({ loading: false });
      }
    } catch (error) {
      console.error("Failed to load user:", error);
      localStorage.removeItem("accessToken");
      localStorage.removeItem("refreshToken");
      set({ loading: false });
    }
  },
}));
```

**Technologies**: Axios, Zustand state management, localStorage

**Study needed**:

- 📚 Zustand state management (1-2 hours)
  - Resource: https://github.com/pmndrs/zustand

## Step 13.6: Create Authentication Pages

**Login Page**:

`src/pages/LoginPage.tsx`:

```typescript
import { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import { useAuthStore } from "../stores/authStore";

export default function LoginPage() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const login = useAuthStore((state) => state.login);
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    setLoading(true);

    try {
      await login(email, password);
      navigate("/");
    } catch (err: any) {
      setError(err.response?.data?.error || "Invalid email or password");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50">
      <div className="max-w-md w-full space-y-8 p-8 bg-white rounded-lg shadow-lg">
        <div>
          <h2 className="text-center text-3xl font-extrabold text-gray-900">
            Welcome to Whispra
          </h2>
          <p className="mt-2 text-center text-sm text-gray-600">
            Sign in to continue
          </p>
        </div>

        <form className="mt-8 space-y-6" onSubmit={handleSubmit}>
          {error && (
            <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded">
              {error}
            </div>
          )}

          <div className="space-y-4">
            <div>
              <label
                htmlFor="email"
                className="block text-sm font-medium text-gray-700"
              >
                Email
              </label>
              <input
                id="email"
                type="email"
                required
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-primary focus:border-primary"
                disabled={loading}
              />
            </div>

            <div>
              <label
                htmlFor="password"
                className="block text-sm font-medium text-gray-700"
              >
                Password
              </label>
              <input
                id="password"
                type="password"
                required
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-primary focus:border-primary"
                disabled={loading}
              />
            </div>
          </div>

          <button
            type="submit"
            disabled={loading}
            className="w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-primary hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary disabled:opacity-50"
          >
            {loading ? "Signing in..." : "Sign in"}
          </button>

          <p className="text-center text-sm text-gray-600">
            Don't have an account?{" "}
            <Link
              to="/register"
              className="font-medium text-primary hover:text-blue-700"
            >
              Sign up
            </Link>
          </p>
        </form>
      </div>
    </div>
  );
}
```

**Register Page**:

`src/pages/RegisterPage.tsx`:

```typescript
import { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import { useAuthStore } from "../stores/authStore";

export default function RegisterPage() {
  const [username, setUsername] = useState("");
  const [email, setEmail] = useState("");
  const [fullName, setFullName] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const register = useAuthStore((state) => state.register);
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");

    if (password !== confirmPassword) {
      setError("Passwords do not match");
      return;
    }

    if (password.length < 6) {
      setError("Password must be at least 6 characters");
      return;
    }

    setLoading(true);

    try {
      await register(username, email, fullName, password);
      navigate("/");
    } catch (err: any) {
      setError(
        err.response?.data?.error || "Registration failed. Please try again."
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50">
      <div className="max-w-md w-full space-y-8 p-8 bg-white rounded-lg shadow-lg">
        <div>
          <h2 className="text-center text-3xl font-extrabold text-gray-900">
            Create Account
          </h2>
          <p className="mt-2 text-center text-sm text-gray-600">
            Join Whispra today
          </p>
        </div>

        <form className="mt-8 space-y-6" onSubmit={handleSubmit}>
          {error && (
            <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded">
              {error}
            </div>
          )}

          <div className="space-y-4">
            <div>
              <label
                htmlFor="username"
                className="block text-sm font-medium text-gray-700"
              >
                Username
              </label>
              <input
                id="username"
                type="text"
                required
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-primary focus:border-primary"
                disabled={loading}
              />
            </div>

            <div>
              <label
                htmlFor="fullName"
                className="block text-sm font-medium text-gray-700"
              >
                Full Name
              </label>
              <input
                id="fullName"
                type="text"
                required
                value={fullName}
                onChange={(e) => setFullName(e.target.value)}
                className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-primary focus:border-primary"
                disabled={loading}
              />
            </div>

            <div>
              <label
                htmlFor="email"
                className="block text-sm font-medium text-gray-700"
              >
                Email
              </label>
              <input
                id="email"
                type="email"
                required
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-primary focus:border-primary"
                disabled={loading}
              />
            </div>

            <div>
              <label
                htmlFor="password"
                className="block text-sm font-medium text-gray-700"
              >
                Password
              </label>
              <input
                id="password"
                type="password"
                required
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-primary focus:border-primary"
                disabled={loading}
              />
            </div>

            <div>
              <label
                htmlFor="confirmPassword"
                className="block text-sm font-medium text-gray-700"
              >
                Confirm Password
              </label>
              <input
                id="confirmPassword"
                type="password"
                required
                value={confirmPassword}
                onChange={(e) => setConfirmPassword(e.target.value)}
                className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-primary focus:border-primary"
                disabled={loading}
              />
            </div>
          </div>

          <button
            type="submit"
            disabled={loading}
            className="w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-primary hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary disabled:opacity-50"
          >
            {loading ? "Creating account..." : "Sign up"}
          </button>

          <p className="text-center text-sm text-gray-600">
            Already have an account?{" "}
            <Link
              to="/login"
              className="font-medium text-primary hover:text-blue-700"
            >
              Sign in
            </Link>
          </p>
        </form>
      </div>
    </div>
  );
}
```

**Technologies**: React forms, Tailwind CSS, React Router

**Study needed**: ✅ None, straightforward forms

## Step 13.7: Create Layout and Navigation

**Main Layout**:

`src/components/Layout.tsx`:

```typescript
import { ReactNode } from "react";
import { Link, useLocation } from "react-router-dom";
import {
  HomeIcon,
  UserGroupIcon,
  ChatBubbleLeftRightIcon,
  BellIcon,
  UserCircleIcon,
} from "@heroicons/react/24/outline";
import {
  HomeIcon as HomeIconSolid,
  UserGroupIcon as UserGroupIconSolid,
  ChatBubbleLeftRightIcon as ChatIconSolid,
  BellIcon as BellIconSolid,
  UserCircleIcon as UserIconSolid,
} from "@heroicons/react/24/solid";

interface LayoutProps {
  children: ReactNode;
}

export default function Layout({ children }: LayoutProps) {
  const location = useLocation();

  const navItems = [
    { path: "/", icon: HomeIcon, iconSolid: HomeIconSolid, label: "Home" },
    {
      path: "/communities",
      icon: UserGroupIcon,
      iconSolid: UserGroupIconSolid,
      label: "Communities",
    },
    {
      path: "/messages",
      icon: ChatBubbleLeftRightIcon,
      iconSolid: ChatIconSolid,
      label: "Messages",
    },
    {
      path: "/notifications",
      icon: BellIcon,
      iconSolid: BellIconSolid,
      label: "Notifications",
    },
    {
      path: "/profile",
      icon: UserCircleIcon,
      iconSolid: UserIconSolid,
      label: "Profile",
    },
  ];

  return (
    <div className="flex h-screen bg-gray-50">
      {/* Sidebar */}
      <aside className="w-64 bg-white border-r border-gray-200">
        <div className="p-6">
          <h1 className="text-2xl font-bold text-primary">Whispra</h1>
        </div>

        <nav className="px-3">
          {navItems.map((item) => {
            const isActive = location.pathname === item.path;
            const Icon = isActive ? item.iconSolid : item.icon;

            return (
              <Link
                key={item.path}
                to={item.path}
                className={`flex items-center gap-3 px-4 py-3 rounded-lg mb-1 transition-colors ${
                  isActive
                    ? "bg-blue-50 text-primary font-semibold"
                    : "text-gray-700 hover:bg-gray-100"
                }`}
              >
                <Icon className="w-6 h-6" />
                <span>{item.label}</span>
              </Link>
            );
          })}
        </nav>
      </aside>

      {/* Main content */}
      <main className="flex-1 overflow-hidden">{children}</main>
    </div>
  );
}
```

**Technologies**: React Router, Heroicons, responsive layout

**Study needed**: ✅ None

## Step 13.8: Create Home Feed Page

`src/pages/HomePage.tsx`:

```typescript
import { useState, useEffect } from "react";
import apiService from "../services/api";
import { Post } from "../types/api";
import PostCard from "../components/PostCard";

export default function HomePage() {
  const [posts, setPosts] = useState<Post[]>([]);
  const [loading, setLoading] = useState(true);
  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);

  useEffect(() => {
    loadPosts();
  }, []);

  const loadPosts = async (pageNum = 1) => {
    try {
      const response = await apiService.getPosts(pageNum, 20);
      if (pageNum === 1) {
        setPosts(response.items);
      } else {
        setPosts([...posts, ...response.items]);
      }
      setHasMore(response.hasMore);
      setPage(pageNum);
    } catch (error) {
      console.error("Failed to load posts:", error);
    } finally {
      setLoading(false);
    }
  };

  const handleLike = async (postId: string) => {
    try {
      await apiService.likePost(postId);
      setPosts(
        posts.map((p) =>
          p.id === postId ? { ...p, currentUserReaction: "Like" } : p
        )
      );
    } catch (error) {
      console.error("Failed to like post:", error);
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-full">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary"></div>
      </div>
    );
  }

  return (
    <div className="h-full overflow-y-auto">
      <div className="max-w-2xl mx-auto py-6">
        <div className="mb-6 bg-white rounded-lg shadow p-4">
          <button className="w-full text-left px-4 py-3 bg-gray-50 rounded-lg text-gray-500 hover:bg-gray-100 transition-colors">
            What's on your mind?
          </button>
        </div>

        <div className="space-y-4">
          {posts.map((post) => (
            <PostCard
              key={post.id}
              post={post}
              onLike={() => handleLike(post.id)}
            />
          ))}

          {hasMore && (
            <button
              onClick={() => loadPosts(page + 1)}
              className="w-full py-3 text-primary hover:bg-gray-50 rounded-lg transition-colors"
            >
              Load more
            </button>
          )}
        </div>
      </div>
    </div>
  );
}
```

**Post Card Component**:

`src/components/PostCard.tsx`:

```typescript
import {
  HeartIcon,
  ChatBubbleLeftIcon,
  ShareIcon,
} from "@heroicons/react/24/outline";
import { HeartIcon as HeartIconSolid } from "@heroicons/react/24/solid";
import { Post } from "../types/api";

interface PostCardProps {
  post: Post;
  onLike: () => void;
}

export default function PostCard({ post, onLike }: PostCardProps) {
  const formatTime = (dateString: string) => {
    const date = new Date(dateString);
    const now = new Date();
    const diffMs = now.getTime() - date.getTime();
    const diffMins = Math.floor(diffMs / 60000);
    const diffHours = Math.floor(diffMs / 3600000);
    const diffDays = Math.floor(diffMs / 86400000);

    if (diffMins < 1) return "Just now";
    if (diffMins < 60) return `${diffMins}m ago`;
    if (diffHours < 24) return `${diffHours}h ago`;
    if (diffDays < 7) return `${diffDays}d ago`;
    return date.toLocaleDateString();
  };

  const isLiked = post.currentUserReaction === "Like";

  return (
    <div className="bg-white rounded-lg shadow p-4">
      {/* Author info */}
      <div className="flex items-center gap-3 mb-4">
        <div className="w-10 h-10 rounded-full bg-primary text-white flex items-center justify-center font-semibold">
          {post.authorUsername[0].toUpperCase()}
        </div>
        <div>
          <p className="font-semibold text-gray-900">{post.authorUsername}</p>
          <p className="text-sm text-gray-500">{formatTime(post.createdAt)}</p>
        </div>
      </div>

      {/* Content */}
      {post.textContent && (
        <p className="text-gray-800 mb-4 whitespace-pre-wrap">
          {post.textContent}
        </p>
      )}

      {/* Media */}
      {post.mediaUrls.length > 0 && (
        <img
          src={post.mediaUrls[0]}
          alt="Post media"
          className="w-full rounded-lg mb-4 max-h-96 object-cover"
        />
      )}

      {/* Actions */}
      <div className="flex items-center gap-6 pt-3 border-t border-gray-100">
        <button
          onClick={onLike}
          className="flex items-center gap-2 text-gray-600 hover:text-red-500 transition-colors"
        >
          {isLiked ? (
            <HeartIconSolid className="w-5 h-5 text-red-500" />
          ) : (
            <HeartIcon className="w-5 h-5" />
          )}
          <span className="text-sm">{post.reactionCounts?.Like || 0}</span>
        </button>

        <button className="flex items-center gap-2 text-gray-600 hover:text-primary transition-colors">
          <ChatBubbleLeftIcon className="w-5 h-5" />
          <span className="text-sm">{post.commentCount}</span>
        </button>

        <button className="flex items-center gap-2 text-gray-600 hover:text-primary transition-colors">
          <ShareIcon className="w-5 h-5" />
        </button>
      </div>
    </div>
  );
}
```

**Technologies**: React components, Heroicons, Tailwind CSS

**Study needed**: ✅ None

## Step 13.9: Set Up Routing and App Entry Point

**App Router**:

`src/App.tsx`:

```typescript
import { useEffect } from "react";
import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import { useAuthStore } from "./stores/authStore";
import Layout from "./components/Layout";
import LoginPage from "./pages/LoginPage";
import RegisterPage from "./pages/RegisterPage";
import HomePage from "./pages/HomePage";
import CommunitiesPage from "./pages/CommunitiesPage";
import MessagesPage from "./pages/MessagesPage";
import NotificationsPage from "./pages/NotificationsPage";
import ProfilePage from "./pages/ProfilePage";

function App() {
  const { isAuthenticated, loading, loadUser } = useAuthStore();

  useEffect(() => {
    loadUser();
  }, [loadUser]);

  if (loading) {
    return (
      <div className="flex items-center justify-center h-screen bg-gray-50">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary"></div>
      </div>
    );
  }

  return (
    <BrowserRouter>
      <Routes>
        {!isAuthenticated ? (
          <>
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />
            <Route path="*" element={<Navigate to="/login" replace />} />
          </>
        ) : (
          <Route
            path="/*"
            element={
              <Layout>
                <Routes>
                  <Route path="/" element={<HomePage />} />
                  <Route path="/communities" element={<CommunitiesPage />} />
                  <Route path="/messages" element={<MessagesPage />} />
                  <Route
                    path="/notifications"
                    element={<NotificationsPage />}
                  />
                  <Route path="/profile" element={<ProfilePage />} />
                  <Route path="*" element={<Navigate to="/" replace />} />
                </Routes>
              </Layout>
            }
          />
        )}
      </Routes>
    </BrowserRouter>
  );
}

export default App;
```

**Technologies**: React Router v6, protected routes

**Study needed**: ✅ None

## Step 13.10: Configure Tauri for Desktop Features

Update `src-tauri/tauri.conf.json`:

```json
{
  "build": {
    "beforeDevCommand": "npm run dev",
    "beforeBuildCommand": "npm run build",
    "devPath": "http://localhost:5173",
    "distDir": "../dist"
  },
  "package": {
    "productName": "Whispra",
    "version": "0.1.0"
  },
  "tauri": {
    "allowlist": {
      "all": false,
      "shell": {
        "all": false,
        "open": true
      },
      "window": {
        "all": false,
        "close": true,
        "hide": true,
        "show": true,
        "maximize": true,
        "minimize": true,
        "unmaximize": true,
        "unminimize": true,
        "startDragging": true
      },
      "notification": {
        "all": true
      }
    },
    "bundle": {
      "active": true,
      "targets": "all",
      "identifier": "com.whispra.desktop",
      "icon": [
        "icons/32x32.png",
        "icons/128x128.png",
        "icons/128x128@2x.png",
        "icons/icon.icns",
        "icons/icon.ico"
      ]
    },
    "security": {
      "csp": null
    },
    "windows": [
      {
        "fullscreen": false,
        "resizable": true,
        "title": "Whispra",
        "width": 1200,
        "height": 800,
        "minWidth": 800,
        "minHeight": 600
      }
    ],
    "systemTray": {
      "iconPath": "icons/icon.png",
      "iconAsTemplate": true
    }
  }
}
```

**Technologies**: Tauri configuration, window management

**Study needed**:

- 📚 Tauri configuration (1-2 hours)
  - Resource: https://tauri.app/v1/api/config/

## Step 13.11: Build and Run the Desktop App

**Development**:

```bash
# Run in development mode
npm run tauri dev
```

**Build for production**:

```bash
# Build for current platform
npm run tauri build

# Outputs:
# Windows: .exe installer in src-tauri/target/release/bundle/
# macOS: .dmg in src-tauri/target/release/bundle/
# Linux: .AppImage, .deb in src-tauri/target/release/bundle/
```

**Package scripts** (update `package.json`):

```json
{
  "scripts": {
    "dev": "vite",
    "build": "tsc && vite build",
    "preview": "vite preview",
    "tauri": "tauri",
    "tauri:dev": "tauri dev",
    "tauri:build": "tauri build"
  }
}
```

**Technologies**: Tauri CLI, Vite build

**Study needed**: ✅ None

---

**🔄 CHECKPOINT - Desktop App Complete!**

You now have:

- ✅ Tauri desktop app with React + Vite + TypeScript
- ✅ Tailwind CSS for styling
- ✅ Authentication (login/register)
- ✅ Zustand state management
- ✅ API integration with token refresh
- ✅ Home feed with posts
- ✅ Sidebar navigation
- ✅ Desktop-optimized UI
- ✅ Cross-platform builds (Windows, macOS, Linux)
- ✅ Small bundle size (~3-5 MB vs 100+ MB)

**Additional features to implement** (optional enhancements):

1. **System Tray**: Minimize to tray instead of closing
2. **Keyboard Shortcuts**: Global shortcuts (Ctrl+N for new post, etc.)
3. **Native Notifications**: Desktop push notifications
4. **Window State**: Remember window size/position
5. **Auto-Updates**: Tauri updater integration
6. **File Drag & Drop**: Drag images into posts
7. **Dark Mode**: Theme switching
8. **Multi-Window**: Separate chat windows

**Testing the desktop app:**

```bash
# Make sure backend is running
cd backend/Whispra.Api
dotnet run

# Start MongoDB, Redis, MinIO
docker compose up -d

# Run desktop app in dev mode
cd apps/desktop
npm run tauri dev
```

**Build sizes (approximate)**:

- Windows: ~4-6 MB (.exe)
- macOS: ~5-7 MB (.dmg)
- Linux: ~4-6 MB (.AppImage)

**Study needed**: ✅ Testing and refinement

Before moving to deployment, confirm:

1. ✅ Desktop app launches successfully?
2. ✅ Can you login and see the feed?
3. ✅ Does navigation work between pages?
4. ✅ Are posts loading from the API?
5. ✅ Can you like posts?
6. ✅ Does the app build for production?

**Next Phase Preview**: Final phase! We'll deploy everything to production with Docker, set up cloud hosting (Azure/AWS), configure CI/CD pipelines, monitoring, and production best practices!

---

# PHASE 14: Deployment & Production

**Goal**: Deploy the complete Whispra application to production with Docker containerization, cloud hosting, CI/CD pipelines, monitoring, logging, and production-ready configurations for scalability and reliability.

## Step 14.1: Understanding Production Architecture

**Production Stack**:

```
                        Users
                          ↓
                    Load Balancer
                          ↓
            ┌─────────────┴─────────────┐
            ↓                           ↓
      API Server 1                API Server 2
            ↓                           ↓
            └─────────────┬─────────────┘
                          ↓
        ┌─────────────────┼─────────────────┐
        ↓                 ↓                  ↓
    MongoDB           Redis Cache       MinIO Storage
    (Replica Set)     (Sentinel)        (Distributed)
```

**Deployment Options**:

1. **Azure (Recommended for .NET)**:

   - Azure App Service (API)
   - Azure Cosmos DB for MongoDB API
   - Azure Cache for Redis
   - Azure Blob Storage (alternative to MinIO)
   - Azure Container Registry
   - Azure Monitor + Application Insights

2. **AWS**:

   - ECS/EKS (containers)
   - DocumentDB (MongoDB-compatible)
   - ElastiCache (Redis)
   - S3 (storage)
   - CloudWatch (monitoring)

3. **Self-Hosted (VPS)**:
   - Docker Compose on DigitalOcean/Linode
   - Nginx reverse proxy
   - Let's Encrypt SSL
   - Monitoring with Prometheus + Grafana

**We'll cover all three approaches!**

📚 **Study needed**: Cloud deployment concepts (4-6 hours)

- Resource: https://learn.microsoft.com/en-us/azure/architecture/
- Resource: https://docs.docker.com/compose/production/
- Resource: https://nginx.org/en/docs/

## Step 14.2: Dockerize the Backend API

Create production-ready Dockerfile for the API.

`backend/Whispra.Api/Dockerfile`:

```dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["Whispra.Api/Whispra.Api.csproj", "Whispra.Api/"]
COPY ["Whispra.Application/Whispra.Application.csproj", "Whispra.Application/"]
COPY ["Whispra.Domain/Whispra.Domain.csproj", "Whispra.Domain/"]
COPY ["Whispra.Infrastructure/Whispra.Infrastructure.csproj", "Whispra.Infrastructure/"]

RUN dotnet restore "Whispra.Api/Whispra.Api.csproj"

# Copy all source code
COPY . .

# Build and publish
WORKDIR "/src/Whispra.Api"
RUN dotnet build "Whispra.Api.csproj" -c Release -o /app/build
RUN dotnet publish "Whispra.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Create non-root user
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Copy published app
COPY --from=build /app/publish .

# Environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 80

ENTRYPOINT ["dotnet", "Whispra.Api.dll"]
```

**Multi-stage build benefits**:

- ✅ Smaller final image (~200 MB vs 1 GB)
- ✅ More secure (no build tools in runtime)
- ✅ Faster deployments

**Technologies**: Docker multi-stage builds, .NET runtime optimization

**Study needed**:

- 📚 Docker best practices (2-3 hours)
  - Resource: https://docs.docker.com/develop/dev-best-practices/

## Step 14.3: Create Production Docker Compose

`docker-compose.prod.yml`:

```yaml
version: "3.8"

services:
  # Backend API
  api:
    build:
      context: ./backend
      dockerfile: Whispra.Api/Dockerfile
    container_name: whispra-api
    ports:
      - "5000:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - MongoDbSettings__ConnectionString=mongodb://mongodb:27017
      - MongoDbSettings__DatabaseName=whispra_prod
      - RedisSettings__ConnectionString=redis:6379
      - MinIOSettings__Endpoint=minio:9000
      - MinIOSettings__AccessKey=${MINIO_ACCESS_KEY}
      - MinIOSettings__SecretKey=${MINIO_SECRET_KEY}
      - JwtSettings__Secret=${JWT_SECRET}
      - JwtSettings__Issuer=https://api.whispra.com
      - JwtSettings__Audience=https://whispra.com
    depends_on:
      - mongodb
      - redis
      - minio
    restart: unless-stopped
    networks:
      - whispra-network
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:80/health"]
      interval: 30s
      timeout: 10s
      retries: 3

  # MongoDB
  mongodb:
    image: mongo:7.0
    container_name: whispra-mongodb
    ports:
      - "27017:27017"
    environment:
      - MONGO_INITDB_ROOT_USERNAME=${MONGO_ROOT_USER}
      - MONGO_INITDB_ROOT_PASSWORD=${MONGO_ROOT_PASSWORD}
    volumes:
      - mongodb-data:/data/db
      - mongodb-config:/data/configdb
    restart: unless-stopped
    networks:
      - whispra-network
    command: mongod --replSet rs0
    healthcheck:
      test: echo 'db.runCommand("ping").ok' | mongosh localhost:27017/test --quiet
      interval: 10s
      timeout: 5s
      retries: 5

  # Redis
  redis:
    image: redis:7-alpine
    container_name: whispra-redis
    ports:
      - "6379:6379"
    command: redis-server --requirepass ${REDIS_PASSWORD} --maxmemory 256mb --maxmemory-policy allkeys-lru
    volumes:
      - redis-data:/data
    restart: unless-stopped
    networks:
      - whispra-network
    healthcheck:
      test: ["CMD", "redis-cli", "ping"]
      interval: 10s
      timeout: 5s
      retries: 5

  # MinIO (S3-compatible storage)
  minio:
    image: minio/minio:latest
    container_name: whispra-minio
    ports:
      - "9000:9000"
      - "9001:9001"
    environment:
      - MINIO_ROOT_USER=${MINIO_ACCESS_KEY}
      - MINIO_ROOT_PASSWORD=${MINIO_SECRET_KEY}
    volumes:
      - minio-data:/data
    command: server /data --console-address ":9001"
    restart: unless-stopped
    networks:
      - whispra-network
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:9000/minio/health/live"]
      interval: 30s
      timeout: 10s
      retries: 3

  # Nginx Reverse Proxy
  nginx:
    image: nginx:alpine
    container_name: whispra-nginx
    ports:
      - "80:80"
      - "443:443"
    volumes:
      - ./nginx/nginx.conf:/etc/nginx/nginx.conf:ro
      - ./nginx/ssl:/etc/nginx/ssl:ro
      - nginx-logs:/var/log/nginx
    depends_on:
      - api
    restart: unless-stopped
    networks:
      - whispra-network

volumes:
  mongodb-data:
    driver: local
  mongodb-config:
    driver: local
  redis-data:
    driver: local
  minio-data:
    driver: local
  nginx-logs:
    driver: local

networks:
  whispra-network:
    driver: bridge
```

**Environment variables** (`.env.production`):

```bash
# MongoDB
MONGO_ROOT_USER=admin
MONGO_ROOT_PASSWORD=your-strong-mongodb-password-here

# Redis
REDIS_PASSWORD=your-strong-redis-password-here

# MinIO
MINIO_ACCESS_KEY=your-minio-access-key
MINIO_SECRET_KEY=your-strong-minio-secret-here

# JWT
JWT_SECRET=your-very-long-and-secure-jwt-secret-key-at-least-32-characters

# API
ASPNETCORE_ENVIRONMENT=Production
```

**Technologies**: Docker Compose production setup, health checks, volumes

**Study needed**: ✅ None, straightforward configuration

## Step 14.4: Configure Nginx Reverse Proxy

`nginx/nginx.conf`:

```nginx
events {
    worker_connections 1024;
}

http {
    # Rate limiting
    limit_req_zone $binary_remote_addr zone=api_limit:10m rate=10r/s;
    limit_req_zone $binary_remote_addr zone=auth_limit:10m rate=5r/m;

    # Upstream API servers
    upstream api_backend {
        least_conn;
        server api:80 max_fails=3 fail_timeout=30s;
        # Add more API servers for load balancing:
        # server api2:80 max_fails=3 fail_timeout=30s;
    }

    # HTTP to HTTPS redirect
    server {
        listen 80;
        server_name api.whispra.com whispra.com;

        location /.well-known/acme-challenge/ {
            root /var/www/certbot;
        }

        location / {
            return 301 https://$host$request_uri;
        }
    }

    # HTTPS API server
    server {
        listen 443 ssl http2;
        server_name api.whispra.com;

        # SSL certificates (Let's Encrypt)
        ssl_certificate /etc/nginx/ssl/fullchain.pem;
        ssl_certificate_key /etc/nginx/ssl/privkey.pem;
        ssl_protocols TLSv1.2 TLSv1.3;
        ssl_ciphers HIGH:!aNULL:!MD5;
        ssl_prefer_server_ciphers on;

        # Security headers
        add_header X-Frame-Options "SAMEORIGIN" always;
        add_header X-Content-Type-Options "nosniff" always;
        add_header X-XSS-Protection "1; mode=block" always;
        add_header Strict-Transport-Security "max-age=31536000; includeSubDomains" always;

        # Gzip compression
        gzip on;
        gzip_vary on;
        gzip_min_length 1024;
        gzip_types text/plain text/css application/json application/javascript text/xml application/xml;

        # API endpoints
        location /api/ {
            limit_req zone=api_limit burst=20 nodelay;

            proxy_pass http://api_backend;
            proxy_http_version 1.1;
            proxy_set_header Upgrade $http_upgrade;
            proxy_set_header Connection 'upgrade';
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
            proxy_set_header X-Forwarded-Proto $scheme;
            proxy_cache_bypass $http_upgrade;

            # Timeouts
            proxy_connect_timeout 60s;
            proxy_send_timeout 60s;
            proxy_read_timeout 60s;
        }

        # Auth endpoints (stricter rate limiting)
        location /api/Auth/ {
            limit_req zone=auth_limit burst=5 nodelay;

            proxy_pass http://api_backend;
            proxy_http_version 1.1;
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
            proxy_set_header X-Forwarded-Proto $scheme;
        }

        # SignalR WebSocket endpoints
        location /hubs/ {
            proxy_pass http://api_backend;
            proxy_http_version 1.1;
            proxy_set_header Upgrade $http_upgrade;
            proxy_set_header Connection "upgrade";
            proxy_set_header Host $host;
            proxy_cache_bypass $http_upgrade;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
            proxy_set_header X-Forwarded-Proto $scheme;

            # WebSocket timeout
            proxy_read_timeout 86400;
        }

        # Health check
        location /health {
            proxy_pass http://api_backend;
            access_log off;
        }
    }

    # Frontend (if serving static files)
    server {
        listen 443 ssl http2;
        server_name whispra.com www.whispra.com;

        ssl_certificate /etc/nginx/ssl/fullchain.pem;
        ssl_certificate_key /etc/nginx/ssl/privkey.pem;

        root /var/www/html;
        index index.html;

        location / {
            try_files $uri $uri/ /index.html;
        }

        # Cache static assets
        location ~* \.(jpg|jpeg|png|gif|ico|css|js|svg|woff|woff2|ttf|eot)$ {
            expires 1y;
            add_header Cache-Control "public, immutable";
        }
    }
}
```

**Technologies**: Nginx reverse proxy, SSL/TLS, rate limiting, load balancing

**Study needed**:

- 📚 Nginx configuration (2-3 hours)
  - Resource: https://nginx.org/en/docs/beginners_guide.html

## Step 14.5: Add Health Check Endpoint

Add health check to the API for monitoring.

`backend/Whispra.Api/Controllers/HealthController.cs`:

```csharp
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Whispra.Infrastructure.Persistence.MongoDB;

namespace Whispra.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    private readonly MongoDbContext _mongoContext;
    private readonly ILogger<HealthController> _logger;

    public HealthController(MongoDbContext mongoContext, ILogger<HealthController> logger)
    {
        _mongoContext = mongoContext;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            // Check MongoDB connection
            await _mongoContext.Users
                .Database
                .RunCommandAsync((Command<MongoDB.Bson.BsonDocument>)"{ping:1}");

            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow,
                version = "1.0.0",
                services = new
                {
                    database = "healthy",
                    api = "healthy"
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return StatusCode(503, new
            {
                status = "unhealthy",
                timestamp = DateTime.UtcNow,
                error = ex.Message
            });
        }
    }

    [HttpGet("ready")]
    public IActionResult Ready()
    {
        // Check if the application is ready to serve traffic
        return Ok(new { status = "ready", timestamp = DateTime.UtcNow });
    }

    [HttpGet("live")]
    public IActionResult Live()
    {
        // Check if the application is alive
        return Ok(new { status = "alive", timestamp = DateTime.UtcNow });
    }
}
```

**Technologies**: Health checks, Kubernetes readiness/liveness probes

**Study needed**: ✅ None

## Step 14.6: Set Up CI/CD with GitHub Actions

`.github/workflows/deploy-backend.yml`:

```yaml
name: Deploy Backend

on:
  push:
    branches: [main]
    paths:
      - "backend/**"
      - ".github/workflows/deploy-backend.yml"
  workflow_dispatch:

env:
  REGISTRY: ghcr.io
  IMAGE_NAME: ${{ github.repository }}/api

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: "8.0.x"

      - name: Restore dependencies
        run: dotnet restore
        working-directory: ./backend

      - name: Build
        run: dotnet build --no-restore
        working-directory: ./backend

      - name: Run unit tests
        run: dotnet test Whispra.UnitTests/Whispra.UnitTests.csproj --no-build --verbosity normal
        working-directory: ./backend

      - name: Run integration tests
        run: dotnet test Whispra.IntegrationTests/Whispra.IntegrationTests.csproj --no-build --verbosity normal
        working-directory: ./backend

  build-and-push:
    needs: test
    runs-on: ubuntu-latest
    permissions:
      contents: read
      packages: write

    steps:
      - uses: actions/checkout@v3

      - name: Log in to Container Registry
        uses: docker/login-action@v2
        with:
          registry: ${{ env.REGISTRY }}
          username: ${{ github.actor }}
          password: ${{ secrets.GITHUB_TOKEN }}

      - name: Extract metadata
        id: meta
        uses: docker/metadata-action@v4
        with:
          images: ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}
          tags: |
            type=ref,event=branch
            type=ref,event=pr
            type=semver,pattern={{version}}
            type=semver,pattern={{major}}.{{minor}}
            type=sha

      - name: Build and push Docker image
        uses: docker/build-push-action@v4
        with:
          context: ./backend
          file: ./backend/Whispra.Api/Dockerfile
          push: true
          tags: ${{ steps.meta.outputs.tags }}
          labels: ${{ steps.meta.outputs.labels }}
          cache-from: type=registry,ref=${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}:buildcache
          cache-to: type=registry,ref=${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}:buildcache,mode=max

  deploy:
    needs: build-and-push
    runs-on: ubuntu-latest
    if: github.ref == 'refs/heads/main'

    steps:
      - name: Deploy to production
        uses: appleboy/ssh-action@v0.1.10
        with:
          host: ${{ secrets.DEPLOY_HOST }}
          username: ${{ secrets.DEPLOY_USER }}
          key: ${{ secrets.DEPLOY_SSH_KEY }}
          script: |
            cd /opt/whispra
            docker-compose -f docker-compose.prod.yml pull api
            docker-compose -f docker-compose.prod.yml up -d api
            docker-compose -f docker-compose.prod.yml exec -T api dotnet ef database update

      - name: Notify deployment
        uses: 8398a7/action-slack@v3
        if: always()
        with:
          status: ${{ job.status }}
          text: "Backend deployment ${{ job.status }}"
          webhook_url: ${{ secrets.SLACK_WEBHOOK }}
```

**GitHub Secrets to configure**:

- `DEPLOY_HOST`: Your server IP/domain
- `DEPLOY_USER`: SSH username
- `DEPLOY_SSH_KEY`: Private SSH key
- `SLACK_WEBHOOK`: Slack webhook URL (optional)

**Technologies**: GitHub Actions, Docker Registry, automated deployments

**Study needed**:

- 📚 GitHub Actions (2-3 hours)
  - Resource: https://docs.github.com/en/actions

## Step 14.7: Azure Deployment (Option 1)

**Azure Resources Setup**:

```bash
# Install Azure CLI
# Windows: Download from https://aka.ms/installazurecliwindows
# macOS: brew install azure-cli
# Linux: curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash

# Login to Azure
az login

# Create resource group
az group create --name whispra-rg --location eastus

# Create Container Registry
az acr create --resource-group whispra-rg \
  --name whispraacr --sku Basic

# Create App Service Plan
az appservice plan create --name whispra-plan \
  --resource-group whispra-rg \
  --is-linux --sku B1

# Create Web App
az webapp create --resource-group whispra-rg \
  --plan whispra-plan \
  --name whispra-api \
  --deployment-container-image-name whispraacr.azurecr.io/api:latest

# Create Cosmos DB (MongoDB API)
az cosmosdb create --name whispra-cosmos \
  --resource-group whispra-rg \
  --kind MongoDB \
  --server-version 4.2

# Create Redis Cache
az redis create --name whispra-redis \
  --resource-group whispra-rg \
  --location eastus \
  --sku Basic --vm-size c0

# Create Storage Account (for media)
az storage account create --name whisprastorage \
  --resource-group whispra-rg \
  --location eastus \
  --sku Standard_LRS

# Configure App Settings
az webapp config appsettings set --resource-group whispra-rg \
  --name whispra-api \
  --settings \
    MongoDbSettings__ConnectionString="<cosmos-connection-string>" \
    MongoDbSettings__DatabaseName="whispra" \
    RedisSettings__ConnectionString="<redis-connection-string>" \
    JwtSettings__Secret="<your-jwt-secret>"
```

**Azure DevOps Pipeline** (`azure-pipelines.yml`):

```yaml
trigger:
  branches:
    include:
      - main
  paths:
    include:
      - backend/**

pool:
  vmImage: "ubuntu-latest"

variables:
  buildConfiguration: "Release"
  azureSubscription: "Whispra-Azure-Connection"
  appName: "whispra-api"
  containerRegistry: "whispraacr.azurecr.io"
  imageName: "api"

stages:
  - stage: Build
    jobs:
      - job: BuildAndTest
        steps:
          - task: UseDotNet@2
            inputs:
              version: "8.0.x"

          - task: DotNetCoreCLI@2
            displayName: "Restore packages"
            inputs:
              command: "restore"
              projects: "backend/**/*.csproj"

          - task: DotNetCoreCLI@2
            displayName: "Build"
            inputs:
              command: "build"
              projects: "backend/**/*.csproj"
              arguments: "--configuration $(buildConfiguration)"

          - task: DotNetCoreCLI@2
            displayName: "Run tests"
            inputs:
              command: "test"
              projects: "backend/**/*Tests.csproj"
              arguments: "--configuration $(buildConfiguration)"

  - stage: Docker
    dependsOn: Build
    jobs:
      - job: BuildAndPush
        steps:
          - task: Docker@2
            displayName: "Build and push image"
            inputs:
              containerRegistry: "whispraacr"
              repository: "$(imageName)"
              command: "buildAndPush"
              Dockerfile: "backend/Whispra.Api/Dockerfile"
              tags: |
                $(Build.BuildId)
                latest

  - stage: Deploy
    dependsOn: Docker
    jobs:
      - deployment: DeployToAzure
        environment: "production"
        strategy:
          runOnce:
            deploy:
              steps:
                - task: AzureWebAppContainer@1
                  inputs:
                    azureSubscription: "$(azureSubscription)"
                    appName: "$(appName)"
                    containers: "$(containerRegistry)/$(imageName):$(Build.BuildId)"
```

**Technologies**: Azure services, Azure DevOps

**Study needed**:

- 📚 Azure App Service (3-4 hours)
  - Resource: https://learn.microsoft.com/en-us/azure/app-service/

## Step 14.8: Monitoring and Logging

**Add Application Insights** (Azure):

```bash
# Install NuGet package
cd backend/Whispra.Api
dotnet add package Microsoft.ApplicationInsights.AspNetCore
```

Update `Program.cs`:

```csharp
using Microsoft.ApplicationInsights.Extensibility;

var builder = WebApplication.CreateBuilder(args);

// Add Application Insights
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
});

// ... rest of configuration
```

**Structured Logging with Serilog**:

```bash
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.Console
dotnet add package Serilog.Sinks.File
dotnet add package Serilog.Sinks.Elasticsearch
```

Update `Program.cs` for Serilog:

```csharp
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .WriteTo.Console()
    .WriteTo.File("logs/whispra-.log", rollingInterval: RollingInterval.Day)
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(builder.Configuration["Elasticsearch:Uri"]))
    {
        AutoRegisterTemplate = true,
        IndexFormat = $"whispra-logs-{DateTime.UtcNow:yyyy-MM}"
    })
    .CreateLogger();

builder.Host.UseSerilog();

// ... rest of configuration

var app = builder.Build();

// ... middleware

app.Run();

Log.CloseAndFlush();
```

**Prometheus Metrics**:

```bash
dotnet add package prometheus-net.AspNetCore
```

Update `Program.cs`:

```csharp
using Prometheus;

var app = builder.Build();

// ... other middleware

// Prometheus metrics
app.UseMetricServer(); // Exposes /metrics endpoint
app.UseHttpMetrics(); // Tracks HTTP request metrics

app.Run();
```

**Technologies**: Application Insights, Serilog, Elasticsearch, Prometheus

**Study needed**:

- 📚 Observability (2-3 hours)
  - Resource: https://learn.microsoft.com/en-us/dotnet/core/diagnostics/

## Step 14.9: Production Configuration Checklist

**Security**:

- ✅ HTTPS only (enforce SSL)
- ✅ Strong JWT secret (256-bit minimum)
- ✅ Secure database passwords
- ✅ API rate limiting
- ✅ CORS configuration (specific origins only)
- ✅ Security headers (CSP, HSTS, X-Frame-Options)
- ✅ Input validation and sanitization
- ✅ SQL/NoSQL injection prevention
- ✅ File upload size limits
- ✅ Disable detailed error messages

**Performance**:

- ✅ Response caching (Redis)
- ✅ Database indexing
- ✅ Connection pooling
- ✅ Gzip compression
- ✅ CDN for static assets
- ✅ Image optimization
- ✅ Lazy loading
- ✅ Pagination for large datasets
- ✅ Background jobs for heavy tasks

**Scalability**:

- ✅ Stateless API design
- ✅ Horizontal scaling ready
- ✅ Load balancing
- ✅ Database replication
- ✅ Redis Sentinel/Cluster
- ✅ Message queues for async processing
- ✅ Auto-scaling rules

**Reliability**:

- ✅ Health checks
- ✅ Graceful shutdown
- ✅ Circuit breakers
- ✅ Retry policies
- ✅ Database backups
- ✅ Disaster recovery plan
- ✅ Monitoring and alerting
- ✅ Log aggregation

**Production appsettings.json**:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "MongoDbSettings": {
    "ConnectionString": "#{MONGODB_CONNECTION_STRING}#",
    "DatabaseName": "whispra_prod"
  },
  "RedisSettings": {
    "ConnectionString": "#{REDIS_CONNECTION_STRING}#"
  },
  "MinIOSettings": {
    "Endpoint": "#{MINIO_ENDPOINT}#",
    "AccessKey": "#{MINIO_ACCESS_KEY}#",
    "SecretKey": "#{MINIO_SECRET_KEY}#",
    "BucketName": "whispra-media",
    "UseSSL": true
  },
  "JwtSettings": {
    "Secret": "#{JWT_SECRET}#",
    "Issuer": "https://api.whispra.com",
    "Audience": "https://whispra.com",
    "ExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
  },
  "ApplicationInsights": {
    "ConnectionString": "#{APPINSIGHTS_CONNECTION_STRING}#"
  },
  "Elasticsearch": {
    "Uri": "#{ELASTICSEARCH_URI}#"
  },
  "Cors": {
    "AllowedOrigins": ["https://whispra.com", "https://www.whispra.com"]
  }
}
```

**Technologies**: Production hardening, security best practices

**Study needed**: ✅ Review and implement checklist

## Step 14.10: Deploy and Monitor

**Deployment Steps**:

```bash
# 1. Build and test locally
docker-compose -f docker-compose.prod.yml build
docker-compose -f docker-compose.prod.yml up -d

# 2. Test health endpoints
curl http://localhost:5000/health
curl http://localhost:5000/health/ready
curl http://localhost:5000/health/live

# 3. Run smoke tests
npm run test:e2e

# 4. Push to registry
docker-compose -f docker-compose.prod.yml push

# 5. Deploy to production (via SSH or CI/CD)
ssh user@production-server
cd /opt/whispra
git pull origin main
docker-compose -f docker-compose.prod.yml pull
docker-compose -f docker-compose.prod.yml up -d

# 6. Verify deployment
curl https://api.whispra.com/health

# 7. Monitor logs
docker-compose -f docker-compose.prod.yml logs -f api
```

**Monitoring Dashboard** (Grafana + Prometheus):

Create `docker-compose.monitoring.yml`:

```yaml
version: "3.8"

services:
  prometheus:
    image: prom/prometheus:latest
    container_name: prometheus
    volumes:
      - ./prometheus/prometheus.yml:/etc/prometheus/prometheus.yml
      - prometheus-data:/prometheus
    command:
      - "--config.file=/etc/prometheus/prometheus.yml"
      - "--storage.tsdb.path=/prometheus"
    ports:
      - "9090:9090"
    networks:
      - whispra-network

  grafana:
    image: grafana/grafana:latest
    container_name: grafana
    volumes:
      - grafana-data:/var/lib/grafana
      - ./grafana/provisioning:/etc/grafana/provisioning
    environment:
      - GF_SECURITY_ADMIN_USER=admin
      - GF_SECURITY_ADMIN_PASSWORD=${GRAFANA_PASSWORD}
    ports:
      - "3000:3000"
    networks:
      - whispra-network
    depends_on:
      - prometheus

volumes:
  prometheus-data:
  grafana-data:

networks:
  whispra-network:
    external: true
```

**Prometheus configuration** (`prometheus/prometheus.yml`):

```yaml
global:
  scrape_interval: 15s

scrape_configs:
  - job_name: "whispra-api"
    static_configs:
      - targets: ["api:80"]
    metrics_path: "/metrics"

  - job_name: "mongodb"
    static_configs:
      - targets: ["mongodb-exporter:9216"]

  - job_name: "redis"
    static_configs:
      - targets: ["redis-exporter:9121"]
```

**Technologies**: Monitoring stack, Grafana dashboards

**Study needed**:

- 📚 Prometheus + Grafana (2-3 hours)
  - Resource: https://prometheus.io/docs/introduction/overview/

---

**🎉 CHECKPOINT - PRODUCTION DEPLOYMENT COMPLETE!**

You now have a **production-ready** Whispra application with:

**Backend Infrastructure**:

- ✅ Dockerized ASP.NET Core API
- ✅ MongoDB with replica set
- ✅ Redis caching layer
- ✅ MinIO object storage
- ✅ Nginx reverse proxy with SSL
- ✅ Health checks and monitoring

**CI/CD Pipeline**:

- ✅ Automated testing (unit + integration)
- ✅ Docker image builds
- ✅ Automated deployments
- ✅ GitHub Actions / Azure DevOps
- ✅ Slack/Teams notifications

**Cloud Deployment Options**:

- ✅ Azure (App Service + Cosmos DB)
- ✅ AWS (ECS + DocumentDB)
- ✅ Self-hosted (VPS + Docker Compose)

**Monitoring & Observability**:

- ✅ Application Insights / CloudWatch
- ✅ Structured logging (Serilog)
- ✅ Prometheus metrics
- ✅ Grafana dashboards
- ✅ Health endpoints
- ✅ Error tracking

**Security & Performance**:

- ✅ HTTPS/SSL encryption
- ✅ Rate limiting
- ✅ Security headers
- ✅ Gzip compression
- ✅ Caching strategies
- ✅ Database indexing

**Estimated Costs** (Monthly):

**Self-Hosted (DigitalOcean/Linode)**:

- VPS (4GB RAM, 2 CPU): $24/month
- Backups: $5/month
- Total: ~$30/month

**Azure**:

- App Service (B1): $13/month
- Cosmos DB (400 RU/s): $24/month
- Redis Cache (Basic): $16/month
- Blob Storage: $5/month
- Total: ~$60-80/month

**AWS**:

- ECS Fargate: $30/month
- DocumentDB: $50/month
- ElastiCache: $15/month
- S3: $5/month
- Total: ~$100/month

**Final Deployment Checklist**:

1. ✅ Domain name registered and DNS configured
2. ✅ SSL certificates installed (Let's Encrypt)
3. ✅ Environment variables secured
4. ✅ Database backups automated
5. ✅ Monitoring alerts configured
6. ✅ CI/CD pipeline tested
7. ✅ Load testing performed
8. ✅ Security audit completed
9. ✅ Documentation updated
10. ✅ Rollback plan prepared

**Next Steps**:

1. **Scale as needed**: Add more API servers behind load balancer
2. **Optimize**: Monitor performance metrics and optimize bottlenecks
3. **Feature flags**: Implement feature toggles for gradual rollouts
4. **A/B testing**: Test new features with subset of users
5. **CDN**: Add CloudFront/Azure CDN for static assets
6. **Message queues**: Add RabbitMQ/Azure Service Bus for background jobs
7. **Caching**: Implement advanced caching strategies
8. **Documentation**: API documentation with Swagger/OpenAPI
9. **Analytics**: User behavior tracking and analytics
10. **Mobile apps**: Deploy React Native and Tauri apps to stores

---

**🎊 CONGRATULATIONS! 🎊**

You've completed the **entire Whispra development roadmap**!

**What you've built**:

1. ✅ **Backend API** - ASP.NET Core with Clean Architecture
2. ✅ **Authentication** - JWT with refresh tokens
3. ✅ **Communities** - Public/private groups with roles
4. ✅ **Posts & Feed** - Text/photo/video posts with reactions
5. ✅ **Real-time Messaging** - SignalR chat with typing indicators
6. ✅ **Notifications** - In-app + push notifications
7. ✅ **Moderation** - Content reporting and user blocking
8. ✅ **Testing** - Unit, integration, and API tests
9. ✅ **Mobile App** - React Native with Expo
10. ✅ **Desktop App** - Tauri with React + Vite
11. ✅ **Production Deployment** - Docker + CI/CD + Monitoring

**Skills Acquired**:

- 🔧 Full-stack development (.NET + React + TypeScript)
- 🏗️ Clean Architecture and design patterns
- 🗄️ NoSQL database design (MongoDB)
- 🔐 Authentication and authorization
- 📱 Mobile development (React Native)
- 💻 Desktop development (Tauri)
- 🐳 Docker and containerization
- ☁️ Cloud deployment (Azure/AWS)
- 🔄 CI/CD pipelines
- 📊 Monitoring and observability
- 🧪 Testing strategies
- 🚀 Production best practices

**Your application is now**:

- 📈 **Scalable** - Handles growth with horizontal scaling
- 🔒 **Secure** - Industry-standard security practices
- ⚡ **Performant** - Optimized for speed
- 🛡️ **Reliable** - Health checks and monitoring
- 🌍 **Production-ready** - Deployed and running!

**Resources for Continued Learning**:

- 📚 Microsoft Learn: https://learn.microsoft.com/
- 📚 React Native: https://reactnative.dev/
- 📚 Tauri: https://tauri.app/
- 📚 MongoDB University: https://learn.mongodb.com/
- 📚 Docker: https://docs.docker.com/
- 📚 System Design: https://github.com/donnemartin/system-design-primer

**Share your success!**

Good luck with your Whispra journey!
