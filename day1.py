def determine_distance(leftList: list[str], rightList: list[str]):
    # Go through each corresponding element least to greatest of each list and compare the distance
    sumDistances = 0
    for i in range(len(leftList)):
        sumDistances += abs(int(leftList[i]) - int(rightList[i])) # Finding the distance between the two numbers
    return sumDistances

def calculate_similarity(leftList: list[str], rightList: list[str]):
    # Go through each element in left list, find the number of occurences in right list, then multiply by the number
    cumulativeSimilarity = 0
    for item in leftList:
        occurences = rightList.count(item) # Get number of occurences of item in leftList
        cumulativeSimilarity += int(item) * occurences
    return cumulativeSimilarity

if __name__ == "__main__":
    # Compile lists into left and right lists
    leftList = []
    rightList = []
    with open("./Data/day1.txt", "r") as file:
        for line in file:
            [left, right] = line.strip().split("   ") # Stripping of white space and separating on the space
            leftList.append(left)
            rightList.append(right)

    # Sort the lists
    leftList.sort()
    rightList.sort()

    # Run the calculations to determine the answers
    sumDistances = determine_distance(leftList, rightList)
    cumulativeSimilarity = calculate_similarity(leftList, rightList)

    # Outputting the answer for part 1
    print(f"(Part 1) Total Distance: {sumDistances}")
    print(f"(Part 2) Total Similarity Score: {cumulativeSimilarity}")
