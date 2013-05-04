using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GameLibrary
{
    public class CAnimationHandler
    {
        private int id;
        private string name;
        private bool loop = false;
        private bool reverseOnEnd = false;
        private List<string> sprites = new List<string>();
        private List<Texture2D> images = new List<Texture2D>();

        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public bool Loop
        {
            get { return loop; }
            set { loop = value; }
        }

        public bool ReverseOnEnd
        {
            get { return reverseOnEnd; }
            set { reverseOnEnd = value; }
        }

        public List<string> Sprites
        {
            get { return sprites; }
            set { sprites = value; }
        }

        [ContentSerializerIgnore]
        public List<Texture2D> Images
        {
            get { return images; }
        }

        public void loadImage(ContentManager content, string imageName)
        {
            images.Add(content.Load<Texture2D>(imageName));
        }
    }

    public class CAnimationContentReader : ContentTypeReader<CAnimationHandler>
    {
        protected override CAnimationHandler Read(ContentReader input, CAnimationHandler existingInstance)
        {
            CAnimationHandler animationHandler = new CAnimationHandler();

            animationHandler.ID = input.ReadInt32();
            animationHandler.Name = input.ReadString();
            animationHandler.Loop = input.ReadBoolean();
            animationHandler.ReverseOnEnd = input.ReadBoolean();
            animationHandler.Sprites = input.ReadObject<List<string>>();

            foreach (string sprite in animationHandler.Sprites)
            {
                animationHandler.loadImage(input.ContentManager, sprite);
            }

            return animationHandler;
        }
    }

    public struct SAnimationInstance
    {
        public int id;
        public string name;

        public Point position;
        public bool showing;

        private int nextImageFactor;
        private int currentImageIndex;
        private const int MILLISECONDS_PER_FRAME = 200;
        private int frameTimer;

        public SAnimationInstance(int id, string name)
        {
            this.id = id;
            this.name = name;
            this.position = new Point(0, 0);
            this.showing = false;

            this.nextImageFactor = 1;
            this.currentImageIndex = 0;
            this.frameTimer = MILLISECONDS_PER_FRAME;
        }

        public Texture2D getCurrentImage(CAnimationHandler animationHandler)
        {
            return animationHandler.Images[currentImageIndex];
        }

        public void nextImage(GameTime gameTime, CAnimationHandler animationHandler)
        {
            frameTimer -= gameTime.ElapsedGameTime.Milliseconds;
            if (frameTimer <= 0)
            {
                frameTimer = MILLISECONDS_PER_FRAME;
                if ((nextImageFactor == 1 && currentImageIndex == animationHandler.Images.Count - 1)
                    || (nextImageFactor == -1 && currentImageIndex == 0))
                {
                    if (animationHandler.Loop)
                    {
                        if (animationHandler.ReverseOnEnd)
                        {
                            nextImageFactor *= -1;
                        }
                        else
                        {
                            currentImageIndex = 0;
                        }
                    }
                }
                else
                {
                    currentImageIndex += nextImageFactor;
                }
            }
        }
    }
}
