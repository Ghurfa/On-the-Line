using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace On_the_Line
{
    public class Sprite
    {
        public Rectangle Hitbox;
        public Texture2D Texture;
        public int XSpeed;
        public int YSpeed;
        public Color Color;
        public Vector2 Position;

        public Sprite(Vector2 position, Texture2D texture, Color color)
        {
            Position = position;
            Texture = texture;
            Color = color;
        }
        public void Update()
        {
            Hitbox = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Hitbox, Color);
        }
    }
}
