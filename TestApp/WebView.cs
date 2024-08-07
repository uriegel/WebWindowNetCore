// using WebWindowNetCore.Data;
// using CsTools.Extensions;
// using GtkDotNet;

// enum Action
// {
//     DevTools = 1,
// }




// record ScriptAction(Action Action, int? Width, int? Height, bool? IsMaximized);

// public class WebView : WebWindowNetCore.Base.WebView
// {
//     public static WebViewBuilder Create() => new();

//     public override int Run()
//         => Application
//             .New(settings.AppId)
//             .OnActivate(app => app
//                 .NewWindow()
//                     .Title(settings.Title)
//                     .SideEffectIf(settings.ResourceIcon != null,
//                         w => w.ResourceIcon(settings.ResourceIcon!))
//                     .SideEffectChoose(settings.SaveBounds,
//                         w =>
//                         {
//                             var bounds = Bounds.Retrieve(settings.AppId!, new Bounds(null, null, settings.Width, settings.Height, null));
//                             w.DefaultSize(bounds.Width ?? 800, bounds.Height ?? 600);
//                             if (bounds.IsMaximized == true)
//                                 w.Maximize();
//                         },
//                         w => w.DefaultSize(settings.Width, settings.Height))
//                     .Child(
//                         WebKit
//                             .New()
//                             .SideEffect(wk =>
//                                 wk
//                                     .GetSettings()
//                                     .SideEffectIf(settings.DevTools == true,
//                                         s => s.SetBool("enable-developer-extras", true)))
//                             .SideEffect(wk => wk.LoadUri(WebViewSettings.GetUri(settings)))
//                     )
//                 .Show())
//             .Run(0, 0);

//     internal WebView(WebViewBuilder builder)
//         => settings = builder.Data;

//     delegate bool CloseDelegate(IntPtr z1, IntPtr z2);

//     delegate bool BoolFunc();

//     readonly WebViewSettings settings;
// }

