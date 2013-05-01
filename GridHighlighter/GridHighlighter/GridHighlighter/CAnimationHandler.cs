using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GridHighlighter
{
    public class CAnimationHandler
    {
        private int id;
        private string name;
        private List<string> sprites;
        private List<Texture2D> images;
        private bool loop = false;
        private bool reverseOnEnd = false;

        public CAnimationHandler(int id, string name, List<string> sprites, bool loop, bool reverseOnEnd)
        {
            this.id = id;
            this.name = name;
            this.sprites = sprites;
            this.images = new List<Texture2D>();
            this.loop = loop;
            this.reverseOnEnd = reverseOnEnd;
        }

        public void loadImages(ContentManager content)
        {
            foreach (string imageName in sprites)
            {
                images.Add(content.Load<Texture2D>(imageName));
            }
        }

        public int getID()
        {
            return id;
        }

        public string getName()
        {
            return name;
        }

        public List<Texture2D> getImages()
        {
            return images;
        }

        public bool reversesOnEnd()
        {
            return reverseOnEnd;
        }

        public bool loops()
        {
            return loop;
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
            return animationHandler.getImages()[currentImageIndex];
        }

        public void nextImage(GameTime gameTime, CAnimationHandler animationHandler)
        {
            frameTimer -= gameTime.ElapsedGameTime.Milliseconds;
            if (frameTimer <= 0)
            {
                frameTimer = MILLISECONDS_PER_FRAME;
                if (nextImageFactor == 1)
                {
                    if (currentImageIndex == animationHandler.getImages().Count - 1)
                    {
                        if (animationHandler.loops())
                        {

                        }
                    }
                }
                if ((nextImageFactor == 1 && currentImageIndex == animationHandler.getImages().Count - 1)
                    || (nextImageFactor == -1 && currentImageIndex == 0))
                {
                    if (animationHandler.loops())
                    {
                        if (animationHandler.reversesOnEnd())
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
