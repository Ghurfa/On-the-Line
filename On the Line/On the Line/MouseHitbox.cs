using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace On_the_Line
{
    public class MouseHitbox
    {
        public Vector2 Position;
        public Vector2 RelativePosition;
        public Color _color;
        MouseState mouseState;
        MouseState lastMouseState;
        public Texture2D _texture;
        Texture2D spotlightTexture;
        public Rectangle _hitbox;
        public Rectangle Spotlight;
        int direction = 0;
        public int _shootStyle;
        bool isclicked = false;
        public List<Laser> lasers = new List<Laser>();
        bool showWhenLose;
        public int reloadCycle = 0;
        public int slow = 0;
        public bool canShoot = true;
        public bool Counting = false;
        public int CountingSecond = 3;
        public int CountingCentisecond = 0;
        public TimeSpan CountingElapsedTime;
        public TimeSpan laserElapsedTime;
        public Tuple<TimeSpan, int, int, string, string, string> stats = new Tuple<TimeSpan, int, int, string, string, string>(new TimeSpan(0, 0, 0, 1, 0), 0, 0, "", "", "");//LaserCooldown, NumOfBullets, BulletPenetration, BulletSpeed, Pros, Cons

        public MouseHitbox(Color color, Texture2D texture, Texture2D spotlightTexure, bool showWhenLose, Vector2 position, int shootStyle = 0, int direction = 0)
        {
            _color = color;
            _texture = texture;
            Position = position;
            RelativePosition = Position;
            _hitbox = new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);
            spotlightTexture = spotlightTexure;
            _shootStyle = shootStyle;
            this.direction = direction;
            this.showWhenLose = showWhenLose;
        }
        public void Update(GameTime gameTime)
        {
            _hitbox = new Rectangle((int)Position.X + _texture.Width / 4, (int)Position.Y + _texture.Height / 4, _texture.Width / 2, _texture.Height / 2);
            if (!Game1.pause)
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
            _shootStyle = Game1.shootStyle;
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
                stats = new Tuple<TimeSpan, int, int, string, string, string>(new TimeSpan(0, 0, 0, 1, 0), 5, 10, "Fast", "Fast Bullets", "High penetration, Focused");
            }
            Spotlight = new Rectangle((int)Position.X + _texture.Width / 2 - 100, (int)Position.Y + _texture.Height / 2 - 100, 200, 200);
            for (int i = 0; i < lasers.Count; i++)
            {
                lasers[i].Update();
                if (Game1.screen == 1 && (lasers[i]._rect.X > 500 || lasers[i]._rect.X < 0 || lasers[i]._rect.Y < 0 || lasers[i]._rect.Y > 1000) || Game1.lose)
                {
                    lasers.Remove(lasers[i]);
                    i--;
                }
            }
            mouseState = Mouse.GetState();
            if (Game1.screen != 2 && !Game1.lose)
            {
                if (Counting)
                {
                    CountingCentisecond = 50 - (int)CountingElapsedTime.Milliseconds/10;
                    if (CountingCentisecond <= 5)
                    {
                        isclicked = true;
                        Game1.pause = false;
                        Counting = false;
                    }
                    if(!_hitbox.Contains(mouseState.X, mouseState.Y))
                    {
                        Counting = false;
                    }
                }
                if (mouseState.LeftButton == ButtonState.Pressed && _hitbox.Contains(mouseState.X, mouseState.Y) && lastMouseState.LeftButton == ButtonState.Released)
                {
                    Counting = true;
                    CountingElapsedTime = TimeSpan.Zero;
                }
                if (mouseState.LeftButton == ButtonState.Released)
                {
                    isclicked = false;
                    Game1.pause = true;
                    Counting = false;
                }
                if (isclicked)
                {
                    KeyboardState ks = Keyboard.GetState();
                    if (ks.IsKeyDown(Keys.LeftShift))
                    {
                        Position.Y++;
                        if (Game1.GameMode == "Fastmode")
                        {
                            Position.Y++;
                        }
                    }
                    else
                    {
                        Position.Y = mouseState.Y - (_texture.Height / 2);
                        Position.X = mouseState.X - (_texture.Width / 2);
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
            Vector2 startPos = new Vector2(Position.X + _texture.Width / 2 - 2, Position.Y + _texture.Height / 2 - 2);
            int laserCount = Game1.mouseHitbox.lasers.Count;
            foreach (Enemy enemy in Game1.enemies)
            {
                laserCount += enemy.body.lasers.Count();
            }
            if (canShoot)
            {
                if (aims)
                {
                    int aimX = ((int)Game1.mouseHitbox._hitbox.X - (int)startPos.X) / 25;
                    int aimY = ((int)Game1.mouseHitbox._hitbox.Y - (int)startPos.Y) / 25;
                    lasers.Add(new Laser(startPos, aimX, aimY, texture, 1, laserColor));
                }
                else
                {
                    reloadCycle++;
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
                                for(int x = -2; x < 3; x+=2)
                                {
                                    for(int y = -2; y < 3; y+=2)
                                    {
                                        if(x != 0 || y != 0)
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
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Laser laser in lasers)
            {
                laser.Draw(spriteBatch);
            }
            if (!Game1.lose || Game1.lose && showWhenLose)
            {
                spriteBatch.Draw(_texture, Position, _color);
                if (Game1.GameMode == "Spotlight")
                {
                    spriteBatch.Draw(spotlightTexture, new Vector2(Spotlight.X, Spotlight.Y), Game1.textColor);
                }
            }
        }
    }
}
