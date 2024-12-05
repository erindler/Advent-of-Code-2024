/// <summary>
/// This class is used to represent the WordSearch puzzle. It does this using a 2x2 list of characters.
/// </summary>
class WordSearch
{
    public int xSize;
    public int ySize;
    public List<List<char>> Entries { get; }

    /// <summary>
    /// Converts the given string array of lines from the file into the entries 2x2 list (matrix).
    /// </summary>
    /// <param name="lines">The lines from the text file for the puzzle.</param>
    private void ConvertToMatrix(string[] lines)
    {
        for (int i = 0; i < lines.Length; i++)
        {
            List<char> sublist = new List<char>();
            for (int j = 0; j < lines[0].Length; j++)
            {
                sublist.Add(lines[i][j]);
            }
            Entries.Add(sublist);
        }
    }

    /// <summary>
    /// Constructor which turns the input lines into a matrix and initializes the xSize and ySize of the puzzle.
    /// </summary>
    /// <param name="lines">The lines from the text file for the puzzle.</param>
    public WordSearch(string[] lines)
    {
        this.Entries = new List<List<char>>();
        this.xSize = lines[0].Length;
        this.ySize = lines.Length;
        ConvertToMatrix(lines);
    }

    /// <summary>
    /// Tests if there is a row which matches the supplied string starting from the given coordinates and spanning the length of the string.
    /// </summary>
    /// <param name="startX">Starting X coordinate.</param>
    /// <param name="startY">Starting Y coordinate.</param>
    /// <param name="compare">The string to compare the puzzle string to.</param>
    /// <returns>True if the string matches, false otherwise.</returns>
    public bool Compare_Row(int startX, int startY, string compare)
    {
        int numChars = compare.Length;
        int endX = startX + numChars;
        // Exceeds bounds
        if (endX > xSize)
        {
            return false;
        }
        string puzzleStr = "";
        for (int i = 0; i < numChars; i++)
        {
            puzzleStr += this.Entries[startY][startX + i];
        }
        if (compare == puzzleStr)
        {
            return true;
        }
        string reversed = new string(puzzleStr.Reverse().ToArray());
        if (compare == reversed)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Tests if there is a column which matches the supplied string starting from the given coordinates and spanning the length of the string.
    /// </summary>
    /// <param name="startX">Starting X coordinate.</param>
    /// <param name="startY">Starting Y coordinate.</param>
    /// <param name="compare">The string to compare the puzzle string to.</param>
    /// <returns>True if the string matches, false otherwise.</returns>
    public bool Compare_Col(int startX, int startY, string compare)
    {
        int numChars = compare.Length;
        int endY = startY + numChars;
        // Exceeds bounds
        if (endY > ySize)
        {
            return false;
        }
        string puzzleStr = "";
        for (int i = 0; i < numChars; i++)
        {
            puzzleStr += this.Entries[startY + i][startX];
        }
        if (compare == puzzleStr)
        {
            return true;
        }
        string reversed = new string(puzzleStr.Reverse().ToArray());
        if (compare == reversed)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Tests if there is a right-bound diagonal which matches the supplied string starting from the given coordinates and spanning the length of the string.
    /// </summary>
    /// <param name="startX">Starting X coordinate.</param>
    /// <param name="startY">Starting Y coordinate.</param>
    /// <param name="compare">The string to compare the puzzle string to.</param>
    /// <returns>True if the string matches, false otherwise.</returns>
    public bool Compare_Right_Diag(int startX, int startY, string compare)
    {
        int numChars = compare.Length;
        int endX = startX + numChars;
        int endY = startY + numChars;
        // Exceeds bounds
        if (endX > xSize || endY > ySize)
        {
            return false;
        }
        string puzzleStr = "";
        for (int i = 0; i < numChars; i++)
        {
            puzzleStr += this.Entries[startY + i][startX + i];
        }
        if (compare == puzzleStr)
        {
            return true;
        }
        string reversed = new string(puzzleStr.Reverse().ToArray());
        if (compare == reversed)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Tests if there is a left-bound diagonal which matches the supplied string starting from the given coordinates and spanning the length of the string.
    /// </summary>
    /// <param name="startX">Starting X coordinate.</param>
    /// <param name="startY">Starting Y coordinate.</param>
    /// <param name="compare">The string to compare the puzzle string to.</param>
    /// <returns>True if the string matches, false otherwise.</returns>
    public bool Compare_Left_Diag(int startX, int startY, string compare)
    {
        int numChars = compare.Length;
        int endX = startX - numChars;
        int endY = startY + numChars;
        // Exceeds bounds
        if (endX < -1 || endY > ySize)
        {
            return false;
        }
        string puzzleStr = "";
        for (int i = 0; i < numChars; i++)
        {
            puzzleStr += this.Entries[startY + i][startX - i];
        }
        if (compare == puzzleStr)
        {
            return true;
        }
        string reversed = new string(puzzleStr.Reverse().ToArray());
        if (compare == reversed)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Counts the number of rows, columns, right-bound diagonals, and left-bound diagonals that match the given compare string.
    /// </summary>
    /// <param name="compare">The string to find in the puzzle.</param>
    /// <returns>The number of matches of the compare string in the puzzle.</returns>
    public int Num_Traditional_Matches(string compare)
    {
        int numMatches = 0;
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {
                if (Compare_Row(j, i, compare))
                {
                    numMatches++;
                }
                if (Compare_Col(j, i, compare))
                {
                    numMatches++;
                }
                if (Compare_Right_Diag(j, i, compare))
                {
                    numMatches++;
                }
                if (Compare_Left_Diag(j, i, compare))
                {
                    numMatches++;
                }
            }
        }
        return numMatches;
    }

    /// <summary>
    /// Determines if the length 3 word forms an X centered at the given coordinates.
    /// </summary>
    /// <param name="centerX">The x coordinate of the center of the X.</param>
    /// <param name="centerY">The y coordinate of the center of the Y.</param>
    /// <param name="compare">The string to compare.</param>
    /// <returns>True if it makes the X with the word, otherwise, false.</returns>
    public bool Compare_X_Word(int centerX, int centerY, string compare)
    {

        if (centerX < 1 || centerY < 1 || centerX >= (xSize - 1) || centerY >= (ySize - 1))
        {
            return false;
        }
        return Compare_Right_Diag(centerX - 1, centerY - 1, compare) && Compare_Left_Diag(centerX + 1, centerY - 1, compare);
    }

    /// <summary>
    /// Counts the number of occurences of the length 3 word supplied forming an X.
    /// </summary>
    /// <param name="compare">The string to compare in the X.</param>
    /// <returns>Number of X's formed with the length 3 word.</returns>
    public int Num_X_Matches(string compare)
    {
        int numMatches = 0;
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {
                if (Compare_X_Word(j, i, compare))
                {
                    numMatches++;
                }
            }
        }
        return numMatches;
    }
}