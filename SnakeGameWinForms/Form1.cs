using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SnakeGameWinForms
{
    public partial class Form1 : Form
    {
        private Timer gameTimer = new Timer();
        private List<Point> snake = new List<Point>();
        private Point food;
        private Size gridSize = new Size(20, 20); // 20x20 cells
        private int cellSize = 20;
        private Point direction = new Point(1, 0); // Initially moving right
        private Random random = new Random();
        private bool gameOver = false;

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.Width = gridSize.Width * cellSize + 16;
            this.Height = gridSize.Height * cellSize + 39;
            this.Text = "Snake Game (.NET Framework)";

            InitGame();

            gameTimer.Interval = 100; // milliseconds
            gameTimer.Tick += GameTick;
            gameTimer.Start();

            this.KeyDown += new KeyEventHandler(OnKeyDown);
            this.Paint += new PaintEventHandler(OnPaint);
        }

        private void InitGame()
        {
            snake.Clear();
            snake.Add(new Point(gridSize.Width / 2, gridSize.Height / 2));
            direction = new Point(1, 0);
            gameOver = false;
            SpawnFood();
        }

        private void GameTick(object sender, EventArgs e)
        {
            if (gameOver)
                return;

            MoveSnake();
            CheckCollision();
            this.Invalidate(); // Triggers OnPaint
        }

        private void MoveSnake()
        {
            Point newHead = new Point(
                snake[snake.Count - 1].X + direction.X,
                snake[snake.Count - 1].Y + direction.Y
            );

            // Eat food
            if (newHead == food)
            {
                snake.Add(newHead);
                SpawnFood();
            }
            else
            {
                // Move
                snake.RemoveAt(0);
                snake.Add(newHead);
            }
        }

        private void CheckCollision()
        {
            Point head = snake[snake.Count - 1];

            // Wall collision
            if (head.X < 0 || head.Y < 0 || head.X >= gridSize.Width || head.Y >= gridSize.Height)
            {
                gameOver = true;
                gameTimer.Stop();
                MessageBox.Show("Game Over! You hit the wall.");
            }

            // Self collision
            for (int i = 0; i < snake.Count - 1; i++)
            {
                if (snake[i] == head)
                {
                    gameOver = true;
                    gameTimer.Stop();
                    MessageBox.Show("Game Over! You hit yourself.");
                }
            }
        }

        private void SpawnFood()
        {
            do
            {
                food = new Point(
                    random.Next(0, gridSize.Width),
                    random.Next(0, gridSize.Height)
                );
            } while (snake.Contains(food));
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Draw grid (optional)
            // for (int x = 0; x < gridSize.Width; x++)
            //     for (int y = 0; y < gridSize.Height; y++)
            //         g.DrawRectangle(Pens.Gray, x * cellSize, y * cellSize, cellSize, cellSize);

            // Draw snake
            foreach (var part in snake)
            {
                g.FillRectangle(Brushes.Green, part.X * cellSize, part.Y * cellSize, cellSize, cellSize);
            }

            // Draw food
            g.FillRectangle(Brushes.Red, food.X * cellSize, food.Y * cellSize, cellSize, cellSize);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (direction.Y != 1) direction = new Point(0, -1);
                    break;
                case Keys.Down:
                    if (direction.Y != -1) direction = new Point(0, 1);
                    break;
                case Keys.Left:
                    if (direction.X != 1) direction = new Point(-1, 0);
                    break;
                case Keys.Right:
                    if (direction.X != -1) direction = new Point(1, 0);
                    break;
                case Keys.Space when gameOver:
                    InitGame();
                    gameTimer.Start();
                    break;
            }
        }
    }
}
