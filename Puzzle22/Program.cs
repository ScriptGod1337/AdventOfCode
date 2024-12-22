var numbers = File.ReadAllLines("input.txt").Select(n => ulong.Parse(n)).ToList();
// var calculated = numbers.Select(n => EvolveSecretNumberMultipleTimes(n, 2000)).ToList();
ulong sum = 0;
for (var n = 0; n < numbers.Count; n++) {
    sum += EvolveSecretNumberMultipleTimes(numbers[n], 2000);
}

Console.WriteLine(sum);

ulong EvolveSecretNumber(ulong secretNumber)
{
    // Step 1: Multiply by 64, mix, and prune
    ulong result1 = secretNumber * 64;
    secretNumber ^= result1;
    secretNumber %= 16777216;

    // Step 2: Divide by 32, mix, and prune
    ulong result2 = secretNumber / 32;
    secretNumber ^= result2;
    secretNumber %= 16777216;

    // Step 3: Multiply by 2048, mix, and prune
    ulong result3 = secretNumber * 2048;
    secretNumber ^= result3;
    secretNumber %= 16777216;

    return secretNumber;
}

ulong EvolveSecretNumberMultipleTimes(ulong secretNumber, int iterations)
{
    for (int i = 0; i < iterations; i++)
    {
        secretNumber = EvolveSecretNumber(secretNumber);
        // Console.WriteLine(secretNumber);
    }
    return secretNumber;
}