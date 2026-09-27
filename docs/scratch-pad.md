### Strategy
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
<hr>

### Challenges faced while implementing
1. Adding trailing spaces between the words
    
    Fixed by just pushing words into a list and joining them with spaces

2. Parsing a max sextillion was accurate
`999,999,999,999,999,999,999,999.99`
but when I included a long value after the decimal point, like
`999,999,999,999,999,999,999,999.9949999`
decimal.TryParse was incorrectly parsing it into
`999,999,999,999,999,999,999,999.9950000`.

    I looked into decimal.MaxValue, and I can see that the max it hold is 
    `79,228,162,514,264,337,593,543,950,335`
    which my input exceeds, and that caused my input to be rounded up incorrectly.

    Solution was to just keep 3 digits in the fraction part, as I only need 3 digits to round up the input at the second decimal point.

3. Learn the convention of structuring numbers into words, not going to lie, I had to look this up online as I was not familiar with the logic and terminologies. Mainly looked online to get an idea of the terminologies and rules of structuring the number into word. LIke when to use "AND", dashes between double digit numbers.

    I also had to look up what comes after trillion, and settled on a max of sextillion, but the max is configurable, just need to add the scale term for it.

4. Breaking down the numbers into groups, and walking each group. Seeing that each group was only made up of hundreds -> tens -> ones, learned from challenge 3.

    I knew we had to have some sort of mapping of the terms and number words. Rough thinking at first was, scale list, "Ones" and "Teens", and "Tys", corny I know.

    - `Scales` was one, ten, hundred, thousand, billion, etc.
    - `Ones` was 1 to 10, this represents the 1 to 10
    - `Teens` was 11 to 19
    - `Tys` was the "Twenty", "Thirty", etc.

    I renamed `Tys` to `Tens` as tys just does not make any sense at all.

    As I was working through the strategy in the `solution-design.md` I realised that if the number was under twenty, it was only expressed with one word, and having two lists were redundant, so I combined `Ones` and `Teens` to `OnesToTeens`.

    I also realised that between each group, we had to add the scale words, but not when there's only one group, and hundred and "AND" were just conjuction words used within each group, or else for input "100,100" we would have "one hundred thousand one hundred hundred". So I had to add a filler for the scale list.

    Next thing was to think about how we can extract the numbers from each group. Soon realised the pattern that mod by 100 gives you the remainder, and division by 100 rounded down gives you the front number, so I could use that to determine two things, if there are remainders, which prompts for another round of processing.

    Next problem was determining what to do if there were remainders after modding by 100. At this point, the remainder could only be 1-99, and taking the knowledge that we can express numbers with just 1 word if it is under 20, we can just return the index from `OnesToTeens`. If it is over 20, we need an extra word, and the first word now would only be from the `Tens` + just the ones (1-9) from `OnesToTeens`