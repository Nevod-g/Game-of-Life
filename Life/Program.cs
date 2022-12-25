using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Life
{
    static class Program
    {
        public static Screen MainScreen;
        public static Scenario MainScenario;
        static void Main()
        {
            MainScreen = new Screen();
            MainScreen.Initialize();
            MainScenario = new Scenario(MainScreen);
            MainScenario.Run();
        }
    }
}
