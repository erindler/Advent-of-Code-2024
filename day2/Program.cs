using System;
using System.IO;

class Program {
    // Determines if the report is safe
    static bool DetermineSafe(string[] values) {
        // Converting entry to list of ints
        List<int> intValues = [];
        foreach(string item in values) {
            intValues.Add(int.Parse(item));
        }
        // Check if testing increasing or decreasing
        if (intValues[0] > intValues[1]) { // Test Decreasing
            for (int i = 0; i < (intValues.Count - 1); i++) {
                int difference = intValues[i] - intValues[i+1];
                if(!(difference >= 1 && difference <= 3)) {
                    return false;
                }
            }
            return true;
        }
        else if (intValues[0] < intValues[1]) { // Test Increasing
            for (int i = 0; i < (intValues.Count - 1); i++) {
                int difference = intValues[i+1] - intValues[i];
                if(!(difference >= 1 && difference <= 3)) {
                    return false;
                }
            }
            return true;
        }
        else {
            return false;
        }
    }

    // Removes a level at a given index in the report
    static bool RemoveLevel(List<int> intValues, string[] values, int index) {
        // Removing bad level
        List<int> temp = new List<int>(intValues);
        temp.RemoveAt(index);
        string[] newValues = new string[values.Length - 1];
        for (int i = 0; i < temp.Count; i++) {
            newValues[i] = temp[i].ToString();
        }
        return DetermineSafe(newValues);
    }

    // Determines if a report is safe given 1 bad level
    static bool DetermineProbSafe(string[] values, bool strike=false) {
        // Convert entry to list of ints
        List<int> intValues = [];
        foreach(string item in values) {
            intValues.Add(int.Parse(item));
        }

        // Go through each level in the entry and test if removing it results in a good report
        for(int i = 0; i < intValues.Count; i++) {
            if(RemoveLevel(intValues, values, i)) {
                return true;
            }
        }
        return false;
    }

    // Calculates the number of safe entries(reports), probDamp specifies whether one problem is allowed per entry
    static int CalculateNumberSafeEntries(string[] entries, bool probDamp) {
        int numSafe = 0;
        foreach (string line in entries) {
            string[] values = line.Split(" ");
            if (probDamp) {
                if (DetermineProbSafe(values)) { // One problem allowed per entry
                    numSafe++;
                }
            }
            else {
                if (DetermineSafe(values)) { // No problems per entry
                    numSafe++;
                }
            }
        }
        return numSafe;
    }


    static void Main() {
        // Loading data
        string filePath = "./../Data/day2.txt";
        string[] lines = File.ReadAllLines(filePath);

        int numSafe = CalculateNumberSafeEntries(lines, false);
        int numProbSafe = CalculateNumberSafeEntries(lines, true);
        Console.WriteLine("(Part 1) Number of safe entries: " + numSafe);
        Console.WriteLine("(Part 2) Number of safe entries including problem dampener: " + numProbSafe);
        
    }
}

