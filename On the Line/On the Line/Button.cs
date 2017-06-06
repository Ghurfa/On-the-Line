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
        public bool Hovered;
        public bool Clicked;

        public Button(Vector2 Position, Texture2D texture)
            : base(Position, texture, Color.White)
        {
        }
        public void Update(Color color, bool turnTranslucent = true)
        {
            Color = color;
            MouseState MS = Mouse.GetState();
            Hovered = Hitbox.Contains(MS.X, MS.Y);
            Hitbox = new Rectangle((int)Position.X, (int)Position.Y, (int)(Size.X * OnTheLine.GlobalScaleFactor), (int)(Size.Y * OnTheLine.GlobalScaleFactor));
            if (Hovered)
            {
                if (turnTranslucent)
                {
                    Color.A = 128;
                }
                if (MS.LeftButton == ButtonState.Released && lastMS.LeftButton == ButtonState.Pressed)
                {
                    Clicked = true;
                }
                else
                {
                    Clicked = false;
                }
            }
            else
            {
                Clicked = false;
            }
            lastMS = MS;
        }
    }
}
