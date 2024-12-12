from concurrent.futures import ThreadPoolExecutor
import threading

def blink(numbers: list[int]) -> list[int]:
    result = []  # Create a new list to store the modified results
    i = 0
    
    while i < len(numbers):
        if numbers[i] == 0:
            result.append(1)
        elif len(str(numbers[i])) % 2 == 0:
            # Instead of converting back and forth, do the logic directly
            num = numbers[i]
            midIndex = len(str(num)) // 2
            left = num // (10 ** midIndex)
            right = num % (10 ** midIndex)
            result.append(left)
            result.append(right)
            #i += 1  # Skip the next element, as it's been inserted already
        else:
            result.append(numbers[i] * 2024)
        
        i += 1

    return result
    
    
def blink_num(numbers: list[int], numBlinks) -> list[int]:
    for i in range(numBlinks):
        print(f"Starting blink {i + 1}")
        if len(numbers) < NUM_THREADS:
            numbers = blink(numbers)
        chunk_size = len(numbers) // NUM_THREADS
        remainder = len(numbers) % NUM_THREADS
        lst = numbers
        splitNumbers = [lst[i * chunk_size + min(i, remainder):(i + 1) * chunk_size + min(i + 1, remainder)] for i in range(NUM_THREADS)]
        with ThreadPoolExecutor(max_workers=8) as executor:
            results = [executor.submit(blink, splitNumbers[i]) for i in range(NUM_THREADS)]
            output = [result.result() for result in results]
            numbers = [item for sublist in output for item in sublist]
    return numbers

NUM_THREADS = 4
P1_NUM_BLINKS = 25
P2_NUM_BLINKS = 75


if __name__ == "__main__":
    initial = ""
    numbers = []
    # Opening the text file and formatting the data
    with open("./Data/day11.txt", "r") as file:
        for line in file:
            initial += line.strip()
    numbers = initial.split(" ")
    for i in range(len(numbers)):
        numbers[i] = int(numbers[i])
    numStonesP1 = len(blink_num(numbers, P1_NUM_BLINKS))
    # numStonesP2 = len(blink_num(numbers, P2_NUM_BLINKS))
    print(f"(Part 1) The number of stones after {P1_NUM_BLINKS} blinks is: {numStonesP1}")
    # print(f"(Part 2) The number of stones after {P2_NUM_BLINKS} blinks is: {numStonesP2}")
    