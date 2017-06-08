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
        public int Selection;
        Rotation rotation;
        public ArrowSelector(Vector2 position, Texture2D increaseTexture, Texture2D decreaseTexture, bool loop, int minValue, int maxValue, int variation, int defaultSelection, Rotation rotation)
            : base(position, decreaseTexture, Color.White)
        {
            this.loop = loop;
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.variation = variation;
            Selection = defaultSelection;
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
        public void Update(Color buttonColor)
        {
            increaseButton.Update(buttonColor);
            decreaseButton.Update(buttonColor);
            if (increaseButton.Clicked)
            {
                if (Selection <= maxValue - variation)
                {
                    Selection += variation;
                }
                else if (loop)
                {
                    Selection = minValue;
                }
            }
            else if (decreaseButton.Clicked)
            {
                if (Selection >= minValue + variation)
                {
                    Selection -= variation;
                }
                else if (loop)
                {
                    Selection = maxValue;
                }
            }
            if (rotation == Rotation.Horizontal)
            {
                decreaseButton.Position = Position;
                increaseButton.Position = Position + new Vector2(decreaseButton.Texture.Width + 50, 0) * OnTheLine.GlobalScaleFactor;
            }
            else
            {
                increaseButton.Position = Position;
                decreaseButton.Position = Position + new Vector2(0, increaseButton.Texture.Height + 50) * OnTheLine.GlobalScaleFactor;
            }
        }
        public new void Draw(SpriteBatch spriteBatch)
        {
            if (Selection + variation <= maxValue || loop)
            {
                increaseButton.Draw(spriteBatch);
            }
            if (Selection - variation >= minValue || loop)
            {
                decreaseButton.Draw(spriteBatch);
            }
            Vector2 textPosition;
            if (rotation == Rotation.Horizontal)
            {
                textPosition = Position + new Vector2(increaseButton.Texture.Width, 0);
            }
            else
            {
                textPosition = Position + new Vector2(10 * OnTheLine.GlobalScaleFactor, increaseButton.Texture.Height + 10 * OnTheLine.GlobalScaleFactor);
            }
            spriteBatch.DrawString(OnTheLine.endGameFont, Selection.ToString(), textPosition, OnTheLine.TextColor);
        }
    }
}
