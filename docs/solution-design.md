## Core requirements:
User inputs numbers on web input, sends it to the server, and server returns the words expressing the numbers.

Australia uses British convention, and short scale.

## Scale terminologies
4. Thousand
5. Million
6. Billion
7. Trillion
8. Quadrillion
9. Quintillion
10. Sextillion (10^21)

## Validation
1. No empty number
2. No negatives
3. Valid number
4. Cap on sextillion at the moment, additional scaling would need to update the config of scaling terminologies
5. Decimals will be rounded up to two decimal places

## Development style:
TDD

## Strategy:
> Still need to check step 4 onwards, and look 
### Mapping list

`Scales = [null], [THOUSAND], [MILLION], [BILLION]`

`OnesToTeens = [ZERO], [ONE], [TWO], [THREE], [FOUR], [FIVE], [SIX], [SEVEN], [EIGHT], [NINE], [TEN], [ELEVEN], [TWELVE], [THIRTEEN], [FOURTEEN], [FIFTEEN], [SIXTEEN], [SEVENTEEN], [EIGHTEEN], [NINETEEN]`

`Tys = [null], [null] [TWENTY], [THIRTY], [FORTY], [FIFTY], [SIXTY], [SEVENTY], [EIGHTY], [NINETY]`

1. Upon receiving input, round input up to two decimal places first
2. Check if rounded up number exceeds maximum cap (my intention is to not have caps, but I'll work on a million first and then extend)
2. Numbers are grouped into groups of 3 from the right excluding cents
3. Map scaling terminlogy to each group
    - each group belongs to the `Scales` mapping list
4. Within each group (cents are not included)
    - If total number in group is 0, index from `OneToTeens` and return
    - If total number in group is < 100
        - if at last group and not the first group, Prepend the word "AND"
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
6. After converting groups to words, determine if total number is less than 1 or more, append "s" to dollar, then append word to the output
7. Cents are always two digits, if not, pad it
    - If cents are less than 1, exit
    - Prepend "AND" to the final output
    - At this point, it could only be 1 - 99
        - If number < 20, just index from `OnesToTeens` no need to mod, and return
            - else divide by 10 and round down to get first word (mapped from `Tys` list) + "-", mod 10 to get the last word (mapping to `OnesToTeens` map)
8. determine if cents are more than 1, append "s" to cent, then append word to the output