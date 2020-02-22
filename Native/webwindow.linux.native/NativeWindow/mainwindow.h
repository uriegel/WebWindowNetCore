#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QMainWindow>
#include <QtWebEngineWidgets/QtWebEngineWidgets>

class MainWindow : public QMainWindow
{
    Q_OBJECT

public:
    MainWindow(const QString& title, const QString& url, QWidget *parent = nullptr);
    ~MainWindow();

    void acceptFullScreen(QWebEngineFullScreenRequest request);
private:
    QWebEngineView* webView;
};
#endif // MAINWINDOW_H
