Test plan for number to words solution
Solution design is `/docs/solution-design.md`

Invalid cases
1. input negative number, returns "Invalid number, no negatives allowed"
2. input non-numeric, returns "Invalid input, only input numerical values (decimals allowed and will be automatically rounded up to 2 decimal places)"
3. input numbers and commas not falling on the third digit, returns "Invalid input, commas must fall on every third digit from the right side of the input"

Edge cases
1. input 0 returns zero dollar(s)
2. input 0.1 returns ten cent(s)
3. input .1 returns ten cent(s)
4. input 0.01 returns one cent(s)
5. input .01 returns one cent(s)
6. input 0.99 returns ninety nine cent(s)
7. input 0.994 returns ninety nine cent(s)
8. input 0.995 returns one dollar(s)
9. input 1 0 returns ten dollar(s)
10. input 1 0 . 0 1 returns ten dollar(s) and one cent(s)

Test cases
input 1 returns one dollar
input 10 returns ten dollars
input 15 returns fifteen dollars
input 25 returns twenty five dollars
input 101 returns one hundred and one dollars
input 110 returns one hundred and ten dollars
input 12345 returns twelve thousand three hundred and forty five dollars
input 123,456,789 returns one hundred and twenty three million four hundred and fifty six thousand seven hundred and eighty nine dollars
input 123,456,009 returns one hundred and twenty three million four hundred and fifty six thousand and nine dollars
input 123.01 returns one hundred and twenty three dollars and one cent
input 123.15 returns one hundred and twenty three dollars and fifteen cents
input 123.20 returns one hundred and twenty three dollars and twenty cents
input 123.35 returns one hundred and twenty three dollars and thirty five cents
input 123,456,789.001 returns one hundred and twenty three million four hundred and fifty six thousand seven hundred and eighty nine dollars
input 123,456,789.10 returns one hundred and twenty three million four hundred and fifty six thousand seven hundred and eighty nine dollars and ten cents
input 9,000 returns nine thousand dollars

2. input 12345 returns twelve thousand three hundred and forty five dollars
3. input 40 return forty dollars
4. input 543 return five hundred and forty three dollars
5. input 45 return forty five dollars
6. input 9 returns nine dollars
7. input 19 returns nineteen dollars
8. 