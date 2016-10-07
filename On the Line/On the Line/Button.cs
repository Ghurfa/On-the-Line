using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace On_the_Line
{
    public class Button
    {
        public Rectangle rectangle;
        public Texture2D _texture;
        MouseState lastMS;
        public bool clicked;
        Color _color = Color.White;
        public Button(Vector2 position, Texture2D texture)
        {
            rectangle = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
            _texture = texture;
        }


        public void Update()
        {
            MouseState MS = Mouse.GetState();
            if (MS.LeftButton == ButtonState.Pressed && lastMS.LeftButton == ButtonState.Released && rectangle.Contains(MS.X, MS.Y))
            {
                clicked = true;
            }
            else
            {
                clicked = false;
            }
            lastMS = MS;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, rectangle, Game1.textColor);
        }
    }
}
