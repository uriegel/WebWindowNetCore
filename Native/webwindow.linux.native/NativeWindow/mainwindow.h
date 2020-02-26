#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QMainWindow>
#include <QtWebEngineWidgets/QtWebEngineWidgets>

using Callback_ptr = std::add_pointer<void(const char* text)>::type;

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
    Callback_ptr callback{nullptr};
};

enum class MenuItemType
{
    MenuItem,
    Checkbox,
    Separator,
};

struct MenuItem {
    MenuItemType menuItemType;
    const char* title;
    const char* accelerator;
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

    void initializeScript(Callback_ptr callback);
    void send_to_browser(const char* text);
    QMenu* add_menu(const char* title, QMenu* parent = nullptr);
    QActionGroup* createMenuGroup();
    int set_menu_item(QMenu* menu, Menu_item menu_item);
    int setGroupedMenuItem(QMenu* menu, Menu_item menu_item, QActionGroup* group);

public slots:
    void postMessage(const QString& msg);
    void action(QAction* action);

private:
    QWebEngineView* webView;
    QString organization;
    QString application;
    Callback_ptr callback{nullptr};
    QMap<int, QAction*> checkableMenuItems;
};
#endif // MAINWINDOW_H
