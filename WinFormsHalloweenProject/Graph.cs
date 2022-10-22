using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WinFormsHalloweenProject
{

    public class Graph
    {
        public static double DistanceFunc(Point start, Point end)
        {
            return Math.Sqrt((end.X - start.X) * (end.X - start.X) + (end.Y - start.Y) * (end.Y - start.Y));
        }

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
            public int AStarQueueIndex;

            public double KnownDistance;
            public Node Founder;

            public Point Location;
            public List<Edge> Edges;

            public Node(Point topLeft)
            {
                AStarQueueIndex = -1;

                KnownDistance = double.MaxValue;
                Founder = null;

                Location = topLeft;
                Edges = new List<Edge>();
            }
        }


        private List<Node> Nodes = new List<Node>();
        Rectangle Screen;
        public int XOffset => Screen.X;
        public int YOffset => Screen.Y;
        public Graph(Rectangle screen)
        {
            Screen = screen;
            Nodes = new List<Node>();

            //List<Node> path = AStar(null, null, DistanceFunc);

            //TestGraph
            //Write a public GetNextPosition function that adds start and end to the graph then calls AStar
            //Hard code start and end and the graph then test if it works
            //Create a heuristic function to calculate the goal position
            //Test everything together

            //then add path width detection
        }

        public void SetGraph(HashSet<Form1.RECT> rectangles)
        {
            Nodes = new List<Node>();

            Func<Form1.RECT, Form1.RECT, bool>[] CheckIntersections = 
            {
                (rect, scaledRect) => rect.Left < scaledRect.Right & rect.Left > scaledRect.Left, //scaledRect blocks the left
                (rect, scaledRect) => rect.Top < scaledRect.Bottom & rect.Top > scaledRect.Top, //scaledRect blocks the top
                (rect, scaledRect) => rect.Right > scaledRect.Left & rect.Right < scaledRect.Right, //scaledRect blocks the right
                (rect, scaledRect) => rect.Bottom > scaledRect.Top & rect.Bottom < scaledRect.Bottom, //scaledRect blocks the bottom
            };

            Node[] nodes = new Node[4]; //topLeft, topRight, bottomRight, bottomLeft
            foreach (Form1.RECT rect in rectangles)
            {
                nodes[0] = new Node(new Point(rect.Left, rect.Top));
                nodes[1] = new Node(new Point(rect.Right, rect.Top));
                nodes[3] = new Node(new Point(rect.Right, rect.Bottom));
                nodes[2] = new Node(new Point(rect.Left, rect.Bottom));
                foreach (Form1.RECT otherRect in rectangles)
                {
                    if (rect.Equals(otherRect)) continue;
                    for (int i = 0; i < nodes.Length; i++)
                    {
                        if (nodes[i] != null && otherRect.ToRectangle().Contains(nodes[i].Location))
                        {
                            nodes[i] = null;
                        }
                    }
                }

                int previousIndex = nodes.Length - 1;
                bool previousResult = nodes[previousIndex] != null;
                for (int i = 0; i < nodes.Length; i++)
                {
                    bool doesCurrentNodeExist = false;
                    if (nodes[i] != null)
                    {
                        Nodes.Add(nodes[i]);
                        doesCurrentNodeExist = true;
                    }
                    if (previousResult & doesCurrentNodeExist)
                    {
                        bool doesIntersect = false; ;
                        foreach(Form1.RECT rectForScale in rectangles)
                        {
                            Form1.RECT scaledRect = new Form1.RECT(rectForScale.Left - 1, rectForScale.Top - 1, rectForScale.Right + 1, rectForScale.Top + 1);
                            doesIntersect = CheckIntersections[i](rect, scaledRect);
                            if(doesIntersect)
                            {
                                break;
                            }
                        }
                        if (!doesIntersect)
                        {
                            AddEdge(nodes[previousIndex], nodes[i]);
                        }
                    }
                    previousResult = doesCurrentNodeExist;
                    previousIndex = i;
                }
            }


            for(int currentNodeIndex = 0; currentNodeIndex < Nodes.Count; currentNodeIndex ++)
            {
                for(int compareNodeIndex = currentNodeIndex + 1; compareNodeIndex < Nodes.Count; compareNodeIndex ++)
                {
                    //bool areNodesConnected = false;
                    //foreach(Edge edge in Nodes[currentNodeIndex].Edges)
                    //{
                    //    if(edge.NodeA == Nodes[compareNodeIndex] | edge.NodeB == Nodes[compareNodeIndex])
                    //    {
                    //        areNodesConnected = true;
                    //        break;
                    //    }
                    //}
                    if (InLineOfSight(Nodes[currentNodeIndex], Nodes[compareNodeIndex], rectangles))
                    {
                        AddEdge(Nodes[currentNodeIndex], Nodes[compareNodeIndex]);
                    }
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

        private void RemoveEdge(Edge targetEdge)
        {
            targetEdge.NodeA.Edges.Remove(targetEdge);
            targetEdge.NodeB.Edges.Remove(targetEdge);
        }

        private void RemoveEdge(Node nodeA, Node nodeB)
        {
            for (int i = 0; i < nodeA.Edges.Count; i++)
            {
                if (nodeA.Edges[i].NodeB == nodeB || nodeA.Edges[i].NodeA == nodeB)
                {
                    RemoveEdge(nodeA.Edges[i]);
                    return;
                }
            }
        }



        private class AStarQueue
        {
            private Point endLocation;
            private Func<Point, Point, double> heuristic;

            private int currentIndex;
            public int Count => currentIndex;
            private (Node node, double priority)[] backingArray;
            public AStarQueue(Point endLocation, Func<Point, Point, double> heuristic, int capacity) //startingCount = Nodes.Count + 2
            {
                this.endLocation = endLocation;
                this.heuristic = heuristic;

                currentIndex = 0;
                backingArray = new (Node node, double priority)[capacity];
            }

            private int GetParentIndex(int index)
            {
                return (index - 1) / 2;
            }

            private int GetLeftChildIndex(int parent)
            {
                return parent * 2 + 1;
            }

            private int GetRightChildIndex(int parent)
            {
                return parent * 2 + 2;
            }

            private void Swap(int index1, int index2)
            {
                (Node, double) temp = backingArray[index1];
                backingArray[index1] = backingArray[index2];
                backingArray[index2] = temp;

                backingArray[index1].node.AStarQueueIndex = index1;
                backingArray[index2].node.AStarQueueIndex = index2;
            }

            public bool Contains(Node targetNode)
            {
                foreach ((Node node, double priority) nodeDoublePair in backingArray)
                {
                    if (nodeDoublePair.node == targetNode)
                    {
                        return true;
                    }
                }
                return false;
            }

            private void HeapifyUp(int index)
            {
                int parentIndex = GetParentIndex(index);
                if (parentIndex >= 0 && backingArray[index].priority < backingArray[parentIndex].priority)
                {
                    Swap(index, parentIndex);
                    HeapifyUp(parentIndex);
                }
            }

            private void HeapifyDown(int index)
            {
                int leftChildIndex = GetLeftChildIndex(index);
                int rightChildIndex = GetRightChildIndex(index);

                int betterChildIndex;
                if (rightChildIndex >= currentIndex)
                {
                    if (leftChildIndex >= currentIndex)
                    {
                        return;
                    }
                    betterChildIndex = leftChildIndex;
                }
                else if (backingArray[leftChildIndex].priority < backingArray[rightChildIndex].priority)
                {
                    betterChildIndex = leftChildIndex;
                }
                else
                {
                    betterChildIndex = rightChildIndex;
                }

                if (backingArray[betterChildIndex].priority < backingArray[index].priority)
                {
                    Swap(betterChildIndex, index);
                    HeapifyDown(betterChildIndex);
                }
            }

            public void Enqueue(Node targetNode)
            {
                if (targetNode.AStarQueueIndex == -1)
                {
                    backingArray[currentIndex] = (targetNode, targetNode.KnownDistance + heuristic(targetNode.Location, endLocation));
                    targetNode.AStarQueueIndex = currentIndex;
                    HeapifyUp(currentIndex);
                    currentIndex++;
                }
                else if (targetNode.KnownDistance < backingArray[targetNode.AStarQueueIndex].node.KnownDistance)
                {
                    backingArray[targetNode.AStarQueueIndex] = (targetNode, targetNode.KnownDistance + heuristic(targetNode.Location, endLocation));
                    HeapifyUp(targetNode.AStarQueueIndex);
                }
            }

            public Node Pop()
            {
                Node returnValue = backingArray[0].node;
                backingArray[0] = backingArray[currentIndex - 1];
                backingArray[0].node.AStarQueueIndex = 0;
                HeapifyDown(0);
                currentIndex--;
                returnValue.AStarQueueIndex = -1;
                return returnValue;
            }
        }

        private List<Node> AStar(Node startNode, Node endNode, Func<Point, Point, double> heuristic)
        {
            List<Node> path = new List<Node>();
            AStarQueue queue = new AStarQueue(endNode.Location, DistanceFunc, Nodes.Count);
            
            //for (int i = 0; i < Nodes.Count; i++)
            //{
            //    Nodes[i].AStarQueueIndex = -1;
            //    Nodes[i].KnownDistance = double.MaxValue;
            //    Nodes[i].Founder = null;
            //}

            startNode.KnownDistance = 0;
            Node currentNode = startNode;
            queue.Enqueue(startNode);
            while (queue.Count > 0)
            {
                currentNode = queue.Pop();
                if (currentNode == endNode)
                {
                    break;
                }

                for (int i = 0; i < currentNode.Edges.Count; i++)
                {
                    Node neighborNode = currentNode.Edges[i].NodeA;
                    if (currentNode.Edges[i].NodeA == currentNode)
                    {
                        neighborNode = currentNode.Edges[i].NodeB;
                    }
                    if (currentNode.KnownDistance + currentNode.Edges[i].Weight < neighborNode.KnownDistance)
                    {
                        neighborNode.Founder = currentNode;
                        neighborNode.KnownDistance = currentNode.KnownDistance + currentNode.Edges[i].Weight;
                        queue.Enqueue(neighborNode);
                    }
                }
            }
            if (currentNode != endNode)
            {
                return path;
            }


            Stack<Node> reversingStack = new Stack<Node>();
            while (currentNode != startNode)
            {
                reversingStack.Push(currentNode);
                currentNode = currentNode.Founder;
            }

            path.Add(startNode);
            while (reversingStack.Count > 0)
            {
                path.Add(reversingStack.Pop());
            }

            return path;
        }

        private bool InLineOfSight(Node nodeA, Node nodeB, HashSet<Form1.RECT> activeRectangles)
        {
            Point pointA = nodeA.Location;
            Point pointB = nodeB.Location;
            if(pointA == pointB)
            {
                return false;
            }

            double distance = Math.Sqrt((pointB.X - pointA.X) * (pointB.X - pointA.X) + (pointB.Y - pointA.Y) * (pointB.Y - pointA.Y));
            double percentIncrement = 100 / distance;
            double currentPercent = percentIncrement;
            double xValue;
            double yValue;
            Point currentPoint;

            while (currentPercent < 100)
            {
                xValue = ((double)pointA.X).Lerp(pointB.X, currentPercent);
                yValue = ((double)pointA.Y).Lerp(pointB.Y, currentPercent);

                currentPercent += percentIncrement;

                currentPoint = new Point((int)xValue, (int)yValue);
                foreach (Form1.RECT rect in activeRectangles)
                {
                    if (rect.Contains(currentPoint))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private double Distance(Node nodeA, Node nodeB) => Math.Sqrt((nodeB.Location.X - nodeA.Location.X) * (nodeB.Location.X - nodeA.Location.X) + (nodeB.Location.Y - nodeA.Location.Y) * (nodeB.Location.Y - nodeA.Location.Y));

    

        //ASTAR TEST IF FUTURE ME NEEDS
            //Node[] nodes = new Node[100];
            //nodes[0] = new Node(new Point(0, 0));
            //for (int y = 1; y< 10; y++)
            //{
            //    nodes[y * 10] = new Node(new Point(0, y));
            //    AddEdge(1, nodes[(y - 1) * 10], nodes[y * 10]);
            //}
            //for (int x = 1; x< 10; x++)
            //{
            //    nodes[x] = new Node(new Point(x, 0));
            //    AddEdge(1, nodes[x - 1], nodes[x]);
            //}
            //for (int y = 1; y < 10; y++)
            //{
            //    for (int x = 1; x < 10; x++)
            //    {
            //        nodes[y * 10 + x] = new Node(new Point(x, y));
            //        AddEdge(1, nodes[(y - 1) * 10 + x - 1], nodes[y * 10 + x]);
            //        AddEdge(1, nodes[(y - 1) * 10 + x], nodes[y * 10 + x]);
            //        AddEdge(1, nodes[y * 10 + x - 1], nodes[y * 10 + x]);
            //    }
            //}
            //for (int i = 0; i < nodes[11].Edges.Count; i++)
            //{
            //    RemoveEdge(nodes[11].Edges[i]);
            //}
            //for (int i = 0; i < nodes[22].Edges.Count; i++)
            //{
            //    RemoveEdge(nodes[22].Edges[i]);
            //}
            //startNode = nodes[0];
            //endNode = nodes[99];
    }
}
