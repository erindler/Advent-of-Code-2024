def determine_correct(rules: dict[list[int]], sequence: list[int]) -> bool:
    """Determines whether or not the given sequence is in the correct order according to the supplied rules.

    Args:
        rules (dict[list[int]]): The rules of the sequence. 
        sequence (list[int]): The sequence to check.

    Returns:
        bool: Whether or not the sequence is in the correct order.
    """
    prev = []
    for item in sequence:
        for prevItem in prev:
            if item not in rules:
                continue
            if prevItem in rules[item]:
                return False
        prev.append(item)
    return True

def get_correct_indexes(rules: dict[list[int]], sequences: list[list[int]]) -> tuple[list[int], list[int]]:
    """Gets all of the indexes of the list holding the sequences that the sequence is in the correct order.

    Args:
        rules (dict[list[int]]): The rules the sequence needs to follow to be considered in order.
        sequences (list[list[int]]): The list holding all of the sequences to check.

    Returns:
        tuple[list[int], list[int]]: The first entry in the tuple is the list of indexes of the correct and the second entry is that of the incorrect.
    """
    correct = []
    incorrect = []
    for i, sequence in enumerate(sequences):
        if determine_correct(rules, sequence):
            correct.append(i)
        else:
            incorrect.append(i)
    return (correct, incorrect)

def sum_middle_index(sequences: list[list[int]], indexes: list[int]) -> int:
    """Adds together the value at the middle indexes of each of the sequences at the indexes supplied in the indexes parameter.

    Args:
        sequences (list[list[int]]): The list of sequences to calculate on.
        indexes (list[int]): The indexes of sequences to add the middle indexes together.

    Returns:
        int: The sum of all the requested middle indexes.
    """
    sum = 0
    for index in indexes:
        sequence = sequences[index]
        middleIndex = len(sequence) // 2 # Get the middle index by floor dividing by 2
        sum += sequence[middleIndex]
    return sum

def fix_incorrect_sequence(rules: dict[list[int]], sequence: list[int]) -> list[int]:
    """Fixes the given incorrect sequence based on the rules.

    Args:
        rules (dict[list[int]]): The rules the sequence need to follow in terms of ordering.
        sequence (list[int]): The sequence to put into correct order.

    Returns:
        list[int]: The reordered list that satisifies the ordering criteria.
    """
    if determine_correct(rules, sequence):
        return sequence
    prev = []
    for item in sequence:
        for prevItem in prev:
            if item not in rules:
                continue
            if prevItem in rules[item]:
                index = sequence.index(item)
                prevIndex = sequence.index(prevItem)
                sequence.pop(prevIndex)
                sequence.insert(index, prevItem)
        prev.append(item)
    return fix_incorrect_sequence(rules, sequence)
        

def fix_incorrect(rules: dict[list[int]], sequences: list[list[int]], indexes: list[int]) -> list[list[int]]:
    """Fixes all sequences at the indexes specified in sequences to be of the correct ordering.

    Args:
        rules (dict[list[int]]): The ordering rules each sequence needs to follow.
        sequences (list[list[int]]): The list of all of the sequences.
        indexes (list[int]): A list of the indexes of all of the incorrect sequences in sequences.

    Returns:
        list[list[int]]: The sequences list with all of the incorrectly ordered sequences fixed.
    """
    for index in indexes:
        sequence = sequences[index]
        sequences[index] = fix_incorrect_sequence(rules, sequence)
    return sequences

if __name__ == "__main__":
    sequencesActive = False
    rules = {}
    sequences = []
    # Opening the text file and formatting the data
    with open("./Data/day5.txt", "r") as file:
        for line in file:
            if (line == "\n"): # When get to empty line, switch to sequences
                sequencesActive = True
                continue
            if not sequencesActive: # in rules section
                [left, right] = line.split("|")
                left = int(left)
                right = int(right.strip())
                if left in rules:
                    rules[left].append(right)
                else:
                    rules[left] = [right]
            else: # in sequences section
                splittedList = line.strip().split(",")
                for i in range(len(splittedList)):
                    splittedList[i] = int(splittedList[i])
                sequences.append(splittedList)

    (correct, incorrect) = get_correct_indexes(rules, sequences)
    sumCorrectMiddleIndex = sum_middle_index(sequences, correct)
    fixedSequences = fix_incorrect(rules, sequences, incorrect)
    sumFixedIncorrectMiddleIndex = sum_middle_index(sequences, incorrect)
    print(f"(Part 1) The sum of the correct middle indexes: {sumCorrectMiddleIndex}")
    print(f"(Part 2) The sum of the newly corrected middle indexes: {sumFixedIncorrectMiddleIndex}")
            
