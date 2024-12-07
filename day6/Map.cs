using System.CodeDom.Compiler;

class Map
{
    public int xSize;
    public int ySize;
    public List<List<char>> MapList { get; }

    public int guardXPos;
    public int guardYPos;

    public int ogGuardXPos;
    public int ogGuardYPos;
    public char ogGuard;

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
                if (lines[i][j] == '^') {
                    guardXPos = j;
                    guardYPos = i;
                    ogGuardXPos = guardXPos;
                    ogGuardYPos = guardYPos;
                    ogGuard = lines[i][j];
                }
                sublist.Add(lines[i][j]);
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
        this.MapList = new List<List<char>>();
        this.xSize = lines[0].Length;
        this.ySize = lines.Length;
        ConvertToMatrix(lines);
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
        this.guardXPos = externalMap.guardXPos;
        this.guardYPos = externalMap.guardYPos;
        this.ogGuardXPos = externalMap.ogGuardXPos;
        this.ogGuardYPos = externalMap.ogGuardYPos;
        this.ogGuard = externalMap.ogGuard;
    }

    /// <summary>
    /// Determines if the given xCoordinate and yCoordinate is valid for the guard to move to.
    /// It is only invalid if the coordinates are an obstacle('#') or a makeshift obstacle('O').
    /// </summary>
    /// <param name="xCoordinate">The x-Coordinate to evaluate.</param>
    /// <param name="yCoordinate">The y-Coordinate to evaluate.</param>
    /// <returns>Whether or not the guard can move to the given position.</returns>
    public bool Is_Valid(int xCoordinate, int yCoordinate) {
        char location = MapList[yCoordinate][xCoordinate];
        if (location == '#' || location == 'O') {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Checks whether or not the guard left the map.
    /// </summary>
    /// <param name="xCoordinate">The x-coordinate that the guard is trying to go to.</param>
    /// <param name="yCoordinate">The y-coordinate that the guard is trying to go to.</param>
    /// <returns>Whether or not the guard leaves the map with the intended coordinates.</returns>
    private bool Leaves_Map(int xCoordinate, int yCoordinate) {
        if (yCoordinate > (ySize-1) ||
            xCoordinate > (xSize-1) ||
            yCoordinate < 0 ||
            xCoordinate < 0) {
                return true;
            }
        return false;
    }
    
    /// <summary>
    /// Moves the guard 1 space.
    /// </summary>
    /// <returns>Whether or not the guard left the map with the move.</returns>
    private bool Move() {
        char guard = MapList[guardYPos][guardXPos];
        if (guard == '^') { // Facing Up
            int newYPos = guardYPos - 1;
            if (Leaves_Map(guardXPos, newYPos)) {
                MapList[guardYPos][guardXPos] = 'X';
                return true;
            }
            if (Is_Valid(guardXPos, newYPos)) {
                MapList[guardYPos][guardXPos] = 'X';
                guardYPos = newYPos;
                MapList[guardYPos][guardXPos] = '^';
            }
            else {
                MapList[guardYPos][guardXPos] = '>';
            }
        }
        else if (guard == '>') { // Facing Right
            int newXPos = guardXPos + 1;
            if (Leaves_Map(newXPos, guardYPos)) {
                MapList[guardYPos][guardXPos] = 'X';
                return true;
            }
            if (Is_Valid(newXPos, guardYPos)) {
                MapList[guardYPos][guardXPos] = 'X';
                guardXPos = newXPos;
                MapList[guardYPos][guardXPos] = '>';
            }
            else {
                MapList[guardYPos][guardXPos] = 'V';
            }
        }
        else if (guard == 'V') { // Facing Left
            int newYPos = guardYPos + 1;
            if (Leaves_Map(guardXPos, newYPos)) {
                MapList[guardYPos][guardXPos] = 'X';
                return true;
            }
            if (Is_Valid(guardXPos, newYPos)) {
                MapList[guardYPos][guardXPos] = 'X';
                guardYPos = newYPos;
                MapList[guardYPos][guardXPos] = 'V';
            }
            else {
                MapList[guardYPos][guardXPos] = '<';
            }
        }
        else if (guard == '<') { // Facing Left
            int newXPos = guardXPos - 1;
            if (Leaves_Map(newXPos, guardYPos)) {
                MapList[guardYPos][guardXPos] = 'X';
                return true;
            }
            if (Is_Valid(newXPos, guardYPos)) {
                MapList[guardYPos][guardXPos] = 'X';
                guardXPos = newXPos;
                MapList[guardYPos][guardXPos] = '<';
            }
            else {
                MapList[guardYPos][guardXPos] = '^';
            }
        }
        else {
            Console.WriteLine("Error: Invalid Guard Character: " + guard);
        }
        return false;
    }

    /// <summary>
    /// Moves the guard until they leave the map or hit the timeout which means a cycle formed.
    /// </summary>
    public void Calculate_Route() {
        // For this implementation we are using time to detect a cycle. It follows the assumption that 
        // if not all the moves are completed in 3 seconds, then there must be a cycle. This is kind of 
        // a shoddy solution but for the scope of the problem it will work and implementing DFS cycle detection
        // or some other was not worth the effort for the given problem.
        DateTime startTime = DateTime.Now;
        while(!Move()) {
            if ((DateTime.Now - startTime).TotalSeconds > 0.01) { // Cycle Detected
                break;
            }
        }
    }

    /// <summary>
    /// Counts the number of distinct positions in the map the guard goes to in their path.
    /// </summary>
    /// <returns>The number of distinct positions the guard crosses in their path.</returns>
    public int Distinct_Positions() {
        Calculate_Route();
        int count = 0;
        for (int i = 0; i < ySize; i++) {
            for (int j = 0; j < xSize; j++) {
                if (MapList[i][j] == 'X') {
                    count++;
                }
            }
        }
        return count;
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
    /// Checks whether or not the guard is in the map.
    /// </summary>
    /// <returns>Whether or not the guard is in the map.</returns>
    public bool Guard_In_Map() {
        char[] guardChars = ['^', '>', 'V', '<'];
        if (guardChars.Contains(MapList[guardYPos][guardXPos])) {
            return true;
        } 
        return false;
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
    /// Adds a temporary obstacle ('O') to the map at the given coordinates.
    /// </summary>
    /// <param name="xCoordinate">The x-coordinate to put the temporary obstacle at.</param>
    /// <param name="yCoordinate">The y-coordinate to put the temporary obstacle at.</param>
    public void Add_Temp_Obstacle(int xCoordinate, int yCoordinate) {
        if (xCoordinate < 0 || yCoordinate < 0 || xCoordinate > (xSize-1) || yCoordinate > (ySize-1)){
            Console.WriteLine("ERROR: Attempting to add Obstacle out of the range of the map.");
            return;
        }
        if (!Is_Valid(xCoordinate, yCoordinate)) {
            Console.WriteLine("ERROR: Attempting to add Obstacle on an obstacle.");
            return;
        }
        if (xCoordinate == guardXPos && yCoordinate == guardYPos) {
            Console.WriteLine("ERROR: Attempting to add Obstacle on guard.");
            return;
        }
        MapList[yCoordinate][xCoordinate] = 'O';
    }
}