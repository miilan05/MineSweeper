using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MineSwepper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        static Button[,] Buttons;
        private void Form1_Load(object sender, EventArgs e)
        {
            int boardWidth = 20;
            int boardHeight = 10;
            int cubeWidth = 50;
            int cubeHeight = 50;
            
            Buttons = new Button[boardWidth, boardHeight];


            this.Size = new Size(boardWidth * cubeWidth + 17, boardHeight * cubeHeight + 40);

            createBoard(boardWidth, boardHeight, cubeWidth, cubeHeight);
        }
        void createBoard(int boardWidth, int boardHeight, int cubeWidth, int cubeHeight)
        {
            for (int i = 0; i < boardWidth; i++)
            {
                for (int j = 0; j < boardHeight; j++)
                {
                    Button button = new Button();
                    button.Size = new Size(cubeWidth, cubeHeight);
                    button.BackColor = Color.Coral;
                    button.Location = new Point(i * cubeHeight, j * cubeWidth);
                    this.Controls.Add(button);
                    Buttons[i, j] = button;
                }
            }
        }
    }
}
