#pragma once

struct Configuration {
    const wchar_t* title;
    const wchar_t* url;
    const wchar_t* icon_path;
    bool debuggingEnabled;
    int debuggingPort;
    wchar_t* organization;
    wchar_t* application;
    bool save_window_settings;
    bool fullScreenEnabled;
};

void create_window(Configuration configuration);
