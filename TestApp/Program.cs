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
    }
}

// TODO: Windows Make Sln WebWindowNetCore.Windows with Tester
// TODO: Windows Make Nuget package 