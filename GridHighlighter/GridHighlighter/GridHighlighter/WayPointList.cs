using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                Waypoint currentWaypoint;
                int listLength = list.Count;
                int i;

                for (i = 0; i < listLength; ++i)
                {
                    currentWaypoint = list[i];
                    if (currentWaypoint.x == x && currentWaypoint.y == y && !IsLastIndex(i, listLength))
                    {
                        return i;
                    }
                }
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
