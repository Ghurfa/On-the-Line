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
    public class OnTheLine : Microsoft.Xna.Framework.Game
    {
        //Instance Variables
        #region Instance Variables
        GraphicsDeviceManager graphics;
        SpriteBatch generalSpriteBatch;

        public static MouseHitbox mouseHitbox;
        Texture2D obstaclePixel;

        SpriteFont infoGameFont;
        SpriteFont statsText;
        SpriteFont endGameFont;
        SpriteFont endGameFont2;

        public static bool hasLost = false;
        public static bool isPaused = false;
        public static bool isLoading = false;

        List<Obstacles> obstacles = new List<Obstacles>();
        public static List<Enemy> enemies = new List<Enemy>();

        public readonly int[] difficulty = { 0, 0, 0, 0, 40, 40, 60, 60, 80, 60, 80, 80, 100, 120, 100, 100, 120, 100, 120, 100, 120, 120, 120, 200, 120, 100 };

        Random random = new Random();

        public static Color WallColor = Color.Black;
        public static Color OuterWallColor = new Color(20, 20, 20);
        public static Color BackgroundColor = Color.White;
        public static Color TextColor = Color.Red;
        public static Color LaserColor = Color.Red;
        public static Color EndGameColor = Color.Black;
        Color pauseMenuColor = Color.LightGray;

        TimeSpan score = TimeSpan.Zero;
        int destroyedObstacles;
        int enemiesKilled;
        int slidingSpeed = 0;
        public static int ObstacleSize = 25;
        public static Screen screen = Screen.MainMenu;
        Screen lastScreen;
        Screen NextScreen;
        public static int shootStyle = 0;

        Sprite menuScreen;
        Sprite optionsScreen;
        Sprite inGameOptionsScreen;
        Sprite title;
        Sprite screenChanger;

        Button startButton;
        Button optionsButton;
        Button colorButton;
        Button backButton;
        Button gamemodeButton;
        Button shootStyleButton;
        Button obstacleSizeButton;
        Button restartButton;
        Button mainMenuButton;

        Button pauseMenu;

        KeyboardState ks;
        KeyboardState lastKs;

        public static GameMode gameMode = GameMode.Regular;
        public static ColorScheme colorScheme = ColorScheme.Default;
        public static int GlobalRotation = 0;
        public static float GlobalScaleFactor = 0.5f;
        #endregion
        public OnTheLine()
        {
            //Load default values
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = (int)(500 * GlobalScaleFactor);
            graphics.PreferredBackBufferHeight = (int)(1000 * GlobalScaleFactor);
        }

        protected override void Initialize()
        {
            IsMouseVisible = true;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            //Spritebatch initialization
            generalSpriteBatch = new SpriteBatch(GraphicsDevice);

            //Font initialization
            infoGameFont = Content.Load<SpriteFont>("Font");
            statsText = Content.Load<SpriteFont>("StatsText");
            endGameFont = Content.Load<SpriteFont>("EndGameFont");
            endGameFont2 = Content.Load<SpriteFont>("LargeText");

            //Pixel initialization
            obstaclePixel = new Texture2D(GraphicsDevice, 1, 1);
            obstaclePixel.SetData<Color>(new Color[] { Color.White });

            //MouseHitbox initialization
            mouseHitbox = new MouseHitbox(Color.LightGray, Content.Load<Texture2D>("Ball"), Content.Load<Texture2D>("Spotlight"), true, new Vector2(238, 250));

            //Button initialization
            startButton = new Button(new Vector2(125 * GlobalScaleFactor, 550 * GlobalScaleFactor), Content.Load<Texture2D>("StartButton"));
            optionsButton = new Button(new Vector2(125 * GlobalScaleFactor, 700 * GlobalScaleFactor), Content.Load<Texture2D>("OptionsButton"));
            colorButton = new Button(new Vector2(125 * GlobalScaleFactor, 100 * GlobalScaleFactor), Content.Load<Texture2D>(string.Format("{0}Button", colorScheme)));
            obstacleSizeButton = new Button(new Vector2(125 * GlobalScaleFactor, 300 * GlobalScaleFactor), Content.Load<Texture2D>("RegularSizeButton"));
            gamemodeButton = new Button(new Vector2(125 * GlobalScaleFactor, 200 * GlobalScaleFactor), Content.Load<Texture2D>(string.Format("{0}Button", gameMode)));
            shootStyleButton = new Button(new Vector2(125 * GlobalScaleFactor, 500 * GlobalScaleFactor), Content.Load<Texture2D>("EmptyButton"));
            backButton = new Button(new Vector2(125 * GlobalScaleFactor, 900 * GlobalScaleFactor), Content.Load<Texture2D>("BackButton"));
            restartButton = new Button(new Vector2(-500 * GlobalScaleFactor, 800 * GlobalScaleFactor), Content.Load<Texture2D>("RestartButton"));
            mainMenuButton = new Button(new Vector2(750 * GlobalScaleFactor, 800 * GlobalScaleFactor), Content.Load<Texture2D>("MainMenuButton"));
            pauseMenu = new Button(new Vector2(500 * GlobalScaleFactor, 250 * GlobalScaleFactor), Content.Load<Texture2D>("PauseMenu"));

            //Sprite initialization
            menuScreen = new Sprite(new Vector2(0 * GlobalScaleFactor, 0 * GlobalScaleFactor), Content.Load<Texture2D>("Screen"), Color.White);
            optionsScreen = new Sprite(new Vector2(-500 * GlobalScaleFactor, 0 * GlobalScaleFactor), Content.Load<Texture2D>("Screen"), Color.White);
            inGameOptionsScreen = new Sprite(new Vector2(500 * GlobalScaleFactor, 0 * GlobalScaleFactor), Content.Load<Texture2D>("Screen"), Color.White);
            title = new Sprite(new Vector2(0 * GlobalScaleFactor, 0 * GlobalScaleFactor), Content.Load<Texture2D>("Title"), Color.White);
            screenChanger = new Sprite(new Vector2(500 * GlobalScaleFactor, 0 * GlobalScaleFactor), Content.Load<Texture2D>("ScreenChanger"), Color.White);
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        /// <summary>
        /// Creates a new obstacle
        /// </summary>
        /// <param name="yOffset">Y offset for the obstacle being added</param>
        void newObstacle(float yOffset)
        {
            int randomNumber = random.Next(1, 27); //Chooses a random obstacle from all 26
            if (difficulty[randomNumber - 1] > score.TotalSeconds) //If you don't have enough score for it to load, try again
            {
                newObstacle(yOffset);
            }
            else
            {
                if (randomNumber == 8 || randomNumber == 15 || randomNumber == 26) //If the obstacle chosen is a longer obstacle
                {
                    yOffset -= 500;
                }
                if (randomNumber == 24) //If the obstacle is a super sized obstacle
                {
                    yOffset -= 1000;
                }
                loadObstacle(yOffset, string.Format("Obstacle{0}", randomNumber));
            }
        }
        /// <summary>
        /// Loads an obstacle based off X & Y offset
        /// </summary>
        /// <param name="yOffset">Y offset for the obstacle to be loaded</param>
        /// <param name="obstacleName"></param>
        /// <param name="xOffset">X offset for the obstacle to be loaded</param>
        void loadObstacle(float yOffset, string obstacleName, float xOffset = 0f)
        {
            //Instance variables
            Texture2D obstacleTexture = Content.Load<Texture2D>(obstacleName);
            Color[] pixels = new Color[obstacleTexture.Width * obstacleTexture.Height];
            obstacleTexture.GetData<Color>(pixels);
            Color currentPixel;
            Texture2D ball = Content.Load<Texture2D>("Ball");
            Texture2D empty = Content.Load<Texture2D>("Empty");
            Texture2D laserTexture = Content.Load<Texture2D>("Laser");
            Vector2 position = new Vector2(0, 0);
            Vector2 obstacleSize = new Vector2(25, 25);

            //Looks at every single pixel and decides what to load there based off the pixel's color
            for (int y = 0; y < obstacleTexture.Height; y++)
            {
                for (int x = 0; x < obstacleTexture.Width; x++)
                {
                    currentPixel = pixels[x + (y * obstacleTexture.Width)];
                    position.X = x * GlobalScaleFactor * 25 + xOffset;
                    position.Y = y * GlobalScaleFactor * 25 - (500 * GlobalScaleFactor) + yOffset;

                    if (currentPixel == Color.Red)
                    {
                        obstacles.Add(new Obstacles(obstaclePixel, position, obstacleSize, OuterWallColor, false, new Vector2(0, 0), 0, false, false, true));
                    }
                    else if (currentPixel == Color.Green)
                    {
                        obstacles.Add(new Obstacles(obstaclePixel, position, obstacleSize, new Color(TextColor.R, TextColor.G, TextColor.B, 230), false, new Vector2(0, 0), 0, false, false));
                    }
                    else if (currentPixel == Color.Purple)
                    {
                        obstacles.Add(new Obstacles(obstaclePixel, position, obstacleSize, WallColor, true, new Vector2(0, 0), 0, false, false));
                    }
                    else if (currentPixel == Color.Orange)
                    {
                        enemies.Add(new Enemy(position, ball, empty, laserTexture, 0, 0, true, false));
                    }
                    else if (currentPixel == Color.Aqua)
                    {
                        enemies.Add(new Enemy(position, ball, empty, laserTexture, 0, 0, true, true));
                    }
                    else if (currentPixel == Color.Black)
                    {
                        obstacles.Add(new Obstacles(obstaclePixel, position, obstacleSize, WallColor, false, new Vector2(0, 0), 0, true, false));
                    }
                    else if (currentPixel == Color.Blue)
                    {
                        obstacles.Add(new Obstacles(obstaclePixel, position, obstacleSize, BackgroundColor, false, new Vector2(0, 0), 0, false, true));
                    }
                    else if (currentPixel.R == 254)
                    {
                        enemies.Add(new Enemy(position, ball, empty, laserTexture, currentPixel.G, currentPixel.B, false, false));
                    }
                    else if (currentPixel != Color.White)
                    {
                        obstacles.Add(new Obstacles(obstaclePixel, position, obstacleSize, WallColor, false, new Vector2(currentPixel.R - 100, currentPixel.G - 100), currentPixel.B, false, false));
                    }
                }
            }
        }
        public void setScreen(Screen screenToSetTo)
        {
            mouseHitbox.lasers.Clear();
            mouseHitbox.IsClicked = false;
            lastScreen = screen;
            if (slidingSpeed == 0)
            {
                if (screenToSetTo == Screen.MainMenu) //If you are switching to main menu
                {
                    hasLost = false;
                    isPaused = false;
                    menuScreen.Position = new Vector2(528 * GlobalScaleFactor, 0);
                    slidingSpeed = 32;
                }
                else if (screenToSetTo == Screen.ScreenTransition)
                {
                    screenChanger.Position = new Vector2(500 * GlobalScaleFactor, 0);
                    screenChanger.Speed = new Vector2(-30, 0);
                }
                else if (screenToSetTo == Screen.GameScreen && lastScreen != Screen.InGameOptionsMenu)
                {
                    hasLost = false;
                    score = TimeSpan.Zero;
                    obstacles.Clear();
                    enemies.Clear();
                    destroyedObstacles = 0;
                    enemiesKilled = 0;
                    loadObstacle(1000 * GlobalScaleFactor, "LowerStartingObstacle");
                    loadObstacle(500 * GlobalScaleFactor, string.Format("startingObstacle{0}", random.Next(1, 4)));
                    mouseHitbox.canShoot = true;
                    mouseHitbox = new MouseHitbox(mouseHitbox.Color, Content.Load<Texture2D>("Ball"), Content.Load<Texture2D>("Spotlight"), true, new Vector2(238 * GlobalScaleFactor, 250 * GlobalScaleFactor));
                    restartButton.Position = new Vector2(-500 * GlobalScaleFactor, 800 * GlobalScaleFactor);
                    mainMenuButton.Position = new Vector2(750 * GlobalScaleFactor, 800 * GlobalScaleFactor);
                }
                else if (screenToSetTo == Screen.OptionsMenu)
                {
                    hasLost = false;
                    optionsScreen.Position = new Vector2(528 * GlobalScaleFactor, 0);
                    slidingSpeed = 32;
                }
                if (screenToSetTo == Screen.InGameOptionsMenu)
                {
                    inGameOptionsScreen.Position = new Vector2(528 * GlobalScaleFactor, 0);
                    slidingSpeed = 32;
                }
                else
                {
                    pauseMenu.Position = new Vector2(500 * GlobalScaleFactor, 250 * GlobalScaleFactor);
                }
                screen = screenToSetTo;
            }
        }
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            menuScreen.Update();
            optionsScreen.Update();
            if (obstacles.Count == 0) //If there are no obstacles, make some
            {
                if (screen == Screen.GameScreen)
                {
                    enemiesKilled = 0;
                    loadObstacle(1000 * GlobalScaleFactor, "LowerStartingObstacle");
                    loadObstacle(500 * GlobalScaleFactor, string.Format("startingObstacle{0}", random.Next(1, 4)));
                }
                else
                {
                    newObstacle(500 * GlobalScaleFactor);
                    newObstacle(1000 * GlobalScaleFactor);
                }
            }
            if (slidingSpeed != 0) //If the screen sprites are supposed to move, move them & decrease the speed they slide
            {
                menuScreen.Position.X -= slidingSpeed * GlobalScaleFactor;
                optionsScreen.Position.X -= slidingSpeed * GlobalScaleFactor;
                inGameOptionsScreen.Position.X -= slidingSpeed * GlobalScaleFactor;
                slidingSpeed--;
            }
            mouseHitbox.Update(gameTime);
            int highestObstacle = 10;
            if (screen != Screen.InGameOptionsMenu)
            {
                optionsButton.Update(TextColor);
            }
            if (screen == (int)Screen.MainMenu || screen == Screen.OptionsMenu)
            {
                screenChanger.Position += screenChanger.Speed;
                startButton.Update(TextColor);
                title.Update();
                if (startButton.Clicked)
                {
                    setScreen(Screen.ScreenTransition);
                    NextScreen = Screen.GameScreen;
                }
                if (optionsButton.Released)
                {
                    setScreen(Screen.OptionsMenu);
                }
                if (obstacles.Count == 0)
                {
                    newObstacle(500 * GlobalScaleFactor);
                    newObstacle(1000 * GlobalScaleFactor);
                }
                startButton.Position = new Vector2(125 * GlobalScaleFactor, 550 * GlobalScaleFactor) - (menuScreen.Position - new Vector2(0, 0));
                optionsButton.Position = menuScreen.Position + new Vector2(125 * GlobalScaleFactor, 700 * GlobalScaleFactor);
                title.Position = new Vector2(90 * GlobalScaleFactor, 30 * GlobalScaleFactor - Math.Abs(menuScreen.Position.X));
                title.Color = TextColor;
                //Screen 2
                #region Checks Color Button
                colorButton.Update(TextColor);
                if (colorButton.Clicked)
                {
                    EndGameColor = Color.White;
                    if (colorScheme == ColorScheme.School)
                    {
                        colorScheme = ColorScheme.Default;
                    }
                    else
                    {
                        colorScheme++;
                    }
                    switch (colorScheme)
                    {
                        case ColorScheme.Default:
                            mouseHitbox.Color = Color.LightGray;
                            TextColor = Color.Red;
                            LaserColor = Color.Red;
                            WallColor = Color.Black;
                            OuterWallColor = new Color(20, 20, 20);
                            BackgroundColor = Color.White;
                            EndGameColor = Color.Black;
                            pauseMenuColor = Color.LightGray;
                            break;
                        case ColorScheme.Ice:
                            mouseHitbox.Color = new Color(255, 150, 0);
                            TextColor = new Color(255, 150, 0);
                            LaserColor = new Color(255, 150, 0);
                            WallColor = new Color(30, 220, 230);
                            OuterWallColor = new Color(37, 37, 37);
                            BackgroundColor = new Color(0, 0, 0);
                            pauseMenuColor = new Color(255, 255, 255);
                            break;
                        case ColorScheme.Beach:
                            mouseHitbox.Color = new Color(45, 105, 174);
                            TextColor = new Color(0, 183, 45);
                            LaserColor = new Color(45, 105, 174);
                            WallColor = new Color(45, 105, 174);
                            OuterWallColor = new Color(30, 44, 96);
                            BackgroundColor = new Color(240, 210, 150);
                            pauseMenuColor = new Color(54, 54, 54);
                            break;
                        case ColorScheme.Gingerbread:
                            mouseHitbox.Color = new Color(50, 20, 0);
                            TextColor = new Color(50, 20, 0);
                            LaserColor = new Color(50, 20, 0);
                            WallColor = Color.White;
                            OuterWallColor = new Color(40, 10, 0);
                            BackgroundColor = new Color(80, 50, 20);
                            pauseMenuColor = new Color(100, 100, 100);
                            break;
                        case ColorScheme.School:
                            mouseHitbox.Color = Color.Black;
                            TextColor = Color.Black;
                            LaserColor = Color.Black;
                            WallColor = new Color(10, 10, 10);
                            OuterWallColor = new Color(20, 20, 20);
                            BackgroundColor = new Color(30, 30, 30);
                            pauseMenuColor = new Color(100, 100, 100);
                            break;
                    }
                    colorButton.Texture = Content.Load<Texture2D>(string.Format("{0}Button", colorScheme));
                }
                #endregion
                #region Checks Gamemode Button
                if (lastScreen != Screen.GameScreen)
                {
                    gamemodeButton.Update(TextColor);
                    if (gamemodeButton.Clicked)
                    {
                        foreach (Obstacles obstacle in obstacles)
                        {
                            obstacle.Show = false;
                        }
                        switch (gameMode)
                        {
                            case GameMode.Regular:
                                gamemodeButton.Texture = Content.Load<Texture2D>("DarkmodeButton");
                                break;
                            case GameMode.Darkmode:
                                gamemodeButton.Texture = Content.Load<Texture2D>("SpotlightButton");
                                break;
                            case GameMode.Spotlight:
                                gamemodeButton.Texture = Content.Load<Texture2D>("FastmodeButton");
                                break;
                            case GameMode.Fastmode:
                                gamemodeButton.Texture = Content.Load<Texture2D>("Regularbutton");
                                break;
                        }
                        if (gameMode == GameMode.Fastmode)
                        {
                            gameMode = GameMode.Regular;
                        }
                        else
                        {
                            gameMode++;
                        }
                    }
                    #endregion
                #region Checks Shootstyle Buttoni
                    shootStyleButton.Update(TextColor);
                    if (shootStyleButton.Clicked)
                    {
                        mouseHitbox.lasers.Clear();
                        shootStyle++;
                        shootStyle %= 6;
                        mouseHitbox.canShoot = true;
                        mouseHitbox.Update(gameTime);
                        mouseHitbox.fireLasers(Content.Load<Texture2D>("Laser"), LaserColor, false);
                    }
                }
                #endregion
                #region Checks Obstacle Size Button
                obstacleSizeButton.Update(TextColor);
                if (obstacleSizeButton.Clicked)
                {
                    if (ObstacleSize == 25)
                    {
                        obstacleSizeButton.Texture = Content.Load<Texture2D>("TinySizeButton");
                        ObstacleSize = 4;
                    }
                    else
                    {
                        obstacleSizeButton.Texture = Content.Load<Texture2D>("RegularSizeButton");
                        ObstacleSize = 25;
                    }
                }
                #endregion
                backButton.Update(TextColor);
                if (backButton.Clicked)
                {
                    setScreen(Screen.MainMenu);
                }
                colorButton.Position = optionsScreen.Position + new Vector2(125 * GlobalScaleFactor, 290 * GlobalScaleFactor);
                gamemodeButton.Position = new Vector2(125 * GlobalScaleFactor, 420 * GlobalScaleFactor) - (optionsScreen.Position - new Vector2(0, 0));
                obstacleSizeButton.Position = optionsScreen.Position + new Vector2(125 * GlobalScaleFactor, 550 * GlobalScaleFactor);
                shootStyleButton.Position = new Vector2(125 * GlobalScaleFactor, 680 * GlobalScaleFactor) - (optionsScreen.Position - new Vector2(0, 0));
                backButton.Position = optionsScreen.Position + new Vector2(125 * GlobalScaleFactor, 900 * GlobalScaleFactor);
            }
            if (!hasLost && !isLoading)
            {
                #region Updates Obstacles
                for (int i = 0; i < obstacles.Count; i++)
                {
                    Obstacles obstacle = obstacles[i];
                    obstacle.Update();
                    obstacle.Size = new Vector2(GlobalScaleFactor * ObstacleSize, GlobalScaleFactor * ObstacleSize);
                    if (gameMode == GameMode.Fastmode)
                    {
                        obstacle.Update();
                    }
                    if (obstacle.Hitbox.Intersects(mouseHitbox.Hitbox) && obstacle.SlideSpeed == 0 && !isPaused && !obstacle.Gateway && screen == Screen.GameScreen)
                    {
                        isLoading = true;
                        obstacle.didKill = true;
                        obstacle.Color = EndGameColor;
                    }
                    if (obstacle.Hitbox.Y < highestObstacle)
                    {
                        highestObstacle = obstacle.Hitbox.Y;
                    }
                    if (obstacle.Breaks)
                    {
                        for (int x = 0; x < mouseHitbox.lasers.Count; x++)
                        {
                            Laser laser = mouseHitbox.lasers[x];
                            if (laser.Hitbox.Intersects(obstacle.Hitbox))
                            {
                                obstacles.Remove(obstacle);
                                destroyedObstacles++;
                                score += new TimeSpan(0, 0, 10);
                                i--;
                                laser._lives--;
                                if (laser._lives <= 0)
                                {
                                    OnTheLine.mouseHitbox.lasers.Remove(laser);
                                }
                            }
                        }
                    }
                    if (obstacle.Hitbox.Y > 2000 * GlobalScaleFactor)
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
                    enemy.Update(gameTime);
                    if (gameMode == GameMode.Fastmode)
                    {
                        enemy.Update(gameTime);
                    }
                    foreach (Obstacles obstacle in obstacles)
                    {
                        if (obstacle.Hitbox.Intersects(enemy.Hitbox) && obstacle.Gateway)
                        {
                            enemies.Remove(enemy);
                            enemiesKilled++;
                        }
                    }
                    for (int x = 0; x < mouseHitbox.lasers.Count; x++)
                    {
                        Laser laser = mouseHitbox.lasers[x];
                        if (enemy.Hitbox.Intersects(laser.Hitbox))
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
                            for (int x = 0; x < enemy.lasers.Count; x++)
                            {
                                Laser laser = enemy.lasers[x];
                                if (targetedEnemy.Hitbox.Intersects(laser.Hitbox))
                                {
                                    enemies.Remove(targetedEnemy);
                                    enemiesKilled++;
                                    enemy.lasers.Remove(laser);
                                }
                            }
                        }
                    }
                }
                #endregion
                if (!isPaused)
                {
                    score += gameTime.ElapsedGameTime;
                    if (gameMode == GameMode.Fastmode)
                    {
                        score += gameTime.ElapsedGameTime;
                    }
                }
            }
            #region Check Keyboard
            ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.M) && !lastKs.IsKeyDown(Keys.M))
            {
                if (screen == Screen.OptionsMenu)
                {
                    setScreen(Screen.MainMenu);
                }
            }
            if (ks.IsKeyDown(Keys.S) && !lastKs.IsKeyDown(Keys.S))
            {
                score = new TimeSpan(0, 0, 100);
            }
            if (!hasLost)
            {
                if (ks.IsKeyDown(Keys.Up) || ks.IsKeyDown(Keys.Down))
                {
                    for (int i = 0; i < obstacles.Count; i++)
                    {
                        Obstacles obstacle = obstacles[i];
                        obstacle.Update();
                        for (int x = 0; x < OnTheLine.mouseHitbox.lasers.Count; x++)
                        {
                            if (obstacle.Hitbox.Intersects(OnTheLine.mouseHitbox.lasers[x].Hitbox) && obstacle.Breaks)
                            {
                                OnTheLine.mouseHitbox.lasers[x]._lives -= 2;
                                if (OnTheLine.mouseHitbox.lasers[x]._lives <= 0)
                                {
                                    OnTheLine.mouseHitbox.lasers.Remove(OnTheLine.mouseHitbox.lasers[x]);
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
                        if (obstacle.Hitbox.Y < highestObstacle)
                        {
                            highestObstacle = obstacle.Hitbox.Y;
                        }
                    }
                    if (highestObstacle >= 0)
                    {
                        newObstacle(highestObstacle);
                    }
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        Enemy enemy = enemies[i];
                        enemy.Update(gameTime);
                        if (ks.IsKeyDown(Keys.RightControl))
                        {
                            enemy.Update(gameTime);
                        }
                        if (ks.IsKeyDown(Keys.LeftControl))
                        {
                            for (int d = 0; d < 6; d++)
                            {
                                enemy.Update(gameTime);
                            }
                        }
                    }
                }
            }
            if (ks.IsKeyDown(Keys.Space) && mouseHitbox.canShoot)
            {
                mouseHitbox.fireLasers(Content.Load<Texture2D>("Laser"), LaserColor, false);
                mouseHitbox.laserElapsedTime = TimeSpan.Zero;
                mouseHitbox.canShoot = false;
            }
            lastKs = ks;
            #endregion
            if (screen == Screen.GameScreen)//gameplay
            {
                screenChanger.Position += screenChanger.Speed;
                pauseMenu.Update(pauseMenuColor, false);
                pauseMenu.Speed.X = 0;
                optionsButton.Position = pauseMenu.Position + new Vector2(40 * GlobalScaleFactor, 50 * GlobalScaleFactor);
                restartButton.Update(TextColor);
                if (restartButton.Clicked)
                {
                    setScreen(Screen.ScreenTransition);
                    NextScreen = Screen.GameScreen;
                }
                mainMenuButton.Update(TextColor);
                if (mainMenuButton.Clicked)
                {
                    setScreen(Screen.ScreenTransition);
                    NextScreen = Screen.MainMenu;
                }
                if (isPaused)
                {
                    if (pauseMenu.Position.X > 480 * GlobalScaleFactor)
                    {
                        pauseMenu.Speed.X = -1 * GlobalScaleFactor;
                    }
                    else if (pauseMenu.Hovered)
                    {
                        if (pauseMenu.Position.X > 170 * GlobalScaleFactor)
                        {
                            pauseMenu.Speed.X = -15 * GlobalScaleFactor;
                        }
                        else if (optionsButton.Clicked)
                        {
                            setScreen(Screen.InGameOptionsMenu);
                        }
                    }
                    else if (pauseMenu.Position.X < 480 * GlobalScaleFactor)
                    {
                        pauseMenu.Speed.X = 15 * GlobalScaleFactor;
                    }
                }
                else
                {
                    if (pauseMenu.Position.X < 500 * GlobalScaleFactor)
                    {
                        pauseMenu.Speed.X = 1 * GlobalScaleFactor;
                    }
                    if (hasLost || isLoading)
                    {
                        if (restartButton.Position.X < 0)
                        {
                            restartButton.Speed.X = 10 * GlobalScaleFactor;
                            mainMenuButton.Speed.X = -10 * GlobalScaleFactor;
                            restartButton.Position += restartButton.Speed;
                            mainMenuButton.Position += mainMenuButton.Speed;
                        }
                        else
                        {
                            restartButton.Speed.X = 0;
                            mainMenuButton.Speed.X = 0;
                        }
                        foreach (Obstacles obstacle in obstacles)
                        {
                            obstacle.Update();
                            if (gameMode == GameMode.Fastmode)
                            {
                                obstacle.Update();
                            }
                        }
                        if (isLoading)
                        {
                            hasLost = true;
                            loadObstacle(525 * GlobalScaleFactor, "YouLose");
                            isLoading = false;
                            mouseHitbox.lasers.Clear();
                        }
                    }
                    else if (!hasLost)
                    {
                        MouseState ms = Mouse.GetState();
                        if (ms.Y < 0)
                        {
                            Mouse.SetPosition(ms.X, 0);
                        }
                        else if (ms.Y > 1000 * GlobalScaleFactor)
                        {
                            Mouse.SetPosition(ms.X, (int)(1000 * GlobalScaleFactor));
                        }
                        if (ms.X < 0)
                        {
                            Mouse.SetPosition(0, ms.Y);
                        }
                        else if (ms.X > 500 * GlobalScaleFactor)
                        {
                            Mouse.SetPosition((int)(500 * GlobalScaleFactor), ms.Y);
                        }
                    }
                }
                pauseMenu.Position.X += pauseMenu.Speed.X;
            }
            else if (screen == Screen.InGameOptionsMenu)
            {
                optionsButton.Position = pauseMenu.Position + new Vector2(40 * GlobalScaleFactor, 50 * GlobalScaleFactor);
                if (pauseMenu.Position.X < 480 * GlobalScaleFactor)
                {
                    pauseMenu.Speed.X = 15 * GlobalScaleFactor;
                }
                pauseMenu.Position.X += pauseMenu.Speed.X;
                pauseMenu.Update(pauseMenuColor, false);
                optionsButton.Update(TextColor, false);
                #region Checks Color Button
                colorButton.Update(TextColor);
                if (colorButton.Clicked)
                {
                    EndGameColor = Color.White;
                    if (colorScheme == ColorScheme.School)
                    {
                        colorScheme = ColorScheme.Default;
                    }
                    else
                    {
                        colorScheme++;
                    }
                    switch (colorScheme)
                    {
                        case ColorScheme.Default:
                            mouseHitbox.Color = Color.LightGray;
                            TextColor = Color.Red;
                            LaserColor = Color.Red;
                            WallColor = Color.Black;
                            OuterWallColor = new Color(20, 20, 20);
                            BackgroundColor = Color.White;
                            EndGameColor = Color.Black;
                            pauseMenuColor = Color.LightGray;
                            break;
                        case ColorScheme.Ice:
                            mouseHitbox.Color = new Color(255, 150, 0);
                            TextColor = new Color(255, 150, 0);
                            LaserColor = new Color(255, 150, 0);
                            WallColor = new Color(30, 220, 230);
                            OuterWallColor = new Color(37, 37, 37);
                            BackgroundColor = new Color(0, 0, 0);
                            pauseMenuColor = new Color(255, 255, 255);
                            break;
                        case ColorScheme.Beach:
                            mouseHitbox.Color = new Color(45, 105, 174);
                            TextColor = new Color(0, 183, 45);
                            LaserColor = new Color(45, 105, 174);
                            WallColor = new Color(45, 105, 174);
                            OuterWallColor = new Color(30, 44, 96);
                            BackgroundColor = new Color(240, 210, 150);
                            pauseMenuColor = new Color(54, 54, 54);
                            break;
                        case ColorScheme.Gingerbread:
                            mouseHitbox.Color = new Color(50, 20, 0);
                            TextColor = new Color(50, 20, 0);
                            LaserColor = new Color(50, 20, 0);
                            WallColor = Color.White;
                            OuterWallColor = new Color(40, 10, 0);
                            BackgroundColor = new Color(80, 50, 20);
                            pauseMenuColor = new Color(100, 100, 100);
                            break;
                        case ColorScheme.School:
                            mouseHitbox.Color = Color.Black;
                            TextColor = Color.Black;
                            LaserColor = Color.Black;
                            WallColor = new Color(10, 10, 10);
                            OuterWallColor = new Color(20, 20, 20);
                            BackgroundColor = new Color(30, 30, 30);
                            pauseMenuColor = new Color(100, 100, 100);
                            break;
                    }
                    colorButton.Texture = Content.Load<Texture2D>(string.Format("{0}Button", colorScheme));
                }
                #endregion
                #region Checks Obstacle Size Button
                obstacleSizeButton.Update(TextColor);
                if (obstacleSizeButton.Clicked)
                {
                    if (ObstacleSize == 25)
                    {
                        obstacleSizeButton.Texture = Content.Load<Texture2D>("TinySizeButton");
                        ObstacleSize = 4;
                    }
                    else
                    {
                        obstacleSizeButton.Texture = Content.Load<Texture2D>("RegularSizeButton");
                        ObstacleSize = 25;
                    }
                }
                #endregion
                backButton.Update(TextColor);
                if (backButton.Clicked)
                {
                    setScreen(lastScreen);
                }
                colorButton.Position = inGameOptionsScreen.Position + new Vector2(125 * GlobalScaleFactor, 290 * GlobalScaleFactor);
                obstacleSizeButton.Position = inGameOptionsScreen.Position + new Vector2(125 * GlobalScaleFactor, 550 * GlobalScaleFactor);
                backButton.Position = inGameOptionsScreen.Position + new Vector2(125 * GlobalScaleFactor, 900 * GlobalScaleFactor);
            }
            if (screen == Screen.ScreenTransition)
            {
                screenChanger.Color = BackgroundColor;
                if (screenChanger.Position.X > -500 * GlobalScaleFactor)
                {
                    screenChanger.Position += screenChanger.Speed;
                }
                else
                {
                    obstacles.Clear();
                    enemies.Clear();
                    setScreen(NextScreen);
                }
                if (restartButton.Position.X > -500 * GlobalScaleFactor)
                {
                    restartButton.Speed.X = -10;
                    mainMenuButton.Speed.X = 10;
                    restartButton.Position += restartButton.Speed;
                    mainMenuButton.Position += mainMenuButton.Speed;
                }
                else
                {
                    restartButton.Speed.X = 0;
                    mainMenuButton.Speed.X = 0;
                }
            }
            base.Update(gameTime);
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(BackgroundColor);
            generalSpriteBatch.Begin();
            foreach (Obstacles obstacle in obstacles) //Layer 1 - Regular obstacles
            {
                obstacle.Draw(generalSpriteBatch);
            }
            if (screen == Screen.InGameOptionsMenu)
            {
                mouseHitbox.Draw(generalSpriteBatch);
                pauseMenu.Draw(generalSpriteBatch);
                optionsButton.Draw(generalSpriteBatch);
                colorButton.Draw(generalSpriteBatch);
                obstacleSizeButton.Draw(generalSpriteBatch);
                backButton.Draw(generalSpriteBatch);
            }
            if (screen == Screen.MainMenu || screen == Screen.OptionsMenu)
            {
                mouseHitbox.Draw(generalSpriteBatch);
                startButton.Draw(generalSpriteBatch);
                optionsButton.Draw(generalSpriteBatch);
                title.Draw(generalSpriteBatch);
                // Screen 2
                colorButton.Draw(generalSpriteBatch);
                obstacleSizeButton.Draw(generalSpriteBatch);
                gamemodeButton.Draw(generalSpriteBatch);
                shootStyleButton.Draw(generalSpriteBatch);
                generalSpriteBatch.DrawString(statsText, string.Format($"Num of Bullets: {mouseHitbox.stats.Item2}"), shootStyleButton.Position + new Vector2(0, 80 * GlobalScaleFactor), TextColor);
                generalSpriteBatch.DrawString(statsText, string.Format($"Bullet Penetration: {mouseHitbox.stats.Item3}"), shootStyleButton.Position + new Vector2(0, 95 * GlobalScaleFactor), TextColor);
                generalSpriteBatch.DrawString(statsText, string.Format($"Bullet Speed: {mouseHitbox.stats.Item4}"), shootStyleButton.Position + new Vector2(0, 110 * GlobalScaleFactor), TextColor);
                generalSpriteBatch.DrawString(statsText, string.Format($"Reload: {mouseHitbox.stats.Item1.Seconds + (float)mouseHitbox.stats.Item1.Milliseconds / 1000} sec(s)"), shootStyleButton.Position + new Vector2(0, 125 * GlobalScaleFactor), TextColor);
                generalSpriteBatch.DrawString(statsText, string.Format($"Pros: {mouseHitbox.stats.Item5}"), shootStyleButton.Position + new Vector2(0, 140 * GlobalScaleFactor), TextColor);
                generalSpriteBatch.DrawString(statsText, string.Format($"Cons: {mouseHitbox.stats.Item6}"), shootStyleButton.Position + new Vector2(0, 155 * GlobalScaleFactor), TextColor);
                mouseHitbox.Position = new Vector2((int)shootStyleButton.Position.X + shootStyleButton.Texture.Width / 2 * GlobalScaleFactor - Content.Load<Texture2D>("Ball").Width / 2 * GlobalScaleFactor, (int)shootStyleButton.Position.Y + shootStyleButton.Texture.Height / 2 * GlobalScaleFactor - (Content.Load<Texture2D>("Ball").Height / 2 * GlobalScaleFactor));
                backButton.Draw(generalSpriteBatch);
                //dotModeCheckbox.Draw(generalSpriteBatch);
            }
            foreach (Enemy enemy in enemies)
            {
                if (gameMode != GameMode.Spotlight || (gameMode == GameMode.Spotlight && Math.Abs(enemy.RelativePositon(mouseHitbox.Position).Y) < 400 * GlobalScaleFactor))
                {
                    enemy.Draw(generalSpriteBatch);
                }
            }
            foreach (Obstacles obtstacle in obstacles) //Layer 2 - Obstacle that killed you
            {
                if (obtstacle.didKill)
                {
                    obtstacle.Draw(generalSpriteBatch);
                }
            }
            restartButton.Draw(generalSpriteBatch);
            mainMenuButton.Draw(generalSpriteBatch);
            if (screen == Screen.GameScreen)//gameplay
            {
                int laserCount = mouseHitbox.lasers.Count;
                mouseHitbox.Draw(generalSpriteBatch);
                if (mouseHitbox.Counting)
                {
                    generalSpriteBatch.DrawString(infoGameFont, string.Format($"0.{mouseHitbox.CountingCentisecond}"), mouseHitbox.Position + new Vector2(-10 * GlobalScaleFactor, -40 * GlobalScaleFactor), TextColor);
                }
                if (hasLost)
                {
                    generalSpriteBatch.DrawString(endGameFont, $"Score: {(int)score.TotalSeconds}", new Vector2(100 * GlobalScaleFactor, 450 * GlobalScaleFactor), TextColor);
                    generalSpriteBatch.DrawString(endGameFont, $"Obstacles Destroyed: {destroyedObstacles}", new Vector2(100 * GlobalScaleFactor, 500 * GlobalScaleFactor), TextColor);
                    generalSpriteBatch.DrawString(endGameFont, $"Enemies Killed :{enemiesKilled}", new Vector2(100 * GlobalScaleFactor, 550 * GlobalScaleFactor), TextColor);
                }
                else
                {
                    generalSpriteBatch.DrawString(infoGameFont, string.Format($"{obstacles.Count}"), new Vector2(0, 950 * GlobalScaleFactor), TextColor);
                    generalSpriteBatch.DrawString(infoGameFont, string.Format($"Score: {(int)score.TotalSeconds}"), new Vector2(380 * GlobalScaleFactor, 950 * GlobalScaleFactor), TextColor);
                    generalSpriteBatch.DrawString(infoGameFont, string.Format($"{laserCount}"), new Vector2(240 * GlobalScaleFactor, 950 * GlobalScaleFactor), TextColor);
                }
                pauseMenu.Draw(generalSpriteBatch);
                optionsButton.Draw(generalSpriteBatch);
            }
            foreach (Obstacles obtstacle in obstacles) //Layer 3 - "You Lose"
            {
                if (obtstacle.Color.A == 230)
                {
                    obtstacle.Draw(generalSpriteBatch);
                }
            }
            screenChanger.Draw(generalSpriteBatch);
            generalSpriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}