using System.Runtime.InteropServices;

namespace WebWindowNetCore;

[ComVisible(true)]
public class Callback
{
    public Callback(WebWindowForm parent) => this.parent = parent;

    public void Init(int width, int height, bool maximize) 
        => parent.Init(width, height, maximize);

    public void ShowDevtools() => parent.ShowDevtools();

    // public void DragDropFile()
    // {
    //     var overlay = new Overlay(parent);
    //     overlay.Top = parent.Top;
    //     overlay.Left = parent.Left;
    //     overlay.Width= parent.Width / 2;
    //     overlay.Height = parent.Height / 2;
    //     try
    //     {
    //         parent.Controls.Add(overlay);
    //     }
    //     catch (Exception e)
    //     {

    //     }
    //     overlay.Show(parent);
    // }

    WebWindowForm parent;
}
