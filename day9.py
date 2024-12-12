def convert_to_raw(compacted: str) -> list:
    """Converts the compacted filesystem string to the format with id number and "." representing free space.

    Args:
        compacted (str): The compacted string to convert.

    Returns:
        list: The list of the id numbers and free space representing how much space each takes up.
    """
    free = False
    raw = []
    idNum = 0
    for char in compacted:
        char = int(char)
        for _ in range(char):
            if not free:
                raw.append(idNum)
            else:
                raw.append(".")
        if not free:
            idNum += 1
        free = not free
    return raw

def backweight_free_space(raw: list) -> list[str]:
    """Starting from the back moves each individual memory slot to the leftmost available free space.

    Args:
        raw (list): The list of ids and free space representing the filesystem.

    Returns:
        list[str]: The list with the updated memory assignments.
    """
    backIndex = len(raw) - 1
    frontIndex = 0
    while frontIndex < backIndex:
        if raw[frontIndex] == ".":
            while not isinstance(raw[backIndex], int):
                backIndex -= 1
            temp = raw[backIndex]
            raw[backIndex] = raw[frontIndex]
            raw[frontIndex] = temp
        frontIndex += 1
    if isinstance(raw[frontIndex-1], int) and isinstance(raw[backIndex], str) and backIndex < frontIndex:
        temp = raw[backIndex]
        raw[backIndex] = raw[frontIndex-1]
        raw[frontIndex-1] = temp
    return raw

def find_chunk(raw: list, char: int|str, size: int, maxMinIndex: int, from_left=True) -> int:
    """Finds the next available chunk from a particular direction which matches the given char and contains consecutive chars of
    length size.

    Args:
        raw (list): The raw filesystem data to optimize the storage for.
        char (int | str): The int or str to find in the memory.
        size (int): The amount of consecutive char that is being searched for.
        maxMinIndex (int): The maximum or minimum index that should be searched to before saying not found.
        from_left (bool, optional): Search starting from the left. Otherwise starts from the right. Defaults to True.

    Returns:
        int: index in the memory of the first instance of the given char. If not found returns -1.
    """
    count = 0
    index = 0
    if from_left:
        for i in range(maxMinIndex+1):
            if raw[i] == char:
                if count == 0:
                    index = i
                count += 1
            else:
                count = 0
                index = 0
            if count == size:
                return index
    else:
        for i in range((len(raw)-1), maxMinIndex-1, -1):
            if raw[i] == char:
                count += 1
            else:
                count = 0
                index = 0
            if count == size:
                index = i
                return index
    return -1

def move_chunks_left(raw: list) -> list[str]:
    """Moves whole chunks to the left in the filesystem to the leftmost available free space that can accomodate 
    the chunk.

    Args:
        raw (list): The filesystem memory arrangement.

    Returns:
        list[str]: The list with the updated memory assignments
    """
    idNum = max([item for item in raw if item != "."])
    count = raw.count(idNum)
    minIndex = 0
    maxIndex = len(raw)-1
    while idNum > 0:
        startIndex = find_chunk(raw, ".", count, maxIndex)
        if startIndex != -1:
            for j in range(count):
                raw[startIndex + j] = idNum
            remIndex = find_chunk(raw, idNum, count, 0, False)
            for j in range(count):
                raw[remIndex + j] = "."
            maxIndex -= count
            minIndex += count
        idNum -= 1
        count = raw.count(idNum)
    return raw

    

def calculate_checksum(memoryChunks: list) -> int:
    """Calculates the checksum for a given memory arrangement.

    Args:
        memoryChunks (list): The memory arrangement to calculate the checksum for.

    Returns:
        int: The checksum for the given memory arrangement.
    """
    checksum = 0
    for i in range(len(memoryChunks)):
        if not memoryChunks[i] == ".":
            checksum += (i * memoryChunks[i])
    return checksum

if __name__ == "__main__":
    compacted = ""
    # Opening the text file and formatting the data
    with open("./Data/day9.txt", "r") as file:
        for line in file:
            compacted += line.strip()
    raw = convert_to_raw(compacted)
    otherRaw = convert_to_raw(compacted)
    backweightFreeSpace = backweight_free_space(raw)
    chunksFreeSpace = move_chunks_left(otherRaw)
    checksum = calculate_checksum(backweightFreeSpace)
    chunkChecksum = calculate_checksum(chunksFreeSpace)
    print(f"(Part 1) The filesystem checksum is: {checksum}")
    print(f"(Part 2) The chunk filesystem checksum is: {chunkChecksum}")