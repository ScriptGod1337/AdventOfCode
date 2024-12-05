// See https://aka.ms/new-console-template for more information

Console.WriteLine("Hello, World!5");

var lines = File.ReadAllLines("input.txt").ToList();
var dependencies = readDependencies(lines);

int sumCorrect = 0;
int sumIncorrect = 0;
foreach (string line in lines.SkipWhile(x => !string.IsNullOrWhiteSpace(x)).SkipWhile(x => string.IsNullOrWhiteSpace(x))) {
    var pages = line.Split(",").ToList().Select(x => int.Parse(x)).ToList();

    if (checkCorrectPrinting(pages)) {
        System.Diagnostics.Debug.Assert(pages.Count % 2 == 1);
        sumCorrect += pages[pages.Count / 2];
    } else {
        var newPages = fixOrder(pages);
        System.Diagnostics.Debug.Assert(newPages.Count % 2 == 1);
        sumIncorrect += newPages[newPages.Count / 2];
    }
}

Console.WriteLine(sumCorrect);
Console.WriteLine(sumIncorrect);

List<int> fixOrder(List<int> pages) {
    var result = pages;

    var page2Idx = createPage2Index(pages);
    for (int n = 0; n < pages.Count; n++) {
        var validPageDep = dependencies.GetValueOrDefault(pages[n])?.Where(x => page2Idx.ContainsKey(x)).ToList();
        if (validPageDep == null) {
            continue; // no dependencies to fix
        }

        var firstIncorrect = validPageDep.FirstOrDefault(depPage => n > page2Idx[depPage], -1);
        if (firstIncorrect >= 0) {
            var newPages = new List<int>(pages);
            // swap elements
            (newPages[n], newPages[page2Idx[firstIncorrect]]) = (newPages[page2Idx[firstIncorrect]], newPages[n]);
            // try again
            return fixOrder(newPages);
        }
    }

    System.Diagnostics.Debug.Assert(checkCorrectPrinting(pages));
    return result;
}

bool checkCorrectPrinting(IList<int> pages) {
    var page2Idx = createPage2Index(pages);

    for (int n = 0; n < pages.Count; n++) {
        if (!checkCorrectDependencies(pages[n], page2Idx)) {
            return false;
        }
    }

    return true;
}

bool checkCorrectDependencies(int page, Dictionary<int, int> page2Idx) {
    // filter page dependencies but pages in the current row
    var validPageDep = dependencies.GetValueOrDefault(page)?.Where(x => page2Idx.ContainsKey(x)).ToList();
    if (validPageDep == null) {
        return true; // dependent pages
    }

    foreach (var depPage in validPageDep) {
        if (page2Idx[page] > page2Idx[depPage]) {
            return false; // dependent page is before current, error
        }
    }

    return true; // no incorrect dependent pages
}

Dictionary<int, ISet<int>> readDependencies(IList<string> lines) {
    var dependencies = new Dictionary<int, ISet<int>>();
    foreach (string line in lines.TakeWhile(x => !string.IsNullOrWhiteSpace(x))) {
        var parts = line.Split("|");
        var toPrint = int.Parse(parts[0]);
        var mustBePrinted = int.Parse(parts[1]);

        dependencies.TryAdd(toPrint, new HashSet<int>());
        dependencies[toPrint].Add(mustBePrinted);
    }

    return dependencies;
}

Dictionary<int, int> createPage2Index(IList<int> pages) {
    return pages.Select((value, index) => new { value, index }).ToDictionary(pair => pair.value, pair => pair.index);
}