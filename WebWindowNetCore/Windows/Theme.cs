
#if Windows
using Microsoft.Win32;
using ClrWinApi;
using CsTools.Extensions;

namespace WebWindowNetCore;

static class Theme
{
    public static bool GetIsDark(RegistryKey? key)
        => Convert.ToInt32(key?.GetValue("SystemUsesLightTheme")) == 0;

    public static bool IsDark()
        => GetIsDark(Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"));

    public static void StartDetection(Action<bool> onChanged)
    {
        var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
        var currentTheme = GetIsDark(key);
        if (key != null)
            new Thread(() =>
            {
                while (true)
                {
                    var status = Api.RegNotifyChangeKeyValue(key.Handle.DangerousGetHandle(), false, 4, 0, false);
                    if (status != 0)
                        break;
                    var theme = GetIsDark(key);
                    if (currentTheme != theme)
                        onChanged(theme);
                    currentTheme = theme;
                }
            })
                .SideEffect(t => t.IsBackground = true)
                .Start();
    }
}
#endif