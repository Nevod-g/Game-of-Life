using System;
using System.Linq;
using System.Windows.Forms;
using static Life.Program;

namespace Life
{
    public static class Controller
    {
        static bool MouseButtonLeftPressed;
        static bool MouseButtonRightPressed;
        public static void KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                MainScenario.Stop();
            else if (e.KeyCode == Keys.Oemplus | e.KeyCode == Keys.Add) // +
                MainScreen.ZoomIncrease();
            else if (e.KeyValue == 188 | e.KeyValue == 109) // -
                MainScreen.ZoomDecrease();
        }

        public static void MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MouseButtonLeftPressed = true;
                MainScreen.ReviveCell(e.X, e.Y);
            }
            else if (e.Button == MouseButtons.Right)
            {
                MouseButtonRightPressed = true;
                MainScreen.KillCell(e.X, e.Y);
            }
        }

        public static void MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                MouseButtonLeftPressed = false;
            else if (e.Button == MouseButtons.Right)
                MouseButtonRightPressed = false;
        }

        public static void MouseMove(object sender, MouseEventArgs e)
        {
            if (MouseButtonLeftPressed)
                MainScreen.ReviveCell(e.X, e.Y);
            else if (MouseButtonRightPressed)
                MainScreen.KillCell(e.X, e.Y);
        }

        public static void MouseHover(object sender, EventArgs e)
        {

        }

        public static void MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
                MainScreen.ZoomIncrease();
            else if (e.Delta < 0)
                MainScreen.ZoomDecrease();
        }
    }
}
