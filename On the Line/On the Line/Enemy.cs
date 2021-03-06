﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace On_the_Line
{
    public class Enemy:Player
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
            Color = OnTheLine.WallColor;
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
                    Player MH = OnTheLine.player;
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
                        laser.Color = Color;
                        if (OnTheLine.screen == Screen.GameScreen && (laser.Hitbox.X > 500 || laser.Hitbox.X < 0 || laser.Hitbox.Y < 0 || laser.Hitbox.Y > 1000))
                        {
                            lasers.Remove(lasers[i]);
                        }
                        if (laser.Hitbox.Intersects(OnTheLine.player.Hitbox) && OnTheLine.screen == Screen.GameScreen)
                        {
                            lasers.Clear();
                            OnTheLine.isLoading = true;
                        }
                    }
                }
            }
            for(int i = 0; i < lasers.Count; i++)
            {
                Laser laser = lasers[i];
                if (laser.Speed.X == 0 ^ laser.Speed.Y == 0)
                {
                    lasers.Remove(laser);
                }
            }
            KeyboardState ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.Up))
            {
                Position.Y++;
                foreach (Laser laser in lasers)
                {
                    laser.Position.X += laser.Speed.X;
                    laser.Position.Y += laser.Speed.Y;
                }
            }
            else if (ks.IsKeyDown(Keys.Down))
            {
                Position.Y--;
                foreach (Laser laser in lasers)
                {
                    laser.Position.X -= laser.Speed.X;
                    laser.Position.Y -= laser.Speed.X;
                }
            }
            else if (!OnTheLine.isPaused && !OnTheLine.hasLost)
            {
                Position.Y++;
                foreach (Laser laser in lasers)
                {
                    laser.Position.Y++;
                }
            }
            Hitbox = new Rectangle((int)Position.X, (int)Position.Y, (int)(Texture.Width * OnTheLine.GlobalScaleFactor), (int)(Texture.Height * OnTheLine.GlobalScaleFactor));
        }
    }
}

