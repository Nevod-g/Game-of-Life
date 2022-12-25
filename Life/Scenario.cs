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

        readonly Random rnd = new Random();
        public int AnimationInterval = 200;
        public int animationFrameIndex = 0;
        public readonly Timer AnimationTimer = new Timer() { Enabled = false };

        // Скорость анимации, как интервал (в ms) между вычислением стадий клеток.
        public int AnimationSpeed
        {
            get => animationSpeed;
            set
            {
                animationSpeed = value;
                AnimationInterval = animationSpeed / Screen.ANIMATION_FPS;
                AnimationTimer.Interval = AnimationInterval;
            }
        }
        private int animationSpeed = 1000;

        public void Run()
        {
            screen.StartDrawing();
            AnimationTimer.Tick += AnimationTick;
            AnimationTimer.Interval = AnimationInterval;
            AnimationTimer.Start();

            while (!MainScenario.IsStopped)
                Application.DoEvents();

            //sw.Stop();
            //MessageBox.Show($"{sw.ElapsedMilliseconds / 1000}");
        }

        public void AnimationTick(object sender, EventArgs e)
        {
            if (animationFrameIndex < 4) animationFrameIndex += 1; else animationFrameIndex = 0;

            // Каждый пятый цикл
            if (animationFrameIndex == 4)
            {   // Колония переживает новую стадию
                screen.CalcCellsStage();

                // Иногда появляется самозванец, если нажат Numlock
                if (Control.IsKeyLocked(Keys.CapsLock))
                {
                    if ((int)(rnd.Next(3)) == 1) screen.CreateImpostor();
                }

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
