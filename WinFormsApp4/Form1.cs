using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Windows.Forms;
using WinFormsApp4.Properties;
namespace WinFormsApp4
{
    public partial class Form1 : Form
    {
        private List<PictureBox> enemies = new List<PictureBox>();
        private List<Point> enemyDirections = new List<Point>();
        private Random random = new Random();
        private int enemySpeed = 3;
        private System.Windows.Forms.Timer timerMove = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer timerEnemySpawn = new System.Windows.Forms.Timer();

        private PictureBox player;
        private List<PictureBox> traps = new List<PictureBox>();
        private int playerSpeed = 10;
        private int maxTraps = 10;
        private int enemyMax = 5;
        private bool gameRunning = false;
        private int score = 0;
        private Label scoreLabel;
        private Label trapsLabel;
        private int spawnInterval = 15000; 
        private int spawnIncreaseAmount = 1;
        private int maxEnemiesOnScreen = 10;

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            InitializePlayer();
            SetPanelBackground(gameRunning);
            gameRunning = true;
            button1.TabStop = false;
            panel1.TabStop = true;
            panel1.Focus();
            timerMove.Interval = 30;
            timerMove.Tick += TimerMove_Tick;
            timerEnemySpawn.Interval = spawnInterval;
            timerEnemySpawn.Tick += TimerEnemySpawn_Tick;
            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;
            panel1.Click += Panel1_Click;
        }

        private void TimerEnemySpawn_Tick(object sender, EventArgs e)
        {
            if (!gameRunning) return;
            int enemiesToAdd = Math.Min(spawnIncreaseAmount, maxEnemiesOnScreen - enemies.Count);
            for (int i = 0; i < enemiesToAdd; i++)
            {
                SpawnEnemy();
            }
            if (enemySpeed < 7)
            {
                enemySpeed += 1;
            }
            if (spawnInterval > 3000)
            {
                spawnInterval -= 1000;
                timerEnemySpawn.Interval = spawnInterval;
            }
        }

        private void SpawnEnemy()
        {
            if (enemies.Count >= maxEnemiesOnScreen) return;
            PictureBox enemy = new PictureBox();
            enemy.Size = new Size(40, 40);
            enemy.Image = Properties.Resources.fff8c3f0b700ad6e6d649afd858b5c45;
            enemy.BackColor = Color.Transparent;
            enemy.SizeMode = PictureBoxSizeMode.StretchImage;
            int side = random.Next(4);
            int x = 0, y = 0;
            switch (side)
            {
                case 0:
                    x = random.Next(0, panel1.Width - enemy.Width);
                    y = -enemy.Height;
                    break;
                case 1:
                    x = panel1.Width;
                    y = random.Next(0, panel1.Height - enemy.Height);
                    break;
                case 2:
                    x = random.Next(0, panel1.Width - enemy.Width);
                    y = panel1.Height;
                    break;
                case 3:
                    x = -enemy.Width;
                    y = random.Next(0, panel1.Height - enemy.Height);
                    break;
            }
            enemy.Location = new Point(x, y);
            panel1.Controls.Add(enemy);
            enemies.Add(enemy);
            int centerX = panel1.Width / 2;
            int centerY = panel1.Height / 2;
            int dx = centerX - enemy.Left;
            int dy = centerY - enemy.Top;
            double length = Math.Sqrt(dx * dx + dy * dy);
            if (length > 0)
            {
                dx = (int)(dx / length * enemySpeed);
                dy = (int)(dy / length * enemySpeed);
            }
            if (dx == 0 && dy == 0)
            {
                while (dx == 0) dx = random.Next(-enemySpeed, enemySpeed + 1);
                while (dy == 0) dy = random.Next(-enemySpeed, enemySpeed + 1);
            }
            enemyDirections.Add(new Point(dx, dy));
        }

        private void InitializeLabels()
        {
            scoreLabel = new Label();
            scoreLabel.Text = $"Счет: {score}";
            scoreLabel.Font = new Font("Verdana", 13, FontStyle.Bold);
            scoreLabel.ForeColor = Color.White;
            scoreLabel.BackColor = Color.MidnightBlue;
            scoreLabel.AutoSize = true;
            scoreLabel.Location = new Point(20, 20);
            this.Controls.Add(scoreLabel);
            scoreLabel.BringToFront();
            trapsLabel = new Label();
            trapsLabel.Text = $"Ловушки: {maxTraps - traps.Count}";
            trapsLabel.Font = new Font("Verdana", 13, FontStyle.Bold);
            trapsLabel.ForeColor = Color.White;
            trapsLabel.BackColor = Color.MidnightBlue;
            trapsLabel.AutoSize = true;
            trapsLabel.Location = new Point(20, 60);
            this.Controls.Add(trapsLabel);
            trapsLabel.BringToFront();
        }

        private void UpdateScoreLabel()
        {
            if (scoreLabel != null)
            {
                scoreLabel.Text = $"Счет: {score}";
            }
        }

        private void UpdateTrapsLabel()
        {
            if (trapsLabel != null)
            {
                trapsLabel.Text = $"Ловушки: {maxTraps - traps.Count}";
            }
        }

        private void InitializeEnemies()
        {
            for (int i = 0; i < enemyMax; i++)
            {
                CreateInitialEnemy();
            }
        }

        private void CreateInitialEnemy()
        {
            PictureBox enemy = new PictureBox();
            enemy.Size = new Size(40, 40);
            enemy.Image = Properties.Resources.fff8c3f0b700ad6e6d649afd858b5c45;
            enemy.BackColor = Color.Transparent;
            enemy.SizeMode = PictureBoxSizeMode.StretchImage;
            int x = random.Next(0, panel1.Width - enemy.Width);
            int y = random.Next(0, panel1.Height - enemy.Height);
            enemy.Location = new Point(x, y);
            panel1.Controls.Add(enemy);
            enemies.Add(enemy);
            int dx = 0, dy = 0;
            while (dx == 0)
                dx = random.Next(-enemySpeed, enemySpeed + 1);
            while (dy == 0)
                dy = random.Next(-enemySpeed, enemySpeed + 1);
            enemyDirections.Add(new Point(dx, dy));
        }

        private void TimerMove_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                PictureBox enemy = enemies[i];
                Point dir = enemyDirections[i];
                int newX = enemy.Left + dir.X;
                int newY = enemy.Top + dir.Y;
                if (newX < -enemy.Width || newX > panel1.Width ||
                    newY < -enemy.Height || newY > panel1.Height)
                {
                    if (newX < -enemy.Width) newX = panel1.Width;
                    else if (newX > panel1.Width) newX = -enemy.Width;
                    if (newY < -enemy.Height) newY = panel1.Height;
                    else if (newY > panel1.Height) newY = -enemy.Height;
                }
                else
                {
                    if (newX < 0)
                    {
                        newX = 0;
                        dir.X = -dir.X;
                    }
                    else if (newX + enemy.Width > panel1.Width)
                    {
                        newX = panel1.Width - enemy.Width;
                        dir.X = -dir.X;
                    }
                    if (newY < 0)
                    {
                        newY = 0;
                        dir.Y = -dir.Y;
                    }
                    else if (newY + enemy.Height > panel1.Height)
                    {
                        newY = panel1.Height - enemy.Height;
                        dir.Y = -dir.Y;
                    }
                }
                enemy.Location = new Point(newX, newY);
                enemyDirections[i] = dir;
            }
            CheckCollisions();
        }

        private void InitializePlayer()
        {
            player = pictureBox1;
            player.BackColor = Color.Transparent;
            player.Location = new Point(panel1.Width / 2, panel1.Height / 2);
            panel1.Controls.Add(player);
            player.BringToFront();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (player == null || !gameRunning) return;
            if (e.KeyCode == Keys.Left) player.Left -= playerSpeed;
            else if (e.KeyCode == Keys.Right) player.Left += playerSpeed;
            else if (e.KeyCode == Keys.Up) player.Top -= playerSpeed;
            else if (e.KeyCode == Keys.Down) player.Top += playerSpeed;
            else if (e.KeyCode == Keys.Space) PlaceTrap();
            player.Left = Math.Max(0, Math.Min(player.Left, panel1.Width - player.Width));
            player.Top = Math.Max(0, Math.Min(player.Top, panel1.Height - player.Height));
        }

        private void PlaceTrap()
        {
            if (traps.Count == maxTraps)
            {
                MessageBox.Show("У вас закончились ловушки!");
                return;
            }
            foreach (var existingTrap in traps)
            {
                if (existingTrap.Bounds.IntersectsWith(player.Bounds))
                {
                    MessageBox.Show("Здесь уже есть ловушка!");
                    return;
                }
            }
            PictureBox trap = new PictureBox();
            trap.Size = new Size(50, 50);
            trap.Image = Properties.Resources.bear_trap;
            trap.BackColor = Color.Transparent;
            trap.SizeMode = PictureBoxSizeMode.StretchImage;
            trap.Location = player.Location;
            panel1.Controls.Add(trap);
            trap.BringToFront();
            traps.Add(trap);
            UpdateTrapsLabel();
        }

        private void CheckCollisions()
        {
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                PictureBox enemy = enemies[i];
                for (int j = traps.Count - 1; j >= 0; j--)
                {
                    PictureBox trap = traps[j];
                    if (enemy.Bounds.IntersectsWith(trap.Bounds))
                    {
                        panel1.Controls.Remove(enemy);
                        enemies.RemoveAt(i);
                        enemyDirections.RemoveAt(i);
                        panel1.Controls.Remove(trap);
                        traps.RemoveAt(j);
                        score++;
                        UpdateScoreLabel();
                        break;
                    }
                }
            }
            foreach (var enemy in enemies)
            {
                if (player.Bounds.IntersectsWith(enemy.Bounds))
                {
                    GameOver();
                    return;
                }
            }
        }

        private void StartGame()
        {
            gameRunning = true;
            label1.Visible = false;
            label2.Visible = false;
            SetPanelBackground(gameRunning);
            InitializeLabels();
            InitializeEnemies();
            timerMove.Start();
            timerEnemySpawn.Start();
        }

        private void GameOver()
        {
            gameRunning = false;
            timerMove.Stop();
            timerEnemySpawn.Stop();
            if (score == enemyMax)
            {
                MessageBox.Show($"Игра окончена! Вы победили всех врагов! Счет: {score}");
            }
            else
            {
                MessageBox.Show($"Игра окончена! Счет: {score}");
            }

            if (scoreLabel != null)
            {
                this.Controls.Remove(scoreLabel);
                scoreLabel.Dispose();
                scoreLabel = null;
            }
            if (trapsLabel != null)
            {
                this.Controls.Remove(trapsLabel);
                trapsLabel.Dispose();
                trapsLabel = null;
            }
            foreach (var enemy in enemies)
            {
                panel1.Controls.Remove(enemy);
                enemy.Dispose();
            }
            enemies.Clear();
            enemyDirections.Clear();
            foreach (var trap in traps)
            {
                panel1.Controls.Remove(trap);
                trap.Dispose();
            }
            traps.Clear();
            enemySpeed = 3;
            spawnInterval = 10000;
            score = 0;
            button1.Visible = true;
            player.Location = new Point(panel1.Width / 2, panel1.Height / 2);
            SetPanelBackground(gameRunning);
            label1.Visible = true;
            label2.Visible = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Enabled)
            {
                StartGame();
                button1.Visible = false;
            }
        }

        private void Panel1_Click(object sender, EventArgs e)
        {
            panel1.Focus();
        }

        private void SetPanelBackground(bool isGameRunning)
        {
            if (isGameRunning)
            {
                panel1.BackgroundImage = Properties.Resources._2327eb6b_d3ae_4dd1_9b20_ae7c1f0b8ddb;
                panel1.BackgroundImageLayout = ImageLayout.Stretch;
            }
            else
            {
                panel1.BackgroundImage = Properties.Resources.background_menu;
                panel1.BackgroundImageLayout = ImageLayout.Stretch;
            }
        }
    }
}