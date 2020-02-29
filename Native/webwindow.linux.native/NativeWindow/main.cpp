#include "mainwindow.h"
#include <QApplication>
using namespace std;

#define DLL_PUBLIC __attribute__ ((visibility ("default")))

extern "C" DLL_PUBLIC void initializeWindow(Configuration configuration);
extern "C" DLL_PUBLIC int execute();
extern "C" DLL_PUBLIC void sendToBrowser(const char* text);
extern "C" DLL_PUBLIC QMenu* addMenu(const char* title);
extern "C" DLL_PUBLIC QMenu* addSubmenu(const char* title, QMenu* parent);
extern "C" DLL_PUBLIC int setMenuItem(QMenu* menu, MenuItem menuItem);
extern "C" DLL_PUBLIC void setMenuItemChecked(int cmdId, bool checked);
extern "C" DLL_PUBLIC void setMenuItemSelected (int cmdId, int groupCount, int id);

QApplication* app{nullptr};
MainWindow *window{nullptr};

void create_window(Configuration configuration) {
    window = new MainWindow(configuration);
    window->show();
}

QMenu* addMenu(const char* title) {
    return window->add_menu(title);
}

QMenu* addSubmenu(const char* title, QMenu* parent) {
    return window->add_menu(title, parent);
}

int setMenuItem(QMenu* menu, MenuItem menu_item) {
   return window->set_menu_item(menu, menu_item);
}

void setMenuItemChecked(int cmdId, bool checked) {
    window->setMenuItemChecked(cmdId, checked);
}

void setMenuItemSelected (int cmdId, int, int id) {
    window->setMenuItemChecked(cmdId + id, true);
}

QString create_debugging_arg(int port) {
    return "--remote-debugging-port=" + QString::number(port > 0 ? port : 8888);
}

void initializeWindow(Configuration configuration) {
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

void sendToBrowser(const char* text) {
    window->send_to_browser(text);
}

