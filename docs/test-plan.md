## Test plan for number to words solution
Solution design is [here](/docs/solution-design.md)

## Scope
1. ✅ Invalid cases are rejected with clear reasons
2. ✅ Aggregation of validation errors where possible to reduce the round trips needed to get the right validation errors per input.
3. ✅ Pluralisation
4. ✅ Dashes between number's words
5. ✅ Input does not exceed max limit (septillion)
6. ✅ Round ups
7. ✅ Cents are dsiplayed correctly with or without dollar amounts
8. ✅ Commas
9. ✅ Edge cases
10. ✅ General operation
    1. Scaling term applies correctly
    2. Conjunction word "AND" is applied correctly
    3. General extraction of numbers to words via mapping lists

## Approach
Unit tests are in [tests\NumberToWords.Tests\NumberToWordsConverterTests.cs](/tests/NumberToWords.Tests/NumberToWordsConverterTests.cs)

## How to run the tests
Tests can be run locally via `dotnet test` at the repo root level, and also run as part of the CI process. Failed test would block deployment

80 passed unit tests at the moment which covers all of the areas mentioned in Scope

## What each scenario covers, and where sits in unit tests

#### 1. Invalid cases
1. Empty or whitespace values
2. Input with positive (+) or negative (-) signs
3. Input with a decimal point that has no digits after it
    - For eg: `5.`
4. More than 1 decimal point
5. Incorrect comma placement before the decimal point. Commas can only be placed after every third digit, from right to left.
6. Commas included after the decimal point
7. Input containing non-numerical values or symbols/special characters, other than commas and dots
8. Input of 1 septillion (1,000,000,000,000,000,000,000,000) or more, after rounding to 2 decimal places

Covered by 
- `Convert_InvalidInput_ReturnsValidationErrors`

#### 2. Aggregation of validation errors where possible to reduce the round trips needed to get the right validation errors per input.
1. Inputs that contains multiple validation errors should return the list of validation errors in one go

Covered by
- `Convert_InputWithMultipleProblems_ReturnsAllErrors`

#### 3. Pluralisation
1. Anything other than 1, we return pluralise (cents or dollars)

Covered by
- `Convert_Input_ReturnsCorrectPluralisation`

#### 4. Dashes between number's words
1. Dashes are only added to words less than a hundred

Covered by
- `Convert_Input_ReturnsCorrectDashes`

#### 5. Input does not exceed max limit (septillion)
1. Input does not reach septillion which is what comes after sextillion

Covered by
- `Convert_InputExceedsSextillion_ReturnsValidationErrors`
- `Convert_InputWithDecimalsExceeding_ExceedsMax_ReturnsError`
- `Convert_InputWith50Digits_ExceedsMax_ReturnsError`

#### 6. Round ups
1. Input with decimals are half way rounded up to 2 decimals

Covered by
- `Convert_InputWithDecimal_ReturnsCorrectlyRoundedOutput`

#### 7. Cents
1. Inputs with cents are displayed correctly
2. Covers a combination of dollars + cents, just cents, just dollars

Covered by
- `Convert_InputWithCents_ReturnsCorrectOutput`

#### 8. Commas
1. Inputs with valid commas are parsed correctly
2. Inputs with invalid commas are rejected 

Covered by
- Happy path - `Convert_InputWithCommas_ReturnsCorrectOutput`
- Validation paths - `Convert_InvalidInput_ReturnsValidationErrors` + `Convert_InputWithMultipleProblems_ReturnsAllErrors`

#### 9. Edge cases
1. Edge cases like 
- lower/upper bound values
- leading zeros
- large number where decimals round up and does not exceeds max

Covered by
- `Convert_EdgeCasesInput_ReturnsCorrectOutput`

#### 10. General operation

1. Scaling term applies correctly
2. Conjunction word "AND" is applied correctly
    - Joining dollars and cents, within a group, and before a final group under 100
3. General extraction of numbers to words via mapping lists

Covered by
- `Convert_Input_ReturnsCorrectOutput`