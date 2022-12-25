using System;
using System.Drawing;
using System.Linq;

namespace Life
{
    public class Cell
    {
        static readonly Color reviveColor = Color.Green;
        static readonly Color killedColor = Color.Red;

        public Cell(int x, int y)
        {
            X = x;
            Y = y;
        }
        public int X { get; set; }
        public int Y { get; set; }
        public int Value { get; set; }
        public Color Color
        {
            get
            {
                switch (Value)
                {
                    case 1: return reviveColor;
                    case -1: return killedColor;
                    default: throw new Exception ("Dead cells do not need to be drawn.");
                }
            }
        }

        public void Revive()
        {
            Value = 1;
        }

        public void Kill()
        {
            Value = -1;
        }
    }
}
