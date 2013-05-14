using System;
using System.Timers;
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
        Rectangle SCREEN_RECT = new Rectangle(0,0,GRID_SIZE*GRID_SIZE,GRID_SIZE*GRID_SIZE);
        Tile[,] grid = new Tile[GRID_SIZE, GRID_SIZE];
        Graph route = new Graph();
        List<Enemy> Enemies = new List<Enemy>();
        List<Projectile> Projectiles = new List<Projectile>();

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
                        if (previousMouseState.LeftButton == ButtonState.Released && mouseState.LeftButton == ButtonState.Pressed &! grid[i,j].IsOccupied())
                        {
                            grid[i, j].setOccupied(true);
                            route.AddWaypoint(i, j);
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
                                Enemies.Add(new Enemy(route.waypoints[0].ConvertToScreenCoordinates(GRID_SIZE), 20, Color.Blue, 3, 1, 150, 1000, route, GraphicsDevice));
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < Enemies.Count; i++)
            {
                //Start shooting
                Enemy target = Enemies[i].FindEnemyInRange(Enemies);
                if (target!=null)
                {
                    if (Enemies[i].GetShotPossible())
                    {
                        Enemies[i].StartShotTimer();
                        Projectile nextProjectile = new Projectile(Enemies[i].GetPosition(), target.GetPosition()-Enemies[i].GetPosition(), Color.Red, 2, 2, 1, Enemies[i], GraphicsDevice);
                        Projectiles.Add(nextProjectile);
                    }
                }

                //Check shot collisions
                for(int j = 0; j < Projectiles.Count; j++) 
                {
                    if (Projectiles[j] != null)
                    {
                        if (Projectiles[j].GetShooter() != Enemies[i])
                        {
                            if (Enemies[i].CheckProjectileCollision(Projectiles[j]))
                            {
                                Projectiles[j] = null;
                            }
                        }
                    }
                }
            }
            RemoveEnemies();        //Non-optimal performance -> objects deleting themselves?
            RemoveProjectiles();    // "

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
                connection.Draw(spriteBatch, GRID_SIZE);
            }

            //Animate enemies
            foreach (Enemy opponent in Enemies)
            {
                opponent.MoveAlongGraph(spriteBatch/*, route*/, GRID_SIZE);
            }

            //Draw projectiles
            foreach (Projectile shot in Projectiles)
            {
                shot.MoveAndDraw(spriteBatch, GRID_SIZE);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        //Removes enemies that have completed their path
        public void RemoveEnemies()
        {
            for (int i = 0; i < Enemies.Count; i++)
            {
                if (Enemies[i].GetCompletion() |! Enemies[i].GetAlive())
                {
                    Enemies[i] = null;
                }
            }
            Enemies.RemoveAll(item => item == null);
        }

        //Removes projectiles that collided with an enemy    !!! also remove if colliding with something else, leaving screen etc...
        public void RemoveProjectiles()
        {
            for (int i = 0; i < Projectiles.Count; i++)
            {
                if(Projectiles[i] != null)
                {
                    if (!Projectiles[i].CheckIfOnScreen(SCREEN_RECT))
                    {
                        Projectiles[i] = null;
                    }
                }
            }
            Projectiles.RemoveAll(item => item == null);
        }
    }

    public class Tile
    {
        private bool active = false;
        private bool occupied = false;
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
        public Vector2 ConvertToScreenCoordinates(int gridSize) {

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

        public void AddWaypoint(int x, int y)
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
            start = lineStart.ConvertToScreenCoordinates(gridSize);
            end = lineEnd.ConvertToScreenCoordinates(gridSize);
            color = lineColor;
            width = lineWidth;
        }

        public void Draw(SpriteBatch batch, int gridSize)
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
        private Graph path;
        private Timer shotTimer;
        private float range;
        private float speed;
        private float shotDelay;
        private float lastDistance = 0;
        private int lastNode = 0;
        private int nextNode = 1;
        private int hitPoints;
        private bool completedPath = false;
        private bool shotPossible = true;
        private bool alive = true;
        

        /*//----------STATE MACHINE-------------
        private delegate void eState();
        private eState enemyState;

        private void Idle()
        {
            enemyState = new eState(Moving);
            enemyState();
        }
        private void Moving()
        {
            //MoveAlongGraph();
        }
        private void Shooting()
        {

        }
        private void Dying()
        {

        }
        //------------------------------------*/

        //Constructor
        public Enemy(Vector2 enemyPosition, int enemySize, Color enemyColor, int enemyHP, int enemySpeed, float enemyRange, float enemyShotDelay, Graph enemyPath, GraphicsDevice graphicsDevice)
        {
            sprite = new Texture2D(graphicsDevice, 1, 1);   //IDs
            sprite.SetData(new[] { Color.White });
            position = enemyPosition;
            hitPoints = enemyHP;
            color = enemyColor;
            speed = enemySpeed;
            range = enemyRange;
            path = enemyPath;
            shotDelay = enemyShotDelay;
            rectangle.Width = enemySize;
            rectangle.Height = enemySize;
            //enemyState = new eState(Idle);
        }

        public void SetShotPossible(bool possible)
        {
            shotPossible = possible;
        }

        public bool GetShotPossible()
        {
            return shotPossible;
        }

        public Vector2 GetPosition()
        {
            return position;
        }

        public bool GetAlive()
        {
            return alive;
        }

        //Returns whether object has completed the path
        public bool GetCompletion()
        {
            return completedPath;
        }

        //Animates the object's movement along a graph
        public void MoveAlongGraph(SpriteBatch batch, int gridSize)
        {
            if (lastDistance == 0)  //Initial setup for lastDistance
            {
                lastDistance = Vector2.Distance(position, path.waypoints[nextNode].ConvertToScreenCoordinates(gridSize));
            }
            if (Vector2.Distance(position, path.waypoints[nextNode].ConvertToScreenCoordinates(gridSize)) > lastDistance)   //Change target node if distance is increasing (--> node has been passed)
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
            lastDistance = Vector2.Distance(position, path.waypoints[nextNode].ConvertToScreenCoordinates(gridSize));
            position += (path.waypoints[nextNode].ConvertToScreenCoordinates(gridSize) - path.waypoints[lastNode].ConvertToScreenCoordinates(gridSize))/Vector2.Distance(path.waypoints[lastNode].ConvertToScreenCoordinates(gridSize),path.waypoints[nextNode].ConvertToScreenCoordinates(gridSize))*speed;
            rectangle.X = (int)(position.X - rectangle.Width * 0.5);
            rectangle.Y = (int)(position.Y - rectangle.Height * 0.5);
            batch.Draw(sprite, rectangle, color);
        }

        //Finds the first enemy within shooting range and returns it. Returns null if no enemies in range
        public Enemy FindEnemyInRange(List<Enemy> Enemies)
        {

            for (int i = 0; i < Enemies.Count; i++)
            {
                if(Enemies[i]!=this && Vector2.Distance(Enemies[i].position,this.position) < range) 
                {
                    return Enemies[i];
                }
            }
            return null;
        }

        public void StartShotTimer()
        {
            shotPossible = false;
            shotTimer = new Timer(shotDelay);
            shotTimer.Elapsed += new ElapsedEventHandler(ShotTimerElapsed);
            shotTimer.Start();
        }

        private void ShotTimerElapsed(object source, ElapsedEventArgs e)
        {
            SetShotPossible(true);
        }

        public bool CheckProjectileCollision(Projectile shot)
        {
            if(rectangle.Intersects(shot.GetRectangle()) || rectangle.Contains(shot.GetRectangle()))
            {
                TakeDamage(shot.GetDamage());
                return true;
            }
            return false;
        }

        private void TakeDamage(int damage)
        {
            hitPoints -= damage;
            if (hitPoints <= 0)
            {
                alive = false;
            }
        }
    }

    public class Projectile
    {
        private Texture2D sprite;
        private Rectangle rectangle;
        private Vector2 position;
        private Color color;
        private Vector2 direction;
        private Enemy shooter;
        private float speed;
        private int damage;

        //Constructor
        public Projectile(Vector2 projectilePosition, Vector2 projectileDirection, Color projectileColor, int projectileSize, float projectileSpeed, int projectileDamage, Enemy projectileShooter, GraphicsDevice graphicsDevice)
        {
            sprite = new Texture2D(graphicsDevice, 1, 1);
            sprite.SetData(new[] { Color.White });
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

        public void MoveAndDraw(SpriteBatch batch, int gridSize)
        {
            direction.Normalize();
            position+=direction*speed;
            UpdateRectanglePosition();
            batch.Draw(sprite, rectangle, color);
        }

        private void UpdateRectanglePosition()
        {
            rectangle.X = (int)(position.X - rectangle.Width / 2);
            rectangle.Y = (int)(position.Y - rectangle.Height / 2);
        }

        public bool CheckIfOnScreen(Rectangle screenRect)
        {
            if(screenRect.Contains(rectangle)) {
                return true;
            }
            return false;
        }
    }
}
