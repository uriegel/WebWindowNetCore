namespace WebWindowNetCore
#if Windows
open Microsoft.Win32
open ClrWinApi

module Theme = 

    let getIsDark (key: RegistryKey) = 
        let value = key.GetValue "SystemUsesLightTheme"
        System.Convert.ToInt32(value) = 0

    let isDark () = 
        getIsDark (Registry.CurrentUser.OpenSubKey @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize")
        

    let startDetection(onChanged: bool->unit) =
        let key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize")

//      [<TailCall>]
        let rec check currentTheme =
            let status = Api.RegNotifyChangeKeyValue(key.Handle.DangerousGetHandle(), false, 4, 0, false)
            if status = 0 then
                let theme = getIsDark key
                if currentTheme <> theme then
                    onChanged theme
                check theme 

        async {
            check <| getIsDark key
        } |> Async.Start

#endif