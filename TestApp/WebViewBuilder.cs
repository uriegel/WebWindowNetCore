
using WebWindowNetCore.Data;

public class WebViewBuilder : WebWindowNetCore.WebViewBuilder
{
    public override WebView Build() => new WebView(this);

    internal new WebViewSettings Data { get => base.Data; }
}