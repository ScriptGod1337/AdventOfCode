using System.Text.RegularExpressions;

var (registers, program) = ParseFile();

// part1(registers, program);
part2(program, 0);

bool part2(List<ulong> program, ulong current) {
    for (var n = 0; n < 8; n++) {
        var nextTry = current + (ulong)n;

        var registerModified = new Dictionary<string, ulong>() {
            { "A",  nextTry},
            { "B",  0 },
            { "C",  0 },
        };
        var m = new Machine(registerModified);
        var procResult = m.process(program);

        if (!program.TakeLast(procResult.Count).SequenceEqual(procResult)) {
            continue;
        }

        if (procResult.Count == program.Count) {
            Console.WriteLine(nextTry);
            return true;
        }

        if (part2(program, nextTry * 8)) {
            return true;
        }
    }

    return false;
}

void part1(Dictionary<string, ulong> registers, List<ulong> program) {
    Machine m = new Machine(registers);
    Console.WriteLine(string.Join(",", m.process(program)));
}

(Dictionary<string, ulong> Registers, List<ulong> Program) ParseFile() {
    var registers = new Dictionary<string, ulong>();
    var program = new List<ulong>();

    foreach (var line in File.ReadLines("input.txt")) {
        if (line.StartsWith("Register")) {
            // Parse register name and value
            var match = Regex.Match(line, @"Register (\w+): (\d+)");
            if (match.Success) {
                registers[match.Groups[1].Value] = ulong.Parse(match.Groups[2].Value);
            }
        } else if (line.StartsWith("Program:")) {
            // Parse program list
            var match = Regex.Match(line, @"Program: ([\d,]+)");
            if (match.Success) {
                foreach (var num in match.Groups[1].Value.Split(',')) {
                    program.Add(ulong.Parse(num));
                }
            }
        }
    }

    return (registers, program);
}

class Machine {

    enum OpCode {
        adv = 0,
        bxl = 1,
        bst = 2,
        jnz = 3,
        bxc = 4,
        out_ = 5,
        bdv = 6,
        cdv = 7,
    };

    private readonly Dictionary<string, ulong> registers = new Dictionary<string, ulong>();

    public Machine(Dictionary<string, ulong> registers) {
        registers.ToList().ForEach(x => this.registers.Add(x.Key, x.Value));
    }

    public List<ulong> process(List<ulong> program) {
        var result = new List<ulong>();
        int outputs = 0;

        for (int n = 0; n < program.Count; n += 2) {
            OpCode opcode = (OpCode)program[n];
            ulong operandCode = program[n + 1];
            switch (opcode) {
                case OpCode.jnz:
                    if (registers["A"] != 0) {
                        n = (int)operandCode - 2;
                    }
                    break;
                case OpCode.adv:
                    registers["A"] = (ulong)(registers["A"] / Math.Pow(2, getOperand(operandCode)));
                    break;
                case OpCode.bdv:
                    registers["B"] = (ulong)(registers["A"] / Math.Pow(2, getOperand(operandCode)));
                    break;
                case OpCode.cdv:
                    registers["C"] = (ulong)(registers["A"] / Math.Pow(2, getOperand(operandCode)));
                    break;
                case OpCode.bxl:
                    registers["B"] = registers["B"] ^ operandCode;
                    break;
                case OpCode.bst:
                    registers["B"] = getOperand(operandCode) % 8;
                    break;
                case OpCode.bxc:
                    registers["B"] = registers["B"] ^ registers["C"];
                    break;
                case OpCode.out_:
                    outputs++;
                    result.Add(getOperand(operandCode) % 8);
                    break;
            }
        }

        return result;
    }

    private ulong getOperand(ulong operandCode) {
        if (0 <= operandCode && operandCode <= 3) {
            return operandCode;
        }

        switch (operandCode) {
            case 4:
                return registers["A"];
            case 5:
                return registers["B"];
            case 6:
                return registers["C"];
        }

        throw new ArgumentException();
    }
}