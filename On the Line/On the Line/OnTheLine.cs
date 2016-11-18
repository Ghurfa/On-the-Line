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

        public readonly int[] difficulty = { 0, 0, 0, 0, 40, 40, 60, 60, 80, 60, 80, 80, 100, 120, 100, 100, 120, 100, 120, 100, 120, 120, 120, 200, 120 };

        Random random = new Random();
        
        public static Color WallColor = Color.Black;
        public static Color OuterWallColor = Color.Black;
        public static Color BackgroundColor = Color.White;
        public static Color TextColor = Color.Red;
        public static Color LaserColor = Color.Red;
        public static Color EndGameColor = Color.Black;
        
        int score = 0;
        int destroyedObstacles;
        int enemiesKilled;
        int highestObstacle;
        int slidingSpeed = 0;
        int obstacleSize = 25;
        public static int screen = (int)Screen.MainMenu;
        public static int shootStyle = 0;

        Sprite menuScreen;
        Sprite optionsScreen;
        Sprite title;

        public static string GameMode = "Regular";
        public static string ColorScheme = "Default";

        Button startButton;
        Button optionsButton;
        Button colorButton;
        Button backButton;
        Button gamemodeButton;
        Button shootStyleButton;
        Checkbox dotModeCheckbox;

        KeyboardState ks;
        KeyboardState lastKs;
        #endregion

        public OnTheLine()
        {
            //Load default values
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 500;
            graphics.PreferredBackBufferHeight = 1000;
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
            startButton = new Button(new Vector2(125, 550), Content.Load<Texture2D>("StartButton"));
            optionsButton = new Button(new Vector2(125, 700), Content.Load<Texture2D>("OptionsButton"));
            colorButton = new Button(new Vector2(125, 100), Content.Load<Texture2D>(string.Format("{0}Button", ColorScheme)));
            gamemodeButton = new Button(new Vector2(125, 300), Content.Load<Texture2D>(string.Format("{0}Button", GameMode)));
            shootStyleButton = new Button(new Vector2(125, 500), Content.Load<Texture2D>("EmptyButton"));
            backButton = new Button(new Vector2(125, 900), Content.Load<Texture2D>("BackButton"));
            dotModeCheckbox = new Checkbox(new Vector2(400, 315), Content.Load<Texture2D>("Checkbox_On"), Content.Load<Texture2D>("Checkbox_Off"), false);

            //Sprite initialization
            menuScreen = new Sprite(new Vector2(0, 0), Content.Load<Texture2D>("Screen"), Color.White);
            optionsScreen = new Sprite(new Vector2(-500, 0), Content.Load<Texture2D>("Screen"), Color.White);
            title = new Sprite(new Vector2(0, 0), Content.Load<Texture2D>("Title"), Color.White);
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
            int randomNumber = random.Next(1, 26); //Chooses a random obstacle from all 25
            if (difficulty[randomNumber - 1] > score / 60) //If you don't have enough score for it to load
            {
                newObstacle(yOffset);
            }
            else
            {
                if (randomNumber == 8 || randomNumber == 15) //If the obstacle chosen is a longer obstacle
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
                    position.X = x * 25 + xOffset;
                    position.Y = y * 25 - 500 + yOffset;

                    if (currentPixel == Color.Red)
                    {
                        if (GameMode == "Darkmode")
                        {
                            obstacles.Add(new Obstacles(obstaclePixel, position, obstacleSize, BackgroundColor, false, 0, 0, 0, false, false));
                        }
                        else
                        {
                            obstacles.Add(new Obstacles(obstaclePixel, position, obstacleSize, OuterWallColor, false, 0, 0, 0, false, false, true));
                        }
                    }
                    else if (currentPixel == Color.Green)
                    {
                        obstacles.Add(new Obstacles(obstaclePixel, position, obstacleSize, new Color(TextColor.R, TextColor.G, TextColor.B, 230), false, 0, 0, 0, false, false));
                    }
                    else if (currentPixel == Color.Purple)
                    {
                        obstacles.Add(new Obstacles(obstaclePixel, position, obstacleSize, WallColor, true, 0, 0, 0, false, false));
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
                        obstacles.Add(new Obstacles(obstaclePixel, position, obstacleSize, WallColor, false, 0, 0, 0, true, false));
                    }
                    else if (currentPixel == Color.Blue)
                    {
                        obstacles.Add(new Obstacles(obstaclePixel, position, obstacleSize, BackgroundColor, false, 0, 0, 0, false, true));
                    }
                    else if (currentPixel.R == 254)
                    {
                        enemies.Add(new Enemy(position, ball, empty, laserTexture, currentPixel.G, currentPixel.B, false, false));
                    }
                    else if (currentPixel != Color.White)
                    {
                        if (GameMode == "Darkmode")
                        {
                            obstacles.Add(new Obstacles(obstaclePixel, position, obstacleSize, BackgroundColor, false, currentPixel.R - 100, currentPixel.G - 100, currentPixel.B, false, false));
                        }
                        else
                        {
                            obstacles.Add(new Obstacles(obstaclePixel, position, obstacleSize, WallColor, false, currentPixel.R - 100, currentPixel.G - 100, currentPixel.B, false, false));
                        }
                    }
                }
            }
        }
        public void setScreen(int screenToSetTo)
        {
            hasLost = false;
            mouseHitbox.lasers.Clear();
            isPaused = false;
            if (screenToSetTo == (int)Screen.MainMenu) //If you are switching to main menu
            {
                menuScreen.Position = new Vector2(528, 0);
                slidingSpeed = 32;
            }
            else if (screenToSetTo == (int)Screen.GameScreen)
            {
                score = 0;
                obstacles.Clear();
                enemies.Clear();
                destroyedObstacles = 0;
                enemiesKilled = 0;
                loadObstacle(1000, "LowerStartingObstacle");
                loadObstacle(500, string.Format("startingObstacle{0}", random.Next(1, 4)));
                mouseHitbox.canShoot = true;
                mouseHitbox = new MouseHitbox(mouseHitbox.Color, Content.Load<Texture2D>("Ball"), Content.Load<Texture2D>("Spotlight"), true, new Vector2(238, 250));
            }
            else if (screenToSetTo == (int)Screen.OptionsMenu)
            {
                optionsScreen.Position = new Vector2(528, 0);
                slidingSpeed = 32;
            }
            if (screen == (int)Screen.GameScreen) //If you are on GameScreen before you switch
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
            menuScreen.Update();
            optionsScreen.Update();
            if (obstacles.Count == 0) //If there are no obstacles, make some
            {
                newObstacle(500);
                newObstacle(1000);
            }
            if (slidingSpeed != 0) //If the screen sprites are supposed to move, move them & decrease the speed they slide
            {
                menuScreen.Position.X -= slidingSpeed;
                optionsScreen.Position.X -= slidingSpeed;
                slidingSpeed--;
            }
            mouseHitbox.Update(gameTime);
            int highestObstacle = 10;
            if (screen == (int)Screen.MainMenu || screen == (int)Screen.OptionsMenu)
            {
                //Screen 0
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
                if (obstacles.Count == 0)
                {
                    newObstacle(500);
                    newObstacle(1000);
                }
                startButton.Position = menuScreen.Position + new Vector2(125, 550);
                optionsButton.Position = menuScreen.Position + new Vector2(125, 700);
                title.Position = menuScreen.Position + new Vector2(0, 50);
                title.Color = TextColor;
                //Screen 2
                colorButton.Update();
                gamemodeButton.Update();
                shootStyleButton.Update();
                dotModeCheckbox.Update();
                backButton.Update();
                #region Checks Color Button
                if (colorButton.Clicked)
                {
                    EndGameColor = Color.White;
                    if (ColorScheme == "Default")
                    {
                        ColorScheme = "Ice";
                        mouseHitbox.Color = new Color(255, 150, 0);
                        TextColor = new Color(255, 150, 0);
                        LaserColor = new Color(255, 150, 0);
                        WallColor = new Color(30, 250, 230);
                        OuterWallColor = new Color(30, 250, 230);
                        BackgroundColor = new Color(13, 13, 13);
                    }
                    else if (ColorScheme == "Ice")
                    {
                        ColorScheme = "Beach";
                        mouseHitbox.Color = new Color(45, 105, 174);
                        TextColor = new Color(0, 183, 45);
                        LaserColor = new Color(45, 105, 174);
                        WallColor = new Color(45, 105, 174);
                        OuterWallColor = new Color(30, 44, 96);
                        BackgroundColor = new Color(240, 210, 150);
                    }
                    else if (ColorScheme == "Beach")
                    {
                        ColorScheme = "Gingerbread";
                        mouseHitbox.Color = new Color(50, 20, 0);
                        TextColor = new Color(50, 20, 0);
                        LaserColor = new Color(50, 20, 0);
                        WallColor = Color.White;
                        OuterWallColor = new Color(40, 10, 0);
                        BackgroundColor = new Color(80, 50, 20);
                    }
                    else if (ColorScheme == "Gingerbread")
                    {
                        ColorScheme = "School";
                        mouseHitbox.Color = Color.Black;
                        TextColor = Color.Black;
                        LaserColor = Color.Black;
                        WallColor = new Color(10, 10, 10);
                        OuterWallColor = new Color(10, 10, 10);
                        BackgroundColor = new Color(20, 20, 20);
                    }
                    else if (ColorScheme == "School")
                    {
                        ColorScheme = "Default";
                        mouseHitbox.Color = Color.LightGray;
                        TextColor = Color.Red;
                        LaserColor = Color.Red;
                        if (GameMode == "Darkmode" || GameMode == "Spotlight")
                        {
                            WallColor = Color.White;
                            OuterWallColor = Color.White;
                            BackgroundColor = Color.Black;
                        }
                        else
                        {
                            WallColor = Color.Black;
                            OuterWallColor = Color.Black;
                            BackgroundColor = Color.White;
                        }
                        EndGameColor = WallColor;
                    }
                    else if (GameMode == "Darkmode" || GameMode == "Spotlight")
                    {
                        WallColor = OuterWallColor;
                    }
                    colorButton.Texture = Content.Load<Texture2D>(string.Format("{0}Button", ColorScheme));
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
                        if (ColorScheme == "Default")
                        {
                            WallColor = Color.White;
                            OuterWallColor = Color.White;
                            BackgroundColor = Color.Black;
                        }
                        else
                        {
                            WallColor = OuterWallColor;
                        }
                    }
                    else if (GameMode == "Darkmode")
                    {
                        gamemodeButton.Texture = Content.Load<Texture2D>("SpotlightButton");
                        GameMode = "Spotlight";
                        if (ColorScheme == "Default")
                        {
                            WallColor = Color.White;
                            OuterWallColor = Color.White;
                            BackgroundColor = Color.Black;
                        }
                        else
                        {
                            WallColor = OuterWallColor;
                        }
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

                    mouseHitbox.fireLasers(Content.Load<Texture2D>("Laser"), LaserColor, false);

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
                colorButton.Position = optionsScreen.Position + new Vector2(125, 100);
                gamemodeButton.Position = optionsScreen.Position + new Vector2(125, 300);
                shootStyleButton.Position = optionsScreen.Position + new Vector2(125, 500);
                dotModeCheckbox.checkBox.Position = optionsScreen.Position + new Vector2(400, 315);
                backButton.Position = optionsScreen.Position + new Vector2(125, 900);
            }
            if (!hasLost && !isLoading)
            {
                #region Updates Obstacles
                for (int i = 0; i < obstacles.Count; i++)
                {
                    Obstacles obstacle = obstacles[i];
                    obstacle.Update();
                    obstacle._size = new Vector2(obstacleSize, obstacleSize);
                    if (GameMode == "Fastmode")
                    {
                        obstacle.Update();
                    }
                    if (obstacle.hitbox.Intersects(mouseHitbox._hitbox) && obstacle._slideSpeed == 0 && !isPaused && !obstacle._gateway && screen == (int)Screen.GameScreen)
                    {
                        isLoading = true;
                        obstacle.didKill = true;
                        obstacle._color = EndGameColor;
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
                                    OnTheLine.mouseHitbox.lasers.Remove(laser);
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
                if (!isPaused)
                {
                    score++;
                    if (GameMode == "Fastmode")
                    {
                        score++;
                    }
                }
            }
            #region Check Keyboard
            ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.M) && !lastKs.IsKeyDown(Keys.M) && screen != (int)Screen.MainMenu)
            {
                setScreen(0);
            }
            if (ks.IsKeyDown(Keys.R) && !lastKs.IsKeyDown(Keys.R) && screen == (int)Screen.GameScreen)
            {
                setScreen(1);
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
                            if (obstacle.hitbox.Intersects(OnTheLine.mouseHitbox.lasers[x]._rect) && obstacle._breaks)
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
            }
            if (ks.IsKeyDown(Keys.Space) && mouseHitbox.canShoot)
            {
                mouseHitbox.fireLasers(Content.Load<Texture2D>("Laser"), LaserColor, false);
                mouseHitbox.laserElapsedTime = TimeSpan.Zero;
                mouseHitbox.canShoot = false;
            }
            lastKs = ks;
            #endregion
            #region Screen 1 Gameplay
            if (screen == (int)Screen.GameScreen)//gameplay
            {
                if (hasLost || isLoading)
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
                        hasLost = true;
                        loadObstacle(525, "YouLose");
                        isLoading = false;
                        mouseHitbox.lasers.Clear();
                    }
                }
                else if (!hasLost && !isPaused)
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
            if (screen != 3)
            {
                foreach (Obstacles obtsacle in obstacles) //Layer 0 - Regular obstacles
                {
                    obtsacle.Draw(generalSpriteBatch);
                }
                foreach (Obstacles obtstacle in obstacles) //Layer 1 - Obstacle that killed you
                {
                    if (obtstacle.didKill)
                    {
                        obtstacle.Draw(generalSpriteBatch);
                    }
                }
                foreach (Obstacles obtstacle in obstacles) //Layer 2 - "You Lose"
                {
                    if (obtstacle._color.A == 230)
                    {
                        obtstacle.Draw(generalSpriteBatch);
                    }
                }
                foreach (Enemy enemy in enemies)
                {
                    enemy.Draw(generalSpriteBatch);
                }
            }
            if (screen == (int)Screen.MainMenu || screen == (int)Screen.OptionsMenu)
            {
                startButton.Draw(generalSpriteBatch);
                optionsButton.Draw(generalSpriteBatch);
                title.Draw(generalSpriteBatch);
                // Screen 2
                colorButton.Draw(generalSpriteBatch);
                gamemodeButton.Draw(generalSpriteBatch);
                shootStyleButton.Draw(generalSpriteBatch);
                mouseHitbox.Position = new Vector2((int)shootStyleButton.Position.X + shootStyleButton.Texture.Width / 2 - Content.Load<Texture2D>("Ball").Width / 2, (int)shootStyleButton.Position.Y + shootStyleButton.Texture.Height / 2 - Content.Load<Texture2D>("Ball").Height / 2 + 10);
                backButton.Draw(generalSpriteBatch);
                mouseHitbox.Draw(generalSpriteBatch);
                int s = (int)shootStyleButton.Position.Y; //make the line look less intimidating
                generalSpriteBatch.DrawString(statsText, string.Format($"Num of Bullets: {mouseHitbox.stats.Item2}"), optionsScreen.Position + new Vector2(125, s + 80), TextColor);
                generalSpriteBatch.DrawString(statsText, string.Format($"Bullet Penetration: { mouseHitbox.stats.Item3}"), optionsScreen.Position + new Vector2(125, s + 95), TextColor);
                generalSpriteBatch.DrawString(statsText, string.Format($"Bullet Speed: {mouseHitbox.stats.Item4}"), optionsScreen.Position + new Vector2(125, s + 110), TextColor);
                generalSpriteBatch.DrawString(statsText, string.Format($"Reload: {mouseHitbox.stats.Item1.Seconds + (float)mouseHitbox.stats.Item1.Milliseconds / 1000} sec(s)"), optionsScreen.Position + new Vector2(125, s + 125), TextColor);
                generalSpriteBatch.DrawString(statsText, string.Format($"Pros: {mouseHitbox.stats.Item5}"), optionsScreen.Position + new Vector2(125, s + 140), TextColor);
                generalSpriteBatch.DrawString(statsText, string.Format($"Cons: {mouseHitbox.stats.Item6}"), optionsScreen.Position + new Vector2(125, s + 155), TextColor);
                KeyboardState ks = Keyboard.GetState();
                dotModeCheckbox.Draw(generalSpriteBatch);
            }
            if (screen == (int)Screen.GameScreen)//gameplay
            {
                int laserCount = mouseHitbox.lasers.Count;
                mouseHitbox.Draw(generalSpriteBatch);
                if (mouseHitbox.Counting)
                {
                    generalSpriteBatch.DrawString(infoGameFont, string.Format($"0.{mouseHitbox.CountingCentisecond}"), mouseHitbox.Position + new Vector2(-10, -40), TextColor);
                }
                if (hasLost)
                {
                    generalSpriteBatch.DrawString(endGameFont, $"Score:{score / 50}", new Vector2(100, 450), TextColor);
                    generalSpriteBatch.DrawString(endGameFont, $"Obstacles Destroyed:{destroyedObstacles}", new Vector2(100, 500), TextColor);
                    generalSpriteBatch.DrawString(endGameFont, $"Enemies Killed:{enemiesKilled}", new Vector2(100, 550), TextColor);
                    generalSpriteBatch.DrawString(endGameFont2, "Verdict:", new Vector2(160, 650), TextColor);
                    if (score < 5000)
                    {
                        generalSpriteBatch.DrawString(endGameFont2, "You're a noob", new Vector2(100, 700), TextColor);
                    }
                    else
                    {
                        generalSpriteBatch.DrawString(endGameFont2, "You're a pro", new Vector2(110, 700), TextColor);
                    }
                    generalSpriteBatch.DrawString(endGameFont2, "Press R to Restart", new Vector2(30, 800), TextColor);
                    generalSpriteBatch.DrawString(endGameFont2, "Press M to go to Menu", new Vector2(0, 850), TextColor);
                }
                else
                {
                    generalSpriteBatch.DrawString(infoGameFont, string.Format("{0}", obstacles.Count), new Vector2(0, 950), TextColor);
                    generalSpriteBatch.DrawString(infoGameFont, string.Format("Score: {0}", score / 60), new Vector2(380, 950), TextColor);
                    generalSpriteBatch.DrawString(infoGameFont, string.Format("{0}", laserCount), new Vector2(240, 950), TextColor);
                }
            }
            menuScreen.Draw(generalSpriteBatch);
            optionsScreen.Draw(generalSpriteBatch);
            generalSpriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}