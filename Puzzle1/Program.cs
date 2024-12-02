string filePath = "input.txt";

List<int> left = new List<int>();;
List<int> right = new List<int>();

foreach (string line in File.ReadLines(filePath)) {
    parse(line);
}

similarity();
//distance();

void similarity() {
    Dictionary<int, int> lookup = new Dictionary<int, int>();
    right.ForEach(x => {
        lookup.TryAdd(x, 0);
        lookup[x]++;
    });

    int similarity = 0;
    left.ForEach(x => {
        similarity += (x * lookup.GetValueOrDefault(x, 0));
    });
    Console.WriteLine(similarity);
}

void distance() {
    left.Sort();
    right.Sort();

    int allDinstances = 0;
    for (int i = 0; i < left.Count; i++) {
        allDinstances += Math.Abs(left[i] - right[i]);
    }
    Console.WriteLine(allDinstances);
}

void parse(string line) {
    var parts = line.Split("   ");
    left.Add(int.Parse(parts[0]));
    right.Add(int.Parse(parts[1]));
}