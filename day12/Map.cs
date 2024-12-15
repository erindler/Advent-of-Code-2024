using System.CodeDom.Compiler;
using Microsoft.AspNetCore.Mvc.Filters;

class Map
{
    public int xSize;
    public int ySize;
    public List<List<char>> MapList { get; }

    public List<HashSet<(int x, int y)>> regions { get; }

    /// <summary>
    /// Converts the given string array of lines from the file into the entries 2x2 list (matrix).
    /// </summary>
    /// <param name="lines">The lines from the text file for the map.</param>
    private void ConvertToMatrix(string[] lines)
    {
        for (int i = 0; i < lines.Length; i++)
        {
            List<char> sublist = new List<char>();
            for (int j = 0; j < lines[0].Length; j++)
            {
                sublist.Add(lines[i][j]);
            }
            MapList.Add(sublist);
        }
    }

    /// <summary>
    /// Uses a breadth first search to separate the regions based on the character of the region.
    /// Sets member variable regions with the regions coordinates.
    /// </summary>
    private void Separate_Regions() {
        char goal;
        bool[,] inRegion = new bool[ySize, xSize];
        
        HashSet<(int, int)> BFS((int x, int y) startPosition)
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
                    if (!visited.Contains(neighbor) && (MapList[neighbor.y][neighbor.x] == goal))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                        inRegion[neighbor.y, neighbor.x] = true;
                    }
                }
            }
            return visited;
        }
        for (int i = 0; i < ySize; i++) {
            for (int j = 0; j < xSize; j++) {
                if (!inRegion[i, j]) {
                    goal = MapList[i][j];
                    regions.Add(BFS((j, i)));
                }
            }
        }
    }

    /// <summary>
    /// Constructor which turns the input lines into a matrix and initializes the xSize and ySize of the map.
    /// </summary>
    /// <param name="lines">The lines from the text file for the map.</param>
    public Map(string[] lines)
    {
        this.MapList = new List<List<char>>();
        this.xSize = lines[0].Length;
        this.ySize = lines.Length;
        ConvertToMatrix(lines);
        this.regions = new List<HashSet<(int x, int y)>>();
        Separate_Regions();
    }

    /// <summary>
    /// Constructor which creates a clone of the given map.
    /// </summary>
    /// <param name="externalMap">The map to make a clone of.</param>
    public Map(Map externalMap) {
        this.MapList = new List<List<char>>();
        foreach (var innerList in externalMap.MapList)
        {
            this.MapList.Add(new List<char>(innerList)); // Create a new List<char> for each inner list
        }
        this.xSize = externalMap.xSize;
        this.ySize = externalMap.ySize;
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

    /// <summary>
    /// Overriding the index operator to get the row list returned that is requested.
    /// </summary>
    /// <param name="row">The indes of the row to get from the map.</param>
    /// <returns>The row from the map that was requested.</returns>
    public List<char> this[int row] {
        get {
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
        for (int i = 0; i < ySize; i++) {
            for (int j = 0; j < xSize; j++) {
                output += MapList[i][j];
            }
            output += '\n';
        }
        return output;
    }

    /// <summary>
    /// Calculates the perimeter of a node.
    /// </summary>
    /// <param name="region">The region the node is in.</param>
    /// <param name="node">The coordinates of the node.</param>
    /// <returns>The perimeter of the given node.</returns>
    public int Calculate_Node_Perimeter(HashSet<(int x, int y)> region, (int x, int y) node) {
        int nodePerimeter = 4;
        (int x, int y) neighborNode;

        // Up Node
        neighborNode = (node.x, node.y - 1);
        if (Valid_Coordinate(neighborNode) && region.Contains(neighborNode)) {
            nodePerimeter -= 1;
        }
        // Right Node
        neighborNode = (node.x + 1, node.y);
        if (Valid_Coordinate(neighborNode) && region.Contains(neighborNode)) {
            nodePerimeter -= 1;
        }
        // Down Node
        // Right Node
        neighborNode = (node.x, node.y + 1);
        if (Valid_Coordinate(neighborNode) && region.Contains(neighborNode)) {
            nodePerimeter -= 1;
        }
        // Left Node
        neighborNode = (node.x - 1, node.y);
        if (Valid_Coordinate(neighborNode) && region.Contains(neighborNode)) {
            nodePerimeter -= 1;
        }
        return nodePerimeter;
    }

    /// <summary>
    /// Calculates the perimeter of a given region.
    /// </summary>
    /// <param name="region">The region to calculate the perimeter for.</param>
    /// <returns>The perimeter of the region.</returns>
    public int Calculate_Region_Perimeter(HashSet<(int x, int y)> region) {
        int regionPerimeter = 0;
        foreach ((int x, int y) coordinate in region) {
            regionPerimeter += Calculate_Node_Perimeter(region, coordinate);
        }
        return regionPerimeter;
    }

    /// <summary>
    /// Calculates the area of a given region.
    /// </summary>
    /// <param name="region">The region to calculate the area for.</param>
    /// <returns>The area of the region.</returns>
    public int Calculate_Region_Area(HashSet<(int x, int y)> region) {
        return region.Count;
    }

    /// <summary>
    /// Calculates the number of sides for a node by counting the valid corners of the node.
    /// </summary>
    /// <param name="region">The region the node is a member of.</param>
    /// <param name="node">The coordinates of the node.</param>
    /// <returns>The number of sides the node has that are valid corners in the region.</returns>
    public int Calculate_Node_Sides(HashSet<(int x, int y)> region, (int x, int y) node) {
        int nodeSides = 0;
        (int x, int y) diagNode;
        (int x, int y) upNode = (node.x, node.y - 1);
        (int x, int y) downNode = (node.x, node.y + 1);
        (int x, int y) leftNode = (node.x - 1, node.y);
        (int x, int y) rightNode = (node.x + 1, node.y);

        // INTERIOR CORNERS = Found when 2 edges different than center
        // Up/Right Nodes Edges
        if (region.Contains(node) && !region.Contains(upNode) && !region.Contains(rightNode)) {
            nodeSides += 1;
        }
        // Up/Left Nodes Edges
        if (region.Contains(node) && !region.Contains(upNode) && !region.Contains(leftNode)) {
            nodeSides += 1;
        }
        // Down/Left Nodes Edges
        if (region.Contains(node) && !region.Contains(downNode) && !region.Contains(leftNode)) {
            nodeSides += 1;
        }
        // Down/Right Nodes Edges
        if (region.Contains(node) && !region.Contains(downNode) && !region.Contains(rightNode)) {
            nodeSides += 1;
        }
        // EXTERIOR CORNERS = Found when edges are same as center but diagonal not
        // Up/Right Node
        diagNode = (node.x + 1, node.y - 1);
        if (region.Contains(node) && region.Contains(upNode) && region.Contains(rightNode) && !region.Contains(diagNode)) {
            nodeSides += 1;
        }
        // Up/Left Node
        diagNode = (node.x -1, node.y - 1);
        if (region.Contains(node) && region.Contains(upNode) && region.Contains(leftNode) && !region.Contains(diagNode)) {
            nodeSides += 1;
        }
        // Down/Left Node
        diagNode = (node.x - 1, node.y + 1);
        if (region.Contains(node) && region.Contains(downNode) && region.Contains(leftNode) && !region.Contains(diagNode)) {
            nodeSides += 1;
        }
        // Down/Right Node
        diagNode = (node.x + 1, node.y + 1);
        if (region.Contains(node) && region.Contains(downNode) && region.Contains(rightNode) && !region.Contains(diagNode)) {
            nodeSides += 1;
        }
        return nodeSides;
    }

    /// <summary>
    /// Calculates the number of sides for a given region.
    /// </summary>
    /// <param name="region">The region to calculate the sides for.</param>
    /// <returns></returns>
    public int Calculate_Region_Sides(HashSet<(int x, int y)> region) {
        int regionSides = 0;
        foreach ((int x, int y) coordinate in region) {
            regionSides += Calculate_Node_Sides(region, coordinate);
        }
        return regionSides;
    }

    /// <summary>
    /// Calculates the cost for fencing the regions for the fence that the price is calculated
    /// by multiplying area and perimeter.
    /// </summary>
    /// <returns>The cost to fence the perimeter-based region fencing.</returns>
    public int Calculate_Perimeter_Based_Fence_Price() {
        int price = 0;
        foreach(HashSet<(int x, int y)> region in regions) {
            price += Calculate_Region_Area(region) * Calculate_Region_Perimeter(region);
        }
        return price;
    }

    /// <summary>
    /// Calculates the cost for fencing the regions for the fence that the price is calculated
    /// by multiplying area and number of sides in the region.
    /// </summary>
    /// <returns>The cost to fence the sides-based region fencing.</returns>
    public int Calculate_Sides_Based_Fence_Price() {
        int price = 0;
        foreach(HashSet<(int x, int y)> region in regions) {
            price += Calculate_Region_Area(region) * Calculate_Region_Sides(region);
        }
        return price;
    }
}