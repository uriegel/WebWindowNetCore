#include "mainwindow.h"

MainWindow::MainWindow(const Configuration& configuration, QWidget *parent)
    : QMainWindow(parent)
{
    setWindowTitle(configuration.title);
    if (configuration.icon_path)
        setWindowIcon(QIcon(configuration.icon_path));
    webView = new QWebEngineView;

    setAttribute( Qt::WA_NativeWindow, true );

    resize(800, 600);

    if (configuration.save_window_settings) {
        organization = configuration.organization;
        application = configuration.application;
        QSettings settings(organization, application);
        QByteArray saved_geometry = settings.value("geometry").toByteArray();
        restoreGeometry(saved_geometry);
        QByteArray saved_state = settings.value("windowState").toByteArray();
        restoreState(saved_state);
    }

    setCentralWidget(webView);

    if (configuration.fullscreen_enabled) {
        webView->settings()->setAttribute(QWebEngineSettings::FullScreenSupportEnabled,true);
        webView->settings()->setAttribute(QWebEngineSettings::PluginsEnabled,true);
        webView->settings()->setAttribute(QWebEngineSettings::JavascriptCanOpenWindows,true);
        connect(webView->page(),&QWebEnginePage::fullScreenRequested,this,&MainWindow::acceptFullScreen);
    }

    QUrl url = QUrl(configuration.url);
    //url.setScheme("http");
    webView->page()->load(url);
}

MainWindow::~MainWindow() {
    if (!organization.isEmpty()) {
        QSettings settings(organization, application);
        settings.setValue("geometry", saveGeometry());
        settings.setValue("windowState", saveState());
    }
}

void MainWindow::acceptFullScreen(QWebEngineFullScreenRequest request){
    request.accept();
    if (request.toggleOn())
        showFullScreen();
    else
        showNormal();
}
