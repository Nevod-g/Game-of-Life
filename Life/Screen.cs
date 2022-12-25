using Life.Tools;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Life.Program;

namespace Life
{
    public class Screen
    {
        public Screen() { }
        Painter mainPainter;
        Graphics mainGraphics;
        Painter framePainter;
        Graphics frameGraphics;

        Bitmap frame;
        string fps;

        int width;
        int height;
        int cellWith = 16;
        int cellHeight = 16;
        int cellsCountHorizontal;
        int cellsCountVertical;
        Cell[,] Cells;

        readonly Random rnd = new Random();
        readonly Form form = new Form();
        //readonly BackgroundWorker worker = new BackgroundWorker();
        Stopwatch sw = new Stopwatch();
        public void Initialize()
        {
            form.WindowState = FormWindowState.Maximized;
            form.ShowInTaskbar = true;
            form.ShowIcon = true;
            form.ControlBox = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Show();

            mainPainter = new Painter();
            mainGraphics = new Painter().Initialize(form);
            width = form.Width;
            height = form.Height;

            CreateCells();

            frame = new Bitmap(width, height);
            framePainter = new Painter();
            frameGraphics = framePainter.Initialize(frame);

            // Controller Handlers
            form.KeyDown += Controller.KeyDown;
            form.MouseDown += Controller.MouseDown;
            form.MouseUp += Controller.MouseUp;
            form.MouseMove += Controller.MouseMove;
            form.MouseHover += Controller.MouseHover;
            form.MouseWheel += Controller.MouseWheel;
        }

        /// <summary>
        /// Create new Cells
        /// </summary>
        /// <param name="cellWith"></param>
        /// <param name="cellHeight"></param> 
        public void CreateCells(int cellWith = 16, int cellHeight = 16)
        {
            this.cellWith = cellWith;
            this.cellHeight = cellHeight;

            // Calc properties
            cellsCountHorizontal = width / cellWith;
            cellsCountVertical = height / cellHeight;
            if (Cells is null)
            {
                Cells = new Cell[cellsCountHorizontal, cellsCountVertical];
                for (int i = 0; i < Cells?.GetLength(0); i++)
                    for (int j = 0; j < Cells?.GetLength(1); j++)
                    {
                        Cells[i, j] = new Cell(i, j);
                    }
            }
            else
            {
                var oldCells = (Cell[,])Cells.Clone();
                Cells = new Cell[cellsCountHorizontal, cellsCountVertical];
                for (int i = 0; i < Cells?.GetLength(0); i++)
                    for (int j = 0; j < Cells?.GetLength(1); j++)
                    {
                        if (i < oldCells.GetLength(0) & j < oldCells.GetLength(1))
                            Cells[i, j] = oldCells[i, j];
                        else
                            Cells[i, j] = new Cell(i, j);
                    }
            }

        }

        public void Clear() { frameGraphics.Clear(Color.Black); }

        public async void StartDrawing()
        {
            while (!MainScenario.IsStopped)
            {
                await Task.Run(() => UpdateFrame());
                await Task.Run(() => UpdateScreen());
            }
        }

        private void UpdateFrame()
        {
            Clear();
            DrawCells();
            //Thread.Sleep(100);
        }

        public void UpdateScreen()
        {
            // Вывести FPS
            //framePainter.DrawText(fps, 4, 4, 9, Color.Black, Color.Black);            
            fps = $"FPS: {1000 / (sw.ElapsedMilliseconds + 1)}";
            sw.Restart();
            framePainter?.DrawText(fps, 4, 4, 9, Color.White, Color.Black);

            // Отобразить фрейм на экране
            mainGraphics?.DrawImage(frame, 0, 0);
        }

        public void DrawCells()
        {
            for (int i = 0; i < Cells?.GetLength(0); i++)
                for (int j = 0; j < Cells?.GetLength(1); j++)
                {
                    Cell cell;
                    try
                    {
                        cell = Cells[i, j];
                    }
                    catch (Exception)
                    {
                        return;
                    }

                    if (!(cell is null) & cell?.Value != 0)
                    {
                        var darkColor = Painter.GetClarifierColor(cell.Color, .2);
                        Pen pen = new Pen(darkColor, 1);
                        SolidBrush brush = new SolidBrush(cell.Color);
                        var x = i * cellWith;
                        var y = j * cellHeight;
                        Rectangle rect = new Rectangle(x, y, cellWith, cellHeight);
                        framePainter.FillRoundedRectangle(brush, rect, cellWith / 4);
                        framePainter.DrawRoundedRectangle(pen, rect, cellWith / 4);
                    }
                }
        }

        public void ReviveCell(int x, int y)
        {
            int i = x / cellWith;
            int j = y / cellHeight;
            if (i < 0) i = 0;
            if (j < 0) j = 0;
            if (i >= cellsCountHorizontal) i = cellsCountHorizontal - 1;
            if (j >= cellsCountVertical) j = cellsCountVertical - 1;
            Cells[i, j].Revive();
        }

        public void KillCell(int x, int y)
        {
            int i = x / cellWith;
            int j = y / cellHeight;
            if (i < 0) i = 0;
            if (j < 0) j = 0;
            if (i >= cellsCountHorizontal) i = cellsCountHorizontal - 1;
            if (j >= cellsCountVertical) j = cellsCountVertical - 1;
            Cells[i, j].Kill();
        }

        public void Dispose()
        {
            mainGraphics.Dispose();
            frameGraphics.Dispose();
            form.Close();
        }

        public void ZoomIncrease()
        {
            if (cellWith < 64)
            {
                cellWith *= 2;
                cellHeight *= 2;
                CreateCells(cellWith, cellHeight);
            }
        }

        public void ZoomDecrease()
        {
            if (cellWith > 2)
            {
                cellWith /= 2;
                cellHeight /= 2;
                CreateCells(cellWith, cellHeight);
            }
        }
    }
}
