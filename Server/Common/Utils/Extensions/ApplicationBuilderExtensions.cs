using CommonServer.Data.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

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
}
