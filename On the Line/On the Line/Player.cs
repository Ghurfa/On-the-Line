using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace On_the_Line
{
    public class Player : Sprite
    {
        MouseState mouseState;
        MouseState lastMouseState;
        Texture2D spotlightTexture;
        public Rectangle Spotlight;
        int direction = 0;
        public int _shootStyle;
        public bool IsClicked = false;
        public List<Laser> lasers = new List<Laser>();
        bool showWhenLose;
        public int slow = 0;
        public bool canShoot = true;
        public bool Counting = false;
        public int CountingSecond = 3;
        public int CountingCentisecond = 0;
        public TimeSpan CountingElapsedTime;
        public TimeSpan laserElapsedTime;
        public Tuple<TimeSpan, int, int, string, string, string> stats = new Tuple<TimeSpan, int, int, string, string, string>(new TimeSpan(0, 0, 0, 1, 0), 0, 0, "", "", "");//LaserCooldown, NumOfBullets, BulletPenetration, BulletSpeed, Pros, Cons

        public Player(Color color, Texture2D texture, Texture2D spotlightTexure, bool showWhenLose, Vector2 position, int shootStyle = 0, int direction = 0)
            : base(position, texture, color)
        {
            Color = color;
            Texture = texture;
            Position = position;
            Hitbox = new Rectangle((int)Position.X, (int)Position.Y, (int)(Texture.Width * OnTheLine.GlobalScaleFactor), (int)(Texture.Height * OnTheLine.GlobalScaleFactor));
            spotlightTexture = spotlightTexure;
            _shootStyle = shootStyle;
            this.direction = direction;
            this.showWhenLose = showWhenLose;
        }
        public void Update(GameTime gameTime)
        {
            Hitbox = new Rectangle((int)Position.X, (int)Position.Y, (int)(Texture.Width * OnTheLine.GlobalScaleFactor), (int)(Texture.Height * OnTheLine.GlobalScaleFactor));
            if (!OnTheLine.isPaused)
            {
                laserElapsedTime += gameTime.ElapsedGameTime;
            }
            if (laserElapsedTime >= stats.Item1)
            {
                laserElapsedTime = TimeSpan.Zero;
                canShoot = true;
            }
            if (Counting)
            {
                CountingElapsedTime += gameTime.ElapsedGameTime;
            }
            _shootStyle = OnTheLine.shootStyle;
            if (_shootStyle == 0)
            {
                stats = new Tuple<TimeSpan, int, int, string, string, string>(new TimeSpan(0, 0, 0, 1, 0), 3, 7, "Normal", "Good penetration, Forward spread", "No back shooting");
            }
            else if (_shootStyle == 1)
            {
                stats = new Tuple<TimeSpan, int, int, string, string, string>(new TimeSpan(0, 0, 0, 1, 500), 8, 5, "Normal", "Shoots in all directions", "Bad Penetration, Slower reload");
            }
            else if (_shootStyle == 2)
            {
                stats = new Tuple<TimeSpan, int, int, string, string, string>(new TimeSpan(0, 0, 0, 3, 500), 25, 2, "Fast", "Large amount of bullets, Huge spread", "Slow reload, No back shooting");
            }
            else if (_shootStyle == 3)
            {
                stats = new Tuple<TimeSpan, int, int, string, string, string>(new TimeSpan(0, 0, 0, 0, 900), 20, 1, "Fast", "Faster reload, Large spread", "No back shooting, Unfocused");
            }
            else if (_shootStyle == 4)
            {
                stats = new Tuple<TimeSpan, int, int, string, string, string>(new TimeSpan(0, 0, 0, 1, 500), 25, 1, "Zero", "Zero movement on screen", "Slower reload, Low penetration");
            }
            else if (_shootStyle == 5)
            {
                stats = new Tuple<TimeSpan, int, int, string, string, string>(new TimeSpan(0, 0, 0, 1, 0), 5, 10, "Fast", "Fast Bullets, Focused Fire", "High penetration, No Spread");
            }
            Spotlight = new Rectangle((int)Position.X + Texture.Width / 2 - (int)(400 * OnTheLine.GlobalScaleFactor), (int)Position.Y + Texture.Height / 2 - (int)(400 * OnTheLine.GlobalScaleFactor), (int)(800 * OnTheLine.GlobalScaleFactor), (int)(800 * OnTheLine.GlobalScaleFactor));
            for (int i = 0; i < lasers.Count; i++)
            {
                lasers[i].Update();
                if (OnTheLine.screen == Screen.GameScreen && (lasers[i].Hitbox.X > 500 || lasers[i].Hitbox.X < 0 || lasers[i].Hitbox.Y < 0 || lasers[i].Hitbox.Y > 1000) || OnTheLine.hasLost)
                {
                    lasers.Remove(lasers[i]);
                    i--;
                }
            }
            mouseState = Mouse.GetState();
            if (OnTheLine.screen != Screen.OptionsMenu && !OnTheLine.hasLost)
            {
                if (Counting)
                {
                    CountingCentisecond = 50 - (int)CountingElapsedTime.Milliseconds / 10;
                    if (CountingCentisecond <= 5)
                    {
                        IsClicked = true;
                        OnTheLine.isPaused = false;
                        Counting = false;
                    }
                    if (!Hitbox.Contains(mouseState.X, mouseState.Y))
                    {
                        Counting = false;
                    }
                }
                if (mouseState.LeftButton == ButtonState.Pressed && Hitbox.Contains(mouseState.X, mouseState.Y) && lastMouseState.LeftButton == ButtonState.Released)
                {
                    Counting = true;
                    CountingElapsedTime = TimeSpan.Zero;
                }
                if (mouseState.LeftButton == ButtonState.Released && OnTheLine.screen == Screen.GameScreen)
                {
                    IsClicked = false;
                    OnTheLine.isPaused = true;
                    Counting = false;
                }
                KeyboardState ks = Keyboard.GetState();
                if (IsClicked)
                {
                    if (ks.IsKeyDown(Keys.LeftShift))
                    {
                        Position.Y++;
                        if (OnTheLine.gameMode == GameMode.Fastmode)
                        {
                            Position.Y++;
                        }
                    }
                    else
                    {
                        Position.Y = mouseState.Y - (Texture.Height / 2);
                        Position.X = mouseState.X - (Texture.Width / 2);
                    }
                }
                if (ks.IsKeyDown(Keys.Up))
                {
                    foreach (Laser laser in lasers)
                    {
                        laser.Position.X += laser.Speed.X;
                        laser.Position.Y += laser.Speed.Y;
                    }
                }
                else if (ks.IsKeyDown(Keys.Down))
                {
                    foreach (Laser laser in lasers)
                    {
                        laser.Position.X -= laser.Speed.X;
                        laser.Position.Y -= laser.Speed.X;
                    }
                }
            }
            lastMouseState = mouseState;
        }
        /// <summary>
        /// This spawns all the lasers
        /// </summary>
        /// <param name="texture">The lasers' texture</param>
        /// <param name="laserColor">The color of the laser</param>
        /// <param name="aims">Whether or not it aims (for enemies)</param>
        public void fireLasers(Texture2D texture, Color laserColor, bool aims)
        {
            Vector2 startPos = new Vector2(Position.X + Texture.Width / 2 - 2, Position.Y + Texture.Height / 2 - 2);
            int laserCount = OnTheLine.player.lasers.Count;
            foreach (Enemy enemy in OnTheLine.enemies)
            {
                laserCount += enemy.lasers.Count();
            }
            if (canShoot)
            {
                if (aims)
                {
                    int aimX = ((int)OnTheLine.player.Hitbox.X - (int)startPos.X) / 25;
                    int aimY = ((int)OnTheLine.player.Hitbox.Y - (int)startPos.Y) / 25;
                    lasers.Add(new Laser(startPos, aimX, aimY, texture, 1, laserColor));
                }
                else
                {
                    if (laserCount < 500)
                    {

                        #region Shoot Style 0
                        if (_shootStyle == 0)
                        {
                            #region Direction 0
                            if (direction == 0)
                            {
                                lasers.Add(new Laser(startPos, 0, -2, texture, stats.Item3, laserColor));
                                lasers.Add(new Laser(startPos, 2, -2, texture, stats.Item3, laserColor));
                                lasers.Add(new Laser(startPos, -2, -2, texture, stats.Item3, laserColor));
                            }
                            #endregion
                            #region Direction 1
                            else if (direction == 1)
                            {
                                lasers.Add(new Laser(startPos, -2, 0, texture, stats.Item3, laserColor));
                                lasers.Add(new Laser(startPos, -2, 2, texture, stats.Item3, laserColor));
                                lasers.Add(new Laser(startPos, -2, -2, texture, stats.Item3, laserColor));
                            }
                            #endregion
                            #region Direction 2
                            else if (direction == 2)
                            {
                                lasers.Add(new Laser(startPos, 0, 2, texture, stats.Item3, laserColor));
                                lasers.Add(new Laser(startPos, 2, 2, texture, stats.Item3, laserColor));
                                lasers.Add(new Laser(startPos, -2, 2, texture, stats.Item3, laserColor));
                            }
                            #endregion
                            #region Direction 3
                            else if (direction == 3)
                            {
                                lasers.Add(new Laser(startPos, 2, 0, texture, stats.Item3, laserColor));
                                lasers.Add(new Laser(startPos, 2, 2, texture, stats.Item3, laserColor));
                                lasers.Add(new Laser(startPos, 2, -2, texture, stats.Item3, laserColor));
                            }
                            #endregion
                        }
                        #endregion

                        #region Shoot Style 1
                        else if (_shootStyle == 1)
                        {
                            if (direction == 0)
                            {
                                for (int x = -2; x < 3; x += 2)
                                {
                                    for (int y = -2; y < 3; y += 2)
                                    {
                                        if (x != 0 || y != 0)
                                            lasers.Add(new Laser(startPos, x, y, texture, stats.Item3, laserColor));
                                    }
                                }

                            }
                            else if (direction == 1)
                            {
                                int xSpeed = 2;
                                int ySpeed = 2;
                                int startX = (int)startPos.X + 50;
                                int startY;
                                for (int x = 0; x < 2; x++)
                                {
                                    startY = (int)startPos.Y + 50;
                                    for (int y = 0; y < 2; y++)
                                    {
                                        lasers.Add(new Laser(new Vector2(startX, startY), xSpeed, ySpeed, texture, stats.Item3, laserColor));
                                        ySpeed *= -1;
                                        startY -= 100;
                                    }
                                    xSpeed *= -1;
                                    startX -= 100;
                                }
                            }
                        }
                        #endregion

                        #region Shoot Style 2
                        else if (_shootStyle == 2)
                        {
                            if (direction == 0)
                            {
                                for (int a = 1; a < 4; a++)
                                {
                                    for (int b = 0; b < 3; b += 2)
                                    {
                                        for (int c = 1; c < 5; c++)
                                        {
                                            lasers.Add(new Laser(startPos, a * (b - 1), -c, texture, stats.Item3, laserColor));
                                        }
                                    }
                                }
                                lasers.Add(new Laser(startPos, 0, -1, texture, stats.Item3, laserColor));
                            }
                            else if (direction == 1)
                            {
                                for (int a = 1; a < 4; a++)
                                {
                                    for (int b = 0; b < 3; b += 2)
                                    {
                                        for (int c = 1; c < 5; c++)
                                        {
                                            lasers.Add(new Laser(startPos, -c, a * (b - 1), texture, stats.Item3, laserColor));
                                        }
                                    }
                                }
                                lasers.Add(new Laser(startPos, -1, 0, texture, stats.Item3, laserColor));
                            }
                            else if (direction == 2)
                            {
                                for (int a = 1; a < 4; a++)
                                {
                                    for (int b = 0; b < 3; b += 2)
                                    {
                                        for (int c = 1; c < 5; c++)
                                        {
                                            lasers.Add(new Laser(startPos, a * (b - 1), c, texture, stats.Item3, laserColor));
                                        }
                                    }
                                }
                                lasers.Add(new Laser(startPos, 0, 1, texture, stats.Item3, laserColor));
                            }
                            else if (direction == 3)
                            {
                                for (int a = 1; a < 4; a++)
                                {
                                    for (int b = 0; b < 3; b += 2)
                                    {
                                        for (int c = 1; c < 5; c++)
                                        {
                                            lasers.Add(new Laser(startPos, c, a * (b - 1), texture, stats.Item3, laserColor));
                                        }
                                    }
                                }
                                lasers.Add(new Laser(startPos, 1, 0, texture, stats.Item3, laserColor));
                            }
                        }
                        #endregion

                        #region Shoot Style 3
                        else if (_shootStyle == 3)
                        {
                            startPos.X = Position.X - 590;
                            for (int d = 0; d < 20; d++)
                            {
                                lasers.Add(new Laser(startPos, 0, -5, texture, stats.Item3, laserColor));
                                startPos.X += 50;
                            }
                        }
                        #endregion

                        #region Shoot Style 4
                        else if (_shootStyle == 4)
                        {
                            startPos.Y = Position.Y - 38;
                            if (direction == 0)
                            {
                                for (int i = 0; i < 5; i++)
                                {
                                    startPos.X = Position.X - 38;
                                    for (int d = 0; d < 5; d++)
                                    {
                                        lasers.Add(new Laser(startPos, 0, 0, texture, stats.Item3, laserColor));
                                        startPos.X += 25;
                                    }
                                    startPos.Y += 25;
                                }
                            }
                            else if (direction == 1)
                            {
                                for (int i = 0; i < 5; i++)
                                {
                                    startPos.X = Position.X - 38;
                                    for (int d = 0; d < 5; d++)
                                    {
                                        lasers.Add(new Laser(startPos, 0, 1, texture, stats.Item3, laserColor));
                                        startPos.X += 25;
                                    }
                                    startPos.Y += 25;
                                }
                            }
                        }
                        #endregion

                        #region Shoot Style 5
                        else if (_shootStyle == 5)
                        {
                            if (direction == 0)
                            {
                                startPos.X -= 25;
                                startPos.Y -= 25;
                                for (int a = 1; a > -2; a -= 2)
                                {
                                    for (int i = 0; i < 3; i++)
                                    {
                                        lasers.Add(new Laser(new Vector2(startPos.X, startPos.Y), 0, -10, texture, stats.Item3, laserColor));
                                        startPos.X += 12;
                                        startPos.Y -= 12 * a;
                                    }
                                    startPos.X -= 12;
                                    startPos.Y += 12;
                                }
                            }
                            else if (direction == 1)
                            {
                                startPos.X += 25;
                                startPos.Y -= 25;
                                for (int a = 1; a > -2; a -= 2)
                                {
                                    for (int i = 0; i < 3; i++)
                                    {
                                        lasers.Add(new Laser(new Vector2(startPos.X, startPos.Y), 10, 0, texture, stats.Item3, laserColor));
                                        startPos.Y += 12;
                                        startPos.X += 12 * a;
                                    }
                                    startPos.Y -= 12;
                                    startPos.X -= 12;
                                }
                            }
                            else if (direction == 2)
                            {
                                startPos.X -= 25;
                                startPos.Y += 25;
                                for (int a = 1; a > -2; a -= 2)
                                {
                                    for (int i = 0; i < 3; i++)
                                    {
                                        lasers.Add(new Laser(new Vector2(startPos.X, startPos.Y), 0, 10, texture, stats.Item3, laserColor));
                                        startPos.X += 12;
                                        startPos.Y += 12 * a;
                                    }
                                    startPos.X -= 12;
                                    startPos.Y -= 12;
                                }
                            }
                            else if (direction == 3)
                            {
                                startPos.X -= 25;
                                startPos.Y -= 25;
                                for (int a = 1; a > -2; a -= 2)
                                {
                                    for (int i = 0; i < 3; i++)
                                    {
                                        lasers.Add(new Laser(new Vector2(startPos.X, startPos.Y), -10, 0, texture, stats.Item3, laserColor));
                                        startPos.Y += 12;
                                        startPos.X -= 12 * a;
                                    }
                                    startPos.Y -= 12;
                                    startPos.X += 12;
                                }
                            }
                        }
                        #endregion
                    }
                }
            }
        }

        /// <summary>
        /// This draws the body, its spotlight, and its lasers
        /// </summary>
        /// <param name="spriteBatch"></param>
        public new void Draw(SpriteBatch spriteBatch)
        {
            if (!OnTheLine.hasLost || OnTheLine.hasLost && showWhenLose)
            {
                foreach (Laser laser in lasers)
                {
                    laser.Draw(spriteBatch);
                }
                spriteBatch.Draw(Texture, Position + new Vector2(Texture.Width / 2, Texture.Height / 2), null, Color, 0, new Vector2(Texture.Width / 2, Texture.Height / 2), OnTheLine.GlobalScaleFactor, SpriteEffects.None, 0);
                if (OnTheLine.gameMode == GameMode.Spotlight && !OnTheLine.hasLost)
                {
                    spriteBatch.Draw(spotlightTexture, new Vector2(Position.X - spotlightTexture.Width / 2 * OnTheLine.GlobalScaleFactor, Position.Y - spotlightTexture.Height / 2 * OnTheLine.GlobalScaleFactor), null, OnTheLine.BackgroundColor, 0, new Vector2(Texture.Width / 2, Texture.Height / 2), OnTheLine.GlobalScaleFactor, SpriteEffects.None, 0);
                }
            }
        }
    }
}
