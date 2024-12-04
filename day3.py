import re
def decipher_mul(expression: str) -> list[str]:
    # Takes the corrupted expression and returns the result
    pattern = "mul\(\s*[+-]?\d+\s*,\s*[+-]?\d+\s*\)"
    validMuls = re.findall(pattern, expression)
    return validMuls

def parse_mul(muls: list[str]) -> list[int]:
    # Parses the mul expression to get lists of the two operands as ints
    multiples = []
    for item in muls:
        item = item.replace("mul(", "").replace(")", "")
        multiples.append([int(item.split(",")[0]), int(item.split(",")[1])])
    return multiples

def determine_sum_mul(expression: str) -> int:
    # Given the corrupted expression, calculates the sum of all the multiples
    operandsList = parse_mul(decipher_mul(expression))
    sum = 0
    for operands in operandsList:
        sum += (operands[0] * operands[1])
    return sum

def handle_conditionals(expression: str) -> str:
    # Changes the expression so that all code between don't() and do() is removed. All code from the start to the first don't() is included
    result = ""
    mode = "do"
    while "don't()" in expression:
        if mode == "do":
            split = expression.split("don't()", 1)
            result += split[0]
            expression = split[1]
            mode = "don't"
        else:
            split = expression.split("do()", 1)
            expression = split[1]
            mode = "do"
        if "don't()" not in expression and "do()" in expression:
            split = expression.split("do()", 1)
            result += split[1]
            result.replace("do()", "")
    return result

        


if __name__ == "__main__":
    expressions = ""
    with open("./Data/day3.txt", "r") as file:
        for line in file:
            expressions += line
    conditionalExpression = handle_conditionals(expressions)
    sumMul = determine_sum_mul(expressions)
    conditionalSumMul = determine_sum_mul(conditionalExpression)
    print(f"(Part 1) Sum of multiplications: {sumMul}")
    print(f"(Part 2) Sum of enabled multiplications: {conditionalSumMul}")

    