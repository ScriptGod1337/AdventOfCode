using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Program {
    static void Main() {
        // Read all non-empty lines from input.txt
        var lines = File.ReadAllLines("input.txt")
                        .Where(l => !string.IsNullOrWhiteSpace(l))
                        .ToList();

        // Split on the first blank line: everything up to it are initial wires, 
        // everything after it are gate definitions.
        // If there's no blank line, adapt logic as needed.
        int dividerIndex = lines.FindIndex(l => l.Trim().Length == 0);
        // But we've removed empty lines above, so if we want a true blank line, 
        // we should not remove them. Instead, let's just find the first line 
        // that doesn't match the pattern for wire definitions.
        // However, if your file truly has a blank line separating sections, 
        // you can re-check your approach. For simplicity, let's assume 
        // the first line that doesn't contain a ':' is the start of gates.

        // If all wire lines contain ":" and no gate lines do, let's do this:
        int idx = 0;
        while (idx < lines.Count && lines[idx].Contains(":")) {
            idx++;
        }

        var initialWireLines = lines.Take(idx).ToList();
        var gateLines = lines.Skip(idx).ToList();

        // 1) Parse initial wires
        var wireValues = new Dictionary<string, int>();
        foreach (var line in initialWireLines) {
            // e.g.: "x00: 1"
            var parts = line.Split(new[] { ':', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string wireName = parts[0];
            int value = int.Parse(parts[1]);
            wireValues[wireName] = value;
        }

        // 2) Parse gates
        var gates = new List<(string op, string in1, string in2, string output)>();
        foreach (var line in gateLines) {
            // e.g.: "x00 AND y00 -> z00"
            var parts = line.Split(' ');
            // structure: [input1, operation, input2, ->, output]
            string input1 = parts[0];
            string operation = parts[1];
            string input2 = parts[2];
            string output = parts[4]; // skip the "->"
            gates.Add((operation, input1, input2, output));
        }

        // 3) Simulate
        bool changed;
        do {
            changed = false;
            foreach (var (op, in1, in2, output) in gates) {
                if (!wireValues.ContainsKey(in1) || !wireValues.ContainsKey(in2))
                    continue;

                int lhs = wireValues[in1];
                int rhs = wireValues[in2];
                int result = 0;

                switch (op) {
                    case "AND": result = (lhs == 1 && rhs == 1) ? 1 : 0; break;
                    case "OR": result = (lhs == 1 || rhs == 1) ? 1 : 0; break;
                    case "XOR": result = (lhs != rhs) ? 1 : 0; break;
                }

                if (!wireValues.ContainsKey(output)) {
                    wireValues[output] = result;
                    changed = true;
                }
            }
        } while (changed);

        // 4) Collect z-wires in ascending numeric order (z00 < z01 < z02 ...)
        var zWires = wireValues.Keys
            .Where(k => k.StartsWith("z"))
            .OrderBy(k => int.Parse(k.Substring(1)))
            .ToList();

        long finalNumber = 0;
        int bitPosition = 0;
        foreach (var zWire in zWires) {
            int bit = wireValues[zWire];
            finalNumber |= ((long)bit << bitPosition);
            bitPosition++;
        }

        Console.WriteLine($"Final decimal result: {finalNumber}");
    }
}