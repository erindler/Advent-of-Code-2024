using System;
using System.IO;

class Program {
    static void Main() {
        // Loading data
        string filePath = "./../Data/day4.txt";
        string[] lines = File.ReadAllLines(filePath);

        WordSearch wordSearch = new WordSearch(lines);
        int numTraditionalMatches = wordSearch.Num_Traditional_Matches("XMAS");
        int numXMatches = wordSearch.Num_X_Matches("MAS");

        Console.WriteLine("(Part 1) Number of 'XMAS' in puzzle: " + numTraditionalMatches);
        Console.WriteLine("(Part 2) Number of 'X-MAS' in puzzle: " + numXMatches);
    }
}

