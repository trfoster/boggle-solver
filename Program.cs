using System.Collections.Frozen;
using System.Diagnostics;

internal class Program
{
    private static readonly int[] indexes = [-6, -5, -4, 1, 6, 5, 4, -1];
    private const string alphabet = "ABCDEFGHIJKlMNOpQRSTUVWXYZ";
    private static readonly int[] scores = [1, 4, 5, 3, 1, 5, 3, 4, 1, 7, 6, 3, 4, 2, 1, 3, 8, 2, 2, 2, 4, 5, 5, 7, 4, 8];
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
        Console.WriteLine("Enter grid string");
        gridString = Console.ReadLine().ToUpper();
        Console.WriteLine("Enter index of double word tile (-1 for not there)");
        doubleWordIndex = int.Parse(Console.ReadLine());
        Console.WriteLine("Enter index of double letter tile (-1 for not there)");
        doubleLetterIndex = int.Parse(Console.ReadLine());
        Console.WriteLine("Enter index of triple letter tile (-1 for not there)");
        tripleLetterIndex = int.Parse(Console.ReadLine());
        
        countsConstant = GenerateCounts(gridString);
        counts = new int[26];
        Array.Copy(countsConstant, counts, 26);

        indexesOfLetters = GenerateIndexesOfLetters(gridString);

        /*uint mask = 0;
        //set 4
        mask += 1 << 25;
        //set 9
        //mask += 1 << 8;
        //retrive 4
        Console.WriteLine(mask);
        Console.ReadLine();
        return;*/
        List<string> possibleWords = GetPossibleWords("collinsWords.txt", 0);

        Console.WriteLine("valid words: " + ScoreWords(possibleWords, 0).Count);
        foreach (Word word in ScoreWords(possibleWords, 0).OrderByDescending(w => w.score).Take(20)) {
            Console.WriteLine($"{possibleWords[word.wordIndex]}, {word.score}");
        }

        Console.WriteLine("possible word count: " + GetPossibleWords("collinsWords.txt", 0).Count);

        Console.ReadLine();
    }
    static int[] GenerateCounts(string grid) {
        int[] counts = new int[26];
        for (int i = 0; i < grid.Length; i++) {
            counts[grid[i] - 65]++;
        }
        return counts;
    }

    static int[][] GenerateIndexesOfLetters(string grid) {
        int[][] indexesOfLetters = new int[26][];
        for (int i = 0; i < 26; i++) {
            indexesOfLetters[i] = grid.IndexOfAll((char)(i + 65), countsConstant);
        }
        return indexesOfLetters;
    }

    static List<string> GetPossibleWords(string fileName, int swapCount) {
        StreamReader reader = new(path + fileName);
        List<string> words = [];
        while (!reader.EndOfStream) {
            string word = reader.ReadLine()!;
            ReadOnlySpan<char> wordSpan = word.AsSpan();
            if (wordSpan.SupersetOfGridCount(counts, countsConstant) <= swapCount) {
                words.Add(word);
            }
        }
        reader.Close();
        return words;
    }

    static List<Word> ScoreWords(List<string> words, int maxSwaps) {
        List<Word> wordList = [];
        /*Parallel.ForEach(words, word =>
        {

        });*/
        //words.Clear();
        //words.Add("KANBANS");
        
        Path[] paths = new Path[20];
        int emptyPathIndex = 0;
        int emptyPathIndexCopy;
        int[] currentLetterIndexes;
        int[] nextLetterIndexes;

        for (int i = 0; i < words.Count; i++) {
            Word word = ScoreWord(i, maxSwaps);
            emptyPathIndex = 0;
            if (word.score == -1) continue;
            wordList.Add(word);
        }
        return wordList;
        
        
        Word ScoreWord(int wordIndex, int maxSwaps) {
            ReadOnlySpan<char> wordSpan = words[wordIndex].AsSpan();
            currentLetterIndexes = indexesOfLetters[wordSpan[0] - 65];
            nextLetterIndexes = indexesOfLetters[wordSpan[1] - 65];
            for (int i = 0; i < currentLetterIndexes.Length; i++) {
                for (int j = 0; j < nextLetterIndexes.Length; j++) {
                    if (!AreNeighbours(currentLetterIndexes[i], nextLetterIndexes[j])) {
                        
                    }
                    uint mask = (uint)((1 << currentLetterIndexes[i]) + (1 << nextLetterIndexes[j]));
                    Path newPath = new Path(mask, nextLetterIndexes[j]);
                    paths[emptyPathIndex] = newPath;
                    emptyPathIndex++;
                }
            }
            if (emptyPathIndex == 0) return new Word(-1, -1, 0);
            for (int i = 1; i < wordSpan.Length - 1; i++) {
                nextLetterIndexes = indexesOfLetters[wordSpan[i + 1] - 65];
                emptyPathIndexCopy = emptyPathIndex;
                bool isInvalidWord = true;
                for (int j = 0; j < emptyPathIndexCopy; j++) {
                    if (paths[j].visitedCellsMask == 0) continue;
                    //grow path
                    int branchCount = 0;
                    uint tempMaskAddition = 0;
                    int tempCurrentCellIndex = -1;
                    for (int k = 0; k < nextLetterIndexes.Length; k++) {
                        if (!AreNeighbours(paths[j].currentCellIndex, nextLetterIndexes[k])) continue;
                        if ((paths[j].visitedCellsMask & (1 << nextLetterIndexes[k])) != 0) continue;
                        branchCount++;
                        switch (branchCount) {
                            case 1:
                                tempMaskAddition = (uint)(1 << nextLetterIndexes[k]);
                                tempCurrentCellIndex = nextLetterIndexes[k];
                                break;
                            case > 1:
                            {
                                uint newMask = paths[j].visitedCellsMask + (uint)(1 << nextLetterIndexes[k]);
                                Path newPath = new Path(newMask, nextLetterIndexes[k]);
                                paths[emptyPathIndex] = newPath;
                                emptyPathIndex++;
                                break;
                            }
                        }
                    }
                    if (branchCount == 0) {
                        paths[j].visitedCellsMask = 0;
                    }
                    else {
                        isInvalidWord = false;
                        //has to be updated later so new paths made from this one are correct
                        paths[j].visitedCellsMask += tempMaskAddition;
                        paths[j].currentCellIndex = tempCurrentCellIndex;
                    }
                }
                if (isInvalidWord) {
                    
                    return new Word(-1, -1, 0);
                }
            }
            int score = 0;
            for (int i = 0; i < emptyPathIndex; i++) {
                if (paths[i].visitedCellsMask == 0) continue;
                int tempScore = 0;
                for (int j = 0; j < 25; j++) {
                    if ((paths[i].visitedCellsMask & (1 << j)) == 0) continue;
                    tempScore += scores[gridString[j] - 65];
                }
                
                if (doubleLetterIndex > -1 && (paths[i].visitedCellsMask & (1 << doubleLetterIndex)) != 0)
                    tempScore += scores[gridString[doubleLetterIndex] - 65];
                if (tripleLetterIndex > -1 && (paths[i].visitedCellsMask & (1 << tripleLetterIndex)) != 0)
                    tempScore += scores[gridString[tripleLetterIndex] - 65] * 2;
                if (doubleWordIndex > -1 && (paths[i].visitedCellsMask & (1 << doubleWordIndex)) != 0) tempScore *= 2;
                if (wordSpan.Length > 5) tempScore += 10;
                
                if (tempScore > score) score = tempScore;
            }
            return new Word(wordIndex, score, 0);
        }
    }

    private struct Path
    {
        public uint visitedCellsMask;
        public int currentCellIndex;

        public Path(uint visitedCellsMask, int currentCellIndex) {
            this.visitedCellsMask = visitedCellsMask;
            this.currentCellIndex = currentCellIndex;
        }
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
    public int wordIndex;
    public int score;
    public int swaps;
    public Word(int wordIndex, int score, int swaps) {
        this.wordIndex = wordIndex;
        this.score = score;
        this.swaps = swaps;
    }
}