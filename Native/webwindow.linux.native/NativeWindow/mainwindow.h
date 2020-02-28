#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QMainWindow>
#include <QtWebEngineWidgets/QtWebEngineWidgets>

using EventCallback = std::add_pointer<void(const char* text)>::type;
using OnMenuCallback = std::add_pointer<void()>::type;
using OnCheckedCallback = std::add_pointer<void(bool)>::type;

struct Configuration {
    const char* title;
    const char* url;
    const char* icon_path;
    bool debugging_enabled;
    int debugging_port;
    const char* organization;
    const char* application;
    bool save_window_settings;
    bool fullscreen_enabled;
    EventCallback onEvent{nullptr};
};

enum class MenuItemType
{
    MenuItem,
    Separator,
    Checkbox,
    Radio
};

struct MenuItem {
    MenuItemType menuItemType;
    const char* title;
    const char* accelerator;
    OnMenuCallback onMenu;
    OnCheckedCallback onChecked;
};

class WebEngineView : public QWebEngineView {
    Q_OBJECT
public:
    WebEngineView(QWidget *parent = nullptr) : QWebEngineView(parent) {}
protected:
    void contextMenuEvent(QContextMenuEvent*) override {}
};

class MainWindow : public QMainWindow
{
    Q_OBJECT

public:
    MainWindow(const Configuration& configuration, QWidget *parent = nullptr);
    ~MainWindow();

    void acceptFullScreen(QWebEngineFullScreenRequest request);

    void initializeScript(EventCallback onEvent);
    void send_to_browser(const char* text);
    QMenu* add_menu(const char* title, QMenu* parent = nullptr);
    QActionGroup* createMenuGroup();
    int set_menu_item(QMenu* menu, MenuItem menu_item);
    int setGroupedMenuItem(QMenu* menu, MenuItem menu_item, QActionGroup* group);
    void setMenuItemChecked(int cmdId, bool checked);

public slots:
    void postMessage(const QString& msg);
    void action(QAction* action);

private:
    QWebEngineView* webView;
    QString organization;
    QString application;
    EventCallback onEvent{nullptr};
    QMap<int, QAction*> checkableMenuItems;
};
#endif // MAINWINDOW_H
