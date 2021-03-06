#include <string>
#include <vector>
#include <algorithm>
#include <map>
#include <type_traits>
#include <windows.h>
#include <wrl.h>
#include <windowsx.h>
#include <wil/com.h>
#include <WebView2.h>
#include "dwmapi.h"
using namespace Microsoft::WRL;
using namespace std;

using EventCallback = add_pointer<void(const wchar_t*)>::type;
using DropFilesCallback = add_pointer<void(const wchar_t*)>::type;
using OnMenuCallback = add_pointer<void()>::type;
using OnCheckedCallback = add_pointer<void(bool)>::type;

struct MenuItemData {
    OnMenuCallback onMenu;
    OnCheckedCallback onChecked;
    bool checkable{ false };
    int groupCount{ 0 };
    int groupId{ 0 };
};

map<int, MenuItemData> menuItemDatas;
DropFilesCallback dropFilesCallback{ nullptr };

struct Configuration {
    const wchar_t* title{ nullptr };
    const wchar_t* url{ L"about:blank" };
    const wchar_t* icon_path{ nullptr };
    bool debugging_enabled{ false };
    int debuggingPort{ 8888 };
    wchar_t* organization{ nullptr };
    wchar_t* application{ nullptr };
    bool save_window_settings{ false };
    bool full_screen_enabled{ false };
    EventCallback callback{ nullptr };
    DropFilesCallback dropFilesCallback{ nullptr };
};

static wil::com_ptr<ICoreWebView2> webviewWindow;
static wil::com_ptr<ICoreWebView2Controller> webviewController;

HWND mainWindow{ nullptr };
HACCEL hAccelTable{ nullptr };
HMENU menubar{ nullptr };
bool window_settings_enabled{ false };
bool is_fullscreen{ false };
wstring organization;
wstring application;
wstring reg_key;
const wchar_t* X = L"x";
const wchar_t* Y = L"y";
const wchar_t* IS_MAXIMIZED = L"IsMaximized";
const wchar_t* IS_MINIMIZED = L"isMinimized";
const wchar_t* WIDTH = L"width";
const wchar_t* HEIGHT = L"height";
EventCallback callback{ nullptr };

struct Window_settings {
    int x{ CW_USEDEFAULT };
    int y{ CW_USEDEFAULT };
    int width{ CW_USEDEFAULT };
    int height{ CW_USEDEFAULT };
    bool is_maximized{ false };
    bool isMinimized{ false };
};

void save_window_settings(HWND hWnd) {
    if (window_settings_enabled && !is_fullscreen) {
        HKEY key;
        DWORD disposition{ 0 };
        RegCreateKeyEx(HKEY_CURRENT_USER, reg_key.c_str(), 0, nullptr, REG_OPTION_NON_VOLATILE, KEY_ALL_ACCESS, nullptr, &key, &disposition);
        if (!IsZoomed(hWnd) && !IsIconic(hWnd)) {
            RECT rect{ 0 };
            GetWindowRect(hWnd, &rect);
            RegSetValueEx(key, X, 0, REG_DWORD, (BYTE*)&rect.left, 4);
            RegSetValueEx(key, Y, 0, REG_DWORD, (BYTE*)&rect.top, 4);
            int width = rect.right - rect.left;
            int height = rect.bottom - rect.top;
            RegSetValueEx(key, WIDTH, 0, REG_DWORD, (BYTE*)&width, 4);
            RegSetValueEx(key, HEIGHT, 0, REG_DWORD, (BYTE*)&height, 4);
            DWORD v{ 0 };
            RegSetValueEx(key, IS_MAXIMIZED, 0, REG_DWORD, (BYTE*)&v, 4);
            RegSetValueEx(key, IS_MINIMIZED, 0, REG_DWORD, (BYTE*)&v, 4);
        }
        if (IsZoomed(hWnd)) {
            DWORD v{ 1 };
            RegSetValueEx(key, IS_MAXIMIZED, 0, REG_DWORD, (BYTE*)&v, 4);
            v = 0;
            RegSetValueEx(key, IS_MINIMIZED, 0, REG_DWORD, (BYTE*)&v, 4);
        }
        if (IsIconic(hWnd)) {
            DWORD v{ 1 };
            RegSetValueEx(key, IS_MINIMIZED, 0, REG_DWORD, (BYTE*)&v, 4);
            v = 0;
            RegSetValueEx(key, IS_MAXIMIZED, 0, REG_DWORD, (BYTE*)&v, 4);
        }

        RegCloseKey(key);
    }
}

Window_settings get_window_settings() {
    Window_settings ws;
    if (window_settings_enabled) {
        HKEY key;
        DWORD disposition{ 0 };
        RegCreateKeyEx(HKEY_CURRENT_USER, reg_key.c_str(), 0, nullptr, REG_OPTION_NON_VOLATILE, KEY_ALL_ACCESS, nullptr, &key, &disposition);
        DWORD s{ 4 };
        DWORD t{ REG_DWORD };
        auto ret = RegQueryValueEx(key, X, 0, &t, (BYTE*)&ws.x, &s);
        if (ret == 0) {
            RegQueryValueEx(key, Y, 0, &t, (BYTE*)&ws.y, &s);
            RegQueryValueEx(key, WIDTH, 0, &t, (BYTE*)&ws.width, &s);
            RegQueryValueEx(key, HEIGHT, 0, &t, (BYTE*)&ws.height, &s);
            DWORD v;
            RegQueryValueEx(key, IS_MAXIMIZED, 0, &t, (BYTE*)&v, &s);
            ws.is_maximized = v == 1;
            RegQueryValueEx(key, IS_MINIMIZED, 0, &t, (BYTE*)&v, &s);
            ws.isMinimized = v == 1;
        }
        RegCloseKey(key);
    }
    return ws;
}

void doCommand(int cmd) {
    auto menuItemData = menuItemDatas[cmd];
    if (menuItemData.checkable) {
        auto state = GetMenuState(GetMenu(mainWindow), cmd, MF_BYCOMMAND);
        CheckMenuItem(GetMenu(mainWindow), cmd, state == MF_CHECKED ? MF_UNCHECKED : MF_CHECKED);
        menuItemData.onChecked(state != MF_CHECKED);
    }
    else if (menuItemData.groupCount) {
        auto first = cmd - menuItemData.groupId;
        CheckMenuRadioItem(GetMenu(mainWindow), first, first + menuItemData.groupCount - 1, cmd, MF_BYCOMMAND);
        menuItemData.onMenu();
    }
    else
        menuItemData.onMenu();

}

struct DropTarget : public IDropTarget
{
    HRESULT __stdcall DragEnter(IDataObject* pDataObj, DWORD grfKeyState, POINTL pt, DWORD* pdwEffect)
    {
        return S_OK;
    }

    HRESULT __stdcall DragOver(DWORD grfKeyState, POINTL pt, DWORD* pdwEffect)
    {
        return S_OK;
    }

    HRESULT __stdcall DragLeave(void)
    {
        return S_OK;
    }

    HRESULT __stdcall Drop(IDataObject* pDataObj, DWORD grfKeyState, POINTL pt, DWORD* pdwEffect)
    {
        FORMATETC fmt = { CF_HDROP, NULL, DVASPECT_CONTENT, -1, TYMED_HGLOBAL };
        STGMEDIUM stg = { TYMED_HGLOBAL };
        auto hr = pDataObj->GetData(&fmt, &stg);
        auto hDrop = (HDROP)GlobalLock(stg.hGlobal);
        auto uNumFiles = DragQueryFile(hDrop, -1, nullptr, 0);
        wstring result;
        for (UINT i = 0; i < uNumFiles; i++) {
            wchar_t text[MAX_PATH];
            DragQueryFile(hDrop, i, text, MAX_PATH);
            result += text;
            if (i < uNumFiles - 1)
                result += L"|";
        }
        dropFilesCallback(result.c_str());
        return S_OK;
    }

    HRESULT __stdcall QueryInterface(REFIID riid, void** ppvObject)
    {
        return E_NOINTERFACE;
    }

    ULONG __stdcall AddRef(void) {
        return 1;
    }

    ULONG __stdcall Release(void) {
        return 0;
    }
};

DropTarget dropTarget;

LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam) {
    switch (message) {
    case WM_CREATE:
    {
        // This plays together with WM_NCALCSIZE.
        MARGINS m{ 0, 0, 0, 1 };
        DwmExtendFrameIntoClientArea(hWnd, &m);
        // Force the system to recalculate NC area (making it send WM_NCCALCSIZE).
        SetWindowPos(hWnd, nullptr, 0, 0, 0, 0, SWP_NOZORDER | SWP_NOOWNERZORDER | SWP_NOMOVE | SWP_NOSIZE | SWP_FRAMECHANGED);
        return TRUE;
    }
    case WM_NCCALCSIZE:
        // Returning 0 from the message when wParam is TRUE removes the standard
        // frame, but keeps the window shadow.
        if (wParam == TRUE)
        {
            SetWindowLong(hWnd, 0 /*DWL_MSGRESULT*/, 0);
            return TRUE;
        }
        return FALSE;
    case WM_NCHITTEST:
        POINT pt;
        pt.x = LOWORD(lParam);
        pt.y = HIWORD(lParam);
        ::ScreenToClient(hWnd, &pt);

        RECT rcClient;
        ::GetClientRect(hWnd, &rcClient);

        //The upper left corner, judging whether it is in the upper left corner, is to see whether the current coordinates are within the range dragged on the left side, and within the range dragged on the upper side, the other angles are judged similarly.
        if (pt.x < rcClient.left + 5 && pt.y < rcClient.top + 5)
        {
            return HTTOPLEFT;
        }
        //top right corner
        else if (pt.x > rcClient.right - 5 && pt.y < rcClient.top + 5)
        {
            return HTTOPRIGHT;
        }
        //lower left corner
        else if (pt.x < rcClient.left + 5 && pt.y > rcClient.bottom - 5)
        {
            return HTBOTTOMLEFT;
        }
        //bottom right corner
        else if (pt.x > rcClient.right - 5 && pt.y > rcClient.bottom - 5)
        {
            return HTBOTTOMRIGHT;
        }
        //left
        else if (pt.x < rcClient.left + 5)
        {
            return HTLEFT;
        }
        //right
        else if (pt.x > rcClient.right - 5)
        {
            return HTRIGHT;
        }
        // 
        else if (pt.y < rcClient.top + 5)
        {
            return HTTOP;
        }
        //Bottom
        else if (pt.y > rcClient.bottom - 5)
        {
            return HTBOTTOM; //The above four are the top, bottom, left and right sides
        }
        //title 
        else
        {
            // other range whole window hit as caption
            return HTCAPTION;
        }
        break;
    case WM_SIZE:
        if (webviewWindow != nullptr) {
            RECT bounds;
            GetClientRect(hWnd, &bounds);
            bounds.bottom -= 2;
            bounds.top += 10;
            bounds.left += 2;
            bounds.right -= 2;
            webviewController->put_Bounds(bounds);
        };
        break;
    case WM_SYSCOMMAND:
        switch (wParam)
        {
        case SC_MINIMIZE:
            save_window_settings(hWnd);
            break;
        case SC_MAXIMIZE:
            save_window_settings(hWnd);
            break;
        }
        return DefWindowProc(hWnd, message, wParam, lParam);
    case WM_COMMAND:
        doCommand(LOWORD(wParam));
        break;
    case WM_SETFOCUS:
        if (webviewController != nullptr)
            webviewController->MoveFocus(COREWEBVIEW2_MOVE_FOCUS_REASON_NEXT);
        break;
    case WM_APP + 1:
        CheckMenuItem(GetMenu(hWnd), (UINT)wParam, lParam ? MF_CHECKED : MF_UNCHECKED);
        break;
    case WM_APP + 2:
    {
        int cmdId = (int)wParam;
        int groupCount = LOWORD(lParam);
        int id = HIWORD(lParam);
        CheckMenuRadioItem(GetMenu(hWnd), cmdId, cmdId + groupCount - 1, cmdId + id, MF_BYCOMMAND);
    }
    break;
    case WM_APP + 3:
        webviewWindow->OpenDevToolsWindow();
        break;

    case WM_DESTROY:
        save_window_settings(hWnd);
        if (menubar)
            DestroyMenu(menubar);
        PostQuitMessage(0);
        break;
    default:
        return DefWindowProc(hWnd, message, wParam, lParam);
    }
    return 0;
}

auto load_icon(const wchar_t* icon_path) {
    return (HICON)LoadImage( // returns a HANDLE so we have to cast to HICON
        nullptr,
        icon_path,
        IMAGE_ICON,
        0,
        0,
        LR_LOADFROMFILE |
        LR_DEFAULTSIZE |
        LR_SHARED         // let the system release the handle when it's no longer used
    );
}

HMENU menu{ nullptr };
WINDOWPLACEMENT previous_placement{ 0 };

void enter_fullscreen(HWND hWnd) {
    is_fullscreen = true;
    DWORD style = GetWindowLong(hWnd, GWL_STYLE);
    MONITORINFO monitor_info = { sizeof(monitor_info) };
    menu = GetMenu(hWnd);
    SetMenu(hWnd, nullptr);
    if (GetWindowPlacement(hWnd, &previous_placement) &&
        GetMonitorInfo(MonitorFromWindow(hWnd, MONITOR_DEFAULTTOPRIMARY), &monitor_info))
    {
        SetWindowLong(hWnd, GWL_STYLE, style & ~WS_OVERLAPPEDWINDOW);
        SetWindowPos(hWnd, HWND_TOP, monitor_info.rcMonitor.left, monitor_info.rcMonitor.top,
            monitor_info.rcMonitor.right - monitor_info.rcMonitor.left,
            monitor_info.rcMonitor.bottom - monitor_info.rcMonitor.top,
            SWP_NOOWNERZORDER | SWP_FRAMECHANGED);
    }
}

void exit_fullscreen(HWND hWnd) {
    is_fullscreen = false;
    DWORD style = GetWindowLong(hWnd, GWL_STYLE);
    SetMenu(hWnd, menu);
    SetWindowLong(hWnd, GWL_STYLE, style | WS_OVERLAPPEDWINDOW);
    SetWindowPlacement(hWnd, &previous_placement);
    SetWindowPos(hWnd, NULL, 0, 0, 0, 0,
        SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_NOOWNERZORDER | SWP_FRAMECHANGED);
}

auto GetKey(wstring accelerator) {
    if (accelerator == L"F1") return make_tuple(VK_F1, FVIRTKEY);
    if (accelerator == L"F2") return make_tuple(VK_F2, FVIRTKEY);
    if (accelerator == L"F3") return make_tuple(VK_F3, FVIRTKEY);
    if (accelerator == L"F4") return make_tuple(VK_F4, FVIRTKEY);
    if (accelerator == L"F5") return make_tuple(VK_F5, FVIRTKEY);
    if (accelerator == L"F6") return make_tuple(VK_F6, FVIRTKEY);
    if (accelerator == L"F7") return make_tuple(VK_F7, FVIRTKEY);
    if (accelerator == L"F8") return make_tuple(VK_F8, FVIRTKEY);
    if (accelerator == L"F9") return make_tuple(VK_F9, FVIRTKEY);
    if (accelerator == L"F10") return make_tuple(VK_F10, FVIRTKEY);
    if (accelerator == L"F11") return make_tuple(VK_F11, FVIRTKEY);
    if (accelerator == L"F12") return make_tuple(VK_F12, FVIRTKEY);
    else return make_tuple((int)accelerator[0], 0);
}

auto getVirtualKey(wstring accelerator) {
    if (accelerator == L"Alt") return FALT | FVIRTKEY;
    if (accelerator == L"Ctrl") return FCONTROL | FVIRTKEY;
    if (accelerator == L"Strg") return FCONTROL | FVIRTKEY;
    else return 0;
}


struct Accelerator {
    int cmd;
    int key;
    BYTE virtkey;

    Accelerator(int cmd, wstring accelerator) : cmd(cmd) {
        auto pos = accelerator.find('+');
        if (pos == string::npos) {
            tie(key, virtkey) = GetKey(accelerator);
        }
        else {
            auto virt = accelerator.substr(0, pos);
            auto k = accelerator.substr(pos + 1);
            int _;
            tie(key, _) = GetKey(k);
            virtkey = getVirtualKey(virt);
        }
    }
};

vector<Accelerator> accelerators;

void initializeWindow(Configuration configuration) {
    dropFilesCallback = configuration.dropFilesCallback;
    if (dropFilesCallback)
        OleInitialize(0);

    window_settings_enabled = configuration.save_window_settings;
    organization = window_settings_enabled ? configuration.organization : L""s;
    application = window_settings_enabled ? configuration.application : L""s;
    callback = configuration.callback;
    reg_key = LR"(Software\)"s + organization + LR"(\)"s + application;
    auto settings = get_window_settings();
    auto instance = LoadLibrary(L"NativeWinWebView");
    auto window_class = L"NativeWebViewClass";
    WNDCLASSEXW wcex;
    wcex.cbSize = sizeof(WNDCLASSEX);
    wcex.style = CS_HREDRAW | CS_VREDRAW;
    wcex.lpfnWndProc = WndProc;
    wcex.cbClsExtra = 0;
    wcex.cbWndExtra = 0;
    wcex.hInstance = instance;
    wcex.hIcon = load_icon(configuration.icon_path);



    wcex.hCursor = LoadCursor(nullptr, IDC_ARROW);
    wcex.hbrBackground = (HBRUSH)(COLOR_WINDOW + 1);
    wcex.lpszMenuName = nullptr;
    wcex.lpszClassName = window_class;

    wcex.hIconSm = nullptr;
    auto atom = RegisterClassExW(&wcex);

    mainWindow = CreateWindowW(window_class, configuration.title, WS_OVERLAPPEDWINDOW,
        settings.x, settings.y, settings.width, settings.height, nullptr, nullptr, instance, nullptr);

    if (!mainWindow)
        return;

    ShowWindow(mainWindow,
        settings.is_maximized
        ? SW_SHOWMAXIMIZED
        : settings.isMinimized
        ? SW_SHOWMINIMIZED
        : SW_SHOWDEFAULT);
    UpdateWindow(mainWindow);

    auto url = wstring(configuration.url);
    auto dev_tools_enabled = configuration.debugging_enabled;
    auto fullscreen_enabled = configuration.full_screen_enabled;

    auto result = CreateCoreWebView2EnvironmentWithOptions(nullptr, nullptr, nullptr,
        Callback<ICoreWebView2CreateCoreWebView2EnvironmentCompletedHandler>(
            [url, dev_tools_enabled](HRESULT result, ICoreWebView2Environment* env) -> HRESULT {
                env->CreateCoreWebView2Controller(mainWindow, Callback<ICoreWebView2CreateCoreWebView2ControllerCompletedHandler>(
                    [url, dev_tools_enabled](HRESULT result, ICoreWebView2Controller* controller) -> HRESULT {
                        if (controller != nullptr) {
                            webviewController = controller;
                            webviewController->get_CoreWebView2(&webviewWindow);
                        }

                        // Add a few settings for the webview
                        // The demo step is redundant since the values are the default settings
                        ICoreWebView2Settings* settings;
                        webviewWindow->get_Settings(&settings);
                        settings->put_IsScriptEnabled(TRUE);
                        settings->put_AreDefaultScriptDialogsEnabled(TRUE);
                        settings->put_IsWebMessageEnabled(TRUE);
                        settings->put_AreDevToolsEnabled(dev_tools_enabled);

                        // Resize WebView to fit the bounds of the parent window
                        RECT bounds;
                        GetClientRect(mainWindow, &bounds);
                        bounds.left += 10;
                        bounds.top += 2;
                        bounds.right -= 2;
                        bounds.bottom -= 2;
                        webviewController->put_Bounds(bounds);


                        // Schedule an async task to navigate to Bing
                        webviewWindow->Navigate(url.c_str());
                        PostMessage(mainWindow, WM_SETFOCUS, 0, 0);
                        webviewController->MoveFocus(COREWEBVIEW2_MOVE_FOCUS_REASON_NEXT);
                        // Step 4 - Navigation events
                        EventRegistrationToken token;
                        webviewWindow->add_NavigationCompleted(Callback<ICoreWebView2NavigationCompletedEventHandler>(
                            [](ICoreWebView2* webview, ICoreWebView2NavigationCompletedEventArgs* args) -> HRESULT {
                                PostMessage(mainWindow, WM_SETFOCUS, 0, 0);
                                return S_OK;
                            }).Get(), &token);

                        //webviewWindow->add_AcceleratorKeyPressed(
                            //    Callback<IWebView2AcceleratorKeyPressedEventHandler>(
                            //        [](IWebView2WebView* sender, IWebView2AcceleratorKeyPressedEventArgs* args)-> HRESULT {
                            //            WEBVIEW2_KEY_EVENT_TYPE type;
                            //            args->get_KeyEventType(&type);
                            //            // We only care about key down events.
                            //            if (type == WEBVIEW2_KEY_EVENT_TYPE_KEY_DOWN || type == WEBVIEW2_KEY_EVENT_TYPE_SYSTEM_KEY_DOWN) {

                        // Step 5 - Scripting

                        // Step 6 - Communication between host and web content
                        //if (callback) {
                        //    EventRegistrationToken token;
                        //    webviewWindow->add_WebMessageReceived(Callback<IWebView2WebMessageReceivedEventHandler>(
                        //        [](auto webview, auto args) -> HRESULT {
                        //            PWSTR message;
                        //            args->get_WebMessageAsString(&message);
                        //            callback(message);
                        //            CoTaskMemFree(message);
                        //            return S_OK;
                        //        }).Get(), &token);
                        //}

                        if (dropFilesCallback) {
                            auto webHost = GetFirstChild(mainWindow);
                            auto chromium = GetFirstChild(webHost);
                            RevokeDragDrop(chromium);
                            HRESULT af = RegisterDragDrop(chromium, &dropTarget);
                            int t = 0;
                        }

                        return S_OK;



                        //IWebView2Settings2* settings2;
                        //settings->QueryInterface(IID_PPV_ARGS(&settings2));
                        //settings2->put_AreDefaultContextMenusEnabled(FALSE);

                        //if (fullscreen_enabled) {
                        //    (webviewWindow->add_ContainsFullScreenElementChanged(Callback<IWebView2ContainsFullScreenElementChangedEventHandler>(
                        //        [](IWebView2WebView5* sender, IUnknown* args) -> HRESULT {
                        //            BOOL contains_fullscreen{ FALSE };
                        //            sender->get_ContainsFullScreenElement(&contains_fullscreen);
                        //            if (contains_fullscreen)
                        //                enter_fullscreen(mainWindow);
                        //            else
                        //                exit_fullscreen(mainWindow);
                        //            return S_OK;
                        //        })
                        //    .Get(),
                        //    nullptr));
                        //}



        //                //// Schedule an async task to add initialization script that
        //                //// 1) Add an listener to print message from the host
        //                //// 2) Post document URL to the host
        //                webviewWindow->AddScriptToExecuteOnDocumentCreated(
        //LR"(var webWindowNetCore = (function() {
        //    var callback
        //
        //    window.chrome.webview.addEventListener('message', event => {
        //        if (callback)
        //            callback(event.data)
        //    })
        //
        //    function postMessage(msg) {
        //        window.chrome.webview.postMessage(msg)                    
        //    }
        //
        //    function setCallback(callbackToHost) {
        //        callback = callbackToHost
        //    }
        //
        //    return {
        //        setCallback,
        //        postMessage
        //    }
        //})())", nullptr);
        //
                        //EventRegistrationToken acceleratorKeyPressedToken;
                        //webviewWindow->add_AcceleratorKeyPressed(
                        //    Callback<IWebView2AcceleratorKeyPressedEventHandler>(
                        //        [](IWebView2WebView* sender, IWebView2AcceleratorKeyPressedEventArgs* args)-> HRESULT {
                        //            WEBVIEW2_KEY_EVENT_TYPE type;
                        //            args->get_KeyEventType(&type);
                        //            // We only care about key down events.
                        //            if (type == WEBVIEW2_KEY_EVENT_TYPE_KEY_DOWN || type == WEBVIEW2_KEY_EVENT_TYPE_SYSTEM_KEY_DOWN) {
                        //                UINT key;
                        //                args->get_VirtualKey(&key);
                        //                if (key != VK_CONTROL && key != VK_MENU) {
                        //                    auto altPressed = GetKeyState(VK_MENU) == -128 || GetKeyState(VK_MENU) == -127;
                        //                    auto ctrlPressed = GetKeyState(VK_CONTROL) == -128 || GetKeyState(VK_CONTROL) == -127;
                        //                    char baffer[2000];
                        //                    wsprintfA(baffer, "Kie: %d %d %d\n", key, altPressed, ctrlPressed);

                        //                    auto cmd = find_if(accelerators.begin(), accelerators.end(), [key, altPressed, ctrlPressed](Accelerator n) {
                        //                        if (n.key == key) {
                        //                            bool ctrl = (n.virtkey & FCONTROL) == FCONTROL;
                        //                            bool alt = (n.virtkey & FALT) == FALT;
                        //                            return (ctrl == ctrlPressed && alt == altPressed);
                        //                        }
                        //                        else
                        //                            return false;
                        //                    });
                        //                    if (cmd != end(accelerators))
                        //                        doCommand(cmd->cmd);
                        //                }
                        //                    // Check if the key is one we want to handle.
                        //            //    if (std::function<void()> action =
                        //            //        m_appWindow->GetAcceleratorKeyFunction(key))
                        //            //    {
                        //            //        // Keep the browser from handling this key, whether it's autorepeated or
                        //            //        // not.
                        //            //        CHECK_FAILURE(args->Handle(TRUE));

                        //            //        // Filter out autorepeated keys.
                        //            //        WEBVIEW2_PHYSICAL_KEY_STATUS status;
                        //            //        CHECK_FAILURE(args->get_PhysicalKeyStatus(&status));
                        //            //        if (!status.WasKeyDown)
                        //            //        {
                        //            //            // Perform the action asynchronously to avoid blocking the
                        //            //            // browser process's event queue.
                        //            //            m_appWindow->RunAsync(action);
                        //            //        }
                        //            //    }
                        //            }
                        //            return S_OK;
                        //        }).Get(), &acceleratorKeyPressedToken);

                        return S_OK;
                    }).Get());
                return S_OK;
            }).Get());
    auto aua = result;
}

void sendToBrowser(const wchar_t* text) {
    webviewWindow->PostWebMessageAsString(text);
}

enum class MenuItemType
{
    MenuItem,
    Separator,
    Checkbox,
    Radio
};

struct MenuItem {
    MenuItemType menuItemType;
    const wchar_t* title;
    const wchar_t* accelerator;
    OnMenuCallback callback;
    OnCheckedCallback onChecked;
    int groupCount;
    int groupId;
};

int cmdIdSeed{ 0 };

HMENU addMenu(const wchar_t* title) {
    if (!menubar)
        menubar = CreateMenu();
    auto menu = CreateMenu();
    AppendMenuW(menubar, MF_POPUP, (UINT_PTR)menu, title);
    return menu;
}

HMENU addSubmenu(const wchar_t* title, HMENU parentMenu) {
    auto subMenu = CreateMenu();
    AppendMenuW(parentMenu, MF_POPUP, (UINT_PTR)subMenu, title);
    return subMenu;
}

int setMenuItem(HMENU menu, MenuItem menuItem) {
    auto cmdId = ++cmdIdSeed;
    wstring text = menuItem.title ? menuItem.title : L""s;
    if (menuItem.accelerator) {
        text += L"\t"s + menuItem.accelerator;
        accelerators.emplace_back(cmdId, menuItem.accelerator);
    }
    switch (menuItem.menuItemType) {
    case MenuItemType::MenuItem:
        AppendMenuW(menu, MF_STRING, cmdId, text.c_str());
        menuItemDatas[cmdId] = { menuItem.callback };
        break;
    case MenuItemType::Separator:
        AppendMenuW(menu, MF_SEPARATOR, 0, nullptr);
        break;
    case MenuItemType::Checkbox:
        AppendMenuW(menu, MF_STRING, cmdId, text.c_str());
        text += L"\t"s + menuItem.accelerator;
        menuItemDatas[cmdId] = { nullptr, menuItem.onChecked, true };
        CheckMenuItem(menu, cmdId, MF_UNCHECKED);
        break;
    case MenuItemType::Radio:
        AppendMenuW(menu, MF_STRING, cmdId, text.c_str());
        menuItemDatas[cmdId] = { menuItem.callback, nullptr, false, menuItem.groupCount, menuItem.groupId };
        if (menuItem.groupCount == menuItem.groupId + 1)
            CheckMenuRadioItem(menu, cmdId - menuItem.groupCount + 1, cmdId, cmdId - menuItem.groupCount + 1, MF_BYCOMMAND);
        break;
    }
    return cmdId;
}

bool getMenuItemChecked(int cmdId) {
    return GetMenuState(GetMenu(mainWindow), cmdId, MF_BYCOMMAND) == MF_CHECKED;
}

void setMenuItemChecked(int cmdId, bool checked) {
    auto menu = GetMenu(mainWindow);
    if (menu)
        CheckMenuItem(menu, cmdId, checked ? MF_CHECKED : MF_UNCHECKED);
    else
        PostMessage(mainWindow, WM_APP + 1, cmdId, checked);
}

void setMenuItemSelected(int cmdId, int groupCount, int id) {
    auto menu = GetMenu(mainWindow);
    if (menu)
        CheckMenuRadioItem(menu, cmdId, cmdId + groupCount - 1, cmdId + id, MF_BYCOMMAND);
    else
        PostMessage(mainWindow, WM_APP + 2, cmdId, MAKELPARAM(groupCount, id));
}

int execute() {
    if (menubar)
        SetMenu(mainWindow, menubar);

    auto size = (int)accelerators.size();
    auto accel = new ACCEL[size];

    auto createAccelerator = [accel, idx = 0](const Accelerator& n) mutable {
        accel[idx].cmd = n.cmd;
        accel[idx].key = n.key;
        accel[idx++].fVirt = n.virtkey;
    };

    for_each(accelerators.begin(), accelerators.end(), createAccelerator);
    //azel[1].cmd = 2;
    //azel[1].key = VK_F6;
    //azel[1].fVirt = FVIRTKEY;
    //azel[2].cmd = 3;
    //azel[3].key = 'O';
    //azel[3].fVirt = FCONTROL | FVIRTKEY;

    hAccelTable = CreateAcceleratorTable(accel, size);

    MSG msg;

    // Main message loop:
    while (GetMessage(&msg, nullptr, 0, 0)) {
        if (!TranslateAccelerator(msg.hwnd, hAccelTable, &msg)) {
            TranslateMessage(&msg);
            DispatchMessage(&msg);
        }
    }
    if (dropFilesCallback)
        OleUninitialize();

    return (int)msg.wParam;
}

void closeWindow() {
    DestroyWindow(mainWindow);
}

void showDevTools() {
    PostMessage(mainWindow, WM_APP + 3, 0, 0);
}

void showFullscreen(bool fullscreen) {
    if (fullscreen)
        enter_fullscreen(mainWindow);
    else
        exit_fullscreen(mainWindow);
}

int WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPSTR lpCmdLine, int nShowCmd) {
    Configuration configuration{
        L"Testbrauser",
        L"https://google.de"
    };
    initializeWindow(configuration);
    execute();
    return 0;
}
