#include "mainwindow.h"

MainWindow::MainWindow(const QString& title, const QString& urlstring, QWidget *parent)
    : QMainWindow(parent)
{
    setWindowTitle(title);
    setWindowIcon(QIcon("/home/uwe/Dokumente/icon.svg"));
    webView = new QWebEngineView;

    setAttribute( Qt::WA_NativeWindow, true );

    resize(800, 600);

    QSettings settings("URiegel", "SuperBrauser2");
    QByteArray saved_geometry = settings.value("geometry").toByteArray();
    restoreGeometry(saved_geometry);
    QByteArray saved_state = settings.value("windowState").toByteArray();
    restoreState(saved_state);

    setCentralWidget(webView);

    // Fullscreen
    webView->settings()->setAttribute(QWebEngineSettings::FullScreenSupportEnabled,true);
    webView->settings()->setAttribute(QWebEngineSettings::PluginsEnabled,true);
    webView->settings()->setAttribute(QWebEngineSettings::JavascriptCanOpenWindows,true);
    connect(webView->page(),&QWebEnginePage::fullScreenRequested,this,&MainWindow::acceptFullScreen);

    QUrl url = QUrl(urlstring);
    //url.setScheme("http");
    webView->page()->load(url);
}

MainWindow::~MainWindow() {
    QSettings settings("URiegel", "SuperBrauser2");
    settings.setValue("geometry", saveGeometry());
    settings.setValue("windowState", saveState());
}

void MainWindow::acceptFullScreen(QWebEngineFullScreenRequest request){
    request.accept();
    if (request.toggleOn())
        showFullScreen();
    else
        showNormal();
}
