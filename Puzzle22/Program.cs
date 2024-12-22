var numbers = File.ReadAllLines("input.txt").Select(n => long.Parse(n)).ToList();
var proc = new SecretNumberProcessor();

for (var n = 0; n < numbers.Count; n++) {
    Console.WriteLine(n);
    proc.EvolveSecretNumberMultipleTimes(numbers[n], 2000);
}

Console.WriteLine(proc.GetMax());

class SecretNumberProcessor {

    private readonly Dictionary<int[], long> overallSeqStats = new Dictionary<int[], long>(new ArrayEqualityComparer<int>());

    public Dictionary<int[], long> EvolveSecretNumberMultipleTimes(long secretNumber, int iterations) {
        var seqStats = new Dictionary<int[], long>(new ArrayEqualityComparer<int>());
        var seq = new LimitedList<int>(4);

        var lastDigitLast = secretNumber % 10;
        for (int i = 0; i < iterations; i++) {
            secretNumber = EvolveSecretNumber(secretNumber);
            var currentDigitLast = secretNumber % 10;
            var change = (int)(currentDigitLast - lastDigitLast);
            lastDigitLast = currentDigitLast;

            seq.Add(change);
            if (seq.Count == 4) {
                var key = seq.Items.ToArray();
                if (seqStats.TryAdd(key, 0L)) {
                    seqStats[key] = currentDigitLast;
                }
            }
        }

        // merge result
        foreach (var (k, v) in seqStats) {
            overallSeqStats.TryAdd(k, 0L);
            overallSeqStats[k] += v;
        }

        return seqStats;
    }
    
    public long EvolveSecretNumber(long secretNumber) {
        // Step 1: Multiply by 64, mix, and prune
        long result1 = secretNumber * 64;
        secretNumber ^= result1;
        secretNumber %= 16777216;

        // Step 2: Divide by 32, mix, and prune
        long result2 = secretNumber / 32;
        secretNumber ^= result2;
        secretNumber %= 16777216;

        // Step 3: Multiply by 2048, mix, and prune
        long result3 = secretNumber * 2048;
        secretNumber ^= result3;
        secretNumber %= 16777216;

        return secretNumber;
    }

    public long GetMax() {
        return overallSeqStats.Values.Max();
    }
}

class ArrayEqualityComparer<T> : IEqualityComparer<T[]> {
    public bool Equals(T[] x, T[] y) {
        if (x == null || y == null)
            return x == y;

        return x.SequenceEqual(y);
    }

    public int GetHashCode(T[] obj) {
        if (obj == null)
            return 0;

        // Combine hash codes of all elements
        unchecked {
            int hash = 17;
            foreach (var val in obj)
                hash = hash * 31 + (val == null ? 0 : val.GetHashCode());
            return hash;
        }
    }
}

public class LimitedList<T> {
    private readonly List<T> _list = new List<T>();
    private readonly int _limit;

    public LimitedList(int limit) {
        if (limit <= 0)
            throw new ArgumentOutOfRangeException(nameof(limit), "Limit must be greater than zero.");

        _limit = limit;
    }

    public void Add(T item) {
        if (_list.Count >= _limit) {
            _list.RemoveAt(0); // Remove the first (oldest) element
        }
        _list.Add(item);
    }

    public IReadOnlyList<T> Items => _list.AsReadOnly(); // Expose items as read-only

    public int Count => _list.Count;

    public override string ToString() {
        return string.Join(", ", _list);
    }
}