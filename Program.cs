using System.Diagnostics;

if (args.Length < 1)
{
    Console.WriteLine("Usage: ./Sudoku-Solver <INPUT>");
    return;
}

List<string> puzzles = File.ReadAllLines(args[0]).ToList();
var sw = Stopwatch.StartNew();
Parallel.ForEach(
    source: puzzles,
    localInit: () =>
    {
        var grids = new Grid[81];
        for (int j = 0; j < grids.Length; j++)
            grids[j] = new Grid();
        return grids;
    },
    body: (puzzle, loopState, grids) =>
    {
        var grid = Grid.Parse(puzzle);

        if (!grid.Search(grids))
            Console.WriteLine("Oops, couldn't solve puzzle!");

        if (!grid.Validate())
            Console.WriteLine("Oops, broken solution!");

        return grids;
    },
    localFinally: _ => { /* nothing to do */ }
);

Console.WriteLine($"Solved puzzles in {sw.ElapsedMilliseconds}ms");