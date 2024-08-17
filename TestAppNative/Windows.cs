#if Windows

using System.Drawing;
using System.Windows.Forms;

static class WindowsExtensions
{
    static ContextMenuStrip? contextMenuStrip1 = null;
    static Form? form = null;

    public static void FormCreation(Form webViewForm)
    {
        var menuStrip1 = new MenuStrip();
        form = webViewForm;
        var toolStripMenuItem1 = new ToolStripMenuItem();
        var toolStripMenuItem2 = new ToolStripMenuItem();
        var toolStripMenuItem3 = new ToolStripMenuItem();
        var toolStripMenuItem4 = new ToolStripMenuItem();
        var toolStripMenuItem5 = new ToolStripMenuItem();
        var toolStripSeparator1 = new ToolStripSeparator();
        var toolStripMenuItem6 = new ToolStripMenuItem();
        contextMenuStrip1 = new();
        var punkt1ToolStripMenuItem = new ToolStripMenuItem();
        var punkt2ToolStripMenuItem = new ToolStripMenuItem();
        var mehrToolStripMenuItem = new ToolStripMenuItem();
        var etwasMehrToolStripMenuItem = new ToolStripMenuItem();
        var vielMehrToolStripMenuItem = new ToolStripMenuItem();
        var toolStripSeparator2 = new ToolStripSeparator();
        var nichtsMehrToolStripMenuItem = new ToolStripMenuItem();
        // 
        // menuStrip1
        // 
        menuStrip1.ImageScalingSize = new Size(20, 20);
        menuStrip1.Items.AddRange(new ToolStripItem[] { toolStripMenuItem1 });
        menuStrip1.Location = new Point(0, 0);
        menuStrip1.Name = "menuStrip1";
        menuStrip1.Size = new Size(800, 28);
        menuStrip1.TabIndex = 0;
        menuStrip1.Text = "menuStrip1";
        // 
        // toolStripMenuItem1
        // 
        toolStripMenuItem1.DropDownItems.AddRange(new ToolStripItem[] { toolStripMenuItem2, toolStripMenuItem3, toolStripMenuItem4, toolStripMenuItem5, toolStripSeparator1, toolStripMenuItem6 });
        toolStripMenuItem1.Name = "toolStripMenuItem1";
        toolStripMenuItem1.Size = new Size(156, 24);
        toolStripMenuItem1.Text = "toolStripMenuItem1";
        // 
        // toolStripMenuItem2
        // 
        toolStripMenuItem2.Name = "toolStripMenuItem2";
        toolStripMenuItem2.Size = new Size(225, 26);
        toolStripMenuItem2.Text = "toolStripMenuItem2";
        // 
        // toolStripMenuItem3
        // 
        toolStripMenuItem3.Name = "toolStripMenuItem3";
        toolStripMenuItem3.Size = new Size(225, 26);
        toolStripMenuItem3.Text = "toolStripMenuItem3";
        // 
        // toolStripMenuItem4
        // 
        toolStripMenuItem4.Name = "toolStripMenuItem4";
        toolStripMenuItem4.Size = new Size(225, 26);
        toolStripMenuItem4.Text = "toolStripMenuItem4";
        // 
        // toolStripMenuItem5
        // 
        toolStripMenuItem5.Name = "toolStripMenuItem5";
        toolStripMenuItem5.Size = new Size(225, 26);
        toolStripMenuItem5.Text = "toolStripMenuItem5";
        // 
        // toolStripSeparator1
        // 
        toolStripSeparator1.Name = "toolStripSeparator1";
        toolStripSeparator1.Size = new Size(222, 6);
        // 
        // toolStripMenuItem6
        // 
        toolStripMenuItem6.Name = "toolStripMenuItem6";
        toolStripMenuItem6.Size = new Size(225, 26);
        toolStripMenuItem6.Text = "toolStripMenuItem6";
        // 
        // contextMenuStrip1
        // 
        contextMenuStrip1.ImageScalingSize = new Size(20, 20);
        contextMenuStrip1.Items.AddRange(new ToolStripItem[] { punkt1ToolStripMenuItem, punkt2ToolStripMenuItem, mehrToolStripMenuItem });
        contextMenuStrip1.Name = "contextMenuStrip1";
        contextMenuStrip1.Size = new Size(127, 76);
        // 
        // punkt1ToolStripMenuItem
        // 
        punkt1ToolStripMenuItem.Name = "punkt1ToolStripMenuItem";
        punkt1ToolStripMenuItem.Size = new Size(126, 24);
        punkt1ToolStripMenuItem.Text = "Punkt1";
        // 
        // punkt2ToolStripMenuItem
        // 
        punkt2ToolStripMenuItem.Name = "punkt2ToolStripMenuItem";
        punkt2ToolStripMenuItem.Size = new Size(126, 24);
        punkt2ToolStripMenuItem.Text = "Punkt 2";
        // 
        // mehrToolStripMenuItem
        // 
        mehrToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { etwasMehrToolStripMenuItem, vielMehrToolStripMenuItem, toolStripSeparator2, nichtsMehrToolStripMenuItem });
        mehrToolStripMenuItem.Name = "mehrToolStripMenuItem";
        mehrToolStripMenuItem.Size = new Size(126, 24);
        mehrToolStripMenuItem.Text = "Mehr";
        // 
        // etwasMehrToolStripMenuItem
        // 
        etwasMehrToolStripMenuItem.Name = "etwasMehrToolStripMenuItem";
        etwasMehrToolStripMenuItem.Size = new Size(171, 26);
        etwasMehrToolStripMenuItem.Text = "Etwas mehr";
        // 
        // vielMehrToolStripMenuItem
        // 
        vielMehrToolStripMenuItem.Name = "vielMehrToolStripMenuItem";
        vielMehrToolStripMenuItem.Size = new Size(171, 26);
        vielMehrToolStripMenuItem.Text = "Viel mehr";
        // 
        // toolStripSeparator2
        // 
        toolStripSeparator2.Name = "toolStripSeparator2";
        toolStripSeparator2.Size = new Size(168, 6);
        // 
        // nichtsMehrToolStripMenuItem
        // 
        nichtsMehrToolStripMenuItem.Name = "nichtsMehrToolStripMenuItem";
        nichtsMehrToolStripMenuItem.Size = new Size(171, 26);
        nichtsMehrToolStripMenuItem.Text = "Nichts mehr";
    }
}
#endif