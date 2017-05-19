using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace On_the_Line
{
    public class ArrowSelector : Sprite
    {
        public enum Rotation
        {
            Vertical,
            Horizontal
        }
        Button decreaseButton;
        Button increaseButton;
        bool loop;
        int minValue;
        int maxValue;
        int variation;
        int selection;
        Rotation rotation;
        public ArrowSelector(Vector2 position, Texture2D decreaseTexture, Texture2D increaseTexture, bool loop, int minValue, int maxValue, int variation, int defaultSelection, Rotation rotation)
            : base(position, decreaseTexture, Color.White)
        {
            this.loop = loop;
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.variation = variation;
            selection = defaultSelection;
            increaseButton = new Button(position, increaseTexture);
            this.rotation = rotation;
            if (rotation == Rotation.Horizontal)
            {
                decreaseButton = new Button(position + new Vector2(increaseTexture.Width + 50, 0), decreaseTexture);
            }
            else
            {
                decreaseButton = new Button(position + new Vector2(0, increaseTexture.Height + 50), decreaseTexture);
            }
        }
        public new void Update()
        {
            increaseButton.Update(Color.White);
            decreaseButton.Update(Color.White);
            if (increaseButton.Clicked)
            {
                if (selection <= maxValue - variation)
                {
                    selection += variation;
                }
                else if (loop)
                {
                    selection = minValue;
                }
            }
            else if (decreaseButton.Clicked)
            {
                if (selection >= minValue + variation)
                {
                    selection -= variation;
                }
                else if (loop)
                {
                    selection = maxValue;
                }
            }
        }
        public new void Draw(SpriteBatch spriteBatch)
        {
            increaseButton.Draw(spriteBatch);
            decreaseButton.Draw(spriteBatch);
            Vector2 textPosition;
            if (rotation == Rotation.Horizontal)
            {
                textPosition = Position + new Vector2(increaseButton.Texture.Width, 0);
            }
            else
            {
                textPosition = Position + new Vector2(0, increaseButton.Texture.Height);
            }
            spriteBatch.DrawString(OnTheLine.endGameFont, selection.ToString(), textPosition, OnTheLine.TextColor);
        }
    }
}
