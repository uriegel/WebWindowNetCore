using AspNetExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

public static class WebApplicationExtensions
{
    public static WebApplication WithMapSubPath(this WebApplication app, string path, Func<HttpContext, string, Task> handler)
        => app.With(async (context, next) =>
        {
            if (context.Request.Path.ToString().StartsWith(path))
            {
                await handler(context, context.Request.Path.ToString()[(path.Length+1)..]);
                return;
            }
            await next(context);
        });
}