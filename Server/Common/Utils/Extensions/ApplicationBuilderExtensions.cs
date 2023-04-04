using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Types.Constants.Errors;
using Types.Dtos;

namespace CommonServer.Utils.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseDefaultCors(this IApplicationBuilder app)
    {
        app.UseCors(cors =>
        {
            cors
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
        return app;
    }

    public static IApplicationBuilder UseDefaultExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseExceptionHandler(options =>
        {
            options.Run(
                async context =>
                {
                    var logger = context.RequestServices.GetRequiredService<ILogger<IApplicationBuilder>>();
                    var exception = context.Features.Get<IExceptionHandlerFeature>();
                    if (exception != null)
                    {
                        logger.LogError(exception.Error.Message);
                    }
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    ErrorDto error = new("internal server error", CommonErrors.InternalServerError);
                    await context.Response.WriteAsJsonAsync(error);
                }
            );
        });
    }
}
