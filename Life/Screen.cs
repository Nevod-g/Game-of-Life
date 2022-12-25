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

        public const int ANIMATION_FPS = 5;
        string fpsText;
        int fps;
        int animationSpeed;
        int fpsCounter; // Счётчик FPS

        int width;
        int height;
        int cellWith = 16;
        int cellHeight = 16;
        int cellsCountHorizontal;
        int cellsCountVertical;
        Cell[,] Cells;

        /// <summary>
        /// Шаг изменения скорости анимации.
        /// </summary>
        public const int ANIMATION_SPEED_STEP = 10;
        public const int ANIMATION_INTERVAL_MAX = 2000;

        readonly Random rnd = new Random();
        readonly Form form = new Form();
        //readonly BackgroundWorker worker = new BackgroundWorker();
        readonly Stopwatch sw = new Stopwatch();
        public void Initialize()
        {
            form.WindowState = FormWindowState.Maximized;
            form.ShowInTaskbar = true;
            form.ShowIcon = true;
            form.ControlBox = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Icon = Life.Properties.Resources.gol;
            form.Text = Application.ProductName;
            form.Show();

            mainPainter = new Painter();
            mainGraphics = mainPainter.Initialize(form);
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
                await Task.Run(() => DrawFrame());
                await Task.Run(() => UpdateScreen());
            }
        }

        /// <summary>
        /// Рисовать кадр.
        /// </summary>
        private void DrawFrame()
        {
            Clear();
            try
            {
                DrawCells();
            }
            catch (Exception) { return; }
        }

        public void UpdateScreen()
        {
            try
            {
                // Вывести FPS
                fpsCounter += 1;
                framePainter?.DrawText(fpsText, 4, 4, 9, Color.White, Color.Black);

                // Отобразить фрейм на экране
                mainGraphics?.DrawImage(frame, 0, 0);
            }
            catch (Exception) { return; }
        }

        public void CalcCellsAnimation()
        {
            for (int i = 0; i < Cells?.GetLength(0); i++)
                for (int j = 0; j < Cells?.GetLength(1); j++)
                {
                    Cell cell = Cells[i, j];
                    cell.CalcAnimation();
                }
        }

        public void CalcCellsStage()
        {
            CalcNeighbourCount();

            for (int i = 0; i < Cells?.GetLength(0); i++)
                for (int j = 0; j < Cells?.GetLength(1); j++)
                {
                    Cell cell = Cells[i, j];
                    cell.CalcStage();
                }
        }

        /// <summary>
        /// Подсчитать количество живых соседей для каждой клетки.
        /// </summary>
        public void CalcNeighbourCount()
        {
            for (int i = 0; i < Cells?.GetLength(0); i++)
                for (int j = 0; j < Cells?.GetLength(1); j++)
                {
                    int neighbourCount = 0;
                    // Перебрать соседей
                    for (int ii = i - 1; ii <= i + 1; ii++)
                        for (int jj = j - 1; jj <= j + 1; jj++)
                        {
                            if (ii != i | jj != j) // Не считать себя
                            {
                                int x = ii;
                                int y = jj;
                                // Телепортироваться через границы массива
                                if (x < 0) x = Cells.GetLength(0) - 1;
                                if (y < 0) y = Cells.GetLength(1) - 1;
                                if (x >= Cells.GetLength(0)) x = 0;
                                if (y >= Cells.GetLength(1)) y = 0;
                                if (Cells[x, y].Value > 0) neighbourCount++;
                            }
                        }
                    Cells[i, j].NeighbourCount = neighbourCount;
                }
        }

        public void CalcFps()
        {
            fps = (int)(fpsCounter * 1000 / (sw.ElapsedMilliseconds + 1));
            animationSpeed = (ANIMATION_INTERVAL_MAX - MainScenario.AnimationSpeed) / 10;
            fpsText = $"FPS: {fps} Speed: {animationSpeed}"; //({fpsCounter}/{sw.ElapsedMilliseconds})";
            sw.Restart(); fpsCounter = 0;
        }

        public void DrawCells()
        {
            for (int i = 0; i < Cells?.GetLength(0); i++)
                for (int j = 0; j < Cells?.GetLength(1); j++)
                {
                    Cell cell = Cells[i, j];

                    if (!(cell is null) && cell.Value != 0)
                    {
                        int cellWithAnimation = cellWith;
                        int cellHeightAnimation = cellHeight;
                        int cellShiftXAnimation = 0;
                        int cellShiftYAnimation = 0;
                        // Умирающие клетки гаснут (постепенно уменьшаются в размере)
                        double cellScale = 1;
                        if (cell.Value < 0 && cell.Value > -Screen.ANIMATION_FPS)
                        {
                            cellScale = (double)(ANIMATION_FPS + cell.Value) / ANIMATION_FPS; // Value = -1 to -5
                            cellWithAnimation = (int)Math.Round(cellWithAnimation * (1 - cellScale));
                            cellHeightAnimation = (int)Math.Round(cellHeightAnimation * (1 - cellScale));
                            cellShiftXAnimation = (cellWith - cellWithAnimation) / 2;
                            cellShiftYAnimation = (cellHeight - cellHeightAnimation) / 2;
                        }

                        SolidBrush brush = new SolidBrush(cell.Color);
                        var x = i * cellWith;
                        var y = j * cellHeight;
                        Rectangle rect = new Rectangle(
                            x + cellShiftXAnimation, y + cellShiftYAnimation,
                            cellWithAnimation, cellHeightAnimation);
                        framePainter.FillRoundedRectangle(brush, rect, cellWith / 4);
                        if (cell.Value != 0 && cellWith > 2)
                        { // Рисовать контур, только если ячейка достаточно большая
                            var darkColor = Painter.GetClarifierColor(cell.Color, .2);
                            Pen pen = new Pen(darkColor, 1);
                            framePainter.DrawRoundedRectangle(pen, rect, cellWith / 4);
                        }
                        if (cell.Value > 0 & cellWith == 16 & cell.NeighbourCount > 0)
                        {
                            // Показать количество соседей.
                            framePainter.DrawText(cell.NeighbourCount.ToString(), x + 2, y - 2, 12, Color.White);
                        }
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
                cellWith += 2;
                cellHeight += 2;
                CreateCells(cellWith, cellHeight); // todo: добавить зум а не Resize
            }
        }
        public void ZoomDecrease()
        {
            if (cellWith > 2)
            {
                cellWith -= 2;
                cellHeight -= 2;
                CreateCells(cellWith, cellHeight);
            }
        }

        public void SpeedIncrease()
        {
            if (MainScenario.AnimationSpeed > ANIMATION_SPEED_STEP)
                MainScenario.AnimationSpeed -= ANIMATION_SPEED_STEP;
        }
        public void SpeedDecrease()
        {
            if (MainScenario.AnimationSpeed < ANIMATION_INTERVAL_MAX)
                MainScenario.AnimationSpeed += ANIMATION_SPEED_STEP;
        }

        public void CreateImpostor()
        {
            var x = rnd.Next(Cells.GetLength(0) - 1);
            var y = rnd.Next(Cells.GetLength(1) - 1);
            Cells[x, y].Revive(true);
        }
    }
}
