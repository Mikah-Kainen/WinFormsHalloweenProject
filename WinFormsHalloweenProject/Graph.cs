using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsHalloweenProject
{

    public class Graph
    {
        private class Edge
        {
            public double Weight;
            public Node NodeA;
            public Node NodeB;
            public Edge(double weight, Node nodeA, Node nodeB)
            {
                Weight = weight;
                NodeA = nodeA;
                NodeB = nodeB;
            }
        }

        private class Node
        {
            public Point TopLeft;
            public List<Edge> Edges;
            public Node(Point topLeft)
            {
                Edges = new List<Edge>();
                TopLeft = topLeft;
            }
        }

        private Dictionary<Point, Node> Nodes;
        public int NodeWidth;
        public int NodeHeight;
        public Graph(int xSegmentCount, int ySegmentCount, Size screen)
        {
            NodeWidth = screen.Width / xSegmentCount;
            NodeHeight = screen.Height / ySegmentCount;
            Nodes = new Dictionary<Point, Node>();

            Point firstPoint = new Point(0, 0);
            Nodes.Add(firstPoint, new Node(firstPoint));

            for(int y = 1; y < ySegmentCount; y ++)
            {
                Point currentPoint = new Point(0, y);
                Nodes.Add(currentPoint, new Node(new Point(0, y * NodeHeight)));
            }

            for(int x = 1; x < xSegmentCount; x ++)
            {

            }

            for (int y = 1; y < ySegmentCount; y++)
            {
                for (int x = 1; x < xSegmentCount; x++)
                {
                    Point currentPoint = new Point(x, y);
                    Nodes.Add(currentPoint, new Node(new Point(x * NodeWidth, y * NodeHeight)));
                    //Nodes[currentPoint]
                }
            }
        }

        private void fillGraph()
        {
            handleNode(0, 0);
        }

        private void AddEdge(int weight, Node nodeA, Node nodeB)
        {
            Edge newEdge = new Edge(weight, nodeA, nodeB);
            nodeA.Edges.Add(newEdge);
            nodeB.Edges.Add(newEdge);
        }

        private Node handleNode(int x, int y)
        {
            Point currentPoint = new Point(x, y);
            if (!Nodes.ContainsKey(currentPoint))
            {
                Nodes.Add(currentPoint, new Node(new Point(x * NodeWidth, y * NodeHeight)));
                //Nodes[currentPoint]
            }
            return Nodes[currentPoint];
        }
    }
}
