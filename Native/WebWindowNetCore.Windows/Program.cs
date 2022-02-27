using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WebWindowNetCore;
public static class Program
{
    public static void Execute()
    {
        var instance = Process.GetCurrentProcess().Handle;

        var wc = new WindowClass()
        {
            Style = WindowClassStyles.HorizontalRedraw | WindowClassStyles.VerticalRedraw,
            WindowProcedure = WindowProcedure,
            Cursor = LoadCursor(IntPtr.Zero, 32512),
            Instance = instance,
            BackgroundBrush = new IntPtr(5 + 1),
            ClassName = className,
        };
        var atom = RegisterClass(ref wc);
    
        var hwnd = IntPtr.Zero;
        unchecked
        {
            hwnd = CreateWindowEx(WindowStylesEx.Null, className, "Ein C++ Fenster", WindowStyles.WS_OVERLAPPEDWINDOW, 
                (int)0x80000000, 0, (int)0x80000000,
                0, IntPtr.Zero, IntPtr.Zero, instance, IntPtr.Zero);
        }
        ShowWindow(hwnd, ShowWindowCommands.Show);
        UpdateWindow(hwnd);

        while (GetMessage(out var msg, IntPtr.Zero, 0, 0) != 0)
            DispatchMessage(ref msg);
    }

    static IntPtr WindowProcedure(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam)
    {
        switch ((WindowMessage)msg)
        {
            case WindowMessage.Destroy:
                PostQuitMessage(124);
                break;
        }
        return DefWindowProc(hWnd, msg, wParam, lParam);
    }

    const string className = "WebWindowNetCore";

    [DllImport("user32.dll")]
    static extern IntPtr LoadCursor(IntPtr instance, int intResource);
    [DllImport("user32.dll", SetLastError = true)]
    static extern ushort RegisterClass([In] ref WindowClass windowClass);
    [DllImport("user32.dll")]
    static extern void PostQuitMessage(int exitCode);
    [DllImport("user32.dll")]
    static extern IntPtr DefWindowProc(IntPtr hWnd, int message, IntPtr wParam, IntPtr lParam);
    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr CreateWindowEx(WindowStylesEx styleEx, string className, string windowName, WindowStyles dwStyle, int x, int y, int width, int height, IntPtr hWndParent, IntPtr menu, IntPtr instance, IntPtr lpParam);
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool ShowWindow(IntPtr hWnd, ShowWindowCommands nCmdShow);
	[DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
	static extern bool UpdateWindow(IntPtr hWnd);
    [DllImport("user32.dll")]
    static extern sbyte GetMessage(out ApiMessage message, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);
    [DllImport("user32.dll")]
    static extern IntPtr DispatchMessage([In] ref ApiMessage message);
}
