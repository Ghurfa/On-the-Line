using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace On_the_Line
{
    class Checkbox:Button
    {
        MouseState lastMS;
        public bool isChecked;
        Texture2D _onTexture;
        Texture2D _offTexture;
        public Checkbox(Vector2 position, Texture2D onTexture, Texture2D offTexture, bool defaultPosition)
            :base(position, offTexture)
        {
            isChecked = defaultPosition;
            _onTexture = onTexture;
            _offTexture = offTexture;
        }

        public new void Update()
        {
            Color = OnTheLine.TextColor;
            MouseState MS = Mouse.GetState();
            bool hovered = Hitbox.Contains(MS.X, MS.Y);
            Hitbox = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
            if (hovered)
            {
                Color.A = 128;
                if (MS.LeftButton == ButtonState.Pressed && lastMS.LeftButton == ButtonState.Released)
                {
                    if (isChecked)
                    {
                        isChecked = false;
                        Texture = _offTexture;
                    }
                    else
                    {
                        isChecked = true;
                        Texture = _onTexture;
                    }
                }
            }
            lastMS = MS;
        }
    }
}
