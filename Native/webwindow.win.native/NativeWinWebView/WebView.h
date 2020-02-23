#pragma once
#include <type_traits>

using callback_ptr = std::add_pointer<void(const wchar_t* text)>::type;

struct Configuration {
    const wchar_t* title{ nullptr };
    const wchar_t* url{ nullptr };
    const wchar_t* icon_path{ nullptr };
    bool debugging_enabled{ false };
    int debuggingPort{ 8888 };
    wchar_t* organization{ nullptr };
    wchar_t* application{ nullptr };
    bool save_window_settings{ false };
    bool full_screen_enabled{ false };
    callback_ptr callback{ nullptr };
};

void create_window(Configuration configuration);
