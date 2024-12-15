using System;
using System.IO;

class Program {
    static void Main() {
        // Loading data
        string filePath = "./../Data/day12.txt";
        string[] lines = File.ReadAllLines(filePath);

        Map map = new Map(lines);
        int perimeterBasedFencePrice = map.Calculate_Perimeter_Based_Fence_Price();
        int sideBasedFencePrice = map.Calculate_Sides_Based_Fence_Price();
        Console.WriteLine($"(Part 1) Fence price based on region perimeter: {perimeterBasedFencePrice}");
        Console.WriteLine($"(Part 2) Fence price based on region sides: {sideBasedFencePrice}");
    }
}