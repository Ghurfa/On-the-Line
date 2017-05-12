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
        public Vector2 Speed;
        public Color Color;
        public Vector2 Position;
        public Vector2 Size;

        public Sprite(Vector2 position, Texture2D texture, Color color)
        {
            Position = position;
            Texture = texture;
            Color = color;
            Size = new Vector2(texture.Width, texture.Height);
        }
        public Vector2 RelativePositon (Vector2 posToCompareAgainst)
        {
            return posToCompareAgainst - Position;
        }
        public void Update()
        {
            Hitbox = new Rectangle((int)Position.X, (int)Position.Y, (int)(Texture.Width * OnTheLine.GlobalScaleFactor), (int)(Texture.Height * OnTheLine.GlobalScaleFactor));
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color, 0, new Vector2(0.5f, 0.5f), OnTheLine.GlobalScaleFactor, SpriteEffects.None, 0);
        }
        public void Draw(SpriteBatch spriteBatch, Vector2 scale)
        {
            spriteBatch.Draw(Texture, Position, null, Color, 0, new Vector2(0, 0), 1 * scale, SpriteEffects.None, 0);
        }
    }
}
