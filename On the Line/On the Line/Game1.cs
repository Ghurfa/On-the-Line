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
        public static bool lose = false;
        public static bool pause = false;
        List<Obstacles> obstacles = new List<Obstacles>();
        Random random = new Random();
        public static Color wallColor;
        public static Color outsideColor;
        public static Color insideColor;
        Color ballColor;
        public static Color textColor;
        public static Color laserColor;
        int score = 0;
        int fps;
        int frames;
        public static int screen = 0;
        bool canShootLaser = true;
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
            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData<Color>(new Color[] { Color.White });
            startButton = new Button(125, 250, Content.Load<Texture2D>("StartButton"));
            optionsButton = new Button(125, 400, Content.Load<Texture2D>("OptionsButton"));
            mouseHitbox = new MouseHitbox(ballColor, Content.Load<Texture2D>("Ball"), Content.Load<Texture2D>("Spotlight"));

            colorButton = new Button(125, 100, Content.Load<Texture2D>(string.Format("{0}Button", colorScheme)));
            gamemodeButton = new Button(125, 300, Content.Load<Texture2D>(string.Format("{0}Button", gamemode)));
            shootStyleButton = new Button(125, 500, Content.Load<Texture2D>("EmptyButton"));

            backButton = new Button(125, 700, Content.Load<Texture2D>("BackButton"));
            //obstacles.Add(new Obstacles(Content.Load<Texture2D>("Obstacle1"), new Vector2(0, 0), Color.White));
            //obstacles.Add(new Obstacles(Content.Load<Texture2D>("Obstacle1"), new Vector2(0, -500), Color.White));
            //randomTexture(obstacles[1]);
            //randomTexture(obstacles[2]);

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
            loadObstacle(1000, "blankObstacle");
            loadObstacle(500, string.Format("startingObstacle{0}", random.Next(1, 5)));
            mouseHitbox = new MouseHitbox(ballColor, Content.Load<Texture2D>("Ball"), Content.Load<Texture2D>("Spotlight"));
            mouseHitbox._position = new Vector2(238, 250);
        }
        /// <summary>
        /// Chooses a random obstacle to load
        /// </summary>
        /// <param name="yOffset"></param>
        void newObstacle(float yOffset)
        {
            int randomNumber = random.Next(19, 20);
            if (randomNumber == 8 || randomNumber == 15)
            {
                yOffset -= 500;
            }
            loadObstacle(yOffset, string.Format("Obstacle{0}", randomNumber));
        }
        /// <summary>
        /// Loads an obstacle
        /// </summary>
        /// <param name="yOffset"></param>
        /// <param name="obstacleName">The name of the obstacle to load</param>
        void loadObstacle(float yOffset, string obstacleName)
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
                            obstacles.Add(new Obstacles(pixel, new Vector2(x * 25, (y * 25) - 500 + yOffset), new Vector2(25, 25), insideColor, true, false, 0, 0, 0)); //Outside Background
                        }
                        else
                        {
                            obstacles.Add(new Obstacles(pixel, new Vector2(x * 25, (y * 25) - 500 + yOffset), new Vector2(25, 25), outsideColor, true, false, 0, 0, 0)); //Outside Background
                        }
                    }
                    else if (currentPixel == Color.Green)
                    {
                        obstacles.Add(new Obstacles(pixel, new Vector2(x * 25, (y * 25) - 500 + yOffset), new Vector2(25, 25), textColor, true, false, 0, 0, 0));
                    }
                    else if (currentPixel == Color.Purple)
                    {
                        obstacles.Add(new Obstacles(pixel, new Vector2(x * 25, (y * 25) - 500 + yOffset), new Vector2(25, 25), wallColor, true, true, 0, 0, 0));
                    }
                    else if (currentPixel == Color.Orange)
                    {
                        enemies.Add(new Enemy(new Vector2(x * 25, (y * 25) - 500 + yOffset), Content.Load<Texture2D>("Ball"), Content.Load<Texture2D>("Spotlight"), Content.Load<Texture2D>("Laser"), 0, 0, true));
                    }
                    else if (currentPixel.R == 254)
                    {
                        enemies.Add(new Enemy(new Vector2(x * 25, (y * 25) - 500 + yOffset), Content.Load<Texture2D>("Ball"), Content.Load<Texture2D>("Spotlight"), Content.Load<Texture2D>("Laser"), currentPixel.G, currentPixel.B, false));
                    }
                    else if (currentPixel != Color.White)
                    {
                        if (gamemode == "darkmode")
                        {
                            obstacles.Add(new Obstacles(pixel, new Vector2(x * 25, (y * 25) - 500 + yOffset), new Vector2(25, 25), insideColor, true, false, currentPixel.R - 100, currentPixel.G - 100, currentPixel.B));
                        }
                        else
                        {
                            obstacles.Add(new Obstacles(pixel, new Vector2(x * 25, (y * 25) - 500 + yOffset), new Vector2(25, 25), wallColor, true, false, currentPixel.R - 100, currentPixel.G - 100, currentPixel.B));
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
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
                        int highestObstacle = 10;
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
                        if (highestObstacle >= 0)
                        {
                            newObstacle(highestObstacle);
                        }
                        for (int i = 0; i < enemies.Count; i++)
                        {
                            Enemy enemy = enemies[i];
                            enemy.Update();
                            if (ks.IsKeyDown(Keys.RightControl) || ks.IsKeyDown(Keys.LeftControl))
                            {
                                enemy.Update();
                            }
                        }
                    }
                }

                if (ks.IsKeyDown(Keys.Space) && canShootLaser)
                {
                    canShootLaser = false;
                    mouseHitbox.fireLasers(Content.Load<Texture2D>("Laser"), laserColor, false);
                    laserElapsedTime = TimeSpan.Zero;
                }
            }
            lastKs = ks;
        }
        /// <summary>
        /// This checks the color scheme and chooses the color
        /// </summary>
        void checkColorScheme()
        {
            if (colorScheme == "Default")
            {
                ballColor = Color.LightGray;
                textColor = Color.DarkGreen;
                laserColor = Color.Red;
                if (gamemode == "darkmode" || gamemode == "spotlight")
                {
                    wallColor = Color.White;
                    outsideColor = Color.White;
                    insideColor = Color.Black;
                }
                else
                {
                    wallColor = Color.Black;
                    outsideColor = Color.Black;
                    insideColor = Color.White;
                }
            }
            else if (colorScheme == "Ice")
            {
                ballColor = new Color(255, 150, 0);
                textColor = new Color(255, 150, 0);
                laserColor = new Color(255, 150, 0);
                if (gamemode == "darkmode" || gamemode == "spotlight")
                {
                    wallColor = new Color(0, 240, 220);
                    outsideColor = new Color(0, 240, 220);
                    insideColor = new Color(13, 13, 13);
                }
                else
                {
                    wallColor = new Color(0, 240, 220);
                    outsideColor = new Color(0, 240, 220);
                    insideColor = new Color(13, 13, 13);
                }
            }
            else if (colorScheme == "Beach")
            {
                ballColor = new Color(45, 105, 174);
                textColor = new Color(45, 105, 174);
                laserColor = new Color(45, 105, 174);
                if (gamemode == "darkmode" || gamemode == "spotlight")
                {
                    wallColor = new Color(30, 44, 96);
                    outsideColor = new Color(30, 44, 96);
                    insideColor = new Color(255, 183, 45);
                }
                else
                {
                    wallColor = new Color(45, 105, 174);
                    outsideColor = new Color(30, 44, 96);
                    insideColor = new Color(255, 183, 45);
                }
            }
            else if (colorScheme == "Chocolate")
            {
                ballColor = new Color(50, 20, 0);
                textColor = new Color(50, 20, 0);
                laserColor = new Color(50, 20, 0);
                if (gamemode == "darkmode" || gamemode == "spotlight")
                {
                    wallColor = new Color(40, 10, 0);
                    outsideColor = new Color(40, 10, 0);
                    insideColor = new Color(80, 50, 20);
                }
                else
                {
                    wallColor = Color.White;
                    outsideColor = new Color(40, 10, 0);
                    insideColor = new Color(80, 50, 20);
                }
            }
        }
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
                if (startButton.clicked)
                {
                    screen = 1;
                    startNewGame();
                }
                optionsButton.Update();
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
                        int loadHeight = highestObstacle;
                        while (loadHeight < 0)
                        {
                            loadHeight += 25;
                        }
                        loadObstacle(500 + loadHeight, "Loading");
                        isLoading = false;
                        mouseHitbox.lasers.Clear();

                    }
                    else
                    {
                        if (enemies.Count > 0)
                        {
                            enemies.RemoveAt(0);
                        }
                        else
                        {
                            if (obstacles.Count > 0)
                            {
                                if (obstacles.Count > 76 /*76 is num of obstacles in the loading obstacles*/)
                                {
                                    obstacles.RemoveAt(0);
                                }
                                else
                                {
                                    //for (int i = 0; i < 76; i++)
                                    //{
                                    int randomNumber = random.Next(0, obstacles.Count - 1);

                                    obstacles.RemoveAt(randomNumber);
                                    //}
                                }
                                /*
                                 * 
                                 * obstacles.RemoveAt(0);
                                 * if ((ks.IsKeyDown(Keys.RightControl) || ks.IsKeyDown(Keys.RightControl)) && obstacles.Count > 3)
                                 * {
                                 *    obstacles.RemoveAt(0);
                                 *    obstacles.RemoveAt(0);
                                 *    obstacles.RemoveAt(0);
                                 *    obstacles.RemoveAt(0);
                                 * }
                                 */
                            }
                            else
                            {
                                lose = false;
                                startNewGame();
                            }
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
                        if (obstacle.hitbox.Intersects(mouseHitbox._hitbox) && obstacle._collide == true && !pause)
                        {
                            isLoading = true;
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
                            if (enemy != targetedEnemy)
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
                        colorButton._texture = Content.Load<Texture2D>("IceButton");
                        colorScheme = "Ice";
                    }
                    else if (colorScheme == "Ice")
                    {
                        colorButton._texture = Content.Load<Texture2D>("BeachButton");
                        colorScheme = "Beach";
                    }
                    else if (colorScheme == "Beach")
                    {
                        colorButton._texture = Content.Load<Texture2D>("ChocolateButton");
                        colorScheme = "Chocolate";
                    }
                    else if (colorScheme == "Chocolate")
                    {
                        colorButton._texture = Content.Load<Texture2D>("DefaultButton");
                        colorScheme = "Default";
                    }
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
                    if (shootStyle != 4)
                    {
                        shootStyle++;
                    }
                    else
                    {
                        shootStyle = 0;
                    }
                    canShootLaser = true;
                    mouseHitbox.Update();
                    mouseHitbox.fireLasers(Content.Load<Texture2D>("Laser"), laserColor, false);
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
            GraphicsDevice.Clear(insideColor);
            spriteBatch.Begin();
            if (screen == 0)//main menu
            {
                startButton.Draw(spriteBatch);
                optionsButton.Draw(spriteBatch);
            }
            else if (screen == 1)//gameplay
            {
                mouseHitbox.Draw(spriteBatch);
                for (int i = 0; i < obstacles.Count; i++)
                {
                    obstacles[i].Draw(spriteBatch);
                }
                foreach (Enemy enemy in enemies)
                {
                    enemy.Draw(spriteBatch);
                }
                spriteBatch.DrawString(font, string.Format("{0}", obstacles.Count), new Vector2(0, 950), textColor);
                spriteBatch.DrawString(font, string.Format("Score: {0}", score / 50), new Vector2(380, 950), textColor);
                int laserCount = mouseHitbox.lasers.Count;
                foreach (Enemy enemy in enemies)
                {
                    laserCount += enemy.body.lasers.Count();
                }
                spriteBatch.DrawString(font, string.Format("{0}", laserCount), new Vector2(240, 950), textColor);

                //Texture2D pixel = new Texture2D(GraphicsDevice, 1, 1);
                //pixel.SetData<Color>(new Color[] { Color.White });

                // if (enemies.Count > 0)
                //    spriteBatch.Draw(pixel, enemies[0].body._hitbox, Color.Red);
            }
            else if (screen == 2)
            {
                colorButton.Draw(spriteBatch);
                gamemodeButton.Draw(spriteBatch);
                shootStyleButton.Draw(spriteBatch);
                Vector2 superLongLineOfText = new Vector2(shootStyleButton.rectangle.X + shootStyleButton._texture.Width / 2 - Content.Load<Texture2D>("Ball").Width / 2, shootStyleButton.rectangle.Y + shootStyleButton._texture.Height / 2 - Content.Load<Texture2D>("Ball").Height / 2 + 10);
                //spriteBatch.Draw(Content.Load<Texture2D>("Ball"), superLongLineOfText, ballColor);
                mouseHitbox._color = ballColor;
                mouseHitbox._position = superLongLineOfText;
                backButton.Draw(spriteBatch);
                mouseHitbox.Draw(spriteBatch);
            }
            spriteBatch.End();
            frames++;
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}