#include "mainwindow.h"
#include <QApplication>
using namespace std;

#define DLL_PUBLIC __attribute__ ((visibility ("default")))

extern "C" DLL_PUBLIC void initialize_window(Configuration configuration);
extern "C" DLL_PUBLIC int execute();
extern "C" DLL_PUBLIC void send_to_browser(const char* text);

QApplication* app{nullptr};
MainWindow *window{nullptr};

void create_window(Configuration configuration) {
    window = new MainWindow(configuration);
    window->show();
}

QString create_debugging_arg(int port) {
    return "--remote-debugging-port=" + QString::number(port > 0 ? port : 8888);
}

void initialize_window(Configuration configuration) {
    int c = configuration.debugging_enabled ? 2 : 0;
    char* args[2];
    args[0] = (char*)"WebWindow";
    auto arg = create_debugging_arg(configuration.debugging_port).toUtf8();
    args[1] = (char*)(const char*)arg;
    char **argv = configuration.debugging_enabled ? args : nullptr;
    app = new QApplication(c, argv);
    create_window(configuration);
}

int execute() {
    auto ret = app->exec();
    delete window;
    delete app;
    return ret;
}

void send_to_browser(const char* text) {
    window->send_to_browser(text);
}

int main()
{
    bool debug_mode = true;
    int c = debug_mode ? 2 : 0;
    char* args[2];
    args[0] = (char*)"WebWindow";
    args[1] = (char*)"--remote-debugging-port=8888";
    char **argv = debug_mode ? args : nullptr;
    app = new QApplication(c, argv);

    auto configuration = Configuration();
    configuration.title = "Der Brauser";
        //"https://www.google.de";
    configuration.url = "file:///media/speicher/projekte/WebWindowNetCore/WebRoot/index.html";
    configuration.icon_path = "/home/uwe/Dokumente/icon.svg";
    configuration.debugging_enabled = true;
    configuration.organization = "uriegel";
    configuration.application ="brauser tester";
    configuration.save_window_settings = true;
    configuration.fullscreen_enabled = true;
    configuration.callback = nullptr;

    MainWindow w(configuration);
    w.show();
    auto ret = app->exec();
    delete app;
    return ret;
}
