var (patterns, designs) = readInput();

makeDesigns();

void makeDesigns() {
    designs.Sort((x, y) => y.Length.CompareTo(x.Length));
    var prefixSet = new HashSet<string>(patterns);

    long count = 0;
    int step = 0;
    var cache = new Dictionary<string, long>();
    foreach (var design in designs) {
        Console.WriteLine($"{step++}: ${design}");
        long currentCount = 0;
        currentCount = walkTreePart2(design, cache, prefixSet);
        count += currentCount;

        // if (walkTreePart1(design, prefixSet)) {
        //     count++;
        // }
    }

    Console.WriteLine(count);
}

long walkTreePart2(string state, Dictionary<string, long> cache, HashSet<string> prefixSet) {
    long cachedResult;
    if (state.Length == 0) {
        return 1;
    } else if (cache.TryGetValue(state, out cachedResult)) {
        return cachedResult;
    }

    long count = 0;
    foreach (var pattern in prefixSet) {
        if (state.StartsWith(pattern)) {
            var nextState = state.Substring(pattern.Length);
            count += walkTreePart2(nextState, cache, prefixSet);
        }
    }

    cache[state] = count;
    return count;
}

bool walkTreePart1(string state, HashSet<string> prefixSet) {
    if (state.Length == 0) {
        return true;
    }

    foreach (var pattern in prefixSet) {
        if (state.StartsWith(pattern)) {
            var nextState = state.Substring(pattern.Length);
            if (walkTreePart1(nextState, prefixSet)) {
                return true;
            }
        }
    }

    return false;
}

(List<string>, List<String>) readInput() {
    var lines = File.ReadAllLines("input.txt");

    var patterns = lines[0].Split(", ").ToList();
    var designs = lines.Skip(2).ToList();
    return (patterns, designs);
}