using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GridHighlighter
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        MouseState mouseState = Mouse.GetState();
        MouseState previousMouseState = Mouse.GetState();

        const int GRID_SIZE = 25;
        Tile[,] grid = new Tile[GRID_SIZE, GRID_SIZE];
        Graph route = new Graph();
        List<Enemy> Enemies = new List<Enemy>();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = GRID_SIZE * GRID_SIZE;
            graphics.PreferredBackBufferWidth = GRID_SIZE * GRID_SIZE;
            IsMouseVisible = true;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            for (int i = 0; i < grid.GetLength(0); ++i)
            {
                for (int j = 0; j < grid.GetLength(1); ++j)
                {
                    grid[i, j] = new Tile(GRID_SIZE, GraphicsDevice);
                }
            }
            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            previousMouseState = mouseState;
            mouseState = Mouse.GetState();
            for (int i = 0; i < grid.GetLength(0); ++i)
            {
                for (int j = 0; j < grid.GetLength(1); ++j)
                {
                    grid[i, j].setActive(false);
                    if (mouseState.X > i * GRID_SIZE && mouseState.X < (i + 1) * GRID_SIZE
                        && mouseState.Y > j * GRID_SIZE && mouseState.Y < (j + 1) * GRID_SIZE)
                    {
                        grid[i, j].setActive(true);

                        //Add waypoints/lines on LMB click
                        if (previousMouseState.LeftButton == ButtonState.Released && mouseState.LeftButton == ButtonState.Pressed)
                        {
                            route.addWaypoint(i, j);
                            if (route.waypoints.Count > 1)
                            {
                                route.lines.Add(new Line(route.waypoints[route.waypoints.Count - 2], route.waypoints[route.waypoints.Count - 1], GRID_SIZE, Color.Orange, 1, GraphicsDevice));
                            }
                        }
                        
                        //Animates an object along the graph on RMB click
                        if (previousMouseState.RightButton == ButtonState.Released && mouseState.RightButton == ButtonState.Pressed)
                        {
                            if (route.waypoints.Count > 1)
                            {
                                Enemies.Add(new Enemy(route.waypoints[0].convertToScreenCoordinates(GRID_SIZE), 20, Color.Blue, 4,GraphicsDevice));
                            }
                        }
                    }
                }
            }
            RemoveEnemies();    //Non-optimal performance

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            for (int i = 0; i < grid.GetLength(0); ++i)
            {
                for (int j = 0; j < grid.GetLength(1); ++j)
                {
                    if (grid[i, j].isActive())
                    {
                        grid[i, j].setColor(Color.White);
                    }
                    else
                    {
                        grid[i, j].setColor(Color.Black);
                    }
                    grid[i, j].setRectanglePosition(i * GRID_SIZE, j * GRID_SIZE);

                    spriteBatch.Draw(grid[i, j].getTexture(), grid[i, j].getRectangle(), grid[i, j].getColor());
                }
            }

            //Color waypoints
            foreach (Waypoint point in route.waypoints)
            {
                grid[point.x, point.y].setColor(Color.Yellow);
                spriteBatch.Draw(grid[point.x, point.y].getTexture(), grid[point.x, point.y].getRectangle(), grid[point.x, point.y].getColor());
            }

            //Draw lines
            foreach (Line connection in route.lines)
            {
                connection.draw(spriteBatch, GRID_SIZE);
            }

            //Animate enemies
            foreach (Enemy opponent in Enemies)
            {
                opponent.moveAlongGraph(spriteBatch, route, GRID_SIZE);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        //Removes enemies that have completed their path
        public void RemoveEnemies()
        {
            for (int i = 0; i < Enemies.Count; i++)
            {
                if (Enemies[i].getCompletion())
                {
                    Enemies[i] = null;
                }
            }
            Enemies.RemoveAll(item => item == null);
        }
    }

    public class Tile
    {
        private bool active = false;
        private Texture2D texture;
        private Rectangle rectangle;
        private Color color;

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

    //Currently very similar to Vector2D, probably will be expanded later on
    public class Waypoint
    {
        public int x;       //Index related to Tiles, not screen position!
        public int y;       //Index related to Tiles, not screen position!

        //Constructor
        public Waypoint(int pointX, int pointY)
        {
            x = pointX;
            y = pointY;
        }

        //Converts from tilearray indices to screen coordinates
        public Vector2 convertToScreenCoordinates(int gridSize) {

            return new Vector2(x * gridSize + gridSize / 2, y * gridSize + gridSize / 2);   // + gridSize/2 will be wrong later on when window size does not depend on gridSize --> tileSize variable or further calculation needed
        }
    }
    
    //Graph representation as a list of waypoints
    public class Graph
    {
        public List<Waypoint> waypoints;
        public List<Line> lines;
        
        //Constructor
        public Graph()
        {
            waypoints = new List<Waypoint>();
            lines = new List<Line>();
        }

        public void addWaypoint(int x, int y)
        {
            waypoints.Add(new Waypoint(x, y));
        }
    }

    //Line for displaying a connection between two waypoints
    public class Line
    {
        private Texture2D sprite;
        private float width;
        private Color color;
        private Vector2 start;
        private Vector2 end;

        //Constructor
        public Line(Waypoint lineStart, Waypoint lineEnd, int gridSize, Color lineColor, float lineWidth, GraphicsDevice graphicsDevice)
        {
            sprite = new Texture2D(graphicsDevice, 1, 1);
            sprite.SetData(new[]{Color.White});
            start = lineStart.convertToScreenCoordinates(gridSize);
            end = lineEnd.convertToScreenCoordinates(gridSize);
            color = lineColor;
            width = lineWidth;
        }

        public void draw(SpriteBatch batch, int gridSize)
        {
            float angle = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);
            float length = Vector2.Distance(new Vector2(start.X, start.Y), new Vector2(end.X, end.Y));
            batch.Draw(sprite, new Vector2(start.X, start.Y), null, color, angle, Vector2.Zero, new Vector2(length, width), SpriteEffects.None, 0);
        }
    }

    //Object that can be moved along a graph
    public class Enemy
    {
        private Texture2D sprite;
        private Vector2 position;
        private Color color;
        private Rectangle rectangle;
        private int lastNode = 0;
        private int nextNode = 1;
        private float speed;
        private float lastDistance = 0;
        private bool completedPath = false;

        //Constructor
        public Enemy(Vector2 enemyPosition, int enemySize, Color enemyColor, int enemySpeed, GraphicsDevice graphicsDevice)
        {
            sprite = new Texture2D(graphicsDevice, 1, 1);
            sprite.SetData(new[] { Color.White });
            position = enemyPosition;
            color = enemyColor;
            speed = enemySpeed;
            rectangle.Width = enemySize;
            rectangle.Height = enemySize;
        }

        //Animates the object's movement along a graph
        public void moveAlongGraph(SpriteBatch batch, Graph path, int gridSize)
        {
            if (lastDistance == 0)  //Initial setup for lastDistance
            {
                lastDistance = Vector2.Distance(position, path.waypoints[nextNode].convertToScreenCoordinates(gridSize));
            }
            if (Vector2.Distance(position, path.waypoints[nextNode].convertToScreenCoordinates(gridSize)) > lastDistance)   //Change target node if distance is increasing (--> node has been passed)
            {
                if (nextNode < path.waypoints.Count - 1)
                {
                    lastNode++;
                    nextNode++;
                }
                else
                {
                    completedPath = true;
                }
            }
            lastDistance = Vector2.Distance(position, path.waypoints[nextNode].convertToScreenCoordinates(gridSize));
            position += (path.waypoints[nextNode].convertToScreenCoordinates(gridSize) - path.waypoints[lastNode].convertToScreenCoordinates(gridSize))/Vector2.Distance(path.waypoints[lastNode].convertToScreenCoordinates(gridSize),path.waypoints[nextNode].convertToScreenCoordinates(gridSize))*speed;
            rectangle.X = (int)(position.X - rectangle.Width / 2);
            rectangle.Y = (int)(position.Y - rectangle.Height / 2);
            batch.Draw(sprite, rectangle, color);
        }

        //Returns whether object has completed the path
        public bool getCompletion() {
            return completedPath;
        }
    }
}
