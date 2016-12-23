using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace On_the_Line
{
    public class Enemy:MouseHitbox
    {
        Texture2D _laserTexture;
        bool aims;
        public bool _rams;
        int _slideSpeed;
        public Enemy(Vector2 position, Texture2D texture, Texture2D spotlightTexture, Texture2D laserTexture, int shootstyle, int direction, bool doesAim, bool rams)
            :base(OnTheLine.WallColor, texture, spotlightTexture, true, position, shootstyle, direction)
        {
            _laserTexture = laserTexture;
            aims = doesAim;
            _rams = rams;
            _slideSpeed = 31;
            Position += new Vector2(496, 0);
        }
        public new void Update(GameTime gameTime)
        {
            if (_slideSpeed > 0)
            {
                Position -= new Vector2(_slideSpeed, 0);
                _slideSpeed--;
            }
            else
            {
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
                if (_rams)
                {
                    MouseHitbox MH = OnTheLine.mouseHitbox;
                    if (aims && Math.Abs(Position.Y - MH.Position.Y) < 250 && !OnTheLine.isPaused)
                    {
                        Position.X += (MH.Position.X - Position.X) / 30;
                        Position.Y += (MH.Position.Y - Position.Y) / 30;
                    }
                }
                else
                {
                    laserElapsedTime += gameTime.ElapsedGameTime;
                    if (laserElapsedTime >= stats.Item1)
                    {
                        if (aims)
                        {
                            fireLasers(_laserTexture, OnTheLine.WallColor, true);
                        }
                        else
                        {
                            fireLasers(_laserTexture, OnTheLine.WallColor, false);
                        }
                        laserElapsedTime = TimeSpan.Zero;
                    }                    
                    for (int i = 0; i < lasers.Count; i++)
                    {
                        Laser laser = lasers[i];
                        lasers[i].Update();
                        if (OnTheLine.screen == (int)Screen.GameScreen && (laser._rect.X > 500 || laser._rect.X < 0 || laser._rect.Y < 0 || laser._rect.Y > 1000))
                        {
                            lasers.Remove(lasers[i]);
                        }
                        if (laser._rect.Intersects(OnTheLine.mouseHitbox.Hitbox))
                        {
                            lasers.Clear();
                            OnTheLine.isLoading = true;
                        }
                    }
                }
            }
            KeyboardState ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.Up))
            {
                Position.Y++;
                foreach (Laser laser in lasers)
                {
                    laser._rect.X += laser._moveX;
                    laser._rect.Y += laser._moveY;
                }
            }
            else if (ks.IsKeyDown(Keys.Down))
            {
                Position.Y--;
                foreach (Laser laser in lasers)
                {
                    laser._rect.X -= laser._moveX;
                    laser._rect.Y -= laser._moveY;
                }
            }
            else if (!OnTheLine.isPaused && !OnTheLine.hasLost)
            {
                Position.Y++;
            }
            Hitbox = new Rectangle((int)Position.X + Texture.Width / 4, (int)Position.Y + Texture.Height / 4, Texture.Width / 2, Texture.Height / 2);
        }
    }

}

