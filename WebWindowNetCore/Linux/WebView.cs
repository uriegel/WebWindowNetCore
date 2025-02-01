using CsTools.Extensions;
using GtkDotNet;
using GtkDotNet.SafeHandles;

namespace WebWindowNetCore.Linux;

public class WebView() : WebWindowNetCore.WebView
{
    public override int Run() =>
        Application.NewAdwaita(appId)
            .OnActivate(OnActivate)
            .Run(0, 0);

    void OnActivate(ApplicationHandle app)
        => app
            .NewWindow()
            .Title(title)
            .SideEffectChoose(saveBounds, RetrieveBounds, w => w.DefaultSize(width, height))
            .Show();

    void RetrieveBounds(WindowHandle window)            
    {
        
    }
}
