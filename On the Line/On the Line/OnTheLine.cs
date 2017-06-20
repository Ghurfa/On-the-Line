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

        public static Player player;
        Texture2D obstaclePixel;

        SpriteFont infoGameFont;
        SpriteFont statsText;
        public static SpriteFont endGameFont;
        SpriteFont endGameFont2;

        public static bool hasLost = false;
        public static bool isPaused = false;
        public static bool isLoading = false;

        List<Obstacles> obstacles = new List<Obstacles>();
        public static List<Enemy> enemies = new List<Enemy>();

        public readonly int[] difficulty = { 0, 0, 0, 0, 40, 40, 60, 60, 80, 60, 80, 80, 100, 120, 100, 100, 120, 100, 120, 100, 120, 120, 120, 200, 120, 100 };
        public readonly int[] level1 = { 1, 2, 3, 4, 5 };
        Dictionary<ColorScheme, Color> wallColors = new Dictionary<ColorScheme, Color>();
        Dictionary<ColorScheme, Color> outerWallColors = new Dictionary<ColorScheme, Color>();
        Dictionary<ColorScheme, Color> backgroundColors = new Dictionary<ColorScheme, Color>();
        Dictionary<ColorScheme, Color> textColors = new Dictionary<ColorScheme, Color>();
        Dictionary<ColorScheme, Color> laserColors = new Dictionary<ColorScheme, Color>();
        Dictionary<ColorScheme, Color> endGameColors = new Dictionary<ColorScheme, Color>();
        Dictionary<ColorScheme, Color> pauseMenuColors = new Dictionary<ColorScheme, Color>();
        Dictionary<ColorScheme, Color> playerColors = new Dictionary<ColorScheme, Color>();

        List<Dictionary<ColorScheme, Color>> colorLists = new List<Dictionary<ColorScheme, Color>>();
        List<PaletteSelector> colorSelectors = new List<PaletteSelector>();

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
        int lastObstacle = 0;
        public static int ObstacleSize = 25;
        public static Screen screen = Screen.MainMenu;
        Screen lastScreen;
        Screen NextScreen;
        public static int shootStyle = 0;

        Sprite menuScreen;
        Sprite optionsScreen;
        Sprite inGameOptionsScreen;
        Sprite levelsScreen;
        Sprite title;
        Sprite screenChanger;
        Sprite leftSideFiller;
        Sprite rightSideFiller;

        Button startButton;
        Button levelsButton;
        Button optionsButton;
        Button colorButton;
        Button backButton;
        Button gamemodeButton;
        Button shootStyleButton;
        Button obstacleSizeButton;
        Button restartButton;
        Button mainMenuButton;
        Button levelStartButton;

        ArrowSelector levelSelector;

        Button pauseMenu;

        KeyboardState ks;
        KeyboardState lastKs;

        int level = 1;
        int obstaclesLoadedSoFar = 0;

        public static GameMode gameMode = GameMode.Regular;
        public static ColorScheme colorScheme = ColorScheme.Default;
        public static int GlobalRotation = 0;
        public static float GlobalScaleFactor = 1f;
        public static int FillerSpaceOnSide = 50;
        #endregion
        public OnTheLine()
        {
            //Load default values
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = (int)(500 * GlobalScaleFactor) + 2 * FillerSpaceOnSide;
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

            //Player initialization
            player = new Player(Color.LightGray, Content.Load<Texture2D>("Ball"), Content.Load<Texture2D>("Spotlight"), true, new Vector2((500 * GlobalScaleFactor + 2 * FillerSpaceOnSide) / 2, 250 * GlobalScaleFactor));

            //Button initialization
            startButton = new Button(new Vector2(125 * GlobalScaleFactor + FillerSpaceOnSide, 550 * GlobalScaleFactor), Content.Load<Texture2D>("StartButton"));
            levelsButton = new Button(new Vector2(125 * GlobalScaleFactor + FillerSpaceOnSide, 725 * GlobalScaleFactor), Content.Load<Texture2D>("LevelsButton"));
            optionsButton = new Button(new Vector2(125 * +FillerSpaceOnSide, 900 * GlobalScaleFactor), Content.Load<Texture2D>("OptionsButton"));
            colorButton = new Button(new Vector2(125 * GlobalScaleFactor + FillerSpaceOnSide, 100 * GlobalScaleFactor), Content.Load<Texture2D>(string.Format("{0}Button", colorScheme)));
            obstacleSizeButton = new Button(new Vector2(125 * GlobalScaleFactor + FillerSpaceOnSide, 300 * GlobalScaleFactor), Content.Load<Texture2D>("RegularSizeButton"));
            gamemodeButton = new Button(new Vector2(125 * GlobalScaleFactor + FillerSpaceOnSide, 200 * GlobalScaleFactor), Content.Load<Texture2D>(string.Format("{0}Button", gameMode)));
            shootStyleButton = new Button(new Vector2(125 * GlobalScaleFactor + FillerSpaceOnSide, 500 * GlobalScaleFactor), Content.Load<Texture2D>("EmptyButton"));
            backButton = new Button(new Vector2(125 * GlobalScaleFactor + FillerSpaceOnSide, 900 * GlobalScaleFactor), Content.Load<Texture2D>("BackButton"));
            restartButton = new Button(new Vector2(-500 * GlobalScaleFactor + FillerSpaceOnSide, 800 * GlobalScaleFactor), Content.Load<Texture2D>("RestartButton"));
            mainMenuButton = new Button(new Vector2(750 * GlobalScaleFactor + FillerSpaceOnSide, 800 * GlobalScaleFactor), Content.Load<Texture2D>("MainMenuButton"));
            levelStartButton = new Button(new Vector2(725 * GlobalScaleFactor + FillerSpaceOnSide, 750 * GlobalScaleFactor), Content.Load<Texture2D>("GoButton"));

            pauseMenu = new Button(new Vector2(500 * GlobalScaleFactor + FillerSpaceOnSide, 250 * GlobalScaleFactor), Content.Load<Texture2D>("PauseMenu"));

            //Sprite initialization
            menuScreen = new Sprite(new Vector2(0 * GlobalScaleFactor, 0 * GlobalScaleFactor), Content.Load<Texture2D>("Screen"), Color.White);
            levelsScreen = new Sprite(new Vector2(-500 * GlobalScaleFactor, 0 * GlobalScaleFactor), Content.Load<Texture2D>("Screen"), Color.White);
            optionsScreen = new Sprite(new Vector2(-500 * GlobalScaleFactor, 0 * GlobalScaleFactor), Content.Load<Texture2D>("Screen"), Color.White);
            inGameOptionsScreen = new Sprite(new Vector2(-500 * GlobalScaleFactor, 0 * GlobalScaleFactor), Content.Load<Texture2D>("Screen"), Color.White);
            title = new Sprite(new Vector2(0 * GlobalScaleFactor, 0 * GlobalScaleFactor), Content.Load<Texture2D>("Title"), Color.White);
            screenChanger = new Sprite(new Vector2(500 * GlobalScaleFactor + 2 * FillerSpaceOnSide, 0 * GlobalScaleFactor), Content.Load<Texture2D>("ScreenChanger"), Color.White);
            leftSideFiller = new Sprite(new Vector2(0, 0), Content.Load<Texture2D>("Pixel"), OuterWallColor);
            rightSideFiller = new Sprite(new Vector2(500 * GlobalScaleFactor + FillerSpaceOnSide, 0), Content.Load<Texture2D>("Pixel"), OuterWallColor);

            //Color Palette Initialization            
            List<Texture2D> colorLists = new List<Texture2D>();
            colorLists.Add(Content.Load<Texture2D>("WallColors"));
            colorLists.Add(Content.Load<Texture2D>("OuterWallColors"));
            colorLists.Add(Content.Load<Texture2D>("BackgroundColors"));
            colorLists.Add(Content.Load<Texture2D>("TextColors"));
            colorLists.Add(Content.Load<Texture2D>("LaserColors"));
            colorLists.Add(Content.Load<Texture2D>("EndGameColors"));
            colorLists.Add(Content.Load<Texture2D>("PauseMenuColors"));
            colorLists.Add(Content.Load<Texture2D>("PlayerColors"));
            for (int i = 0; i < 8; i++)
            {
                colorSelectors.Add(new PaletteSelector(colorLists[i], new Vector2(20, 20), 6, 1, 5, Color.Black, Content.Load<Texture2D>("Pixel"), new Vector2(175 + 30 * i, 370)));
            }

            //Level Selector Initialization
            levelSelector = new ArrowSelector(new Vector2(260, 500), Content.Load<Texture2D>("UpArrow"), Content.Load<Texture2D>("DownArrow"), false, 1, 2, 1, 1, ArrowSelector.Rotation.Vertical);
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
            if (difficulty[randomNumber - 1] > score.TotalSeconds || randomNumber == lastObstacle) //If you don't have enough score for it to load, try again
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
                //Sets the obstacle to be loaded as the last obstacle so the next obstacle is not the same obstacle
                lastObstacle = randomNumber;

                //Load the obstacle
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
                    position.X = x * GlobalScaleFactor * 25 + xOffset + FillerSpaceOnSide;
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
            player.lasers.Clear();
            player.IsClicked = false;
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
                    lastObstacle = 0;
                    screenChanger.Position = new Vector2(500 * GlobalScaleFactor + 2 * FillerSpaceOnSide, 0);
                    screenChanger.Speed = new Vector2(-30, 0);
                }
                else if (screenToSetTo == Screen.GameScreen)
                {
                    slidingSpeed = 32;
                    if (lastScreen != Screen.InGameOptionsMenu)
                    {
                        hasLost = false;
                        score = TimeSpan.Zero;
                        obstacles.Clear();
                        enemies.Clear();
                        destroyedObstacles = 0;
                        enemiesKilled = 0;
                        loadObstacle(500 * GlobalScaleFactor, "UpperStartingObstacle");
                        loadObstacle(1000 * GlobalScaleFactor, string.Format("startingObstacle{0}", random.Next(1, 4)));
                        player.canShoot = true;
                        player = new Player(player.Color, Content.Load<Texture2D>("Ball"), Content.Load<Texture2D>("Spotlight"), true, new Vector2(238 * GlobalScaleFactor + FillerSpaceOnSide, 750 * GlobalScaleFactor));
                        restartButton.Position = new Vector2(-500 * GlobalScaleFactor + FillerSpaceOnSide, 800 * GlobalScaleFactor);
                        mainMenuButton.Position = new Vector2(750 * GlobalScaleFactor + FillerSpaceOnSide, 800 * GlobalScaleFactor);
                    }
                }
                else if (screenToSetTo == Screen.LevelGameScreen)
                {
                    slidingSpeed = 32;
                    if (lastScreen != Screen.InGameOptionsMenu)
                    {
                        hasLost = false;
                        score = TimeSpan.Zero;
                        obstacles.Clear();
                        enemies.Clear();
                        destroyedObstacles = 0;
                        enemiesKilled = 0;
                        loadObstacle(500 * GlobalScaleFactor, "UpperStartingObstacle");
                        loadObstacle(1000 * GlobalScaleFactor, string.Format("startingObstacle{0}", random.Next(1, 4)));
                        player.canShoot = true;
                        player = new Player(player.Color, Content.Load<Texture2D>("Ball"), Content.Load<Texture2D>("Spotlight"), true, new Vector2(238 * GlobalScaleFactor + FillerSpaceOnSide, 750 * GlobalScaleFactor));
                        restartButton.Position = new Vector2(-500 * GlobalScaleFactor + FillerSpaceOnSide, 800 * GlobalScaleFactor);
                        mainMenuButton.Position = new Vector2(750 * GlobalScaleFactor + FillerSpaceOnSide, 800 * GlobalScaleFactor);
                    }
                }
                else if (screenToSetTo == Screen.OptionsMenu)
                {
                    hasLost = false;
                    optionsScreen.Position = new Vector2(528 * GlobalScaleFactor, 0);
                    slidingSpeed = 32;
                }
                else if (screenToSetTo == Screen.LevelsMenu)
                {
                    hasLost = false;
                    optionsScreen.Position = new Vector2(-528 * GlobalScaleFactor, 0);
                    levelsScreen.Position = new Vector2(528 * GlobalScaleFactor, 0);
                    slidingSpeed = 32;
                }
                if (screenToSetTo == Screen.InGameOptionsMenu)
                {
                    inGameOptionsScreen.Position = new Vector2(528 * GlobalScaleFactor, 0);
                    slidingSpeed = 32;
                }
                else
                {
                    pauseMenu.Position = new Vector2(500 * GlobalScaleFactor + FillerSpaceOnSide, 250 * GlobalScaleFactor);
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
            ks = Keyboard.GetState();
            menuScreen.Update();
            optionsScreen.Update();
            if (gameMode == GameMode.Darkmode || gameMode == GameMode.Spotlight)
            {
                leftSideFiller.Color = BackgroundColor;
                rightSideFiller.Color = BackgroundColor;
            }
            else
            {
                leftSideFiller.Color = OuterWallColor;
                rightSideFiller.Color = OuterWallColor;
            }
            if (obstacles.Count == 0) //If there are no obstacles, make some
            {
                if (screen == Screen.GameScreen)
                {
                    enemiesKilled = 0;
                    loadObstacle(500 * GlobalScaleFactor, "UpperStartingObstacle");
                    loadObstacle(1000 * GlobalScaleFactor, string.Format("startingObstacle{0}", random.Next(1, 4)));
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
                levelsScreen.Position.X -= slidingSpeed * GlobalScaleFactor;
                slidingSpeed--;
            }
            player.Update(gameTime);
            if (screen != Screen.InGameOptionsMenu)
            {
                optionsButton.Update(TextColor);
            }
            if (screen == Screen.MainMenu || screen == Screen.OptionsMenu || screen == Screen.LevelsMenu)
            {
                screenChanger.Position += screenChanger.Speed;
                startButton.Update(TextColor);
                levelsButton.Update(TextColor);
                title.Update();
                if (startButton.Clicked)
                {
                    setScreen(Screen.ScreenTransition);
                    NextScreen = Screen.GameScreen;
                }
                if (levelsButton.Clicked)
                {
                    setScreen(Screen.LevelsMenu);
                }
                if (optionsButton.Clicked)
                {
                    setScreen(Screen.OptionsMenu);
                }
                if (obstacles.Count == 0)
                {
                    newObstacle(500 * GlobalScaleFactor);
                    newObstacle(1000 * GlobalScaleFactor);
                }
                startButton.Position = new Vector2(125 * GlobalScaleFactor + FillerSpaceOnSide, 550 * GlobalScaleFactor) - (menuScreen.Position - new Vector2(0, 0));
                levelsButton.Position = menuScreen.Position + new Vector2(125 * GlobalScaleFactor + FillerSpaceOnSide, 675 * GlobalScaleFactor);
                optionsButton.Position = new Vector2(125 * GlobalScaleFactor + FillerSpaceOnSide, 800 * GlobalScaleFactor) - (menuScreen.Position - new Vector2(0, 0));
                title.Position = new Vector2(90 * GlobalScaleFactor + FillerSpaceOnSide, 30 * GlobalScaleFactor - Math.Abs(menuScreen.Position.X));
                title.Color = TextColor;
                foreach (PaletteSelector selector in colorSelectors)
                {
                    selector.Update();
                }
                WallColor = colorSelectors[0].getColor();
                OuterWallColor = colorSelectors[1].getColor();
                BackgroundColor = colorSelectors[2].getColor();
                TextColor = colorSelectors[3].getColor();
                LaserColor = colorSelectors[4].getColor();
                EndGameColor = colorSelectors[5].getColor();
                pauseMenuColor = colorSelectors[6].getColor();
                player.Color = colorSelectors[7].getColor();
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
                        player.lasers.Clear();
                        shootStyle++;
                        shootStyle %= 6;
                        player.canShoot = true;
                        player.Update(gameTime);
                        player.fireLasers(Content.Load<Texture2D>("Laser"), LaserColor, false);
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
                for (int i = 0; i < colorSelectors.Count(); i++)
                {
                    colorSelectors[i].Position = new Vector2(125 * GlobalScaleFactor + FillerSpaceOnSide + 30 * i, 360 * GlobalScaleFactor) - optionsScreen.Position;
                }
                gamemodeButton.Position = new Vector2(125 * GlobalScaleFactor + FillerSpaceOnSide, 540 * GlobalScaleFactor) - (optionsScreen.Position - new Vector2(0, 0));
                obstacleSizeButton.Position = optionsScreen.Position + new Vector2(125 * GlobalScaleFactor + FillerSpaceOnSide, 630 * GlobalScaleFactor);
                shootStyleButton.Position = new Vector2(125 * GlobalScaleFactor + FillerSpaceOnSide, 720 * GlobalScaleFactor) - (optionsScreen.Position - new Vector2(0, 0));

                levelSelector.Update(TextColor);
                levelSelector.Position = new Vector2(270 * GlobalScaleFactor + FillerSpaceOnSide, 500 * GlobalScaleFactor) - (levelsScreen.Position - new Vector2(0, 0));
                level = levelSelector.Selection;
                levelStartButton.Update(TextColor);
                levelStartButton.Position = levelsScreen.Position + new Vector2(225 * GlobalScaleFactor + FillerSpaceOnSide, 700 * GlobalScaleFactor);
                if (levelStartButton.Clicked)
                {
                    setScreen(Screen.ScreenTransition);
                    NextScreen = Screen.LevelGameScreen;
                }

                if (screen == Screen.OptionsMenu)
                {
                    backButton.Position = optionsScreen.Position + new Vector2(125 * GlobalScaleFactor + FillerSpaceOnSide, 900 * GlobalScaleFactor);
                }
                else
                {
                    backButton.Position = levelsScreen.Position + new Vector2(125 * GlobalScaleFactor + FillerSpaceOnSide, 900 * GlobalScaleFactor);
                }
            }
            int highestObstacleY = 10;
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
                    if (obstacle.Hitbox.Intersects(player.Hitbox) && obstacle.SlideSpeed == 0 && !isPaused && !obstacle.Gateway && (screen == Screen.GameScreen || screen == Screen.LevelGameScreen))
                    {
                        isLoading = true;
                        obstacle.didKill = true;
                        obstacle.Color = EndGameColor;
                    }
                    if (obstacle.Position.Y < highestObstacleY)
                    {
                        highestObstacleY = (int)obstacle.Position.Y;
                    }
                    if (obstacle.Breaks)
                    {
                        for (int x = 0; x < player.lasers.Count; x++)
                        {
                            Laser laser = player.lasers[x];
                            if (laser.Hitbox.Intersects(obstacle.Hitbox))
                            {
                                obstacles.Remove(obstacle);
                                destroyedObstacles++;
                                score += new TimeSpan(0, 0, 10);
                                i--;
                                laser._lives--;
                                if (laser._lives <= 0)
                                {
                                    OnTheLine.player.lasers.Remove(laser);
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
                if (highestObstacleY >= 0 && obstacles.Count < 2000)
                {
                    if (screen == Screen.LevelGameScreen)
                    {
                        if (obstaclesLoadedSoFar == level1.Length)
                        {
                            loadObstacle(highestObstacleY, $"ObstacleX");
                        }
                        else
                        {
                            loadObstacle(highestObstacleY, $"Obstacle{level1[obstaclesLoadedSoFar]}");
                            obstaclesLoadedSoFar++;
                        }
                    }
                    else
                    {
                        newObstacle(highestObstacleY);
                    }
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
                    for (int x = 0; x < player.lasers.Count; x++)
                    {
                        Laser laser = player.lasers[x];
                        if (enemy.Hitbox.Intersects(laser.Hitbox))
                        {
                            enemies.Remove(enemy);
                            enemiesKilled++;
                            player.lasers.Remove(laser);
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
                        for (int x = 0; x < OnTheLine.player.lasers.Count; x++)
                        {
                            if (obstacle.Hitbox.Intersects(OnTheLine.player.lasers[x].Hitbox) && obstacle.Breaks)
                            {
                                OnTheLine.player.lasers[x]._lives -= 2;
                                if (OnTheLine.player.lasers[x]._lives <= 0)
                                {
                                    OnTheLine.player.lasers.Remove(OnTheLine.player.lasers[x]);
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
                        if (obstacle.Hitbox.Y < highestObstacleY)
                        {
                            highestObstacleY = obstacle.Hitbox.Y;
                        }
                    }
                    if (highestObstacleY >= 0)
                    {
                        newObstacle(highestObstacleY);
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
            if (ks.IsKeyDown(Keys.Space) && player.canShoot)
            {
                player.fireLasers(Content.Load<Texture2D>("Laser"), LaserColor, false);
                player.laserElapsedTime = TimeSpan.Zero;
                player.canShoot = false;
            }
            lastKs = ks;
            #endregion
            if (screen == Screen.GameScreen || screen == Screen.LevelGameScreen)//gameplay
            {
                screenChanger.Position += screenChanger.Speed;
                pauseMenu.Update(pauseMenuColor, false);
                pauseMenu.Speed.X = 0;
                optionsButton.Position = pauseMenu.Position + new Vector2(40 * GlobalScaleFactor, 50 * GlobalScaleFactor);
                foreach (PaletteSelector selector in colorSelectors)
                {
                    selector.Update();
                }
                restartButton.Update(TextColor);
                if (restartButton.Clicked)
                {
                    if (screen == Screen.GameScreen)
                    {
                        NextScreen = Screen.GameScreen;
                    }
                    else
                    {
                        NextScreen = Screen.LevelGameScreen;
                        obstaclesLoadedSoFar = 0;
                    }
                    setScreen(Screen.ScreenTransition);
                }
                mainMenuButton.Update(TextColor);
                if (mainMenuButton.Clicked)
                {
                    setScreen(Screen.ScreenTransition);
                    NextScreen = Screen.MainMenu;
                }
                if (isPaused)
                {
                    if (pauseMenu.Position.X > 480 * GlobalScaleFactor + 2 * FillerSpaceOnSide)
                    {
                        pauseMenu.Speed.X = -1 * GlobalScaleFactor;
                    }
                    else if (pauseMenu.Hovered && Mouse.GetState().X < 500 + 2 * FillerSpaceOnSide)
                    {
                        if (pauseMenu.Position.X > 170 * GlobalScaleFactor + 2 * FillerSpaceOnSide)
                        {
                            pauseMenu.Speed.X = -15 * GlobalScaleFactor;
                        }
                        else if (optionsButton.Clicked)
                        {
                            setScreen(Screen.InGameOptionsMenu);
                        }
                    }
                    else if (pauseMenu.Position.X < 480 * GlobalScaleFactor + 2 * FillerSpaceOnSide)
                    {
                        pauseMenu.Speed.X = 15 * GlobalScaleFactor;
                    }
                }
                else
                {
                    if (pauseMenu.Position.X < 500 * GlobalScaleFactor + 2 * FillerSpaceOnSide)
                    {
                        pauseMenu.Speed.X = 1 * GlobalScaleFactor;
                    }
                    if (hasLost || isLoading)
                    {
                        if (restartButton.Position.X < FillerSpaceOnSide)
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
                            player.lasers.Clear();
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
                        else if (ms.X > 500 * GlobalScaleFactor + 2 * FillerSpaceOnSide)
                        {
                            Mouse.SetPosition((int)(500 * GlobalScaleFactor), ms.Y);
                        }
                    }
                }
                pauseMenu.Position.X += pauseMenu.Speed.X;
                for (int i = 0; i < colorSelectors.Count(); i++)
                {
                    colorSelectors[i].Position = new Vector2(125 * GlobalScaleFactor + FillerSpaceOnSide + 30 * i, 360 * GlobalScaleFactor) - inGameOptionsScreen.Position;
                }
                obstacleSizeButton.Position = inGameOptionsScreen.Position + new Vector2(125 * GlobalScaleFactor + FillerSpaceOnSide, 550 * GlobalScaleFactor);
                backButton.Position = inGameOptionsScreen.Position + new Vector2(125 * GlobalScaleFactor + FillerSpaceOnSide, 900 * GlobalScaleFactor);
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
                foreach (PaletteSelector selector in colorSelectors)
                {
                    selector.Update();
                }
                WallColor = colorSelectors[0].getColor();
                OuterWallColor = colorSelectors[1].getColor();
                BackgroundColor = colorSelectors[2].getColor();
                TextColor = colorSelectors[3].getColor();
                LaserColor = colorSelectors[4].getColor();
                EndGameColor = colorSelectors[5].getColor();
                pauseMenuColor = colorSelectors[6].getColor();
                player.Color = colorSelectors[7].getColor();
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
                for (int i = 0; i < colorSelectors.Count(); i++)
                {
                    colorSelectors[i].Position = new Vector2(125 * GlobalScaleFactor + FillerSpaceOnSide + 30 * i, 360 * GlobalScaleFactor) - inGameOptionsScreen.Position;
                }
                obstacleSizeButton.Position = inGameOptionsScreen.Position + new Vector2(125 * GlobalScaleFactor + FillerSpaceOnSide, 550 * GlobalScaleFactor);
                backButton.Position = inGameOptionsScreen.Position + new Vector2(125 * GlobalScaleFactor + FillerSpaceOnSide, 900 * GlobalScaleFactor);
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
            leftSideFiller.Draw(generalSpriteBatch, new Vector2(FillerSpaceOnSide, 1000 * GlobalScaleFactor));
            rightSideFiller.Draw(generalSpriteBatch, new Vector2(FillerSpaceOnSide, 1000 * GlobalScaleFactor));
            foreach (Obstacles obstacle in obstacles) //Layer 1 - Regular obstacles
            {
                obstacle.Draw(generalSpriteBatch);
            }
            //colorButton.Draw(generalSpriteBatch);
            obstacleSizeButton.Draw(generalSpriteBatch);
            if (screen == Screen.InGameOptionsMenu || screen == Screen.GameScreen || screen == Screen.LevelGameScreen)
            {
                player.Draw(generalSpriteBatch);
                pauseMenu.Draw(generalSpriteBatch);
                optionsButton.Draw(generalSpriteBatch);
                backButton.Draw(generalSpriteBatch);
            }
            if (screen == Screen.MainMenu || screen == Screen.OptionsMenu || screen == Screen.LevelsMenu)
            {
                player.Draw(generalSpriteBatch);
                startButton.Draw(generalSpriteBatch);
                levelsButton.Draw(generalSpriteBatch);
                optionsButton.Draw(generalSpriteBatch);
                title.Draw(generalSpriteBatch);
                // Screen 2
                gamemodeButton.Draw(generalSpriteBatch);
                shootStyleButton.Draw(generalSpriteBatch);
                generalSpriteBatch.DrawString(statsText, string.Format($"Num of Bullets: {player.stats.Item2}"), shootStyleButton.Position + new Vector2(0, 80 * GlobalScaleFactor), TextColor);
                generalSpriteBatch.DrawString(statsText, string.Format($"Bullet Penetration: {player.stats.Item3}"), shootStyleButton.Position + new Vector2(0, 95 * GlobalScaleFactor), TextColor);
                generalSpriteBatch.DrawString(statsText, string.Format($"Bullet Speed: {player.stats.Item4}"), shootStyleButton.Position + new Vector2(0, 110 * GlobalScaleFactor), TextColor);
                generalSpriteBatch.DrawString(statsText, string.Format($"Reload: {player.stats.Item1.Seconds + (float)player.stats.Item1.Milliseconds / 1000} sec(s)"), shootStyleButton.Position + new Vector2(0, 125 * GlobalScaleFactor), TextColor);
                generalSpriteBatch.DrawString(statsText, string.Format($"Pros: {player.stats.Item5}"), shootStyleButton.Position + new Vector2(0, 140 * GlobalScaleFactor), TextColor);
                generalSpriteBatch.DrawString(statsText, string.Format($"Cons: {player.stats.Item6}"), shootStyleButton.Position + new Vector2(0, 155 * GlobalScaleFactor), TextColor);
                player.Position = new Vector2((int)shootStyleButton.Position.X + shootStyleButton.Texture.Width / 2 * GlobalScaleFactor - Content.Load<Texture2D>("Ball").Width / 2 * GlobalScaleFactor, (int)shootStyleButton.Position.Y + shootStyleButton.Texture.Height / 2 * GlobalScaleFactor - (Content.Load<Texture2D>("Ball").Height / 2 * GlobalScaleFactor));

                generalSpriteBatch.DrawString(endGameFont, "Choose a level", levelsScreen.Position + new Vector2(150 * GlobalScaleFactor + FillerSpaceOnSide, 400 * GlobalScaleFactor), TextColor);
                generalSpriteBatch.DrawString(endGameFont, "Level", levelsScreen.Position + new Vector2(190 * GlobalScaleFactor + FillerSpaceOnSide, 540 * GlobalScaleFactor), TextColor);
                levelSelector.Draw(generalSpriteBatch);
                levelStartButton.Draw(generalSpriteBatch);

                backButton.Draw(generalSpriteBatch);
            }
            foreach (Enemy enemy in enemies)
            {
                if (gameMode != GameMode.Spotlight || (gameMode == GameMode.Spotlight && Math.Abs(enemy.RelativePositon(player.Position).Y) < 400 * GlobalScaleFactor))
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
            if (screen == Screen.GameScreen || screen == Screen.LevelGameScreen)//gameplay
            {
                int laserCount = player.lasers.Count;
                player.Draw(generalSpriteBatch);
                if (player.Counting)
                {
                    generalSpriteBatch.DrawString(infoGameFont, string.Format($"0.{player.CountingCentisecond}"), player.Position + new Vector2(-10 * GlobalScaleFactor, -40 * GlobalScaleFactor), TextColor);
                }
                if (hasLost)
                {
                    generalSpriteBatch.DrawString(endGameFont, $"Score: {(int)score.TotalSeconds}", new Vector2(100 * GlobalScaleFactor + FillerSpaceOnSide, 450 * GlobalScaleFactor), TextColor);
                    generalSpriteBatch.DrawString(endGameFont, $"Obstacles Destroyed: {destroyedObstacles}", new Vector2(100 * GlobalScaleFactor + FillerSpaceOnSide, 500 * GlobalScaleFactor), TextColor);
                    generalSpriteBatch.DrawString(endGameFont, $"Enemies Killed: {enemiesKilled}", new Vector2(100 * GlobalScaleFactor + FillerSpaceOnSide, 550 * GlobalScaleFactor), TextColor);
                }
                else
                {
                    generalSpriteBatch.DrawString(infoGameFont, string.Format($"{obstacles.Count}"), new Vector2(0 + FillerSpaceOnSide, 950 * GlobalScaleFactor), TextColor);
                    generalSpriteBatch.DrawString(infoGameFont, string.Format($"Score: {(int)score.TotalSeconds}"), new Vector2(380 * GlobalScaleFactor + FillerSpaceOnSide, 950 * GlobalScaleFactor), TextColor);
                    generalSpriteBatch.DrawString(infoGameFont, string.Format($"{laserCount}"), new Vector2(240 * GlobalScaleFactor + FillerSpaceOnSide, 950 * GlobalScaleFactor), TextColor);
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
            foreach (PaletteSelector selector in colorSelectors)
            {
                selector.Draw(generalSpriteBatch);
            }
            screenChanger.Draw(generalSpriteBatch);
            menuScreen.Draw(generalSpriteBatch);
            optionsScreen.Draw(generalSpriteBatch);
            inGameOptionsScreen.Draw(generalSpriteBatch);
            generalSpriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}