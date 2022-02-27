using System.Runtime.InteropServices;

namespace WebWindowNetCore;

delegate IntPtr WindowProcedure(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

[StructLayout(LayoutKind.Sequential)]
struct WindowClass
{
    /// <summary>
    /// Die kombinierten Fensterstile
    /// </summary>
    public WindowClassStyles Style;
    [MarshalAs(UnmanagedType.FunctionPtr)]
    public WindowProcedure WindowProcedure;
    public int cbClsExtra;
    public int cbWndExtra;
    public IntPtr Instance;
    public IntPtr Icon;
    public IntPtr Cursor;
    public IntPtr BackgroundBrush;
    [MarshalAs(UnmanagedType.LPTStr)]
    public string MenuName;
    [MarshalAs(UnmanagedType.LPStr)]
    public string ClassName;
}

[StructLayout(LayoutKind.Sequential)]
struct ApiMessage
{
    /// <summary>
    /// Das Fenster, welches die Nachricht bekommen soll/hat
    /// </summary>
    public IntPtr Window;
    /// <summary>
    /// Die Art der Nachricht
    /// </summary>
    public UInt32 MessageType;
    public IntPtr WParam;
    public IntPtr LParam;
    public UInt32 Time;
    public Point MousePoint;
}

[StructLayout(LayoutKind.Sequential)]
struct Point
{
    public int X;
    public int Y;
}

[Flags]
enum WindowClassStyles : uint
{
    /// <summary>
    /// Aligns the window's client area on a byte boundary (in the x direction). 
    /// This style affects the width of the window and its horizontal placement on the display.</summary>
    ByteAlignClient = 0x1000,

    /// <summary>Aligns the window on a byte boundary (in the x direction). 
    /// This style affects the width of the window and its horizontal placement on the display.</summary>
    ByteAlignWindow = 0x2000,

    /// <summary>
    /// Allocates one device context to be shared by all windows in the class.
    /// Because window classes are process specific, it is possible for multiple threads of an application to create a window of the same class.
    /// It is also possible for the threads to attempt to use the device context simultaneously. When this happens, the system allows only one thread to successfully finish its drawing operation.
    /// </summary>
    ClassDC = 0x40,

    /// <summary>Sends a double-click message to the window procedure when the user double-clicks the mouse while the cursor is within a window belonging to the class.</summary>
    DoubleClicks = 0x8,

    /// <summary>
    /// Enables the drop shadow effect on a window. The effect is turned on and off through SPI_SETDROPSHADOW.
    /// Typically, this is enabled for small, short-lived windows such as menus to emphasize their Z order relationship to other windows.
    /// </summary>
    DropShadow = 0x20000,

    /// <summary>Indicates that the window class is an application global class. For more information, see the "Application Global Classes" section of About Window Classes.</summary>
    GlobalClass = 0x4000,

    /// <summary>Redraws the entire window if a movement or size adjustment changes the width of the client area.</summary>
    HorizontalRedraw = 0x2,

    /// <summary>Disables Close on the window menu.</summary>
    NoClose = 0x200,

    /// <summary>Allocates a unique device context for each window in the class.</summary>
    OwnDC = 0x20,

    /// <summary>
    /// Sets the clipping rectangle of the child window to that of the parent window so that the child can draw on the parent.
    /// A window with the CS_PARENTDC style bit receives a regular device context from the system's cache of device contexts.
    /// It does not give the child the parent's device context or device context settings. Specifying CS_PARENTDC enhances an application's performance.
    /// </summary>
    ParentDC = 0x80,

    /// <summary>
    /// Saves, as a bitmap, the portion of the screen image obscured by a window of this class.
    /// When the window is removed, the system uses the saved bitmap to restore the screen image, including other windows that were obscured.
    /// Therefore, the system does not send WM_PAINT messages to windows that were obscured if the memory used by the bitmap has not been discarded and if other screen actions have not invalidated the stored image.
    /// This style is useful for small windows (for example, menus or dialog boxes) that are displayed briefly and then removed before other screen activity takes place.
    /// This style increases the time required to display the window, because the system must first allocate memory to store the bitmap.
    /// </summary>
    SaveBits = 0x800,

    /// <summary>Redraws the entire window if a movement or size adjustment changes the height of the client area.</summary>
    VerticalRedraw = 0x1
}

enum WindowMessage : uint
{
    /// <summary>
    /// Wird bei der Erzeugung des Fensters, nach der Erzeugung des Fensterrahmens, aufgerufen
    /// <remarks>
    /// Rückgabe von 0 bedeutet Erfolg, Rückgabe von -1 Fehler bei der Erzeugung des Fensters
    /// </remarks>
    /// </summary> 
    Create = 1,
    /// <summary>      
    /// Wird beim Zerstören des Fensters aufgerufen
    /// </summary>
    Destroy = 0x0002,
    /// <summary>
    /// Wird aufgerufen, wenn das Fenster seine Größe ändert
    /// </summary>
    Size = 0x0005,
    Activate = 0x0006,
    /// <summary>
    /// Wenn der Darstellungsbereich des Fensters ungültig geworden ist, wird diese Nachricht generiert
    /// </summary>
    Paint = 0x000F,
    Close = 0x0010,
    ShowWindow = 0x0018,
    SettingChanged = 0x001A,
    GetMinMaxInfo = 0x0024,
    WindowPosChanging = 0x0046,
    WindowPosChanged = 0x0047,
    StyleChanging = 0x007C,
    GetIcon = 0x007F,
    SetIcon = 0x0080,
    NonClientCreate = 0x0081,
    NonClientActivate = 0x0086,
    Command = 0x0111,
    /// <summary>
    /// Gets the dimensions of the rectangle that bounds a list box item as it is currently displayed in the list box.
    /// </summary>
    ListBoxGetItemRect = 0x0198,
    /// <summary>
    /// Gets the zero-based index of the item nearest the specified point in a list box.
    /// </summary>
    ListBoxItemFromPoint = 0x01A9,
    MouseMove = 0x0200,
    LeftButtonUp = 0x0202,
    MouseWheel = 0x020A,
    MouseHover = 0x02A1,
    MouseLeave = 0x02A3,
    SendIconicThumbnail = 0x0323,
    PropertySheetChanged = 0x0468,
    /// <summary>
    /// Ermittelt das umgebende Rechteck eines ListView-Eintrages oder Teilen davon
    /// <remarks>wParam ist der Index, des zu betrachtenden Listeneintrages, lParam ist das ermittelte Rechteck. Beim Aufruf muss der 
    /// <see cref="Api.RECT.Left"/>-Parameter mit einem Wert aus <see cref="ListViewItemRectBounds"/> versehen werden. Rückgabe: true im Erfolgsfall
    /// </remarks>
    /// </summary>
    ListViewGetItemtRect = 0x100e,
    /// <summary>
    /// Ermittlung, welches ListViewItem einer ListView sich an einer bestimmten Position befindet
    /// </summary>
    ListViewItemHitTest = 0x1012,
    /// <summary>
    /// Determines which list-view item or subitem is at a given position. 
    /// </summary>
    ListViewSubItemHitTest = 0x1039,
    /// <summary>
    /// Sets the textual cue, or tip, that is displayed by the edit control to prompt the user for information.
    /// 
    /// <list type="bullet">
    /// <listheader>Parameters</listheader>
    /// <item>
    /// <term>wParam [in]</term>
    /// <description>TRUE if the cue banner should show even when the edit control has focus; otherwise, FALSE. FALSE is the default behavior—the cue banner disappears when the user clicks in the control.</description>
    /// </item>
    /// <item>
    /// <term>lParam [in]</term>
    /// <description>A pointer to a Unicode string that contains the text to display as the textual cue</description>
    /// </item>
    /// </list>
    /// </summary>
    SetCueBanner = 0x1501
}

[Flags]
enum WindowStylesEx : uint
{
    /// <summary>
    /// Kein Ex-Stil
    /// </summary>
    Null = 0,
    /// <summary>
    /// Specifies that a window created with this style accepts drag-drop files.
    /// </summary>
    AcceptFiles = 0x00000010,
    /// <summary>
    /// Forces a top-level window onto the taskbar when the window is visible.
    /// </summary>
    AppWindow = 0x00040000,
    /// <summary>
    /// Specifies that a window has a border with a sunken edge.
    /// </summary>
    ClientEdge = 0x00000200,
    /// <summary>
    /// Windows XP: Paints all descendants of a window in bottom-to-top painting order using double-buffering. For more information, see Remarks. This cannot be used if the window has a class style of either CS_OWNDC or CS_CLASSDC. 
    /// </summary>
    Composited = 0x02000000,
    /// <summary>
    /// Includes a question mark in the title bar of the window. When the user clicks the question mark, the cursor changes to a question mark with a pointer. If the user then clicks a child window, the child receives a WM_HELP message. The child window should pass the message to the parent window procedure, which should call the WinHelp function using the HELP_WM_HELP command. The Help application displays a pop-up window that typically contains help for the child window.
    /// WS_EX_CONTEXTHELP cannot be used with the WS_MAXIMIZEBOX or WS_MINIMIZEBOX styles.
    /// </summary>
    ContextHelp = 0x00000400,
    /// <summary>
    /// The window itself contains child windows that should take part in dialog box navigation. If this style is specified, the dialog manager recurses into children of this window when performing navigation operations such as handling the TAB key, an arrow key, or a keyboard mnemonic.
    /// </summary>
    ControlParent = 0x00010000,
    /// <summary>
    /// Creates a window that has a double border; the window can, optionally, be created with a title bar by specifying the WS_CAPTION style in the dwStyle parameter.
    /// </summary>
    DialogModalFrame = 0x00000001,
    /// <summary>
    /// Windows 2000/XP: Creates a layered window. Note that this cannot be used for child windows. Also, this cannot be used if the window has a class style of either CS_OWNDC or CS_CLASSDC. 
    /// </summary>
    Layered = 0x00080000,
    /// <summary>
    /// Arabic and Hebrew versions of Windows 98/Me, Windows 2000/XP: Creates a window whose horizontal origin is on the right edge. Increasing horizontal values advance to the left. 
    /// </summary>
    LayoutRtl = 0x00400000,
    /// <summary>
    /// Creates a window that has generic left-aligned properties. This is the default.
    /// </summary>
    Left = 0x00000000,
    /// <summary>
    /// If the shell language is Hebrew, Arabic, or another language that supports reading order alignment, the vertical scroll bar (if present) is to the left of the client area. For other languages, the style is ignored.
    /// </summary>
    LeftScrollbar = 0x00004000,
    /// <summary>
    /// The window text is displayed using left-to-right reading-order properties. This is the default.
    /// </summary>
    LtrReading = 0x00000000,
    /// <summary>
    /// Creates a multiple-document interface (MDI) child window.
    /// </summary>
    MdiChild = 0x00000040,
    /// <summary>
    /// Windows 2000/XP: A top-level window created with this style does not become the foreground window when the user clicks it. The system does not bring this window to the foreground when the user minimizes or closes the foreground window. 
    /// To activate the window, use the SetActiveWindow or SetForegroundWindow function.
    /// The window does not appear on the taskbar by default. To force the window to appear on the taskbar, use the WS_EX_APPWINDOW style.
    /// </summary>
    NoActivate = 0x08000000,
    /// <summary>
    /// Windows 2000/XP: A window created with this style does not pass its window layout to its child windows.
    /// </summary>
    NoInheritLayout = 0x00100000,
    /// <summary>
    /// Specifies that a child window created with this style does not send the WM_PARENTNOTIFY message to its parent window when it is created or destroyed.
    /// </summary>
    NoParentNotify = 0x00000004,
    /// <summary>
    /// Combines the WS_EX_CLIENTEDGE and WS_EX_WINDOWEDGE styles.
    /// </summary>
    OverlappedWindow = WindowEdge | ClientEdge,
    /// <summary>
    /// Combines the WS_EX_WINDOWEDGE, WS_EX_TOOLWINDOW, and WS_EX_TOPMOST styles.
    /// </summary>
    PaletteWindow = WindowEdge | ToolWindow | TopMost,
    /// <summary>
    /// The window has generic "right-aligned" properties. This depends on the window class. This style has an effect only if the shell language is Hebrew, Arabic, or another language that supports reading-order alignment; otherwise, the style is ignored.
    /// Using the WS_EX_RIGHT style for static or edit controls has the same effect as using the SS_RIGHT or ES_RIGHT style, respectively. Using this style with button controls has the same effect as using BS_RIGHT and BS_RIGHTBUTTON styles.
    /// </summary>
    Right = 0x00001000,
    /// <summary>
    /// Vertical scroll bar (if present) is to the right of the client area. This is the default.
    /// </summary>
    RightScrollbar = 0x00000000,
    /// <summary>
    /// If the shell language is Hebrew, Arabic, or another language that supports reading-order alignment, the window text is displayed using right-to-left reading-order properties. For other languages, the style is ignored.
    /// </summary>
    RtlReading = 0x00002000,
    /// <summary>
    /// Creates a window with a three-dimensional border style intended to be used for items that do not accept user input.
    /// </summary>
    StaticEdge = 0x00020000,
    /// <summary>
    /// Creates a tool window; that is, a window intended to be used as a floating toolbar. A tool window has a title bar that is shorter than a normal title bar, and the window title is drawn using a smaller font. A tool window does not appear in the taskbar or in the dialog that appears when the user presses ALT+TAB. If a tool window has a system menu, its icon is not displayed on the title bar. However, you can display the system menu by right-clicking or by typing ALT+SPACE. 
    /// </summary>
    ToolWindow = 0x00000080,
    /// <summary>
    /// Specifies that a window created with this style should be placed above all non-topmost windows and should stay above them, even when the window is deactivated. To add or remove this style, use the SetWindowPos function.
    /// </summary>
    TopMost = 0x00000008,
    /// <summary>
    /// Specifies that a window created with this style should not be painted until siblings beneath the window (that were created by the same thread) have been painted. The window appears transparent because the bits of underlying sibling windows have already been painted.
    /// To achieve transparency without these restrictions, use the SetWindowRgn function.
    /// </summary>
    Transparent = 0x00000020,
    /// <summary>
    /// Specifies that a window has a border with a raised edge.
    /// </summary>
    WindowEdge = 0x00000100
}

[Flags()]
enum WindowStyles : uint
{
    /// <summary>
    /// Kein Stil
    /// </summary>
    WS_NULL = 0,
    /// <summary>The window has a thin-line border.</summary>
    WS_BORDER = 0x800000,

    /// <summary>The window has a title bar (includes the WS_BORDER style).</summary>
    WS_CAPTION = 0xc00000,

    /// <summary>The window is a child window. A window with this style cannot have a menu bar. This style cannot be used with the WS_POPUP style.</summary>
    WS_CHILD = 0x40000000,

    /// <summary>Excludes the area occupied by child windows when drawing occurs within the parent window. This style is used when creating the parent window.</summary>
    WS_CLIPCHILDREN = 0x2000000,

    /// <summary>
    /// Clips child windows relative to each other; that is, when a particular child window receives a WM_PAINT message, the WS_CLIPSIBLINGS style clips all other overlapping child windows out of the region of the child window to be updated.
    /// If WS_CLIPSIBLINGS is not specified and child windows overlap, it is possible, when drawing within the client area of a child window, to draw within the client area of a neighboring child window.
    /// </summary>
    WS_CLIPSIBLINGS = 0x4000000,

    /// <summary>The window is initially disabled. A disabled window cannot receive input from the user. To change this after a window has been created, use the EnableWindow function.</summary>
    WS_DISABLED = 0x8000000,

    /// <summary>The window has a border of a style typically used with dialog boxes. A window with this style cannot have a title bar.</summary>
    WS_DLGFRAME = 0x400000,

    /// <summary>
    /// The window is the first control of a group of controls. The group consists of this first control and all controls defined after it, up to the next control with the WS_GROUP style.
    /// The first control in each group usually has the WS_TABSTOP style so that the user can move from group to group. The user can subsequently change the keyboard focus from one control in the group to the next control in the group by using the direction keys.
    /// You can turn this style on and off to change dialog box navigation. To change this style after a window has been created, use the SetWindowLong function.
    /// </summary>
    WS_GROUP = 0x20000,

    /// <summary>The window has a horizontal scroll bar.</summary>
    WS_HSCROLL = 0x100000,

    /// <summary>The window is initially maximized.</summary> 
    WS_MAXIMIZE = 0x1000000,

    /// <summary>The window has a maximize button. Cannot be combined with the WS_EX_CONTEXTHELP style. The WS_SYSMENU style must also be specified.</summary> 
    WS_MAXIMIZEBOX = 0x10000,

    /// <summary>The window is initially minimized.</summary>
    WS_MINIMIZE = 0x20000000,

    /// <summary>The window has a minimize button. Cannot be combined with the WS_EX_CONTEXTHELP style. The WS_SYSMENU style must also be specified.</summary>
    WS_MINIMIZEBOX = 0x20000,

    /// <summary>The window is an overlapped window. An overlapped window has a title bar and a border.</summary>
    WS_OVERLAPPED = 0x0,

    /// <summary>The window is an overlapped window.</summary>
    WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_SIZEFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,

    /// <summary>The window is a pop-up window. This style cannot be used with the WS_CHILD style.</summary>
    WS_POPUP = 0x80000000u,

    /// <summary>The window is a pop-up window. The WS_CAPTION and WS_POPUPWINDOW styles must be combined to make the window menu visible.</summary>
    WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,

    /// <summary>The window has a sizing border.</summary>
    WS_SIZEFRAME = 0x40000,

    /// <summary>The window has a window menu on its title bar. The WS_CAPTION style must also be specified.</summary>
    WS_SYSMENU = 0x80000,

    /// <summary>
    /// The window is a control that can receive the keyboard focus when the user presses the TAB key.
    /// Pressing the TAB key changes the keyboard focus to the next control with the WS_TABSTOP style.  
    /// You can turn this style on and off to change dialog box navigation. To change this style after a window has been created, use the SetWindowLong function.
    /// For user-created windows and modeless dialogs to work with tab stops, alter the message loop to call the IsDialogMessage function.
    /// </summary>
    WS_TABSTOP = 0x10000,

    /// <summary>The window is initially visible. This style can be turned on and off by using the ShowWindow or SetWindowPos function.</summary>
    WS_VISIBLE = 0x10000000,

    /// <summary>The window has a vertical scroll bar.</summary>
    WS_VSCROLL = 0x200000
}

enum ShowWindowCommands : int
{
    /// <summary>
    /// Hides the window and activates another window.
    /// </summary>
    Hide = 0,
    /// <summary>
    /// Activates and displays a window. If the window is minimized or 
    /// maximized, the system restores it to its original size and position.
    /// An application should specify this flag when displaying the window 
    /// for the first time.
    /// </summary>
    Normal = 1,
    /// <summary>
    /// Activates the window and displays it as a minimized window.
    /// </summary>
    ShowMinimized = 2,
    /// <summary>
    /// Maximizes the specified window.
    /// </summary>
    Maximize = 3, // is this the right value?
    /// <summary>
    /// Activates the window and displays it as a maximized window.
    /// </summary>       
    ShowMaximized = 3,
    /// <summary>
    /// Displays a window in its most recent size and position. This value 
    /// is similar to <see cref="Caseris.Native.ShowWindowCommands.Normal"/>, except 
    /// the window is not activated.
    /// </summary>
    ShowNoActivate = 4,
    /// <summary>
    /// Activates the window and displays it in its current size and position. 
    /// </summary>
    Show = 5,
    /// <summary>
    /// Minimizes the specified window and activates the next top-level 
    /// window in the Z order.
    /// </summary>
    Minimize = 6,
    /// <summary>
    /// Displays the window as a minimized window. This value is similar to
    /// <see cref="Caseris.Native.ShowWindowCommands.ShowMinimized"/>, except the 
    /// window is not activated.
    /// </summary>
    ShowMinNoActive = 7,
    /// <summary>
    /// Displays the window in its current size and position. This value is 
    /// similar to <see cref="Caseris.Native.ShowWindowCommands.Show"/>, except the 
    /// window is not activated.
    /// </summary>
    ShowNA = 8,
    /// <summary>
    /// Activates and displays the window. If the window is minimized or 
    /// maximized, the system restores it to its original size and position. 
    /// An application should specify this flag when restoring a minimized window.
    /// </summary>
    Restore = 9,
    /// <summary>
    /// Sets the show state based on the SW_* value specified in the 
    /// STARTUPINFO structure passed to the CreateProcess function by the 
    /// program that started the application.
    /// </summary>
    ShowDefault = 10,
    /// <summary>
    ///  <b>Windows 2000/XP:</b> Minimizes a window, even if the thread 
    /// that owns the window is not responding. This flag should only be 
    /// used when minimizing windows from a different thread.
    /// </summary>
    ForceMinimize = 11
}


