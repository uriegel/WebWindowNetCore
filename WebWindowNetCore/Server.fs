namespace WebWindowNetCore
open System.Reflection
open System.Text.Json
open System.Text.Encodings.Web
open System.Text.Json.Serialization
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.Server.Kestrel.Core
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Hosting
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.Extensions.Logging
open Giraffe
open GiraffeTools

module Server =
    let start (webView: WebViewBase) =
        let configureServices (services : IServiceCollection) = 
            let jsonOptions = JsonSerializerOptions()
            jsonOptions.PropertyNamingPolicy <- JsonNamingPolicy.CamelCase
            jsonOptions.Encoder <- JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            jsonOptions.DefaultIgnoreCondition <- JsonIgnoreCondition.WhenWritingNull
            services
                .AddSingleton(jsonOptions) 
                .AddResponseCompression()
                .AddGiraffe()
                .AddCors()
            |> ignore

        let useCors (builder: CorsPolicyBuilder) = 
            builder
                .WithOrigins(webView.CorsDomainsValue)
                .AllowAnyHeader()
                .SetPreflightMaxAge(webView.CorsCacheValue)
                .AllowAnyMethod () 
                |> ignore
            ()

        let warble (request: Request) =
            route  ("/requests/" + request.Method) >=> warbler (fun _ -> request.Request ())

        let getResource path= 
            Assembly
                .GetExecutingAssembly()
                .GetManifestResourceStream(path)

        let getResourceFile path = 
            setContentType <| ContentType.get path >=> streamData false (getResource <| sprintf "web/%s" path) None None

        let getStatic = routef "/static/js/%s" (fun _ -> httpHandlerParam getResourceFile "scripts/script.js")            

        let configureRoutes (app : IApplicationBuilder) = 
            let host (host: string) (next: HttpFunc) (ctx: HttpContext) =
                match ctx.Request.Host.Host with
                | value when value = host -> next ctx
                | _                       -> skipPipeline

            let routes = choose [ host "localhost" 
                >=> choose (getStatic :: (webView.Requests |> List.map warble)) ]
            
            app
                .UseResponseCompression()
                .UseCors(useCors)
                .UseGiraffe routes      

        let configureKestrel (options: KestrelServerOptions) = 
            options.ListenAnyIP(webView.RequestPortValue)

        let configureLogging (builder : ILoggingBuilder) =
            // Set a logging filter (optional)
            let filter l = l.Equals LogLevel.Warning

            // Configure the logging factory
            builder.AddFilter(filter) // Optional filter
                .AddConsole()      // Set up the Console logger
                .AddDebug()        // Set up the Debug logger

                // Add additional loggers if wanted...
            |> ignore            

        let webHostBuilder (webHostBuilder: IWebHostBuilder) = 
            webHostBuilder
                .ConfigureKestrel(configureKestrel)
                .Configure(configureRoutes)
                .ConfigureServices(configureServices)
                .ConfigureLogging(configureLogging)
                |> ignore

        Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webHostBuilder)
            .Build()
            .Start()
