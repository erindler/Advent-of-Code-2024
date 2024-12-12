using System;
using System.IO;

class Program {
    static void Main() {
        // Loading data
        string filePath = "./../Data/day10.txt";
        string[] lines = File.ReadAllLines(filePath);

        Map map = new Map(lines);
        int sumTrailheadScores = map.Sum_Trailhead_Scores();
        int sumTrailheadRatings = map.Sum_trailhead_Ratings();
        Console.WriteLine($"(Part 1) Sum of the trailhead scores: {sumTrailheadScores}");
        Console.WriteLine($"(Part 2) Sum of the trailhead ratings: {sumTrailheadRatings}");
    }
}