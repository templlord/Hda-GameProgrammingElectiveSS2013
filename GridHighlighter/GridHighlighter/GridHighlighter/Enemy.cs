using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GridHighlighter
{
    public class Enemy
    {
        //private Texture2D sprite;
        private Vector2 position;
        private Color color;
        private Rectangle rectangle;
        private Graph path;
        //private Timer shotTimer;
        private float range;
        private float speed;
        private float shotDelay;
        private float distanceInPreviousFrame = 0;
        private int lastNode = 0;
        private int nextNode = 1;
        private int hitPoints;
        private int spriteID;
        private int shotTimer;
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
        public Enemy(Vector2 enemyPosition, int enemySize, Color enemyColor, int enemyHP, int enemySpeed, float enemyRange, float enemyShotDelay, Graph enemyPath)
        {
            //sprite = new Texture2D(graphicsDevice, 1, 1);   //IDs
            //sprite.SetData(new[] { Color.White });
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
            Vector2 nextNodeScreenPosition = path.waypoints[nextNode].ConvertToScreenCoordinates(gridSize);
            Vector2 lastNodeScreenPosition;
            if (distanceInPreviousFrame == 0)  //Initial setup for distanceInPreviousFrame
            {
                distanceInPreviousFrame = Vector2.Distance(position, nextNodeScreenPosition);
            }
            if (Vector2.Distance(position, nextNodeScreenPosition) > distanceInPreviousFrame)   //Change target node if distance is increasing (--> node has been passed)
            {
                if (nextNode < path.waypoints.Count - 1)
                {
                    ++lastNode;
                    ++nextNode;
                }
                else
                {
                    completedPath = true;
                }
            }
            nextNodeScreenPosition = path.waypoints[nextNode].ConvertToScreenCoordinates(gridSize);
            lastNodeScreenPosition = path.waypoints[lastNode].ConvertToScreenCoordinates(gridSize);
            distanceInPreviousFrame = Vector2.Distance(position, nextNodeScreenPosition);
            position += (nextNodeScreenPosition - lastNodeScreenPosition) / Vector2.Distance(lastNodeScreenPosition, nextNodeScreenPosition) * speed;
            rectangle.X = (int)(position.X - rectangle.Width * 0.5);
            rectangle.Y = (int)(position.Y - rectangle.Height * 0.5);
            //batch.Draw(sprite, rectangle, color);
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
            if (rectangle.Intersects(shot.GetRectangle()) || rectangle.Contains(shot.GetRectangle()))
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
