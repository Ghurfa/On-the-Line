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
        bool hovered;
        public bool Clicked;
        public bool Released;
        Color _color = Color.White;
        public Vector2 Position;

        public Button(Vector2 Position, Texture2D texture)
        {
            this.Position = Position;
            _texture = texture;
        }


        public void Update()
        {
            _color = Game1.textColor;
            MouseState MS = Mouse.GetState();
            hovered = rectangle.Contains(MS.X, MS.Y);
            rectangle = new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);
            if (hovered)
            {
                _color.A = 128;
                if (MS.LeftButton == ButtonState.Pressed && lastMS.LeftButton == ButtonState.Released)
                {
                    Clicked = true;
                }
                else
                {
                    Clicked = false;
                }
                if (MS.LeftButton == ButtonState.Released && lastMS.LeftButton == ButtonState.Pressed)
                {
                    Released = true;
                }
                else
                {

                    Released = false;
                }
            }
            else
            {
                Clicked = false;
                Released = false;
            }
            lastMS = MS;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Position, _color);
        }
    }
}
