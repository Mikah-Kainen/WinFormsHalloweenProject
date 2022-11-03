using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using System.Xml.Linq;


namespace WinFormsHalloweenProject
{
    using static Pain;

    public enum PathStatus
    {
        GhostInWall,
        NoPath,
        Path
    }

    public partial class Graph
    {
        public static double DistanceFunc(Point start, Point end)
        {
            return Math.Sqrt(0 *( (end.X - start.X) * (end.X - start.X) + (end.Y - start.Y) * (end.Y - start.Y)));
        }

        private List<Node> Nodes = new List<Node>();
        Rectangle Screen;
        double screenSize;
        Vector2 aspectRatio;
        public Vector2 AspectRatio
        { get => aspectRatio; set => screenSize = GetRectSize(Screen, RectangleComparer.Instance.AspectRatio = aspectRatio = value); }
        public int XOffset => Screen.X;
        public int YOffset => Screen.Y;
        public Graph(Rectangle screen, Vector2 aspectRatio)
        {
         
            Screen = screen;
            AspectRatio = aspectRatio;
            Nodes = new List<Node>();

            //List<Node> path = AStar(null, null, DistanceFunc);

            //TestGraph
            //Write a public GetNextPosition function that adds start and end to the graph then calls AStar
            //Hard code start and end and the graph then test if it works
            //Create a heuristic function to calculate the goal position
            //Test everything together

            //then add path width detection
        }

        Node SetupNodes(HashSet<Ghost.RECT> rectangles, Point ghostLocation)
        {
            Nodes.Clear();

            Func<Ghost.RECT, Ghost.RECT, bool>[] CheckIntersections =
            {
                //(rect, scaledRect) => rect.Left <= scaledRect.Right & rect.Left >= scaledRect.Left & (rect.Top <= scaledRect.Bottom & rect.Bottom >= scaledRect.Top), //scaledRect blocks the left
                //(rect, scaledRect) => rect.Top <= scaledRect.Bottom & rect.Top >= scaledRect.Top & (rect.Left <= scaledRect.Right & rect.Right >= scaledRect.Left), //scaledRect blocks the top
                //(rect, scaledRect) => rect.Right >= scaledRect.Left & rect.Right <= scaledRect.Right & (rect.Top <= scaledRect.Bottom & rect.Bottom >= scaledRect.Top), //scaledRect blocks the right
                //(rect, scaledRect) => rect.Bottom >= scaledRect.Top & rect.Bottom <= scaledRect.Bottom & (rect.Left <= scaledRect.Right & rect.Right >= scaledRect.Left), //scaledRect blocks the bottom
                (rect, scaledRect) => rect.ContainsLeft(scaledRect),
                (rect, scaledRect) => rect.ContainsTop(scaledRect),
                (rect, scaledRect) => rect.ContainsRight(scaledRect),
                (rect, scaledRect) => rect.ContainsBottom(scaledRect),
            };

            Node?[] sideNodes
                = new Node[4]; //topLeft, topRight, bottomRight, bottomLeft
            foreach (Ghost.RECT rect in rectangles)
            {
                var topLeft = new Point(rect.Left, rect.Top);
                var topRight = new Point(rect.Right, rect.Top);
                var bottomRight = new Point(rect.Right, rect.Bottom);
                var bottomLeft = new Point(rect.Left, rect.Bottom);


                sideNodes[0] = Screen.GenerousContains(topLeft) ? new Node(new Point(rect.Left, rect.Top), this) : null;
                sideNodes[1] = Screen.GenerousContains(topRight) ? new Node(new Point(rect.Right, rect.Top), this) : null;
                sideNodes[2] = Screen.GenerousContains(bottomRight) ? new Node(new Point(rect.Right, rect.Bottom), this) : null;
                sideNodes[3] = Screen.GenerousContains(bottomLeft) ? new Node(new Point(rect.Left, rect.Bottom), this) : null;

                foreach (Ghost.RECT otherRect in rectangles)
                {
                    if (rect.Equals(otherRect)) continue;
                    for (int i = 0; i < sideNodes.Length; i++)
                    {
                        if (sideNodes[i] != null && otherRect.Pad(1).Contains(sideNodes[i].Location))
                        {
                            sideNodes[i] = null;
                        }
                    }
                }

                int previousIndex = sideNodes.Length - 1;
                bool previousResult = sideNodes[previousIndex] != null;
                for (int i = 0; i < sideNodes.Length; i++)
                {
                    bool doesCurrentNodeExist = false;
                    if (sideNodes[i] != null)
                    {
                        Nodes.Add(sideNodes[i]);
                        doesCurrentNodeExist = true;

                        if (previousResult)
                        {
                            bool doesIntersect = false;
                            if (Screen.MoneyGrubbingContains(sideNodes[i].Location) || Screen.MoneyGrubbingContains(sideNodes[previousIndex].Location))
                            {
                                foreach (Ghost.RECT rectForScale in rectangles)
                                {
                                    if (rect.Equals(rectForScale)) continue;
                                    Ghost.RECT scaledRect = new Ghost.RECT(rectForScale.Left - 1, rectForScale.Top - 1, rectForScale.Right + 1, rectForScale.Bottom + 1);
                                    doesIntersect = CheckIntersections[i](rect, scaledRect);
                                    if (doesIntersect)
                                    {
                                        break;
                                    }
                                }
                                if (!doesIntersect)
                                {
                                    AddEdge(sideNodes[previousIndex], sideNodes[i]);
                                }
                            }
                        }
                    }

                    previousResult = doesCurrentNodeExist;
                    previousIndex = i;
                }
            }

            Node startNode = new Node(ghostLocation, this);
            Nodes.Add(startNode);
            return startNode;
        }

        public Point[] GetPath(HashSet<Ghost.RECT> rectangles, Point ghostLocation, out PathStatus result, out Rectangle endGoal, out LinkedList<Rectangle> biggestRectangles)
        {
            Node startNode = SetupNodes(rectangles, ghostLocation);
            RectangleComparer.Instance.Start = new Vector2(startNode.Location.X, startNode.Location.Y);
            biggestRectangles = Pain.FindBiggestSpace(rectangles.ToRectangles(), Screen.Size);
            Node endNode = null;
            
            if (biggestRectangles.Count > 0)
            {
                endNode = new Node(biggestRectangles.First().GetCenter(), this);
                Nodes.Add(endNode);
            }
            else
            {
                result = PathStatus.NoPath;
                endGoal = Rectangle.Empty;
                return null;
            }

            foreach (var rect in rectangles)
            {
                if (rect.Contains(ghostLocation))
                {
                    endGoal = rect.ToRectangle();
                    result = PathStatus.GhostInWall;
                    return null;
                }
            }

            for (int currentNodeIndex = 0; currentNodeIndex < Nodes.Count; currentNodeIndex++)
            {
                for (int compareNodeIndex = currentNodeIndex + 1; compareNodeIndex < Nodes.Count; compareNodeIndex++)
                {
                    if (!AreConnected(Nodes[currentNodeIndex], Nodes[compareNodeIndex]) && InLineOfSight(Nodes[currentNodeIndex], Nodes[compareNodeIndex], rectangles))
                    {
                        AddEdge(Nodes[currentNodeIndex], Nodes[compareNodeIndex]);
                    }
                }
            }
            List<Node> nodePath;
            LinkedListNode<Rectangle> curr = biggestRectangles.First;
            while (true)
            {
                nodePath = AStar(startNode, endNode, DistanceFunc, biggestRectangles);
                if (nodePath.Count > 0) break;

                curr = curr.Next;
                if (curr == null)
                {
                    result = PathStatus.NoPath;
                    endGoal = Rectangle.Empty;
                    return null;
                }
                RemoveNode(endNode);
                endNode = new Node(curr.Value.GetCenter(), this);
                Nodes.Add(endNode);

                foreach (var node in Nodes)
                {
                    node.Reset();
                }
                startNode.KnownDistance = 0;

                foreach (var otherNode in Nodes)
                {
                    if (!AreConnected(endNode, otherNode) && InLineOfSight(endNode, otherNode, rectangles))
                    {
                        AddEdge(endNode, otherNode);
                    }
                }

            }
            Point[] pointPath = new Point[nodePath.Count];
            int index = 0;
            foreach (Node node in nodePath)
            {
                pointPath[index] = node.Location;
                index++;
            }
            result = PathStatus.Path;
            endGoal = curr.Value;
            return pointPath;
        }

        private void AddEdge(double weight, Node nodeA, Node nodeB)
        {
            Edge newEdge = new Edge(weight, nodeA, nodeB);
            nodeA.Edges.Add(newEdge);
            nodeB.Edges.Add(newEdge);
        }

        private void AddEdge(Node nodeA, Node nodeB) => AddEdge(Distance(nodeA, nodeB), nodeA, nodeB);

        private bool AreConnected(Node nodeA, Node nodeB)
        {
            for (int i = 0; i < nodeA.Edges.Count; i++)
            {
                if (nodeA.Edges[i].NodeB == nodeB || nodeA.Edges[i].NodeA == nodeB)
                {
                    return true;
                }
            }
            return false;
        }

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
        private void RemoveNode(Node node)
        {
            foreach (var edge in node.Edges)
            {
                if (edge.NodeB != node)
                {
                    edge.NodeB.Edges.Remove(edge);
                }
                else
                {
                    edge.NodeA.Edges.Remove(edge);
                }
            }
            Nodes.Remove(node);
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

        private List<Node> AStar(Node startNode, Node endNode, Func<Point, Point, double> heuristic, LinkedList<Rectangle> biggestRectangles)
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
            Node? currentNode = startNode;
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
                    Node neighborNode = currentNode.Edges[i].NodeA == currentNode ? currentNode.Edges[i].NodeB : currentNode.Edges[i].NodeA;
                    var neighborEdge = currentNode.Edges[i];
                    if (!currentNode.Edges[i].SizeSet)
                    {
                        currentNode.Edges[i].SetSize(biggestRectangles);
                    }
                    var tentativeDistance = currentNode.KnownDistance + currentNode.Edges[i].Weight;
                    if (tentativeDistance < neighborNode.KnownDistance)
                    {
                        neighborNode.Founder = currentNode;
                        neighborNode.KnownDistance = tentativeDistance;
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

        private bool InLineOfSight(Node nodeA, Node nodeB, HashSet<Ghost.RECT> activeRectangles)
        {
            Point pointA = nodeA.Location;
            Point pointB = nodeB.Location;
            if (pointA == pointB)
            {
                return false;
            }

            double distance = Math.Sqrt((pointB.X - pointA.X) * (pointB.X - pointA.X) + (pointB.Y - pointA.Y) * (pointB.Y - pointA.Y));
            double percentIncrement = 1 / distance;
            double currentPercent = percentIncrement;
            double xValue;
            double yValue;
            Point currentPoint;

            bool straight = pointA.X == pointB.X | pointA.Y == pointB.Y;

            while (currentPercent < 1)
            {                
                xValue = ((double)pointA.X).Lerp(pointB.X, currentPercent) + .99999;
                yValue = ((double)pointA.Y).Lerp(pointB.Y, currentPercent) + .99999;

                currentPercent += percentIncrement;

                currentPoint = new Point((int)xValue, (int)yValue);
                int generousContainment = 0;
                foreach (Ghost.RECT rect in activeRectangles)
                {
                    generousContainment += rect.Pad(1).GenerousContains(currentPoint).ToByte();
                    if ((straight.ToByte() + generousContainment > 1 || rect.Contains(currentPoint)))
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
