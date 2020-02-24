#include "mainwindow.h"

int recentId{0};

MainWindow::MainWindow(const Configuration& configuration, QWidget *parent)
    : QMainWindow(parent)
{
    setWindowTitle(configuration.title);
    if (configuration.icon_path)
        setWindowIcon(QIcon(configuration.icon_path));
    webView = new WebEngineView;
    this->callback = configuration.callback;

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

    initializeScript(configuration.callback);

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

void MainWindow::action(QAction* action) {
    auto data = action->data();
    if (data == 4) {
        auto checked = action->isChecked();
        auto test = 9;
    }
}

QMenu* MainWindow::add_menu(const char* title, QMenu* parent) {
    auto menu = parent ? parent->addMenu(title) : menuBar()->addMenu(title);
    connect(menu, &QMenu::triggered, this, &MainWindow::action);
    return menu;
}

QActionGroup* MainWindow::createMenuGroup() {
    return new QActionGroup(this);
}

int MainWindow::set_menu_item(QMenu* menu, Menu_item menu_item) {
    switch (menu_item.menu_item_type) {
    case Menu_item_type::Menu: {
        auto id = ++recentId;
        auto new_action = new QAction(menu_item.title, this);
        if (menu_item.accelerator)
            new_action->setShortcut(QKeySequence(menu_item.accelerator));
        new_action->setData(id);
        menu->addAction(new_action);
        return id;
    }
    case Menu_item_type::Checkbox: {
        auto id = ++recentId;
        auto new_action = new QAction(menu_item.title, this);
        if (menu_item.accelerator)
            new_action->setShortcut(QKeySequence(menu_item.accelerator));
        new_action->setData(id);
        new_action->setCheckable(true);
        menu->addAction(new_action);
        checkableMenuItems[id] = new_action;
        return id;
    }
    case Menu_item_type::Separator:
        menu->addSeparator();
        return -1;
    }
}

int MainWindow::setGroupedMenuItem(QMenu* menu, Menu_item menu_item, QActionGroup* group) {
    auto id = ++recentId;
    auto new_action = new QAction(menu_item.title, this);
    if (menu_item.accelerator)
        new_action->setShortcut(QKeySequence(menu_item.accelerator));
    new_action->setData(id);
    new_action->setCheckable(true);
    new_action->setActionGroup(group);
    menu->addAction(new_action);
    checkableMenuItems[id] = new_action;
    return id;
}

void MainWindow::initializeScript(Callback_ptr callback) {
    QWebEngineScript script;

    QString scripttext;
    if (callback) {
        QFile file(":/qtwebchannel/qwebchannel.js");
        file.open(QIODevice::ReadOnly);
        scripttext = QString(file.readAll());
    }

    scripttext.append(
R"(

var webWindowNetCore = (function() {
    if (window.QWebChannel)
        new QWebChannel(qt.webChannelTransport,
            function(channel) {
                channelObject = channel.objects.webobj
            }
        )

    function setCallback(callbackFromHost) {
        callback = callbackFromHost
    }

    function callCallback(text) {
        if (callback)
            callback(text)
    }

    function postMessage(text) {
        channelObject.postMessage(text)
    }

    var callback
    var channelObject

    return {
        setCallback,
        callCallback,
        postMessage
    }
})())");
    script.setName("Initial Script");
    script.setSourceCode(scripttext);
    script.setInjectionPoint(QWebEngineScript::DocumentCreation);
    script.setRunsOnSubFrames(true);
    script.setWorldId(QWebEngineScript::MainWorld);

    webView->page()->scripts().insert(script);

    if (callback) {
        auto channel = new QWebChannel(this);
        channel->registerObject("webobj", this);
        webView->page()->setWebChannel(channel);
    }
}

void MainWindow::send_to_browser(const char* text) {
    webView->page()->runJavaScript(QString::fromUtf8("webWindowNetCore.callCallback('%1');").arg(text));
}

void MainWindow::postMessage(const QString& msg) {
    callback(msg.toUtf8());
}
