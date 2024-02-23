using System.Drawing.Drawing2D;

namespace Q2_AsyncDrawing;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
    }

    private void DrawCircle(PaintEventArgs e)
    {
        GraphicsPath gp = new GraphicsPath();
        gp.AddEllipse(100, 15, 70, 70);
        Region r = new Region(gp);
        Graphics gr = e.Graphics;
        gr.FillRegion(Brushes.LawnGreen, r);
    }

    private void DrawRectangle(PaintEventArgs e)
    {
        GraphicsPath gp = new GraphicsPath();
        Rectangle rc = new Rectangle(220, 15, 100, 70);
        gp.AddRectangle(rc);
        Region r = new Region(gp);
        Graphics gr = e.Graphics;
        gr.FillRegion(Brushes.OrangeRed, r);
    }

    private async void Form1_Paint(object sender, PaintEventArgs e)
    {
        Task circleTask = Task.Run(() => DrawCircle(e));
        Task rectangleTask = Task.Run(() => DrawRectangle(e));

        await circleTask;
        await rectangleTask;
    }
}
