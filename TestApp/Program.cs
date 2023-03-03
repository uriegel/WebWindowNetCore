using WebWindowNetCore;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        var setting = new Configuration(FullscreenEnabled: true);
        // TODO: App.run();
    }
}

// TODO: Structure:
// TODO: WebWindoNetCore is a Dll with a webView
// TODO: the app is a program with resources such as web site, icon, native webViewHandler
// TODO: Builder concept
// TODO: Nuget with platform dependant references
// TODO: Windows Make Sln WebWindowNetCore.Windows with Tester
// TODO: Windows Make Nuget package 