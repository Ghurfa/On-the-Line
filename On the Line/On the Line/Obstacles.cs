using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace On_the_Line
{
    class Obstacles
    {
        private Vector2 position;

        public Vector2 Position
        {
            get { return this.position; }
            set { this.position = value; }
        }


        public Vector2 _startPosition;
        public Vector2 _size;
        public Color _color;
        public Texture2D _texture;
        public Rectangle hitbox;
        public bool _breaks;
        public bool _indestrucable;
        public int _moveX;
        public int _moveY;
        public int _maxMove;
        public int move = 1;
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
        {
            _startPosition = position;
            Position = _startPosition;
            _color = color;
            _size = size;
            _texture = texture;
            hitbox = new Rectangle((int)position.X, (int)position.Y, (int)(_texture.Width * _size.X), (int)(_texture.Height * _size.Y));
            _moveX = moveX;
            _moveY = moveY;
            _maxMove = maxmove;
            _breaks = breaks;
            _indestrucable = indescructable;
            _slideSpeed = 31;
            _gateway = gateway;
            isOuterWall = outerWall;
            Show = false;
        }

        public void Update()
        {
            if (_color.A != 230)
            {
                if (Show)
                {
                    if (isOuterWall)
                    {
                        _color = OnTheLine.OuterWallColor;
                    }
                    else
                    {
                        _color = OnTheLine.WallColor;
                    }                    
                }
                else
                {
                    _color = OnTheLine.BackgroundColor;
                }
            }
            if (OnTheLine.GameMode == "Regular" || OnTheLine.GameMode == "Fastmode")
            {
                if (_moveX != 0 || _moveY != 0)
                {
                    _color = OnTheLine.OuterWallColor;
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
                if ((OnTheLine.GameMode == "Spotlight" && !hitbox.Intersects(OnTheLine.mouseHitbox._hitbox)) || OnTheLine.GameMode == "'")
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
                    if (laser._rect.Intersects(hitbox))
                    {
                        if (OnTheLine.GameMode == "Darkmode" && !Show)
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
                if (OnTheLine.GameMode == "Spotlight")
                {
                    if (hitbox.Intersects(OnTheLine.mouseHitbox.Spotlight))
                    {
                        Show = true;
                    }
                    else
                    {
                        Show = false;
                    }
                }
                if ((OnTheLine.screen == (int)Screen.GameScreen && (position.Y > 930 && _color != OnTheLine.TextColor && _maxMove == 0)) || _gateway)
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
                    this.position.Y += _moveY * move;
                    this.position.X += _moveX * move;
                    if (_moveX > 0)
                    {
                        if (Position.X > _startPosition.X + _moveX * _maxMove || this.position.X < _startPosition.X)
                        {
                            move *= -1;
                        }
                    }
                    else if (_moveX < 0)
                    {
                        if (Position.X < _startPosition.X + _moveX * _maxMove || this.position.X > _startPosition.X)
                        {
                            move *= -1;
                        }
                    }
                    else if (_moveY < 0)
                    {
                        if (this.position.Y < _startPosition.Y + _moveY * _maxMove || this.position.Y > _startPosition.Y)
                        {
                            move *= -1;
                        }
                    }
                    else if (_moveY > 0)
                    {
                        if (this.position.Y > _startPosition.Y + _moveY * _maxMove || this.position.Y < _startPosition.Y)
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
                        _color = reverseColor(OnTheLine.WallColor);
                    }
                }
                else
                {
                    _size += new Vector2(growAmount, growAmount);
                    growAmount += 0.4f;
                    _color = OnTheLine.EndGameColor;
                }
            }
            KeyboardState ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.Up))
            {
                this.position.Y++;
                _startPosition.Y++;
            }
            else if (ks.IsKeyDown(Keys.Down))
            {
                this.position.Y--;
                _startPosition.Y--;
            }
            else if (!OnTheLine.isPaused && !OnTheLine.hasLost)
            {
                this.position.Y++;
                _startPosition.Y++;
            }
            hitbox = new Rectangle((int)this.position.X, (int)this.position.Y, (int)(_texture.Width * 25), (int)(_texture.Height * 25));
        }
        Color reverseColor(Color color)
        {
            return new Color(255 - color.R, 255 - color.G, 255 - color.B);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (_slideSpeed < 30)
            {
                spriteBatch.Draw(_texture, new Vector2(hitbox.X + 12.5f, hitbox.Y + 12.5f), null, _color, rotation, new Vector2(0.5f, 0.5f), _size, SpriteEffects.None, 0);
            }

        }
    }
}
