## Core requirements
User inputs a number on the web page, sends it to the server, and the server returns the number expressed in words.

## Approach

1. Validate (in tiers, collecting all errors)
2. Trim the fraction to 3 digits
3. Parse to decimal
4. Round half up to 2 decimal places, then split into dollars and cents
5. Split dollars into groups of 3
6. Convert each group with lookup tables and add its scale word
7. Apply the AND rules
8. Combine dollars and cents

Every scale (thousand, million, etc) follows the same hundreds -> tens -> ones pattern, so splitting into groups of 3 lets one function convert any group, with the scale word looked up by position. The AND rules then only depend on where a group sits, which the loop already knows.

## Solution alternatives
My approach came from working through the problem by hand, so I didn't prototype alternatives. Below I evaluate the main alternatives against it in hindsight.

1. Recursion, eg: `words(n) = words(n/largest scale that fit) + 'scaleName' + words(n % largest scale that fit)`
    - It is the most concise option.
    - However, under the Aus/British convention the "AND" between groups is only added before the final group, and only when there are groups before it. Whether to add it depends on the group's position, which recursion hides.
    - We would need to pass that information down as an extra parameter or flag, whereas a loop over the groups knows it is at the final group for free (`i == 0`).
2. Walking the number digit by digit, mapping each digit to its place (ones, tens, hundreds)
    - Its advantage is that there is no numeric precision limit, if we went with just a pure string approach as the string is never converted into a decimal.
    - However, this means we would need to do rounding purely by hand while my approach uses `Math.Round`.
    - Another thing is the place to value logic gets fiddly. Teens span two digits (`1` then `5` is "FIFTEEN", not "TEN FIVE"), and we would need to look ahead to decide on "AND" and to skip zero groups.
        - This would result in having a while loop in a for loop, while my approach only has one level of looping. Multiple nesting level hurts readability, and we would also need some sort of pointer to peek into the next digit, which is harder to follow while you are already in a loop.
3. An if/else for each scale (`< 20`, `< 100`, `< 1000`, `< 1,000,000`, ...)
    - Not scalable, as every scale repeats the same logic.
    - Adding a new scale means writing a new if/else branch, whereas my approach only requires adding 1 item to the `Scales` mapping list.

## Scale terminologies
1. Thousand
2. Million
3. Billion
4. Trillion
5. Quadrillion
6. Quintillion
7. Sextillion (10^21)

The largest supported amount is 999 sextillion (999,999,999,999,999,999,999,999.99). Supporting larger amounts only requires adding the next scale term to the `Scales` list.

## Assumptions
1. Output is in Australian dollars and cents
2. Output is in uppercase
3. Leading zeros (`007`) and thousands separators (`1,000`) are accepted
4. Amounts are rounded to 2 decimal places, half up (`0.005` → `ONE CENT`, `0.004` → `ZERO DOLLARS`)
5. Australian/British convention, short scale

## Validation
1. Empty or whitespace values
2. Input with positive (+) or negative (-) signs
3. Input with a decimal point that has no digits after it
    - For eg: `5.`
4. More than 1 decimal point
5. Incorrect comma placement before the decimal point. Commas can only be placed after every third digit, from right to left.
6. Commas included after the decimal point
7. Input containing non-numerical values or symbols/special characters, other than commas and dots
8. Input of 1 septillion (1,000,000,000,000,000,000,000,000) or more, after rounding to 2 decimal places

## Design decisions
1. `decimal` over `double`
    - `double` is binary floating point and cannot represent values like 0.1 exactly, which leads to errors in cents. `decimal` is base 10 and exact for this range.
2. Trimming the fraction to 3 digits before parsing
    - My initial approach parsed the full input directly. `decimal` only holds ~28-29 significant digits, so long inputs were rounded during parsing (`999,999,999,999,999,999,999,999.9949999` became `...999.995`), which then rounded up past the maximum and crashed.
    - Rounding half up to 2 decimal places only depends on the 3rd decimal digit, so trimming to 3 digits loses nothing and keeps every accepted input (at most 24 + 3 digits) within `decimal`'s precision.
3. Collecting all validation errors, in tiers
    - My initial approach returned the first error found, which meant the user had to submit several times to see every problem with their input.
    - Checks are now grouped into tiers: empty input stops immediately, format checks all run and report together, and the maximum check only runs once the input is a valid number.
4. Deciding on "ZERO DOLLARS" when combining dollars and cents
    - Initially the whole number conversion returned "ZERO DOLLARS" itself, which produced "ZERO DOLLARS AND FIVE CENTS" for `0.05`.
    - Whether to say zero depends on both parts, so the dollar and cent conversions only handle non zero amounts, and `Convert` decides how to combine them.
5. One `OnesToTeens` list instead of separate `Ones` and `Teens` lists
    - Every number under 20 is expressed as a single word, so a single lookup covers them all.
6. Adding trailing spaces between the words constructed for each group.
    - Initially I was doing it manually in the loop itself that goes over each group, which is troublesome to get right, so I decided to use a list of strings instead to keep all of the words and join them with a space after the loop.

## Development and architecture
TDD - The logic of constructing the words from numbers is important, and is split into
- grouping numbers
- construction of words in each grouping
- conjunction within and between each group

I wanted to solidify the logic before the code was written.

Architecture - Changed from a minimal API (1 GET and 1 POST endpoint) to ASP.NET Core MVC.
- Mainly to avoid writing JavaScript and rely on a server-rendered form
- Built-in safeguards (anti-forgery token)
- Familiarity and maintainability

The solution is split into three projects:
- `NumberToWords.Core` - conversion and validation logic, with no dependency on the web layer
- `NumberToWords.Web` - the MVC controller and view
- `NumberToWords.Tests` - xUnit tests against Core, so the logic is tested without the web layer

CI/CD - Every push to `main` runs a GitHub Actions workflow that builds, runs the unit tests, and deploys to Azure App Service. A failing test blocks the deploy.