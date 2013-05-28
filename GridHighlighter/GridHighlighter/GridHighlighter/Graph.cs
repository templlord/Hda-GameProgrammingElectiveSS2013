using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GridHighlighter
{
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
}
