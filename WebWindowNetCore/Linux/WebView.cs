using GtkDotNet;
using GtkDotNet.SafeHandles;

namespace WebWindowNetCore.Linux;

public class WebView() : WebWindowNetCore.WebView
{
    public override int Run() =>
        Application.NewAdwaita(appId)
            .OnActivate(OnActivate)
            .Run(0, 0);

    static void OnActivate(ApplicationHandle app)
        => app
            .NewWindow()
            .Title("NOCH ZU FÜLLEN")
            .Show();
}
