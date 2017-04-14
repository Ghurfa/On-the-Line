using System;
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
            Speed = new Vector2(moveX, moveY);
            _lives = lives;
        }
        public new void Update()
        {
            if (!OnTheLine.isPaused)
            {
                Position += Speed;
            }
            Hitbox = new Rectangle((int)Position.X, (int)Position.Y, (int)(Texture.Width * OnTheLine.GlobalScaleFactor), (int)(Texture.Height * OnTheLine.GlobalScaleFactor));
        }
    }
}
