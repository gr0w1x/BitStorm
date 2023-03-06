using Microsoft.AspNetCore.Builder;

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
