namespace WebWindowNetCore;

public abstract class WebWindow
{
#if Windows
    public static WebWindowBuilder Builder() => new Windows.WebWindowBuilder();
#elif Linux
    public static WebWindowBuilder Builder() => new Linux.WebWindowBuilder();
#endif

    public abstract bool IsMaximized { get; }
    public abstract int Run();
    public abstract void ShowDevTools();
    public abstract Task StartDragFiles(string[] dragFiles);
    public abstract void RunJavascript(string script);
    public abstract void Close();
    public abstract void Minimize();
    public abstract void Maximize();
    public abstract void Restore();
    public abstract void BeginInvoke(Action action);
    public abstract Task<T> InvokeAsync<T>(Func<T> func);
    public abstract void SetFocus();
}
