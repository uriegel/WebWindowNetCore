#include "mainwindow.h"

MainWindow::MainWindow(QWidget *parent)
    : QMainWindow(parent)
{
    setWindowTitle("Der Webbrauser");
    webView = new QWebEngineView;

    setAttribute( Qt::WA_NativeWindow, true );

    setCentralWidget(webView);

    QUrl url = QUrl("caesar2go.caseris.de");
    url.setScheme("http");
    webView->page()->load(url);
}

MainWindow::~MainWindow()
{
}

