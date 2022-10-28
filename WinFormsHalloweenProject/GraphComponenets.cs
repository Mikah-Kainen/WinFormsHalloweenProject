using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//using Rectangle_Hueristic;

namespace WinFormsHalloweenProject
{
    using static Pain;
    public partial class Graph
    {
        private class Edge
        {
            public double Weight => distance * Size;

            public bool SizeSet => !double.IsNegativeInfinity(Size);
            public double Size { get; private set; } = double.NegativeInfinity;
            double distance;
            public Node NodeA;
            public Node NodeB;

            const int pixelsToLerp = 10;
            public void SetSize(IEnumerable<Rectangle> rectangles)
            {
                double totalLerps = Math.Min((int)(Math.Sqrt(Math.Pow(NodeA.Location.X - NodeB.Location.X, 2) + Math.Pow(NodeA.Location.Y - NodeB.Location.Y, 2)) / pixelsToLerp), 100);
                int lerpAmount = (int)(100 / totalLerps);
                for (double i = 0; i < 1; i += lerpAmount)
                {
                    GetBiggestRectangle(NodeA.Location.Lerp(NodeB.Location, lerpAmount), NodeA.OwnerGraph.aspectRatio, rectangles, out var newSize);
                    Size += newSize;
                }
                Size = GetRectSize(NodeA.OwnerGraph.Screen, NodeA.OwnerGraph.aspectRatio) / (Size * totalLerps);
            }
            public Edge(double distance, Node nodeA, Node nodeB)
            {
                this.distance = distance;
                NodeA = nodeA;
                NodeB = nodeB;
            }
        }

        private class Node
        {
            public int AStarQueueIndex;

            public double KnownDistance;
            public Node? Founder;

            public Point Location;
            public List<Edge> Edges;
            internal Graph OwnerGraph;

            public Node(Point topLeft, Graph graph)
            {
                AStarQueueIndex = -1;

                KnownDistance = double.MaxValue;
                Founder = null;

                OwnerGraph = graph;
                Location = topLeft;
                Edges = new List<Edge>();
            }
        }
    }
}
