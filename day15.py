class WarehouseRobotMap():
    """The object which represents the warehouse robot map.
    """
    def __init__(self, warehouseText: list[str]):
        """The WarehouseRobotMap initializer which takes the input text and turns it
        into the map.

        Args:
            warehouseText (list[str]): The input text.
        """
        self.warehouse = []
        self.robotPos = ()
        for i,line in enumerate(warehouseText):
            row = []
            for j,char in enumerate(line):
                row.append(char)
                if (char == "@"):
                    self.robotPos = (j, i)
            self.warehouse.append(row)

    def __repr__(self) -> str:
        """Created so the map can be printed.

        Returns:
            str: The string to be printed when instance of class is printed.
        """
        retStr = ""
        for row in range(len(self.warehouse)):
            for col in range(len(self.warehouse[row])):
                retStr += self.warehouse[row][col]
            retStr += "\n"
        return retStr
    
    def move_object(self, direction: str, coords: tuple[int, int]):
        """Moves and object on the map. This is meant to be recursive for boxes
        moving into other boxes.

        Args:
            direction (str): The direction the object is moving.
            coords (tuple[int, int]): The coordinates of the object to be moved.
        """
        objectSymbol = self.warehouse[coords[1]][coords[0]]
        def move_object_helper(newCoords: tuple[int, int]):
            """Helps with moving the object based on the direction.

            Args:
                newCoords (tuple[int, int]): The coordinates the object is destined to.
            """
            if self.warehouse[newCoords[1]][newCoords[0]] == ".":
                self.warehouse[coords[1]][coords[0]] = "."
                if objectSymbol == "@":
                    self.robotPos = newCoords
                self.warehouse[newCoords[1]][newCoords[0]] = objectSymbol
            elif self.warehouse[newCoords[1]][newCoords[0]] == "O":
                self.move_object(direction, newCoords)
                if self.warehouse[newCoords[1]][newCoords[0]] != "O":
                    self.move_object(direction, coords)
        match direction:
            case "^":
                newCoords = (coords[0], coords[1]-1)
                move_object_helper(newCoords)
            case ">":
                newCoords = (coords[0]+1, coords[1])
                move_object_helper(newCoords)
            case "v":
                newCoords = (coords[0], coords[1]+1)
                move_object_helper(newCoords)
            case "<":
                newCoords = (coords[0]-1, coords[1])
                move_object_helper(newCoords)
            case "_":
                print("Invalid direction")

    def execute_commands(self, commands: str):
        """Executes the given string of commands.

        Args:
            commands (str): The commands to execute.
        """
        for command in commands:
            self.move_object(command, self.robotPos)

    def box_coordinate_values(self) -> int:
        """Calculates the sum of all of the box GPS coordinate values.

        Returns:
            int: THe value of the sum of all of the box GPS coordinate values.
        """
        sum = 0
        for row in range(len(self.warehouse)):
            for col in range(len(self.warehouse[row])):
                if self.warehouse[row][col] == "O":
                    sum += (100 * row) + col
        return sum
        
class WideWarehouseRobotMap(WarehouseRobotMap):
    """The wide WideWarehouseRobotMap which inherits from the base WarehouseRobotMap.
    """
    def __init__(self, warehouseText: list[str]):
        """The initializer for WideWarehouseRobotMap which uses the input text
        to create the map.

        Args:
            warehouseText (list[str]): The input text used to create the map.
        """
        self.warehouse = []
        self.robotPos = ()
        x = 0
        y = 0
        for line in warehouseText:
            row = []
            for char in line:
                match char:
                    case "O":
                        row.append("[")
                        row.append("]")
                        x += 2
                    case "#":
                        row.append("#")
                        row.append("#")
                        x += 2
                    case ".":
                        row.append(".")
                        row.append(".")
                        x += 2
                    case "@":
                        self.robotPos = (x, y)
                        row.append("@")
                        row.append(".")
                        x += 2
            x = 0
            y += 1 
            self.warehouse.append(row)
        
    def __repr__(self) -> str:
        """Created so the map can be printed.

        Returns:
            str: The string to be printed when instance of class is printed.
        """
        retStr = ""
        for row in range(len(self.warehouse)):
            for col in range(len(self.warehouse[row])):
                retStr += self.warehouse[row][col]
            retStr += "\n"
        return retStr
    
    # ISSUE: WORKS FOR EXAMPLES BUT OFF BY A LITTLE ON THE ACTUAL DAY 15 INPUT TEXT FOR PART 2
    def move_object(self, direction: str, coords: tuple[int, int]):
        """Moves an object in a given direction from the coords.

        Args:
            direction (str): The direction to move the object.
            coords (tuple[int, int]): The starting coordinates of the object.
        """
        objectSymbol = self.warehouse[coords[1]][coords[0]]
        def move_object_helper(newCoords: tuple[int, int]):
            """Helper for the move object function which does the moving.

            Args:
                newCoords (tuple[int, int]): The new coordinates of the object to move to.
            """
            if self.warehouse[newCoords[1]][newCoords[0]] == ".":
                self.warehouse[coords[1]][coords[0]] = "."
                if objectSymbol == "@":
                    self.robotPos = newCoords
                self.warehouse[newCoords[1]][newCoords[0]] = objectSymbol
            elif self.warehouse[newCoords[1]][newCoords[0]] == "[":
                move_wide_object(newCoords, (newCoords[0]+1, newCoords[1]))
                if self.warehouse[newCoords[1]][newCoords[0]] != "[":
                    self.move_object(direction, coords)
            elif self.warehouse[newCoords[1]][newCoords[0]] == "]":
                move_wide_object((newCoords[0]-1, newCoords[1]), newCoords)
                if self.warehouse[newCoords[1]][newCoords[0]] != "]":
                    self.move_object(direction, coords)
        def move_wide_object(leftCoords: tuple[int, int], rightCoords: tuple[int, int]):
            """Used to move a wide object such as a box.

            Args:
                leftCoords (tuple[int, int]): The left coordinates of the wide object.
                rightCoords (tuple[int, int]): The right coordinates of the wide object.
            """
            leftObjectSymbol = self.warehouse[leftCoords[1]][leftCoords[0]]
            rightObjectSymbol = self.warehouse[rightCoords[1]][rightCoords[0]]
            def move_wide_object_helper(newLeftCoords: tuple[int, int], newRightCoords: tuple[int, int]):
                """Helper for the wide object move function which does the moving.

                Args:
                    newLeftCoords (tuple[int, int]): The left coordinates of the wide object.
                    newRightCoords (tuple[int, int]): The right coordinates of the wide object.
                """
                if direction == ">" and self.warehouse[newRightCoords[1]][newRightCoords[0]] == ".":
                    self.warehouse[leftCoords[1]][leftCoords[0]] = "."
                    self.warehouse[rightCoords[1]][rightCoords[0]] = "."

                    self.warehouse[newLeftCoords[1]][newLeftCoords[0]] = leftObjectSymbol
                    self.warehouse[newRightCoords[1]][newRightCoords[0]] = rightObjectSymbol
                elif direction == "<" and self.warehouse[newLeftCoords[1]][newLeftCoords[0]] == ".":
                    self.warehouse[leftCoords[1]][leftCoords[0]] = "."
                    self.warehouse[rightCoords[1]][rightCoords[0]] = "."

                    self.warehouse[newLeftCoords[1]][newLeftCoords[0]] = leftObjectSymbol
                    self.warehouse[newRightCoords[1]][newRightCoords[0]] = rightObjectSymbol
                elif (direction == "^" or direction == "v") and self.warehouse[newRightCoords[1]][newRightCoords[0]] == "." and self.warehouse[newLeftCoords[1]][newLeftCoords[0]] == ".":
                    self.warehouse[leftCoords[1]][leftCoords[0]] = "."
                    self.warehouse[rightCoords[1]][rightCoords[0]] = "."

                    self.warehouse[newLeftCoords[1]][newLeftCoords[0]] = leftObjectSymbol
                    self.warehouse[newRightCoords[1]][newRightCoords[0]] = rightObjectSymbol
                elif direction == ">" and self.warehouse[newRightCoords[1]][newRightCoords[0]] == "[":
                    move_wide_object(newRightCoords, (newRightCoords[0]+1, newRightCoords[1]))
                    if self.warehouse[newRightCoords[1]][newRightCoords[0]] != "[":
                        move_wide_object(leftCoords, rightCoords)
                elif direction == "<" and self.warehouse[newLeftCoords[1]][newLeftCoords[0]] == "]":
                    move_wide_object((newLeftCoords[0]-1, newRightCoords[1]), newLeftCoords)
                    if self.warehouse[newLeftCoords[1]][newLeftCoords[0]] != "]":
                        move_wide_object(leftCoords, rightCoords)
                elif (direction == "^" or direction == "v") and self.warehouse[newRightCoords[1]][newRightCoords[0]] in "[]":
                    if self.warehouse[newRightCoords[1]][newRightCoords[0]] == "[":
                        move_wide_object(newRightCoords, (newRightCoords[0]+1, newRightCoords[1]))
                    else:
                        move_wide_object((newRightCoords[0]-1, newRightCoords[1]), newRightCoords)
                    if self.warehouse[newRightCoords[1]][newRightCoords[0]] not in "[]":
                        move_wide_object(leftCoords, rightCoords)
                elif (direction == "^" or direction == "v") and self.warehouse[newLeftCoords[1]][newLeftCoords[0]] in "[]":
                    if self.warehouse[newLeftCoords[1]][newLeftCoords[0]] == "[":
                        move_wide_object(newLeftCoords, (newLeftCoords[0]+1, newLeftCoords[1]))
                    else:
                        move_wide_object((newLeftCoords[0]-1, newLeftCoords[1]), newLeftCoords)
                    if self.warehouse[newLeftCoords[1]][newLeftCoords[0]] not in "[]":
                        move_wide_object(leftCoords, rightCoords)
            match direction:
                case "^":
                    newLeftCoords = (leftCoords[0], leftCoords[1]-1)
                    newRightCoords = (rightCoords[0], rightCoords[1]-1)
                    move_wide_object_helper(newLeftCoords, newRightCoords)
                case ">":
                    newLeftCoords = (leftCoords[0]+1, leftCoords[1])
                    newRightCoords = (rightCoords[0]+1, rightCoords[1])
                    move_wide_object_helper(newLeftCoords, newRightCoords)
                case "v":
                    newLeftCoords = (leftCoords[0], leftCoords[1]+1)
                    newRightCoords = (rightCoords[0], rightCoords[1]+1)
                    move_wide_object_helper(newLeftCoords, newRightCoords)
                case "<":
                    newLeftCoords = (leftCoords[0]-1, leftCoords[1])
                    newRightCoords = (rightCoords[0]-1, rightCoords[1])
                    move_wide_object_helper(newLeftCoords, newRightCoords)
                case "_":
                    print("Invalid direction")
        match direction:
            case "^":
                newCoords = (coords[0], coords[1]-1)
                move_object_helper(newCoords)
            case ">":
                newCoords = (coords[0]+1, coords[1])
                move_object_helper(newCoords)
            case "v":
                newCoords = (coords[0], coords[1]+1)
                move_object_helper(newCoords)
            case "<":
                newCoords = (coords[0]-1, coords[1])
                move_object_helper(newCoords)
            case "_":
                print("Invalid direction")

    def box_coordinate_values(self) -> int:
        """Calculates the sum of the GPS coordinate values of the boxes.

        Returns:
            int: The sum of the GPS coordinate values of the boxes.
        """
        sum = 0
        for row in range(len(self.warehouse)):
            for col in range(len(self.warehouse[row])):
                if self.warehouse[row][col] == "[":
                    sum += ((100 * row) + col)
        return sum

if __name__ == "__main__":
    warehouseData = []
    moves = ""
    warehouseActive = True
    with open("./Data/day15.txt", "r") as file:
        for line in file:
            if line == "\n":
                warehouseActive = False
                continue
            if warehouseActive:
                warehouseData.append(line.strip())
            else:
                moves += line.strip()
    warehouse = WarehouseRobotMap(warehouseData)
    wideWarehouse = WideWarehouseRobotMap(warehouseData)
    warehouse.execute_commands(moves)
    wideWarehouse.execute_commands(moves)
    gpsSum = warehouse.box_coordinate_values()
    wideGPSSum = wideWarehouse.box_coordinate_values()
    print(f"(Part 1) The GPS coordinate sum of all the boxes is: {gpsSum}")
    print(f"(Part 2) The GPS coordinate sum of all the boxes in the wide warehouse is: {wideGPSSum}") # NOTE: Correct answer for part 2 for day15 prompt is 1521635. This implementation is currently not equipped for it but it does work for the examples.
