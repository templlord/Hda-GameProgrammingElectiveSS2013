using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Content
//using Microsoft.Xna.Framework.Graphics;

namespace GridHighlighter
{
    public class Line
    {
        //private Texture2D sprite;
        private Color color;
        private Vector2 start;
        private Vector2 end;
        private float width;
        private int spriteID;

        //Constructor
        public Line(Waypoint lineStart, Waypoint lineEnd, int gridSize, Color lineColor, float lineWidth)
        {
            start = lineStart.ConvertToScreenCoordinates(gridSize);
            end = lineEnd.ConvertToScreenCoordinates(gridSize);
            color = lineColor;
            width = lineWidth;
        }

        /*public void Draw(SpriteBatch batch, int gridSize)
        {
            float angle = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);
            float length = Vector2.Distance(new Vector2(start.X, start.Y), new Vector2(end.X, end.Y));
            batch.Draw(sprite, new Vector2(start.X, start.Y), null, color, angle, Vector2.Zero, new Vector2(length, width), SpriteEffects.None, 0);
        }*/
    }
}
