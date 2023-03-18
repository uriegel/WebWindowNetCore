using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using WebWindowNetCore.Data;
using AspNetExtensions;
using Microsoft.Extensions.Logging;
using CsTools.Extensions;

namespace WebWindowNetCore;

static class Kestrel
{
    public static void Start(HttpSettings settings)
    {
        WebApplication.CreateBuilder()
            .ConfigureWebHost(webHostBuilder =>
                webHostBuilder
                    .ConfigureKestrel(options =>
                        options
                            .ListenLocalhost(settings.Port))
                    .ConfigureServices(services =>
                        services
                            .When(!string.IsNullOrEmpty(settings.CorsOrigin), s => s.AddCors())
                            .AddResponseCompression())
                    .ConfigureLogging(builder =>
                        builder
                            .AddFilter(a => a == LogLevel.Warning)
                            .AddConsole()
                            .AddDebug()))
            .Build()
            .WithResponseCompression()
            .When(!string.IsNullOrEmpty(settings.CorsOrigin), app =>
                app.WithCors(builder =>
                    builder
                        .WithOrigins(settings.CorsOrigin!)
                        .AllowAnyHeader()
                        .AllowAnyMethod()))
            .WithRouting()
            .With(settings.RequestDelegates)

            .When(!string.IsNullOrEmpty(settings.WebrootUrl), app =>
                app.WithMapSubPath(settings.WebrootUrl!, async (context, subPath) =>
                {
                    await context.Response.StartAsync();
                    await Resources.Get("webroot/" + subPath)!.CopyToAsync(context.Response.Body);
                }))
            .StartAsync(); 
    }
}

