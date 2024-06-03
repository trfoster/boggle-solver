const string grid = "ahdlshgiehajeuidnzlfkqieu";

int[] indexes = [-6, -5, -4, 1, 6, 5, 4, -1];

int[] neighbours = GetNeighbours(0);
Console.WriteLine("done");

//TrimWords(3, @"Boggle.collinsWords.txt");


void TrimWords(int length, string fileName)
{
    StreamReader reader = new (@"../../../collinsWords.txt");
    List<string> words = [];
    while (!reader.EndOfStream)
    {
        string word = reader.ReadLine()!;
        if (word.Length > length) words.Add(word);
    }
    reader.Close();
    File.WriteAllLines("../../../trimmedWords.txt", words);
}

int[] GetNeighbours(int index)
{
    bool hasAbove = index > 4;
    bool hasRight = (index + 1) % 5 != 0;
    bool hasBelow = index < 20;
    bool hasLeft = index % 5 != 0;
    
    int length;
    if (index == 0 || index == 4 || index == 20 || index == 24)
    {
        length = 3;
    }
    else if (!hasAbove || !hasRight || !hasBelow || !hasLeft)
    {
        length = 5;
    }
    else
    {
        length = 8;
    }
    int[] neighbours = new int[length];
    int currentIndex = 0;
    if (hasAbove && hasLeft)
    {
        neighbours[currentIndex] = index - 6;
        currentIndex++;
    }
    if (hasAbove)
    {
        neighbours[currentIndex] = index - 6;
        currentIndex++;
    }
    if (hasAbove && hasRight)
    {
        neighbours[currentIndex] = index - 4;
        currentIndex++;
    }
    if (hasRight)
    {
        neighbours[currentIndex] = index + 1;
        currentIndex++;
    }
    if (hasRight && hasBelow)
    {
        neighbours[currentIndex] = index + 6;
        currentIndex++;
    }
    if (hasBelow)
    {
        neighbours[currentIndex] = index + 5;
        currentIndex++;
    }
    if (hasBelow && hasLeft)
    {
        neighbours[currentIndex] = index + 4;
        currentIndex++;
    }
    if (hasLeft)
    {
        neighbours[currentIndex] = index - 1;
    }
    return neighbours;
}

struct Tile
{
    public int index;
    public char letter;
}