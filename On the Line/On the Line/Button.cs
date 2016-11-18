using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace On_the_Line
{
    public class Button : Sprite
    {
        MouseState lastMS;
        bool hovered;
        public bool Clicked;
        public bool Released;

        public Button(Vector2 Position, Texture2D texture)
            : base(Position, texture, Color.White)
        {
        }
        public new void Update()
        {
            Color = OnTheLine.TextColor;
            MouseState MS = Mouse.GetState();
            hovered = Hitbox.Contains(MS.X, MS.Y);
            Hitbox = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
            if (hovered)
            {
                Color.A = 128;
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
    }
}
