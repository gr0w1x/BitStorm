using CommonServer.Asp.HostedServices;
using CommonServer.Data.Repositories;
using CommonServer.Utils.Extensions;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Users.HostedServices;
using Users.Repositories;
using Users.Services;

var builder = WebApplication.CreateBuilder(args);

// Builder configuration setup
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddDbConnection(builder.Configuration, "UsersDB");

// Builder services

// Users DB
builder.Services.AddDbContext<UsersContext>(
    options => options.UseMySQL(builder.Configuration.GetConnectionString("UsersDB")!)
);

// BCrypt
builder.Services.AddSingleton<IHasher, BcryptService>(
    _ => new BcryptService(Convert.ToInt32(builder.Configuration["BCRYPT_SALT"]))
);

// JWT
builder.Services.AddSingleton<JwtService>();

// Auth
builder.Services.AddJwtAuth(builder.Configuration.GetJwtOptions());

// Rabbit MQ
builder.Services.AddSingleton<IConnectionFactory>(new ConnectionFactory().DefaultRabbitMqConnection());
builder.Services.AddSingleton<RabbitMqProvider>();
builder.Services.AddHostedService<UsersRabbitMqService>();

// Hosted services
builder.Services.AddCron(
    new CronOptions<DeleteRefreshTokensJob>
    {
        Cron = "0 */1 * * * *"
    }
);

// Data layer
builder.Services.AddScoped<IUnitOfWork, DbContextUnitOfWork<UsersContext>>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IRefreshTokenRecordsRepository, RefreshTokenRecordsRepository>();
builder.Services.AddScoped<IConfirmRecordsRepository, ConfirmRecordsRepository>();

// Service layer
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UsersService>();

// Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger
builder.Services.AddDefaultSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    scope.ServiceProvider.GetRequiredService<IUnitOfWork>().Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultCors();

app.MapControllers();

app.Run();
