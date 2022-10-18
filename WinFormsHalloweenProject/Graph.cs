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
            public Point Location;
            public List<Edge> Edges;
            public Node(Point topLeft)
            {
                Edges = new List<Edge>();
                Location = topLeft;
            }
        }


        private List<Node> Nodes;
        Rectangle Screen;
        public int XOffset => Screen.X;
        public int YOffset => Screen.Y;
        public Graph(Rectangle screen)
        {
            Screen = screen;
            Nodes = new List<Node>();
        }

        public void SetGraph(HashSet<Form1.RECT> rectangles)
        {
            Node[] nodes = new Node[4]; //topLeft, topRight, bottomRight, bottomLeft
            foreach (Form1.RECT rect in rectangles)
            {
                nodes[0] = new Node(new Point(rect.Left,  rect.Top));
                nodes[1] = new Node(new Point(rect.Right, rect.Top));
                nodes[3] = new Node(new Point(rect.Right, rect.Bottom));
                nodes[2] = new Node(new Point(rect.Left,  rect.Bottom));
                foreach(Form1.RECT otherRect in rectangles)
                {
                    if (rect.Equals(otherRect)) continue;
                    for(int k = 0; k < nodes.Length; k ++)
                    {

                        if (nodes[k] != null && otherRect.ToRectangle().Contains(nodes[k].Location))
                        {
                            nodes[k] = null;
                        }
                    }
                }


                bool previousResult = nodes[nodes.Length - 1] != null;
                int previousIndex = nodes.Length - 1;
                for(int j = 0; j < nodes.Length; j ++)
                {
                    if (previousResult && nodes[j] != null)
                    {
                        previousResult = true;
                        AddEdge(nodes[previousIndex], nodes[j]);
                    }
                    else
                    {
                        previousResult = false;
                    }
                    previousIndex = j;
                }
            }
        }

        private void AddEdge(double weight, Node nodeA, Node nodeB)
        {
            Edge newEdge = new Edge(weight, nodeA, nodeB);
            nodeA.Edges.Add(newEdge);
            nodeB.Edges.Add(newEdge);
        }

        private void AddEdge(Node nodeA, Node nodeB) => AddEdge(Distance(nodeA, nodeB), nodeA, nodeB);

        public Point Dijkstra(Point start)
        {
            return new Point(1000, 1000);
        }

        private double Distance(Node nodeA, Node nodeB) => Math.Sqrt((nodeB.Location.X - nodeA.Location.X) * (nodeB.Location.X - nodeA.Location.X) + (nodeB.Location.Y - nodeA.Location.Y) * (nodeB.Location.Y - nodeA.Location.Y));
    }
}
