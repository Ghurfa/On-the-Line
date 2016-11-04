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
        Sprite P;
        Sprite B;
        Sprite Title;
        //public static bool darkmode = true;
        public static string gamemode = "regular";

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
        public int[] difficulty = { 0, 0, 0, 0, 20, 20, 30, 30, 40, 30, 40, 40, 50, 60, 50, 50, 60, 50, 60, 50, 60, 60, 60, 100, 60 };
        public static int[] reloadCycles = { 1, 2, 1, 1, 1, 1 };

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
            endGameFont = Content.Load<SpriteFont>("LargeText");
            extraLargeText = Content.Load<SpriteFont>("ExtraLargeText");
            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData<Color>(new Color[] { Color.White });
            startButton = new Button(new Vector2(125, 550), Content.Load<Texture2D>("StartButton"));
            optionsButton = new Button(new Vector2(125, 700), Content.Load<Texture2D>("OptionsButton"));
            mouseHitbox = new MouseHitbox(ballColor, Content.Load<Texture2D>("Ball"), Content.Load<Texture2D>("Spotlight"), true);

            colorButton = new Button(new Vector2(125, 100), Content.Load<Texture2D>(string.Format("{0}Button", colorScheme)));
            gamemodeButton = new Button(new Vector2(125, 300), Content.Load<Texture2D>(string.Format("{0}Button", gamemode)));
            shootStyleButton = new Button(new Vector2(125, 500), Content.Load<Texture2D>("EmptyButton"));
            dotModeCheckbox = new Checkbox(new Vector2(400, 315), Content.Load<Texture2D>("Checkbox_On"), Content.Load<Texture2D>("Checkbox_Off"), false);

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
        /// Sets up everything for a new game
        /// </summary>
        void startNewGame()
        {
            destroyedObstacles = 0;
            enemiesKilled = 0;
            mouseHitbox.canShoot = true;
            loadObstacle(1000, "LowerStartingObstacle");
            loadObstacle(500, string.Format("startingObstacle{0}", random.Next(1, 4)));
            mouseHitbox = new MouseHitbox(ballColor, Content.Load<Texture2D>("Ball"), Content.Load<Texture2D>("Spotlight"), true);
            mouseHitbox._position = new Vector2(238, 250);
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
                        if (gamemode == "darkmode")
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
                        if (gamemode == "darkmode")
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
            score = 0;
            if (screenToSetTo == 0)
            {
                startButton = new Button(startButton.EndPosition, startButton._texture);
                optionsButton = new Button(optionsButton.EndPosition, optionsButton._texture);
            }
            else if (screenToSetTo == 1)
            {
                obstacles.Clear();
                enemies.Clear();
                startNewGame();
            }
            else if(screenToSetTo == 2)
            {
                colorButton = new Button(colorButton.EndPosition, colorButton._texture);
                gamemodeButton = new Button(gamemodeButton.EndPosition, gamemodeButton._texture);
                shootStyleButton = new Button(shootStyleButton.EndPosition, shootStyleButton._texture);
                dotModeCheckbox.checkBox = new Button(dotModeCheckbox.checkBox.EndPosition, dotModeCheckbox.checkBox._texture);
                backButton = new Button(backButton.EndPosition, backButton._texture);
            }
            if (screen == 1)
            {
                obstacles.Clear();
                enemies.Clear();
            }
            screen = screenToSetTo;
        }
        public void keyboardStuff()
        {
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
        }
        /// <summary>
        /// This checks the color scheme and chooses the color
        /// </summary>
        void checkColorScheme()
        {
            endGameColor = Color.White;
            if (colorScheme == "Default")
            {
                ballColor = Color.LightGray;
                textColor = Color.Red;
                laserColor = Color.Red;
                if (gamemode == "darkmode" || gamemode == "spotlight")
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
            else if (colorScheme == "Ice")
            {
                ballColor = new Color(255, 150, 0);
                textColor = new Color(255, 150, 0);
                laserColor = new Color(255, 150, 0);
                if (gamemode == "darkmode" || gamemode == "spotlight")
                {
                    wallColor = new Color(30, 250, 230);
                    outerWallColor = new Color(30, 250, 230);
                    backgroundColor = new Color(13, 13, 13);
                }
                else
                {
                    wallColor = new Color(30, 250, 230);
                    outerWallColor = new Color(30, 250, 230);
                    backgroundColor = new Color(13, 13, 13);
                }
            }
            else if (colorScheme == "Beach")
            {
                ballColor = new Color(45, 105, 174);
                textColor = new Color(0, 183, 45);
                laserColor = new Color(45, 105, 174);
                if (gamemode == "darkmode" || gamemode == "spotlight")
                {
                    wallColor = new Color(30, 44, 96);
                    outerWallColor = new Color(30, 44, 96);
                    backgroundColor = new Color(240, 210, 150);
                }
                else
                {
                    wallColor = new Color(45, 105, 174);
                    outerWallColor = new Color(30, 44, 96);
                    backgroundColor = new Color(240, 210, 150);
                }
            }
            else if (colorScheme == "Gingerbread")
            {
                ballColor = new Color(50, 20, 0);
                textColor = new Color(50, 20, 0);
                laserColor = new Color(50, 20, 0);
                if (gamemode == "darkmode" || gamemode == "spotlight")
                {
                    wallColor = new Color(40, 10, 0);
                    outerWallColor = new Color(40, 10, 0);
                    backgroundColor = new Color(80, 50, 20);
                }
                else
                {
                    wallColor = Color.White;
                    outerWallColor = new Color(40, 10, 0);
                    backgroundColor = new Color(80, 50, 20);
                }
            }
            else if (colorScheme == "School")
            {
                ballColor = Color.Black;
                textColor = Color.Black;
                laserColor = Color.Black;
                if (gamemode == "darkmode" || gamemode == "spotlight")
                {
                    wallColor = new Color(10, 10, 10);
                    outerWallColor = new Color(10, 10, 10);
                    backgroundColor = new Color(20, 20, 20);
                }
                else
                {
                    wallColor = new Color(10, 10, 10);
                    outerWallColor = new Color(10, 10, 10);
                    backgroundColor = new Color(20, 20, 20);
                }
            }
            else if (colorScheme == "Colorful")
            {
                ballColor = Color.White;
                textColor = Color.White;
                laserColor = Color.White;
                if (gamemode == "darkmode" || gamemode == "spotlight")
                {
                    wallColor = Color.White;
                    outerWallColor = Color.White;
                    backgroundColor = Color.Black;
                }
                else
                {
                    wallColor = Color.Black;
                    outerWallColor = Color.Black;
                    backgroundColor = Color.Black;
                }
            }
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
            if (!pause)
            {
                foreach (Enemy enemy in enemies)
                {
                    enemy.laserElapsedTime += gameTime.ElapsedGameTime;
                }
            }
            checkColorScheme();
            #region Screen 0 Main Menu
            if (screen == 0)
            {
                startButton.Update();
                optionsButton.Update();
                if (startButton.Clicked)
                {
                    setScreen(1);
                }
                if (optionsButton.Released)
                {
                    setScreen(2);
                }
                Title.Hitbox.Y = (int)startButton.EndPosition.Y - 550;
                Title.Color = textColor;
                if (obstacles.Count == 0)
                {
                    newObstacle(500);
                    newObstacle(1000);
                }
                score++;
                if (gamemode == "fastmode")
                {
                    score++;
                }
                #region Updates Obstacles

                highestObstacle = 10;
                for (int i = 0; i < obstacles.Count; i++)
                {
                    Obstacles obstacle = obstacles[i];
                    obstacle.Update();
                    obstacle._size = new Vector2(obstacleSize, obstacleSize);
                    if (gamemode == "fastmode")
                    {
                        obstacle.Update();
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
            }
            #endregion
            #region Screen 1 Gameplay
            else if (screen == 1)//gameplay
            {
                KeyboardState ks = Keyboard.GetState();

                mouseHitbox.Update(gameTime);
                if (lose || isLoading)
                {
                    foreach (Obstacles obstacle in obstacles)
                    {
                        obstacle.Update();
                        if (gamemode == "fastmode")
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
                        //version 1 - to work on
                        /*
                        for (int obstacleToRemove = 0; obstacleToRemove < obstacles.Count; obstacleToRemove++)
                        {
                            if (!obstacles[obstacleToRemove].didKill && obstacles[obstacleToRemove]._color != textColor)
                            {
                                obstacles.RemoveAt(obstacleToRemove+1);
                            }
                        }

                        enemies.Clear();
                        */
                    }
                    else
                    {
                        //version 2

                        if (obstacles.Count > 134)
                        {
                            for (int obstacleToRemove = 0; obstacleToRemove < obstacles.Count; obstacleToRemove++)
                            {
                                if (!obstacles[obstacleToRemove].didKill && obstacles[obstacleToRemove]._color != textColor)
                                {
                                    obstacles.RemoveAt(obstacleToRemove);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            enemies.Clear();
                        }
                    }
                }
                else if (!lose && !pause)
                {
                    score++;
                    if (gamemode == "fastmode")
                    {
                        score++;
                    }
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

                if (!isLoading && !lose)
                {
#region Updates Obstacles
                    highestObstacle = 10;
                    for (int i = 0; i < obstacles.Count; i++)
                    {
                        Obstacles obstacle = obstacles[i];
                        obstacle.Update();
                        obstacle._size = new Vector2(obstacleSize, obstacleSize);
                        if (gamemode == "fastmode")
                        {
                            obstacle.Update();
                        }
                        if (obstacle.hitbox.Intersects(mouseHitbox._hitbox) && obstacle._slideSpeed == 0 && !pause && !obstacle._gateway)
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
                        if (gamemode == "fastmode")
                        {
                            enemy.Update();
                        }
                        if (enemy._rams && enemy.body._hitbox.Intersects(mouseHitbox._hitbox))
                        {
                            isLoading = true;
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
                }
                keyboardStuff();
            }
            #endregion
            #region Screen 2 Options
            else if (screen == 2)
            {
                score++;
                if (gamemode == "fastmode")
                {
                    score++;
                }
                #region Updates Obstacles

                highestObstacle = 10;
                for (int i = 0; i < obstacles.Count; i++)
                {
                    Obstacles obstacle = obstacles[i];
                    obstacle.Update();
                    obstacle._size = new Vector2(obstacleSize, obstacleSize);
                    if (gamemode == "fastmode")
                    {
                        obstacle.Update();
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
                mouseHitbox.Update(gameTime);
                #region Checks Color Button
                colorButton.Update();
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
                    colorButton._texture = Content.Load<Texture2D>(string.Format("{0}Button", colorScheme));
                }
                #endregion
                #region Checks Gamemode Button
                gamemodeButton.Update();
                if (gamemodeButton.Clicked)
                {
                    foreach (Obstacles obstacle in obstacles)
                    {
                        obstacle.Show = false;
                    }
                    if (gamemode == "regular")
                    {
                        gamemodeButton._texture = Content.Load<Texture2D>("DarkmodeButton");
                        gamemode = "darkmode";
                    }
                    else if (gamemode == "darkmode")
                    {
                        gamemodeButton._texture = Content.Load<Texture2D>("SpotlightButton");
                        gamemode = "spotlight";
                    }
                    else if (gamemode == "spotlight")
                    {
                        gamemodeButton._texture = Content.Load<Texture2D>("FastmodeButton");
                        gamemode = "fastmode";
                    }
                    else if (gamemode == "fastmode")
                    {
                        gamemodeButton._texture = Content.Load<Texture2D>("Regularbutton");
                        gamemode = "regular";
                    }
                }
                #endregion
                #region Checks Shootstyle Button
                shootStyleButton.Update();
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
                dotModeCheckbox.Update();
                if (dotModeCheckbox.isChecked)
                {
                    obstacleSize = 4;
                }
                else
                {
                    obstacleSize = 25;
                }
                backButton.Update();
                if (backButton.Clicked)
                {
                    setScreen(0);
                }
                keyboardStuff();
                if(secret >= 20)
                {
                    setScreen(3);
                }
            }
            #endregion
            #region Screen 3 
            else if (screen == 3)
            {
                keyboardStuff();
                B.Hitbox.X += B.XSpeed;
                B.Hitbox.Y += B.YSpeed;
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
                foreach (Obstacles obtsacle in obstacles)
                {
                    obtsacle.Draw(spriteBatch);
                }
            }
            if (screen == 0)//main menu
            {
                startButton.Draw(spriteBatch);
                optionsButton.Draw(spriteBatch);
                Title.Draw(spriteBatch);
            }
            else if (screen == 1)//gameplay
            {
                int laserCount = mouseHitbox.lasers.Count;
                foreach (Enemy enemy in enemies)
                {
                    enemy.Draw(spriteBatch);
                    laserCount += enemy.body.lasers.Count();
                }
                mouseHitbox.Draw(spriteBatch);
                if (mouseHitbox.Counting)
                {
                    spriteBatch.DrawString(font, string.Format($"0.{mouseHitbox.CountingCentisecond}"), mouseHitbox._position + new Vector2(-10, -40), textColor);
                }
                if (lose)
                {
                    spriteBatch.DrawString(endGameFont, $"Score:{score / 50}", new Vector2(125, 500), textColor);
                    spriteBatch.DrawString(endGameFont, $"Obstacles Destroyed:{destroyedObstacles}", new Vector2(125, 600), textColor);
                    spriteBatch.DrawString(endGameFont, $"Enemies Killed:{enemiesKilled}", new Vector2(125, 700), textColor);
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
            else if (screen == 2)
            {
                colorButton.Draw(spriteBatch);
                gamemodeButton.Draw(spriteBatch);
                shootStyleButton.Draw(spriteBatch);
                Vector2 superLongLineOfText = new Vector2(shootStyleButton.rectangle.X + shootStyleButton._texture.Width / 2 - Content.Load<Texture2D>("Ball").Width / 2, shootStyleButton.rectangle.Y + shootStyleButton._texture.Height / 2 - Content.Load<Texture2D>("Ball").Height / 2 + 10);
                mouseHitbox._color = ballColor;
                mouseHitbox._position = superLongLineOfText;
                backButton.Draw(spriteBatch);
                mouseHitbox.Draw(spriteBatch);
                int s = shootStyleButton.rectangle.Y; //make the line look less intimidating
                spriteBatch.DrawString(smallText, string.Format($"Num of Bullets: {mouseHitbox.stats.Item2}"), new Vector2(125, s + 80), textColor);
                spriteBatch.DrawString(smallText, string.Format($"Bullet Penetration: { mouseHitbox.stats.Item3}"), new Vector2(125, s + 95), textColor);
                spriteBatch.DrawString(smallText, string.Format($"Bullet Speed: {mouseHitbox.stats.Item4}"), new Vector2(125, s + 110), textColor);
                spriteBatch.DrawString(smallText, string.Format($"Reload: {mouseHitbox.stats.Item1.Seconds + (float)mouseHitbox.stats.Item1.Milliseconds / 1000} sec(s)"), new Vector2(125, s + 125), textColor);
                spriteBatch.DrawString(smallText, string.Format($"Pros: {mouseHitbox.stats.Item5}"), new Vector2(125, s + 140), textColor);
                spriteBatch.DrawString(smallText, string.Format($"Cons: {mouseHitbox.stats.Item6}"), new Vector2(125, s + 155), textColor);
                KeyboardState ks = Keyboard.GetState();
                spriteBatch.DrawString(smallText, string.Format($"{shootingLaser}"), new Vector2(125, s + 170), textColor);
                dotModeCheckbox.Draw(spriteBatch);

            }
            else if (screen == 3)
            {
                P.Draw(spriteBatch);
                B.Draw(spriteBatch);
                spriteBatch.DrawString(endGameFont, $"Score:{score}", new Vector2(0, 950), textColor);
            }
            spriteBatch.End();
            frames++;
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}