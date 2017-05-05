using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace On_the_Line
{
    public class PaletteSelector:Sprite
    {
        List<Button> colorButtons = new List<Button>();
        List<Color> buttonColors = new List<Color>();
        Sprite background;
        Vector2 buttonSize;
        int rows;
        int columns;
        int margin;
        Color backgroundColor;
        public PaletteSelector(List<Color> colors, Vector2 buttonSize, int rows, int columns, int margin, Color backgroundColor, Texture2D pixelTexture, Vector2 position)
            :base(position, pixelTexture, Color.White)
        {
            Vector2 spawnPosition = position;
            for (int x = 0; x < columns; x++)
            {
                spawnPosition.Y = position.Y + margin;
                spawnPosition.X += margin;
                for (int y = 0; y < rows; y++)
                {
                    colorButtons.Add(new Button(spawnPosition, pixelTexture));
                    spawnPosition.Y += buttonSize.Y + margin;
                }
                spawnPosition.X += buttonSize.X;
            }
            buttonColors = colors;
            this.buttonSize = buttonSize;
            this.rows = rows;
            this.columns = columns; 
            this.margin = margin;
            this.backgroundColor = backgroundColor;
            background = new Sprite(position, pixelTexture, backgroundColor);
        }
        public void GetColor()
        {
            for(int i = 0; i < colorButtons.Count(); i++)
            {
                Button colorButton = colorButtons[i];
                colorButton.Update(buttonColors[i]);
            }
        }
        public new void Draw(SpriteBatch spriteBatch)
        {
            background.Draw(spriteBatch, new Vector2(buttonSize.X * columns + margin * (columns + 1), buttonSize.Y * rows + margin * (rows + 1)) * OnTheLine.GlobalScaleFactor);
            foreach (Button colorButton in colorButtons)
            {
                colorButton.Draw(spriteBatch, buttonSize * OnTheLine.GlobalScaleFactor);
            }
        }
    }
}
