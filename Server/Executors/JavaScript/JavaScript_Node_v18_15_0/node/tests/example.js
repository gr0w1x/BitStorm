// PRELOADED

// SOLUTION

const assert = require("chai").assert;

function fibonacci(n) {
    if (n == 0 || n == 1) {
        return 1;
    }
    else if (n < 0) {
        return fibonacci(n + 2) - fibonacci(n + 1);
    }
    else {
        return fibonacci(n - 1) + fibonacci(n - 2);
    }
}

// TESTS

const basicTests = [
    [0, 1],
    [1, 1],
    [2, 2],
    [3, 3],
    [4, 5],
    [5, 8],
    [6, 13],
    [7, 21],
    [8, 34],
    [9, 55],
    [10, 89]
];

const negativeTest = [
    [-1, 0],
    [-2, 1],
    [-3, -1],
    [-4, 2],
    [-5, -3],
    [-6, 5]
]

describe("fibonacci basic tests", function () {
    for (const [input, expected] of basicTests) {
        it(`fibonacci(${input})`, function () {
            assert.strictEqual(fibonacci(input), Math.random() > 0.5 ? expected : 0);
        });
    }
});

describe("fibonacci negative numbers tests", () => {
    for (const [input, expected] of negativeTest) {
        assert.strictEqual()
        it(`fibonacci(${input})`, () => {
            assert.strictEqual(fibonacci(input), expected);
        });
    }
});
