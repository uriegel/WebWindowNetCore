#include "mainwindow.h"

int recentId{0};

MainWindow::MainWindow(const Configuration& configuration, QWidget *parent)
    : QMainWindow(parent)
{
    setWindowTitle(configuration.title);
    if (configuration.icon_path)
        setWindowIcon(QIcon(configuration.icon_path));
    webView = new WebEngineView;
    this->onEvent = configuration.onEvent;

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

    initializeScript(configuration.onEvent);

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
    auto data = action->data().toUInt();
    auto menuItem = menuItems[data];
    switch (menuItem.menuItemType) {
        case MenuItemType::MenuItem:
        case MenuItemType::Radio:
            menuItem.onMenu();
            break;
        case MenuItemType::Checkbox:
            menuItem.onChecked(action->isChecked());
            break;
        default:
            break;
    }
}

QMenu* MainWindow::add_menu(const char* title, QMenu* parent) {
    auto menu = parent ? parent->addMenu(title) : menuBar()->addMenu(title);
    connect(menu, &QMenu::triggered, this, &MainWindow::action);
    return menu;
}

struct MenuData : public QObject {
    MenuData(MenuItem menuItem) : menuItem(menuItem) {}
    MenuItem menuItem;
};

QActionGroup* recentGroup {nullptr};

int MainWindow::set_menu_item(QMenu* menu, MenuItem menuItem) {
    auto id = ++recentId;
    menuItems[id] = menuItem;
    switch (menuItem.menuItemType) {
    case MenuItemType::MenuItem: {
        auto new_action = new QAction(menuItem.title, this);
        if (menuItem.accelerator)
            new_action->setShortcut(QKeySequence(menuItem.accelerator));
        new_action->setData(id);
        menu->addAction(new_action);
        return id;
    }
    case MenuItemType::Checkbox: {
        auto new_action = new QAction(menuItem.title, this);
        if (menuItem.accelerator)
            new_action->setShortcut(QKeySequence(menuItem.accelerator));
        new_action->setData(id);
        new_action->setCheckable(true);
        checkableMenuItems[id] = new_action;
        menu->addAction(new_action);
        return id;
    }
    case MenuItemType::Radio: {

        auto new_action = new QAction(menuItem.title, this);
        new_action->setCheckable(true);
        if (!recentGroup) {
            recentGroup = new QActionGroup(this);
            new_action->setChecked(true);
        }
        if (menuItem.accelerator)
            new_action->setShortcut(QKeySequence(menuItem.accelerator));
        new_action->setData(id);
        new_action->setActionGroup(recentGroup);
        menu->addAction(new_action);
        checkableMenuItems[id] = new_action;
        if (menuItem.groupCount == menuItem.groupId + 1)
            recentGroup = nullptr;
        return id;
    }
    case MenuItemType::Separator:
        menu->addSeparator();
        return -1;
    default:
        return -1;
    }
}

void MainWindow::setMenuItemChecked(int cmdId, bool checked) {
    checkableMenuItems[cmdId]->setChecked(checked);

    auto affe = checkableMenuItems[cmdId];
    auto af = affe;
}

void MainWindow::initializeScript(EventCallback onEvent) {
    QWebEngineScript script;

    QString scripttext;
    if (onEvent) {
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

    if (onEvent) {
        auto channel = new QWebChannel(this);
        channel->registerObject("webobj", this);
        webView->page()->setWebChannel(channel);
    }
}

void MainWindow::send_to_browser(const char* text) {
    webView->page()->runJavaScript(QString::fromUtf8("webWindowNetCore.callCallback('%1');").arg(text));
}

void MainWindow::postMessage(const QString& msg) {
    onEvent(msg.toUtf8());
}
