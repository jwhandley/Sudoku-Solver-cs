# C# Sudoku solver

A C# program to solve lots of Sudoku puzzles very quickly.

It's based on the
[constraint propagation](https://colab.research.google.com/github/norvig/pytudes/blob/main/ipynb/Sudoku.ipynb)
approach from Peter Norvig's Python notebook. This implementation aims to be
nearly as high level and expressive as his Python version, while matching his
optimized Java version in terms of speed.

On my machine (11 core M3 Pro Macbook Pro), it is able to solve 250,000 puzzles
in just over 2s when run in parallel with `Parallel.ForEach`. When I run the
[Java program](https://github.com/norvig/pytudes/blob/main/ipynb/Sudoku.java) on
my machine, it has roughly the same performance

Incidentally, I also have a
[Rust implementation](https://github.com/jwhandley/sudoku-solver) that runs at
about 3x the speed while remaining reasonably high-level.

## C# vs. Rust implementation

The structure and verbosity of the Rust and C# implementations ended up being
pretty similar, and given the speed boost from the Rust version, that would make
it an obvious favorite. However, the Rust version depends on the rayon crate to
get functionality that comes bundled with C# with Parallel.ForEach. For such a
small project a single dependency isn't a huge problem, but I think I ultimately
prefer the "batteries included" approach of C# to Rust's deliberately lean
standard library.
