namespace WebWindowNetCore
open Giraffe
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open System.Text.Json
open System.Text.Encodings.Web
open System.Text.Json.Serialization
open Microsoft.AspNetCore.Server.Kestrel.Core
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Hosting
open Microsoft.AspNetCore.Cors.Infrastructure

module Server =
    open System.Threading.Tasks
    open Microsoft.Extensions.Logging
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
            // TODO Cors origin
            builder.WithOrigins([|"*"|]).AllowAnyHeader().AllowAnyMethod () |> ignore
            ()

        let warble (request: unit->HttpFunc->HttpContext->Task<option<HttpContext>>) =
            route  "/test" >=> warbler (fun _ -> request ())

        let configureRoutes (app : IApplicationBuilder) = 
            let host (host: string) (next: HttpFunc) (ctx: HttpContext) =
                match ctx.Request.Host.Host with
                | value when value = host -> next ctx
                | _                       -> skipPipeline

            let routes = choose [ host "localhost" >=> choose (webView.Requests |> List.map warble) ]
            
            app
                .UseResponseCompression()
                .UseCors(useCors)
                .UseGiraffe routes      

        let configureKestrel (options: KestrelServerOptions) = 

            // TODO configure port
            let httpPort  () = 20000
            
            options.ListenAnyIP(httpPort ())

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
