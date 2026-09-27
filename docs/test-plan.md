Test plan for number to words solution
Solution design is `/docs/solution-design.md`

## Main areas to test for
1. ✅ Invalid cases are rejected with clear reasons
2. ✅ Aggregation of validation errors where possible to reduce the round trips needed to get the right validation errors per input.
2. ✅ Pluralisation
3. ✅ Dashes between number's words
4. ✅ Input does not exceed max limit (sextillion)
5. ✅ Round ups
6. ✅ Cents are dsiplayed correctly with or without dollar amounts
7. ✅ Commas
7. ✅ Edge cases
8. ✅ General operation
    1. Scaling term applies correctly
    2. Conjunction word "AND" is applied correctly
    3. General extraction of numbers to words via mapping lists

### In depth of what to test
<hr>

#### Invalid cases
1. Null value
2. Input with symbols/special characters
3. More than 1 decimal points
4. Incorrect comma placing on the whole number part, commas can only be placed after every third digit from right to left.
5. Commas exist after decimal points
6. Input contains only numerical values, commas, and dots.
7. Input does not exceed sextillion.

#### Pluralisation
1. Anything other than 1, we return pluralise (cents or dollars)

#### Edge cases
1. 0 returns "ZERO DOLLARS"
2. 0.00 returns "ZERO DOLLARS"
3. trailing zeros are trimmed - 007 returns "SEVEN DOLLARS"
4. testing out large numbers billion onwards returns correct word

#### Dashes
1. Dashes are only added to words less than a hundred

### Commas
1. If a comma is used in the input, then the input must adhere to the rule of commas (comma after every 3rd digit from the left to right)

#### Cents
1. .X format is allowed as long as we have digit(s) after dot. For eg: .1 returns "ONE CENT"

#### Roundings
1. Rounding of only up to 2 decimal points will be done on all inputs
2. We need to ensure number does not exceed maximum (sextillion) after rounding

#### General Operation
1. Conjunction word "AND" is only ever added for 3 scenarios
    - To join cents and dollars
    - Within each group in the dollars, if the value has remainders modding by 100
    - When concatenating each groups together, and the last group is less than 100.
2. Numbers to words are expressed correctly