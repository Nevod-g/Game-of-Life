using System;
using System.Linq;
using System.Windows.Forms;
using static Life.Program;

namespace Life
{
    internal class Scenario
    {
        private Screen screen { get; set; }
        public Scenario(Screen screen) { this.screen = screen; }
        public bool IsStopped { get; set; }

        public int AnimationInterval = 200;
        public int animationFrameIndex = 0;
        readonly Timer animationTimer = new Timer() { Enabled = false };

        public void Run()
        {            
            screen.StartDrawing();
            animationTimer.Tick += AnimationTick;
            animationTimer.Interval = AnimationInterval;
            animationTimer.Start();

            while (!MainScenario.IsStopped)
                Application.DoEvents();

            //sw.Stop();
            //MessageBox.Show($"{sw.ElapsedMilliseconds / 1000}");
        }

        public void AnimationTick(object sender, EventArgs e)
        {
            if (animationFrameIndex < 4) animationFrameIndex += 1; else animationFrameIndex = 0;

            // Каждый пятый цикл
            if (animationFrameIndex==4)
            {   // Колония переживает новую стадию
                screen.CalcCellsStage();

                // Вычислить FPS
                screen.CalcFps();
            }
            else
            {  // Вычислить состояния анимации
                screen.CalcCellsAnimation();
            }
        }

        public void Stop()
        {
            IsStopped = true;
            screen.Dispose();
        }
    }
}
