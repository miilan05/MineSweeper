using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace MineSwepper
{
    public partial class MineSweeper : Form
    {
        public MineSweeper()
        {
            InitializeComponent();
        }
        static Button[,] Buttons;
        static int boardWidth = 10;
        static int boardHeight = 10;
        static int mineCount = 10;
        static int cubeWidth = 30;
        static int cubeHeight = 30;
        static int counter = 0;
        private void Form1_Load(object sender, EventArgs e)
        {
            createBoard(boardWidth, boardHeight, cubeWidth, cubeHeight);
            HashSet<Position> minePositions = generateRandomPositions(boardWidth, boardHeight, mineCount);
            minePositions.ToList().ForEach(position => Buttons[position.y, position.x].Tag = "-1");
            assignTags();
            resizeElements();
        }
        void createBoard(int boardWidth, int boardHeight, int cubeWidth, int cubeHeight)
        {
            Buttons = new Button[boardWidth, boardHeight];
            for (int i = 0; i < boardWidth; i++) {
                for (int j = 0; j < boardHeight; j++) {
                    Button button = new Button {
                        Name = $"{j}_{i}",
                        Size = new Size(cubeWidth, cubeHeight),
                        Location = new Point(i * cubeWidth, j * cubeHeight + 84),
                        BackColor = Color.LightGray,
                        FlatStyle = FlatStyle.Standard,
                        Tag = "0",
                        Font = new Font("Arial", 12, FontStyle.Bold),
                        Margin = new Padding(0, 0, 0, 0),
                    };
                    button.FlatAppearance.BorderColor = Color.DimGray;
                    button.MouseDown += btnClick;
                    Buttons[i, j] = button;
                    this.Controls.Add(button);
                }
            }
        }

        void resizeElements() {
            this.Size = new Size(boardWidth * cubeWidth + 17, boardHeight * cubeHeight + 124);
            label2.Location = new Point(10, 34);
            button2.Location = new Point(boardWidth * cubeWidth / 2 - button2.Width / 2, 34);
            label3.Location = new Point(boardWidth * cubeWidth - 10 - label3.Width, 34);
            label2.Text = mineCount.ToString("D" + 3);
        }
        static HashSet<Position> generateRandomPositions(int maxX, int maxY, int n) 
        {
            HashSet<Position> positions = new HashSet<Position>();
            Random r = new Random();
            for (int i = 0;i < n; i++)
            {
                if (!positions.Add(new Position(r.Next(maxY), r.Next(maxX)))) i--;
            }
            return positions;
        }

        static void assignTags()
        {
            int numRows = Buttons.GetLength(0);
            int numCols = Buttons.GetLength(1);

            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    if (Buttons[i, j].Tag as string != "-1") Buttons[i, j].Tag = getNeighboringBombsCount(j, i, "-1").ToString();
                }
            }
        }

        static int getNeighboringBombsCount(int x, int y, string bomb)
        {
            int counter = 0;
            for (int i = -1; i <= 1; i++) 
            {
                for (int j =  -1; j <= 1; j++) 
                {
                    int newX = x + j;
                    int newY = y + i;
                    if ((i == 0 && j == 0) || newY < 0 || newX < 0 || newY > Buttons.GetLength(0) - 1 || newX > Buttons.GetLength(1) - 1) continue;
                    if (Buttons[newY, newX].Tag as string == bomb) counter++;
                }
            }
            return counter;
        }
        void btnClick(object sender, MouseEventArgs e) 
        {
            Button btn = (sender as Button);
            string tag = btn.Tag as string;
            if ((btn.Text != "" && (btn.Text != "🚩")) || tag == "-") return;
            if (tag == "0" && e.Button == MouseButtons.Left) revealAllZeros(Convert.ToInt16(btn.Name.Split('_')[0]), Convert.ToInt16(btn.Name.Split('_')[1]), new HashSet<Position>());
            else if (tag == "-1" && e.Button == MouseButtons.Left)
            {
                revealAll();
                timer1.Stop();
                MessageBox.Show("You lost :( ");
                return;
            }
            else btn.Text = (e.Button == MouseButtons.Right) ? "🚩" : tag == "-1" ? "💣" : tag;
            if (e.Button == MouseButtons.Left) btn.FlatStyle = FlatStyle.Flat;
            showColor(btn);
            label1.Focus();
            label2.Text = countRemainingFlags().ToString();
            if (checkForWin()) { timer1.Stop(); MessageBox.Show("You won!");  }
        }

        static bool checkForWin()
        {
            return !Buttons.Cast<Button>().ToList().Any(btn => (btn.Text == "" && btn.Tag as string != "-") || (btn.Text == "🚩" && btn.Tag as string != "-1"));
        }

        static int countRemainingFlags()
        {
            return mineCount - Buttons.Cast<Button>().ToList().Count(btn => (btn.Text == "🚩"));

        }

        static void showColor(Button btn)
        {
            string tag = btn.Tag as string;
            btn.ForeColor = (tag == "-1" || btn.Text == "🚩") ? Color.Black : tag == "0" ? Color.Black : tag == "1" ? Color.Blue : tag == "2" ? Color.Green : Color.DarkRed;
        }

        static void revealAllZeros(int x, int y, HashSet<Position> checkedSet)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue;

                    int newX = x + j;
                    int newY = y + i;

                    if (newX < 0 || newY < 0 || newX >= Buttons.GetLength(1) || newY >= Buttons.GetLength(0)) continue;

                    Position newPosition = new Position(newX, newY);

                    if (checkedSet.Contains(newPosition)) continue;

                    Button button = Buttons[newY, newX];
                    string btnTag = button.Tag as string;

                    showColor(button);
                    button.Text = btnTag;
                    button.FlatStyle = FlatStyle.Flat;

                    checkedSet.Add(newPosition);

                    if (btnTag != "0") continue;
                        
                    button.Text = "";
                    button.Tag = "-";
                    revealAllZeros(newX, newY, checkedSet);
                }
            }
        }

        static void revealAll()
        {
            foreach (Button btn in Buttons)
            {
                if (btn.Tag as string == "-1")
                {
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.Text = "💣";
                    btn.ForeColor = Color.Black;
                }
            }
        }

        public class Position {
            public Position(int x, int y) {
                this.x = x;
                this.y = y;
            }
            public int x { get; set; }
            public int y { get; set; }
            public override bool Equals(object obj) {
                if (obj == null || GetType() != obj.GetType()) return false;
                Position other = (Position)obj;
                return x == other.x && y == other.y;
            }

            public override int GetHashCode() {
                unchecked {
                    int hash = 17;
                    hash = hash * 23 + x.GetHashCode();
                    hash = hash * 23 + y.GetHashCode();
                    return hash;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            restart();
        }
        void restart()
        {
            foreach (Button btn in Buttons)
            {
                btn.BackColor = Color.LightGray;
                btn.FlatStyle = FlatStyle.Standard;
                btn.Tag = "0";
                btn.Text = "";
            }
            HashSet<Position> minePositions = generateRandomPositions(boardWidth, boardHeight, mineCount);
            minePositions.ToList().ForEach(position => Buttons[position.y, position.x].Tag = "-1");
            assignTags();
            label3.Text = "000";
            counter = 0;
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            counter++;
            label3.Text = counter.ToString("D" + 3);
        }

        void removeAll()
        {
            foreach (Button btn in Buttons) Controls.Remove(btn);
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            restart();
        }

        private void begginnerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            boardHeight = 8;
            boardWidth = 8;
            mineCount = 8;
            removeAll();
            createBoard(boardWidth, boardHeight, cubeWidth, cubeHeight);
            resizeElements();
            restart();
        }

        private void intermediateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            boardHeight = 14;
            boardWidth = 14;
            mineCount = 20;
            removeAll();
            createBoard(boardWidth, boardHeight, cubeWidth, cubeHeight);
            resizeElements();
            restart();
        }

        private void expertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            boardHeight = 30;
            boardWidth = 30;
            mineCount = 99;
            removeAll();
            createBoard(boardWidth, boardHeight, cubeWidth, cubeHeight);
            resizeElements();
            restart();
        }

        private void customToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.FormClosing += Form2_FormClosing;
            form2.Show();
        }
        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form2 form2 = (Form2)sender;
            if (form2.cancel) return;
            boardHeight = form2.height;
            boardWidth = form2.width;
            mineCount = form2.mines;
            removeAll();
            createBoard(boardWidth, boardHeight, cubeWidth, cubeHeight);
            resizeElements();
            restart();
        }
    }
}
