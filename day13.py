import numpy as np

def organize_machines(machines: list[str]) -> list[dict]:
    """Parses the input file and stores each machine in a dictionary.

    Args:
        machines (list[str]): The list of the string information from the text file for each machine.

    Returns:
        list[dict]: The list of dictionary sorted information for each machine.
    """
    newMachines = []
    for machine in machines:
        newDict = {}
        data = machine.split('\n')
        newDict["A"] = (int(data[0].split("X+")[1].split(",")[0]), int(data[0].split("Y+")[1]))
        newDict["B"] = (int(data[1].split("X+")[1].split(",")[0]), int(data[1].split("Y+")[1]))
        newDict["PRIZE"] = (int(data[2].split("X=")[1].split(",")[0]), int(data[2].split("Y=")[1]))
        newMachines.append(newDict)
    return newMachines

def unit_converstion_correction(machines: list[dict]) -> list[dict]:
    """Converts the regular machine prize location to the new one for part 2.

    Args:
        machines (list[dict]): The list of machines to convert the prize location for.

    Returns:
        list[dict]: The converted list of machines.
    """
    newMachines = []
    for machine in machines:
        newDict = {}
        newDict["A"] = machine["A"]
        newDict["B"] = machine["B"]
        newDict["PRIZE"] = (machine["PRIZE"][0] + 10000000000000, machine["PRIZE"][1] + 10000000000000)
        newMachines.append(newDict)
    return newMachines

def calculate_least_tokens(machine: dict) -> int:
    """Calculates the smallest amount of tokens needed to get to the prize location for a machine.
    It does this using linear algebra with 2 equations solving for 2 variables (A button presses and 
    B button presses).

    Args:
        machine (dict): The machine to do the least token calculation on.

    Returns:
        int: The least amount of tokens to get to the prize location for the machine.
    """
    x1, x2, xt = machine["A"][0], machine["B"][0], machine["PRIZE"][0]
    y1, y2, yt = machine["A"][1], machine["B"][1], machine["PRIZE"][1]

    A = np.array(((x1, x2), (y1, y2)))
    B = np.array((xt, yt))

    aPress, bPress = np.linalg.solve(A, B)
    if np.allclose(aPress, np.round(aPress), 0, 0.001) and np.allclose(bPress, np.round(bPress), 0, 0.001):
        return int((np.round(aPress) * 3) + np.round(bPress))
    return -1

def all_machines_least_tokens(machines: list[dict]) -> int:
    """Gets the least amount of tokens needed for each machine and adds them together to 
    get the total tokens needed to complete every machine.

    Args:
        machines (list[dict]): List of machines to determine the least number of tokens for.

    Returns:
        int: The least number of tokens for all of the machines summed together.
    """
    sum = 0
    for machine in machines:
        leastCost = calculate_least_tokens(machine)
        if leastCost != -1:
            sum += leastCost
    return sum

if __name__ == "__main__":
    machines = []
    machineData = ""
    # Opening the text file and formatting the data
    with open("./Data/day13.txt", "r") as file:
        for line in file:
            if line == '\n':
                machines.append(machineData)
                machineData = ""
            else:
                machineData += line
        machines.append(machineData)
    machines = organize_machines(machines)
    machinesConverted = unit_converstion_correction(machines)
    machinesLeastTokens = all_machines_least_tokens(machines)
    machinesConvertedLeastTokens = all_machines_least_tokens(machinesConverted)

    print(f"(Part 1) Fewest tokens to win all possible prizes: {machinesLeastTokens}")
    print(f"(Part 2) Fewest tokens to win all possible prizes with converted prize: {machinesConvertedLeastTokens}")
