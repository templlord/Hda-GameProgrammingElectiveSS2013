using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.Graphics;

namespace GridHighlighter
{
    public class Projectile
    {
        //private Texture2D sprite;
        private Rectangle rectangle;
        private Vector2 position;
        private Color color;
        private Vector2 direction;
        private Enemy shooter;
        private float speed;
        private int damage;
        private int spriteID;

        //Constructor
        public Projectile(Vector2 projectilePosition, Vector2 projectileDirection, Color projectileColor, int projectileSize, float projectileSpeed, int projectileDamage, Enemy projectileShooter)
        {
            //sprite = new Texture2D(graphicsDevice, 1, 1);
            //sprite.SetData(new[] { Color.White });
            position = projectilePosition;
            direction = projectileDirection;
            color = projectileColor;
            speed = projectileSpeed;
            damage = projectileDamage;
            rectangle.Width = projectileSize;
            rectangle.Height = projectileSize;
            shooter = projectileShooter;
            UpdateRectanglePosition();
        }

        public Rectangle GetRectangle()
        {
            return rectangle;
        }

        public Enemy GetShooter()
        {
            return shooter;
        }

        public int GetDamage()
        {
            return damage;
        }

        public void Move(int gridSize)
        {
            direction.Normalize();
            position += direction * speed;
            UpdateRectanglePosition();
            //batch.Draw(sprite, rectangle, color);
        }

        private void UpdateRectanglePosition()
        {
            rectangle.X = (int)(position.X - rectangle.Width / 2);
            rectangle.Y = (int)(position.Y - rectangle.Height / 2);
        }

        public bool CheckIfOnScreen(Rectangle screenRect)
        {
            if (screenRect.Contains(rectangle))
            {
                return true;
            }
            return false;
        }
    }
}
