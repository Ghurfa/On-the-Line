using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace On_the_Line
{
    class Obstacles:Sprite
    {
        public Vector2 StartPosition;
        public Vector2 Size;
        public bool Breaks;
        public bool Indestrucable;
        public int MaxMove;
        int moveDirection = 1;
        public bool Gateway;
        int slow = 0;
        public int SlideSpeed;
        float growAmount = -0.5f;
        public bool didKill = false;
        int growTimes;
        public bool Show = true;
        bool isOuterWall;
        /// <summary>
        /// Loads an obstacle
        /// </summary>
        /// <param name="texture">The texture of the obstacle</param>
        /// <param name="position">The position of the obstacle</param>
        /// <param name="size">The size of the obstacle</param>
        /// <param name="color">The color of the obstacle</param>
        /// <param name="breaks">Whether or not the obstacle breaks</param>
        /// <param name="moveX">How much the obstacle moves in the X axis</param>
        /// <param name="moveY">How much the obstacle moves in the Y axis</param>
        /// <param name="maxmove">The maximum amount of times the obstacle moves</param>
        /// <param name="indescructable">Whether or not the obstacle absorbs a laser</param>
        public Obstacles(Texture2D texture, Vector2 position, Vector2 size, Color color, bool breaks, Vector2 speed, int maxmove, bool indescructable, bool gateway, bool outerWall = false)
            :base(position, texture, color)
        {
            StartPosition = position;
            Size = size;
            Hitbox = new Rectangle((int)position.X, (int)position.Y, (int)(Texture.Width * Size.X), (int)(Texture.Height * Size.Y));
            Speed = speed;
            MaxMove = maxmove;
            Breaks = breaks;
            Indestrucable = indescructable;
            SlideSpeed = 31;
            Gateway = gateway;
            isOuterWall = outerWall;
            Show = false;
        }

        public new void Update()
        {
            if (Color.A != 230)
            {
                if (Show)
                {
                    if (isOuterWall)
                    {
                        Color = OnTheLine.OuterWallColor;
                    }
                    else
                    {
                        Color = OnTheLine.WallColor;
                    }                    
                }
                else
                {
                    Color = OnTheLine.BackgroundColor;
                }
            }
            if (OnTheLine.gameMode == GameMode.Regular || OnTheLine.gameMode == GameMode.Fastmode)
            {
                if (Speed.X != 0 || Speed.Y != 0)
                {
                    Color = OnTheLine.OuterWallColor;
                }
                else
                {
                    Show = true;
                }
            }
            if (SlideSpeed > 0)
            {
                if (SlideSpeed == 31)
                {
                    Position += new Vector2(496, 0);
                }
                Position -= new Vector2(SlideSpeed, 0);
                if ((OnTheLine.gameMode == GameMode.Spotlight && !Hitbox.Intersects(OnTheLine.mouseHitbox.Hitbox)))
                {
                    Show = false;
                }
                SlideSpeed--;
            }
            else
            {
                if (OnTheLine.hasLost)
                {
                    Show = true;
                }
                for (int i = 0; i < OnTheLine.mouseHitbox.lasers.Count; i++)
                {
                    Laser laser = OnTheLine.mouseHitbox.lasers[i];
                    if (laser.Hitbox.Intersects(Hitbox))
                    {
                        if (OnTheLine.gameMode == GameMode.Darkmode && !Show)
                        {
                            OnTheLine.mouseHitbox.lasers[i]._lives--;
                            Show = true;
                        }
                        if (OnTheLine.mouseHitbox.lasers[i]._lives <= 0 || Indestrucable)
                        {
                            OnTheLine.mouseHitbox.lasers.Remove(laser);
                        }
                    }
                }
                if (OnTheLine.gameMode == GameMode.Spotlight)
                {
                    if (Hitbox.Intersects(OnTheLine.mouseHitbox.Spotlight) || OnTheLine.hasLost)
                    {
                        Show = true;
                    }
                    else
                    {
                        Show = false;
                    }
                }
                if (Gateway)
                {
                    Show = false;
                }
                slow++;
                if (slow == 5)
                {
                    slow = 0;
                }
                if (slow == 0 && !OnTheLine.isPaused && OnTheLine.hasLost == false)
                {
                    Position.X += Speed.X * moveDirection * OnTheLine.GlobalScaleFactor;
                    Position.Y += Speed.Y * moveDirection * OnTheLine.GlobalScaleFactor;
                    if (Speed.X > 0)
                    {
                        if (Position.X > StartPosition.X + Speed.X * MaxMove || Position.X < StartPosition.X)
                        {
                            moveDirection *= -1;
                        }
                    }
                    else if (Speed.X < 0)
                    {
                        if (Position.X < StartPosition.X + Speed.X * MaxMove || Position.X > StartPosition.X)
                        {
                            moveDirection *= -1;
                        }
                    }
                    else if (Speed.Y < 0)
                    {
                        if (Position.Y < StartPosition.Y + Speed.Y * MaxMove || Position.Y > StartPosition.Y)
                        {
                            moveDirection *= -1;
                        }
                    }
                    else if (Speed.Y > 0)
                    {
                        if (Position.Y > StartPosition.Y + Speed.Y * MaxMove || Position.Y < StartPosition.Y)
                        {
                            moveDirection *= -1;
                        }
                    }
                }
            }
            if (didKill)
            {
                growTimes++;
                if (growTimes < 100)
                {
                    if (growTimes % 20 >= 0 && growTimes % 20 <= 10)
                    {
                        Color = reverseColor(OnTheLine.WallColor);
                    }
                }
                else
                {
                    Size += new Vector2(growAmount, growAmount);
                    growAmount += 0.4f;
                    Color = OnTheLine.EndGameColor;
                }
            }
            KeyboardState ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.Up))
            {
                Position.Y++;
                StartPosition.Y++;
            }
            else if (ks.IsKeyDown(Keys.Down))
            {
                Position.Y--;
                StartPosition.Y--;
            }
            else if (!OnTheLine.isPaused && !OnTheLine.hasLost)
            {
                Position.Y += OnTheLine.GlobalScaleFactor;
                StartPosition.Y += OnTheLine.GlobalScaleFactor;
            }
            Hitbox = new Rectangle((int)Position.X, (int)Position.Y, (int)(Texture.Width * 25 * OnTheLine.GlobalScaleFactor), (int)(Texture.Height * 25 * OnTheLine.GlobalScaleFactor));
        }
        Color reverseColor(Color color)
        {
            return new Color(255 - color.R, 255 - color.G, 255 - color.B);
        }
        public new void Draw(SpriteBatch spriteBatch)
        {
            if (SlideSpeed < 30)
            {
                spriteBatch.Draw(Texture, Position + new Vector2(12.5f, 12.5f), null, Color, 0, new Vector2(0.5f, 0.5f), Size, SpriteEffects.None, 0);
            }

        }
    }
}
