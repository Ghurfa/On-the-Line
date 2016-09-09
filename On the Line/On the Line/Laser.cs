using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace On_the_Line
{
    public class Laser
    {
        public int _moveX;
        public int _moveY;
        public Rectangle _rect;
        Texture2D _texture;
        public int _lives;
        Color _color;
        public Laser(Vector2 startPos, int moveX, int moveY, Texture2D texture, int lives, Color color)
        {
            _moveX = moveX;
            _moveY = moveY;
            _texture = texture;
            _rect = new Rectangle((int)startPos.X, (int)startPos.Y, texture.Width, texture.Height);
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
            KeyboardState ks = new KeyboardState();
            if (ks.IsKeyDown(Keys.Up))
            {
                _rect.Y++;
                if (ks.IsKeyDown(Keys.RightControl))
                {
                    _rect.Y++;
                }
                if (ks.IsKeyDown(Keys.LeftControl))
                {
                    _rect.Y += 6;
                }
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
