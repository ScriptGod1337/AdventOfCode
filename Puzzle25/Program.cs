// Read file lines (adjust "input.txt" path as needed)
var fileLines = File.ReadAllLines("input.txt");

// Separate into 7-line “blocks,” ignoring blank lines
var blocks = SeparateSchematics(fileLines);

// Parse each block to either a lock or a key
var locks = new List<int[]>();
var keys = new List<int[]>();

foreach (var block in blocks) {
    // block should be exactly 7 lines
    if (IsLock(block))
        locks.Add(ConvertLockToHeights(block));
    else if (IsKey(block))
        keys.Add(ConvertKeyToHeights(block));
    else
        Console.WriteLine("Warning: Unrecognized schematic encountered.");
}

// Count how many (lock, key) pairs fit
int validPairs = 0;
foreach (var lockHeights in locks) {
    foreach (var keyHeights in keys) {
        if (DoTheyFit(lockHeights, keyHeights))
            validPairs++;
    }
}

// Print the result
Console.WriteLine($"Number of valid lock/key pairs: {validPairs}");

/// <summary>
/// Splits all lines in the file into 7-line blocks, using blank lines as separators.
/// Each returned block should have exactly 7 lines.
/// </summary>
static List<string[]> SeparateSchematics(string[] fileLines) {
    var blocks = new List<string[]>();
    var temp = new List<string>();

    foreach (string line in fileLines) {
        // If we reach a blank line, that means we’ve ended one schematic (if any lines collected)
        if (string.IsNullOrWhiteSpace(line)) {
            if (temp.Count == 7)
                blocks.Add(temp.ToArray());
            temp.Clear();
        } else {
            temp.Add(line);
            // If we already have 7 lines, that means we completed one block
            if (temp.Count == 7) {
                blocks.Add(temp.ToArray());
                temp.Clear();
            }
        }
    }
    // In case the last schematic is not followed by a blank line
    if (temp.Count == 7)
        blocks.Add(temp.ToArray());

    return blocks;
}

/// <summary>
/// Check if a 7-line schematic block is a lock (top row all '#' and bottom row all '.').
/// </summary>
static bool IsLock(string[] block) {
    return block[0].All(c => c == '#') &&
           block[6].All(c => c == '.');
}

/// <summary>
/// Check if a 7-line schematic block is a key (top row all '.' and bottom row all '#').
/// </summary>
static bool IsKey(string[] block) {
    return block[0].All(c => c == '.') &&
           block[6].All(c => c == '#');
}

/// <summary>
/// Convert lock's schematic (7 lines) to an array of pin heights (5 columns).
/// Skip row 0 and row 6; for rows 1..5, count continuous '#' from top down.
/// </summary>
static int[] ConvertLockToHeights(string[] diagram) {
    int columns = diagram[0].Length; // typically 5 in the puzzle
    var heights = new int[columns];

    for (int col = 0; col < columns; col++) {
        int height = 0;
        for (int row = 1; row <= 5; row++) {
            if (diagram[row][col] == '#')
                height++;
            else
                break;
        }
        heights[col] = height;
    }
    return heights;
}

/// <summary>
/// Convert key's schematic (7 lines) to an array of pin heights (5 columns).
/// Skip row 0 and row 6; for rows 5..1, count continuous '#' upward.
/// </summary>
static int[] ConvertKeyToHeights(string[] diagram) {
    int columns = diagram[0].Length;
    var heights = new int[columns];

    for (int col = 0; col < columns; col++) {
        int height = 0;
        for (int row = 5; row >= 1; row--) {
            if (diagram[row][col] == '#')
                height++;
            else
                break;
        }
        heights[col] = height;
    }
    return heights;
}

/// <summary>
/// Returns true if for every column, lockHeight + keyHeight <= 5.
/// If the sum > 5 in any column, they overlap.
/// </summary>
static bool DoTheyFit(int[] lockHeights, int[] keyHeights) {
    for (int i = 0; i < lockHeights.Length; i++)
        if (lockHeights[i] + keyHeights[i] > 5)
            return false;
    return true;
}