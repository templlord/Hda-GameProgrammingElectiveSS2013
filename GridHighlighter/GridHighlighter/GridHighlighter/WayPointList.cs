using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GridHighlighter
{
    public class WayPointList
    {
        public List<Waypoint> list;

        //Constructor
        public WayPointList()
        {
            list = new List<Waypoint>();
        }

        public bool HasValidLength()
        {
            if (list.Count > 1)
            {
                return true;
            }
            return false;
        }

        public int FindNearestStartingWaypointIndex(int x, int y)
        {
            if (HasValidLength())
            {
                Vector2 selectedPoint = new Vector2(x,y);
                Waypoint currentWaypoint;
                int listLength = list.Count;
                int i;
                int nearestWayPointIndex = 0;

                for (i = 0; i < listLength; ++i)
                {
                    currentWaypoint = list[i];
                    if (currentWaypoint.x == x && currentWaypoint.y == y && !IsLastIndex(i, listLength))
                    {
                        return i;
                    }
                }
                for (i = 0; i < listLength-2; ++i)
                {
                    currentWaypoint = list[i];
                    Console.WriteLine(Vector2.Distance(currentWaypoint.ToVector2(),selectedPoint));
                    if(Vector2.Distance(currentWaypoint.ToVector2(),selectedPoint)<Vector2.Distance(list[nearestWayPointIndex].ToVector2(),selectedPoint))
                    {
                        nearestWayPointIndex = i;
                    }
                }
                return nearestWayPointIndex;
            }
            return 0;
        }

        private bool IsLastIndex(int i, int length)
        {
            if (i == length - 1)
            {
                return true;
            }
            return false;
        }
    }
}
