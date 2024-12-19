using System.Reflection.Emit;
using System.Text.RegularExpressions;

Console.WriteLine("Hello, World!");
var (registers, program) = ParseFile();
// Machine m = new Machine(registers);
// Console.WriteLine(string.Join(",", m.process(program, null)));
part2(registers, program);

void part2(Dictionary<string, int> registers, List<int> program) {

    Parallel.For(0, int.MaxValue, (n, state) => {
    // for (int n = 0; n < int.MaxValue; n++) {
        var registerModified = new Dictionary<string, int>();
        registers.ToList().ForEach(x => registerModified.Add(x.Key, x.Value));
        registerModified["A"] = n;

        Machine m = new Machine(registerModified);
        var result = m.process(program, program.Count);
        if (result.SequenceEqual(program)) {
            Console.WriteLine(n);
            state.Break();
        }
    });
}

(Dictionary<string, int> Registers, List<int> Program) ParseFile() {
    var registers = new Dictionary<string, int>();
    var program = new List<int>();

    foreach (var line in File.ReadLines("input.txt")) {
        if (line.StartsWith("Register")) {
            // Parse register name and value
            var match = Regex.Match(line, @"Register (\w+): (\d+)");
            if (match.Success) {
                registers[match.Groups[1].Value] = int.Parse(match.Groups[2].Value);
            }
        } else if (line.StartsWith("Program:")) {
            // Parse program list
            var match = Regex.Match(line, @"Program: ([\d,]+)");
            if (match.Success) {
                foreach (var num in match.Groups[1].Value.Split(',')) {
                    program.Add(int.Parse(num));
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

    private readonly Dictionary<string, int> registers = new Dictionary<string, int>();

    public Machine(Dictionary<string, int> registers) {
        registers.ToList().ForEach(x => this.registers.Add(x.Key, x.Value));
    }

    public List<int> process(List<int> program, int? maxOutput) {
        var result = new List<int>();
        int outputs = 0;

        for (int n = 0; n < program.Count; n += 2) {
            if (maxOutput.HasValue && maxOutput.Value <= outputs) {
                break; // allow only a max. number of iteration
            }
            OpCode opcode = (OpCode)program[n];
            int operandCode = program[n + 1];
            switch (opcode) {
                case OpCode.jnz:
                    if (registers["A"] != 0) {
                        n = operandCode - 2;
                    }
                    break;
                case OpCode.adv:
                    registers["A"] = (int)(registers["A"] / Math.Pow(2, getOperand(operandCode)));
                    break;
                case OpCode.bdv:
                    registers["B"] = (int)(registers["A"] / Math.Pow(2, getOperand(operandCode)));
                    break;
                case OpCode.cdv:
                    registers["C"] = (int)(registers["A"] / Math.Pow(2, getOperand(operandCode)));
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
                    // Console.WriteLine(getOperand(operandCode) % 8);
                    outputs++;
                    result.Add(getOperand(operandCode) % 8);
                    break;
            }
        }

        return result;
    }

    private int getOperand(int operandCode) {
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