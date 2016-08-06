using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace On_the_Line
{
    public class MouseHitbox
    {
        public Vector2 _position;
        public Color _color;
        MouseState mouseState;
        MouseState lastMouseState;
        public Texture2D _texture;
        Texture2D _spotlightTexture;
        public Rectangle _hitbox;
        public Rectangle _spotlight;
        int _direction = 0;
        public int _shootStyle;
        bool isclicked = false;
        public List<Laser> lasers = new List<Laser>();
        bool showWhenLose;
        public MouseHitbox(Color color, Texture2D texture, Texture2D spotlightTexure, int shootStyle = 0, int direction = 0)
        {
            _color = color;
            _texture = texture;
            _hitbox = new Rectangle((int)_position.X, (int)_position.Y, _texture.Width, _texture.Height);
            _spotlightTexture = spotlightTexure;
            _shootStyle = shootStyle;
            _direction = direction;
        }
        public void Update()
        {
            _shootStyle = Game1.shootStyle;
            if (_shootStyle == 0)
            {
                Game1.laserCooldown = new TimeSpan(0, 0, 0, 1, 0);
            }
            else if (_shootStyle == 1)
            {                
                Game1.laserCooldown = new TimeSpan(0, 0, 0, 1, 500);
            }
            else if (_shootStyle == 2)
            {
                Game1.laserCooldown = new TimeSpan(0, 0, 0, 4, 0);
            }
            else if (_shootStyle == 3)
            {
                Game1.laserCooldown = new TimeSpan(0, 0, 0, 0, 900);
            }
            _spotlight = new Rectangle((int)_position.X + _texture.Width/2 - 100, (int)_position.Y + _texture.Height/2 - 100, 200, 200);
            for(int i = 0; i < lasers.Count; i++)
            {
                lasers[i].Update();
                if ((Game1.screen == 1 && (lasers[i]._rect.X > 500 || lasers[i]._rect.X < 0 || lasers[i]._rect.Y < 0 || lasers[i]._rect.Y > 1000) || Game1.lose)||Game1.screen == 2 && !lasers[i]._rect.Intersects(Game1.shootStyleButton.rectangle))
                {
                    lasers.Remove(lasers[i]);
                    i--;
                }
            }
            mouseState = Mouse.GetState();
            if (Game1.screen != 2)
            {
                if (mouseState.LeftButton == ButtonState.Pressed && _hitbox.Contains(mouseState.X, mouseState.Y))
                {
                    isclicked = true;
                    Game1.pause = false;
                }
                if (mouseState.LeftButton == ButtonState.Released)
                {
                    isclicked = false;
                    Game1.pause = true;
                }
                if (isclicked)
                {
                    KeyboardState ks = Keyboard.GetState();
                    if (ks.IsKeyDown(Keys.LeftShift))
                    {
                        _position.Y++;
                        if(Game1.gamemode == "fastmode")
                        {
                            _position.Y++;
                        }
                    }
                    else
                    {
                        _position.Y = mouseState.Y - (_texture.Height / 2);
                        _position.X = mouseState.X - (_texture.Width / 2);
                    }
                }
            }
            _hitbox = new Rectangle((int)_position.X + _texture.Width/4, (int)_position.Y + _texture.Height/4, _texture.Width/2, _texture.Height/2);
            lastMouseState = mouseState;
        }
        public void fireLasers(Texture2D texture, Color laserColor)
        {
            Vector2 startPos = new Vector2(_position.X + _texture.Width / 2, _position.Y + _texture.Height / 2);
            if (lasers.Count < 500)
            {
                if (_shootStyle == 0)
                {
                    int laserLives = 7;
                    if (_direction == 0)
                    {
                        lasers.Add(new Laser(startPos, 0, -2, texture, laserLives, laserColor));
                        lasers.Add(new Laser(startPos, 2, -2, texture, laserLives, laserColor));
                        lasers.Add(new Laser(startPos, -2, -2, texture, laserLives, laserColor));
                    }
                    else if (_direction == 1)
                    {
                        lasers.Add(new Laser(startPos, -2, 0, texture, laserLives, laserColor));
                        lasers.Add(new Laser(startPos, -2, 2, texture, laserLives, laserColor));
                        lasers.Add(new Laser(startPos, -2, -2, texture, laserLives, laserColor));
                    }
                    else if (_direction == 2)
                    {
                        lasers.Add(new Laser(startPos, 0, 2, texture, laserLives, laserColor));
                        lasers.Add(new Laser(startPos, 2, 2, texture, laserLives, laserColor));
                        lasers.Add(new Laser(startPos, -2, 2, texture, laserLives, laserColor));
                    }
                    else if (_direction == 3)
                    {
                        lasers.Add(new Laser(startPos, 2, 0, texture, laserLives, laserColor));
                        lasers.Add(new Laser(startPos, 2, 2, texture, laserLives, laserColor));
                        lasers.Add(new Laser(startPos, 2, -2, texture, laserLives, laserColor));
                    }
                }
                else if (_shootStyle == 1)
                {
                    int laserLives = 5;
                    lasers.Add(new Laser(startPos, 0, 2, texture, laserLives, laserColor));
                    lasers.Add(new Laser(startPos, 2, 2, texture, laserLives, laserColor));
                    lasers.Add(new Laser(startPos, 2, 0, texture, laserLives, laserColor));
                    lasers.Add(new Laser(startPos, 2, -2, texture, laserLives, laserColor));
                    lasers.Add(new Laser(startPos, 0, -2, texture, laserLives, laserColor));
                    lasers.Add(new Laser(startPos, -2, -2, texture, laserLives, laserColor));
                    lasers.Add(new Laser(startPos, -2, 0, texture, laserLives, laserColor));
                    lasers.Add(new Laser(startPos, -2, 2, texture, laserLives, laserColor));
                }
                else if (_shootStyle == 2)
                {
                    int laserLives = 2;
                    lasers.Add(new Laser(startPos, -1, 0, texture, laserLives, laserColor));
                    lasers.Add(new Laser(startPos, -1, -1, texture, laserLives, laserColor));
                    lasers.Add(new Laser(startPos, -1, -2, texture, laserLives, laserColor));
                    lasers.Add(new Laser(startPos, -1, -3, texture, laserLives, laserColor));
                    lasers.Add(new Laser(startPos, -1, -4, texture, laserLives, laserColor));
                    lasers.Add(new Laser(startPos, -1, -5, texture, laserLives, laserColor));
                    lasers.Add(new Laser(startPos, 0, -1, texture, laserLives, laserColor));
                    lasers.Add(new Laser(startPos, 1, 0, texture, laserLives, laserColor));
                    lasers.Add(new Laser(startPos, 1, -1, texture, laserLives, laserColor));
                    lasers.Add(new Laser(startPos, 1, -2, texture, laserLives, laserColor));
                    lasers.Add(new Laser(startPos, 1, -3, texture, laserLives, laserColor));
                    lasers.Add(new Laser(startPos, 1, -4, texture, laserLives, laserColor));
                    lasers.Add(new Laser(startPos, 1, -5, texture, laserLives, laserColor));
                    lasers.Add(new Laser(startPos, -2, -1, texture, laserLives, laserColor));
                    lasers.Add(new Laser(startPos, -2, -3, texture, laserLives, laserColor));
                    lasers.Add(new Laser(startPos, -2, -4, texture, laserLives, laserColor));
                    lasers.Add(new Laser(startPos, -2, -5, texture, laserLives, laserColor));
                    lasers.Add(new Laser(startPos, 2, -1, texture, laserLives, laserColor));
                    lasers.Add(new Laser(startPos, 2, -3, texture, laserLives, laserColor));
                    lasers.Add(new Laser(startPos, 2, -4, texture, laserLives, laserColor));
                    lasers.Add(new Laser(startPos, 2, -5, texture, laserLives, laserColor));
                    lasers.Add(new Laser(startPos, -3, -1, texture, laserLives, laserColor));
                    lasers.Add(new Laser(startPos, -3, -2, texture, laserLives, laserColor));
                    lasers.Add(new Laser(startPos, -3, -4, texture, laserLives, laserColor));
                    lasers.Add(new Laser(startPos, -3, -5, texture, laserLives, laserColor));
                    lasers.Add(new Laser(startPos, 3, -1, texture, laserLives, laserColor));
                    lasers.Add(new Laser(startPos, 3, -2, texture, laserLives, laserColor));
                    lasers.Add(new Laser(startPos, 3, -4, texture, laserLives, laserColor));
                    lasers.Add(new Laser(startPos, 3, -5, texture, laserLives, laserColor));
                }
                else if (_shootStyle == 3)
                {
                    int laserLives = 1;
                    startPos.X = 10;
                    for (int d = 0; d < 20; d++)
                    {
                        lasers.Add(new Laser(startPos, 0, -5, texture, laserLives, laserColor));
                        startPos.X += 25;
                    }     
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Laser laser in lasers)
            {
                laser.Draw(spriteBatch);
            }
            if (!Game1.lose|| Game1.lose && showWhenLose)
            {
                spriteBatch.Draw(_texture, _position, _color);
                if (Game1.gamemode == "spotlight")
                {
                    spriteBatch.Draw(_spotlightTexture, new Vector2(_spotlight.X, _spotlight.Y), Game1.textColor);
                }
            }
        }
    }
}
 