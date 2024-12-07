using System;
using System.IO;

class Program {
    /// <summary>
    /// Checks whether or not the given map creates a cycle with the path of the guard.
    /// </summary>
    /// <param name="map">The map to check whether or not there is a cycle.</param>
    /// <returns>Whether or not a cycle formed.</returns>
    static bool Does_Create_Cycle(Map map) {
        Map copyMap = new Map(map);
        copyMap.Calculate_Route();
        //Console.WriteLine(copyMap);
        if (copyMap.Guard_In_Map()) {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Counts the number of possible cycles the guard makes by adding one temporary obstacle.
    /// </summary>
    /// <param name="ogMap">The original map.</param>
    /// <param name="map">The map that was used and created the original path.</param>
    /// <returns>The number of cycles that were formed by adding one temporary obstacle.</returns>
    static int Number_Cycles(Map ogMap, Map map) {
        int count = 0;
        for (int i = 0; i < map.ySize; i++) {
            for (int j = 0; j < map.xSize; j++) {
                Map copyMap = new Map(ogMap);
                if (copyMap[i][j] != '#' &&
                    copyMap[i][j] != '^' &&
                    copyMap[i][j] != '>' &&
                    copyMap[i][j] != 'V' &&
                    copyMap[i][j] != '<') {
                    copyMap.Add_Temp_Obstacle(j, i);
                    if (Does_Create_Cycle(copyMap)) {
                        count++;
                    }
                }
            }
        }
        return count;
    }

    static void Main() {
        // Loading data
        string filePath = "./../Data/day6.txt";
        string[] lines = File.ReadAllLines(filePath);

        Map ogMap = new Map(lines);
        Map map = new Map(ogMap);
        int distinctPositions = map.Distinct_Positions();
        int numCycles = Number_Cycles(ogMap, map);
        Console.WriteLine("(Part 1) The number of distinct positions: " + distinctPositions);
        Console.WriteLine("(Part 2) The number of cycles from adding temporary obstacles: " + numCycles);
    }
}