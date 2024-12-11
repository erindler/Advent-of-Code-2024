import operator
from itertools import product

def evaluate_equation(numbers: list[int], operators: list) -> int:
    """Evaluates the equation given a list of numbers and a list of operators. It will apply
        the each operator[i] to each number[i] and number[i+1]. This evaluates left to right and
        not by normal precedence rules.

    Args:
        numbers (list[int]): The list of numbers to evaluate in the equation.
        operators (list): The list of operators to use in the equation.

    Returns:
        int: The result of the left-to-right evaluation.
    """
    prev = numbers[0]
    opNum = 0
    for i in range(1, len(numbers)):
        prev = operators[opNum](prev, numbers[i])
        opNum += 1
    return prev

def possibly_valid(result: int, equation: list[int], operators: list = [operator.add, operator.mul]) -> bool:
    """Determins if there is a set of operators that can be used on the numbers in the equation
        given the order that will result in the given result.

    Args:
        result (int): The answer to check for.
        equation (list[int]): The list of numbers to check for.
        operators (list, optional): The list of valid operators. Defaults to [operator.add, operator.mul].

    Returns:
        bool: Whether or not the given equation will result in the result given the valid operators.
    """
    opCombs = list(product(operators, repeat=(len(equation) - 1)))
    for ops in opCombs:
        if evaluate_equation(equation, ops) == result:
            return True
    return False

def sum_of_possibly_valid(results: list[int], equations: list[list[int]], part1=True) -> int:
    """Returns the sum of all of the equations that have possibly valid solutions given their result.

    Args:
        results (list[int]): The results to compare to the equations. These match one-to-one with the equations.
        equations (list[list[int]]): The list of equations to compare to the results which match one-to-one with.
        part1 (bool, optional): Whether evaluating for part 1(2 operators) or part2 (3 operators). Defaults to True.

    Returns:
        int: Sum of all of the possibly valid equations.
    """
    sum = 0
    if part1:
        for i in range(len(equations)):
            if possibly_valid(results[i], equations[i]):
                sum += results[i]
    else:
        for i in range(len(equations)):
            if possibly_valid(results[i], equations[i], [operator.add, operator.mul, lambda x,y: int(str(x) + str(y))]):
                sum += results[i]
    return sum



if __name__ == "__main__":
    results = []
    equations = []
    # Opening the text file and formatting the data
    with open("./Data/day7.txt", "r") as file:
        for line in file:
            (result, equation) = line.rstrip("\n").split(": ")
            results.append(int(result))
            equation = equation.split(" ")
            for i in range(len(equation)):
                equation[i] = int(equation[i])
            equations.append(equation)

    posValidSum = sum_of_possibly_valid(results, equations)
    posThreeVarValidSum = sum_of_possibly_valid(results, equations, False)
    print(f"(Part 1) The sum of the possible valid results is: {posValidSum}")
    print(f"(Part 2) The sum of the possible valid results with concatenation is: {posThreeVarValidSum}")