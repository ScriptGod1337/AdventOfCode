using Common;

part2("input.txt");

void part2(string file) {
    var matrix = Matrix<long>.LoadFile(
        file,
        c => c switch {
            'S' => -2,
            '^' => -1,
            _ => 0
        }
    );
    var start = matrix.FindFirst(-2);
    if (start == null) {
        throw new Exception("Invalid input");
    }

    // set bean start
    matrix[start.X, start.Y] = 1;

    for (var row = start.X + 1; row < matrix.RowCount; row++) {
        for (var col = 0; col < matrix.ColumnCount; col++) {
            var above = matrix[row - 1, col];
            if (above <= 0) {
                continue; // no bean above -> skip
            }

            // increase number of hit bey beans
            var current = matrix[row, col];
            if (current < 0) {
                // split
                matrix[row, col - 1] += above;
                matrix[row, col + 1] += above;
            } else {
                matrix[row, col] += above;
            }
        }

        // using (StreamWriter writer = new StreamWriter(Console.OpenStandardOutput())) {
        //     matrix.Print(writer, x => x + "\t");
        // }
    }

    long sum = 0;
    foreach (var element in matrix.EnumerateInRow(matrix.RowCount - 1)) {
        sum += element;
    }

    Console.WriteLine($"sum {sum}");
}

void part1(string file) {
    var matrix = Matrix<char>.LoadFile(
        file,
        x => x
    );
    var start = matrix.FindFirst('S');
    if (start == null) {
        throw new Exception("Invalid input");
    }

    // set bean start
    matrix[start.X, start.Y] = '|';

    var split = 0;
    for (var row = start.X + 1; row < matrix.RowCount; row++) {
        for (var col = 0; col < matrix.ColumnCount; col++) {
            var above = matrix[row - 1, col];
            if (above != '|') {
                continue; // no bean above -> skip
            }

            var current = matrix[row, col];
            if (current == '^') {
                // split
                matrix[row, col - 1] = '|';
                matrix[row, col + 1] = '|';
                split++;
            } else {
                matrix[row, col] = '|';
            }
        }

        // using (StreamWriter writer = new StreamWriter(Console.OpenStandardOutput())) {
        //     matrix.Print(writer);
        // }
    }

    Console.WriteLine($"split {split}");
}