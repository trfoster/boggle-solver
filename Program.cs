using System.Collections.Frozen;
using System.Diagnostics;

internal class Program
{
    private static readonly int[] indexes = [-6, -5, -4, 1, 6, 5, 4, -1];
    private const string alphabet = "abcdefghijklmnopqrstuvwxyz";
    private static readonly int[] scores = [1, 3, 3, 2, 1, 4, 2, 4, 1, 8, 5, 1, 3, 1, 1, 3, 8, 1, 1, 1, 1, 4, 4, 7, 4, 8];
    private static string gridString;
    private const string path = "../../../";
    private static int[] countsConstant;
    private static int[] counts;
    public static int[][] indexesOfLetters;
    public static int doubleWordIndex = -1;
    public static int doubleLetterIndex = -1;
    public static int tripleLetterIndex = -1;
    
    public static void Main(string[] args) {
        //temp grid setup
        gridString = "NQJTAAAKRYBNSXUVRURIIHUEY";
        doubleWordIndex = 10;
        doubleLetterIndex = -1;
        tripleLetterIndex = 5;
        
        countsConstant = GenerateCounts(gridString);
        counts = new int[26];
        Array.Copy(countsConstant, counts, 26);

        indexesOfLetters = GenerateIndexesOfLetters(gridString);
        
        foreach (string word in GetPossibleWords("trimmedWords.txt").OrderByDescending(x => x.Length).Take(5)) {
            Console.WriteLine(word);
        }

        Console.WriteLine();
        List<string> possibleWords = GetPossibleWords("trimmedWords.txt");

        foreach (Word word in ScoreWords(possibleWords).OrderByDescending(w => w.score).Take(20)) {
            Console.WriteLine($"{word.word}, {word.score}");
        }

        Console.WriteLine(GetPossibleWords("trimmedWords.txt").Count);
        
        Console.WriteLine(AreNeighbours(10, 9));

    }
    static int[] GenerateCounts(string grid) {
        int[] counts = new int[26];
        for (int i = 0; i < grid.Length; i++) {
            counts[grid[i] - 65]++;
        }
        return counts;
    }

    static int[][] GenerateIndexesOfLetters(string grid) {
        int[][] indexesOfLetters = new int[25][];
        for (int i = 0; i < 25; i++) {
            indexesOfLetters[i] = grid.IndexOfAll((char)(i + 65), countsConstant);
        }
        return indexesOfLetters;
    }

    static List<string> GetPossibleWords(string fileName) {
        StreamReader reader = new(path + fileName);
        List<string> words = [];
        while (!reader.EndOfStream) {
            string word = reader.ReadLine()!;
            ReadOnlySpan<char> wordSpan = word.AsSpan();
            if (wordSpan.SupersetOfGridCount(counts, countsConstant) == 0) {
                words.Add(word);
            }
        }
        reader.Close();
        return words;
    }

    static List<Word> ScoreWords(List<string> words) {
        List<Word> wordList = [];
        /*Parallel.ForEach(words, word =>
        {

        });*/
        words.Clear();
        //words.Add("AN");
        words.Add("ANKRY");
        foreach (string word in words) {
            
            /*ReadOnlySpan<char> wordSpan = word.AsSpan();
            int[] startingIndexes = gridString.IndexOfAll(wordSpan[0]);
            for (int i = 0; i < startingIndexes.Length; i++) {
                //walk
                int wordIndex = 1;
                int score = 0;
                bool isDoubleScore = false;
                
                
                if (CheckNode()) {
                    if (isDoubleScore) score *= 2;
                    wordList.Add(new Word(word, score));
                }
            }*/
        }
        return wordList;
    }

    static bool AreNeighbours(int index1, int index2) {
        if (index1 == -1) return false;
        int absDifference = index2 - index1;
        if (absDifference < 0) absDifference *= -1;

        if (absDifference != 6 && absDifference != 5 && absDifference != 4 && absDifference != 1) return false;
        
        int index1Row = index1 / 5;
        int index2Row = index2 / 5;
        
        switch (absDifference) {
            case 1:
                return index1Row == index2Row;
            case 6:
            {
                int rowDifference = index1Row - index2Row;
                return rowDifference != 2 && rowDifference != -2;
            }
            default:
                return index1Row != index2Row;
        }
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

    static int GetScore(char letter, int currentIndex) {
        return scores[letter - 65];
    }
}

static class Extensions
{
    public static int SupersetOfGridCount(this ReadOnlySpan<char> subSpan, int[] counts, int[] countsConstant) {
        int count = 0;
        Span<int> countsSpan = counts.AsSpan();
        for (int i = 0; i < subSpan.Length; i++) {
            if (--countsSpan[subSpan[i] - 65] < 0) count++;
        }
        //reset counts array
        ReadOnlySpan<int> countsConstantSpan = countsConstant.AsSpan();
        for (int j = 0; j < 26; j++) {
            countsSpan[j] = countsConstantSpan[j];
        }
        return count;
    }

    public static int[] IndexOfAll(this string grid, char character, int[] countsConstant) {
        int indexCount = countsConstant.AsSpan()[character - 65];
        int[] indexes = new int[indexCount];
        ReadOnlySpan<char> gridSpan = grid.AsSpan();
        for (int i = gridSpan.IndexOf(character); i > -1; i = grid.IndexOf(character, i + 1)) {
            indexes[--indexCount] = i;
        }
        return indexes;
    }
}

internal struct Word
{
    public string word;
    public int score;
    public Word(string word, int score) {
        this.word = word;
        this.score = score;
    }
}