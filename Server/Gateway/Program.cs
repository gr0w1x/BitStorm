using CommonServer.Utils.Extensions;
using Gateway;
using Gateway.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Builder configuration setup
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddJsonFile("Ocelot.Localhost.json", true, true);
builder.Configuration.AddDbConnection(builder.Configuration, "UsersDB");

builder.Services.AddOcelot();

builder.Services.AddDbContext<OnlyUsersContext>(
    options => options.UseMySQL(builder.Configuration.GetConnectionString("UsersDB")!)
);

var jwtOptions = builder.Configuration.GetJwtOptions();
builder.Services.AddSingleton(jwtOptions);
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddScheme<JwtBearerOptions, JwtBarerChecksUsersHandler>(
        JwtBearerDefaults.AuthenticationScheme,
        (options) =>
        {
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = jwtOptions.Key,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                ClockSkew = TimeSpan.Zero
            };
        }
    );

builder.Services.AddCors();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseDefaultCors();

app.UseRouting();
app.MapControllers();

await app.UseOcelot();

app.Run();
