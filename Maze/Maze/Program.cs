using System;
using System.Collections.Generic;

namespace Maze
{
    class Position
    {
        public int x;
        public int y;
        public int blockCount;

        public Position(int x, int y, int blockCount)
        {
            this.x = x;
            this.y = y;
            this.blockCount = blockCount;
        }
    }
    class NodePosition
    {
        public int i;
        public int j;
        public NodePosition next;

        public NodePosition(int i, int j, NodePosition next)
        {
            this.i = i;
            this.j = j;
            this.next = next;
        }
    }
    internal class Program
    {
        static void SolveMaze(int[][] mazeArray)
        {
            List<(int, int)> lst = GetPath(BFSolveMaze(mazeArray));
            for(int i = 1; i < lst.Count; i++)
            {
                mazeArray[lst[i].Item1][lst[i].Item2] = 9;
            }
        }
        static List<(int,int)> GetPath(NodePosition node)
        {
            List<(int,int)> lst = new List<(int, int)>();
            while(node.next != null)
            {
                lst.Add((node.i, node.j));
                node = node.next;
            }
            return lst;
        }
        static NodePosition BFSolveMaze(int[][] mazeArray)
        {
            int[,] directions =
            {
                {-1,0},
                {1,0},
                {0,-1},
                {0,1}
            };
            int row, col;
            NodePosition node;
            bool[][] visited = new bool[mazeArray.Length][];
            InitiateArray(visited, false, mazeArray.Length, mazeArray[0].Length);
            Queue<NodePosition> mazeQueue = new Queue<NodePosition>();
            mazeQueue.Enqueue(new NodePosition(0, 0, null));
            while(mazeQueue.Count != 0)
            {
                node = mazeQueue.Dequeue();
                if (mazeArray[node.i][node.j] == 2)
                {
                    return node;
                }
                for(int i = 0; i < directions.GetLength(0); i++)
                {
                    row = node.i + directions[i, 0];
                    col = node.j + directions[i, 1];
                    if(row >= 0 && row < mazeArray.Length && col >= 0 && col < mazeArray[row].Length)
                    {
                        if (!visited[row][col] && mazeArray[row][col] != 1)
                        {
                            mazeQueue.Enqueue(new NodePosition(row, col, node));
                            visited[row][col] = true;
                        }
                    }
                }
            }
            return null;

        }
        static bool CountOnes(int[][] mazeArray, int row,int col)
        {
            int count = 0;
            for(int i = 0; i < row; i++)
            {
                for(int j = 0; j < col; j++)
                {
                    if (mazeArray[i][j] == 1)
                    {
                        count += 1;
                    }
                }
            }
            if(count < (row * col) / 4)
            {
                return false;
            }
            return true;
        }
        static void MazeBFS()
        {
            Console.WriteLine("Enter Row Amount: ");
            int row = int.Parse(Console.ReadLine());
            Console.WriteLine("Enter Column Amount: ");
            int col = int.Parse(Console.ReadLine());
            Console.WriteLine("Select Diffculty 1 - 3 \n 1 - Hard \n 2 - Medium \n 3 - Easy");
            int diffculty = int.Parse(Console.ReadLine());
            if (row > 100 || col > 100 || row < 2 || col < 2 || diffculty < 0 || diffculty > 3)
            {
                Console.WriteLine("Invald Amount");
                return;
            }
            int[][] mazeArray = MazeBFS(new Random(), row, col,diffculty);
            while (!CountOnes(mazeArray,row,col))
            {
                mazeArray = MazeBFS(new Random(), row, col, diffculty);
            }
            PrintMaze(mazeArray);
            Console.WriteLine("Show Solved? (Y/N)");
            string check = Console.ReadLine();
            if(check.ToLower() == "y")
            {
                SolveMaze(mazeArray);
            }
            PrintMaze(mazeArray);
        }
        static void PrintMaze(int[][] mazeArray)
        {
            for (int i = 0; i < mazeArray.Length; i++)
            {
                for (int j = 0; j < mazeArray[i].Length; j++)
                {
                    if (mazeArray[i][j] == 1)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        if(i < mazeArray.Length - 1)
                        {
                            if (mazeArray[i + 1][j] == 1)
                            {
                                Console.Write("|" + " ");
                                continue;
                            }
                        }
                        if (i > 0)
                        {
                            if (mazeArray[i - 1][j] != 1)
                            {
                                Console.Write("_" + " ");
                            }
                            else
                            {
                                Console.Write("|" + " ");
                            }
                        }
                        else
                        {
                            Console.Write("|" + " ");
                        }
                        continue;
                    }
                    else if (mazeArray[i][j] == 2)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    else if (mazeArray[i][j] == 4)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                    }
                    else if (mazeArray[i][j] == 9)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    Console.Write(mazeArray[i][j] + " ");
                }
                Console.WriteLine();
            }
            Console.ForegroundColor = ConsoleColor.White;
        }
        static int[][] MazeBFS(Random rnd,int row, int col, int difficulty)
        {
            int mazeRow, mazeCol,countBlocks = 0;
            List<(int, int)> addedNeighbors;
            List<Position> deadEnds = new List<Position>();
            int[][] mazeArray = new int[row][];
            bool[][] visitedMaze = new bool[row][];
            InitiateArray(mazeArray, 0, row, col);
            InitiateArray(visitedMaze, false, row, col);
            Queue<(int, int)> mazeQueue = new Queue<(int, int)>();
            mazeQueue.Enqueue((0,0));
            mazeArray[0][0] = 4;
            visitedMaze[0][0] = true;
            while(mazeQueue.Count != 0)
            {
                (mazeRow, mazeCol) = mazeQueue.Dequeue();
                if (mazeArray[mazeRow][mazeCol] != 1 && mazeArray[mazeRow][mazeCol] != 2)
                {
                    addedNeighbors = AddDirections(mazeQueue, mazeArray, visitedMaze, mazeRow, mazeCol);
                    if(addedNeighbors.Count == 0)
                    {
                        deadEnds.Add(new Position(mazeRow, mazeCol,countBlocks));
                    }
                    else
                    {
                        RandomWalls(rnd, addedNeighbors, mazeArray);
                    }
                }
                countBlocks++;
            }
            SortListThirdItem(deadEnds);
            if(difficulty == 1)
            {
                mazeArray[deadEnds[deadEnds.Count - 1].x][deadEnds[deadEnds.Count - 1].y] = 2;
            }
            else if (difficulty == 2)
            {
                if(deadEnds.Count > 2)
                {
                    mazeArray[deadEnds[deadEnds.Count / 2].x][deadEnds[deadEnds.Count / 2].y] = 2;
                }
                else
                {
                    mazeArray[deadEnds[0].x][deadEnds[0].y] = 2;
                }
            }
            else
            {
                mazeArray[deadEnds[0].x][deadEnds[0].y] = 2;
            }
            return mazeArray;
        }
        static void SortListThirdItem(List<Position> lst)
        {
            int temp;
            for(int i = 0; i < lst.Count; i++)
            {
                for(int j = i; j < lst.Count - 1; j++)
                {
                    if (lst[j].blockCount > lst[j + 1].blockCount)
                    {
                        temp = lst[j].blockCount;
                        lst[j].blockCount = lst[j + 1].blockCount;
                        lst[j + 1].blockCount = temp;

                    }
                }
            }
        }
        static void RandomWalls(Random rnd,List<(int,int)> addedNeighbors, int[][] mazeArray)
        {
            int countWalls = addedNeighbors.Count - rnd.Next(1,addedNeighbors.Count);
            int rndNum;
            bool[] visited = new bool[addedNeighbors.Count];
            for(int i = 0; i < visited.Length; i++)
            {
                visited[i] = false;
            }
            while(countWalls > 0)
            {
                rndNum = rnd.Next(0, addedNeighbors.Count);
                if (!visited[rndNum])
                {
                    mazeArray[addedNeighbors[rndNum].Item1][addedNeighbors[rndNum].Item2] = 1;
                    countWalls -= 1;
                    visited[rndNum] = true;
                }
            }
        }
        static List<(int, int)> AddDirections(Queue<(int,int)> mazeQueue, int[][] mazeArray, bool[][] visitedMaze, int mazeRow, int mazeCol)
        {
            List<(int,int)> addedNeighbors = new List<(int,int)>();
            int row, col;
            int[,] directions =
            {
                {-1,0},
                {1,0},
                {0,-1},
                {0,1}
            };
            for(int i = 0; i < directions.GetLength(0); i++)
            {
                row = mazeRow + directions[i, 0];
                col = mazeCol + directions[i, 1];
                if(row >= 0 && row <  mazeArray.GetLength(0) && col >= 0 && col < mazeArray[row].Length)
                {
                    if (!visitedMaze[row][col] && mazeArray[row][col] != 1)
                    {
                        mazeQueue.Enqueue((row, col));
                        addedNeighbors.Add((row, col));
                        visitedMaze[row][col] = true;
                    }
                }
            }
            return addedNeighbors;
        }
        static void InitiateArray<T>(T[][] arr, T deafultValue, int row, int col)
        {
            for (int i = 0; i < row; i++)
            {
                arr[i] = new T[col];
                for (int j = 0; j < col; j++)
                {
                    arr[i][j] = deafultValue;
                }
            }
        }
        static void Main(string[] args)
        {
            string str = "y";
            while (str.ToLower() == "y")
            {
                MazeBFS();
                Console.WriteLine("Enter (y) To Do Again");
                str = Console.ReadLine();
            }
        }
    }
}
