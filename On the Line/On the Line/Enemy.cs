using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace On_the_Line
{
    public class Enemy
    {
        public MouseHitbox body;
        public static TimeSpan laserCooldown;
        public TimeSpan laserElapsedTime;
        Texture2D _laserTexture;
        int slow = 0;
        bool aims;
        public bool _rams;
        bool shooting = false;
        int _slideSpeed;
        public Enemy(Vector2 position, Texture2D texture, Texture2D spotlightTexture, Texture2D laserTexture, int shootstyle, int direction, bool doesAim, bool rams)
        {
            body = new MouseHitbox(Game1.wallColor, texture, spotlightTexture, true, shootstyle, direction);
            body._position = position;
            _laserTexture = laserTexture;
            aims = doesAim;
            if (body._shootStyle == 0)
            {
                laserCooldown = new TimeSpan(0, 0, 0, 2, 0);
            }
            else if (body._shootStyle == 1)
            {
                laserCooldown = new TimeSpan(0, 0, 0, 0, 750);
            }
            else if (body._shootStyle == 2)
            {
                laserCooldown = new TimeSpan(0, 0, 0, 4, 0);
            }
            else if (body._shootStyle == 3)
            {
                laserCooldown = new TimeSpan(0, 0, 0, 0, 900);
            }
            else if (body._shootStyle == 4)
            {
                laserCooldown = new TimeSpan(0, 0, 0, 3, 0);
            }
            else if (body._shootStyle == 5)
            {
                laserCooldown = new TimeSpan(0, 0, 0, 1, 0);
            }
            _rams = rams;
            _slideSpeed = 31;
            body._position += new Vector2(496, 0);
        }
        public void Update()
        {
            if (_slideSpeed > 0)
            {
                body._position -= new Vector2(_slideSpeed, 0);
                _slideSpeed--;
            }
            else
            {
                if (_rams)
                {
                    MouseHitbox MH = Game1.mouseHitbox;
                    if (aims && Math.Abs(body._position.Y - MH._position.Y) < 250 && !Game1.pause)
                    {
                        body._position.X += (MH._position.X - body._position.X) / 30;
                        body._position.Y += (MH._position.Y - body._position.Y) / 30;
                    }
                }
                else
                {
                    if (laserElapsedTime >= laserCooldown)
                    {
                        if (aims)
                        {
                            body.fireLasers(_laserTexture, Game1.wallColor, true);
                            body.fireLasers(_laserTexture, Game1.wallColor, false);
                        }
                        laserElapsedTime = TimeSpan.Zero;
                    }                    
                    for (int i = 0; i < body.lasers.Count; i++)
                    {
                        Laser laser = body.lasers[i];
                        body.lasers[i].Update();
                        if (Game1.screen == 1 && (laser._rect.X > 500 || laser._rect.X < 0 || laser._rect.Y < 0 || laser._rect.Y > 1000))
                        {
                            body.lasers.Remove(body.lasers[i]);
                        }
                        if (laser._rect.Intersects(Game1.mouseHitbox._hitbox))
                        {
                            body.lasers.Clear();
                            Game1.isLoading = true;
                        }
                    }
                }
                body._hitbox = new Rectangle((int)body._position.X + body._texture.Width / 4, (int)body._position.Y + body._texture.Height / 4, body._texture.Width / 2, body._texture.Height / 2);
            }
            KeyboardState ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.Up))
            {
                body._position.Y++;
                foreach (Laser laser in body.lasers)
                {
                    laser._rect.X += laser._moveX;
                    laser._rect.Y += laser._moveY;
                }
            }
            else if (ks.IsKeyDown(Keys.Down))
            {
                body._position.Y--;
                foreach (Laser laser in body.lasers)
                {
                    laser._rect.X -= laser._moveX;
                    laser._rect.Y -= laser._moveY;
                }
            }
            else if (!Game1.pause && !Game1.lose)
            {
                body._position.Y++;
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            body.Draw(spriteBatch);
        }
    }

}

