using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Whispra.Application.Interfaces.Repositories;
using Whispra.Application.Interfaces.Services;
using Whispra.Application.UseCases.Auth.Login;
using Whispra.Application.UseCases.Auth.RefreshTokens;
using Whispra.Application.UseCases.Communities.Create;
using Whispra.Application.UseCases.Communities.CreateInvite;
using Whispra.Application.UseCases.Communities.Join;
using Whispra.Application.UseCases.Communities.Leave;
using Whispra.Application.UseCases.Communities.UpdateRole;
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