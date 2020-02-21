#include <string>
#include <windows.h>
#include <wrl.h>
#include <wil/com.h>
#include <WebView2.h>
#include "WebView.h"
using namespace Microsoft::WRL;
using namespace std;

static wil::com_ptr<IWebView2WebView> webviewWindow;

LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam) {
    switch (message) {
    case WM_SIZE:
        if (webviewWindow != nullptr) {
            RECT bounds;
            GetClientRect(hWnd, &bounds);
            webviewWindow->put_Bounds(bounds);
        };
        break;
    case WM_DESTROY:
        PostQuitMessage(0);
        break;
    default:
        return DefWindowProc(hWnd, message, wParam, lParam);
    }
    return 0;
}

void create_window(Configuration configuration) {
    auto instance = LoadLibrary(L"NativeWinWebView");
    auto window_class = L"NativeWebViewClass";
    WNDCLASSEXW wcex;
    wcex.cbSize = sizeof(WNDCLASSEX);
    wcex.style = CS_HREDRAW | CS_VREDRAW;
    wcex.lpfnWndProc = WndProc;
    wcex.cbClsExtra = 0;
    wcex.cbWndExtra = 0;
    wcex.hInstance = instance;
    wcex.hIcon = nullptr;
    wcex.hCursor = LoadCursor(nullptr, IDC_ARROW);
    wcex.hbrBackground = (HBRUSH)(COLOR_WINDOW + 1);
    wcex.lpszMenuName = nullptr;
    wcex.lpszClassName = window_class;
    wcex.hIconSm = nullptr;
    auto atom = RegisterClassExW(&wcex);

    auto hWnd = CreateWindowW(window_class, configuration.title, WS_OVERLAPPEDWINDOW,
        CW_USEDEFAULT, 0, CW_USEDEFAULT, 0, nullptr, nullptr, instance, nullptr);

    if (!hWnd)
        return;

    ShowWindow(hWnd, SW_SHOWDEFAULT);
    UpdateWindow(hWnd);
    auto url = wstring(configuration.url);

    // Step 3 - Create a single WebView within the parent window
    // Locate the browser and set up the environment for WebView
    CreateWebView2EnvironmentWithDetails(nullptr, nullptr, nullptr,
        Callback<IWebView2CreateWebView2EnvironmentCompletedHandler>([hWnd, url](HRESULT result, IWebView2Environment* env) -> HRESULT {

        // Create a WebView, whose parent is the main window hWnd
        env->CreateWebView(hWnd, Callback<IWebView2CreateWebViewCompletedHandler>(
            [hWnd, url](HRESULT result, IWebView2WebView* webview) -> HRESULT {
                if (webview != nullptr) {
                    webviewWindow = webview;
                }

                // Add a few settings for the webview
                // this is a redundant demo step as they are the default settings values
                IWebView2Settings* Settings;
                webviewWindow->get_Settings(&Settings);
                Settings->put_IsScriptEnabled(TRUE);
                Settings->put_AreDefaultScriptDialogsEnabled(TRUE);
                Settings->put_IsWebMessageEnabled(TRUE);

                // Resize WebView to fit the bounds of the parent window
                RECT bounds;
                GetClientRect(hWnd, &bounds);
                webviewWindow->put_Bounds(bounds);

                // Schedule an async task to navigate to Bing
                webviewWindow->Navigate(url.c_str());

                // Step 6 - Communication between host and web content
                 // Set an event handler for the host to return received message back to the web content
                EventRegistrationToken token;
                webviewWindow->add_WebMessageReceived(Callback<IWebView2WebMessageReceivedEventHandler>(
                    [](IWebView2WebView* webview, IWebView2WebMessageReceivedEventArgs* args) -> HRESULT {
                        PWSTR message;
                        args->get_WebMessageAsString(&message);
                        // processMessage(&message);
                        webview->PostWebMessageAsString(message);
                        CoTaskMemFree(message);
                        return S_OK;
                    }).Get(), &token);

                //// Schedule an async task to add initialization script that
                //// 1) Add an listener to print message from the host
                //// 2) Post document URL to the host
                //webviewWindow->AddScriptToExecuteOnDocumentCreated(
                //	L"window.chrome.webview.addEventListener(\'message\', event => alert(event.data));" \
 					//	L"window.chrome.webview.postMessage(window.document.URL);",
                        //	nullptr);

//						EventRegistrationToken token;
//						webviewWindow->add_WebMessageReceived(Callback<IWebView2WebMessageReceivedEventHandler>(
//							[](IWebView2WebView* webview, IWebView2WebMessageReceivedEventArgs* args) -> HRESULT {
//								PWSTR message;
//								args->get_WebMessageAsString(&message);
//								// processMessage(&message);
////						webview->PostWebMessageAsString(message);
//								CoTaskMemFree(message);
//								return S_OK;
//							}).Get(), &token);

                return S_OK;
            }).Get());
        return S_OK;
        }).Get());
}