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

module Server =
    open Microsoft.Extensions.Hosting

    type Input = {
        Name: string
    }
    type Registered = {
        Registered: bool
    }

    let configureServices (services : IServiceCollection) = 
        let jsonOptions = JsonSerializerOptions()
        jsonOptions.PropertyNamingPolicy <- JsonNamingPolicy.CamelCase
        jsonOptions.Encoder <- JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        jsonOptions.DefaultIgnoreCondition <- JsonIgnoreCondition.WhenWritingNull
        services
            .AddSingleton(jsonOptions) 
            .AddResponseCompression()
            .AddGiraffe()
        |> ignore

    let login (input: Input) = 
        printfn "Registered superfit user: %s" "3456"
        { Registered = true }

    let SuperfitLogin () = 
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! input = ctx.BindJsonAsync<Input> ()
                let result = login input
                return! json result next ctx
            }

    let configureRoutes (app : IApplicationBuilder) = 
        let host (host: string) (next: HttpFunc) (ctx: HttpContext) =
            match ctx.Request.Host.Host with
            | value when value = host -> next ctx
            | _                       -> skipPipeline

        let routes =
            choose [
                host "localhost"                          >=>
                    choose [  
                        route  "/superfit/login"    >=> warbler (fun _ -> SuperfitLogin ())
                    ]
            ]
        
        app
            .UseResponseCompression()
            .UseGiraffe routes      
    
    let configureKestrel (options: KestrelServerOptions) = 

        let httpPort  () = 20000
        
        options.ListenAnyIP(httpPort ())

    let webHostBuilder (webHostBuilder: IWebHostBuilder) = 
        webHostBuilder
            .ConfigureKestrel(configureKestrel)
            .Configure(configureRoutes)
            .ConfigureServices(configureServices)
            |> ignore

    let start () =
        Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webHostBuilder)
            .Build()
            .Start()
