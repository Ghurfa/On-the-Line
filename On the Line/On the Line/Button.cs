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
        int YSpeed;
        public Vector2 EndPosition;

        public Button(Vector2 endPosition, Texture2D texture)
        {
            YSpeed = 200;
            this.EndPosition = endPosition;
            rectangle = new Rectangle((int)endPosition.X, (int)endPosition.Y - 300, texture.Width, texture.Height);
            _texture = texture;
        }


        public void Update()
        {
            _color = Game1.textColor;
            if (Math.Abs(YSpeed) > 1)
            {
                rectangle.Y += (YSpeed / 10);
                if (rectangle.Y < EndPosition.Y)
                {
                    YSpeed = YSpeed * 20 / 21;
                }
                else
                {
                    YSpeed--;
                }
                if (rectangle.Y >= EndPosition.Y)
                {
                    YSpeed *= -1;
                }

            }
            MouseState MS = Mouse.GetState();
            hovered = rectangle.Contains(MS.X, MS.Y);
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
            spriteBatch.Draw(_texture, rectangle, _color);
        }
    }
}
