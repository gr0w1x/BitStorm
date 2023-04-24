using CommonServer.Asp.HostedServices;
using CommonServer.Data.Types;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace CommonServer.Utils.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddCron<T>(this IServiceCollection collection, ICronOptions<T> options)
        where T : CronJobService
    {
        collection.AddSingleton(options);
        collection.AddHostedService<T>();
        return collection;
    }

    public static IServiceCollection AddScheduler(this IServiceCollection collection)
    {
        collection.AddSingleton<SchedulerController>();
        collection.AddHostedService<SchedulerService>();
        return collection;
    }

    public static IServiceCollection AddJwtAuth(this IServiceCollection services, JwtOptions jwtOptions)
    {
        services.AddSingleton(jwtOptions);
        var builder = (JwtBearerOptions options) =>
        {
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                IssuerSigningKey = jwtOptions.Key,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                ClockSkew = TimeSpan.Zero
            };
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                }
            };
        };
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddScheme<JwtBearerOptions, JwtBearerHandler>(
                JwtBearerDefaults.AuthenticationScheme,
                builder
            );
        return services;
    }

    public static IServiceCollection AddDefaultSwaggerGen(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description =
                    "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme,
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }
}
