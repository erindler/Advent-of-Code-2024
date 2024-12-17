using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;

class Program {
    /// <summary>
    /// Parses the input data and formats it into a pair of position coordinates and velocity coordinate values.
    /// </summary>
    /// <param name="lines">The array of strings that are the lines inputted from the text file.</param>
    /// <returns>A list of formatted robots.</returns>
    static List<((int x, int y) position, (int x, int y) velocity)> Parse_Data(string[] lines) {
        List<((int x, int y) position, (int x, int y) velocity)> robots = new List<((int x, int y) position, (int x, int y) velocity)>();
        foreach(string line in lines) {
            string[] splitLine = line.Split(" ");
            string[] strPos = splitLine[0].Replace("p=", "").Split(",");
            (int x, int y) pos = (int.Parse(strPos[0]), int.Parse(strPos[1]));
            string[] strVel = splitLine[1].Replace("v=", "").Split(",");
            (int x, int y) vel = (int.Parse(strVel[0]), int.Parse(strVel[1]));
            robots.Add((pos, vel));
        }
        return robots;
    }
    
    /// <summary>
    /// Simulates one second of movement for a single robot.
    /// </summary>
    /// <param name="robot">The robot's data (position, velocity).</param>
    /// <param name="width">The width of the map.</param>
    /// <param name="height">The height of the map.</param>
    /// <returns>The robot's data after one second of movement.</returns>
    static ((int x, int y) position, (int x, int y) velocity) Simulate_Robot_Move(((int x, int y) position, (int x, int y) velocity) robot, int width, int height) {
        int maxWidthIndex = width - 1;
        int maxHeightIndex = height - 1;
        (int x, int y) newPos = (robot.position.x + robot.velocity.x, robot.position.y + robot.velocity.y);
        if (newPos.x < 0) {
            newPos.x += width; 
        }
        else if (newPos.x > maxWidthIndex) {
            newPos.x -= width;
        }

        if (newPos.y < 0) {
            newPos.y += height; 
        }
        else if (newPos.y > maxHeightIndex) {
            newPos.y -= height;
        }
        robot.position.x = newPos.x;
        robot.position.y = newPos.y;
        return robot;
    }

    /// <summary>
    /// Simulates all of the robot movement for a given number of seconds.
    /// </summary>
    /// <param name="robots">The list of robots and their respective data.</param>
    /// <param name="width">The width of the map.</param>
    /// <param name="height">The height of the map.</param>
    /// <param name="seconds">The number of seconds to simulate robot movement for.</param>
    /// <returns>The new robot state.</returns>
    static List<((int x, int y) position, (int x, int y) velocity)> Simulate_Robots_Moves_Seconds(List<((int x, int y) position, (int x, int y) velocity)> robots, int width, int height, int seconds) {
        for (int _ = 0; _ < seconds; _++) {
            for (int i = 0; i < robots.Count; i++) {
                robots[i] = Simulate_Robot_Move(robots[i], width, height);
            }
        }
        return robots;
    }

    /// <summary>
    /// Calculates the safety factor of the robot's state which is taken by multiplying the number of robots in each
    /// quadrant. The midlines do not belong to any quadrant and thus robots on the midlines too.
    /// </summary>
    /// <param name="robots">The current state of the robots.</param>
    /// <param name="width">The width of the map.</param>
    /// <param name="height">The height of the map.</param>
    /// <returns>The safety value of the current robots state.</returns>
    static int Calculate_Safety_Factor(List<((int x, int y) position, (int x, int y) velocity)> robots, int width, int height) {
        int horizontalSplit = height / 2; // Integer division floor by default in C#
        int verticalSplit = width / 2; // Integer division floor by default in C#
        int q1Count = 0; // top right quadrant
        int q2Count = 0; // top left quadrant
        int q3Count = 0; // bottom left quadrant
        int q4Count = 0; // bottom right quadrant
        foreach(var robot in robots) {
            if (robot.position.x > verticalSplit && robot.position.y < horizontalSplit) {
                q1Count++;
            }
            else if (robot.position.x < verticalSplit && robot.position.y < horizontalSplit) {
                q2Count++;
            }
            else if (robot.position.x < verticalSplit && robot.position.y > horizontalSplit) {
                q3Count++;
            }
            else if (robot.position.x > verticalSplit && robot.position.y > horizontalSplit) {
                q4Count++;
            }
        }
        int safetyFactor = q1Count * q2Count * q3Count * q4Count;
        return safetyFactor;
    }

    /// <summary>
    /// Determines if there is a vertical line of guards/robots of at least the specified minLength in the state.
    /// </summary>
    /// <param name="robots">The current state of the robots.</param>
    /// <param name="minLength">The min length of vertical alignment of guards to look for.</param>
    /// <returns>Whether there is a vertical alignment of the guards of at least the minLength.</returns>
    static bool Contains_Vertical_Line_Of_Guards(List<((int x, int y) position, (int x, int y) velocity)> robots, int minLength) {
        Dictionary<int, List<int>> pos = new Dictionary<int, List<int>>();
        foreach (var robot in robots) {
            if (!pos.ContainsKey(robot.position.x)) {
                pos[robot.position.x] = new List<int>();
            }
            pos[robot.position.x].Add(robot.position.y);
        }
        foreach (var key in pos.Keys) {
            if (pos[key].Count > minLength) {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Prints the map of the current state of the robots.
    /// </summary>
    /// <param name="robots">The current state of the robots.</param>
    /// <param name="width">The width of the map.</param>
    /// <param name="height">The height of the map.</param>
    static void Print_Map(List<((int x, int y) position, (int x, int y) velocity)> robots, int width, int height) {
        for (int i = 0; i < height; i++) {
            for (int j = 0; j < width; j++) {
                char toPrint = '.';
                foreach (var robot in robots) {
                    if (robot.position.x == j && robot.position.y == i) {
                        toPrint = '#';
                    }
                }
                Console.Write(toPrint);
            }
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Finds the christmas tree in the map. This takes some manual effort because whenever a possible configuration is found 
    /// to contain the christmas tree, the user must answer whether there is actually a christmas tree or not. If the user does not 
    /// see one it will continue. Otherwise it will return the number of seconds it took for the robots to form the christmas tree.
    /// </summary>
    /// <param name="robots">The current state of the robots.</param>
    /// <param name="width">The width of the map.</param>
    /// <param name="height">The height of the map.</param>
    /// <returns></returns>
    static int Find_Christmas_Tree(List<((int x, int y) position, (int x, int y) velocity)> robots, int width, int height) {
        bool christmasTreeFound = false;
        int minVerticalLineLength = 20;
        Console.WriteLine(minVerticalLineLength);
        int seconds = 0;
        while (!christmasTreeFound) {
            robots = Simulate_Robots_Moves_Seconds(robots, width, height, 1);
            seconds++;
            if (Contains_Vertical_Line_Of_Guards(robots, minVerticalLineLength)) {
                Print_Map(robots, width, height);
                Console.WriteLine("Is there a Christmas Tree Present ('Y' for yes): ");
                string userInput = Console.ReadLine();
                christmasTreeFound = userInput == "Y";
            }
        }
        return seconds;
    }

    static void Main() {
        const int WIDTH = 101;
        const int HEIGHT = 103;

        // Loading data
        string filePath = "./../Data/day14.txt";
        string[] lines = File.ReadAllLines(filePath);
        List<((int x, int y) position, (int x, int y) velocity)> robots = Parse_Data(lines);
        List<((int x, int y) position, (int x, int y) velocity)> robots2 = Parse_Data(lines);
        robots = Simulate_Robots_Moves_Seconds(robots, WIDTH, HEIGHT, 100);
        int safetyFactor = Calculate_Safety_Factor(robots, WIDTH, HEIGHT);
        int secondsForChristmasTree = Find_Christmas_Tree(robots2, WIDTH, HEIGHT);
        Console.WriteLine($"(Part 1) The safety factor is: {safetyFactor}");
        Console.WriteLine($"(Part 2) The amount of seconds until the robots resemble a christmas tree is: {secondsForChristmasTree}");
    }
}