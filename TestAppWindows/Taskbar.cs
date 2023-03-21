using System.Runtime.InteropServices;

static class Taskbar
{
    [DllImport("user32.dll")]
    static extern int FindWindow(string className, string windowText);

    [DllImport("user32.dll")]
    static extern int ShowWindow(int hwnd, int command);

    [DllImport("user32.dll")]
    static extern int FindWindowEx(int parentHandle, int childAfter, string className, int windowTitle);

    [DllImport("user32.dll")]
    static extern int GetDesktopWindow();

    const int SW_HIDE = 0;
    const int SW_SHOW = 1;

    static int Handle
    {
        get
        {
            return FindWindow("Shell_TrayWnd", "");
        }
    }

    static int HandleOfStartButton
    {
        get
        {
            int handleOfDesktop = GetDesktopWindow();
            int handleOfStartButton = FindWindowEx(handleOfDesktop, 0, "button", 0);
            return handleOfStartButton;
        }
    }

    public static void Show()
    {
        ShowWindow(Handle, SW_SHOW);
        ShowWindow(HandleOfStartButton, SW_SHOW);
    }

    public static void Hide()
    {
        ShowWindow(Handle, SW_HIDE);
        ShowWindow(HandleOfStartButton, SW_HIDE);
    }
}