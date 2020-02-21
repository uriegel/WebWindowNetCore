#pragma once

struct Configuration {
    const wchar_t* title;
    const wchar_t* url;
};

void create_window(Configuration configuration);
