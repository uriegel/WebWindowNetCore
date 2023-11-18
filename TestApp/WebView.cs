using GtkDotNet;
using WebWindowNetCore.Data;
using System.Text.Json;

using LinqTools;
using System.Runtime.InteropServices;

using static AspNetExtensions.Core;

enum Action
{
    DevTools = 1,
}

record ScriptAction(Action Action, int? Width, int? Height, bool? IsMaximized);

public class WebView : WebWindowNetCore.Base.WebView
{
    public static WebViewBuilder Create() => new();

    public override int Run()
        => Application.Run(settings.AppId, app =>
            Application
                .NewWindow(app)
                    .SideEffect(_ => Application.EnableSynchronizationContext())
                    .SideEffect(w => w.SetTitle(settings.Title))
                    .SideEffectIf(settings.ResourceIcon != null,
                        w => Window.SetIconFromDotNetResource(w, settings.ResourceIcon))
                    .SideEffectChoose(settings.SaveBounds,
                        w =>
                        {
                            var bounds = Bounds.Retrieve(settings.AppId!, new Bounds(null, null, settings.Width, settings.Height, null));
                            w.SetDefaultSize(bounds.Width ?? 800, bounds.Height ?? 600);
                            if (bounds.IsMaximized == true)
                                w.Maximize();
                        },
                        w => w.SetDefaultSize(settings.Width, settings.Height))
                    .SideEffect(w => w.SetChild(
                        WebKit
                            .New()
                            .SideEffect(wk =>
                                wk
                                    .GetSettings()
                                    .SideEffectIf(settings.DevTools == true,
                                        s => s.SetBool("enable-developer-extras", true)))
                            .SideEffectIf(settings.DefaultContextMenuEnabled != true,
                                wk => wk.SignalConnect<BoolFunc>("context-menu", () => true))
                            .SideEffect(wk => wk.LoadUri(WebViewSettings.GetUri(settings)))
                            .SideEffect(wk => Gtk.SignalConnect<TwoIntPtr>(wk, "script-dialog", (_, d) =>
                            {
                                var msg = WebKit.ScriptDialogGetMessage(d);
                                var text = Marshal.PtrToStringUTF8(msg);
                                Console.WriteLine(text);
                                var action = JsonSerializer.Deserialize<ScriptAction>(text ?? "", JsonWebDefaults);

                                switch (action?.Action)
                                {
                                    case Action.DevTools:
                                        WebKit.InspectorShow(WebKit.GetInspector(wk));
                                        break;
                                }
                            }))
                            .SideEffect(wk => Gtk.SignalConnect<TwoIntPtr>(wk, "load-changed", (_, e) =>
                            {
                                if ((WebKitLoadEvent)e == WebKitLoadEvent.WEBKIT_LOAD_COMMITTED)
                                {
                                    if (settings.DevTools == true)
                                        WebKit.RunJavascript(wk,
                                            """ 
                                                function webViewShowDevTools() {
                                                    alert(JSON.stringify({action: 1}))
                                                }
                                            """);
                                    if ((settings.HttpSettings?.RequestDelegates?.Length ?? 0) > 0)
                                        WebKit.RunJavascript(wk,
                                            """ 
                                                async function webViewRequest(method, input) {
                                                    const msg = {
                                                        method: 'POST',
                                                        headers: { 'Content-Type': 'application/json' },
                                                        body: JSON.stringify(input)
                                                    }

                                                    const response = await fetch(`/request/${method}`, msg) 
                                                    return await response.json() 
                                                }
                                            """);
                                    settings.OnStarted?.Invoke();
                                }
                            }))
                    ))
                .SideEffectIf(settings.SaveBounds == true,
                    w => w.SideEffect(da => da.SignalConnectAfter<CloseDelegate>("close-request", (_, __) =>
                        false.SideEffect(_ =>
                            (Bounds.Retrieve(settings.AppId, new Bounds(null, null, settings.Width, settings.Height, null))
                                with { IsMaximized = w.IsMaximized(), Width = w.GetWidth(), Height = w.GetHeight() })
                                    .Save(settings.AppId))
                    )))
                .SideEffect(w => w.Show())
            );

    internal WebView(WebViewBuilder builder)
        => settings = builder.Data;

    delegate bool CloseDelegate(IntPtr z1, IntPtr z2);

    delegate bool BoolFunc();

    readonly WebViewSettings settings;
}

