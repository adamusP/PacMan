using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSHra
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;

            timer1.Interval = 16;

        }

        Map map;
        Graphics g;
        private void button1_Click(object sender, EventArgs e)
        {
            g = CreateGraphics(); 
            map = new Map("map.txt", "icons.png");

            timer1.Enabled = true;
            button1.Visible = false;
            label1.Visible = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            switch (map.state)
            {
                case State.runs:
                    map.MoveEverything(pressedArrow);
                    Invalidate();
                    break;
                case State.runsBack:
                    map.MoveEverything(pressedArrow);
                    Invalidate();
                    map.timerChange();
                    break;
                case State.ready:
                    Invalidate();
                    map.timerChange();
                    break;
                case State.loss:
                    timer1.Enabled = false;
                    break;
                default:
                    break;
            }
        }

        PressedArrow pressedArrow = PressedArrow.none;

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Up)
            {
                pressedArrow = PressedArrow.up;
                return true;
            }
            if (keyData == Keys.Down)
            {
                pressedArrow = PressedArrow.down;
                return true;
            }
            if (keyData == Keys.Left)
            {
                pressedArrow = PressedArrow.left;
                return true;
            }
            if (keyData == Keys.Right)
            {
                pressedArrow = PressedArrow.right;
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            pressedArrow = PressedArrow.none;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (map != null)
            {
                map.Draw(e.Graphics);
            }
        }
    }
}
