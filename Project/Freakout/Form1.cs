using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Freakout
{
    public partial class Form1 : Form
    {
        // Container for disposable components
        private readonly System.ComponentModel.IContainer components = new System.ComponentModel.Container();

        // Game elements
        private Rectangle paddle;
        private Rectangle ball;
        private readonly List<Rectangle> bricks = new List<Rectangle>();

        // Score tracking
        private uint highScore;
        private uint score = 0;

        // Game state flags
        private bool gameOver = false;
        private bool gameStarted = false;

        // Input tracking
        private bool leftPressed = false;
        private bool rightPressed = false;

        // Ball movement speed
        private int ballX = 8, ballY = -8;

        // Paddle movement speed
        private readonly int paddleSpeed = 10;

        // Timer to drive the game loop
        private readonly Timer gameTimer;

        // Constructor: sets up the form and initializes the game
        public Form1()
        {
            InitializeComponent(); // Basic form setup
            DoubleBuffered = true; // Reduces flickering during rendering

            // Event handlers for input and rendering
            KeyDown += Form1_KeyDown;
            KeyUp += Form1_KeyUp;
            Paint += Form1_Paint;

            // Configure the game timer
            gameTimer = new Timer
            {
                Interval = 20 // ~50 frames per second
            };
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Enabled = false;

            // Start with a fresh game state
            ResetGame();
        }

        // Resets the game state for a new round
        private void ResetGame()
        {
            // Position paddle and ball
            paddle = new Rectangle((ClientSize.Width / 2) - 40, ClientSize.Height - 30, 80, 10);
            ball = new Rectangle((ClientSize.Width / 2) - 10, ClientSize.Height - 50, 20, 20);

            // Clear bricks and score
            bricks.Clear();
            score = 0;
            gameStarted = false;
            gameOver = false;

            // Generate brick layout
            for (int y = 50; y < 150; y += 30)
            {
                for (int x = 20; x < ClientSize.Width - 60; x += 60)
                {
                    bricks.Add(new Rectangle(x, y, 50, 20));
                }
            }

            // Force repaint
            Invalidate();
        }

        // Main game loop: updates game state every tick
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (!gameStarted) return;

            // Move paddle based on input
            if (leftPressed && paddle.Left > 0)
                paddle.X -= paddleSpeed;

            if (rightPressed && paddle.Right < ClientSize.Width)
                paddle.X += paddleSpeed;

            // Move ball
            ball.X += ballX;
            ball.Y += ballY;

            // Ball collision with walls
            if (ball.Left < 0 || ball.Right > ClientSize.Width)
                ballX = -ballX;

            if (ball.Top < 0)
                ballY = -ballY;

            // Ball collision with paddle
            if (ball.IntersectsWith(paddle))
                ballY = -ballY;

            // Ball collision with bricks
            for (int i = bricks.Count - 1; i >= 0; i--)
            {
                if (ball.IntersectsWith(bricks[i]))
                {
                    bricks.RemoveAt(i);
                    ballY = -ballY;
                    score += 10;
                    break;
                }
            }

            // Ball falls below screen — game over
            if (ball.Bottom > ClientSize.Height && !gameOver)
            {
                gameOver = true;
                gameTimer.Stop(); // Stop game loop

                // Update high score if needed
                if (score > highScore)
                    highScore = score;

                // Show game over message
                _ = MessageBox.Show($"Game Over! Your score: {score}", "Freakout");

                // Reset game for next round
                ResetGame();
            }

            // Trigger repaint
            Invalidate();
        }

        // Handles all drawing on the screen
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Show instructions before game starts
            if (!gameStarted)
            {
                g.DrawString("Press SPACE to start\nUse ← and → to control", new Font("Arial", 24), Brushes.Black, 320, 170);
                return;
            }

            // Draw paddle and ball
            g.FillRectangle(Brushes.Blue, paddle);
            g.FillEllipse(Brushes.Red, ball);

            // Draw bricks
            foreach (Rectangle brick in bricks)
                g.FillRectangle(Brushes.Green, brick);

            // Draw score and high score
            g.DrawString($"Score: {score}", new Font("Arial", 12), Brushes.Black, 10, 10);
            g.DrawString("High Score:", new Font("Arial", 10), Brushes.Black, ClientSize.Width - 150, 10);
            g.DrawString($" {highScore}", new Font("Arial", 10), Brushes.Black, ClientSize.Width - 150, 30);
        }

        // Handles key press events
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
                leftPressed = true;

            if (e.KeyCode == Keys.Right)
                rightPressed = true;

            // Start game on spacebar
            if (e.KeyCode == Keys.Space && !gameStarted)
            {
                gameStarted = true;
                gameTimer.Start();
            }
        }

        // Handles key release events
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
                leftPressed = false;

            if (e.KeyCode == Keys.Right)
                rightPressed = false;
        }

        // Basic form setup (called by constructor)
        private void InitializeComponent()
        {
            SuspendLayout();
            ClientSize = new System.Drawing.Size(1043, 424);
            Name = "Form1";
            Text = "Freakout";
            ResumeLayout(false);
        }
    }
}
