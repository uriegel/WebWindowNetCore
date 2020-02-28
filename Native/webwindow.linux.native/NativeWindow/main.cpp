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

int setGroupedMenuItem(QMenu* menu, MenuItem menu_item, QActionGroup* group) {
   return window->setGroupedMenuItem(menu, menu_item, group);
}

QActionGroup* createMenuGroup() {
    return window->createMenuGroup();
}

void setMenuItemChecked(int cmdId, bool checked) {
    window->setMenuItemChecked(cmdId, checked);
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

int main() {
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
    configuration.onEvent = nullptr;
    initializeWindow(configuration);

    auto menu = addMenu("&Datei");
    auto id = setMenuItem(menu, {
        MenuItemType::MenuItem,
        "&Neu",
        "Ctrl+N"
    });
    id = setMenuItem(menu, {
        MenuItemType::MenuItem,
        "&Kopieren",
        "F5"
    });
    setMenuItem(menu, {
        MenuItemType::Separator,
        "",
        ""
    });
    id = setMenuItem(menu, {
        MenuItemType::MenuItem,
        "&Beenden",
        "Alt+F4"
    });
    menu = addMenu("&Ansicht");
    id = setMenuItem(menu, {
        MenuItemType::Checkbox,
        "&Versteckte Dateien",
        "Ctrl+H"
    });

    auto hiddenID = id;
    setMenuItem(menu, {
        MenuItemType::Separator,
        "",
        ""
    });
    auto submenu = addSubmenu("&Themen", menu);
    auto group = createMenuGroup();
    id = setGroupedMenuItem(submenu, {
        MenuItemType::Checkbox,
        "&Rot",
        nullptr
    }, group);
    id = setGroupedMenuItem(submenu, {
        MenuItemType::Checkbox,
        "&Blau",
        nullptr
    }, group);
    id = setGroupedMenuItem(submenu, {
        MenuItemType::Checkbox,
        "&Dunkel",
        nullptr
    }, group);
    setMenuItem(menu, {
        MenuItemType::Separator,
        "",
        ""
    });
    id = setMenuItem(menu, {
        MenuItemType::MenuItem,
        "&Vorschau",
        "F3"
    });

    setMenuItemChecked(hiddenID, true);


    return execute();
}

