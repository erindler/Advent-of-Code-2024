using System.CodeDom.Compiler;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing.Matching;

class Map
{
    public int xSize;
    public int ySize;
    public List<List<int>> MapList { get; }
    public List<(int x, int y)> Trailhead { get; }

    /// <summary>
    /// Converts the given string array of lines from the file into the entries 2x2 list (matrix).
    /// Also, initializes the trailheads with their coordinates.
    /// </summary>
    /// <param name="lines">The lines from the text file for the map.</param>
    private void ConvertToMatrix(string[] lines)
    {
        for (int i = 0; i < lines.Length; i++)
        {
            List<int> sublist = new List<int>();
            for (int j = 0; j < lines[0].Length; j++)
            {
                sublist.Add(lines[i][j] - '0');
                if (lines[i][j] == '0')
                {
                    Trailhead.Add((j, i));
                }
            }
            MapList.Add(sublist);
        }
    }

    /// <summary>
    /// Constructor which turns the input lines into a matrix and initializes the xSize and ySize of the map.
    /// </summary>
    /// <param name="lines">The lines from the text file for the map.</param>
    public Map(string[] lines)
    {
        this.MapList = new List<List<int>>();
        this.xSize = lines[0].Length;
        this.ySize = lines.Length;
        this.Trailhead = new List<(int x, int y)>();
        ConvertToMatrix(lines);
    }

    /// <summary>
    /// Overriding the index operator to get the row list returned that is requested.
    /// </summary>
    /// <param name="row">The indes of the row to get from the map.</param>
    /// <returns>The row from the map that was requested.</returns>
    public List<int> this[int row]
    {
        get
        {
            return MapList[row];
        }
    }

    /// <summary>
    /// Overriding ToString so that outputting a map prints the map in matrix form.
    /// </summary>
    /// <returns>The map in matrix form so the map prints cleanly.</returns>
    public override string ToString()
    {
        string output = "";
        for (int i = 0; i < ySize; i++)
        {
            for (int j = 0; j < xSize; j++)
            {
                output += MapList[i][j];
            }
            output += '\n';
        }
        return output;
    }

    /// <summary>
    /// Calculates the sum of the scores of all of the trailheads.
    /// </summary>
    /// <returns>Sum of all of the trailhead scores.</returns>
    public int Sum_Trailhead_Scores() {
        int sum = 0;
        foreach((int x, int y) trailhead in Trailhead) {
            sum += Find_Paths(trailhead);
        }
        return sum;
    }

    /// <summary>
    /// Calculates the sum of the ratings of all of the trailheads.
    /// </summary>
    /// <returns>Sum of all of the trailhead ratings.</returns>
    public int Sum_trailhead_Ratings() {
        int sum = 0;
        foreach((int x, int y) trailhead in Trailhead) {
            sum += Count_Paths(trailhead, 9);
        }
        return sum;
    }

    /// <summary>
    /// Calculates the score using all of the paths of the given trailhead coordinates.
    /// </summary>
    /// <param name="trailhead">The coordinates of a trailhead.</param>
    /// <returns>Count of all the paths ending at 9 for the trailhead.</returns>
    public int Find_Paths((int x, int y) trailhead)
    {
        return Count_Full_Path(BFS(trailhead));
    }

    /// <summary>
    /// Counts the number of times a path ends with 9.
    /// </summary>
    /// <param name="paths">The HashSet of all of the locations in the possible paths.</param>
    /// <returns>A count of the number of locations where it is at elevation 9.</returns>
    public int Count_Full_Path(HashSet<(int x, int y)> paths) {
        int count = 0;
        foreach ((int x, int y) location in paths) {
            if (MapList[location.y][location.x] == 9) {
                count++;
            }
        }
        return count;
    }

    /// <summary>
    /// Calculates the trailhead rating given a trailhead coordinate pair.
    /// </summary>
    /// <param name="trailhead">The trailhead coordinates.</param>
    /// <returns>The trailhead rating for the given trailhead coordinates.</returns>
    public int Get_Trailhead_Rating((int x, int y) trailhead) {
        foreach (var coordinate in BFS(trailhead)) {
            Console.WriteLine(coordinate);
        }
        return BFS(trailhead).Count;
    }

    /// <summary>
    /// Performs a breadth first search for the given path conditions to find all of the 
    /// valid paths from the given start position.
    /// </summary>
    /// <param name="startPosition">The coordinates of the start position.</param>
    /// <returns>A HashSet of all of the locations on the valid paths.</returns>
    public HashSet<(int, int)> BFS((int x, int y) startPosition)
    {
        // Keep track of visited nodes
        HashSet<(int x, int y)> visited = new HashSet<(int x, int y)>();

        // Queue for BFS traversal
        Queue<(int x, int y)> queue = new Queue<(int x, int y)>();

        // Enqueue the start node and mark it as visited
        queue.Enqueue(startPosition);
        visited.Add(startPosition);

        // Process nodes until the queue is empty
        while (queue.Count > 0)
        {
            // Dequeue a node
            (int x, int y) currentNode = queue.Dequeue();

            List<(int x, int y)> neighbors = new List<(int x, int y)>();
            if (Valid_Coordinate((currentNode.x+1, currentNode.y))) {
                neighbors.Add((currentNode.x+1, currentNode.y));
            }
            if (Valid_Coordinate((currentNode.x-1, currentNode.y))) {
                neighbors.Add((currentNode.x-1, currentNode.y));
            }
            if (Valid_Coordinate((currentNode.x, currentNode.y+1))) {
                neighbors.Add((currentNode.x, currentNode.y+1));
            }
            if (Valid_Coordinate((currentNode.x, currentNode.y-1))) {
                neighbors.Add((currentNode.x, currentNode.y-1));
            }


            // Enqueue all unvisited neighbors
            foreach (var neighbor in neighbors)
            {
                if (!visited.Contains(neighbor) && ((MapList[neighbor.y][neighbor.x] - MapList[currentNode.y][currentNode.x]) == 1))
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }
        return visited;
    }

    /// <summary>
    /// Counts all of the valid paths from a given start to a specified ending elevation.
    /// </summary>
    /// <param name="start">The coordinates to start the search from.</param>
    /// <param name="end">The elevation value where the path should end.</param>
    /// <returns>The number of valid paths from the start position to a position that has the specified end elevation.</returns>
    public int Count_Paths((int x, int y) start, int end)
    {
        // Helper function for DFS
        int DFS((int x, int y) currentNode, HashSet<(int x, int y)> visited)
        {
            // If we've reached the end node, we've found one path
            if (MapList[currentNode.y][currentNode.x] == end)
                return 1;

            // Mark the current node as visited
            visited.Add(currentNode);

            int pathCount = 0;

            List<(int x, int y)> neighbors = new List<(int x, int y)>();
            if (Valid_Coordinate((currentNode.x+1, currentNode.y))) {
                neighbors.Add((currentNode.x+1, currentNode.y));
            }
            if (Valid_Coordinate((currentNode.x-1, currentNode.y))) {
                neighbors.Add((currentNode.x-1, currentNode.y));
            }
            if (Valid_Coordinate((currentNode.x, currentNode.y+1))) {
                neighbors.Add((currentNode.x, currentNode.y+1));
            }
            if (Valid_Coordinate((currentNode.x, currentNode.y-1))) {
                neighbors.Add((currentNode.x, currentNode.y-1));
            }

            // Explore neighbors
            foreach (var neighbor in neighbors)
            {
                // Only visit unvisited nodes to avoid revisiting
                if (!visited.Contains(neighbor)  && ((MapList[neighbor.y][neighbor.x] - MapList[currentNode.y][currentNode.x]) == 1))
                {
                    pathCount += DFS(neighbor, visited);
                }
            }

            // Backtrack: unmark the current node
            visited.Remove(currentNode);

            return pathCount;
        }

        // Start DFS with an empty visited set
        return DFS(start, new HashSet<(int x, int y)>());
    }

    /// <summary>
    /// Checks whether the given coordinate pair is valid. Checks if it is in the bounds of the map.
    /// </summary>
    /// <param name="coordinate">The coordinate pair to check.</param>
    /// <returns>Whether the coordinate pair is in the map(is valid).</returns>
    public bool Valid_Coordinate((int x, int y) coordinate) {
        if (coordinate.x > (xSize-1) || 
            coordinate.y > (ySize-1) || 
            coordinate.x < 0 || 
            coordinate.y < 0) {
            return false;
        }
        return true;
    }

}