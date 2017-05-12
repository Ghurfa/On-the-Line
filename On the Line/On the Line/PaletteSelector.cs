﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace On_the_Line
{
    public class PaletteSelector:Sprite
    {
        List<Button> colorButtons;
        List<Color> buttonColors;
        Sprite background;
        Vector2 buttonSize;
        int rows;
        int columns;
        int margin;
        Color selection;
        public PaletteSelector(List<Color> colors, Vector2 buttonSize, int rows, int columns, int margin, Color backgroundColor, Texture2D pixelTexture, Vector2 position)
            :base(position, pixelTexture, Color.White)
        {
            Vector2 spawnPosition = position;
            buttonColors = new List<Color>();
            colorButtons = new List<Button>();
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
            background = new Sprite(position, pixelTexture, backgroundColor);
            selection = buttonColors[0];
        }
        public new void Update()
        {
            for(int i = 0; i < colorButtons.Count(); i++)
            {
                Button colorButton = colorButtons[i];
                colorButton.Size = buttonSize;
                int column = i % columns;
                int row = (int)(i / columns);
                colorButton.Position = Position + new Vector2(column * buttonSize.X + (column + 1) * margin, row * buttonSize.Y + (row + 1) * margin);
                colorButton.Update(buttonColors[i], false);
                if (colorButton.Clicked) 
                {
                    selection = buttonColors[i];
                }
            }
            background.Position = Position;
            background.Color = selection;
        }
        public Color getColor()
        {
            return selection;
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
