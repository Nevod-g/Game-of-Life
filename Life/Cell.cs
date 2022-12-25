using System;
using System.Drawing;
using System.Linq;

namespace Life
{
    public class Cell
    {
        static readonly Color reviveColor = Color.Green;
        static readonly Color killedColor = Color.Red;
        static readonly Color staticColor = Color.Blue;
        static readonly Color impostorColor = Color.White;
        /// <summary>
        /// Максимальный возраст клетки.
        /// </summary>
        const int AGE_MAX = Screen.ANIMATION_FPS * Screen.ANIMATION_SPEED_STEP;        

        public Cell(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Value { get; set; }
        public int NeighbourCount; // Количество живых соседей
        public Color Color
        {
            get
            {
                if (IsImpostor) return impostorColor;
                if (Value == AGE_MAX) return staticColor;
                if (Value > 0) return reviveColor;
                else if (Value < 0) return killedColor;
                throw new Exception("Dead cells do not need to be drawn.");
            }
        }

        /// <summary>
        /// Самозванец, клетка сгенерированная случайным образом.
        /// Самозванцы переживают первую стадию в любом случае.
        /// </summary>
        public bool IsImpostor;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isImpostor">Самозванец</param>
        public void Revive(bool isImpostor = false)
        {
            Value = 1;
            IsImpostor = isImpostor;
        }

        public void Kill()
        {
            Value = -Screen.ANIMATION_FPS;
        }

        public void CalcAnimation()
        {
            // Живые стареют до максимального возраста
            if (Value > 0 & Value < AGE_MAX) Value += 1;
            // Значение Мёртвых сводится до нуля, в результате они пропадают.
            else if (Value < 0) Value += 1;
        }

        public void CalcStage()
        {
            // Фича: Самозванцы переживают первую стадию в любом случае.
            if (IsImpostor) { IsImpostor = !IsImpostor; return; }

            // Основная логика поведения клеток
            if (Value > 0 & NeighbourCount > 1 & NeighbourCount < 4) return; // Клетка продолжает жить
            else if (Value <= 0 & NeighbourCount == 3) Revive(); // Клетка оживает
            else if (Value > 0)
            { // Клетка погибает
                if (IsLoneliness()) Kill();
                else if (IsOverpopulation()) Kill();
            }
        }

        /// <summary>
        /// Одиночество: меньше двух соседей.
        /// </summary>
        public bool IsLoneliness()
        {
            if (NeighbourCount < 2) return true;
            return false;
        }

        /// <summary>
        /// Перенаселение: больше трёх соседей.
        /// </summary>
        public bool IsOverpopulation()
        {
            if (NeighbourCount > 3) return true;
            return false;
        }
    }
}
