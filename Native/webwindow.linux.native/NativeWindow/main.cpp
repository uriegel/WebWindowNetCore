#include "mainwindow.h"
#include <QApplication>

#define DLL_PUBLIC __attribute__ ((visibility ("default")))

struct Configuration {
    const char* title;
    const char* url;
};

extern "C" DLL_PUBLIC void initialize_window(Configuration configuration);
extern "C" DLL_PUBLIC int execute();

QApplication* app{nullptr};
MainWindow *window{nullptr};

void create_window(Configuration configuration) {
    window = new MainWindow(configuration.title, configuration.url);
    window->resize(1000, 600);
    window->show();
}

void initialize_window(Configuration configuration) {
    int c{0};
    app = new QApplication(c, nullptr);
    create_window(configuration);
}

int execute() {
    auto ret = app->exec();
    delete window;
    delete app;
    return ret;
}

int main(int argc, char *argv[])
{
    int c{0};
    app = new QApplication(c, nullptr);
    MainWindow w(" Der Brauser>", "https://www.google.de");
    w.resize(800, 600);
    w.show();
    auto ret = app->exec();
    delete app;
    return ret;
}
