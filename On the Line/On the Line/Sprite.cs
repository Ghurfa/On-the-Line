using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace On_the_Line
{
    class Sprite
    {
        public Rectangle Hitbox;
        public Texture2D Texture;
        public int XSpeed;
        public int YSpeed;
        public Color Color;

        public Sprite(Vector2 position, Texture2D texture, Color color)
        {
            Hitbox = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
            Texture = texture;
            Color = color;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Hitbox, Color);
        }
    }
}
