using CommonServer.Asp.UserIdProviders;
using CommonServer.Utils.Extensions;
using CommonServer.Utils.RabbitMq;
using Executions.HostedServices;
using Executions.Hubs;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Builder configuration setup
builder.Configuration.AddEnvironmentVariables();

// Builder services
builder.Services.AddScheduler();

// Auth
builder.Services.AddJwtAuth(builder.Configuration.GetJwtOptions());
builder.Services.AddSingleton<IUserIdProvider, UserIdByIdProvider>();

// RabbitMq
builder.Services.AddSingleton<IConnectionFactory>(new ConnectionFactory().DefaultRabbitMqConnection());
builder.Services.AddSingleton<MessageHandlers>();
builder.Services.AddSingleton<ExecutionsRabbitMqProvider>();
builder.Services.AddHostedService<ExecutionsRabbitMqService>();

// Controllers
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseDefaultCors();

app.UseAuthorization();
app.UseAuthentication();

app.MapHub<ExecutionsHub>("/hub");

app.UseDefaultExceptionHandler();

app.Run();
