##Core requirements:
User inputs numbers on web input, sends it to the server, and server returns the words expressing the numbers

##Scale terminologies
Ones
Teens
1. Tens
3. Hundred
4. Thousand
5. Million
<!-- 6. Billion
7. Trillion
8. Quadrillion
9. Quintillion
10. Sextillion -->

##Validation
1. No empty number
2. No negatives
3. Valid number
4. Cap on Million at the moment, additional scaling would need to update the config of scaling terminologies
5. Decimals will be rounded up to two decimal places

##Development style:
TDD

##Strategy:
###Context: Australia uses British convention, and short cale.

1. Upon receiving input, round input up to two decimal places first
2. Check if rounded up number exceeds maximum cap (my intention is to not have caps, but I'll work on a million first and then extend)
2. Numbers are grouped into groups of 3 from the right excluding cents
3. Map scaling terminlogy to each group
4. Within each group
    - Do computation
5. Conjunction word "AND"
    - Determine within each group if "AND" is needed
6. Cents are always two digits, if not, pad it
    - Always add "AND" before the cents word