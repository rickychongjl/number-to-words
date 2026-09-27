## Core requirements:
User inputs numbers on web input, sends it to the server, and server returns the words expressing the numbers.

Australia uses British convention, and short scale.

## Scale terminologies
1. Thousand
2. Million
3. Billion
4. Trillion
5. Quadrillion
6. Quintillion
7. Sextillion (10^21)

## Validation
1. No empty number
2. No negatives
3. Valid number
4. Cap on sextillion at the moment, additional scaling would need to update the config of scaling terminologies
5. Decimals will be rounded up to two decimal places

## Strategy:
> Still need to check step 4 onwards, and look

### Mapping list

`Scales = [null], [THOUSAND], [MILLION], [BILLION], [TRILLION], [QUADRILLION], [QUINTILLION], [SEXTILLION]`

`OnesToTeens = [ZERO], [ONE], [TWO], [THREE], [FOUR], [FIVE], [SIX], [SEVEN], [EIGHT], [NINE], [TEN], [ELEVEN], [TWELVE], [THIRTEEN], [FOURTEEN], [FIFTEEN], [SIXTEEN], [SEVENTEEN], [EIGHTEEN], [NINETEEN]`

`Tys = [null], [null] [TWENTY], [THIRTY], [FORTY], [FIFTY], [SIXTY], [SEVENTY], [EIGHTY], [NINETY]`

1. [DONE] Upon receiving input
    - Perform data validation
    - If it passes all validation, round input up to two decimal places first
    - Check if rounded up number exceeds maximum cap (currently set at sextillion)
2. [DONE] Numbers are grouped into groups of 3 from the right excluding cents
    - we always need the right most 3, modding it by 1000 gives us that, but then we need to also remove those last 3 digits to move on to the next group.
    - dividing by 1000 and rounding down gives us the number without the right most 3 digits
    - Repeat the same steps until we are done with the whole number.
    - Also to remove the decimal part, we can round down input and that will be our whole number part.
3. [DONE] Map scaling terminlogy to each group
    - each group belongs to the `Scales` mapping list
Before we start looping over each group, if there are no groups after we split the numbers into groups, then the number is zero, and we return ZERO DOLLARS
4. [DONE] Within each group (cents are not included)
    - If total number in group is < 100
        - if at last group and its not the only group, Prepend the word "AND"
        - If number < 20, just index from `OnesToTeens` no need to mod, and return
            - (we dont want to return one five for 15, and we can just return the single digit mapping if number is single digit so I decided to join ones to teens mapping list, where it was previously separated)
        - else try mod by 10 and if remainder is <=0, divide by 10, map from `Tys` list, and return
            - else divide by 10 and round down to get first word (mapped from `Tys` list) + "-", mod 10 to get the last word (mapping to `OnesToTeens` map)
    - else
        - mod by 100, if remainder <= 0, we devide by 100 to get the first word, map the number to word using the `OnesToTeens` mapping, finish for this group.
        - if remainder > 0, we append "AND" to the first word
            - At this point, it could only be 1 - 99
            - If number < 20, just index from `OnesToTeens` no need to mod, and return
                - else divide by 10 and round down to get first word (mapped from `Tys` list) + "-", mod 10 to get the last word (mapping to `OnesToTeens` map)
        - add a "HUNDRED" to the word
    - Between each loop, we add the scaling term
6. [DONE] After converting groups to words, determine if total number is less than 1 or more, append "s" to dollar, then append word to the output
7. [DONE] Cents are always two digits, if not, pad it
    - If cents are less than 1, exit
    - Prepend "AND" to the final output
    - At this point, it could only be 1 - 99
        - If number < 20, just index from `OnesToTeens` no need to mod, and return
            - else divide by 10 and round down to get first word (mapped from `Tys` list) + "-", mod 10 to get the last word (mapping to `OnesToTeens` map)
8. [DONE] Determine if cents are more than 1, append "s" to cent, then append word to the output

## Development and architecture:
TDD - Logic of constructing the words from numbers is important and is split into
    - grouping numbers
    - construction of words in each grouping
    - conjunction within and between each group
so i wanted to solidify the logic before code is written

Architecture - Changed from minimal API for 1 get and post endpoint for now to ASP.NET CORE MVC.
    - Mainly to save from writing javascript and rely on server rendered form
    - Built in safeguards
    - Familiarity and maintainability