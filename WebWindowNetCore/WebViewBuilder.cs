using CsTools.Extensions;
using WebWindowNetCore.Data;


// public abstract class WebViewBuilder
// {
//     public T DownCast<T>()
//         where T : WebViewBuilder
//         => (this as T)!;

//     /// <summary>
//     /// Callback onScriptAction is called via javascript function webViewScriptAction(id: number)
//     /// </summary>
//     /// <param name="onScriptAction"></param>
//     /// <returns></returns>
//     public WebViewBuilder OnScriptAction(Action<int, string?> onScriptAction)
//         => this.SideEffect(n => Data.OnScriptAction = onScriptAction);

//     /// <summary>
//     /// Add a query string to the Url (or DebugUrl, or HttpBuilder.ResourceWebroot)
//     /// </summary>
//     /// <param name="getQuery">A function returning a query string such as '?theme=adwaita&amp;platform=linux' which is added to the Url</param>
//     /// <returns></returns>
//     public WebViewBuilder QueryString(Func<string> getQuery)
//         => this.SideEffect(n => Data.GetQuery = getQuery);

//     public WebViewBuilder SaveBounds()
//         => this.SideEffect(n => Data.SaveBounds = true);

//     public WebViewBuilder DebuggingEnabled()
//         => this.SideEffect(n => Data.DevTools = true);

//     public WebViewBuilder ResourceIcon(string resourcePath)
//         => this.SideEffect(n =>  Data.ResourceIcon = resourcePath);

//     public WebViewBuilder ConfigureHttp(Func<HttpBuilder, HttpSettings> builder)
//         => this.SideEffect(n => Data.HttpSettings = builder(new HttpBuilder()));

//     public WebViewBuilder WithoutNativeTitlebar()
//         => this.SideEffect(n => Data.WithoutNativeTitlebar = true);

//     public WebViewBuilder OnWindowStateChanged(Action<WebWindowState> action)
//         => this.SideEffect(n => Data.OnWindowStateChanged = action);        

//     public WebViewBuilder OnFilesDrop(Action<string, bool, string[]> onFilesDrop)
//         => this.SideEffect(n => Data.OnFilesDrop = onFilesDrop);        

//     public WebViewBuilder OnStarted(Action onStarted)
//         => this.SideEffect(n => Data.OnStarted = onStarted);        

//     public WebViewBuilder OnClosing(Func<bool> canClose)
//         => this.SideEffect(n => Data.CanClose = canClose);        

//     public WebViewBuilder DefaultContextMenuEnabled()
//         => this.SideEffect(n => Data.DefaultContextMenuEnabled = true);        

//     public abstract WebView Build();
    
//     protected WebViewBuilder() { }

//     protected WebViewSettings Data { get; } = new();
// }
