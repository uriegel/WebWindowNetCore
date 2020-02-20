#pragma once

struct Configuration {
    const wchar_t* title;
    unsigned short second;
};

void create_window(Configuration configuration);
