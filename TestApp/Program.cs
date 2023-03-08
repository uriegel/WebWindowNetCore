static class Program
{
    [STAThread]    
    static void Main()
    {
        WebView
            .Create()
            .InitialBounds(600, 800)
            .Title("Commander")
            .Url($"file://{Directory.GetCurrentDirectory()}/webroot/index.html")
#if DEBUG            
            .DebuggingEnabled()
#endif            
            .Build()
            .Run("de.uriegel.Commander");    }
}

