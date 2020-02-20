#include <windows.h>
#include <stdlib.h>
#include <string>
#include <tchar.h>
#include <wrl.h>
#include <wil/com.h>
// include WebView2 header
#include "WebView2.h"

using namespace Microsoft::WRL; 

// Pointer to WebView window
static wil::com_ptr<IWebView2WebView> webviewWindow;

#define MAX_LOADSTRING 100

// Global Variables:
HINSTANCE hInst;                                // current instance
WCHAR szTitle[MAX_LOADSTRING];                  // The title bar text
WCHAR szWindowClass[MAX_LOADSTRING];            // the main window class name

// Forward declarations of functions included in this code module:
ATOM                MyRegisterClass(HINSTANCE hInstance);
BOOL                InitInstance(HINSTANCE, int);
LRESULT CALLBACK    WndProc(HWND, UINT, WPARAM, LPARAM);
INT_PTR CALLBACK    About(HWND, UINT, WPARAM, LPARAM);

int APIENTRY wWinMain(_In_ HINSTANCE hInstance,
                     _In_opt_ HINSTANCE hPrevInstance,
                     _In_ LPWSTR    lpCmdLine,
                     _In_ int       nCmdShow)
{
    UNREFERENCED_PARAMETER(hPrevInstance);
    UNREFERENCED_PARAMETER(lpCmdLine);

    // TODO: Place code here.

    // Initialize global strings
    wcscpy(szTitle, L"WebBrauser");
    wcscpy(szWindowClass, L"IDC_NATIVEWINWEBVIEW");
    MyRegisterClass(hInstance);

    // Perform application initialization:
    if (!InitInstance (hInstance, nCmdShow))
    {
        return FALSE;
    }

    MSG msg;

    // Main message loop:
    while (GetMessage(&msg, nullptr, 0, 0))
    {
        TranslateMessage(&msg);
        DispatchMessage(&msg);
    }

    return (int) msg.wParam;
}



//
//  FUNCTION: MyRegisterClass()
//
//  PURPOSE: Registers the window class.
//
ATOM MyRegisterClass(HINSTANCE hInstance)
{
    WNDCLASSEXW wcex;

    wcex.cbSize = sizeof(WNDCLASSEX);

    wcex.style          = CS_HREDRAW | CS_VREDRAW;
    wcex.lpfnWndProc    = WndProc;
    wcex.cbClsExtra     = 0;
    wcex.cbWndExtra     = 0;
    wcex.hInstance      = hInstance;
    wcex.hIcon          = nullptr;
    wcex.hCursor        = LoadCursor(nullptr, IDC_ARROW);
    wcex.hbrBackground  = (HBRUSH)(COLOR_WINDOW+1);
    wcex.lpszMenuName   = nullptr;
    wcex.lpszClassName  = szWindowClass;
    wcex.hIconSm        = nullptr;

    return RegisterClassExW(&wcex);
}

//
//   FUNCTION: InitInstance(HINSTANCE, int)
//
//   PURPOSE: Saves instance handle and creates main window
//
//   COMMENTS:
//
//        In this function, we save the instance handle in a global variable and
//        create and display the main program window.
//
BOOL InitInstance(HINSTANCE hInstance, int nCmdShow)
{
   hInst = hInstance; // Store instance handle in our global variable

   HWND hWnd = CreateWindowW(szWindowClass, szTitle, WS_OVERLAPPEDWINDOW,
      CW_USEDEFAULT, 0, CW_USEDEFAULT, 0, nullptr, nullptr, hInstance, nullptr);

   if (!hWnd)
   {
      return FALSE;
   }

   ShowWindow(hWnd, nCmdShow);
   UpdateWindow(hWnd);

   // Step 3 - Create a single WebView within the parent window
   // Locate the browser and set up the environment for WebView
   CreateWebView2EnvironmentWithDetails(nullptr, nullptr, nullptr,
       Callback<IWebView2CreateWebView2EnvironmentCompletedHandler>([hWnd](HRESULT result, IWebView2Environment* env) -> HRESULT {

           // Create a WebView, whose parent is the main window hWnd
           env->CreateWebView(hWnd, Callback<IWebView2CreateWebViewCompletedHandler>(
               [hWnd](HRESULT result, IWebView2WebView* webview) -> HRESULT {
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
                   webviewWindow->Navigate(L"https://cas-ws200110.caseris.intern/web/timio/timio.html");

                   // Step 4 - Navigation events


                   // Step 5 - Scripting


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

   return TRUE;
}

//
//  FUNCTION: WndProc(HWND, UINT, WPARAM, LPARAM)
//
//  PURPOSE: Processes messages for the main window.
//
//  WM_COMMAND  - process the application menu
//  WM_PAINT    - Paint the main window
//  WM_DESTROY  - post a quit message and return
//
//
LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    switch (message)
    {
    case WM_PAINT:
        {
            PAINTSTRUCT ps;
            HDC hdc = BeginPaint(hWnd, &ps);
            // TODO: Add any drawing code that uses hdc here...
            EndPaint(hWnd, &ps);
        }
        break;
    case WM_DESTROY:
        PostQuitMessage(0);
        break;
    default:
        return DefWindowProc(hWnd, message, wParam, lParam);
    }
    return 0;
}

// Message handler for about box.
INT_PTR CALLBACK About(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam)
{
    UNREFERENCED_PARAMETER(lParam);
    switch (message)
    {
    case WM_INITDIALOG:
        return (INT_PTR)TRUE;

    case WM_COMMAND:
        if (LOWORD(wParam) == IDOK || LOWORD(wParam) == IDCANCEL)
        {
            EndDialog(hDlg, LOWORD(wParam));
            return (INT_PTR)TRUE;
        }
        break;
    }
    return (INT_PTR)FALSE;
}
