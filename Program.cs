using System.Collections.Frozen;

internal class Program
{
    private static readonly int[] indexes = [-6, -5, -4, 1, 6, 5, 4, -1];
    private const string alphabet = "abcdefghijklmnopqrstuvwxyz";
    private static readonly int[] scores = [1, 3, 3, 2, 1, 4, 2, 4, 1, 8, 5, 1, 3, 1, 1, 3, 10, 1, 1, 1, 1, 4, 4, 8, 4, 10];
    private static string grid;
    private const string path = "../../../";
    
    public static void Main(string[] args) {
        grid = "ZJFIESOJUVIHUAEBCHTUIOSDT";
        ReadOnlySpan<char> gridSpan = grid.AsSpan();

        Console.WriteLine(GetPossibleWords("trimmedWords.txt", gridSpan).Count);

    }

    static List<string> GetPossibleWords(string fileName, ReadOnlySpan<char> gridSpan) {
        StreamReader reader = new(path + fileName);
        List<string> words = [];
        while (!reader.EndOfStream) {
            string word = reader.ReadLine()!;
            ReadOnlySpan<char> wordSpan = word.AsSpan();
            if (!wordSpan.ContainsAnyExcept(gridSpan)) {
                words.Add(word);
            }
        }
        reader.Close();
        return words;
    }

    static void TrimWords(int length, string fileName) {
        StreamReader reader = new(path + fileName);
        List<string> words = [];
        while (!reader.EndOfStream) {
            string word = reader.ReadLine()!;
            if (word.Length > length) words.Add(word);
        }
        reader.Close();
        File.WriteAllLines(path + "trimmedWords.txt", words);
    }

    static int[] GetNeighbours(int index) {
        bool hasAbove = index > 4;
        bool hasRight = (index + 1) % 5 != 0;
        bool hasBelow = index < 20;
        bool hasLeft = index % 5 != 0;

        int length;
        if (index == 0 || index == 4 || index == 20 || index == 24) {
            length = 3;
        }
        else if (!hasAbove || !hasRight || !hasBelow || !hasLeft) {
            length = 5;
        }
        else {
            length = 8;
        }

        int[] neighbours = new int[length];
        int currentIndex = 0;

        if (hasAbove) {
            neighbours[currentIndex] = index - 5;
            currentIndex++;
            if (hasLeft) {
                neighbours[currentIndex] = index - 6;
                currentIndex++;
            }

            if (hasRight) {
                neighbours[currentIndex] = index - 4;
                currentIndex++;
            }
        }

        if (hasRight) {
            neighbours[currentIndex] = index + 1;
            currentIndex++;
        }

        if (hasBelow) {
            neighbours[currentIndex] = index + 5;
            currentIndex++;
            if (hasRight) {
                neighbours[currentIndex] = index + 6;
                currentIndex++;
            }

            if (hasLeft) {
                neighbours[currentIndex] = index + 4;
                currentIndex++;
            }
        }

        if (hasLeft) {
            neighbours[currentIndex] = index - 1;
        }

        return neighbours;
    }

    static int GetScore(char letter) {
        return scores[letter - 65];
    }
}

internal struct Tile {
    public int index;
    public char letter;
}