using System;
using System.IO;

class Program {
    static void Main() {
        // Loading data
        string filePath = "./../Data/day8.txt";
        string[] lines = File.ReadAllLines(filePath);

        Map map = new Map(lines);
        int numAntinodes = map.Get_Num_Antinodes();
        int numTFreqAntinodes = map.Get_Num_Antinodes(true);
        Console.WriteLine("(Part 1) The number of antinodes is: " + numAntinodes);
        Console.WriteLine("(Part 2) The number of T-Frequency antinodes is: " + numTFreqAntinodes);
    }
}