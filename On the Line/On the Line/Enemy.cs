using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace On_the_Line
{
    public class Enemy
    {
        public MouseHitbox body;
        public static TimeSpan laserCooldown;
        public TimeSpan laserElapsedTime;
        Texture2D _laserTexture;
        int slow = 0;
        bool aims;
        public Enemy(Vector2 position, Texture2D texture, Texture2D spotlightTexture, Texture2D laserTexture, int shootstyle, int direction, bool doesAim)
        {
            body = new MouseHitbox(Game1.wallColor, texture, spotlightTexture, shootstyle, direction);
            body._position = position;
            _laserTexture = laserTexture;
            aims = doesAim;
            if (body._shootStyle == 0)
            {
                laserCooldown = new TimeSpan(0, 0, 0, 2, 0);
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
        }
        public void Update()
        {
            if (laserElapsedTime >= laserCooldown)
            {
                laserElapsedTime = TimeSpan.Zero;
                if (aims)
                {
                    body.fireLasers(_laserTexture, Game1.wallColor, true);
                }
                else
                {
                    body.fireLasers(_laserTexture, Game1.wallColor, false);
                    Random random = new Random();
                    TimeSpan addTimeSpan = new TimeSpan(0, 0, 0, 0, 100 * (random.Next(1, 4) - 2));
                    laserCooldown = laserCooldown + addTimeSpan;
                }
            }
            body._hitbox = new Rectangle((int)body._position.X + body._texture.Width / 4, (int)body._position.Y + body._texture.Height / 4, body._texture.Width / 2, body._texture.Height / 2);
            for (int i = 0; i < body.lasers.Count; i++)
            {
                Laser laser = body.lasers[i];
                body.lasers[i].Update();
                if (Game1.screen == 1 && (laser._rect.X > 500 || laser._rect.X < 0 || laser._rect.Y < 0 || laser._rect.Y > 1000))
                {
                    body.lasers.Remove(body.lasers[i]);
                    //i--;
                }
                if (laser._rect.Intersects(Game1.mouseHitbox._hitbox))
                {
                    body.lasers.Clear();
                    Game1.isLoading = true;
                }
            }
            slow++;
            if (slow == 5)
            {
                slow = 0;
            }
            KeyboardState ks = Keyboard.GetState();
            if ((ks.IsKeyDown(Keys.Up)))
            {
                body._position.Y++;
                if (ks.IsKeyDown(Keys.RightControl))
                {
                    body._hitbox.Y++;
                }
                if (ks.IsKeyDown(Keys.LeftControl))
                {
                    body._hitbox.Y += 6;
                }

                foreach (Laser laser in body.lasers)
                {
                    laser._rect.Y++;
                }
            }
            else if (ks.IsKeyDown(Keys.Down))
            {
                body._position.Y--;
                if (ks.IsKeyDown(Keys.RightControl))
                {
                    body._hitbox.Y--;
                }
                if (ks.IsKeyDown(Keys.LeftControl))
                {
                    body._hitbox.Y -= 6;
                }
                foreach (Laser laser in body.lasers)
                {
                    laser._rect.Y--;
                }
            }
            else if (!Game1.pause && !Game1.lose)
            {
                body._position.Y++;
                if (ks.IsKeyDown(Keys.RightControl))
                {
                    body._hitbox.Y++;
                }
                if (ks.IsKeyDown(Keys.LeftControl))
                {
                    body._hitbox.Y += 6;
                }

                foreach (Laser laser in body.lasers)
                {
                    laser._rect.Y++;
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            body.Draw(spriteBatch);
        }
    }
}
