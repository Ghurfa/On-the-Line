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

        public static TimeSpan laserCooldown;
        TimeSpan laserElapsedTime;

        SpriteFont font;
        SpriteFont smallText;
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
        public static int screen = 0;
        bool canShootLaser = true;
        bool shootingLaser = false;
        public static int shootStyle = 0;
        //public static bool darkmode = true;
        public static string gamemode = "regular";

        Button startButton;
        Button optionsButton;
        Button colorButton;
        Button backButton;
        Button gamemodeButton;
        public static Button shootStyleButton;

        public static bool isLoading = false;
        int highestObstacle = -500;

        public static string colorScheme = "Default";

        List<int> levels = new List<int>();
        public static List<Enemy> enemies = new List<Enemy>();
        KeyboardState lastKs;
        public int[] difficulty = { 0, 0, 0, 0, 20, 20, 30, 30, 40, 30, 40, 40, 50, 60, 50, 50, 60, 50, 60, 50, 60, 60, 60, 100, 60 };
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
            smallText = Content.Load<SpriteFont>("SmallText");
            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData<Color>(new Color[] { Color.White });
            startButton = new Button(125, 250, Content.Load<Texture2D>("StartButton"));
            optionsButton = new Button(125, 400, Content.Load<Texture2D>("OptionsButton"));
            mouseHitbox = new MouseHitbox(ballColor, Content.Load<Texture2D>("Ball"), Content.Load<Texture2D>("Spotlight"), true);

            colorButton = new Button(125, 100, Content.Load<Texture2D>(string.Format("{0}Button", colorScheme)));
            gamemodeButton = new Button(125, 300, Content.Load<Texture2D>(string.Format("{0}Button", gamemode)));
            shootStyleButton = new Button(125, 500, Content.Load<Texture2D>("EmptyButton"));

            backButton = new Button(125, 900, Content.Load<Texture2D>("BackButton"));

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
            lose = false;
            canShootLaser = true;
            score = 0;
            obstacles.Clear();
            enemies.Clear();
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
                            obstacles.Add(new Obstacles(pixel, new Vector2(x * 25 + xOffset, (y * 25) - 500 + yOffset), new Vector2(25, 25), backgroundColor, false, 0, 0, 0, false, false, 31)); //Outside Background
                        }
                        else
                        {
                            obstacles.Add(new Obstacles(pixel, new Vector2(x * 25 + xOffset, (y * 25) - 500 + yOffset), new Vector2(25, 25), outerWallColor, false, 0, 0, 0, false, false, 31)); //Outside Background
                        }
                    }
                    else if (currentPixel == Color.Green)
                    {
                        obstacles.Add(new Obstacles(pixel, new Vector2(x * 25 + xOffset, (y * 25) - 500 + yOffset), new Vector2(25, 25), textColor, false, 0, 0, 0, false, false, 31));
                    }
                    else if (currentPixel == Color.Purple)
                    {
                        obstacles.Add(new Obstacles(pixel, new Vector2(x * 25 + xOffset, (y * 25) - 500 + yOffset), new Vector2(25, 25), wallColor, true, 0, 0, 0, false, false, 31));
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
                        obstacles.Add(new Obstacles(pixel, new Vector2(x * 25 + xOffset, (y * 25) - 500 + yOffset), new Vector2(25, 25), wallColor, false, 0, 0, 0, true, false, 31));
                    }
                    else if (currentPixel == Color.Blue)
                    {
                        obstacles.Add(new Obstacles(pixel, new Vector2(x * 25 + xOffset, (y * 25) - 500 + yOffset), new Vector2(25, 25), backgroundColor, false, 0, 0, 0, false, true, 31));
                    }
                    else if (currentPixel.R == 254)
                    {
                        enemies.Add(new Enemy(new Vector2(x * 25 + xOffset, (y * 25) - 500 + yOffset), Content.Load<Texture2D>("Ball"), Content.Load<Texture2D>("Empty"), Content.Load<Texture2D>("Laser"), currentPixel.G, currentPixel.B, false, false));
                    }
                    else if (currentPixel != Color.White)
                    {
                        if (gamemode == "darkmode")
                        {
                            obstacles.Add(new Obstacles(pixel, new Vector2(x * 25 + xOffset, (y * 25) - 500 + yOffset), new Vector2(25, 25), backgroundColor, false, currentPixel.R - 100, currentPixel.G - 100, currentPixel.B, false, false, 31));
                        }
                        else
                        {
                            obstacles.Add(new Obstacles(pixel, new Vector2(x * 25 + xOffset, (y * 25) - 500 + yOffset), new Vector2(25, 25), wallColor, false, currentPixel.R - 100, currentPixel.G - 100, currentPixel.B, false, false, 31));
                        }
                    }
                }
            }
        }

        Color reverseColor(Color color)
        {
            return new Color(255 - color.R, 255 - color.G, 255 - color.B);
        }
        public void keyboardStuff()
        {
            KeyboardState ks = Keyboard.GetState();
            if (screen == 1 || screen == 2)
            {
                if (screen == 1)
                {
                    if (ks.IsKeyDown(Keys.R) && !lastKs.IsKeyDown(Keys.R))
                    {
                        obstacles.Clear();
                        enemies.Clear();
                        startNewGame();
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
                    if (ks.IsKeyDown(Keys.Space) && canShootLaser)
                    {
                        shootingLaser = true;
                        laserElapsedTime = TimeSpan.Zero;
                    }
                    if (shootingLaser)
                    {
                        int times = 0;
                        //do
                        //{
                        mouseHitbox.fireLasers(Content.Load<Texture2D>("Laser"), laserColor, false);
                        if (mouseHitbox.reloadCycle == 0 && mouseHitbox.slow == 0)
                        {
                            times++;
                        }
                        //} while (times != 2);
                        if (times == 1)
                        {
                            canShootLaser = false;
                            shootingLaser = false;
                        }
                    }
                }
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
                laserElapsedTime += gameTime.ElapsedGameTime;
                foreach (Enemy enemy in enemies)
                {
                    enemy.laserElapsedTime += gameTime.ElapsedGameTime;
                }
            }
            if (laserElapsedTime >= laserCooldown)
            {
                laserElapsedTime = TimeSpan.Zero;
                canShootLaser = true;
            }
            checkColorScheme();
            if (screen == 0)//main menu
            {
                startButton.Update();
                optionsButton.Update();
                if (startButton.clicked)
                {
                    screen = 1;
                    startNewGame();
                }
                if (optionsButton.clicked)
                {
                    screen = 2;
                }
            }
            else if (screen == 1)//gameplay
            {
                KeyboardState ks = Keyboard.GetState();

                mouseHitbox.Update();
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
                        loadObstacle(525, "Loading");
                        isLoading = false;
                        mouseHitbox.lasers.Clear();
                    }
                    else
                    {
                        if (obstacles.Count > 1)
                        {
                            if (!obstacles[0].didKill)
                            {
                                obstacles.RemoveAt(0);
                            }
                            else
                            {
                                obstacles.RemoveAt(1);
                            }
                        }
                        else
                        {
                            enemies.Clear();
                            lose = false;
                            startNewGame();
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
                    highestObstacle = 10;
                    for (int i = 0; i < obstacles.Count; i++)
                    {
                        Obstacles obstacle = obstacles[i];
                        obstacle.Update();
                        if (gamemode == "fastmode")
                        {
                            obstacle.Update();
                        }
                        if (obstacle.hitbox.Intersects(mouseHitbox._hitbox) && obstacle._slideSpeed == 0 && !pause && !obstacle._gateway)
                        {
                            isLoading = true;
                            obstacle.didKill = true;
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
                            }
                        }
                        for (int x = 0; x < mouseHitbox.lasers.Count; x++)
                        {
                            Laser laser = mouseHitbox.lasers[x];
                            if (enemy.body._hitbox.Intersects(laser._rect))
                            {
                                enemies.Remove(enemy);
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
                                        enemy.body.lasers.Remove(laser);
                                    }
                                }
                            }
                        }
                    }
                }
                keyboardStuff();
            }
            else if (screen == 2)//settings
            {
                mouseHitbox.Update();
                colorButton.Update();
                if (colorButton.clicked)
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
                gamemodeButton.Update();
                if (gamemodeButton.clicked)
                {
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
                shootStyleButton.Update();
                if (shootStyleButton.clicked)
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
                    canShootLaser = true;
                    mouseHitbox.Update();
                    int times = 0;
                    do
                    {
                        mouseHitbox.fireLasers(Content.Load<Texture2D>("Laser"), laserColor, false);
                        if (mouseHitbox.reloadCycle != 0 && mouseHitbox.slow == 0)
                        {
                            times++;
                        }
                    } while (times != 2);
                }
                backButton.Update();
                if (backButton.clicked)
                {
                    screen = 0;
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
            GraphicsDevice.Clear(backgroundColor);
            spriteBatch.Begin();
            if (screen == 0)//main menu
            {
                startButton.Draw(spriteBatch);
                optionsButton.Draw(spriteBatch);
            }
            else if (screen == 1)//gameplay
            {
                int laserCount = mouseHitbox.lasers.Count;
                foreach (Enemy enemy in enemies)
                {
                    enemy.Draw(spriteBatch);
                    laserCount += enemy.body.lasers.Count();
                }
                for (int i = 0; i < obstacles.Count; i++)
                {
                    obstacles[i].Draw(spriteBatch);
                }
                mouseHitbox.Draw(spriteBatch);
                spriteBatch.DrawString(font, string.Format("{0}", obstacles.Count), new Vector2(0, 950), textColor);
                spriteBatch.DrawString(font, string.Format("Score: {0}", score / 50), new Vector2(380, 950), textColor);
                spriteBatch.DrawString(font, string.Format("{0}", laserCount), new Vector2(240, 950), textColor);
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
                spriteBatch.DrawString(smallText, string.Format("Num of Bullets: {0}", mouseHitbox.numOfBullets), new Vector2(125, 580), textColor);
                spriteBatch.DrawString(smallText, string.Format("Bullet Penetration: {0}", mouseHitbox.BulletPen), new Vector2(125, 595), textColor);
                spriteBatch.DrawString(smallText, string.Format("Bullet Speed: {0}", mouseHitbox.bulletSpeed), new Vector2(125, 610), textColor);
                spriteBatch.DrawString(smallText, string.Format("Reload: {0} sec(s)", laserCooldown.Seconds + (float)laserCooldown.Milliseconds / 1000f), new Vector2(125, 625), textColor);
                spriteBatch.DrawString(smallText, string.Format("Pros: {0}", mouseHitbox.pros), new Vector2(125, 640), textColor);
                spriteBatch.DrawString(smallText, string.Format("Cons: {0}", mouseHitbox.cons), new Vector2(125, 655), textColor);
            }
            spriteBatch.End();
            frames++;
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}