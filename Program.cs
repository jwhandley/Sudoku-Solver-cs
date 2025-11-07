using System.Diagnostics;

if (args.Length < 1)
{
    Console.WriteLine("Usage: ./Sudoku-Solver <INPUT>");
    return;
}

List<string> puzzles = File.ReadAllLines(args[0]).ToList();
var threadCount = 10;
var puzzlesPerThread = puzzles.Count / threadCount;

var sw = Stopwatch.StartNew();
List<Task> tasks = [];
for (int i = 0; i < threadCount; i++)
{

    var end = i == threadCount - 1 ? puzzles.Count : (i + 1) * puzzlesPerThread;
    var partition = puzzles[(i * puzzlesPerThread)..end];

    var t = Task.Run(() =>
    {
        var grids = new Grid[81];
        for (int j = 0; j < 81; j++)
        {
            grids[j] = new Grid();
        }

        foreach (var puzzle in partition)
        {
            var grid = Grid.Parse(puzzle);
            if (!grid.Search(grids))
            {
                Console.WriteLine("Oops, couldn't solve puzzle!");
            }

            if (!grid.Validate())
            {
                Console.WriteLine("Oops, broken solution!");
            }
        }
    });
    tasks.Add(t);
}

Task.WaitAll(tasks);

Console.WriteLine($"Solved puzzles in {sw.ElapsedMilliseconds}ms");