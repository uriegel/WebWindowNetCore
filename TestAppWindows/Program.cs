using WebWindowNetCore;

var sseEventSource = WebView.CreateEventSource<Event>();
StartEvents(sseEventSource.Send);

WebView
    .Create()
    .InitialBounds(800, 600)
    .Title("WebView Test")
    .ResourceIcon("icon")
    .SaveBounds()
    //.DebugUrl("https://www.google.de")
    //.Url($"file://{Directory.GetCurrentDirectory()}/webroot/index.html")
    .ConfigureHttp(http => http
        .ResourceWebroot("webroot", "/web")
        .UseSse("sse/test", sseEventSource)
        .Build())
#if DEBUG            
    .DebuggingEnabled()
#endif            
    .Build()
    .Run("de.uriegel.Commander");    

void StartEvents(Action<Event> onChanged)   
{
    var counter = 0;
    new Thread(_ =>
        {
            while (true)
            {
                Thread.Sleep(5000);
                onChanged(new($"Ein Event {counter++}"));
           }
        })
        {
            IsBackground = true
        }.Start();   
}

record Event(string Content);
// TODO take transparent drag source window from test project

/*
using System.Runtime.InteropServices;


*/
/*
Overlay

namespace WebView2Form;
public partial class Overlay : Form
{
    public Overlay(Form parent) 
    {
        InitializeComponent();
    }
}
using System.Runtime.InteropServices;

namespace WebView2Form;

namespace WebView2Form;

partial class Overlay
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
            this.SuspendLayout();
            // 
            // Overlay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Name = "Overlay";
            this.Opacity = 0.3D;
            this.Text = "Overlay";
            this.ResumeLayout(false);

    }

    #endregion
}*/