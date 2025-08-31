using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Freakout
{
    /// <summary>
    /// A basic bat and ball style game implemented in a Windows Forms application.
    /// </summary>
    public class PadForm : Form
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
        private string gameOverMessage = string.Empty;
        private readonly string gameStartMessage = "Press SPACE to start\nUse ← and → to control";

        // Input tracking
        private bool leftPressed = false;
        private bool rightPressed = false;

        // Ball movement speed
        private int ballX = 7, ballY = -7;

        // Paddle movement speed
        private readonly int paddleSpeed = 12;

        // Timer to drive the game loop
        private readonly System.Windows.Forms.Timer gameTimer;

        // Constructor: sets up the form and initializes the game
        public PadForm()
        {
            InitializeComponent(); // Basic form setup
            DoubleBuffered = true; // Reduces flickering during rendering

            // Event handlers for input and rendering
            KeyDown += Form1_KeyDown;
            KeyUp += Form1_KeyUp;
            Paint += Form1_Paint;

            // Configure the game timer
            gameTimer = new System.Windows.Forms.Timer
            {
                Interval = 20 // ~50 frames per second
            };
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Enabled = false;

            // Start with a fresh game state
            ResetGame();
        }

        /// <summary>
        /// Launches the game form modally from external DLL calls.
        /// This method handles the Windows Forms message loop required for DLL context.
        /// </summary>
        /// <returns>DialogResult indicating how the form was closed</returns>
        public static DialogResult LaunchGame()
        {
            try
            {
                // Try to set Application settings, but catch and ignore the exception if they're already set
                try
                {
                    if (!Application.MessageLoop)
                    {
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                    }
                }
                catch (InvalidOperationException)
                {
                    // Application settings already initialized - this is expected in many scenarios
                    System.Diagnostics.Debug.WriteLine("Application settings already initialized (this is normal)");
                }

                using (PadForm game = new PadForm())
                {
                    return game.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error launching PadForm: {ex.Message}");
                _ = MessageBox.Show($"Failed to launch game: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return DialogResult.Abort;
            }
        }

        /// <summary>
        /// Shows the game form non-modally from external DLL calls.
        /// Caller is responsible for managing the form lifecycle.
        /// </summary>
        /// <returns>The PadForm instance that was created and shown</returns>
        public static PadForm ShowGame()
        {
            try
            {
                // Try to set Application settings, but catch and ignore the exception if they're already set
                try
                {
                    if (!Application.MessageLoop)
                    {
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                    }
                }
                catch (InvalidOperationException)
                {
                    // Application settings already initialized - this is expected in many scenarios
                    System.Diagnostics.Debug.WriteLine("Application settings already initialized (this is normal)");
                }

                PadForm game = new PadForm();
                game.Show();
                game.Activate();
                game.BringToFront();
                return game;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error showing PadForm: {ex.Message}");
                _ = MessageBox.Show($"Failed to show game: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
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
                    bricks.Add(new Rectangle(x, y, 55, 22));
                }
            }

            // Force repaint
            Invalidate();
        }

        // Main game loop: updates game state every tick
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (!gameStarted)
            {
                return;
            }

            // Move paddle based on input
            if (leftPressed && paddle.Left > 0)
            {
                paddle.X -= paddleSpeed;
            }

            if (rightPressed && paddle.Right < ClientSize.Width)
            {
                paddle.X += paddleSpeed;
            }

            // Move ball
            ball.X += ballX;
            ball.Y += ballY;

            // Ball collision with walls
            if (ball.Left < 0 || ball.Right > ClientSize.Width)
            {
                ballX = -ballX;
            }

            if (ball.Top < 0)
            {
                ballY = -ballY;
            }

            // Ball collision with paddle
            if (ball.IntersectsWith(paddle))
            {
                ballY = -ballY;
            }

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

            // Ball falls below screen or all bricks gone — game over
            if (!gameOver && (ball.Bottom > ClientSize.Height || bricks.Count == 0))
            {
                gameOver = true;
                gameStarted = false;
                gameTimer.Stop(); // Stop game loop

                // Update high score if needed
                if (score > highScore)
                {
                    highScore = score;
                }

                // Prepare game over message to be drawn on screen
                gameOverMessage = $"GAME OVER!\nYour score: {score}";
                Invalidate(); // Force repaint so message appears
            }

            // Trigger repaint
            Invalidate();
        }

        // Handles all drawing on the screen
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Draw paddle and ball
            g.FillRectangle(Brushes.Cyan, paddle);
            g.FillEllipse(Brushes.White, ball);

            // Draw bricks
            foreach (Rectangle brick in bricks)
            {
                g.FillRectangle(Brushes.RosyBrown, brick);
            }

            // Draw score and high score
            g.DrawString($"Score: {score}", new Font("Arial", 12), Brushes.White, 10, 10);
            g.DrawString($"High Score: {highScore}", new Font("Arial", 12), Brushes.White, ClientSize.Width - 150, 10);


                using (Font font = new Font("Arial", 34, FontStyle.Bold))
                using (StringFormat sf = new StringFormat()
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                })
                {
                // Draw Game Over message if game is over
                if (gameOver && !string.IsNullOrEmpty(gameOverMessage))
                {
                    g.DrawString(gameOverMessage, font, Brushes.Yellow, new RectangleF(0, 0, ClientSize.Width, ClientSize.Height), sf);
                    g.DrawString(gameStartMessage, font, Brushes.White, new RectangleF(0, 100, ClientSize.Width, ClientSize.Height), sf);
                }
                // Draw Start message if game hasn't started yet
                if(!gameStarted && !gameOver)
                {
                    g.DrawString(gameStartMessage, font, Brushes.White, new RectangleF(0, 100, ClientSize.Width, ClientSize.Height), sf);
                }
            }
            return;
        }

        // Handles key press events
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                leftPressed = true;
            }

            if (e.KeyCode == Keys.Right)
            {
                rightPressed = true;
            }

            if (e.KeyCode == Keys.Space)
            {
                if (!gameStarted)
                {
                    if (gameOver)
                    {
                        ResetGame();
                    }

                    gameStarted = true;
                    gameTimer.Start();
                }
            }
        }

        // Handles key release events
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                leftPressed = false;
            }

            if (e.KeyCode == Keys.Right)
            {
                rightPressed = false;
            }
        }

        // Basic form setup (called by constructor)
        private void InitializeComponent()
        {
            SuspendLayout();
            ClientSize = new System.Drawing.Size(1043, 424);
            BackColor = System.Drawing.Color.Black;
            Name = "Form1";
            Text = "Freakout";
            ResumeLayout(false);
        }
    }
}