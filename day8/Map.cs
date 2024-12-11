using System.CodeDom.Compiler;
using Microsoft.AspNetCore.Mvc.Filters;

class Map
{
    public int xSize;
    public int ySize;
    public List<List<char>> MapList { get; }

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
    }

    /// <summary>
    /// Determines the position of the antinodes for two antennas. This is meant to only take two antenna positions 
    /// of a given frequency at a time. It can perform actions for regular or with T-Frequency.
    /// </summary>
    /// <param name="coordinate">The coordinate pair of the first antenna.</param>
    /// <param name="coordinate2">The coordinate pair of the second antenna.</param>
    /// <param name="TFreq">Whether to calculate for T-Frequency or not.</param>
    /// <returns>List of pairs of the coordinates of the antinodes.</returns>
    public List<(int, int)> Determine_Antinode_Positions((int x, int y) coordinate, (int x, int y) coordinate2, bool TFreq=false) {
        List<(int x, int y)> antinodes = new List<(int x, int y)>();
        int xDiff = Math.Abs(coordinate2.x - coordinate.x);
        int yDiff = Math.Abs(coordinate2.y - coordinate.y);

        if (TFreq) {
            // Creating the new coordinates to add
            (int x, int y) newCoordinate = coordinate;
            (int x, int y) newCoordinate2 = coordinate2;
            while (true) {
                int checker = 0;
                

                // x coordinate
                if (newCoordinate2.x > newCoordinate.x) {
                    newCoordinate.x = newCoordinate.x - xDiff;
                    newCoordinate2.x = newCoordinate2.x + xDiff;
                }
                else {
                    newCoordinate.x = newCoordinate.x + xDiff;
                    newCoordinate2.x = newCoordinate2.x - xDiff;
                }
                // y coordinate
                if (newCoordinate2.y > newCoordinate.y) {
                    newCoordinate.y = newCoordinate.y - yDiff;
                    newCoordinate2.y = newCoordinate2.y + yDiff;
                }
                else {
                    newCoordinate.y = newCoordinate.y + yDiff;
                    newCoordinate2.y = newCoordinate2.y - yDiff;
                }

                if (Valid_Coordinate(newCoordinate) && !antinodes.Contains(newCoordinate)) {
                    antinodes.Add(newCoordinate);
                }
                else {
                    checker++;
                }
                if (Valid_Coordinate(newCoordinate2) && !antinodes.Contains(newCoordinate2)) {
                    antinodes.Add(newCoordinate2);
                }
                else {
                    checker++;
                }
                if (checker == 2) {
                    break;
                }
            }
        }
        else {
            // Creating the new coordinates to add
            (int x, int y) newCoordinate;
            (int x, int y) newCoordinate2;
            

            // x coordinate
            if (coordinate2.x > coordinate.x) {
                newCoordinate.x = coordinate.x - xDiff;
                newCoordinate2.x = coordinate2.x + xDiff;
            }
            else {
                newCoordinate.x = coordinate.x + xDiff;
                newCoordinate2.x = coordinate2.x - xDiff;
            }
            // y coordinate
            if (coordinate2.y > coordinate.y) {
                newCoordinate.y = coordinate.y - yDiff;
                newCoordinate2.y = coordinate2.y + yDiff;
            }
            else {
                newCoordinate.y = coordinate.y + yDiff;
                newCoordinate2.y = coordinate2.y - yDiff;
            }

            if (Valid_Coordinate(newCoordinate)) {
                antinodes.Add(newCoordinate);
            }
            if (Valid_Coordinate(newCoordinate2)) {
                antinodes.Add(newCoordinate2);
            }
        }

        return antinodes;
    }

    /// <summary>
    /// Calculates the locations of all of the antinodes of an antenna type in the form of coordinate pairs.
    /// </summary>
    /// <param name="coordinates">List of all the coordinate pairs of the antennas.</param>
    /// <param name="Tfreq">Whether to calculate for T-Frequency or not.</param>
    /// <returns>A list of all of the antinodes for the given antenna type.</returns>
    public List<(int, int)> Calculate_Antinodes(List<(int x, int y)> coordinates, bool Tfreq=false) {
        List<(int x, int y)> antinodes = new List<(int x, int y)>();
        for(int i = 0; i < coordinates.Count; i++) {
            for (int j = i+1; j < coordinates.Count; j++) {
                (int x, int y) coordinate = coordinates[i];
                (int x, int y) coordinate2 = coordinates[j];
                if (Tfreq) {
                    foreach (var newCoord in Determine_Antinode_Positions(coordinate, coordinate2, true)) {
                        antinodes.Add(newCoord);
                    }
                }
                foreach (var newCoord in Determine_Antinode_Positions(coordinate, coordinate2)) {
                    antinodes.Add(newCoord);
                }
            }
        }
        return antinodes;
    }

    /// <summary>
    /// Gets the coordinates of all the antennas and arranges them by type (frequency) in a dictionary.
    /// </summary>
    /// <returns>The dictionary of all of the antenna positions arranged by antenna type (frequency).</returns>
    public Dictionary<char, List<(int x, int y)>> Get_Antennas() {
        char letter;
        Dictionary<char, List<(int x, int y)>> results = new Dictionary<char, List<(int x, int y)>>();
        for (int i = 0; i < xSize; i++) {
            for (int j = 0; j < ySize; j++) {
                letter = MapList[i][j];
                if (letter == '.') {
                    continue;
                }
                if (!results.ContainsKey(letter)) {
                    results[letter] = new List<(int x, int y)>();
                }
                results[letter].Add((j, i));
            }
        }
        return results;
    }

    /// <summary>
    /// Gets the number of antinodes for the entire map over all frequencies.
    /// </summary>
    /// <param name="TFreq">Whether to use T-Frequency or not.</param>
    /// <returns>The number of antinodes for the entire map over all frequencies.</returns>
    public int Get_Num_Antinodes(bool TFreq=false) {
        HashSet<(int x, int y)> results = new HashSet<(int x, int y)>();
        Dictionary<char, List<(int x, int y)>> antennas = new Dictionary<char, List<(int x, int y)>>();
        antennas = Get_Antennas();
        foreach (var letter in antennas.Keys) {
            foreach(var coordinate in Calculate_Antinodes(antennas[letter], TFreq)) {
                results.Add(coordinate);
                
            }
            if (TFreq) {
                foreach(var antenna in antennas[letter]) {
                    results.Add(antenna);
                }
            }
            
        }
        return results.Count;
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
}