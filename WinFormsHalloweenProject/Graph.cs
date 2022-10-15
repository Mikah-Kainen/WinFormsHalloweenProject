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
            public bool IsWall;
            public Node(Point topLeft)
            {
                Edges = new List<Edge>();
                TopLeft = topLeft;
                IsWall = false;
            }
        }

        private Dictionary<Point, Node> Nodes;
        Rectangle Screen;
        public int XOffset;
        public int YOffset;
        public int NodeWidth;
        public int NodeHeight;
        public int XSegmentCount;
        public int YSegmentCount;
        private Node GetNodeAtUniversalPoint(Point targetPoint) => Nodes[new Point((targetPoint.X - XOffset) / NodeWidth, (targetPoint.Y - YOffset) / NodeHeight)];
        private Node GetNodeAtOffsetPoint(Point targetPoint) => Nodes[new Point(targetPoint.X / NodeWidth, targetPoint.Y / NodeHeight)];
        public Graph(int xSegmentCount, int ySegmentCount, Rectangle screen)
        {
            Screen = screen;
            XOffset = screen.X;
            YOffset = screen.Y;
            NodeWidth = screen.Width / xSegmentCount;
            NodeHeight = screen.Height / ySegmentCount;
            XSegmentCount = xSegmentCount;
            YSegmentCount = ySegmentCount;
            Nodes = new Dictionary<Point, Node>();

            Point firstPoint = new Point(0, 0);
            Nodes.Add(firstPoint, new Node(firstPoint));

            Point currentPoint;
            Node newNode;
            for(int y = 1; y < ySegmentCount; y ++)
            {
                currentPoint = new Point(0, y);
                newNode = new Node(new Point(0, y * NodeHeight));
                Nodes.Add(currentPoint, newNode);
                AddEdge(NodeHeight, newNode, Nodes[new Point(0, y - 1)]);
            }

            for(int x = 1; x < xSegmentCount; x ++)
            {
                currentPoint = new Point(x, 0);
                newNode = new Node(new Point(x * NodeWidth, 0));
                Nodes.Add(currentPoint, newNode);
                AddEdge(NodeWidth, newNode, Nodes[new Point(x - 1, 0)]);
            }

            double diagonalDistance = Math.Sqrt(NodeWidth * NodeWidth + NodeHeight * NodeHeight);
            for (int y = 1; y < ySegmentCount; y++)
            {
                for (int x = 1; x < xSegmentCount; x++)
                {
                    currentPoint = new Point(x, y);
                    newNode = new Node(new Point(x * NodeWidth, y * NodeHeight));
                    Nodes.Add(currentPoint, newNode);
                    AddEdge(diagonalDistance, newNode, Nodes[new Point(x - 1, y - 1)]);
                    AddEdge(NodeHeight, newNode, Nodes[new Point(x, y - 1)]);
                    AddEdge(NodeWidth, newNode, Nodes[new Point(x - 1, y)]);
                }
            }
        }


        private void AddEdge(double weight, Node nodeA, Node nodeB)
        {
            Edge newEdge = new Edge(weight, nodeA, nodeB);
            nodeA.Edges.Add(newEdge);
            nodeB.Edges.Add(newEdge);
        }


        public void ClearWalls()
        {
            for(int y = Screen.Y; y < NodeHeight * YSegmentCount - 1; y ++)
            {
                for(int x = Screen.X; x < NodeWidth * XSegmentCount - 1; x++)
                {
                    GetNodeAtOffsetPoint(new Point(x, y)).IsWall = false;
                }
            }
        }

        public void SetWallState(Rectangle targetRectangle, bool wallState)
        {
            int xMin = Screen.Left - XOffset;
            int xMax = xMin + NodeWidth * XSegmentCount - 1;
            int yMin = Screen.Top - YOffset;
            int yMax = yMin + NodeHeight * YSegmentCount - 1;
            
            if(targetRectangle.X > xMin)
            {
                xMin = targetRectangle.X;
            }
            if(targetRectangle.X + targetRectangle.Width < xMin)
            {
                xMax = targetRectangle.X + targetRectangle.Width;
            }
            if(targetRectangle.Y > yMin)
            {
                yMin = targetRectangle.Y;
            }
            if(targetRectangle.Y + targetRectangle.Height < yMax)
            {
                yMax = targetRectangle.Y + targetRectangle.Height; 
            }

            for(int y = yMin; y < yMax; y ++)
            {
                for(int x = xMin; x < xMax; x ++)
                {
                    GetNodeAtUniversalPoint(new Point(x, y)).IsWall = wallState;
                }
            }
        }

        public Point Dijkstra(Point start)
        {
            //if (!GetNodeAtUniversalPoint(start).IsWall)
            //{
            //    return start;
            //}
            //for(int i = 0; i < 5; i ++)
            //{
            //    if (!GetNodeAtUniversalPoint(start).IsWall)
            //    {
            //        return new Point(start.X + i, start.Y);
            //    }
            //}
            //return new Point(start.X - 5, start.Y);
            return new Point(1000, 1000);
        }
    }
}
