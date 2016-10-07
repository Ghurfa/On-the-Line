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
        public Vector2 _position;
        public Color _color;
        MouseState mouseState;
        MouseState lastMouseState;
        public Texture2D _texture;
        Texture2D _spotlightTexture;
        public Rectangle _hitbox;
        public Rectangle _spotlight;
        int _direction = 0;
        public int _shootStyle;
        bool isclicked = false;
        public List<Laser> lasers = new List<Laser>();
        bool _showWhenLose;
        public int reloadCycle = 0;
        public int slow = 0;
        public int numOfBullets;
        public int BulletPen = 7;
        public string bulletSpeed;
        public string pros;
        public string cons;

        public MouseHitbox(Color color, Texture2D texture, Texture2D spotlightTexure, bool showWhenLose, int shootStyle = 0, int direction = 0)
        {
            _color = color;
            _texture = texture;
            _hitbox = new Rectangle((int)_position.X, (int)_position.Y, _texture.Width, _texture.Height);
            _spotlightTexture = spotlightTexure;
            _shootStyle = shootStyle;
            _direction = direction;
            _showWhenLose = showWhenLose;
        }
        public void Update()
        {
            _shootStyle = Game1.shootStyle;
            if (_shootStyle == 0)
            {
                Game1.laserCooldown = new TimeSpan(0, 0, 0, 1, 0);
                numOfBullets = 3;
                bulletSpeed = "Normal";
                pros = "Good penetration, Forward spread";
                cons = "No back shooting";
            }
            else if (_shootStyle == 1)
            {
                Game1.laserCooldown = new TimeSpan(0, 0, 0, 1, 500);
                numOfBullets = 8;
                bulletSpeed = "Normal";
                pros = "Shoots in all directions";
                cons = "Bad Penetration, Slower reload";
            }
            else if (_shootStyle == 2)
            {
                Game1.laserCooldown = new TimeSpan(0, 0, 0, 3, 500);
                numOfBullets = 25;
                bulletSpeed = "Fast";
                pros = "Large amount of bullets, Huge spread";
                cons = "Slow reload, No back shooting";
            }
            else if (_shootStyle == 3)
            {
                Game1.laserCooldown = new TimeSpan(0, 0, 0, 0, 900);
                numOfBullets = 20;
                bulletSpeed = "Fast";
                pros = "Faster reload, Large spread";
                cons = "No back shooting, Unfocused";
            }
            else if (_shootStyle == 4)
            {
                Game1.laserCooldown = new TimeSpan(0, 0, 0, 1, 500);
                numOfBullets = 25;
                bulletSpeed = "Zero";
                pros = "Zero movement on screen";
                cons = "Slower reload, Low penetration";
            }
            else if (_shootStyle == 5)
            {
                Game1.laserCooldown = new TimeSpan(0, 0, 0, 1, 0);
                numOfBullets = 5;
                bulletSpeed = "Fast";
                pros = "Fast Bullets, High penetration, Focused";
                cons = "Zero spread";
            }
            _spotlight = new Rectangle((int)_position.X + _texture.Width / 2 - 100, (int)_position.Y + _texture.Height / 2 - 100, 200, 200);
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
                if (mouseState.LeftButton == ButtonState.Pressed && _hitbox.Contains(mouseState.X, mouseState.Y))
                {
                    isclicked = true;
                    Game1.pause = false;
                }
                if (mouseState.LeftButton == ButtonState.Released)
                {
                    isclicked = false;
                    Game1.pause = true;
                }
                if (isclicked)
                {
                    KeyboardState ks = Keyboard.GetState();
                    if (ks.IsKeyDown(Keys.LeftShift))
                    {
                        _position.Y++;
                        if (Game1.gamemode == "fastmode")
                        {
                            _position.Y++;
                        }
                    }
                    else
                    {
                        _position.Y = mouseState.Y - (_texture.Height / 2);
                        _position.X = mouseState.X - (_texture.Width / 2);
                    }
                }
            }
            _hitbox = new Rectangle((int)_position.X + _texture.Width / 4, (int)_position.Y + _texture.Height / 4, _texture.Width / 2, _texture.Height / 2);
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
            Vector2 startPos = new Vector2(_position.X + _texture.Width / 2, _position.Y + _texture.Height / 2);
            int laserCount = Game1.mouseHitbox.lasers.Count;
            foreach (Enemy enemy in Game1.enemies)
            {
                laserCount += enemy.body.lasers.Count();
            }
            if (aims)
            {
                int aimX = ((int)Game1.mouseHitbox._hitbox.X - (int)startPos.X) / 25;
                int aimY = ((int)Game1.mouseHitbox._hitbox.Y - (int)startPos.Y) / 25;
                lasers.Add(new Laser(startPos, aimX, aimY, texture, 1, laserColor));
            }
            else
            {
                if (slow == 10)
                {
                    slow = 0;
                    reloadCycle++;
                    if (laserCount < 500)
                    {

                        #region Shoot Style 0
                        if (_shootStyle == 0)
                        {
                            BulletPen = 7;
                            #region Direction 0
                            if (_direction == 0)
                            {
                                lasers.Add(new Laser(startPos, 0, -2, texture, BulletPen, laserColor));
                                lasers.Add(new Laser(startPos, 2, -2, texture, BulletPen, laserColor));
                                lasers.Add(new Laser(startPos, -2, -2, texture, BulletPen, laserColor));
                            }
                            #endregion
                            #region Direction 1
                            else if (_direction == 1)
                            {
                                lasers.Add(new Laser(startPos, -2, 0, texture, BulletPen, laserColor));
                                lasers.Add(new Laser(startPos, -2, 2, texture, BulletPen, laserColor));
                                lasers.Add(new Laser(startPos, -2, -2, texture, BulletPen, laserColor));
                            }
                            #endregion
                            #region Direction 2
                            else if (_direction == 2)
                            {
                                lasers.Add(new Laser(startPos, 0, 2, texture, BulletPen, laserColor));
                                lasers.Add(new Laser(startPos, 2, 2, texture, BulletPen, laserColor));
                                lasers.Add(new Laser(startPos, -2, 2, texture, BulletPen, laserColor));
                            }
                            #endregion
                            #region Direction 3
                            else if (_direction == 3)
                            {
                                lasers.Add(new Laser(startPos, 2, 0, texture, BulletPen, laserColor));
                                lasers.Add(new Laser(startPos, 2, 2, texture, BulletPen, laserColor));
                                lasers.Add(new Laser(startPos, 2, -2, texture, BulletPen, laserColor));
                            }
                            #endregion
                            if (reloadCycle == 1)
                            {
                                reloadCycle = 0;
                            }
                        }
                        #endregion

                        #region Shoot Style 1
                        else if (_shootStyle == 1)
                        {
                            BulletPen = 5;
                            if (_direction == 0)
                            {
                                if (reloadCycle == 1)
                                {
                                    lasers.Add(new Laser(startPos, 0, 2, texture, BulletPen, laserColor));
                                    lasers.Add(new Laser(startPos, 2, 0, texture, BulletPen, laserColor));
                                    lasers.Add(new Laser(startPos, 0, -2, texture, BulletPen, laserColor));
                                    lasers.Add(new Laser(startPos, -2, 0, texture, BulletPen, laserColor));
                                }
                                else
                                {
                                    lasers.Add(new Laser(startPos, 2, 2, texture, BulletPen, laserColor));
                                    lasers.Add(new Laser(startPos, 2, -2, texture, BulletPen, laserColor));
                                    lasers.Add(new Laser(startPos, -2, -2, texture, BulletPen, laserColor));
                                    lasers.Add(new Laser(startPos, -2, 2, texture, BulletPen, laserColor));
                                }
                                if (reloadCycle == 2)
                                {
                                    reloadCycle = 0;
                                }
                            }
                            else if (_direction == 1)
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
                                        lasers.Add(new Laser(new Vector2(startX, startY), xSpeed, ySpeed, texture, BulletPen, laserColor));
                                        ySpeed *= -1;
                                        startY -= 100;
                                    }
                                    xSpeed *= -1;
                                    startX -= 100;
                                }
                                if (reloadCycle == 1)
                                {
                                    reloadCycle = 0;
                                }
                            }
                        }
                        #endregion

                        #region Shoot Style 2
                        else if (_shootStyle == 2)
                        {
                            BulletPen = 2;
                            if (_direction == 0)
                            {
                                for (int a = 1; a < 4; a++)
                                {
                                    for (int b = 0; b < 3; b += 2)
                                    {
                                        for (int c = 1; c < 5; c++)
                                        {
                                            lasers.Add(new Laser(startPos, a * (b - 1), -c, texture, BulletPen, laserColor));
                                        }
                                    }
                                }
                                lasers.Add(new Laser(startPos, 0, -1, texture, BulletPen, laserColor));
                            }
                            else if (_direction == 1)
                            {
                                for (int a = 1; a < 4; a++)
                                {
                                    for (int b = 0; b < 3; b += 2)
                                    {
                                        for (int c = 1; c < 5; c++)
                                        {
                                            lasers.Add(new Laser(startPos, -c, a * (b - 1), texture, BulletPen, laserColor));
                                        }
                                    }
                                }
                                lasers.Add(new Laser(startPos, -1, 0, texture, BulletPen, laserColor));
                            }
                            else if (_direction == 2)
                            {
                                for (int a = 1; a < 4; a++)
                                {
                                    for (int b = 0; b < 3; b += 2)
                                    {
                                        for (int c = 1; c < 5; c++)
                                        {
                                            lasers.Add(new Laser(startPos, a * (b - 1), c, texture, BulletPen, laserColor));
                                        }
                                    }
                                }
                                lasers.Add(new Laser(startPos, 0, 1, texture, BulletPen, laserColor));
                            }
                            else if (_direction == 3)
                            {
                                for (int a = 1; a < 4; a++)
                                {
                                    for (int b = 0; b < 3; b += 2)
                                    {
                                        for (int c = 1; c < 5; c++)
                                        {
                                            lasers.Add(new Laser(startPos, c, a * (b - 1), texture, BulletPen, laserColor));
                                        }
                                    }
                                }
                                lasers.Add(new Laser(startPos, 1, 0, texture, BulletPen, laserColor));
                            }
                            if (reloadCycle == 1)
                            {
                                reloadCycle = 0;
                            }
                        }
                        #endregion

                        #region Shoot Style 3
                        else if (_shootStyle == 3)
                        {
                            BulletPen = 1;
                            startPos.X = _position.X - 590;
                            for (int d = 0; d < 20; d++)
                            {
                                lasers.Add(new Laser(startPos, 0, -5, texture, BulletPen, laserColor));
                                startPos.X += 50;
                            }
                            if (reloadCycle == 1)
                            {
                                reloadCycle = 0;
                            }
                        }
                        #endregion

                        #region Shoot Style 4
                        else if (_shootStyle == 4)
                        {
                            BulletPen = 1;
                            startPos.Y = _position.Y - 38;
                            if (_direction == 0)
                            {
                                for (int i = 0; i < 5; i++)
                                {
                                    startPos.X = _position.X - 38;
                                    for (int d = 0; d < 5; d++)
                                    {
                                        lasers.Add(new Laser(startPos, 0, 0, texture, BulletPen, laserColor));
                                        startPos.X += 25;
                                    }
                                    startPos.Y += 25;
                                }
                            }
                            else if (_direction == 1)
                            {
                                for (int i = 0; i < 5; i++)
                                {
                                    startPos.X = _position.X - 38;
                                    for (int d = 0; d < 5; d++)
                                    {
                                        lasers.Add(new Laser(startPos, 0, 1, texture, BulletPen, laserColor));
                                        startPos.X += 25;
                                    }
                                    startPos.Y += 25;
                                }
                            }
                            if (reloadCycle == 1)
                            {
                                reloadCycle = 0;
                            }
                        }
                        #endregion

                        #region Shoot Style 5
                        else if (_shootStyle == 5)
                        {
                            BulletPen = 10;
                            if (_direction == 0)
                            {
                                startPos.X -= 25;
                                startPos.Y -= 25;
                                for (int a = 1; a > -2; a -= 2)
                                {
                                    for (int i = 0; i < 3; i++)
                                    {
                                        lasers.Add(new Laser(new Vector2(startPos.X, startPos.Y), 0, -10, texture, BulletPen, laserColor));
                                        startPos.X += 12;
                                        startPos.Y -= 12 * a;
                                    }
                                    startPos.X -= 12;
                                    startPos.Y += 12;
                                }
                            }
                            else if (_direction == 1)
                            {
                                startPos.X += 25;
                                startPos.Y -= 25;
                                for (int a = 1; a > -2; a -= 2)
                                {
                                    for (int i = 0; i < 3; i++)
                                    {
                                        lasers.Add(new Laser(new Vector2(startPos.X, startPos.Y), 10, 0, texture, BulletPen, laserColor));
                                        startPos.Y += 12;
                                        startPos.X += 12 * a;
                                    }
                                    startPos.Y -= 12;
                                    startPos.X -= 12;
                                }
                            }
                            else if (_direction == 2)
                            {
                                startPos.X -= 25;
                                startPos.Y += 25;
                                for (int a = 1; a > -2; a -= 2)
                                {
                                    for (int i = 0; i < 3; i++)
                                    {
                                        lasers.Add(new Laser(new Vector2(startPos.X, startPos.Y), 0, 10, texture, BulletPen, laserColor));
                                        startPos.X += 12;
                                        startPos.Y += 12 * a;
                                    }
                                    startPos.X -= 12;
                                    startPos.Y -= 12;
                                }
                            }
                            else if (_direction == 3)
                            {
                                startPos.X -= 25;
                                startPos.Y -= 25;
                                for (int a = 1; a > -2; a -= 2)
                                {
                                    for (int i = 0; i < 3; i++)
                                    {
                                        lasers.Add(new Laser(new Vector2(startPos.X, startPos.Y), -10, 0, texture, BulletPen, laserColor));
                                        startPos.Y += 12;
                                        startPos.X -= 12 * a;
                                    }
                                    startPos.Y -= 12;
                                    startPos.X += 12;
                                }
                            }
                            if (reloadCycle == 1)
                            {
                                reloadCycle = 0;
                            }
                        }
                        #endregion
                    }
                }

                else
                {
                    slow++;
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
            if (!Game1.lose || Game1.lose && _showWhenLose)
            {
                spriteBatch.Draw(_texture, _position, _color);
                if (Game1.gamemode == "spotlight")
                {
                    spriteBatch.Draw(_spotlightTexture, new Vector2(_spotlight.X, _spotlight.Y), Game1.textColor);
                }
            }
        }
    }
}
