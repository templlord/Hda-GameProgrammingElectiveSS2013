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

namespace WindowsGame1
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        List<Texture2D> m_marioList = new List<Texture2D>();
        int m_marioDrawIndex = 0;
        int frameDuration = 5;
        int curFrameCount = 0;
        

        List<SoundEffect> m_soundEffects = new List<SoundEffect>();

        KeyboardState m_kbState;    
        Keys m_jumpKey;
        Keys m_leftKey;
        Keys m_rightKey;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
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
            // TODO: Add your initialization logic here
            base.Initialize();

            m_jumpKey = Keys.Up;
            m_leftKey = Keys.Left;
            m_rightKey = Keys.Right;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            m_marioList.Add(this.Content.Load<Texture2D>("Mario"));
            m_marioList.Add(this.Content.Load<Texture2D>("Mario2"));
            m_marioList.Add(this.Content.Load<Texture2D>("Mario3"));
            m_marioList.Add(this.Content.Load<Texture2D>("Mario4"));
            // TODO: use this.Content to load your game content here


            m_soundEffects.Add(this.Content.Load<SoundEffect>("button1"));
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

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            m_kbState = Keyboard.GetState(); 

            

            if (curFrameCount >= frameDuration)
            {
                curFrameCount = 0;
                ++m_marioDrawIndex;
                
                if (m_marioDrawIndex >= m_marioList.Count)
                {
                    m_marioDrawIndex = 0;
                }
            }
                
            Texture2D curTex = m_marioList[m_marioDrawIndex];
            SoundEffectInstance soundInstance = null;

            spriteBatch = new SpriteBatch(GraphicsDevice);

            spriteBatch.Begin();

            if (m_kbState.IsKeyDown(Keys.A) == true)
            {
                spriteBatch.Draw(curTex, new Rectangle(100, 100, curTex.Width, curTex.Height), Color.White);
                //soundInstance = m_soundEffects[0].CreateInstance();
                //soundInstance.Play();
            }
            else if (m_kbState.IsKeyDown(Keys.D) == true)
            {
                spriteBatch.Draw(curTex, new Rectangle(100, 100, curTex.Width * -1, curTex.Height), Color.White);
            }
            else
            {
                spriteBatch.Draw(m_marioList[0], new Rectangle(100, 100, curTex.Width, curTex.Height), Color.White);
            }
            spriteBatch.End();

            base.Draw(gameTime);

           ++curFrameCount;
        }
    }
}