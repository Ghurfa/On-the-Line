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
        bool shooting = false;
        public Enemy(Vector2 position, Texture2D texture, Texture2D spotlightTexture, Texture2D laserTexture, int shootstyle, int direction, bool doesAim)
        {
            body = new MouseHitbox(Game1.wallColor, texture, spotlightTexture, true, shootstyle, direction);
            body._position = position;
            _laserTexture = laserTexture;
            aims = doesAim;
            if (body._shootStyle == 0)
            {
                laserCooldown = new TimeSpan(0, 0, 0, 2, 0);
            }
            else if (body._shootStyle == 1)
            {
                laserCooldown = new TimeSpan(0, 0, 0, 0, 750);
            }
            else if (body._shootStyle == 2)
            {
                laserCooldown = new TimeSpan(0, 0, 0, 4, 0);
            }
            else if (body._shootStyle == 3)
            {
                laserCooldown = new TimeSpan(0, 0, 0, 0, 900);
            }
            else if (body._shootStyle == 4)
            {
                laserCooldown = new TimeSpan(0, 0, 0, 3, 0);
            }
            else if (body._shootStyle == 5)
            {
                laserCooldown = new TimeSpan(0, 0, 0, 1, 0);
            }

            body.fireLasers(_laserTexture, Game1.wallColor, false);
        }
        public void Update()
        {
            if (laserElapsedTime >= laserCooldown)
            {
                if (aims)
                {
                    body.fireLasers(_laserTexture, Game1.wallColor, true);
                }
                else
                {
                    shooting = true;
                }

                laserElapsedTime = TimeSpan.Zero;
            }
            if (shooting)
            {
                int times = 0;
                body.fireLasers(_laserTexture, Game1.wallColor, false);
                if (body.reloadCycle == 0 && body.slow == 0)
                {
                    times++;
                }
                laserElapsedTime = TimeSpan.Zero;
                if (times == 1)
                {
                    shooting = false;
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
                }
                if (laser._rect.Intersects(Game1.mouseHitbox._hitbox))
                {
                    body.lasers.Clear();
                    Game1.isLoading = true;
                }
            }
            KeyboardState ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.Up))
            {
                body._position.Y++;
                foreach (Laser laser in body.lasers)
                {
                    laser._rect.X += laser._moveX;
                    laser._rect.Y += laser._moveY;
                }
            }
            else if (ks.IsKeyDown(Keys.Down))
            {
                body._position.Y--;
                foreach (Laser laser in body.lasers)
                {
                    laser._rect.X -= laser._moveX;
                    laser._rect.Y -= laser._moveY;
                }
            }
            else if (!Game1.pause && !Game1.lose)
            {
                body._position.Y++;
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            body.Draw(spriteBatch);
        }
    }

}

