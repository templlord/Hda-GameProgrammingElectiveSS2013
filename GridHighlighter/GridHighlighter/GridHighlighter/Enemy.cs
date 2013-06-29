using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GameLibrary;

namespace GridHighlighter
{
    public class Enemy
    {
        private Vector2 position;
        private Rectangle hitBox;
        private Graph path;
        //private Timer shotTimer;
        private float range;
        private float speed;
        private float shotDelay;
        private float distanceInPreviousFrame = 0;
        private int lastNode = 0;
        private int nextNode = 1;
        private int hitPoints;
        //private int spriteID;
        private int shotTimer;
        private bool completedPath = false;
        private bool shotPossible = true;
        private bool alive = true;

        private SAnimationInstance animationInstance;


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
        public Enemy(Vector2 enemyPosition, Rectangle enemyhitBox, int enemyHP, int enemySpeed, float enemyRange, float enemyShotDelay, Graph enemyPath, int startNode, CAnimationHandler animationHandler)
        {
            animationInstance = new SAnimationInstance(animationHandler.ID, animationHandler.Name);
            animationInstance.position.X = (int)enemyPosition.X;
            animationInstance.position.Y = (int)enemyPosition.Y;
            position = enemyPosition;
            position = enemyPosition;
            hitPoints = enemyHP;
            speed = enemySpeed;
            range = enemyRange;
            path = enemyPath;
            shotDelay = enemyShotDelay;
            hitBox = enemyhitBox;
            lastNode = startNode;
            nextNode = startNode + 1;
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

        public SAnimationInstance getAnimationInstance()
        {
            return animationInstance;
        }

        public void animationNextImage(GameTime gameTime, CAnimationHandler animationHandler)
        {
            animationInstance.nextImage(gameTime, animationHandler);
        }

        public Vector2 getPosition() 
        {
            return position;
        }

        public Rectangle getRect()
        {
            return hitBox;
        }

        //Animates the object's movement along a graph
        public void MoveAlongGraph(int gridSize)
        {
            Vector2 nextNodeScreenPosition = path.waypoints.list[nextNode].ConvertToScreenCoordinates(gridSize);
            Vector2 lastNodeScreenPosition;
            if (distanceInPreviousFrame == 0)  //Initial setup for distanceInPreviousFrame
            {
                distanceInPreviousFrame = Vector2.Distance(position, nextNodeScreenPosition);
            }
            if (Vector2.Distance(position, nextNodeScreenPosition) > distanceInPreviousFrame)   //Change target node if distance is increasing (--> node has been passed)
            {
                if (nextNode < path.waypoints.list.Count - 1)
                {
                    ++lastNode;
                    ++nextNode;
                }
                else
                {
                    completedPath = true;
                }
            }
            nextNodeScreenPosition = path.waypoints.list[nextNode].ConvertToScreenCoordinates(gridSize);
            lastNodeScreenPosition = path.waypoints.list[lastNode].ConvertToScreenCoordinates(gridSize);
            distanceInPreviousFrame = Vector2.Distance(position, nextNodeScreenPosition);
            position += (nextNodeScreenPosition - lastNodeScreenPosition) / Vector2.Distance(lastNodeScreenPosition, nextNodeScreenPosition) * speed;
            hitBox.X = (int)(position.X - hitBox.Width * 0.5);
            hitBox.Y = (int)(position.Y - hitBox.Height * 0.5);

        }

        //Finds the first enemy within shooting range and returns it. Returns null if no enemies in range
        public Enemy FindEnemyInRange(List<Enemy> Enemies)
        {
            for (int i = 0; i < Enemies.Count; ++i)
            {
                if (Enemies[i] != this && Vector2.Distance(Enemies[i].position, this.position) < range)
                {
                    return Enemies[i];
                }
            }
            return null;
        }

        public void StartShotTimer()
        {
            shotPossible = false;
            //shotTimer = new Timer(shotDelay);
            //shotTimer.Elapsed += new ElapsedEventHandler(ShotTimerElapsed);
            //shotTimer.Start();
        }

        private void ShotTimerElapsed(object source, ElapsedEventArgs e)
        {
            SetShotPossible(true);
        }

        public bool CheckProjectileCollision(Projectile shot)
        {
            if (hitBox.Intersects(shot.GetRectangle()) || hitBox.Contains(shot.GetRectangle()))
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
}
