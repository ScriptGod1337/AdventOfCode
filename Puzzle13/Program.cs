using System.Numerics;
using System.Text.RegularExpressions;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var calculators = parseInput();
calc();

void calc() {
    BigInteger sum = 0;
    calculators.ForEach(c => {
        var costs = c.lowCosMatrix();
        sum += costs ?? 0;
    });

    Console.WriteLine("---");
    Console.WriteLine(sum);
}


List<CostCalulator> parseInput() {
    var calculators = new List<CostCalulator>();

    var input = File.ReadAllLines("input.txt").ToList();
    while (input.Count > 0) {
        var line = input.First();
        input.RemoveAt(0);
        if (string.IsNullOrEmpty(line)) {
            continue;
        }

        var buttonA = parseLine(line);
        var buttonB = parseLine(input.First());
        input.RemoveAt(0);

        var endPos = parseLine(input.First());
        endPos += new Position(10000000000000, 10000000000000);
        input.RemoveAt(0);

        calculators.Add(new CostCalulator(endPos, (Button.A, buttonA), (Button.B, buttonB)));
    }

    return calculators;
}

Position parseLine(string input) {
    var match = Regex.Match(input, @"\b\w+(?: \w+)?:\s*X[=+](\d+),\s*Y[=+](\d+)");
    if (match.Success) {
        return new Position(long.Parse(match.Groups[2].Value), long.Parse(match.Groups[1].Value));
    } else {
        return null;
    }
}


enum Button {
    A = 3,
    B = 1,
}


class CostCalulator {

    private Dictionary<Button, Position> buttons;

    private Position endPos;

    private Dictionary<(Position, Button), long> costsCache;

    private long minCosts;

    public CostCalulator(Position endPos, params (Button, Position)[] buttons) {
        this.buttons = new Dictionary<Button, Position>();
        this.endPos = endPos;
        this.costsCache = new Dictionary<(Position, Button), long>();
        foreach (var button in buttons) {
            this.buttons.Add(button.Item1, button.Item2);
        }
    }

    public BigInteger? lowCosMatrix() {
        var dirA = buttons[Button.A];
        var dirB = buttons[Button.B];

        var det = dirA.row * dirB.col - dirB.row * dirA.col;
        if (det == 0) {
            return null;
        }
        var a = (endPos.row * dirB.col - dirB.row * endPos.col) / det;
        var b = (dirA.row * endPos.col - endPos.row * dirA.col) / det;

        var pos = new Position(dirA.row * a + dirB.row * b, dirA.col * a + dirB.col * b);
        if (pos.Equals(endPos)) {
            return a * (int) Button.A + b * (int) Button.B;
        } else {
            return null;
        }

        
    }

    public BigInteger? lowCostsTree() {
        this.minCosts = long.MaxValue;

        foreach (var (k, v) in buttons) {
            lowCosts(new Position(0, 0), k, 0, 1, endPos);
        }
        if (this.minCosts == long.MaxValue) {
            return null;
        }

        Console.WriteLine(this.minCosts);
        return this.minCosts;
    }

    private void lowCosts(Position pos, Button button, long currentCosts, int depth, Position stopAt) {
        var key = (pos, button);
        if (costsCache.ContainsKey(key)) {
            return; // already done
        } else if ((pos.row > stopAt.row) || (pos.col > stopAt.col) || (depth >= 1000)) {
            return; // too far
        }
        costsCache[key] = -1; // unknown

        if (pos.Equals(stopAt)) {
            minCosts = Math.Min(minCosts, currentCosts);
            costsCache[key] = currentCosts; // unknown
            return;
        }
        var newCost = currentCosts + (int)button;

        foreach (var (k, v) in buttons) {
            var nextPos = pos + v;
            lowCosts(nextPos, k, newCost, depth + 1, stopAt);
        }
    }

}


record Position(long row, long col) {
    public static Position operator +(Position p1, Position p2) => new Position(p1.row + p2.row, p1.col + p2.col);
    public static Position operator -(Position p1, Position p2) => new Position(p1.row - p2.row, p1.col - p2.col);
    public static Position operator *(Position p, long factor) => new Position(p.row * factor, p.col * factor);
    public static Position operator *(long factor, Position p) => new Position(p.row * factor, p.col * factor);
}