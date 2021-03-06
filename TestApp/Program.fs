﻿open System
open WebWindowNetCore
open WebWindow
open Commands

//let iconPath = @"C:\Users\urieg\source\repos\WebWindowNetCore\Native\webwindow.win.native\Tester\Brauser.ico"
//let iconPath = @"D:\Projekte\WebWindowNetCore\Native\webwindow.win.native\Tester\Brauser.ico"
let iconPath = "/media/speicher/projekte/WebWindowNetCore/TestApp/icon.svg"
//let url = @"file://C:\Users\urieg\source\repos\WebWindowNetCore\WebRoot\index.html"
//let url = @"file://D:\Projekte\WebWindowNetCore\WebRoot\index.html"
//let url = @"file:///media/speicher/projekte/WebWindowNetCore/WebRoot/index.html"
// let url = "http://localhost:8080"
let url = "https://google.de"

let callback (text: string) = ()
let dropFilesCallback (text: string) = 
    ()

let configuration = { 
    defaultConfiguration () with
        Title = "Web brauser😎😎👌"
        Url = url
        IconPath = iconPath
        DebuggingEnabled = true
        Organization = "URiegel"
        Application = "TestBrauser"
        SaveWindowSettings = true
        FullScreenEnabled = true
        OnEvent = callback
        DropFiles = dropFilesCallback
}

initialize configuration

let menu = [ 
    Menu {
        Title = "&Datei"
        Items = [ 
            CmdItem { Title = "&Umbenennen"; Accelerator = Some "F2"; Action = onRename } 
            CmdItem { Title = "&Erweitertes Umbenennen"; Accelerator = Some "Strg+F2"; Action = onExtendedRename } 
            Separator 
            CmdItem { Title = "&Kopieren"; Accelerator = Some "F5"; Action = onCopy } 
            CmdItem { Title = "&Verschieben"; Accelerator = Some "F6"; Action = onMove } 
            CmdItem { Title = "&Löschen"; Accelerator = Some "Entf"; Action = onDelete } 
            Separator 
            CmdItem { Title = "&Ordner anlegen"; Accelerator = Some "F7"; Action = onCreatefolder } 
            Separator 
            CmdItem { Title = "&Eigenschaften"; Accelerator = Some "Alt+Eingabe"; Action = onProperties } 
            CmdItem { Title = "&Öffnen mit"; Accelerator = Some "Strg+Eingabe"; Action = onOpenWith } 
            Separator 
            CmdItem { Title = "&Beenden"; Accelerator = Some "Alt+F4"; Action = onClose } 
        ]
    } 
    Menu {
        Title = "&Navigation"
        Items = [ 
            CmdItem { Title = "&Favoriten"; Accelerator = Some "F1"; Action = onFavorites } 
            CmdItem { Title = "&Gleichen Ordner öffnen"; Accelerator = Some "F9"; Action = onAdaptPath } 
        ]
    }
    Menu {
        Title = "&Selektion"
        Items = [ 
            CmdItem { Title = "&Alles"; Accelerator = Some "Num +"; Action = onSelectAll } 
            CmdItem { Title = "Alle &deselektieren"; Accelerator = Some "Num -"; Action = onDeselectAll } 
        ]
    }
    Menu {
        Title = "&Ansicht"
        Items = [ 
            Checkbox { Title = "&Versteckte Dateien"; Accelerator = Some "Strg+H"; OnChecked = onHidden; SetCheckedFunction = Some setSetHidden } 
            CmdItem { Title = "&Aktualisieren"; Accelerator = Some "Strg+R"; Action = onRefresh } 
            Separator 
            Checkbox { Title = "&Vorschau"; Accelerator = Some "F3"; OnChecked = onPreview; SetCheckedFunction = None } 
            Separator 
            Menu {
                Title = "&Themen"
                Items = [
                    MenuGroup {
                        OnSelected = onTheme
                        SetSelected = None
                        Items = [
                            Radio { Title = "&Blau"; Accelerator = None; Key = "blau"} 
                            Radio { Title = "&Hellblau"; Accelerator = None; Key = "hellblau"} 
                            Radio { Title = "&Dunkel"; Accelerator = None; Key = "dunkel" } 
                        ]
                    }
                ]
            }
            Separator 
            Menu {
                Title = "&Zoomlevel"
                Items = [
                    MenuGroup {
                        OnSelected = onZoom
                        SetSelected = Some setSetZoom
                        Items = [ 
                            Radio { Title = "50%"; Accelerator = None; Key = 50 } 
                            Radio { Title = "75%"; Accelerator = None; Key = 75 } 
                            Radio { Title = "100%"; Accelerator = None; Key = 100 } 
                            Radio { Title = "150%"; Accelerator = None; Key = 150 } 
                            Radio { Title = "200%"; Accelerator = None; Key = 200 } 
                            Radio { Title = "250%"; Accelerator = None; Key = 250 } 
                            Radio { Title = "300%"; Accelerator = None; Key = 300 } 
                            Radio { Title = "350%"; Accelerator = None; Key = 350 } 
                            Radio { Title = "400%"; Accelerator = None; Key = 400 } 
                        ]
                    }
                ]
            }
            CmdItem { Title = "Voll&bild"; Accelerator = Some "F11"; Action = onFullscreen } 
            Separator 
            CmdItem { Title = "&Entwicklungswerkzeuge"; Accelerator = Some "F12"; Action = onDevTools } 
        ]
    }
]

setMenu menu
setHidden true
setZoom 100

execute () |> ignore


    //================================
    // TODO: Test seamless window

    //#include <string>
    //#include <vector>
    //#include <algorithm>
    //#include <map>
    //#include <type_traits>
    //#include <windows.h>
    //#include <windowsx.h>
    //#include <wrl.h>
    //#include <wil/com.h>
    //#include <WebView2.h>
    //#include <Dwmapi.h>
    
    //using namespace Microsoft::WRL;
    //using namespace std;
    
    //using EventCallback = add_pointer<void(const wchar_t*)>::type;
    //using OnMenuCallback = add_pointer<void()>::type;
    //using OnCheckedCallback = add_pointer<void(bool)>::type;
    
    //void Schreibwas(const wchar_t* text) {
    //    auto eventlog = OpenEventLog(NULL, L"Application");
    //    LPCTSTR errorMessage[1];
    //    *errorMessage = text;
    //    ReportEvent(eventlog, EVENTLOG_ERROR_TYPE, 0, 0, nullptr, 1, 0, errorMessage, nullptr);
    //}
    
    //struct MenuItemData {
    //    OnMenuCallback onMenu;
    //    OnCheckedCallback onChecked;
    //    bool checkable{ false };
    //    int groupCount{ 0 };
    //    int groupId{ 0 };
    //};
    
    //map<int, MenuItemData> menuItemDatas;
    
    //struct Configuration {
    //    const wchar_t* title{ nullptr };
    //    const wchar_t* url{ nullptr };
    //    const wchar_t* icon_path{ nullptr };
    //    bool debugging_enabled{ false };
    //    int debuggingPort{ 8888 };
    //    wchar_t* organization{ nullptr };
    //    wchar_t* application{ nullptr };
    //    bool save_window_settings{ false };
    //    bool full_screen_enabled{ false };
    //    EventCallback callback{ nullptr };
    //};
    
    //static wil::com_ptr<IWebView2WebView5> webviewWindow;
    //HWND mainWindow{ nullptr };
    //HACCEL hAccelTable{ nullptr };
    //HMENU menubar{ nullptr };
    //bool window_settings_enabled{ false };
    //bool is_fullscreen{ false };
    //wstring organization;
    //wstring application;
    //wstring reg_key;
    //const wchar_t* X = L"x";
    //const wchar_t* Y = L"y";
    //const wchar_t* IS_MAXIMIZED = L"IsMaximized";
    //const wchar_t* IS_MINIMIZED = L"isMinimized";
    //const wchar_t* WIDTH = L"width";
    //const wchar_t* HEIGHT = L"height";
    //EventCallback callback{ nullptr };
    
    //struct Window_settings {
    //    int x{ CW_USEDEFAULT };
    //    int y{ CW_USEDEFAULT };
    //    int width{ CW_USEDEFAULT };
    //    int height{ CW_USEDEFAULT };
    //    bool is_maximized{ false };
    //    bool isMinimized{ false };
    //};
    
    //void save_window_settings(HWND hWnd) {
    //    if (window_settings_enabled && !is_fullscreen) {
    //        HKEY key;
    //        DWORD disposition{ 0 };
    //        RegCreateKeyEx(HKEY_CURRENT_USER, reg_key.c_str(), 0, nullptr, REG_OPTION_NON_VOLATILE, KEY_ALL_ACCESS, nullptr, &key, &disposition);
    //        if (!IsZoomed(hWnd) && !IsIconic(hWnd)) {
    //            RECT rect{ 0 };
    //            GetWindowRect(hWnd, &rect);
    //            RegSetValueEx(key, X, 0, REG_DWORD, (BYTE*)&rect.left, 4);
    //            RegSetValueEx(key, Y, 0, REG_DWORD, (BYTE*)&rect.top, 4);
    //            int width = rect.right - rect.left;
    //            int height = rect.bottom - rect.top;
    //            RegSetValueEx(key, WIDTH, 0, REG_DWORD, (BYTE*)&width, 4);
    //            RegSetValueEx(key, HEIGHT, 0, REG_DWORD, (BYTE*)&height, 4);
    //            DWORD v{ 0 };
    //            RegSetValueEx(key, IS_MAXIMIZED, 0, REG_DWORD, (BYTE*)&v, 4);
    //            RegSetValueEx(key, IS_MINIMIZED, 0, REG_DWORD, (BYTE*)&v, 4);
    //        }
    //        if (IsZoomed(hWnd)) {
    //            DWORD v{ 1 };
    //            RegSetValueEx(key, IS_MAXIMIZED, 0, REG_DWORD, (BYTE*)&v, 4);
    //            v = 0;
    //            RegSetValueEx(key, IS_MINIMIZED, 0, REG_DWORD, (BYTE*)&v, 4);
    //        }
    //        if (IsIconic(hWnd)) {
    //            DWORD v{ 1 };
    //            RegSetValueEx(key, IS_MINIMIZED, 0, REG_DWORD, (BYTE*)&v, 4);
    //            v = 0;
    //            RegSetValueEx(key, IS_MAXIMIZED, 0, REG_DWORD, (BYTE*)&v, 4);
    //        }
    
    //        RegCloseKey(key);
    //    }
    //}
    
    //Window_settings get_window_settings() {
    //    Window_settings ws;
    //    if (window_settings_enabled) {
    //        HKEY key;
    //        DWORD disposition{ 0 };
    //        RegCreateKeyEx(HKEY_CURRENT_USER, reg_key.c_str(), 0, nullptr, REG_OPTION_NON_VOLATILE, KEY_ALL_ACCESS, nullptr, &key, &disposition);
    //        DWORD s{ 4 };
    //        DWORD t{ REG_DWORD };
    //        auto ret = RegQueryValueEx(key, X, 0, &t, (BYTE*)&ws.x, &s);
    //        if (ret == 0) {
    //            RegQueryValueEx(key, Y, 0, &t, (BYTE*)&ws.y, &s);
    //            RegQueryValueEx(key, WIDTH, 0, &t,(BYTE*)&ws.width, &s);
    //            RegQueryValueEx(key, HEIGHT, 0, &t, (BYTE*)&ws.height, &s);
    //            DWORD v;
    //            RegQueryValueEx(key, IS_MAXIMIZED, 0, &t, (BYTE*)&v, &s);
    //            ws.is_maximized = v == 1;
    //            RegQueryValueEx(key, IS_MINIMIZED, 0, &t, (BYTE*)&v, &s);
    //            ws.isMinimized = v == 1;
    //        }
    //        RegCloseKey(key);
    //    }
    //    return ws;
    //}
    
    //void doCommand(int cmd) {
    //    auto menuItemData = menuItemDatas[cmd];
    //    if (menuItemData.checkable) {
    //        auto state = GetMenuState(GetMenu(mainWindow), cmd, MF_BYCOMMAND);
    //        CheckMenuItem(GetMenu(mainWindow), cmd, state == MF_CHECKED ? MF_UNCHECKED : MF_CHECKED);
    //        menuItemData.onChecked(state != MF_CHECKED);
    //    }
    //    else if (menuItemData.groupCount) {
    //        auto first = cmd - menuItemData.groupId;
    //        CheckMenuRadioItem(GetMenu(mainWindow), first, first + menuItemData.groupCount - 1, cmd, MF_BYCOMMAND);
    //        menuItemData.onMenu();
    //    }
    //    else
    //        menuItemData.onMenu();
    
    //}
    
    //WNDPROC batProc = nullptr;
    
    //LRESULT CALLBACK BatnProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
    //{
    
    //    switch (uMsg)
    //    {
    //    case WM_NCHITTEST:
    //    {
    //        //do default processing, but allow resizing from top-border
    //        LRESULT result = DefWindowProc(hwnd, uMsg, wParam, lParam);
    //        if (result == HTCLIENT)
    //        {
    //            POINT pt = { GET_X_LPARAM(lParam), GET_Y_LPARAM(lParam) };
    //            ScreenToClient(hwnd, &pt);
    //            if (pt.y < 6) return HTTRANSPARENT;
    //        }
    //        return result;
    //    }
    //    }
    
    //    return CallWindowProc(batProc, hwnd, uMsg, wParam, lParam);
    //}
    
    //LRESULT Hookproc(int code, WPARAM wParam, LPARAM lParam) {
    
    //    auto asdas = (CWPRETSTRUCT*)lParam;
    //    if (asdas->message == WM_NCHITTEST) {
    //        Schreibwas(L"Hittet§");
    //        asdas->lResult = HTTRANSPARENT;
    //    }
    //    if (code >= 0)
    //        return CallNextHookEx(0, code, wParam, lParam);
    
    //    return CallNextHookEx(0, code, wParam, lParam);
    //}
    
    //LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam) {
    //    static RECT border_thickness;
    
    //    switch (message) {
    //    case WM_CREATE:
    //    {
    //        //find border thickness
    //        SetRectEmpty(&border_thickness);
    //        if (GetWindowLongPtr(hWnd, GWL_STYLE) & WS_THICKFRAME)
    //        {
    //            AdjustWindowRectEx(&border_thickness, GetWindowLongPtr(hWnd, GWL_STYLE) & ~WS_CAPTION, FALSE, NULL);
    //            border_thickness.left *= -1;
    //            border_thickness.top *= -1;
    //        }
    //        else if (GetWindowLongPtr(hWnd, GWL_STYLE) & WS_BORDER)
    //        {
    //            SetRect(&border_thickness, 1, 1, 1, 1);
    //        }
    
    //        MARGINS margins = { 0 };
    //        DwmExtendFrameIntoClientArea(hWnd, &margins);
    //        SetWindowPos(hWnd, NULL, 0, 0, 0, 0, SWP_SHOWWINDOW | SWP_NOMOVE | SWP_NOSIZE | SWP_FRAMECHANGED);
    //        break;
    //    }
    //    case WM_SIZE:
    //        if (webviewWindow != nullptr) {
    //            RECT bounds;
    //            GetClientRect(hWnd, &bounds);
    //            webviewWindow->put_Bounds(bounds);
    
    //            if (!batProc) {
    //                auto kind = GetFirstChild(hWnd);
    //                auto kind2 = GetFirstChild(kind);
    //                auto kind3 = GetFirstChild(kind2);
    //                auto kind4 = GetWindow(kind3, GW_HWNDNEXT);
    
    //                auto mod = LoadLibrary(L"NativeWinWebView");
    
    //                DWORD processId;
    //                DWORD thid = GetWindowThreadProcessId(kind2, &processId);
    
    //                auto hook = SetWindowsHookEx(WH_CALLWNDPROCRET, Hookproc, mod, thid);
    
    //                batProc = (WNDPROC)SetWindowLongPtr(kind, GWLP_WNDPROC, (LONG_PTR)BatnProc);
    //            }
    
    //        };
    //        break;
    //    case WM_SYSCOMMAND:
    //        switch (wParam)
    //        {
    //        case SC_MINIMIZE:
    //            save_window_settings(hWnd);
    //            break;
    //        case SC_MAXIMIZE:
    //            save_window_settings(hWnd);
    //            break;
    //        }        
    //        return DefWindowProc(hWnd, message, wParam, lParam);
    //    case WM_COMMAND:
    //        doCommand(LOWORD(wParam));
    //        break;
    //    case WM_APP + 1:
    //        CheckMenuItem(GetMenu(hWnd), (UINT)wParam, lParam ? MF_CHECKED : MF_UNCHECKED);
    //        break;
    //    case WM_APP + 2:
    //    {
    //        int cmdId = (int)wParam;
    //        int groupCount = LOWORD(lParam);
    //        int id = HIWORD(lParam);
    //        CheckMenuRadioItem(GetMenu(hWnd), cmdId, cmdId + groupCount - 1, cmdId + id, MF_BYCOMMAND);
    //    }
    //    break;
    
    //    case WM_NCACTIVATE:
    //        return 0;
    
    //    case WM_NCCALCSIZE:
    //        if (lParam)
    //        {
    //            NCCALCSIZE_PARAMS* sz = (NCCALCSIZE_PARAMS*)lParam;
    //            sz->rgrc[0].left += border_thickness.left;
    //            sz->rgrc[0].right -= border_thickness.right;
    //            sz->rgrc[0].bottom -= border_thickness.bottom;
    //            return 0;
    //        }
    //        break;
    
    //    case WM_NCHITTEST:
    //    {
    //        //do default processing, but allow resizing from top-border
    //        LRESULT result = DefWindowProc(hWnd, message, wParam, lParam);
    //        if (result == HTCLIENT)
    //        {
    //            POINT pt = { GET_X_LPARAM(lParam), GET_Y_LPARAM(lParam) };
    //            ScreenToClient(hWnd, &pt);
    //            if (pt.y < border_thickness.top) return HTTOP;
    //        }
    //        return result;
    //    }
    
    //    case WM_DESTROY:
    //        save_window_settings(hWnd);
    //        if (menubar)
    //            DestroyMenu(menubar);
    //        PostQuitMessage(0);
    //        break;
    //    default:
    //        return DefWindowProc(hWnd, message, wParam, lParam);
    //    }
    //    return 0;
    //}
    
    //auto load_icon(const wchar_t* icon_path) {
    //    return (HICON)LoadImage( // returns a HANDLE so we have to cast to HICON
    //        nullptr,             
    //        icon_path,   
    //        IMAGE_ICON,  
    //        0,           
    //        0,                
    //        LR_LOADFROMFILE | 
    //        LR_DEFAULTSIZE |    
    //        LR_SHARED         // let the system release the handle when it's no longer used
    //    );
    //}
    
    //HMENU menu{ nullptr };
    //WINDOWPLACEMENT previous_placement{ 0 };
    
    //void enter_fullscreen(HWND hWnd) {
    //    is_fullscreen = true;
    //    DWORD style = GetWindowLong(hWnd, GWL_STYLE);
    //    MONITORINFO monitor_info = { sizeof(monitor_info) };
    //    menu = GetMenu(hWnd);
    //    SetMenu(hWnd, nullptr);
    //    if (GetWindowPlacement(hWnd, &previous_placement) &&
    //        GetMonitorInfo(MonitorFromWindow(hWnd, MONITOR_DEFAULTTOPRIMARY), &monitor_info))
    //    {
    //        SetWindowLong(hWnd, GWL_STYLE, style & ~WS_OVERLAPPEDWINDOW);
    //        SetWindowPos(hWnd, HWND_TOP, monitor_info.rcMonitor.left, monitor_info.rcMonitor.top,
    //            monitor_info.rcMonitor.right - monitor_info.rcMonitor.left,
    //            monitor_info.rcMonitor.bottom - monitor_info.rcMonitor.top,
    //            SWP_NOOWNERZORDER | SWP_FRAMECHANGED);
    //    }
    //}
    
    //void exit_fullscreen(HWND hWnd) {
    //    is_fullscreen = false;
    //    DWORD style = GetWindowLong(hWnd, GWL_STYLE);
    //    SetMenu(hWnd, menu);
    //    SetWindowLong(hWnd, GWL_STYLE, style | WS_OVERLAPPEDWINDOW);
    //    SetWindowPlacement(hWnd, &previous_placement);
    //    SetWindowPos(hWnd, NULL, 0, 0, 0, 0,
    //        SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_NOOWNERZORDER | SWP_FRAMECHANGED);
    //}
    
    //auto GetKey(wstring accelerator) {
    //    if (accelerator == L"F1") return make_tuple(VK_F1, FVIRTKEY);
    //    if (accelerator == L"F2") return make_tuple(VK_F2, FVIRTKEY);
    //    if (accelerator == L"F3") return make_tuple(VK_F3, FVIRTKEY);
    //    if (accelerator == L"F4") return make_tuple(VK_F4, FVIRTKEY);
    //    if (accelerator == L"F5") return make_tuple(VK_F5, FVIRTKEY);
    //    if (accelerator == L"F6") return make_tuple(VK_F6, FVIRTKEY);
    //    if (accelerator == L"F7") return make_tuple(VK_F7, FVIRTKEY);
    //    if (accelerator == L"F8") return make_tuple(VK_F8, FVIRTKEY);
    //    if (accelerator == L"F9") return make_tuple(VK_F9, FVIRTKEY);
    //    if (accelerator == L"F10") return make_tuple(VK_F10, FVIRTKEY);
    //    if (accelerator == L"F11") return make_tuple(VK_F11, FVIRTKEY);
    //    if (accelerator == L"F12") return make_tuple(VK_F12, FVIRTKEY);
    //    else return make_tuple((int)accelerator[0], 0);
    //}
    
    //auto getVirtualKey(wstring accelerator) {
    //    if (accelerator == L"Alt") return FALT | FVIRTKEY;
    //    if (accelerator == L"Ctrl") return FCONTROL | FVIRTKEY;
    //    if (accelerator == L"Strg") return FCONTROL | FVIRTKEY;
    //    else return 0;
    //}
    
    
    //struct Accelerator {
    //    int cmd;
    //    int key;
    //    BYTE virtkey;
    
    //    Accelerator(int cmd, wstring accelerator) : cmd(cmd) {
    //        auto pos = accelerator.find('+');
    //        if (pos == string::npos) {
    //            tie(key, virtkey) = GetKey(accelerator);
    //        }
    //        else {
    //            auto virt = accelerator.substr(0, pos);
    //            auto k = accelerator.substr(pos + 1);
    //            int _;
    //            tie(key, _) = GetKey(k);
    //            virtkey = getVirtualKey(virt);
    //        }
    //    }
    //};
    
    //vector<Accelerator> accelerators;
    
    //void initializeWindow(Configuration configuration) {
    //    window_settings_enabled = configuration.save_window_settings;
    //    organization = window_settings_enabled ? configuration.organization : L""s; 
    //    application = window_settings_enabled ? configuration.application : L""s;
    //    callback = configuration.callback;
    //    reg_key = LR"(Software\)"s + organization + LR"(\)"s + application;
    //    auto settings = get_window_settings();
    //    auto instance = LoadLibrary(L"NativeWinWebView");
    //    auto window_class = L"NativeWebViewClass";
    //    WNDCLASSEXW wcex;
    //    wcex.cbSize = sizeof(WNDCLASSEX);
    //    wcex.style = CS_HREDRAW | CS_VREDRAW;
    //    wcex.lpfnWndProc = WndProc;
    //    wcex.cbClsExtra = 0;
    //    wcex.cbWndExtra = 0;
    //    wcex.hInstance = instance;
    //    wcex.hIcon = load_icon(configuration.icon_path);
    //    wcex.hCursor = LoadCursor(nullptr, IDC_ARROW);
    //    wcex.hbrBackground = (HBRUSH)(COLOR_WINDOW + 1);
    //    wcex.lpszMenuName = nullptr;
    //    wcex.lpszClassName = window_class;
    //    wcex.hIconSm = nullptr;
    //    auto atom = RegisterClassExW(&wcex);
    
    //    mainWindow = CreateWindowW(window_class, configuration.title, WS_POPUP | WS_THICKFRAME | WS_MAXIMIZEBOX,
    //        settings.x, settings.y, settings.width, settings.height, nullptr, nullptr, instance, nullptr);
    
    //    if (!mainWindow)
    //        return;
    
    //    ShowWindow(mainWindow,
    //        settings.is_maximized
    //        ? SW_SHOWMAXIMIZED
    //        : settings.isMinimized
    //        ? SW_SHOWMINIMIZED
    //        : SW_SHOWDEFAULT);
    //    UpdateWindow(mainWindow);
    //    auto url = wstring(configuration.url);
    //    auto dev_tools_enabled = configuration.debugging_enabled;
    //    auto fullscreen_enabled = configuration.full_screen_enabled;
    
    //    CreateWebView2EnvironmentWithDetails(nullptr, nullptr, nullptr, 
    //        Callback<IWebView2CreateWebView2EnvironmentCompletedHandler>([url, dev_tools_enabled, fullscreen_enabled](HRESULT result, IWebView2Environment* env) -> HRESULT {
    //            env->CreateWebView(mainWindow, Callback<IWebView2CreateWebViewCompletedHandler>(
    //                    [url, dev_tools_enabled, fullscreen_enabled](HRESULT result, IWebView2WebView* webview) -> HRESULT {
    //                if (webview != nullptr)
    //                    webview->QueryInterface(IID_PPV_ARGS(&webviewWindow));
    
    //                IWebView2Settings* settings;
    //                webviewWindow->get_Settings(&settings);
    //                settings->put_IsScriptEnabled(TRUE);
    //                settings->put_AreDefaultScriptDialogsEnabled(TRUE);
    //                settings->put_IsWebMessageEnabled(TRUE);
    //                settings->put_AreDevToolsEnabled(dev_tools_enabled);
    
    //                IWebView2Settings2* settings2;
    //                settings->QueryInterface(IID_PPV_ARGS(&settings2));
    //                settings2->put_AreDefaultContextMenusEnabled(FALSE);
    
    //                // Resize WebView to fit the bounds of the parent window
    //                RECT bounds;
    //                GetClientRect(mainWindow, &bounds);
    //                webviewWindow->put_Bounds(bounds);
    
    //                // Schedule an async task to navigate to Bing
    //                webviewWindow->Navigate(url.c_str());
    //                webviewWindow->MoveFocus(WEBVIEW2_MOVE_FOCUS_REASON_PROGRAMMATIC);
                    
    //                if (fullscreen_enabled) {
    //                    (webviewWindow->add_ContainsFullScreenElementChanged(Callback<IWebView2ContainsFullScreenElementChangedEventHandler>(
    //                        [](IWebView2WebView5* sender, IUnknown* args) -> HRESULT {
    //                            BOOL contains_fullscreen{ FALSE };
    //                            sender->get_ContainsFullScreenElement(&contains_fullscreen);
    //                            if (contains_fullscreen)
    //                                enter_fullscreen(mainWindow);
    //                            else
    //                                exit_fullscreen(mainWindow);
    //                            return S_OK;
    //                        })
    //                    .Get(),
    //                    nullptr));
    //                }
    
    //                // Step 6 - Communication between host and web content
    //                // Set an event handler for the host to return received message back to the web content
    //                if (callback) {
    //                    EventRegistrationToken token;
    //                    webviewWindow->add_WebMessageReceived(Callback<IWebView2WebMessageReceivedEventHandler>(
    //                        [](auto webview, auto args) -> HRESULT {
    //                            PWSTR message;
    //                            args->get_WebMessageAsString(&message);
    //                            callback(message);
    //                            CoTaskMemFree(message);
    //                            return S_OK;
    //                        }).Get(), &token);
    //                }
    
    //                //// Schedule an async task to add initialization script that
    //                //// 1) Add an listener to print message from the host
    //                //// 2) Post document URL to the host
    //                webviewWindow->AddScriptToExecuteOnDocumentCreated(
    //LR"(var webWindowNetCore = (function() {
    //    var callback
    
    //    window.chrome.webview.addEventListener('message', event => {
    //        if (callback)
    //            callback(event.data)
    //    })
    
    //    function postMessage(msg) {
    //        window.chrome.webview.postMessage(msg)                    
    //    }
    
    //    function setCallback(callbackToHost) {
    //        callback = callbackToHost
    //    }
    
    //    return {
    //        setCallback,
    //        postMessage
    //    }
    //})())", nullptr);
    
    //                EventRegistrationToken acceleratorKeyPressedToken;
    //                webviewWindow->add_AcceleratorKeyPressed(
    //                    Callback<IWebView2AcceleratorKeyPressedEventHandler>(
    //                        [](IWebView2WebView* sender, IWebView2AcceleratorKeyPressedEventArgs* args)-> HRESULT {
    //                            WEBVIEW2_KEY_EVENT_TYPE type;
    //                            args->get_KeyEventType(&type);
    //                            // We only care about key down events.
    //                            if (type == WEBVIEW2_KEY_EVENT_TYPE_KEY_DOWN || type == WEBVIEW2_KEY_EVENT_TYPE_SYSTEM_KEY_DOWN) {
    //                                UINT key;
    //                                args->get_VirtualKey(&key);
    //                                if (key != VK_CONTROL && key != VK_MENU) {
    //                                    auto altPressed = GetKeyState(VK_MENU) == -128 || GetKeyState(VK_MENU) == -127;
    //                                    auto ctrlPressed = GetKeyState(VK_CONTROL) == -128 || GetKeyState(VK_CONTROL) == -127;
    //                                    char baffer[2000];
    //                                    wsprintfA(baffer, "Kie: %d %d %d\n", key, altPressed, ctrlPressed);
    
    //                                    auto cmd = find_if(accelerators.begin(), accelerators.end(), [key, altPressed, ctrlPressed](Accelerator n) {
    //                                        if (n.key == key) {
    //                                            bool ctrl = (n.virtkey & FCONTROL) == FCONTROL;
    //                                            bool alt = (n.virtkey & FALT) == FALT;
    //                                            return (ctrl == ctrlPressed && alt == altPressed);
    //                                        }
    //                                        else
    //                                            return false;
    //                                    });
    //                                    if (cmd != end(accelerators))
    //                                        doCommand(cmd->cmd);
    //                                }
    //                                    // Check if the key is one we want to handle.
    //                            //    if (std::function<void()> action =
    //                            //        m_appWindow->GetAcceleratorKeyFunction(key))
    //                            //    {
    //                            //        // Keep the browser from handling this key, whether it's autorepeated or
    //                            //        // not.
    //                            //        CHECK_FAILURE(args->Handle(TRUE));
    
    //                            //        // Filter out autorepeated keys.
    //                            //        WEBVIEW2_PHYSICAL_KEY_STATUS status;
    //                            //        CHECK_FAILURE(args->get_PhysicalKeyStatus(&status));
    //                            //        if (!status.WasKeyDown)
    //                            //        {
    //                            //            // Perform the action asynchronously to avoid blocking the
    //                            //            // browser process's event queue.
    //                            //            m_appWindow->RunAsync(action);
    //                            //        }
    //                            //    }
    //                            }
    //                            return S_OK;
    //                        }).Get(), &acceleratorKeyPressedToken);
    
    //                return S_OK;
    //            }).Get());
    //        return S_OK;
    //    }).Get());
    //}
    
    //void sendToBrowser(const wchar_t* text) {
    //    webviewWindow->PostWebMessageAsString(text);
    //}
    
    //enum class MenuItemType
    //{
    //    MenuItem,
    //    Separator,
    //    Checkbox,
    //    Radio
    //};
    
    //struct MenuItem {
    //    MenuItemType menuItemType;
    //    const wchar_t* title;
    //    const wchar_t* accelerator;
    //    OnMenuCallback callback;
    //    OnCheckedCallback onChecked;
    //    int groupCount;
    //    int groupId;
    //};
    
    //int cmdIdSeed{ 0 };
    
    //HMENU addMenu(const wchar_t* title) {
    //    if (!menubar) 
    //        menubar = CreateMenu();
    //    auto menu = CreateMenu();
    //    AppendMenuW(menubar, MF_POPUP, (UINT_PTR)menu, title);
    //    return menu;
    //}
    
    //HMENU addSubmenu(const wchar_t* title, HMENU parentMenu) {
    //    auto subMenu = CreateMenu();
    //    AppendMenuW(parentMenu, MF_POPUP, (UINT_PTR)subMenu, title);
    //    return subMenu;
    //}
    
    //int setMenuItem(HMENU menu, MenuItem menuItem) {
    //    auto cmdId = ++cmdIdSeed;
    //    wstring text = menuItem.title ? menuItem.title : L""s;
    //    if (menuItem.accelerator) {
    //        text += L"\t"s + menuItem.accelerator;
    //        accelerators.emplace_back(cmdId, menuItem.accelerator);
    //    }
    //    switch (menuItem.menuItemType) {
    //    case MenuItemType::MenuItem:
    //        AppendMenuW(menu, MF_STRING, cmdId, text.c_str());
    //        menuItemDatas[cmdId] = { menuItem.callback };
    //        break;
    //    case MenuItemType::Separator:
    //        AppendMenuW(menu, MF_SEPARATOR, 0, nullptr);
    //        break;
    //    case MenuItemType::Checkbox:
    //        AppendMenuW(menu, MF_STRING, cmdId, text.c_str());
    //        text += L"\t"s + menuItem.accelerator;
    //        menuItemDatas[cmdId] = { nullptr, menuItem.onChecked, true };
    //        CheckMenuItem(menu, cmdId, MF_UNCHECKED);
    //        break;
    //    case MenuItemType::Radio:
    //        AppendMenuW(menu, MF_STRING, cmdId, text.c_str());
    //        menuItemDatas[cmdId] = { menuItem.callback, nullptr, false, menuItem.groupCount, menuItem.groupId };
    //        if (menuItem.groupCount == menuItem.groupId + 1)
    //            CheckMenuRadioItem(menu, cmdId - menuItem.groupCount + 1, cmdId, cmdId - menuItem.groupCount + 1, MF_BYCOMMAND);
    //        break;
    //    }
    //    return cmdId;
    //}
    
    //bool getMenuItemChecked(int cmdId) {
    //    return GetMenuState(GetMenu(mainWindow), cmdId, MF_BYCOMMAND) == MF_CHECKED;
    //}
    
    //void setMenuItemChecked(int cmdId, bool checked) {
    //    auto menu = GetMenu(mainWindow);
    //    if (menu)
    //        CheckMenuItem(menu, cmdId, checked ? MF_CHECKED : MF_UNCHECKED);
    //    else
    //        PostMessage(mainWindow, WM_APP + 1, cmdId, checked);
    //}
    
    //void setMenuItemSelected(int cmdId, int groupCount, int id) {
    //    auto menu = GetMenu(mainWindow);
    //    if (menu)
    //        CheckMenuRadioItem(menu, cmdId, cmdId + groupCount - 1, cmdId + id, MF_BYCOMMAND);
    //    else
    //        PostMessage(mainWindow, WM_APP + 2, cmdId, MAKELPARAM(groupCount, id));
    //}
        
    //int execute() {
    //    if (menubar)
    //        SetMenu(mainWindow, menubar);
    
    //    auto size = (int)accelerators.size();
    //    auto accel = new ACCEL[size];
    
    //    auto createAccelerator = [accel, idx = 0](const Accelerator& n) mutable {
    //        accel[idx].cmd = n.cmd;
    //        accel[idx].key = n.key;
    //        accel[idx++].fVirt = n.virtkey;
    //    };
    
    //    for_each(accelerators.begin(), accelerators.end(), createAccelerator);
    //    //azel[1].cmd = 2;
    //    //azel[1].key = VK_F6;
    //    //azel[1].fVirt = FVIRTKEY;
    //    //azel[2].cmd = 3;
    //    //azel[3].key = 'O';
    //    //azel[3].fVirt = FCONTROL | FVIRTKEY;
    
    //    hAccelTable = CreateAcceleratorTable(accel, size);
        
    //    MSG msg;
    
    //    // Main message loop:
    //    while (GetMessage(&msg, nullptr, 0, 0)) {
    //        if (!TranslateAccelerator(msg.hwnd, hAccelTable, &msg)) {
    //            TranslateMessage(&msg);
    //            DispatchMessage(&msg);
    //        }
    //    }
    
    //    return (int)msg.wParam;
    //}
    
  