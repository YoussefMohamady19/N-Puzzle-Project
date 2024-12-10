// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace nPuzzle
{
    class Node
    {
        //Node Parent
        public Node node;
        //Puzzle mat
        public int[,] array;
        //posation blank tile
        public int x, y;
        //Distane cost
        public int cost;
        //Depth tree
        public int level;
    };
    class Program
    {

        //check if puzzle is solved or not solved
        static bool isSolvable(int[,] arr,int[] puzzle)
        {
            
            int inv = getInvCount(puzzle);


            if ((puzzle.Length % 2 != 0) && (inv % 2 == 0))
            {
                return true;
            }
            else
            {
                int pos = findXPosition(arr);
                if (pos % 2 == 0 && inv % 2 != 0)
                    return true;
                else if (pos % 2 != 0 && inv % 2 == 0)
                    return true;
                else
                    return false;
            }
        }
        //get number of inversion
        static int getInvCount(int[] arr)
        {
            int count = 0;
            for (int i = 0; i < (arr.Length * arr.Length) - 1; i++)
            {
                for (int j = i + 1; j < arr.Length; j++)
                {
                    if (arr[j] != 0 && arr[i] != 0 && arr[i] > arr[j])
                    {
                        count++;
                    }
                }
            }
            return count;
        }
        
        static int findXPosition(int[,] puzzle)
        {

            for (int i = puzzle.GetLength(0) - 1; i >= 0; i--)
                for (int j = puzzle.GetLength(1) - 1; j >= 0; j--)
                    if (puzzle[i, j] == 0)
                        return puzzle.GetLength(1) - i;
            return 0;
        }
        //create new node and assign INFO
        static  Node newNode(int[,]mat,int x,int y,int newx,int newy,int levelNode,Node parent,int num)
        {
             
            Node node = new Node();
            node.node = parent;
            node.array = new int[num,num];
            node.array = mat;
            int temp = node.array[x, y];
            node.array[x, y]=node.array[newx, newy];
            node.array[newx, newy]=temp;
            node.x = newx;
            node.y = newy;
            node.level = levelNode;
            //cost in Astar
            return node;
        }
        //Find postion blank tile 
        static (int,int) findBlankTile(int[,] mat,int num)
        {
            int x1=-1, y1=-1;
            for (int i = 0; i < num; i++)
                for(int j = 0; j < num; j++)
                {
                    if(mat[i,j] == 0)
                    {
                        x1 = i;
                        y1= j;
                        break;
                    }
                }
            return (x1, y1);
        }
        //Print mat in Console
        static void printMatraix(int[,]mat,int num)
        {
            for(int i = 0; i < num; i++)
            {
                
                for( int j = 0; j < num; j++)
                {
                    Console.Write(mat[i,j]+" ");
                }
                Console.WriteLine();
            }
        }
        //number of steps to get goal
        static int numberSteps;
        //show path form root to goal
        static void showPath(Node path,int num)
        {
            if (path.node == null)
                return;
            numberSteps++;
            showPath(path.node,num);
            Console.WriteLine("###############################################");
            printMatraix(path.array,num);

        }
        //Calculate Heuristic value by Hamming priority function
        static int Hamming_method(Node n, int[,] goal,int num)
        {
            int count = 0;
            for (int i = 0; i < num; i++)
                for (int j = 0; j < num; j++)
                    if (n.array[i,j] != goal[i,j]&&n.array[i,j]!=0)
                    {
                        count++;
                    }

            return count;            
        }
        //Calculate Heuristic value by manhattan priority function
        static int manhattan(Node initial, int [,]goal,int N)
        {
            int manhattanDistance = 0;
            int []arr=new int[N*N];
            int count = 0;
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    arr[count] = initial.array[i,j];
                    count++;
                }
            }

            for (int i = 0; i < N * N; i++)
            {
                if (arr[i] == 0)
                    continue;
                int vertical = Math.Abs((i / N) - ((arr[i] - 1) / N));
                int horizontal = Math.Abs((i % N) - ((arr[i] - 1) % N));
                manhattanDistance += vertical + horizontal;
            }
            return manhattanDistance;
        }
        //find road from root to goal by A* algo
        static void Astar(int[,] initial, int[,] goal,int n,string s)
        {
            // bottom, left, top, right
            int[] row = { 1, 0, -1, 0 };
            int[] col = { 0, -1, 0, 1 };
            
            Node root;
            PriorityQueue<Node,int> openlist=new PriorityQueue<Node, int>();
            Node parent=new Node();
            parent.node = null;
            
            var blank=findBlankTile(initial,n);
            root=newNode(initial, blank.Item1, blank.Item2, blank.Item1, blank.Item2,0,parent,n);
            root.node.x = -1;
            root.node.y = -1;
            if(s=="1")
                root.cost = Hamming_method(root, goal, n);
            else
                root.cost = manhattan(root, goal, n);
            
            openlist.Enqueue(root, 0);
            //path.Enqueue(root, proiraty);
            //proiraty++;
            while(openlist.Count > 0)
            {
                Node node=openlist.Dequeue();
                if(node.cost==0)
                {
                    
                    showPath(node,n);
                    Console.WriteLine("Number of steps : "+(numberSteps-1));
                    return;
                }
                int xnew,ynew;
                //create 4 child for parent node
                for(int i=0;i<4;i++)
                {
                    Node child =new Node();
                    xnew= node.x + row[i];
                    ynew= node.y + col[i];
                    if(xnew>=0&& xnew<n&&ynew>=0&& ynew<n&&(node.node.x!= xnew||node.node.y!= ynew))
                    {
                        int[,] newmat=(int[,])node.array.Clone();
                        child = newNode(newmat, node.x, node.y, xnew, ynew, (node.level + 1),node,n);
                        if(s=="1")
                            child.cost = Hamming_method(child, goal, n);
                        else
                            child.cost = manhattan(child, goal, n);
                        int proiraty = child.cost + child.level;
                        openlist.Enqueue(child, proiraty);

                    }
                }
                

            }

        }

        // Main function, execution entry point of the program  
        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            
            bool check;
            //read from file
            String input = File.ReadAllText(@"D:\project\Visual\nPuzzle\Test Case\15-TEST.txt");
            int row=0,col=0;
            string[] Lines = input.Split('\n');
            Lines=Lines.Where(l => l!="\r").ToArray();
            int number=int.Parse(Lines[0]);
            int[,] test = new int[number, number];
            int[,] test2=new int[number, number];
            int[] arr = new int[number*number];
            int checker = 0;
            int count = 1;
            for(int i=1; i<Lines.Length; i++)
            {
                col = 0;
                foreach (var num in Lines[i].Trim().Split(' '))
                {
                    if (num.Trim() == "")
                        continue;
                    test[row,col]=int.Parse(num.Trim());
                    arr[checker]= int.Parse(num.Trim());
                    checker++;
                    col++;
                }
                row++;
            }
            for(int i=0; i<number; i++)
            {
                for(int j=0; j<number; j++)
                {
                    test2[i, j] = count;
                    count++;
                }
            }
            test2[number - 1, number - 1] = 0;
            check=isSolvable(test,arr);
            if(check)
            {
                Console.WriteLine("Solvable");
                Console.WriteLine("Write number [1] if you want run by Hamming priority function");
                Console.WriteLine("Write number [2] if you want run by Manhattan priority function");
               
                string select=Console.ReadLine();

                stopwatch.Start();
                Astar(test, test2, number, select);
                stopwatch.Stop();
                Console.WriteLine("Elapsed Time is {0} ms", (stopwatch.ElapsedMilliseconds));
            }
            else
            {
                Console.WriteLine("Unsolvable");
            }
            
            


        }
    }
}