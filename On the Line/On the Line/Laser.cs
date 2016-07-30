using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace On_the_Line
{
    public class Laser
    {
        int _moveX;
        int _moveY;
        public Rectangle _rect;
        Texture2D _texture;
        public int _lives;
        Color _color;
        public Laser(Vector2 startPos, int moveX, int moveY, Texture2D texture, int lives, Color color)
        {
            _moveX = moveX;
            _moveY = moveY;
            _texture = texture;
            _rect = new Rectangle((int) startPos.X, (int)startPos.Y, texture.Width, texture.Height);
            _lives = lives;
            _color = color;
        }
        public void Update()
        {
            if (!Game1.pause)
            {
                _rect.X += _moveX;
                _rect.Y += _moveY;
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (Game1.lose == false)
            {
                spriteBatch.Draw(_texture, _rect, _color);
            }
        }
    }
}
