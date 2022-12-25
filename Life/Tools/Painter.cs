using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;

namespace Life.Tools
{
    public class Painter
    {
        Graphics graphics = null;

        public Graphics Initialize(Form form)
        {
            graphics = form.CreateGraphics();
            SetDefaultParameters();
            return graphics;
        }

        public Graphics Initialize(Bitmap bitmap)
        {
            graphics = Graphics.FromImage(bitmap);
            SetDefaultParameters();
            return graphics;
        }

        private void SetDefaultParameters()
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            graphics.Clear(Color.Black);
        }

        public void DrawRoundedRectangle(Pen pen, Rectangle rect, int cornerRadius)
        {
            using (GraphicsPath path = RoundedRect(rect, cornerRadius))
                graphics?.DrawPath(pen, path);
        }

        public void FillRoundedRectangle(Brush brush, Rectangle rect, int cornerRadius)
        {
            using (GraphicsPath path = RoundedRect(rect, cornerRadius))
                graphics?.FillPath(brush, path);
        }

        private GraphicsPath RoundedRect(Rectangle rect, int radius)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(rect.Location, size);
            GraphicsPath path = new GraphicsPath();

            if (radius == 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            // top left arc  
            path.AddArc(arc, 180, 90);

            // top right arc  
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);

            // bottom right arc  
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // bottom left arc 
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        public void DrawText(string text, int x, int y, int size,
            Color foreColor, Color backColor = default,
            string fontName = "Calibri", int shadowShift = 1)
        {
            if (backColor == default) backColor = Color.Gray;
            var drawFormat = new StringFormat
            {
                FormatFlags = StringFormatFlags.NoWrap,
                Alignment = StringAlignment.Near
            };
            var fontBrush = new SolidBrush(foreColor);
            var backBrush = new SolidBrush(backColor);
            var fontText = new Font(fontName, size);

            try
            {
                graphics.DrawString(text, fontText, backBrush, x + shadowShift, y + shadowShift);
                graphics.DrawString(text, fontText, fontBrush, x, y);
            }
            catch (Exception) { return; }
        }

        /// <summary>
        /// Получить более светлый или тёмный цвет.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="clarifier">Множитель, значение от 0 до 2.</param>
        /// <param name="increase">Слагаемое, нормальное значение от -255 до +255</param>
        /// <returns></returns>
        public static Color GetClarifierColor(Color color, double clarifier, int increase = 0)
        {
            var r = (int)(color.R * clarifier) + increase;
            var g = (int)(color.G * clarifier) + increase;
            var b = (int)(color.B * clarifier) + increase;
            if (r > 255) r = 255;
            if (g > 255) g = 255;
            if (b > 255) b = 255;
            if (r < 0) r = 0;
            if (g < 0) g = 0;
            if (b < 0) b = 0;
            return Color.FromArgb(r, g, b);
        }
    }
}
