TEMPLATE = lib

QT += webengine

SOURCES += webwindow.cpp

RESOURCES += qml.qrc

target.path = $$[QT_INSTALL_EXAMPLES]/webengine/minimal
INSTALLS += target
