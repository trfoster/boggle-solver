using System.Collections.Frozen;
using System.Diagnostics;

internal class Program
{
    private static readonly int[] indexes = [-6, -5, -4, 1, 6, 5, 4, -1];
    private const string alphabet = "abcdefghijklmnopqrstuvwxyz";
    private static readonly int[] scores = [1, 3, 3, 2, 1, 4, 2, 4, 1, 8, 5, 1, 3, 1, 1, 3, 8, 1, 1, 1, 1, 4, 4, 7, 4, 8];
    private static string grid;
    private const string path = "../../../";
    public static int[] countsConstant;
    public static int[] counts;
    public static int doubleWordIndex = -1;
    public static int doubleLetterIndex = -1;
    public static int tripleLetterIndex = -1;
    public static int[] gemIndexes;
    
    public static void Main(string[] args) {
        //temp grid setup
        grid = "NQJTAAAKRYBNSXUVRURIIHUEY";
        doubleWordIndex = 10;
        tripleLetterIndex = 5;
        gemIndexes = [1, 4, 5, 7, 11, 14, 17, 18, 21, 23];
        
        countsConstant = GenerateCounts(grid);
        counts = new int[26];
        Array.Copy(countsConstant, counts, 26);
        
        foreach (string word in GetPossibleWords("trimmedWords.txt").OrderByDescending(x => x.Length).Take(10)) {
            Console.WriteLine(word);
        }

        ReadOnlySpan<char> wordSpan = "ABACUS".AsSpan();
        Console.WriteLine(wordSpan.SupersetOfGridCount() == 0);
        //Console.WriteLine(wordSpan.IsSubsetOfGrid());
        //Console.WriteLine(wordSpan.IsSubsetOfGrid());

        Console.WriteLine(GetPossibleWords("trimmedWords.txt").Count);

    }
    static int[] GenerateCounts(string grid) {
        int[] counts = new int[26];
        for (int i = 0; i < grid.Length; i++) {
            counts[grid[i] - 65]++;
        }
        return counts;
    }

    static List<string> GetPossibleWords(string fileName) {
        StreamReader reader = new(path + fileName);
        List<string> words = [];
        while (!reader.EndOfStream) {
            string word = reader.ReadLine()!;
            ReadOnlySpan<char> wordSpan = word.AsSpan();
            if (wordSpan.SupersetOfGridCount() == 0) {
                words.Add(word);
            }
        }
        reader.Close();
        return words;
    }

    static List<Word> ScoreWords(List<string> words) {
        
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

static class Extensions
{
    public static int SupersetOfGridCount(this ReadOnlySpan<char> subSpan) {
        int count = 0;
        int i;
        Span<int> countsSpan = Program.counts.AsSpan();
        for (i = 0; i < subSpan.Length; i++) {
            if (--countsSpan[subSpan[i] - 65] < 0) count++;
        }
        //reset counts array
        ReadOnlySpan<int> countsConstantSpan = Program.countsConstant.AsSpan();
        for (int j = 0; j < 26; j++) {
            countsSpan[j] = countsConstantSpan[j];
        }
        return count;
    }
}

internal struct Word
{
    public string word;
    public int score;
}