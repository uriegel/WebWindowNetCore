namespace WebWindowNetCore
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
open FSharpTools
open Giraffe

type CustomJsonSerializer() =
    let jsonOptions = JsonSerializerOptions(
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    )
       
    interface Json.ISerializer with
        member _.SerializeToString<'T>(input: 'T) : string =
            JsonSerializer.Serialize<'T>(input, jsonOptions)
        member _.Deserialize<'T>(json: string) : 'T =
            JsonSerializer.Deserialize<'T>(json, jsonOptions)
        member this.Deserialize(bytes: byte array): 'T = 
            JsonSerializer.Deserialize(bytes, jsonOptions)
        member this.DeserializeAsync<'T>(stream: System.IO.Stream): System.Threading.Tasks.Task<'T> = 
            task {
                return! JsonSerializer.DeserializeAsync<'T>(stream, jsonOptions)
            }
        member this.SerializeToBytes(input: 'T): byte array = 
            JsonSerializer.SerializeToUtf8Bytes(input, jsonOptions)
        member this.SerializeToStreamAsync(value: 'T) (stream: System.IO.Stream): System.Threading.Tasks.Task = 
            JsonSerializer.SerializeAsync(stream, value, jsonOptions)

module internal Server =
    open Giraffe
    open GiraffeTools
    let start (webView: WebViewBase) =
        let configureServices (services : IServiceCollection) = 
            services
                .AddSingleton<Json.ISerializer, CustomJsonSerializer>() 
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

        let getWebrootResource path= 
            Resources.get ("webroot/" + path) 
            |> Option.defaultValue null

        let getResourceFile path = 
            setContentType <| ContentType.get path >=> streamData false (getWebrootResource path) None None

        let getStatic = subRoute "/webroot" (routePathes () <| httpHandlerParam getResourceFile)

        let prependIf predicate handler  handlerList =
            if predicate then
                handler :: handlerList
            else
                handlerList

        let configureRoutes (app : IApplicationBuilder) = 
            let host (host: string) (next: HttpFunc) (ctx: HttpContext) =
                match ctx.Request.Host.Host with
                | value when value = host -> next ctx
                | _                       -> skipPipeline

            let routes = choose [ host "localhost" 
                >=> choose ((webView.RequestsValue 
                                        |> List.map warble) 
                                        |> List.append webView.RawRequestsValue
                                        |> prependIf webView.ResourceFromHttpValue getStatic)]
            
            app
                .UseResponseCompression()
                .UseCors(useCors)
                .UseGiraffe routes      

            webView.RequestsDelegatesValue
            |> Array.iter (fun d ->  d.Invoke(app))
                
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

