// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World2!");

string filePath = "input.txt";

int count = 0;
foreach (string line in File.ReadLines(filePath)) {
    var parts = line.Split(' ').Select(x => int.Parse(x)).ToList();
    if (isSafe2(parts)) {
        count++;
        //Console.WriteLine("safe");
    }
}
Console.WriteLine(count);

bool isSafe2(List<int> elements) {
    if (isSafe(elements)) {
        return true;
    }

    for (int n = 0; n < elements.Count; n++) {
        var smallElements = new List<int>(elements);
        smallElements.RemoveAt(n);
        
        if (isSafe(smallElements)) {
            return true;
        }

    }

    return false;
}

bool isSafe1(List<int> elements) {
    return isSafe(elements);
}

bool isSafe(List<int> elements) {
    int? last = null;
    int? lastChange = null;

    foreach (var current in elements) {
        if (!last.HasValue) {
            last = current;
            continue;
        }

        int currentChange = last.Value - current;
        int absChange = Math.Abs(currentChange);
        
        if (absChange < 1 || absChange > 3) {
            return false;
        } else if (lastChange.HasValue && (Math.Sign(currentChange) != Math.Sign(lastChange.Value))) {
            return false;
        }

        last = current;
        lastChange = currentChange;
    }

    return true;
}