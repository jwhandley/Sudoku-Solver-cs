using System.Diagnostics;
using Sudoku_Solver;

if (args.Length < 1)
{
    Console.WriteLine("Usage: ./Sudoku-Solver <INPUT>");
    return;
}

List<string> puzzles = File.ReadAllLines(args[0]).ToList();
var sw = Stopwatch.StartNew();

sw.Restart();
Parallel.ForEach(puzzles, (puzzle) =>
{
    using var grid = Grid.Parse(puzzle);


    if (!grid.Search())
    {
        Console.WriteLine("Oops, couldn't solve puzzle!");
        return;
    }

    if (!grid.Validate())
    {
        Console.WriteLine("Oops, solution is not valid!");
    }
});
Console.WriteLine($"Solved puzzles in {sw.ElapsedMilliseconds}ms");
