﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace On_the_Line
{
    public class Laser:Sprite
    {
        public int _lives;
        public Laser(Vector2 startPos, int moveX, int moveY, Texture2D texture, int lives, Color color)
            :base(startPos, texture, color)
        {
            XSpeed = moveX;
            YSpeed = moveY;
            _lives = lives;
        }
        public new void Update()
        {
            if (!OnTheLine.isPaused)
            {
                Position.X += XSpeed;
                Position.Y += YSpeed;
            }
            KeyboardState ks = new KeyboardState();
            if (ks.IsKeyDown(Keys.Up))
            {
                Position.Y++;
                if (ks.IsKeyDown(Keys.RightControl))
                {
                    Position.Y++;
                }
                if (ks.IsKeyDown(Keys.LeftControl))
                {
                    Position.Y += 6;
                }
            }
            Hitbox = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
        }
    }
}
