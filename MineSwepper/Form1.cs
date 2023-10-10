using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
        private void Form1_Load(object sender, EventArgs e)
        {
            int boardWidth = 10;
            int boardHeight = 10;
            int cubeWidth = 50;
            int cubeHeight = 50;
            
            this.Size = new Size(boardWidth * cubeWidth + 17, boardHeight * cubeHeight + 40);
            Buttons = new Button[boardWidth, boardHeight];

            createBoard(boardWidth, boardHeight, cubeWidth, cubeHeight);
            HashSet<Position> minePositions = generateRandomPositions(boardWidth, boardHeight, 10);
            minePositions.ToList().ForEach(position => Buttons[position.y, position.x].Tag = "-1");
            assignTags();
        }
        void createBoard(int boardWidth, int boardHeight, int cubeWidth, int cubeHeight) 
        {
            for (int i = 0; i < boardWidth; i++) {
                for (int j = 0; j < boardHeight; j++) {
                    Button button = new Button {
                        Name = $"{j}_{i}",
                        Size = new Size(cubeWidth, cubeHeight),
                        BackColor = Color.Coral,
                        Location = new Point(i * cubeWidth, j * cubeHeight),
                        FlatStyle = FlatStyle.Flat,
                        Font = new Font("Arial", 24),
                        Tag = "0"
                    };

                    button.MouseDown += btnClick;
                    Buttons[i, j] = button;
                    this.Controls.Add(button);
                }
            }
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
                    if (Buttons[i, j].Tag as string == "-1") continue;
                    Buttons[i, j].Tag = getNeighboringBombsCount(j, i, "-1").ToString();
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
            showColor(btn);
            if (tag == "0") revealAllZeros(Convert.ToInt16(btn.Name.Split('_')[0]), Convert.ToInt16(btn.Name.Split('_')[1]), new HashSet<Position>());
            btn.Text = (e.Button == MouseButtons.Right) ? "🚩" : tag == "-1" ? "💣" : tag;
            if (tag == "-1" && e.Button == MouseButtons.Left) { revealAll(); MessageBox.Show("You lost :( "); };
            label1.Focus();
        }
        static void showColor(Button btn)
        {
            string tag = btn.Tag as string;
            btn.ForeColor = tag == "-1" ? Color.Black : tag == "0" ? Color.Black : tag == "1" ? Color.Blue : tag == "2" ? Color.Green : Color.DarkRed;
        }

        static void revealAllZeros(int x, int y, HashSet<Position> checkedSet)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int newX = x + j;
                    int newY = y + i;
                    if ((i == 0 && j == 0) || newY < 0 || newX < 0 || newY > Buttons.GetLength(0) - 1 || newX > Buttons.GetLength(1) - 1) continue;
                    string btnTag = Buttons[newY, newX].Tag as string;
                    showColor(Buttons[newY, newX]);
                    Buttons[newY, newX].Text = btnTag;
                    if (btnTag == "0")
                    {
                        if (checkedSet.Add(new Position(newX, newY))) revealAllZeros(newX, newY, checkedSet);
                    }
                }
            }

        }

        static void revealAll()
        {
            foreach (Button btn in Buttons) btn.Text = btn.Tag as string == "-1" ? "💣" : btn.Tag as string;
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
    }
}
