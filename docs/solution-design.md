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