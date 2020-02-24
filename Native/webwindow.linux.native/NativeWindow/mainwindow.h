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
public slots:
    void postMessage(const QString& msg);
    void action(QAction* action);

private:
    QWebEngineView* webView;
    QString organization;
    QString application;
    Callback_ptr callback{nullptr};

    void createActions();
};
#endif // MAINWINDOW_H
