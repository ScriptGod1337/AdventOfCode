List<string> lines = new List<string>(File.ReadAllLines("input.txt"));

part1();
part2();

void part2() {
    var value = new ModInt(50);
    var count = 0;
    foreach (var line in lines) {
        char direction = line[0];
        int number = int.Parse(line.Substring(1));

        while (number > 0) {
            var toZero = value.DistanceToZero(value, direction);
            if (toZero > number) {
                // not enough to reach 0
                value = value.rotateValue(number, direction);
                number -= number;
            } else {
                // rotate to 0 and try again
                value = value.rotateValue(toZero, direction);
                number -= toZero;
                count++;
            }
        }
    }

    Console.WriteLine($"{count}");
}

void part1() {
    var value = new ModInt(50);
    var count = 0;
    foreach (var line in lines) {
        char direction = line[0];
        int number = int.Parse(line.Substring(1));
        
        value = value.rotateValue(number, direction);
        if (value.Value == 0) {
            count++;
        }
    }
    Console.WriteLine($"{count}");
}

public readonly struct ModInt {
    public int Value { get; }

    public ModInt(int v) { 
        Value = ((v % 100) + 100) % 100;
    }

    public static ModInt operator +(ModInt a, int b)
        => new ModInt(a.Value + b);

    public static ModInt operator -(ModInt a, int b)
        => new ModInt(a.Value - b);

    public int DistanceForwardToZero()  => (100 - Value) % 100;
    public int DistanceBackwardToZero() => Value;

    public int DistanceToZero(ModInt value, char direction) {
        int toZero;
        switch (direction) {
            case 'L':
                toZero = DistanceForwardToZero();
                break;
            default:
                toZero = DistanceBackwardToZero();
                break;
        }

        if (toZero == 0) {
            toZero = 100;    
        }

        return toZero;
    }
    public ModInt rotateValue(int number, char direction) {
        return this + ('L' == direction ? number : -number);
    }

    public override string ToString() => Value.ToString();
}
