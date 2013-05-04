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
using GameLibrary;

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

        const int GRID_SIZE = 20;
        Tile[,] grid = new Tile[GRID_SIZE, GRID_SIZE];

        CAnimationHandler marioHandler = new CAnimationHandler();
        SAnimationInstance marioLeftClick;
        SAnimationInstance marioRightClick;

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

            marioHandler = this.Content.Load<CAnimationHandler>(@"Animations");
            marioLeftClick = new SAnimationInstance(marioHandler.ID, marioHandler.Name);
            marioRightClick = new SAnimationInstance(marioHandler.ID, marioHandler.Name);
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

            mouseState = Mouse.GetState();
            for (int i = 0; i < grid.GetLength(0); ++i)
            {
                for (int j = 0; j < grid.GetLength(1); ++j)
                {
                    grid[i, j].setActive(false);
                    if (mouseState.X > i * GRID_SIZE && mouseState.X < (i + 1) * GRID_SIZE + 1
                        && mouseState.Y > j * GRID_SIZE && mouseState.Y < (j + 1) * GRID_SIZE + 1)
                    {
                        grid[i, j].setActive(true);
                    }
                }
            }

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                marioLeftClick.position.X = mouseState.X;
                marioLeftClick.position.Y = mouseState.Y;
                marioLeftClick.showing = true;
            }
            else if (mouseState.RightButton == ButtonState.Pressed)
            {
                marioRightClick.position.X = mouseState.X;
                marioRightClick.position.Y = mouseState.Y;
                marioRightClick.showing = true;
            }

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

            if (marioLeftClick.showing)
            {
                spriteBatch.Draw(marioLeftClick.getCurrentImage(marioHandler), new Rectangle(marioLeftClick.position.X, marioLeftClick.position.Y, marioLeftClick.getCurrentImage(marioHandler).Width, marioLeftClick.getCurrentImage(marioHandler).Height), Color.White);
                marioLeftClick.nextImage(gameTime, marioHandler);
            }
            if (marioRightClick.showing)
            {
                spriteBatch.Draw(marioRightClick.getCurrentImage(marioHandler), new Rectangle(marioRightClick.position.X, marioRightClick.position.Y, marioRightClick.getCurrentImage(marioHandler).Width, marioRightClick.getCurrentImage(marioHandler).Height), Color.White);
                marioRightClick.nextImage(gameTime, marioHandler);
            }

            spriteBatch.End();

            base.Draw(gameTime);
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
}
