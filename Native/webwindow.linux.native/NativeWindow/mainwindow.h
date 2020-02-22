#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QMainWindow>
#include <QtWebEngineWidgets/QtWebEngineWidgets>

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
};

class MainWindow : public QMainWindow
{
    Q_OBJECT

public:
    MainWindow(const Configuration& configuration, QWidget *parent = nullptr);
    ~MainWindow();

    void acceptFullScreen(QWebEngineFullScreenRequest request);
private:
    QWebEngineView* webView;
    QString organization;
    QString application;
};
#endif // MAINWINDOW_H
