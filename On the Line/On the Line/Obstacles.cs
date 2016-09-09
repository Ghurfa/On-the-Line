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
        int slow = 0;
        public int _slideSpeed;
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
        public Obstacles(Texture2D texture, Vector2 position, Vector2 size, Color color, bool breaks, int moveX, int moveY, int maxmove, bool indescructable, int slideSpeed)
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
            _slideSpeed = slideSpeed;
        }

        public void Update()
        {
            if (_slideSpeed > 0)
            {
                    if (_slideSpeed == 31)//random number
                    {
                        Position += new Vector2(496, 0);
                    }
                    Position -= new Vector2(_slideSpeed, 0);
                
                _slideSpeed--;
            }
            else
            {
                if (_color != Game1.textColor)
                {
                    if (Game1.gamemode == "darkmode")
                    {
                        if (Game1.lose)
                        {
                            _color = Game1.wallColor;
                        }
                        for (int i = 0; i < Game1.mouseHitbox.lasers.Count; i++)
                        {
                            Laser laser = Game1.mouseHitbox.lasers[i];
                            if (hitbox.Intersects(laser._rect) && _color != Game1.wallColor)
                            {
                                if (_indestrucable)
                                {
                                    Game1.mouseHitbox.lasers.Remove(laser);
                                }
                                Game1.mouseHitbox.lasers[i]._lives--;
                                if (Game1.mouseHitbox.lasers[i]._lives <= 0)
                                {
                                    Game1.mouseHitbox.lasers.Remove(laser);
                                }
                                _color = Game1.wallColor;
                            }
                        }
                    }
                    else if (Game1.gamemode == "spotlight")
                    {
                        if (Game1.lose)
                        {
                            _color = Game1.wallColor;
                        }
                        else
                        {
                            if (hitbox.Intersects(Game1.mouseHitbox._spotlight))
                            {
                                _color = Game1.wallColor;
                            }
                            else
                            {
                                _color = Game1.backgroundColor;
                            }
                        }
                    }
                }
                for (int i = 0; i < Game1.mouseHitbox.lasers.Count; i++)
                {
                    Laser laser = Game1.mouseHitbox.lasers[i];
                    if (laser._rect.Intersects(hitbox) && _indestrucable)
                    {
                        Game1.mouseHitbox.lasers.Remove(laser);
                    }
                }
                slow++;
                if (slow == 5)
                {
                    slow = 0;
                }
                if ((slow == 0 && !Game1.pause) || (slow == 0 && Game1.lose == true))
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
            else if (!Game1.pause && !Game1.lose)
            {
                this.position.Y++;
                _startPosition.Y++;
            }
            hitbox = new Rectangle((int)this.position.X, (int)this.position.Y, (int)(_texture.Width * _size.X), (int)(_texture.Height * _size.Y));
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (_slideSpeed < 30)
            {
                if(position.Y > 930 && _color != Game1.textColor)
                {
                    _color = Game1.backgroundColor;
                }
                spriteBatch.Draw(_texture, hitbox, _color);
            }

        }
    }
}
