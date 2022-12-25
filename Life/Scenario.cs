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
        public int StageInterval = 1000;
        Timer stageTimer = new Timer() { Enabled = false };
        public void Run()
        {            
            screen.StartDrawing();
            stageTimer.Tick += StageTick;
            stageTimer.Interval = StageInterval;
            stageTimer.Start();

            while (!MainScenario.IsStopped)
                Application.DoEvents();

            //sw.Stop();
            //MessageBox.Show($"{sw.ElapsedMilliseconds / 1000}");
        }

        public void StageTick(object sender, EventArgs e)
        {

        }

        public void Stop()
        {
            IsStopped = true;
            screen.Dispose();
        }
    }
}
