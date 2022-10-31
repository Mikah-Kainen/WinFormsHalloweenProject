using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//using Rectangle_Hueristic;

namespace WinFormsHalloweenProject
{
    using static Pain;
    using static Extensions;
    public partial class Graph
    {
        private class Edge
        {
            public double Weight => Distance * Size;

            public bool SizeSet => !double.IsNegativeInfinity(Size);
            public double Size { get; private set; } = double.NegativeInfinity;
            public double Distance { get; private set; }
            public Node NodeA;
            public Node NodeB;

            const int pixelsToLerp = 1;
            public void SetSize(IEnumerable<Rectangle> rectangles)
            {  
                Size = 0;
                float currSize = 0;
                int currIterations = 0;
                double totalLerps = (int)Math.Max((Math.Sqrt(Math.Pow(NodeA.Location.X - NodeB.Location.X, 2) + Math.Pow(NodeA.Location.Y - NodeB.Location.Y, 2)) / pixelsToLerp), 3);
                double lerpStep = 1 / totalLerps;

                double lerpAmount = lerpStep;
                for (int i = 1; i < totalLerps - 1; i++)
                {                    
                    lerpAmount += lerpStep;
                    GetBiggestRectangle(NodeA.Location.Lerp(NodeB.Location, lerpAmount), NodeA.OwnerGraph.AspectRatio, rectangles, out var newSize);
                    if (currIterations != 0 & currSize != newSize)
                    {
                        Size += NodeA.OwnerGraph.screenSize / currSize * currIterations;
                        currIterations = 0;
                    }
                    currSize = newSize;
                    currIterations++;
                }
                Size += NodeA.OwnerGraph.screenSize / currSize * currIterations;
                Size /= (totalLerps - 2);
            }
            public void Reset()
            {
                Size = double.NegativeInfinity;                
            }
            public Edge(double distance, Node nodeA, Node nodeB)
            {
                this.Distance = distance;
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

                KnownDistance = double.PositiveInfinity;
                Founder = null;

                OwnerGraph = graph;
                Location = topLeft;
                Edges = new List<Edge>();
            }

            internal void Reset()
            {
                Founder = null;
                KnownDistance = double.PositiveInfinity;
            }
        }
    }
}
