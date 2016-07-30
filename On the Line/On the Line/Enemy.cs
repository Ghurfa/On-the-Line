using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace On_the_Line
{
    class Enemy
    {
        MouseHitbox body;
        public static TimeSpan laserCooldown;
        public TimeSpan laserElapsedTime;
        Texture2D _laserTexture;
        int slow = 0;
        public Enemy(Vector2 position, Texture2D texture, Texture2D spotlightTexture, Texture2D laserTexture, int shootstyle, int direction)
        {
            body = new MouseHitbox(Game1.wallColor, texture, spotlightTexture, shootstyle, direction);
            body._position = position;
            _laserTexture = laserTexture;
        }
        public void Update()
        {
            if (body._shootStyle == 0)
            {
                laserCooldown = new TimeSpan(0, 0, 0, 1, 0);
            }
            else if (body._shootStyle == 1)
            {
                laserCooldown = new TimeSpan(0, 0, 0, 1, 500);
            }
            else if (body._shootStyle == 2)
            {
                laserCooldown = new TimeSpan(0, 0, 0, 4, 0);
            }
            else if (body._shootStyle == 3)
            {
                laserCooldown = new TimeSpan(0, 0, 0, 0, 900);
            }
            if (laserElapsedTime >= laserCooldown)
            {
                laserElapsedTime = TimeSpan.Zero;
                body.fireLasers(_laserTexture, Game1.wallColor);
            }
            for (int i = 0; i < body.lasers.Count; i++)
            {
                body.lasers[i].Update();
                if (Game1.screen == 1 && (body.lasers[i]._rect.X > 500 || body.lasers[i]._rect.X < 0 || body.lasers[i]._rect.Y < 0 || body.lasers[i]._rect.Y > 1000))
                {
                   body.lasers.Remove(body.lasers[i]);
                   i--;
                }
            }
            slow++;
            if (slow == 5)
            {
                slow = 0;
            }
            KeyboardState ks = Keyboard.GetState();
            if ((ks.IsKeyDown(Keys.Up)) || (!Game1.pause && !Game1.lose))
            {
                body._position.Y++;
                foreach(Laser laser in body.lasers)
                {
                    laser._rect.Y++;
                }
            }
            else if (ks.IsKeyDown(Keys.Down))
            {
                body._position.Y--;
                foreach (Laser laser in body.lasers)
                {
                    laser._rect.Y--;
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            body.Draw(spriteBatch);
        }
    }
}
