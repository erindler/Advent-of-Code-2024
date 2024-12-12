
from concurrent.futures import ThreadPoolExecutor
import threading

from collections import defaultdict
    
def blink(numbers: defaultdict[int]) -> defaultdict[int]:
    """Calculates the new format of the stones after blinking according to the rules specified in AoC.
    For this a defaultdict is being used where the key keeps track of the number and this causes it to
    be memory efficient as well as time efficient. This is because the number of each valued stone is 
    represented by the key with the value as the number of times it appears in the sequence. Thus we 
    know the solution for the calculation for one number will be identical to all of the same numbers.
    This also gives back the new dictionary after completing.

    Args:
        numbers (defaultdict[int]): The initial state of the stones.

    Returns:
        defaultdict[int]: The state of the stones after the blink
    """
    newDict = defaultdict(int)
    for number in numbers:
        if number == 0:
            newDict[1] += numbers[0]
        elif len(str(number)) % 2 == 0:
            # Instead of converting back and forth, do the logic directly
            midIndex = len(str(number)) // 2
            left = number // (10 ** midIndex)
            right = number % (10 ** midIndex)
            newDict[left] += numbers[number]
            newDict[right] += numbers[number]
            #i += 1  # Skip the next element, as it's been inserted already
        else:
            newDict[number * 2024] += numbers[number]
    return newDict
    
def blink_num(numbers: defaultdict[int], numBlinks: int) -> int:
    """Blinks a given number of times.

    Args:
        numbers (defaultdict[int]): The defaultdict which represents the starting state of the stones.
        numBlinks (int): The number of times to blink.

    Returns:
        int: The number of stones in the final state of the stones.
    """
    for i in range(numBlinks):
        numbers = blink(numbers)
    return sum(numbers.values())

# CONSTANTS
NUM_THREADS = 4
P1_NUM_BLINKS = 25
P2_NUM_BLINKS = 75


if __name__ == "__main__":
    initial = ""
    numbers = defaultdict(int)
    # Opening the text file and formatting the data
    with open("./Data/day11.txt", "r") as file:
        for line in file:
            initial += line.strip()
    ogList = initial.split(" ")
    for i in ogList:
        numbers[int(i)] += 1
    print(numbers)
    numStonesP1 = blink_num(numbers, P1_NUM_BLINKS)
    numStonesP2 = blink_num(numbers, P2_NUM_BLINKS)
    print(f"(Part 1) The number of stones after {P1_NUM_BLINKS} blinks is: {numStonesP1}")
    print(f"(Part 2) The number of stones after {P2_NUM_BLINKS} blinks is: {numStonesP2}")
    