using CommonServer.Asp.HostedServices;
using CommonServer.Data.Repositories;
using CommonServer.Utils.Extensions;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Tasks.HostedServices;
using Tasks.Repositories;
using Tasks.Services;

var builder = WebApplication.CreateBuilder(args);

// Builder configuration setup
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddDbConnection(builder.Configuration, "TasksDB");

// Builder services

// Tasks DB
builder.Services.AddDbContext<TasksContext>(
    options => options.UseMySQL(builder.Configuration.GetConnectionString("TasksDB")!)
);

// Auth
builder.Services.AddJwtAuth(builder.Configuration.GetJwtOptions());

// RabbitMq
builder.Services.AddSingleton<IConnectionFactory>(new ConnectionFactory().DefaultRabbitMqConnection());
builder.Services.AddSingleton<RabbitMqProvider>();
builder.Services.AddHostedService<TasksRabbitMqService>();

// Data layer
builder.Services.AddScoped<IUnitOfWork, DbContextUnitOfWork<TasksContext>>();
builder.Services.AddScoped<ITasksRepository, TasksRepository>();
builder.Services.AddScoped<ITagsRepository, TagsRepository>();
builder.Services.AddScoped<ITaskImplementationsRepository, TaskImplementationRepository>();

// Service layer
builder.Services.AddScoped<TasksService>();

// Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger
builder.Services.AddDefaultSwaggerGen();

var app = builder.Build();

// Migration
using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    await scope.ServiceProvider.GetRequiredService<IUnitOfWork>().Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultCors();

app.MapControllers();

app.UseDefaultExceptionHandler();

app.Run();
