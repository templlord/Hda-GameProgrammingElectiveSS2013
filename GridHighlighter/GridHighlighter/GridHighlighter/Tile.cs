using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GridHighlighter
{
    public class Tile
    {
        private Texture2D texture;
        private Rectangle rectangle;
        private Color color;
        private bool active = false;
        private bool occupied = false;

        public Tile(int size, GraphicsDevice graphicsDevice)
        {
            texture = new Texture2D(graphicsDevice, 1, 1);
            texture.SetData(new Color[] { Color.White });

            rectangle.Width = size;
            rectangle.Height = size;
        }

        public bool isActive()
        {
            return this.active;
        }

        public bool IsOccupied()
        {
            return this.occupied;
        }

        public Texture2D getTexture()
        {
            return texture;
        }

        public Rectangle getRectangle()
        {
            return rectangle;
        }

        public Color getColor()
        {
            return color;
        }

        public void setActive(bool a)
        {
            this.active = a;
        }

        public void setOccupied(bool o)
        {
            this.occupied = o;
        }

        public void setRectanglePosition(int x, int y)
        {
            this.rectangle.X = x;
            this.rectangle.Y = y;
        }

        public void setColor(Color c)
        {
            this.color = c;
        }
    }
}
