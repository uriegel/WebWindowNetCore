#include "mainwindow.h"

MainWindow::MainWindow(const QString& title, const QString& urlstring, QWidget *parent)
    : QMainWindow(parent)
{
    setWindowTitle(title);
    webView = new QWebEngineView;

    setAttribute( Qt::WA_NativeWindow, true );

    setCentralWidget(webView);

    QUrl url = QUrl(urlstring);
    //url.setScheme("http");
    webView->page()->load(url);
}

MainWindow::~MainWindow()
{
}

