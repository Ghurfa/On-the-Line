using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;

namespace On_the_Line
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public static MouseHitbox mouseHitbox;
        Texture2D pixel;

        TimeSpan timeToWait = new TimeSpan(0, 0, 0, 1, 0);
        TimeSpan elapsedTime;

        TimeSpan laserCooldown = new TimeSpan(0, 0, 0, 1, 0);
        SpriteFont font;
        SpriteFont smallText;
        SpriteFont endGameFont;
        SpriteFont extraLargeText;

        public static bool lose = false;
        public static bool pause = false;
        List<Obstacles> obstacles = new List<Obstacles>();
        Random random = new Random();
        public static Color wallColor;
        public static Color outerWallColor;
        public static Color backgroundColor;
        Color ballColor;
        public static Color textColor;
        public static Color laserColor;
        public static Color endGameColor;
        int score = 0;
        int fps;
        int frames;
        int destroyedObstacles;
        int enemiesKilled;
        public static int screen = 0;
        bool shootingLaser = false;
        public static int shootStyle = 0;
        int secret = 0;

        Sprite Screen0;
        Sprite Screen2;

        Sprite P;
        Sprite B;
        Sprite Title;
        public static string GameMode = "Regular";

        Button startButton;
        Button optionsButton;
        Button colorButton;
        Button backButton;
        Button gamemodeButton;
        Checkbox dotModeCheckbox;

        public static Button shootStyleButton;

        public static bool isLoading = false;
        int highestObstacle = -500;

        public static string colorScheme = "Default";

        List<int> levels = new List<int>();
        public static List<Enemy> enemies = new List<Enemy>();
        KeyboardState lastKs;
        public int[] difficulty = { 0, 0, 0, 0, 40, 40, 60, 60, 80, 60, 80, 80, 100, 120, 100, 100, 120, 100, 120, 100, 120, 120, 120, 200, 120 };
        public static int[] reloadCycles = { 1, 2, 1, 1, 1, 1 };
        int slidingSpeed = 0;

        int obstacleSize = 25;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 500;
            graphics.PreferredBackBufferHeight = 1000;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            IsMouseVisible = true;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("Font");
            smallText = Content.Load<SpriteFont>("StatsText");
            endGameFont = Content.Load<SpriteFont>("EndGameFont");
            extraLargeText = Content.Load<SpriteFont>("LargeText");
            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData<Color>(new Color[] { Color.White });
            startButton = new Button(new Vector2(125, 550), Content.Load<Texture2D>("StartButton"));
            optionsButton = new Button(new Vector2(125, 700), Content.Load<Texture2D>("OptionsButton"));
            mouseHitbox = new MouseHitbox(ballColor, Content.Load<Texture2D>("Ball"), Content.Load<Texture2D>("Spotlight"), true, new Vector2(238, 250));

            colorButton = new Button(new Vector2(125, 100), Content.Load<Texture2D>(string.Format("{0}Button", colorScheme)));
            gamemodeButton = new Button(new Vector2(125, 300), Content.Load<Texture2D>(string.Format("{0}Button", GameMode)));
            shootStyleButton = new Button(new Vector2(125, 500), Content.Load<Texture2D>("EmptyButton"));
            dotModeCheckbox = new Checkbox(new Vector2(400, 315), Content.Load<Texture2D>("Checkbox_On"), Content.Load<Texture2D>("Checkbox_Off"), false);

            Screen0 = new Sprite(new Vector2(0, 0), Content.Load<Texture2D>("Screen"), Color.White);
            Screen2 = new Sprite(new Vector2(500, 0), Content.Load<Texture2D>("Screen"), Color.White);

            P = new Sprite(new Vector2(10, 10), Content.Load<Texture2D>("P"), Color.White);
            B = new Sprite(new Vector2(250, 500), Content.Load<Texture2D>("B"), Color.White);

            Title = new Sprite(new Vector2(0, 0), Content.Load<Texture2D>("Title"), Color.White);

            backButton = new Button(new Vector2(125, 900), Content.Load<Texture2D>("BackButton"));

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        /// <summary>
        /// Chooses a random obstacle to load
        /// </summary>
        /// <param name="yOffset"></param>
        void newObstacle(float yOffset)
        {
            int randomNumber = random.Next(1, 26);
            if (difficulty[randomNumber - 1] > score / 50)
            {
                newObstacle(yOffset);
            }
            else
            {
                if (randomNumber == 8 || randomNumber == 15)
                {
                    yOffset -= 500;
                }
                if (randomNumber == 24)
                {
                    yOffset -= 1000;
                }
                loadObstacle(yOffset, string.Format("Obstacle{0}", randomNumber));
            }
        }
        /// <summary>
        /// Loads an obstacle
        /// </summary>
        /// <param name="yOffset">The y-offset to load  the obstacle at</param>
        /// <param name="obstacleName">The name of the obstacle to load</param>
        void loadObstacle(float yOffset, string obstacleName, float xOffset = 0f)
        {
            Texture2D obstacleTexture = Content.Load<Texture2D>(obstacleName);
            Color[] pixels = new Color[obstacleTexture.Width * obstacleTexture.Height];

            obstacleTexture.GetData<Color>(pixels);

            for (int y = 0; y < obstacleTexture.Height; y++)
            {
                for (int x = 0; x < obstacleTexture.Width; x++)
                {
                    Color currentPixel = pixels[x + (y * obstacleTexture.Width)];
                    if (currentPixel == Color.Red)
                    {
                        if (GameMode == "Darkmode")
                        {
                            obstacles.Add(new Obstacles(pixel, new Vector2(x * 25 + xOffset, (y * 25) - 500 + yOffset), new Vector2(25, 25), backgroundColor, false, 0, 0, 0, false, false)); //Outside Background
                        }
                        else
                        {
                            obstacles.Add(new Obstacles(pixel, new Vector2(x * 25 + xOffset, (y * 25) - 500 + yOffset), new Vector2(25, 25), outerWallColor, false, 0, 0, 0, false, false, true)); //Outside Background
                        }
                    }
                    else if (currentPixel == Color.Green)
                    {
                        obstacles.Add(new Obstacles(pixel, new Vector2(x * 25 + xOffset, (y * 25) - 500 + yOffset), new Vector2(25, 25), new Color(textColor.R, textColor.G, textColor.B, 230), false, 0, 0, 0, false, false));
                    }
                    else if (currentPixel == Color.Purple)
                    {
                        obstacles.Add(new Obstacles(pixel, new Vector2(x * 25 + xOffset, (y * 25) - 500 + yOffset), new Vector2(25, 25), wallColor, true, 0, 0, 0, false, false));
                    }
                    else if (currentPixel == Color.Orange)
                    {
                        enemies.Add(new Enemy(new Vector2(x * 25 + xOffset, (y * 25) - 500 + yOffset), Content.Load<Texture2D>("Ball"), Content.Load<Texture2D>("Empty"), Content.Load<Texture2D>("Laser"), 0, 0, true, false));
                    }
                    else if (currentPixel == Color.Aqua)
                    {
                        enemies.Add(new Enemy(new Vector2(x * 25 + xOffset, (y * 25) - 500 + yOffset), Content.Load<Texture2D>("Ball"), Content.Load<Texture2D>("Empty"), Content.Load<Texture2D>("Laser"), 0, 0, true, true));
                    }
                    else if (currentPixel == Color.Black)
                    {
                        obstacles.Add(new Obstacles(pixel, new Vector2(x * 25 + xOffset, (y * 25) - 500 + yOffset), new Vector2(25, 25), wallColor, false, 0, 0, 0, true, false));
                    }
                    else if (currentPixel == Color.Blue)
                    {
                        obstacles.Add(new Obstacles(pixel, new Vector2(x * 25 + xOffset, (y * 25) - 500 + yOffset), new Vector2(25, 25), backgroundColor, false, 0, 0, 0, false, true));
                    }
                    else if (currentPixel.R == 254)
                    {
                        enemies.Add(new Enemy(new Vector2(x * 25 + xOffset, (y * 25) - 500 + yOffset), Content.Load<Texture2D>("Ball"), Content.Load<Texture2D>("Empty"), Content.Load<Texture2D>("Laser"), currentPixel.G, currentPixel.B, false, false));
                    }
                    else if (currentPixel != Color.White)
                    {
                        if (GameMode == "Darkmode")
                        {
                            obstacles.Add(new Obstacles(pixel, new Vector2(x * 25 + xOffset, (y * 25) - 500 + yOffset), new Vector2(25, 25), backgroundColor, false, currentPixel.R - 100, currentPixel.G - 100, currentPixel.B, false, false));
                        }
                        else
                        {
                            obstacles.Add(new Obstacles(pixel, new Vector2(x * 25 + xOffset, (y * 25) - 500 + yOffset), new Vector2(25, 25), wallColor, false, currentPixel.R - 100, currentPixel.G - 100, currentPixel.B, false, false));
                        }
                    }
                }
            }
        }
        public void setScreen(int screenToSetTo)
        {
            P.Hitbox.X = 10;
            P.Hitbox.Y = 10;
            B.XSpeed = 2;
            B.YSpeed = 2;
            secret = 0;
            lose = false;
            mouseHitbox.lasers.Clear();
            pause = false;

            if (screenToSetTo == 0)
            {
                Screen0.Position = new Vector2(528, 0);
                slidingSpeed = 32;
                if (screen != 2)
                {
                    Screen2.Position = new Vector2(1000, 0);
                    score = 0;
                }
            }
            else if (screenToSetTo == 1)
            {
                score = 0;
                obstacles.Clear();
                enemies.Clear();
                destroyedObstacles = 0;
                enemiesKilled = 0;
                loadObstacle(1000, "LowerStartingObstacle");
                loadObstacle(500, string.Format("startingObstacle{0}", random.Next(1, 4)));
                mouseHitbox.canShoot = true;
                mouseHitbox = new MouseHitbox(ballColor, Content.Load<Texture2D>("Ball"), Content.Load<Texture2D>("Spotlight"), true, new Vector2(238, 250));
            }
            else if (screenToSetTo == 2)
            {
                Screen2.Position = new Vector2(528, 0);
                slidingSpeed = 32;
            }
            else
            {
                score = 0;
                P = new Sprite(new Vector2(10, 10), Content.Load<Texture2D>("P"), Color.White);
                B = new Sprite(new Vector2(250, 500), Content.Load<Texture2D>("B"), Color.White);
            }
            if (screen == 1)
            {
                obstacles.Clear();
                enemies.Clear();
            }
            screen = screenToSetTo;
        }
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;
            if (elapsedTime >= timeToWait)
            {
                elapsedTime = TimeSpan.Zero;
                fps = frames;
                frames = 0;
            }
            Screen0.Update();
            Screen2.Update();
            if (obstacles.Count == 0)
            {
                if (screen == 0)
                {
                    newObstacle(500);
                    newObstacle(1000);
                }
                else if (screen == 1)
                {
                    loadObstacle(1000, "LowerStartingObstacle");
                    loadObstacle(500, string.Format("startingObstacle{0}", random.Next(1, 4)));
                }
            }
            if (slidingSpeed != 0)
            {
                Screen0.Position.X -= slidingSpeed;
                Screen2.Position.X -= slidingSpeed;
                slidingSpeed--;
            }
            mouseHitbox._color = ballColor;
            //Screen 0
            startButton.Update();
            optionsButton.Update();
            startButton.Position = Screen0.Position + new Vector2(125, 550);
            optionsButton.Position = Screen0.Position + new Vector2(125, 700);
            Title.Position = Screen0.Position + new Vector2(0, 50);
            Title.Color = textColor;
            //Screen 2
            colorButton.Update();
            gamemodeButton.Update();
            shootStyleButton.Update();
            dotModeCheckbox.Update();
            backButton.Update();
            colorButton.Position = Screen2.Position + new Vector2(125, 100);
            gamemodeButton.Position = Screen2.Position + new Vector2(125, 300);
            shootStyleButton.Position = Screen2.Position + new Vector2(125, 500);
            dotModeCheckbox.checkBox.Position = Screen2.Position + new Vector2(400, 315);
            backButton.Position = Screen2.Position + new Vector2(125, 900);
            if (!lose && !isLoading)
            {
                #region Updates Obstacles
                highestObstacle = 10;
                for (int i = 0; i < obstacles.Count; i++)
                {
                    Obstacles obstacle = obstacles[i];
                    obstacle.Update();
                    obstacle._size = new Vector2(obstacleSize, obstacleSize);
                    if (GameMode == "Fastmode")
                    {
                        obstacle.Update();
                    }
                    if (obstacle.hitbox.Intersects(mouseHitbox._hitbox) && obstacle._slideSpeed == 0 && !pause && !obstacle._gateway && screen == 1)
                    {
                        isLoading = true;
                        obstacle.didKill = true;
                        obstacle._color = endGameColor;
                    }
                    if (obstacle.hitbox.Y < highestObstacle)
                    {
                        highestObstacle = obstacle.hitbox.Y;
                    }
                    if (obstacle._breaks)
                    {
                        for (int x = 0; x < mouseHitbox.lasers.Count; x++)
                        {
                            Laser laser = mouseHitbox.lasers[x];
                            if (laser._rect.Intersects(obstacle.hitbox))
                            {
                                obstacles.Remove(obstacle);
                                destroyedObstacles++;
                                score += 10;
                                i--;
                                laser._lives--;
                                if (laser._lives <= 0)
                                {
                                    Game1.mouseHitbox.lasers.Remove(laser);
                                }
                            }
                        }
                    }
                    if (obstacle.hitbox.Y > 2000)
                    {
                        obstacles.Remove(obstacle);
                        i--;
                    }
                }
                if (highestObstacle >= 0 && obstacles.Count < 2000)
                {
                    newObstacle(highestObstacle);
                }
                #endregion
                #region Update Enemies
                for (int i = 0; i < enemies.Count; i++)
                {
                    Enemy enemy = enemies[i];
                    enemy.Update();
                    if (GameMode == "Fastmode")
                    {
                        enemy.Update();
                    }
                    foreach (Obstacles obstacle in obstacles)
                    {
                        if (obstacle.hitbox.Intersects(enemy.body._hitbox) && obstacle._gateway)
                        {
                            enemies.Remove(enemy);
                            enemiesKilled++;
                        }
                    }
                    for (int x = 0; x < mouseHitbox.lasers.Count; x++)
                    {
                        Laser laser = mouseHitbox.lasers[x];
                        if (enemy.body._hitbox.Intersects(laser._rect))
                        {
                            enemies.Remove(enemy);
                            enemiesKilled++;
                            mouseHitbox.lasers.Remove(laser);
                        }
                    }
                    for (int y = 0; y < enemies.Count; y++)
                    {
                        Enemy targetedEnemy = enemies[y];
                        if (enemy != targetedEnemy && !targetedEnemy._rams)
                        {
                            for (int x = 0; x < enemy.body.lasers.Count; x++)
                            {
                                Laser laser = enemy.body.lasers[x];
                                if (targetedEnemy.body._hitbox.Intersects(laser._rect))
                                {
                                    enemies.Remove(targetedEnemy);
                                    enemiesKilled++;
                                    enemy.body.lasers.Remove(laser);
                                }
                            }
                        }
                    }
                }
                #endregion
                if (screen != 3 && !pause)
                {
                    score++;
                    if (GameMode == "Fastmode")
                    {
                        score++;
                    }
                }
            }

            #region Check Keyboard
            KeyboardState ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.Left))
            {
                P.Hitbox.Y -= 10;
            }
            if (ks.IsKeyDown(Keys.Right))
            {
                P.Hitbox.Y += 10;
            }
            if (ks.IsKeyDown(Keys.M) && !lastKs.IsKeyDown(Keys.M))
            {
                setScreen(0);
            }
            if (ks.IsKeyDown(Keys.R) && !lastKs.IsKeyDown(Keys.R) && screen == 1)
            {
                setScreen(1);
            }
            if (ks.IsKeyDown(Keys.M) && !lastKs.IsKeyDown(Keys.M))
            {
                setScreen(0);
            }
            if (!lose)
            {
                if (ks.IsKeyDown(Keys.Up))
                {
                    int highestObstacle = 10;
                    for (int i = 0; i < obstacles.Count; i++)
                    {
                        Obstacles obstacle = obstacles[i];
                        obstacle.Update();
                        for (int x = 0; x < Game1.mouseHitbox.lasers.Count; x++)
                        {
                            if (obstacle.hitbox.Intersects(Game1.mouseHitbox.lasers[x]._rect) && obstacle._breaks)
                            {
                                Game1.mouseHitbox.lasers[x]._lives -= 2;
                                if (Game1.mouseHitbox.lasers[x]._lives <= 0)
                                {
                                    Game1.mouseHitbox.lasers.Remove(Game1.mouseHitbox.lasers[x]);
                                }
                                obstacles.Remove(obstacle);
                            }
                        }
                        if (ks.IsKeyDown(Keys.RightControl))
                        {
                            obstacle.Update();
                        }
                        if (ks.IsKeyDown(Keys.LeftControl))
                        {
                            for (int d = 0; d < 6; d++)
                            {
                                obstacle.Update();
                            }
                        }
                        if (obstacle.hitbox.Y < highestObstacle)
                        {
                            highestObstacle = obstacle.hitbox.Y;
                        }
                    }
                    if (highestObstacle >= 0)
                    {
                        newObstacle(highestObstacle);
                    }
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        Enemy enemy = enemies[i];
                        enemy.Update();
                        if (ks.IsKeyDown(Keys.RightControl))
                        {
                            enemy.Update();
                        }
                        if (ks.IsKeyDown(Keys.LeftControl))
                        {
                            for (int d = 0; d < 6; d++)
                            {
                                enemy.Update();
                            }
                        }
                    }
                }
                else if (ks.IsKeyDown(Keys.Down))
                {
                    for (int i = 0; i < obstacles.Count; i++)
                    {
                        Obstacles obstacle = obstacles[i];
                        obstacle.Update();
                        if (ks.IsKeyDown(Keys.RightControl))
                        {
                            obstacle.Update();
                        }
                        if (ks.IsKeyDown(Keys.LeftControl))
                        {
                            for (int d = 0; d < 6; d++)
                            {
                                obstacle.Update();
                            }
                        }
                        if (obstacle.hitbox.Y < highestObstacle)
                        {
                            highestObstacle = obstacle.hitbox.Y;
                        }
                    }
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        Enemy enemy = enemies[i];
                        enemy.Update();
                        if (ks.IsKeyDown(Keys.RightControl))
                        {
                            enemy.Update();
                        }
                        if (ks.IsKeyDown(Keys.LeftControl))
                        {
                            for (int d = 0; d < 6; d++)
                            {
                                enemy.Update();
                            }
                        }
                    }
                }
            }
            if (ks.IsKeyDown(Keys.Space) && mouseHitbox.canShoot)
            {
                mouseHitbox.fireLasers(Content.Load<Texture2D>("Laser"), laserColor, false);
                mouseHitbox.laserElapsedTime = TimeSpan.Zero;
                mouseHitbox.canShoot = false;
            }
            lastKs = ks;
            #endregion
            #region Check Color Scheme
            endGameColor = Color.White;
            if (colorScheme == "Ice")
            {
                ballColor = new Color(255, 150, 0);
                textColor = new Color(255, 150, 0);
                laserColor = new Color(255, 150, 0);
                wallColor = new Color(30, 250, 230);
                outerWallColor = new Color(30, 250, 230);
                backgroundColor = new Color(13, 13, 13);

            }
            else if (colorScheme == "Beach")
            {
                ballColor = new Color(45, 105, 174);
                textColor = new Color(0, 183, 45);
                laserColor = new Color(45, 105, 174);
                wallColor = new Color(45, 105, 174);
                outerWallColor = new Color(30, 44, 96);
                backgroundColor = new Color(240, 210, 150);

            }
            else if (colorScheme == "Gingerbread")
            {
                ballColor = new Color(50, 20, 0);
                textColor = new Color(50, 20, 0);
                laserColor = new Color(50, 20, 0);
                wallColor = Color.White;
                outerWallColor = new Color(40, 10, 0);
                backgroundColor = new Color(80, 50, 20);

            }
            else if (colorScheme == "School")
            {
                ballColor = Color.Black;
                textColor = Color.Black;
                laserColor = Color.Black;
                wallColor = new Color(10, 10, 10);
                outerWallColor = new Color(10, 10, 10);
                backgroundColor = new Color(20, 20, 20);

            }
            else if (colorScheme == "Colorful")
            {
                ballColor = Color.White;
                textColor = Color.White;
                laserColor = Color.White;
                wallColor = Color.Black;
                outerWallColor = Color.Black;
                backgroundColor = Color.Black;
                if (GameMode == "Darkmode" || GameMode == "Spotlight")
                {
                    outerWallColor = Color.White;
                }
            }
            if (colorScheme == "Default")
            {
                ballColor = Color.LightGray;
                textColor = Color.Red;
                laserColor = Color.Red;
                if (GameMode == "Darkmode" || GameMode == "Spotlight")
                {
                    wallColor = Color.White;
                    outerWallColor = Color.White;
                    backgroundColor = Color.Black;
                }
                else
                {
                    wallColor = Color.Black;
                    outerWallColor = Color.Black;
                    backgroundColor = Color.White;
                }
                endGameColor = wallColor;
            }
            else if (GameMode == "Darkmode" || GameMode == "Spotlight")
            {
                wallColor = outerWallColor;
            }
            #endregion
            #region Screen 0 Main Menu
            if (screen == 0)
            {
                if (startButton.Clicked)
                {
                    setScreen(1);
                }
                if (optionsButton.Released)
                {
                    setScreen(2);
                }
                if (obstacles.Count == 0)
                {
                    newObstacle(500);
                    newObstacle(1000);
                }
            }
            #endregion
            #region Screen 1 Gameplay
            else if (screen == 1)//gameplay
            {
                mouseHitbox.Update(gameTime);
                if (lose || isLoading)
                {
                    foreach (Obstacles obstacle in obstacles)
                    {
                        obstacle.Update();
                        if (GameMode == "Fastmode")
                        {
                            obstacle.Update();
                        }
                    }
                    if (isLoading)
                    {
                        lose = true;
                        loadObstacle(525, "YouLose");
                        isLoading = false;
                        mouseHitbox.lasers.Clear();
                    }
                }
                else if (!lose && !pause)
                {
                    MouseState ms = Mouse.GetState();
                    if (ms.Y < 0)
                    {
                        Mouse.SetPosition(ms.X, 0);
                    }
                    else if (ms.Y > 1000)
                    {
                        Mouse.SetPosition(ms.X, 1000);
                    }
                    if (ms.X < 0)
                    {
                        Mouse.SetPosition(0, ms.Y);
                    }
                    else if (ms.X > 500)
                    {
                        Mouse.SetPosition(500, ms.Y);
                    }
                }
            }
            #endregion
            #region Screen 2 Options
            else if (screen == 2)
            {
                mouseHitbox.Update(gameTime);
                #region Checks Color Button
                if (colorButton.Clicked)
                {
                    if (colorScheme == "Default")
                    {
                        colorScheme = "Ice";
                    }
                    else if (colorScheme == "Ice")
                    {
                        colorScheme = "Beach";
                    }
                    else if (colorScheme == "Beach")
                    {
                        colorScheme = "Gingerbread";
                    }
                    else if (colorScheme == "Gingerbread")
                    {
                        colorScheme = "School";
                    }
                    else if (colorScheme == "School")
                    {
                        colorScheme = "Colorful";
                    }
                    else if (colorScheme == "Colorful")
                    {
                        colorScheme = "Default";
                    }
                    colorButton.Texture = Content.Load<Texture2D>(string.Format("{0}Button", colorScheme));
                }
                #endregion
                #region Checks Gamemode Button
                if (gamemodeButton.Clicked)
                {
                    foreach (Obstacles obstacle in obstacles)
                    {
                        obstacle.Show = false;
                    }
                    if (GameMode == "Regular")
                    {
                        gamemodeButton.Texture = Content.Load<Texture2D>("DarkmodeButton");
                        GameMode = "Darkmode";
                    }
                    else if (GameMode == "Darkmode")
                    {
                        gamemodeButton.Texture = Content.Load<Texture2D>("SpotlightButton");
                        GameMode = "Spotlight";
                    }
                    else if (GameMode == "Spotlight")
                    {
                        gamemodeButton.Texture = Content.Load<Texture2D>("FastmodeButton");
                        GameMode = "Fastmode";
                    }
                    else if (GameMode == "Fastmode")
                    {
                        gamemodeButton.Texture = Content.Load<Texture2D>("Regularbutton");
                        GameMode = "Regular";
                    }
                }
                #endregion
                #region Checks Shootstyle Button
                if (shootStyleButton.Clicked)
                {
                    secret++;
                    mouseHitbox.lasers.Clear();
                    if (shootStyle != 5)
                    {
                        shootStyle++;
                    }
                    else
                    {
                        shootStyle = 0;
                    }
                    mouseHitbox.canShoot = true;
                    mouseHitbox.Update(gameTime);

                    mouseHitbox.fireLasers(Content.Load<Texture2D>("Laser"), laserColor, false);

                }
                #endregion
                if (dotModeCheckbox.isChecked)
                {
                    obstacleSize = 4;
                }
                else
                {
                    obstacleSize = 25;
                }
                if (backButton.Clicked)
                {
                    setScreen(0);
                }
                if (secret >= 20)
                {
                    setScreen(3);
                }
            }
            #endregion
            #region Screen 3 
            else if (screen == 3)
            {
                P.Color = wallColor;
                B.Color = outerWallColor;
                B.Position.X += B.XSpeed;
                B.Position.Y += B.YSpeed;
                if (B.Hitbox.Intersects(P.Hitbox))
                {
                    B.XSpeed = Math.Abs(B.XSpeed);
                    score++;
                    B.XSpeed++;
                    B.YSpeed += B.YSpeed / Math.Abs(B.YSpeed);

                }
                if (B.Hitbox.X < 0)
                {
                    setScreen(3);
                }
                if (B.Hitbox.X + B.Texture.Width > 500)
                {
                    B.XSpeed = -Math.Abs(B.XSpeed);
                }
                if (B.Hitbox.Y < 0)
                {
                    B.YSpeed = Math.Abs(B.YSpeed);
                }
                if (B.Hitbox.Y + B.Texture.Height > 1000)
                {
                    B.YSpeed = -Math.Abs(B.YSpeed);
                }
            }
            #endregion
            base.Update(gameTime);
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(backgroundColor);
            spriteBatch.Begin();
            if (screen != 3)
            {
                foreach (Obstacles obtsacle in obstacles) //Layer 0 - Regular obstacles
                {
                    obtsacle.Draw(spriteBatch);
                }
                foreach (Obstacles obtstacle in obstacles) //Layer 1 - Obstacle that killed you
                {
                    if (obtstacle.didKill)
                    {
                        obtstacle.Draw(spriteBatch);
                    }
                }
                foreach (Obstacles obtstacle in obstacles) //Layer 2 - "You Lose"
                {
                    if (obtstacle._color.A == 230)
                    {
                        obtstacle.Draw(spriteBatch);
                    }
                }
                foreach (Enemy enemy in enemies)
                {
                    enemy.Draw(spriteBatch);
                }
            }
            if (screen == 0 || screen == 2)
            {
                startButton.Draw(spriteBatch);
                optionsButton.Draw(spriteBatch);
                Title.Draw(spriteBatch);
                // Screen 2
                colorButton.Draw(spriteBatch);
                gamemodeButton.Draw(spriteBatch);
                shootStyleButton.Draw(spriteBatch);
                mouseHitbox.Position = new Vector2((int)shootStyleButton.Position.X + shootStyleButton.Texture.Width / 2 - Content.Load<Texture2D>("Ball").Width / 2, (int)shootStyleButton.Position.Y + shootStyleButton.Texture.Height / 2 - Content.Load<Texture2D>("Ball").Height / 2 + 10);
                backButton.Draw(spriteBatch);
                mouseHitbox.Draw(spriteBatch);
                int s = (int)shootStyleButton.Position.Y; //make the line look less intimidating
                spriteBatch.DrawString(smallText, string.Format($"Num of Bullets: {mouseHitbox.stats.Item2}"), Screen2.Position + new Vector2(125, s + 80), textColor);
                spriteBatch.DrawString(smallText, string.Format($"Bullet Penetration: { mouseHitbox.stats.Item3}"), Screen2.Position + new Vector2(125, s + 95), textColor);
                spriteBatch.DrawString(smallText, string.Format($"Bullet Speed: {mouseHitbox.stats.Item4}"), Screen2.Position + new Vector2(125, s + 110), textColor);
                spriteBatch.DrawString(smallText, string.Format($"Reload: {mouseHitbox.stats.Item1.Seconds + (float)mouseHitbox.stats.Item1.Milliseconds / 1000} sec(s)"), Screen2.Position + new Vector2(125, s + 125), textColor);
                spriteBatch.DrawString(smallText, string.Format($"Pros: {mouseHitbox.stats.Item5}"), Screen2.Position + new Vector2(125, s + 140), textColor);
                spriteBatch.DrawString(smallText, string.Format($"Cons: {mouseHitbox.stats.Item6}"), Screen2.Position + new Vector2(125, s + 155), textColor);
                KeyboardState ks = Keyboard.GetState();
                spriteBatch.DrawString(smallText, string.Format($"{shootingLaser}"), Screen2.Position + new Vector2(125, s + 170), textColor);
                dotModeCheckbox.Draw(spriteBatch);
            }
            if (screen == 1)//gameplay
            {
                int laserCount = mouseHitbox.lasers.Count;
                mouseHitbox.Draw(spriteBatch);
                if (mouseHitbox.Counting)
                {
                    spriteBatch.DrawString(font, string.Format($"0.{mouseHitbox.CountingCentisecond}"), mouseHitbox.Position + new Vector2(-10, -40), textColor);
                }
                if (lose)
                {
                    spriteBatch.DrawString(endGameFont, $"Score:{score / 50}", new Vector2(100, 450), textColor);
                    spriteBatch.DrawString(endGameFont, $"Obstacles Destroyed:{destroyedObstacles}", new Vector2(100, 500), textColor);
                    spriteBatch.DrawString(endGameFont, $"Enemies Killed:{enemiesKilled}", new Vector2(100, 550), textColor);
                    spriteBatch.DrawString(extraLargeText, "Verdict:", new Vector2(160, 650), textColor);
                    if (score < 5000)
                    {
                        spriteBatch.DrawString(extraLargeText, "You're a noob", new Vector2(100, 700), textColor);
                    }
                    else
                    {
                        spriteBatch.DrawString(extraLargeText, "You're a pro", new Vector2(110, 700), textColor);
                    }
                    spriteBatch.DrawString(extraLargeText, "Press R to Restart", new Vector2(30, 800), textColor);
                    spriteBatch.DrawString(extraLargeText, "Press M to go to Menu", new Vector2(0, 850), textColor);
                }
                else
                {
                    spriteBatch.DrawString(font, string.Format("{0}", obstacles.Count), new Vector2(0, 950), textColor);
                    spriteBatch.DrawString(font, string.Format("Score: {0}", score / 50), new Vector2(380, 950), textColor);
                    spriteBatch.DrawString(font, string.Format("{0}", laserCount), new Vector2(240, 950), textColor);
                }
            }
            else if (screen == 3)
            {
                P.Draw(spriteBatch);
                B.Draw(spriteBatch);
                spriteBatch.DrawString(endGameFont, $"Score:{score}", new Vector2(0, 950), textColor);
            }
            Screen0.Draw(spriteBatch);
            Screen2.Draw(spriteBatch);
            spriteBatch.End();
            frames++;
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}