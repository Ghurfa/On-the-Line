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
        public Vector2 _startPosition;
        public Vector2 _size;
        public bool _breaks;
        public bool _indestrucable;
        public int MoveX;
        public int MoveY;
        public int MaxMove;
        int move = 1;
        public bool _gateway;
        int slow = 0;
        public int _slideSpeed;
        float growAmount = -0.5f;
        public bool didKill = false;
        float rotation = 0;
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
        public Obstacles(Texture2D texture, Vector2 position, Vector2 size, Color color, bool breaks, int moveX, int moveY, int maxmove, bool indescructable, bool gateway, bool outerWall = false)
            :base(position, texture, color)
        {
            _startPosition = position;
            _size = size;
            Hitbox = new Rectangle((int)position.X, (int)position.Y, (int)(Texture.Width * _size.X), (int)(Texture.Height * _size.Y));
            MoveX = moveX;
            MoveY = moveY;
            MaxMove = maxmove;
            _breaks = breaks;
            _indestrucable = indescructable;
            _slideSpeed = 31;
            _gateway = gateway;
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
                if (MoveX != 0 || MoveY != 0)
                {
                    Color = OnTheLine.OuterWallColor;
                }
                else
                {
                    Show = true;
                }
            }
            if (_slideSpeed > 0)
            {
                if (_slideSpeed == 31)
                {
                    Position += new Vector2(496, 0);
                }
                Position -= new Vector2(_slideSpeed, 0);
                if ((OnTheLine.gameMode == GameMode.Spotlight && !Hitbox.Intersects(OnTheLine.mouseHitbox.Hitbox)))
                {
                    Show = false;
                }
                _slideSpeed--;
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
                        if (OnTheLine.mouseHitbox.lasers[i]._lives <= 0 || _indestrucable)
                        {
                            OnTheLine.mouseHitbox.lasers.Remove(laser);
                        }
                    }
                }
                if (OnTheLine.gameMode == GameMode.Spotlight)
                {
                    if (Hitbox.Intersects(OnTheLine.mouseHitbox.Spotlight))
                    {
                        Show = true;
                    }
                    else
                    {
                        Show = false;
                    }
                }
                if ((OnTheLine.screen == Screen.GameScreen && (Position.Y > 930 && Color != OnTheLine.TextColor && MaxMove == 0)) || _gateway)
                {
                    Show = false;
                }
                slow++;
                if (slow == 5)
                {
                    slow = 0;
                }
                if ((slow == 0 && !OnTheLine.isPaused) || (slow == 0 && OnTheLine.hasLost == true))
                {
                    Position.Y += MoveY * move;
                    Position.X += MoveX * move;
                    if (MoveX > 0)
                    {
                        if (Position.X > _startPosition.X + MoveX * MaxMove || Position.X < _startPosition.X)
                        {
                            move *= -1;
                        }
                    }
                    else if (MoveX < 0)
                    {
                        if (Position.X < _startPosition.X + MoveX * MaxMove || Position.X > _startPosition.X)
                        {
                            move *= -1;
                        }
                    }
                    else if (MoveY < 0)
                    {
                        if (Position.Y < _startPosition.Y + MoveY * MaxMove || Position.Y > _startPosition.Y)
                        {
                            move *= -1;
                        }
                    }
                    else if (MoveY > 0)
                    {
                        if (Position.Y > _startPosition.Y + MoveY * MaxMove || Position.Y < _startPosition.Y)
                        {
                            move *= -1;
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
                    _size += new Vector2(growAmount, growAmount);
                    growAmount += 0.4f;
                    Color = OnTheLine.EndGameColor;
                }
            }
            KeyboardState ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.Up))
            {
                Position.Y++;
                _startPosition.Y++;
            }
            else if (ks.IsKeyDown(Keys.Down))
            {
                Position.Y--;
                _startPosition.Y--;
            }
            else if (!OnTheLine.isPaused && !OnTheLine.hasLost)
            {
                Position.Y++;
                _startPosition.Y++;
            }
            Hitbox = new Rectangle((int)Position.X, (int)Position.Y, (int)(Texture.Width * 25), (int)(Texture.Height * 25));
        }
        Color reverseColor(Color color)
        {
            return new Color(255 - color.R, 255 - color.G, 255 - color.B);
        }
        public new void Draw(SpriteBatch spriteBatch)
        {
            if (_slideSpeed < 30)
            {
                spriteBatch.Draw(Texture, new Vector2(Hitbox.X + 12.5f, Hitbox.Y + 12.5f), null, Color, rotation, new Vector2(0.5f, 0.5f), _size, SpriteEffects.None, 0);
            }

        }
    }
}
