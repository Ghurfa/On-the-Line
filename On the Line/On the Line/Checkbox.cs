using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace On_the_Line
{
    class Checkbox
    {
        Button checkBox;
        public bool isChecked;
        Texture2D _onTexture;
        Texture2D _offTexture;
        public Checkbox(Vector2 position, Texture2D onTexture, Texture2D offTexture, bool defaultPosition)
        {
            checkBox = new Button(position, offTexture);
            isChecked = defaultPosition;
            _onTexture = onTexture;
            _offTexture = offTexture;
        }

        public void Update()
        {
            checkBox.Update();
            if (checkBox.clicked)
            {
                if (isChecked)
                {
                    isChecked = false;
                    checkBox._texture = _offTexture;
                }
                else
                {
                    isChecked = true;
                    checkBox._texture = _onTexture;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            checkBox.Draw(spriteBatch);
        }
    }
}
